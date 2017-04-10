using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("EmailLinks", "Id", PrimaryKeyType.Identity, DefaultOrder = "Ordering")]
	public abstract class EmailLink : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Column("description")]
		public abstract string Description
		{
			get;
			set;
		}

		[Column("ordering")]
		public abstract int Ordering
		{
			get;
			set;
		}

		[Children(typeof(EmailClickTotal), "Link")]
		public abstract ServerObjectCollection Clicks
		{
			get;
		}
	}
}
