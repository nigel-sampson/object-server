using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("GuidChildren", "Id", PrimaryKeyType.Guid)]
	public abstract class GuidChildTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Parent("parent")]
		public abstract GuidParentTestObject Parent
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
