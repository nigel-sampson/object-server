using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("IdentityParents", "Id", PrimaryKeyType.Identity)]
	public abstract class IdentityParentTestObject : ServerObject
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
		
		[Children(typeof(IdentityChildTestObject), "Parent")]
		public abstract ServerObjectCollection ChildObjects
		{
			get;
		}
	}
}
