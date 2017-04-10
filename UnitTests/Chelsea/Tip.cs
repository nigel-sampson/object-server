using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Tips", "Id", PrimaryKeyType.Identity, DefaultOrder = "title")]
	public abstract class Tip : ServerObject
	{
		public const int MaxTitleLength = 50;

		protected Tip()
		{
			WhenCreated = DateTime.Now;
			ShowOnSite = false;
		}

		[Column("id")]
		public abstract int Id
		{
			get;
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

		[Parent("author")]
		public abstract User Author
		{
			get;
			set;
		}

		[Column("body")]
		public abstract string Body
		{
			get;
			set;
		}

		[Column("created")]
		public abstract DateTime WhenCreated
		{
			get;
			set;
		}

		[Column("show")]
		public abstract bool ShowOnSite
		{
			get;
			set;
		}

		[Children(typeof(WeeklyRecipe), "Tip")]
		public abstract ServerObjectCollection WeeksFeaturedIn
		{
			get;
		}
	}
}
