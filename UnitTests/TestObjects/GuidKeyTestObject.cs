using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("GuidKeys", "Id", PrimaryKeyType.Guid)]
	public abstract class GuidKeyTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
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
