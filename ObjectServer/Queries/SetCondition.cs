using System;
using System.Text;
using System.Collections;
using System.Globalization;
using Nichevo.ObjectServer.Schema;
using Nichevo.ObjectServer.DataAdapter;

namespace Nichevo.ObjectServer.Queries
{
	public class SetCondition : ConditionBase, IQueryBuilder
	{
		private string column;
		private ConditionBase[] conditions;
		private ArrayList joinSchemas;
		private IDataContext context;
		private Type type;
		private ParameterCollection parameters;

		public SetCondition(string column, params ConditionBase[] conditions)
		{
			this.column = column;
			this.conditions = conditions;
			joinSchemas = new ArrayList();
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

		ParameterCollection IQueryBuilder.Parameters
		{
			get
			{
				return parameters;
			}
		}

		internal override string BuildQuery(IQueryBuilder query)
		{
			// Sample set query would be an exists
			// WHERE <primary key> IN ( SELECT <foreign key> FROM <child tables> WHERE <defined conditions> )
			// Issues are that SetCondition operates in a similar way to the Query class, it has to have its own 
			// series of ParentJoins as it has a complete internal query.
			// Paramater collection should be the same, as parameters are shared.
			// Foreign key should contribute to outer query joins
			// Primary key should contribute inner query joins

			Type childType = DetermineChildType(query, column);
			TypeSchema schema = SchemaCache.Current.GetSchema(childType);

			this.context = query.Context;
			this.type = query.QueriedType;
			this.parameters = query.Parameters;

			string table = String.Format(CultureInfo.CurrentCulture, context.TableFormat, schema.TableName);

			string primaryKey = DeterminePrimaryKey(query, column);
			string foreignKey = DetermineForeginKey(this, column);

			this.type = childType;

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

			return String.Format(CultureInfo.CurrentCulture, "{0} IN ( SELECT {1} FROM {2} {3} {4} )", primaryKey, foreignKey, table, fromClause, whereClause);
		}
	}
}
