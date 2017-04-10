using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

using Nichevo.ObjectServer.Schema;
using Nichevo.ObjectServer.DataAdapter;

namespace Nichevo.ObjectServer.Queries
{
	public sealed class Query : IQueryBuilder
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("Query", String.Empty);
		private string order;
		private ConditionBase[] conditions;
		private ArrayList joinSchemas;
		private IDataContext context;
		private Type type;
		private ParameterCollection parameters;
		private int top;

		public Query(params ConditionBase[] conditions)
		{
			this.conditions = conditions;
			joinSchemas = new ArrayList();
			parameters = new ParameterCollection();
			top = 0;
			order = String.Empty;
		}

		public int Top
		{
			get
			{
				return top;
			}
			set
			{
				if(value < 0)
					throw new ArgumentOutOfRangeException("value", value, "Top should be 0 (unlimited) or greater");

				top = value;
			}
		}

		public string Order
		{
			get
			{
				return order;
			}
			set
			{
				order = value;
			}
		}

		ParameterCollection IQueryBuilder.Parameters
		{
			get
			{
				return parameters;
			}
		}

		void IQueryBuilder.AddParentJoin(ParentSchema joinSchema)
		{
			if(!joinSchemas.Contains(joinSchema))
				joinSchemas.Add(joinSchema);
		}

		IDataContext IQueryBuilder.Context
		{
			get
			{
				return context;
			}
		}

		Type IQueryBuilder.QueriedType
		{
			get
			{
				return type;
			}
		}

		internal IDbCommand CreateCommand(IDataContext context, Type type)
		{
			this.context = context;
			this.type = type;

			parameters.Clear();
			joinSchemas.Clear();

			string topClause = top != 0 ? "TOP " + top : String.Empty;

			TypeSchema schema = SchemaCache.Current.GetSchema(type);
			IDbCommand cmd = context.Connection.CreateCommand();

			string table = String.Format(CultureInfo.CurrentCulture, context.TableFormat, schema.TableName);

			StringBuilder whereClause = new StringBuilder();

			foreach(ConditionBase condition in conditions)
			{
				whereClause.AppendFormat("({0}) AND ", condition.BuildQuery(this));
			}

			if(whereClause.Length > 0)
			{
				whereClause.Insert(0, "WHERE ");
				whereClause.Remove(whereClause.Length - 5, 5);
			}

			string ordering = order;

			if(ordering.Length == 0)
				ordering = schema.DefaultOrder;

			ordering = ParseOrder(ordering);

			StringBuilder fromClause = new StringBuilder();

			foreach(ParentSchema parentSchema in joinSchemas)
			{
				foreach(ChildrenSchema childSchema in SchemaCache.Current.GetSchema(parentSchema.Property.PropertyType).ChildrenSchemas)
				{
					if(childSchema.PropertyName == parentSchema.Property.Name && childSchema.ChildType == parentSchema.Schema.Type)
					{
						string parentTable = String.Format(CultureInfo.CurrentCulture, context.TableFormat, childSchema.Schema.TableName);
						string parentColumn = String.Format(CultureInfo.CurrentCulture, context.ColumnFormat, childSchema.Schema.PrimaryKey.ColumnName);
						string childTable = String.Format(CultureInfo.CurrentCulture, context.TableFormat, parentSchema.Schema.TableName);
						string childColumn = String.Format(CultureInfo.CurrentCulture, context.ColumnFormat, parentSchema.ColumnName);
						
						fromClause.AppendFormat("INNER JOIN {0} ON {1}.{2} = {0}.{3}", parentTable, childTable, childColumn, parentColumn);
						break;
					}
				}
			}
			
			cmd.CommandText = String.Format(CultureInfo.CurrentCulture, "SELECT {0} {1}.* FROM {1} {2} {3} ORDER BY {4}", topClause, table, fromClause, whereClause, ordering);

			foreach(Parameter param in ((IQueryBuilder)this).Parameters)
			{
				IDataParameter iParam = cmd.CreateParameter();
				iParam.ParameterName = param.Name;
				iParam.Value = param.Value;

				cmd.Parameters.Add(iParam);
			}

			Trace.WriteLineIf(DebugOutput.Enabled, cmd.CommandText);

			foreach(IDataParameter parameter in cmd.Parameters)
			{
				Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "{0}: {1}", parameter.ParameterName, parameter.Value));
			}

			return cmd;
		}

		private string ParseOrder(string ordering)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Building Ordering query for '" + ordering + "'");

			TypeSchema schema = SchemaCache.Current.GetSchema(type);
			StringBuilder query = new StringBuilder(ordering);

			string table = String.Format(CultureInfo.CurrentCulture, context.TableFormat, schema.TableName);

			Regex expression = new Regex(@"[A-Za-z_.]+");

			foreach(Match match in expression.Matches(ordering))
			{
				if(match.Value.ToLower(CultureInfo.CurrentCulture)  == "asc" || match.Value.ToLower(CultureInfo.CurrentCulture) == "desc")
					continue;

				string column = ConditionBase.DetermineColumnName(this, match.Value);

				query.Replace(match.Value, column);
			}

			return query.ToString();
		}
	}
}
