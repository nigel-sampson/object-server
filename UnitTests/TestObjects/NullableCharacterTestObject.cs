using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("NullableCharacters", "Id", PrimaryKeyType.Guid)]
	public abstract class NullableCharacterTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("chCol", NullValue = 'x')]
		public abstract string Char
		{
			get;
			set;
		}

		[Column("nchCol", NullValue = 'z')]
		public abstract string NChar
		{
			get;
			set;
		}

		[Column("vchCol", NullValue = "<null value>")]
		public abstract string VChar
		{
			get;
			set;
		}

		[Column("nvchCol", NullValue = "<null n value>")]
		public abstract string NVChar
		{
			get;
			set;
		}

		[Column("txtCol", NullValue = "<null text value>")]
		public abstract string Text
		{
			get;
			set;
		}

		[Column("ntxtCol", NullValue = "<null ntext value>")]
		public abstract string NText
		{
			get;
			set;
		}
	}
}
