using System;
using System.Xml.Serialization;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("RecipeBookItems", "Id", PrimaryKeyType.Identity)]
	public abstract class RecipeBookItem : ServerObject
	{
		[XmlType("RecipeBookItem")]
		public class ServiceProxy
		{
			private int id;
			private int recipeId;
			private string title;
			private string notes;
			private string image;

			public ServiceProxy()
			{
				id = -1;
				recipeId = -1;
				title = String.Empty;
				notes = String.Empty;
				image = String.Empty;
			}

			public ServiceProxy(RecipeBookItem item)
			{
				id = item.Id;
				recipeId = item.Recipe.Id;
				title = item.Recipe.Name;
				notes = item.Notes;
				image = item.Recipe.Image;
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

			public int RecipeId
			{
				get
				{
					return recipeId;
				}
				set
				{
					recipeId = value;
				}
			}

			public string Title
			{
				get
				{
					return title;
				}
				set
				{
					title = value;
				}
			}

			public string Notes
			{
				get
				{
					return notes;
				}
				set
				{
					notes = value;
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

		[Parent("recipe", DeleteAction = DeleteAction.Cascade)]
		public abstract Recipe Recipe
		{
			get;
			set;
		}

		[Parent("userid", DeleteAction = DeleteAction.Cascade)]
		public abstract User User
		{
			get;
			set;
		}

		[Column("notes")]
		public abstract string Notes
		{
			get;
			set;
		}
	}
}
