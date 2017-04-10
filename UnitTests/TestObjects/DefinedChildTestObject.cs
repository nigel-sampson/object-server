using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("DefinedChildren", "Id", PrimaryKeyType.Defined)]
	public abstract class DefinedChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract string Id
		{
			get;
		}

		[Parent("parent")]
		public abstract DefinedParentTestObject Parent
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
