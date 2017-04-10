using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("News", "Id", PrimaryKeyType.Identity, DefaultOrder = "Order, Created")]
	public abstract class NewsItem : ServerObject
	{
		public const int MaxTitleLength = 50;
		public const int MaxThumbnailLength = 50;
		public const string ThumbnailDir = "~/shared/images/news/";
		public const string DefaultThumnail = "~/images/noimage.jpg";

		protected NewsItem()
		{
			WhenCreated = DateTime.Now;
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

		public string Teaser
		{
			get
			{
				return Utility.LimitString(150, Body) + "...";
			}
		}

		[Column("body")]
		public abstract string Body
		{
			get;
			set;
		}

		[Column("ordering")]
		public abstract int Order
		{
			get;
			set;
		}

		[Column("enabled")]
		public abstract bool IsEnabled
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

		[Column("type")]
		public abstract NewTypes Type
		{
			get;
			set;
		}

		[Column("thumbnail", NullValue = "")]
		public abstract string Thumbnail
		{
			get;
			set;
		}

		public string ThumbnailPath
		{
			get
			{
				return ThumbnailDir + Thumbnail;
			}
		}

		public string Thumb
		{
			get
			{
				return Thumbnail.Length > 0 ? ThumbnailPath : DefaultThumnail;
			}
		}
	}
}
