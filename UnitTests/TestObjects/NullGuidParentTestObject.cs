using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullGuidParents", "Id", PrimaryKeyType.Guid)]
	public abstract class NullGuidParentTestObject : ServerObject
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

		[Children(typeof(NullGuidChildTestObject), "Parent")]
		public abstract ServerObjectCollection ChildObjects
		{
			get;
		}
	}
}
