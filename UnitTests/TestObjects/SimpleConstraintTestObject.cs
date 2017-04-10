using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("GeneralData", "Id", PrimaryKeyType.Identity)]
	public abstract class SimpleConstraintTestObject : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Column("vch")]
		public abstract string Varchar
		{
			get;
			set;
		}

		[Column("num")]
		public abstract int Integer
		{
			get;
			set;
		}

		[Column("bl")]
		public abstract bool Boolean
		{
			get;
			set;
		}

		[Column("dt")]
		public abstract DateTime Date
		{
			get;
			set;
		}

		[Column("nullint", NullValue = -1)]
		public abstract int NullableInteger
		{
			get;
			set;
		}
	}
}