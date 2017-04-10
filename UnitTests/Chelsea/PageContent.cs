using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea 
{
	[Table("Pages", "Name", PrimaryKeyType.Defined, DefaultOrder = "Title")]
	public abstract class PageContent : ServerObject
	{
		public const string ImageDir = "~/shared/images/content/";

		[Column("page")]
		public abstract string Name
		{
			get;
		}

		[Column("txt")]
		public abstract string Text
		{
			get;
			set;
		}

		public string Teaser
		{
			get
			{
				return Utility.LimitString(150, Text) + "...";
			}
		}

		[Column("url")]
		public abstract string Url
		{
			get;
			set;
		}

		[Column("title")]
		public abstract string Title
		{
			get;
			set;
		}
	}
}
