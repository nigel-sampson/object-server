using System;
using System.Xml.Serialization;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Products", "Id", PrimaryKeyType.Identity)]
	public abstract class Product : ServerObject
	{
		public const int MaxNameLength = 50;
		public const string ThumbnailDir = "~/shared/images/productthumbnails/";
		public const string LargeThumbnailDir = "~/shared/images/productthumbnails/large/";

		[XmlType("Product")]
		public class ServiceProxy
		{
			private int id;
			private string name;
			private string description;
			private string image;

			public ServiceProxy()
			{
				id = -1;
				name = String.Empty;
				description = String.Empty;
				image = String.Empty;
			}

			public ServiceProxy(Product product)
			{
				id = product.Id;
				name = product.Name;
				description = product.Ingredients;
				image = product.Thumbnail;
			}

			public int Id
			{
				get
				{
					return id;
				}
				set
				{
					id = value;
				}	
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

			public string Description
			{
				get
				{
					return description;
				}
				set
				{
					description = value;
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

		[Column("thumbnail")]
		protected abstract string thumbnail
		{
			get;
			set;
		}

		public string Thumbnail
		{
			get
			{
				return thumbnail;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Thumbnail cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Thumbnail cannot be an empty string");

				thumbnail = value;
			}
		}

		public string ThumbnailPath
		{
			get
			{
				return ThumbnailDir + Thumbnail;
			}
		}

		public string LargeThumbnailPath
		{
			get
			{
				return LargeThumbnailDir + Thumbnail;
			}
		}

		[Column("Ingredients")]
		public abstract string Ingredients
		{
			get;
			set;
		}
		
		[Column("nutritional")]
		public abstract string NutritionalInformation
		{
			get;
			set;
		}
		
		[Parent("category")]
		public abstract ProductCategory Category
		{
			get;
			set;
		}

		[Children(typeof(ProductSize), "Product")]
		public abstract ServerObjectCollection Sizes
		{
			get;
		}

		[Children(typeof(RecipeProductMap), "Product")]
		public abstract ServerObjectCollection RecipeMaps
		{
			get;
		}
	}
}
