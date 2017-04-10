using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("RecipesProductMaps", "Id", PrimaryKeyType.Identity)]
	public abstract class RecipeProductMap : ServerObject
	{
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

		[Parent("product", DeleteAction = DeleteAction.Cascade)]
		public abstract Product Product
		{
			get;
			set;
		}
	}
}
