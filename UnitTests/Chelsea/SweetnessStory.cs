using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("SweetnessStories", "Id", PrimaryKeyType.Identity)]
	public abstract class SweetnessStory : ServerObject
	{
		public const int MaxTitleLength = 50;
		public const int MaxAuthorLength = 50;	
	
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Column("story")]
		public abstract string Story
		{
			get;
			set;
		}

		[Column("title")]
		protected abstract string title
		{
			get;
			set;
		}

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Title cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Title cannot be an empty string");

				if(value.Length > MaxTitleLength)
					throw new ArgumentException(String.Format("Title cannot be more than {0} characters", MaxTitleLength));

				title = value;
			}
		}

		[Column("author")]
		protected abstract string author
		{
			get;
			set;
		}

		public string Author
		{
			get
			{
				return author;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Author cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Author cannot be an empty string");

				if(value.Length > MaxAuthorLength)
					throw new ArgumentException(String.Format("Author cannot be more than {0} characters", MaxAuthorLength));

				author = value;
			}
		}
	}
}
