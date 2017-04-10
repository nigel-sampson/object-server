using System;

using Nichevo.ObjectServer;

namespace UnitTests.TestObjects
{
	[Table("Characters", "Id", PrimaryKeyType.Guid)]
	public abstract class CharacterTestObject : ServerObject
	{
		[Column("id")]
		public abstract Guid Id
		{
			get;
		}

		[Column("chCol")]
		public abstract string Char
		{
			get;
			set;
		}

		[Column("nchCol")]
		public abstract string NChar
		{
			get;
			set;
		}

		[Column("vchCol")]
		public abstract string VChar
		{
			get;
			set;
		}

		[Column("nvchCol")]
		public abstract string NVChar
		{
			get;
			set;
		}

		[Column("txtCol")]
		public abstract string Text
		{
			get;
			set;
		}

		[Column("ntxtCol")]
		public abstract string NText
		{
			get;
			set;
		}
	}
}
