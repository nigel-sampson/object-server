using System;
using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("ChildA", "Id", PrimaryKeyType.Identity)]
	public abstract class ChildATestObject : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("parentA", DeleteAction = DeleteAction.Cascade)]
		public abstract ParentATestObject ParentA
		{
			get;
			set;
		}

		[Parent("parentB", DeleteAction = DeleteAction.Cascade)]
		public abstract ParentBTestObject ParentB
		{
			get;
			set;
		}
	}
}
