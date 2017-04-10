using System;
using System.IO;
using System.Web;
using System.Collections;
using System.Xml.Serialization;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Recipes", "Id", PrimaryKeyType.Identity, DefaultOrder = "name")]
	public abstract class Recipe : ServerObject, IComparable
	{
		public const int MaxNameLength = 100;
		public const string ImageDir = "~/shared/images/recipes/";
		public const string MovieDir = "shared/movies/recipes/";
		public const string ScreensaverDir = "~/shared/images/screensaver/";
		public const int ImageWidth = 238;
		public const float MaxImageRatio = 1.0f;
		public const float MinImageRatio = 0.75f;
		private static Hashtable replacements;

		protected Recipe()
		{
			WhenCreated = DateTime.Now;
			IsApproved = false;
			Display = RecipeDisplay.RecipeClub;
		}

		static Recipe()
		{
			replacements = new Hashtable();
			replacements.Add("tsp", "teaspoon");
			replacements.Add("sugar", "Chelsea Sugar");
			replacements.Add("treacle", "Chelsea Treacle");
			replacements.Add("golden syrup", "Chelsea Golden Syrup");
		}

		[XmlType("Recipe")]
		public class ServiceProxy
		{
			private int id;
			private string name;
			private string method;
			private string image;
			private Occasion.ServiceProxy[] occasions;
			private Product.ServiceProxy[] products;

			public ServiceProxy()
			{
				id = -1;
				name = String.Empty;
				method = String.Empty;
				image = String.Empty;
				occasions = new Occasion.ServiceProxy[] {};
				products = new Product.ServiceProxy[] {};
			}

			public ServiceProxy(Recipe recipe)
			{
				id = recipe.Id;
				name = recipe.Name;
				image = recipe.Image;
				method = Utility.LimitString(150, recipe.Method) + "...";

				occasions = new Occasion.ServiceProxy[recipe.OccasionMaps.Count];
				int i = 0;
				foreach(RecipeOccasionMap map in recipe.OccasionMaps)
				{
					occasions[i++] = map.Occasion.ConstructProxy();
				}

				products = new Product.ServiceProxy[recipe.ProductMaps.Count];
				int j = 0;
				foreach(RecipeProductMap map in recipe.ProductMaps)
				{
					products[j++] = map.Product.ConstructProxy();
				}
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

			public string Method
			{
				get
				{
					return method;
				}
				set
				{
					method = value;
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

			public Occasion.ServiceProxy[] Occasions
			{
				get
				{
					return occasions;
				}
				set
				{
					occasions = value;
				}
			}

			public Product.ServiceProxy[] Products
			{
				get
				{
					return products;
				}
				set
				{
					products = value;
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

		[Parent("author", CanBeNull = true, DeleteAction = DeleteAction.Null)]
		public abstract User Author
		{
			get;
			set;
		}

		[Column("authorComment", NullValue = "")]
		public abstract string AuthorsComments
		{
			get;
			set;
		}

		public void MakeReplacements()
		{
			foreach(DictionaryEntry entry in replacements)
			{
				Method = Method.Replace(entry.Key.ToString(), entry.Value.ToString());
				Ingredients = Ingredients.Replace(entry.Key.ToString(), entry.Value.ToString());
			}
		}

		[Column("method")]
		public abstract string Method
		{
			get;
			set;
		}

		[Column("ingredients")]
		public abstract string Ingredients
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

		[Column("authorApproved")]
		public abstract bool IsApproved
		{
			get;
			set;
		}
	
		[Parent("category")]
		public abstract RecipeCategory Category
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

		public int CompareTo(object obj)
		{
			if(!(obj is Recipe))
				throw new ApplicationException("object is not a Recipe");

			Recipe recipe = obj as Recipe;

			return Name.CompareTo(recipe.Name);
		}

		[Children(typeof(RecipeCategory), "Featured")]
		public abstract ServerObjectCollection CategoriesFeaturedIn
		{
			get;
		}

		[Children(typeof(Occasion), "Featured")]
		public abstract ServerObjectCollection OccasionsFeaturedIn
		{
			get;
		}

		[Children(typeof(RecipeOccasionMap), "Recipe")]
		public abstract ServerObjectCollection OccasionMaps
		{
			get;
		}

		[Children(typeof(RecipeProductMap), "Recipe")]
		public abstract ServerObjectCollection ProductMaps
		{
			get;
		}

		[Children(typeof(WeeklyRecipe), "Recipe")]
		public abstract ServerObjectCollection WeeksFeaturedIn
		{
			get;
		}

		public ServerObjectCollection PerfectFor
		{
			get
			{
				if(OccasionMaps.Count < 8)
					return OccasionMaps;

				ServerObjectCollection perfect = new ServerObjectCollection();

				for(int i = 0; i < 8; i++)
				{
					perfect.Add(OccasionMaps[i]);
				}

				return perfect;
			}
		}

		[Column("image", NullValue = "")]
		protected abstract string image
		{
			get;
			set;
		}

		[Column("movie", NullValue = "")]
		protected abstract string movie
		{
			get;
			set;
		}

		public string Modem
		{
			get
			{
				return String.Format("{0}{1}_56k.asx", MovieDir, movie);
			}
		}

		public string Broadband
		{
			get
			{
				return String.Format("{0}{1}_128k.asx", MovieDir, movie);
			}
		}

		public bool HasMovie
		{
			get
			{
				return movie.Length != 0;
			}
		}

		public bool HasScreensaverImage
		{
			get
			{
				return File.Exists(HttpContext.Current.Server.MapPath(ScreensaverDir + Image));
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
				if(value == null)
					throw new ArgumentNullException("value", "Image cannot be null");

				value = value.Trim();

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

		[Children(typeof(RecipeBookItem), "Recipe")]
		public abstract ServerObjectCollection BookItems
		{
			get;
		}

		[Column("display")]
		public abstract RecipeDisplay Display
		{
			get;
			set;
		}
	}
}
