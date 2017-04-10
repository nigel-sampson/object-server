using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("IdentityChildren", "Id", PrimaryKeyType.Identity)]
	public abstract class CascadeIdentityChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("parent", DeleteAction = DeleteAction.Cascade)]
		public abstract CascadeIdentityParentTestObject Parent
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
