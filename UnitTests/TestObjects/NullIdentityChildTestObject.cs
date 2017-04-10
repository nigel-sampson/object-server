using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullIdentityChildren", "Id", PrimaryKeyType.Identity)]
	public abstract class NullIdentityChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("parent", CanBeNull = true, DeleteAction = DeleteAction.Null)]
		public abstract NullIdentityParentTestObject Parent
		{
			get;
			set;
		}

		[Column("data")]
		public abstract string ObjData
		{
			get;
			set;
		}
	}
}
