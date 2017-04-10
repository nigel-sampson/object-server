using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("UserUseMaps", "Id", PrimaryKeyType.Identity)]
	public abstract class UserSugerUseMap : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("userid")]
		public abstract User User
		{
			get;
			set;
		}

		[Parent("useid")]
		public abstract SugarUse Use
		{
			get;
			set;
		}
	}
}
