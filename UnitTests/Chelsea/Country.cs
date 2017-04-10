using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Countries", "Id", PrimaryKeyType.Identity)]
	public abstract class Country : ServerObject
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

		[Column("code")]
		public abstract string Code
		{
			get;
			set;
		}

		[Children(typeof(User), "Country")]
		public abstract ServerObjectCollection Users
		{
			get;
		}
	}
}
