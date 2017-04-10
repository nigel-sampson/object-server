using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("IdentityChildren", "Id", PrimaryKeyType.Identity)]
	public abstract class IdentityChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("parent")]
		public abstract IdentityParentTestObject Parent
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
