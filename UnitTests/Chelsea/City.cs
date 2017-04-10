using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Cities", "Id", PrimaryKeyType.Identity, DefaultOrder = "Order DESC, Name")]
	public abstract class City : ServerObject
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

		[Column("ordering")]
		public abstract int Order
		{
			get;
		}

		[Children(typeof(User), "City")]
		public abstract ServerObjectCollection Users
		{
			get;
		}
	}
}