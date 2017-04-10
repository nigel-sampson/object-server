using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;

using Nichevo.ObjectServer.Schema;

namespace Nichevo.ObjectServer.DataAdapter
{
	internal class ObjectFactory
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("ObjectFactory", String.Empty);

		private ObjectFactory()
		{			
		}

		public static ServerObject ToObject(Type type, IDataReader reader)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Creating object of type " + type.FullName);

			TypeSchema schema = SchemaCache.Current.GetSchema(type);
			ServerObject obj = Activator.CreateInstance(schema.Proxy) as ServerObject;

			if(!reader.Read())
			{
				reader.Close();
				return null;
			}

			foreach(PropertySchema propertySchema in schema.PropertySchemas)
			{
				if(Convert.IsDBNull(reader[propertySchema.ColumnName]))
				{
					if(propertySchema.CanBeNull)
					{
						Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Setting {0} to NullValue", propertySchema.Property.Name));
						obj.Data.SetValue(propertySchema.Property.Name, propertySchema.NullValue);
					}
					else
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} has a null value from the database and CanBeNull is false", obj.ServerObjectType.FullName, propertySchema.Property.Name));
				}
				else
				{
					Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Setting {0} to {1}", propertySchema.Property.Name, reader[propertySchema.ColumnName]));
					obj.Data.SetValue(propertySchema.Property.Name, reader[propertySchema.ColumnName]);
				}
			}

			foreach(ParentSchema parentSchema in schema.ParentSchemas)
			{
				if(Convert.IsDBNull(reader[parentSchema.ColumnName]))
				{
					if(parentSchema.CanBeNull)
					{
						Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Setting {0} to DBNull.Value", parentSchema.Property.Name));
						obj.Data.SetValue(parentSchema.Property.Name, DBNull.Value);
					}
					else
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} has a null value from the database and CanBeNull is false", obj.ServerObjectType.FullName, parentSchema.Property.Name));
				}
				else
				{
					Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Setting {0} to {1}", parentSchema.Property.Name, reader[parentSchema.ColumnName]));
					obj.Data.SetValue(parentSchema.Property.Name, reader[parentSchema.ColumnName]);
				}
			}

			reader.Close();

			return obj;
		}

		public static ServerObjectCollection ToCollection(Type type, IDataReader reader)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Creating Collection");
			ServerObjectCollection collection = new ServerObjectCollection();
			TypeSchema schema = SchemaCache.Current.GetSchema(type);

			while(reader.Read())
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Creating object of type " + type.FullName);
				ServerObject obj = Activator.CreateInstance(schema.Proxy) as ServerObject;

				foreach(PropertySchema propertySchema in schema.PropertySchemas)
				{
					if(Convert.IsDBNull(reader[propertySchema.ColumnName]))
					{
						if(propertySchema.CanBeNull)
						{
							Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Setting {0} to NullValue", propertySchema.Property.Name));
							obj.Data.SetValue(propertySchema.Property.Name, propertySchema.NullValue);
						}
						else
							throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} has a null value from the database and CanBeNull is false", obj.ServerObjectType.FullName, propertySchema.Property.Name));
					}
					else
					{
						Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Setting {0} to {1}", propertySchema.Property.Name, reader[propertySchema.ColumnName]));
						obj.Data.SetValue(propertySchema.Property.Name, reader[propertySchema.ColumnName]);
					}
				}

				foreach(ParentSchema parentSchema in schema.ParentSchemas)
				{
					if(Convert.IsDBNull(reader[parentSchema.ColumnName]))
					{
						if(parentSchema.CanBeNull)
						{
							Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Setting {0} to DBNull.Value", parentSchema.Property.Name));
							obj.Data.SetValue(parentSchema.Property.Name, DBNull.Value);
						}
						else
							throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} has a null value from the database and CanBeNull is false", obj.ServerObjectType.FullName, parentSchema.Property.Name));
					}
					else
					{
						Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Setting {0} to {1}", parentSchema.Property.Name, reader[parentSchema.ColumnName]));
						obj.Data.SetValue(parentSchema.Property.Name, reader[parentSchema.ColumnName]);
					}
				}


				collection.Add(obj);
			}

			reader.Close();

			return collection;
		}
	}
}
