using System;

namespace UnitTests
{
	internal class Constants
	{
		public const string ConnectionString = @"Pooling=false; Integrated Security=SSPI; Persist Security Info=False; Initial Catalog=ObjectServer; Data Source=Azrael;";
		public const string ChelseaConnectionString = @"Pooling=false; Integrated Security=SSPI; Persist Security Info=False; Initial Catalog=Chelsea; Data Source=Azrael;";

		private Constants()
		{
			
		}
	}
}
