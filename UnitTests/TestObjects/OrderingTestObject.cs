using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("OrderedData", "Id", PrimaryKeyType.Identity, DefaultOrder = "Varchar DESC")]
	public abstract class OrderingTestObject : ServerObject
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
	}
}
