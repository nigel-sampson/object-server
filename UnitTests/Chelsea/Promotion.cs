using System;
using System.Xml.Serialization;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Promotions", "Id", PrimaryKeyType.Identity)]
	public abstract class Promotion : ServerObject
	{
		public const int MaxNameLength = 50;
		public const int MaxImageLength = 50;
		public const int ImageWidth = 100;
		public const string ImageDir = "~/shared/images/promos/";

		[XmlType("Promotion")]
		public class ServiceProxy
		{
			private string name;
			private string text;
			private string image;

			public ServiceProxy()
			{
				name = String.Empty;
				text = String.Empty;
				image = String.Empty;
			}

			public ServiceProxy(Promotion promotion)
			{
				name = promotion.Name;
				text = promotion.Text;
				image = promotion.Image;
			}

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
				}
			}

			public string Text
			{
				get
				{
					return text;
				}
				set
				{
					text = value;
				}
			}

			public string Image
			{
				get
				{
					return image;
				}
				set
				{
					image = value;
				}
			}
		}

		public ServiceProxy ConstructProxy()
		{
			return new ServiceProxy(this);
		}

		[Column("id")]
		public abstract int Id
		{
			get;
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

		[Column("txt")]
		public abstract string Text
		{
			get;
			set;
		}

		[Column("image")]
		protected abstract string image
		{
			get;
			set;
		}

		public string Image
		{
			get
			{
				return image;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Image cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Image cannot be an empty string");

				if(value.Length > MaxImageLength)
					throw new ArgumentException(String.Format("Image cannot be more than {0} characters", MaxImageLength));

				image = value;
			}
		}

		public string ImagePath
		{
			get
			{
				return ImageDir + Image;
			}
		}
	}
}
