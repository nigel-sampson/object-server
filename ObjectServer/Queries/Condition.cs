using System;
using System.Text;
using System.Globalization;

using Nichevo.ObjectServer.DataAdapter;

namespace Nichevo.ObjectServer.Queries
{
	public class Condition : ConditionBase
	{
		private string propertyName;
		private Expression exp;
		private object[] values;

		public Condition(string propertyName, Expression exp)
		{
			this.exp = exp;
			this.propertyName = propertyName;

			switch(exp)
			{
				case Expression.IsNull:
				case Expression.IsNotNull:
					break;
				default:
					throw new ObjectServerException("Invalid amount of parameters for operator " + exp);
			};
		}

		public Condition(string propertyName, Expression exp, object conditionValue)
		{
			this.exp = exp;
			this.propertyName = propertyName;
			values = new object[]{conditionValue};

			switch(exp)
			{
				case Expression.Equal:
				case Expression.GreaterThan:
				case Expression.GreaterThanOrEqualTo:
				case Expression.LessThan:
				case Expression.LessThanOrEqualTo:
				case Expression.NotEqual:
				case Expression.Like:
				case Expression.NotLike:
				case Expression.In:
				case Expression.NotIn:
					break;
				default:
					throw new ObjectServerException("Invalid amount of parameters for operator " + exp);
			};
		}

		public Condition(string propertyName, Expression exp, object conditionValue1, object conditionValue2)
		{
			this.exp = exp;
			this.propertyName = propertyName;
			values = new object[]{conditionValue1, conditionValue2};

			switch(exp)
			{
				case Expression.In:
				case Expression.NotIn:
				case Expression.Between:
				case Expression.NotBetween:
					break;
				default:
					throw new ObjectServerException("Invalid amount of parameters for operator " + exp);
			};
		}

		public Condition(string propertyName, Expression exp, params object[] conditionValues)
		{
			this.exp = exp;
			this.propertyName = propertyName;
			this.values = conditionValues;

			switch(exp)
			{
				case Expression.In:
				case Expression.NotIn:
					break;
				default:
					throw new ObjectServerException("Invalid amount of parameters for operator " + exp);
			};
		}

		internal override string BuildQuery(IQueryBuilder query)
		{
			string column = DetermineColumnName(query, propertyName);
			string op = String.Empty;

			switch(exp)
			{
				case Expression.Equal:
					op = "=";
					break;
				case Expression.NotEqual:
					op = "<>";
					break;
				case Expression.GreaterThan:
					op = ">";
					break;
				case Expression.GreaterThanOrEqualTo:
					op = ">=";
					break;
				case Expression.LessThan:
					op = "<";
					break;
				case Expression.LessThanOrEqualTo:
					op = "<=";
					break;
				case Expression.Like:
					op = "LIKE";
					break;
				case Expression.NotLike:
					op = "NOT LIKE";
					break;
			};

			switch(exp)
			{
				case Expression.Equal:
				case Expression.NotEqual:
				case Expression.GreaterThan:
				case Expression.GreaterThanOrEqualTo:
				case Expression.LessThan:
				case Expression.LessThanOrEqualTo:
				case Expression.Like:
				case Expression.NotLike:
					string param = query.Parameters.GenerateName();
					query.Parameters.Add(param, values[0]);
					return String.Format(CultureInfo.CurrentCulture, "{0} {2} {1}", column, param, op);
				case Expression.IsNull:
					return String.Format(CultureInfo.CurrentCulture, "{0} IS NULL", column);
				case Expression.IsNotNull:
					return String.Format(CultureInfo.CurrentCulture, "{0} IS NOT NULL", column);
				case Expression.Between:
					string betParam1 = query.Parameters.GenerateName();
					query.Parameters.Add(betParam1, values[0]);
					string betParam2 = query.Parameters.GenerateName();
					query.Parameters.Add(betParam2, values[1]);
					return String.Format(CultureInfo.CurrentCulture, "{0} BETWEEN {1} AND {2}", column, betParam1, betParam2);
				case Expression.NotBetween:
					string nbetParam1 = query.Parameters.GenerateName();
					query.Parameters.Add(nbetParam1, values[0]);
					string nbetParam2 = query.Parameters.GenerateName();
					query.Parameters.Add(nbetParam2, values[1]);
					return String.Format(CultureInfo.CurrentCulture, "{0} NOT BETWEEN {1} AND {2}", column, nbetParam1, nbetParam2);
				case Expression.In:
					StringBuilder inClause = new StringBuilder();

					foreach(object val in values)
					{
						string inParam = query.Parameters.GenerateName();
						query.Parameters.Add(inParam, val);
						inClause.AppendFormat("{0}, ", inParam);
					}
					if(inClause.Length > 0)
						inClause.Remove(inClause.Length - 2, 2);

					return String.Format(CultureInfo.CurrentCulture, "{0} IN ({1})", column, inClause);
			};

			return String.Empty;
		}
	}
}
