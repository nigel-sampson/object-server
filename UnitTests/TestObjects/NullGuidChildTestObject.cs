using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullGuidChildren", "Id", PrimaryKeyType.Guid)]
	public abstract class NullGuidChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Parent("parent", CanBeNull = true, DeleteAction = DeleteAction.Null)]
		public abstract NullGuidParentTestObject Parent
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
