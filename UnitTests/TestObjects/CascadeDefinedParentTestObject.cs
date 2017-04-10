using System;
using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("DefinedParents", "Id", PrimaryKeyType.Defined)]
	public abstract class CascadeDefinedParentTestObject : ServerObject
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

		[Children(typeof(CascadeDefinedChildTestObject), "Parent")]
		public abstract ServerObjectCollection ChildObjects
		{
			get;
		}
	}
}
