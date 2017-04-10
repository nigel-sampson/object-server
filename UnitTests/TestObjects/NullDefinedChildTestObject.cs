using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullDefinedChildren", "Id", PrimaryKeyType.Defined)]
	public abstract class NullDefinedChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract string Id
		{
			get;
		}

		[Parent("parent", CanBeNull = true, DeleteAction = DeleteAction.Null)]
		public abstract NullDefinedParentTestObject Parent
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
