using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullIdentityParents", "Id", PrimaryKeyType.Identity)]
	public abstract class NullIdentityParentTestObject : ServerObject
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
		
		[Children(typeof(NullIdentityChildTestObject), "Parent")]
		public abstract ServerObjectCollection ChildObjects
		{
			get;
		}
	}
}
