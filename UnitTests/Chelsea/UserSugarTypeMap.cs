using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("UserTypeMaps", "Id", PrimaryKeyType.Identity)]
	public abstract class UserSugarTypeMap : ServerObject
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

		[Parent("type")]
		public abstract SugarType Type
		{
			get;
			set;
		}
	}
}
