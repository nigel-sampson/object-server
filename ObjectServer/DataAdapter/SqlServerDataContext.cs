using System;
using System.Data;
using System.Data.SqlClient;

namespace Nichevo.ObjectServer.DataAdapter
{
	internal class SqlServerDataContext : IDataContext
	{
		private SqlConnection conn;

		public SqlServerDataContext(string connectionString)
		{
			conn = new SqlConnection(connectionString);
		}

		public IDbConnection Connection
		{
			get
			{
				return conn;
			}
		}

		public string TableFormat
		{
			get
			{
				return "[dbo].[{0}]";
			}
		}

		public string ColumnFormat
		{
			get
			{
				return "[{0}]";
			}
		}

		public string TableColumnFormat
		{
			get
			{
				return "{0}.{1}";
			}
		}

		public string ParameterFormat
		{
			get
			{
				return "@{0}";
			}
		}

		public string Separator
		{
			get
			{
				return "; ";
			}
		}

		public string IdentitySelect
		{
			get
			{
				//return "SELECT SCOPE_IDENTITY()";
				return "SELECT @@IDENTITY";
			}
		}
	}
}
