using System;
using System.Data;
using System.Text;
using System.Diagnostics;
using System.Globalization;

using Nichevo.ObjectServer.Schema;

namespace Nichevo.ObjectServer.DataAdapter
{
	internal class CommandFactory
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("CommandFactory", String.Empty);

		private IDataContext context;

		public CommandFactory(IDataContext context)
		{
			this.context = context;
		} 

		public IDbCommand Select(Type type, object key)
		{
			TypeSchema schema = SchemaCache.Current.GetSchema(type);
			IDbCommand cmd = context.Connection.CreateCommand();

			string table = String.Format(CultureInfo.CurrentCulture, context.TableFormat, schema.TableName);
			string keyColumn = String.Format(CultureInfo.CurrentCulture, context.ColumnFormat, schema.PrimaryKey.ColumnName);
			string paramName = String.Format(CultureInfo.CurrentCulture, context.ParameterFormat, schema.PrimaryKey.Property.Name);

			cmd.CommandText = String.Format(CultureInfo.CurrentCulture, "SELECT {0}.* FROM {0} WHERE {1}={2}", table, keyColumn, paramName);

			IDataParameter param = cmd.CreateParameter();

			param.ParameterName = paramName;
			param.Value = key;

			cmd.Parameters.Add(param);

			Debug.WriteLineIf(DebugOutput.Enabled, cmd.CommandText);

			foreach(IDataParameter parameter in cmd.Parameters)
			{
				Debug.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "{0}: {1}", parameter.ParameterName, parameter.Value));
			}

			return cmd;
		}

		private object DetermineActualValue(ServerObject obj, PropertySchema propertySchema)
		{
			object val = obj.Data.GetValue(propertySchema.Property.Name);

			if(val == null)
			{
				if(propertySchema.CanBeNull)
					val = DBNull.Value;
				else
					throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} has no value", obj.ServerObjectType.FullName, propertySchema.Property.Name));
			}
			else
			{
				if(propertySchema.CanBeNull && val.Equals(propertySchema.NullValue))
					val = DBNull.Value;
			}

			return val;
		}

		private object DetermineParentValue(ServerObject obj, ParentSchema parentSchema)
		{
			object val = obj.Data.GetValue(parentSchema.Property.Name);

			if(Convert.IsDBNull(val) || val == null)
			{
				if(parentSchema.CanBeNull)
					val = DBNull.Value;
				else
					throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} has no value", obj.ServerObjectType.FullName, parentSchema.Property.Name));
			}

			return val;
		}

		public IDbCommand Insert(ServerObject obj)
		{
			TypeSchema schema = SchemaCache.Current.GetSchema(obj.ServerObjectType);
			IDbCommand cmd = context.Connection.CreateCommand();

			string table = String.Format(CultureInfo.CurrentCulture, context.TableFormat, schema.TableName);

			StringBuilder columns = new StringBuilder();
			StringBuilder values = new StringBuilder();

			foreach(PropertySchema propertySchema in schema.PropertySchemas)
			{
				if(schema.KeyType == PrimaryKeyType.Identity && propertySchema == schema.PrimaryKey)
					continue;

				string column = String.Format(CultureInfo.CurrentCulture, context.ColumnFormat, propertySchema.ColumnName);
				string paramName = String.Format(CultureInfo.CurrentCulture, context.ParameterFormat, propertySchema.Property.Name);

				columns.AppendFormat("{0}, ", column);
				values.AppendFormat("{0}, ", paramName);

				IDataParameter param = cmd.CreateParameter();
				param.ParameterName = paramName;

				param.Value = obj.Data.GetValue(propertySchema.Property.Name);
				param.Value = DetermineActualValue(obj, propertySchema);

				cmd.Parameters.Add(param);
			}

			foreach(ParentSchema parentSchema in schema.ParentSchemas)
			{
				string column = String.Format(CultureInfo.CurrentCulture, context.ColumnFormat, parentSchema.ColumnName);
				string paramName = String.Format(CultureInfo.CurrentCulture, context.ParameterFormat, parentSchema.Property.Name);

				columns.AppendFormat("{0}, ", column);
				values.AppendFormat("{0}, ", paramName);

				IDataParameter param = cmd.CreateParameter();
				param.ParameterName = paramName;
				param.Value = DetermineParentValue(obj, parentSchema);

				cmd.Parameters.Add(param);
			}

			columns.Remove(columns.Length - 2, 2);
			values.Remove(values.Length - 2, 2);

			string identity = String.Empty;

			cmd.CommandText = String.Format(CultureInfo.CurrentCulture, "INSERT INTO {0} ({1}) VALUES ({2}){3}", table, columns, values, identity);

			Debug.WriteLineIf(DebugOutput.Enabled, cmd.CommandText);

			foreach(IDataParameter parameter in cmd.Parameters)
			{
				Debug.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "{0}: {1}", parameter.ParameterName, parameter.Value));
			}
	
			return cmd;
		}

		public IDbCommand Update(ServerObject obj)
		{
			TypeSchema schema = SchemaCache.Current.GetSchema(obj.ServerObjectType);
			IDbCommand cmd = context.Connection.CreateCommand();

			string table = String.Format(CultureInfo.CurrentCulture, context.TableFormat, schema.TableName);

			StringBuilder values = new StringBuilder();

			foreach(PropertySchema propertySchema in schema.PropertySchemas)
			{
				if(propertySchema == schema.PrimaryKey)
					continue;

				string column = String.Format(CultureInfo.CurrentCulture, context.ColumnFormat, propertySchema.ColumnName);
				string paramName = String.Format(CultureInfo.CurrentCulture, context.ParameterFormat, propertySchema.Property.Name);

				values.AppendFormat("{0}={1}, ", column, paramName);

				IDataParameter param = cmd.CreateParameter();
				param.ParameterName = paramName;

				param.Value = obj.Data.GetValue(propertySchema.Property.Name);
				param.Value = DetermineActualValue(obj, propertySchema);

				cmd.Parameters.Add(param);
			}

			foreach(ParentSchema parentSchema in schema.ParentSchemas)
			{
				string column = String.Format(CultureInfo.CurrentCulture, context.ColumnFormat, parentSchema.ColumnName);
				string paramName = String.Format(CultureInfo.CurrentCulture, context.ParameterFormat, parentSchema.Property.Name);

				values.AppendFormat("{0}={1}, ", column, paramName);

				IDataParameter param = cmd.CreateParameter();
				param.ParameterName = paramName;
				param.Value = DetermineParentValue(obj, parentSchema);

				cmd.Parameters.Add(param);
			}

			values.Remove(values.Length - 2, 2);

			string pkColumn = String.Format(CultureInfo.CurrentCulture, context.ColumnFormat, schema.PrimaryKey.ColumnName);
			string pkParamName = String.Format(CultureInfo.CurrentCulture, context.ParameterFormat, schema.PrimaryKey.Property.Name);

			IDataParameter pkParam = cmd.CreateParameter();
			pkParam.ParameterName = pkParamName;

			pkParam.Value = obj.Data.GetValue(schema.PrimaryKey.Property.Name);
			pkParam.Value = DetermineActualValue(obj, schema.PrimaryKey);

			cmd.Parameters.Add(pkParam);

			cmd.CommandText = String.Format(CultureInfo.CurrentCulture, "UPDATE {0} SET {1} WHERE {2}={3}", table, values, pkColumn, pkParamName);;

			Debug.WriteLineIf(DebugOutput.Enabled, cmd.CommandText);

			foreach(IDataParameter parameter in cmd.Parameters)
			{
				Debug.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "{0}: {1}", parameter.ParameterName, parameter.Value));
			}

			return cmd;
		}

		public IDbCommand Delete(ServerObject obj)
		{
			TypeSchema schema = SchemaCache.Current.GetSchema(obj.ServerObjectType);
			IDbCommand cmd = context.Connection.CreateCommand();

			string table = String.Format(CultureInfo.CurrentCulture, context.TableFormat, schema.TableName);

			string pkColumn = String.Format(CultureInfo.CurrentCulture, context.ColumnFormat, schema.PrimaryKey.ColumnName);
			string pkParamName = String.Format(CultureInfo.CurrentCulture, context.ParameterFormat, schema.PrimaryKey.Property.Name);

			IDataParameter pkParam = cmd.CreateParameter();
			pkParam.ParameterName = pkParamName;

			pkParam.Value = obj.Data.GetValue(schema.PrimaryKey.Property.Name);
			pkParam.Value = DetermineActualValue(obj, schema.PrimaryKey);

			cmd.Parameters.Add(pkParam);

			cmd.CommandText = String.Format(CultureInfo.CurrentCulture, "DELETE FROM {0} WHERE {1}={2}", table, pkColumn, pkParam);

			Debug.WriteLineIf(DebugOutput.Enabled, cmd.CommandText);

			foreach(IDataParameter parameter in cmd.Parameters)
			{
				Debug.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "{0}: {1}", parameter.ParameterName, parameter.Value));
			}

			return cmd;
		}
	}
}
