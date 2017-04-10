using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Brands", "Id", PrimaryKeyType.Identity)]
	public abstract class Brand : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Column("name")]
		public abstract string Name
		{
			get;
			set;
		}

		[Children(typeof(UserBrandMap), "Brand")]
		public abstract ServerObjectCollection UserMaps
		{
			get;
		}
	}
}
