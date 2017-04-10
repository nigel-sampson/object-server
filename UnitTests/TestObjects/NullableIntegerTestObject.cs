using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullableIntegers", "Id", PrimaryKeyType.Guid)]
	public abstract class NullableIntegerTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("bCol", NullValue = false)]
		public abstract bool Boolean
		{
			get;
			set;
		}

		[Column("tintCol", NullValue = 1)]
		public abstract byte TinyInt
		{
			get;
			set;
		}

		[Column("sintCol", NullValue = -1)]
		public abstract short SmallInt
		{
			get;
			set;
		}

		[Column("intCol", NullValue = -2)]
		public abstract int Int
		{
			get;
			set;
		}

		[Column("bintCol", NullValue = -3)]
		public abstract long BigInt
		{
			get;
			set;
		}
	}
}
