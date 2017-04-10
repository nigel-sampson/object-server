using System;
using System.Data;

namespace Nichevo.ObjectServer.DataAdapter
{
	internal interface IDataContext
	{
		IDbConnection Connection
		{
			get;
		}

		string TableFormat
		{
			get;
		}

		string ColumnFormat
		{
			get;
		}

		string TableColumnFormat
		{
			get;
		}

		string ParameterFormat
		{
			get;
		}

		string Separator
		{
			get;
		}

		string IdentitySelect
		{
			get;
		}
	}
}
