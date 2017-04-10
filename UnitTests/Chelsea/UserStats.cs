using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("UserStatistics", "Id", PrimaryKeyType.Defined)]
	public abstract class UserStats : ServerObject
	{
		public static UserStats Current(ObjectTransaction transaction)
		{
			return transaction.Select(typeof(UserStats), 1) as UserStats;
		}

		[Column("Id")]
		protected abstract int Id
		{
			get;
		}

		[Column("Recipes")]
		protected abstract int recipes
		{
			get;
			set;
		}

		public int WantsWeeklyRecipes
		{
			get
			{
				return recipes;
			}
		}

		[Column("Promotions")]
		protected abstract int promotions
		{
			get;
			set;
		}

		public int WantsPromotionalEmails
		{
			get
			{
				return promotions;
			}
		}

		[Column("Recent")]
		protected abstract int recent
		{
			get;
			set;
		}

		public int RecentRegistrations
		{
			get
			{
				return recent;
			}
		}

		public DateTime RecentRegistrationDate
		{
			get
			{
				return DateTime.Today.AddMonths(-1);
			}
		}

		[Column("Total")]
		protected abstract int total
		{
			get;
			set;
		}

		public int TotalRegistrations
		{
			get
			{
				return total;
			}
		}
	}
}
