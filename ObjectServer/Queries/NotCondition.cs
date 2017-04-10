using System;
using System.Text;

namespace Nichevo.ObjectServer.Queries
{
	public class NotCondition : ConditionBase
	{
		private ConditionBase[] conditions;

		public NotCondition(params ConditionBase[] conditions)
		{
			this.conditions = conditions;
		}

		internal override string BuildQuery(IQueryBuilder query)
		{
			StringBuilder conditionTxt = new StringBuilder();

			conditionTxt.Append("NOT (");
			foreach(ConditionBase condition in conditions)
			{
				conditionTxt.AppendFormat("({0}) AND ", condition.BuildQuery(query));
			}
			if(conditionTxt.Length > 0)
				conditionTxt.Remove(conditionTxt.Length - 5, 5);
			conditionTxt.Append(")");
			
			return conditionTxt.ToString();
		}
	}
}
