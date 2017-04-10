using System;
using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("ParentB", "Id", PrimaryKeyType.Identity)]
	public abstract class ParentBTestObject : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Column("data")]
		public abstract string ObjData
		{
			get;
			set;
		}

		[Children(typeof(ParentATestObject), "ParentB")]
		public abstract ServerObjectCollection ParentAs
		{
			get;
		}

		[Children(typeof(ChildATestObject), "ParentB")]
		public abstract ServerObjectCollection ChildObjs
		{
			get;
		}
	}
}
