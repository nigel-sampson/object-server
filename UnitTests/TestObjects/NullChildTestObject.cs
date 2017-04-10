using System;
using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullChildren", "Id", PrimaryKeyType.Identity)]
	public abstract class NullChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("parent", CanBeNull = true)]
		public abstract NullParentTestObject Parent
		{
			get;
			set;
		}

		[Column("value")]
		public abstract int Value
		{
			get;
			set;
		}
	}
}
