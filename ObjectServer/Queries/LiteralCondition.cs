using System;
using System.Globalization;

using Nichevo.ObjectServer.Schema;

namespace Nichevo.ObjectServer.Queries
{
	public class LiteralCondition : ConditionBase
	{
		private string text;
		private string propertyName;

		public LiteralCondition(string propertyName, string text)
		{
			this.propertyName = propertyName;
			this.text = text;
		}

		internal override string BuildQuery(IQueryBuilder query)
		{
			string column = DetermineColumnName(query, propertyName);

			return String.Format(CultureInfo.CurrentCulture, "{0} {1}", column, text);
		}
	}
}
