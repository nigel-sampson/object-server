using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("IdentityKeys", "Id", PrimaryKeyType.Identity)]
	public abstract class IdentityKeyTestObject : ServerObject
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
	}
}
