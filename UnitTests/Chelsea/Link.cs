using System;
using System.Text.RegularExpressions;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Links", "Id", PrimaryKeyType.Identity, DefaultOrder = "title")]
	public abstract class Link : ServerObject
	{
		public const int MaxTitleLength = 100;
		public const int MaxUrlLength = 100;

		public const string UrlValidationExpression = Constants.UrlValidationExpression;

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

		[Column("url")]
		protected abstract string url
		{
			get;
			set;
		}

		public string Url
		{
			get
			{
				return url;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Url cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Url cannot be an empty string");

				if(value.Length > MaxUrlLength)
					throw new ArgumentException(String.Format("Url cannot be more than {0} characters", MaxUrlLength));

				Regex expression = new Regex(UrlValidationExpression);

				if(!expression.IsMatch(value))
					throw new ArgumentException("Url is not a valid internet address");

				url = value;
			}
		}

		[Column("description")]
		public abstract string Description
		{
			get;
			set;
		}
	}
}
