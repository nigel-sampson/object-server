using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("Binary", "Id", PrimaryKeyType.Guid)]
	public abstract class BinaryTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("bin")]
		public abstract byte[] Binary
		{
			get;
			set;
		}

		[Column("vbin")]
		public abstract byte[] VarBinary
		{
			get;
			set;
		}
		[Column("img")]
		public abstract byte[] Image
		{
			get;
			set;
		}
	}
}
