using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("RecipeCategories", "Id", PrimaryKeyType.Identity)]
	public abstract class RecipeCategory : ServerObject
	{
		public const int MaxNameLength = 30;

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

		[Children(typeof(Recipe), "Category")]
		public abstract ServerObjectCollection Recipes
		{
			get;
		}

		[Parent("featured", CanBeNull = true, DeleteAction = DeleteAction.Null)]
		public abstract Recipe Featured
		{
			get;
			set;
		}
	}
}
