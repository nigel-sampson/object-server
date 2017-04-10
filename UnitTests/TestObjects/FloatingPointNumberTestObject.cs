using System;

using Nichevo.ObjectServer;
namespace UnitTests.TestObjects
{
	[Table("FloatingPointNumbers", "Id", PrimaryKeyType.Guid)]
	public abstract class FloatingPointNumberTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("dec")]
		public abstract decimal Decimal
		{
			get;
			set;
		}

		[Column("num")]
		public abstract decimal Numeric
		{
			get;
			set;
		}

		[Column("flo")]
		public abstract double Float
		{
			get;
			set;
		}

		[Column("rea")]
		public abstract float Real
		{
			get;
			set;
		}

		[Column("mon")]
		public abstract decimal Money
		{
			get;
			set;
		}

		[Column("smon")]
		public abstract decimal SmallMoney
		{
			get;
			set;
		}
	}
}
