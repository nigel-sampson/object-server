using System;
using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("DefinedChildren", "Id", PrimaryKeyType.Defined)]
	public abstract class CascadeDefinedChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract string Id
		{
			get;
		}

		[Parent("parent", DeleteAction = DeleteAction.Cascade)]
		public abstract CascadeDefinedParentTestObject Parent
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
