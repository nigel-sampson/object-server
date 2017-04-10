using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullableDateTimes", "Id", PrimaryKeyType.Guid)]
	public abstract class NullableDateTimeTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("dt", NullValue = "01/01/2004")]
		public abstract DateTime Date
		{
			get;
			set;
		}

		[Column("smdt", NullValue = "11/07/1981")]
		public abstract DateTime SmallDate
		{
			get;
			set;
		}
	}
}
