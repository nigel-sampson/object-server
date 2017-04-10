using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("RecipeOccasionMaps", "Id", PrimaryKeyType.Identity)]
	public abstract class RecipeOccasionMap : ServerObject
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

		[Parent("occasion", DeleteAction = DeleteAction.Cascade)]
		public abstract Occasion Occasion
		{
			get;
			set;
		}
	}
}
