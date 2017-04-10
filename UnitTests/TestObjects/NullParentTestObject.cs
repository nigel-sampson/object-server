using System;
using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullParents", "Id", PrimaryKeyType.Identity)]
	public abstract class NullParentTestObject : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Children(typeof(NullChildTestObject), "Parent")]
		public abstract ServerObjectCollection ChildObjects
		{
			get;
		}

		[Column("value")]
		public abstract int Value
		{
			get;
			set;
		}
	}
}
