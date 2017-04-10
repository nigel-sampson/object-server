using System;
using System.Data.SqlClient;

namespace UnitTests
{
	internal class DataUtil
	{
		private DataUtil()
		{
			
		}

		public static bool IsRowNull(string table, string keyColumn, object key)
		{
			SqlConnection conn = new SqlConnection(Constants.ConnectionString);
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = String.Format("SELECT * FROM {0} WHERE {1} = @{1}", table, keyColumn);
			cmd.Parameters.Add("@" + keyColumn, key);

			try
			{
				conn.Open();

				SqlDataReader reader = cmd.ExecuteReader();

				if(!reader.Read())
					return false;

				for(int i = 0; i < reader.FieldCount; i++)
				{
					if(reader.GetName(i) == keyColumn)
						continue;

					if(reader[i] != DBNull.Value)
						return false;
				}
			}
			finally
			{
				conn.Close();
			}

			return true;
		}

		public static int CountRows(string table)
		{
			SqlConnection conn = new SqlConnection(Constants.ConnectionString);
			SqlCommand cmd = conn.CreateCommand();
			cmd.CommandText = String.Format("SELECT COUNT(*) FROM {0}", table);

			int count = 0;

			try
			{
				conn.Open();

				count = Convert.ToInt32(cmd.ExecuteScalar());
			}
			finally
			{
				conn.Close();
			}

			return count;
		}
	}
}
