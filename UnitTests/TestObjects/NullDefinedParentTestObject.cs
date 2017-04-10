using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullDefinedParents", "Id", PrimaryKeyType.Defined)]
	public abstract class NullDefinedParentTestObject : ServerObject
	{
		[Column("id")]
		public abstract string Id
		{
			get;
		}

		[Column("data")]
		public abstract string ObjData
		{
			get;
			set;
		}

		[Children(typeof(NullDefinedChildTestObject), "Parent")]
		public abstract ServerObjectCollection ChildObjects
		{
			get;
		}
	}
}
