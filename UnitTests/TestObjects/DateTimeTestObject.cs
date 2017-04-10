using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("DateTimes", "Id", PrimaryKeyType.Guid)]
	public abstract class DateTimeTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("dt")]
		public abstract DateTime Date
		{
			get;
			set;
		}

		[Column("smdt")]
		public abstract DateTime SmallDate
		{
			get;
			set;
		}
	}
}
