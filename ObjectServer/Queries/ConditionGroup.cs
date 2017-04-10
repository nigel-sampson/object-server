using System;
using System.Text;

namespace Nichevo.ObjectServer.Queries
{
	public class ConditionGroup : ConditionBase
	{
		private ConditionGroupType groupType;
		private ConditionBase[] conditions;

		public ConditionGroup(ConditionGroupType groupType, params ConditionBase[] conditions)
		{
			this.groupType = groupType;
			this.conditions = conditions;
		}

		internal override string BuildQuery(IQueryBuilder query)
		{
			StringBuilder conditionTxt = new StringBuilder();

			string op = groupType == ConditionGroupType.And ? "AND" : "OR";

			conditionTxt.Append("(");
			foreach(ConditionBase condition in conditions)
			{
				conditionTxt.AppendFormat("({0}) {1} ", condition.BuildQuery(query), op);
			}
			if(conditionTxt.Length > 0)
				conditionTxt.Remove(conditionTxt.Length - 2 - op.Length, 2 + op.Length);
			conditionTxt.Append(")");
			
			return conditionTxt.ToString();
		}
	}
}
