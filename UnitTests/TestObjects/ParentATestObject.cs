using System;
using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("ParentA", "Id", PrimaryKeyType.Identity)]
	public abstract class ParentATestObject : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("parent", DeleteAction = DeleteAction.Cascade)]
		public abstract ParentBTestObject ParentB
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

		[Children(typeof(ChildATestObject), "ParentA")]
		public abstract ServerObjectCollection ChildObjs
		{
			get;
		}
	}
}
