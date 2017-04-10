using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("ProductSizes", "Id", PrimaryKeyType.Identity)]
	public abstract class ProductSize : ServerObject
	{
		public const string ImageDir = "~/shared/images/products/";
		public const int MaxNameLength = 50;

		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("product")]
		public abstract Product Product
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
