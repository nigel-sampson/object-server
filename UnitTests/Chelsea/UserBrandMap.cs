using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("UserBrandMaps", "Id", PrimaryKeyType.Identity)]
	public abstract class UserBrandMap : ServerObject
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

		[Parent("brand")]
		public abstract Brand Brand
		{
			get;
			set;
		}
	}
}
