using System;

using Nichevo.ObjectServer;
namespace UnitTests.TestObjects
{
	[Table("NullableFloatingPointNumbers", "Id", PrimaryKeyType.Guid)]
	public abstract class NullableFloatingPointNumberTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("dec", NullValue = -1)]
		public abstract decimal Decimal
		{
			get;
			set;
		}

		[Column("num", NullValue = -1)]
		public abstract decimal Numeric
		{
			get;
			set;
		}

		[Column("flo", NullValue = Double.MinValue)]
		public abstract double Float
		{
			get;
			set;
		}

		[Column("rea", NullValue = Single.MinValue)]
		public abstract float Real
		{
			get;
			set;
		}

		[Column("mon", NullValue = -1)]
		public abstract decimal Money
		{
			get;
			set;
		}

		[Column("smon", NullValue = -1)]
		public abstract decimal SmallMoney
		{
			get;
			set;
		}
	}
}
