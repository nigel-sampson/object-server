using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("GuidParents", "Id", PrimaryKeyType.Guid)]
	public abstract class CascadeGuidParentTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("data")]
		public abstract string ObjData
		{
			get;
			set;
		}

		[Children(typeof(CascadeGuidChildTestObject), "Parent")]
		public abstract ServerObjectCollection ChildObjects
		{
			get;
		}
	}
}
