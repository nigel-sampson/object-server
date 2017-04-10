using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("DefinedKeys", "Id", PrimaryKeyType.Defined)]
	public abstract class DefinedKeyTestObject : ServerObject
	{
		[Column("id")]
		public abstract string Id
		{
			get;
		}

		[Column("data")]
		public abstract int ObjData
		{
			get;
			set;
		}
	}
}
