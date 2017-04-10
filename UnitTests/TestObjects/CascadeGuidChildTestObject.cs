using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("GuidChildren", "Id", PrimaryKeyType.Guid)]
	public abstract class CascadeGuidChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Parent("parent", DeleteAction = DeleteAction.Cascade)]
		public abstract CascadeGuidParentTestObject Parent
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
