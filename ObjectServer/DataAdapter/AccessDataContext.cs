using System;
using System.Data;
using System.Data.OleDb;

namespace Nichevo.ObjectServer.DataAdapter
{
	internal class AccessDataContext : IDataContext
	{
		private OleDbConnection conn;

		public AccessDataContext(string connectionString)
		{
			conn = new OleDbConnection(connectionString);
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
				return "[{0}]";
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
				return "?";
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
				return "SELECT @@IDENTITY";
			}
		}
	}
}