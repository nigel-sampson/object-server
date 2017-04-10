using System;

namespace Nichevo.ObjectServer.Queries
{
	internal class Parameter
	{
		private string name;
		private object parameterValue;

		public Parameter(string name)
		{
			this.name = name;
			parameterValue = DBNull.Value;
		}

		public Parameter(string name, object parameterValue)
		{
			this.name = name;
			this.parameterValue = parameterValue;
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				name = value;
			}
		}

		public object Value
		{
			get
			{
				return parameterValue;
			}
			set
			{
				parameterValue = value;
			}
		}
	}
}
