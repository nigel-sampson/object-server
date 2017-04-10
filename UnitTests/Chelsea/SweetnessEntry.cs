using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("SweetnessEntries", "Id", PrimaryKeyType.Identity, DefaultOrder = "WhenEntered DESC")]
	public abstract class SweetnessEntry : ServerObject
	{
		public const int MaxNameLength = 50;
		public const int MaxTitleLength = 50;

		protected SweetnessEntry()
		{
			WhenEntered = DateTime.Now;
			Rating = 0;
		}

		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("userid")]
		public abstract User Entrant
		{
			get;
			set;
		}

		[Column("story")]
		public abstract string Story
		{
			get;
			set;
		}

		[Column("entered")]
		public abstract DateTime WhenEntered
		{
			get;
			set;
		}

		[Column("name")]
		protected abstract string name
		{
			get;
			set;
		}

		public string Name
		{
			get
			{
				return name;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Name cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Name cannot be an empty string");

				if(value.Length > MaxNameLength)
					throw new ArgumentException(String.Format("Name cannot be more than {0} characters", MaxNameLength));

				name = value;
			}
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

		[Children(typeof(SweetnessFriend), "Entrant")]
		public abstract ServerObjectCollection Friends
		{
			get;
		}

		[Column("rating")]
		protected abstract int rating
		{
			get;
			set;
		}

		public int Rating
		{
			get
			{
				return rating;
			}
			set
			{
				if(value < 0 || value > 10)
					throw new ArgumentOutOfRangeException("value", value, "Rating must be between 0 and 10");

				rating = value;
			}
		}
	}
}
