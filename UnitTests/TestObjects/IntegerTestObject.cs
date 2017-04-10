using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("Integers", "Id", PrimaryKeyType.Guid)]
	public abstract class IntegerTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("bCol")]
		public abstract bool Boolean
		{
			get;
			set;
		}

		[Column("tintCol")]
		public abstract byte TinyInt
		{
			get;
			set;
		}

		[Column("sintCol")]
		public abstract short SmallInt
		{
			get;
			set;
		}

		[Column("intCol")]
		public abstract int Int
		{
			get;
			set;
		}

		[Column("bintCol")]
		public abstract long BigInt
		{
			get;
			set;
		}
	}
}
