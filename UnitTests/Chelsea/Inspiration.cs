using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Inspirations", "Id", PrimaryKeyType.Identity)]
	public abstract class Inspiration : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Column("quote")]
		protected abstract string quote
		{
			get;
			set;
		}

		public string Quote
		{
			get
			{
				return quote;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Quote cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Quote cannot be an empty string");

				quote = value;
			}
		}

		public string Teaser
		{
			get
			{
				return Utility.LimitString(60, Quote) + "...";
			}
		}

		[Column("author", NullValue = "")]
		public abstract string Author
		{
			get;
			set;
		}

		public string Display
		{
			get
			{
				string author = Author.Length > 0 ? " - " + Author : String.Empty;

				return Quote + author;
			}
		}
	}
}
