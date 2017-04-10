using System;
using System.Globalization;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("WeeklyRecipes", "Week", PrimaryKeyType.Defined)]
	public abstract class WeeklyRecipe : ServerObject, IComparable
	{
		protected WeeklyRecipe()
		{
			Sent = 0;
		}

		public static string CurrentWeek()
		{
			int week = CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Today, CalendarWeekRule.FirstFullWeek, DayOfWeek.Wednesday);
			return String.Format("{0}-{1}", DateTime.Today.Year, week);
		}
	
		public string Subject
		{
			get
			{
				return String.Format("{0} : \"This week's complimentary recipe from Chelsea Sugar\"", Recipe.Name);
			}
		}

		public int CompareTo(object obj)
		{
			if(!(obj is WeeklyRecipe))
				throw new ApplicationException("object is not a WeeklyRecipe");

			WeeklyRecipe recipe = obj as WeeklyRecipe;

			if(YearNumber.CompareTo(recipe.YearNumber) == 0)
			{
				return WeekNumber.CompareTo(recipe.WeekNumber);
			}
			else
				return YearNumber.CompareTo(recipe.YearNumber);
		}

		[Column("week")]
		public abstract string Week
		{
			get;
		}

		private int YearNumber
		{
			get
			{
				return Convert.ToInt32(Week.Split('-')[0]);
			}
		}

		private int WeekNumber
		{
			get
			{
				return Convert.ToInt32(Week.Split('-')[1]);
			}
		}

		public DateTime Starting
		{
			get
			{
				return new DateTime(YearNumber, 1, 1).AddDays((WeekNumber * 7) - 1);
			}
		}

		[Parent("recipe")]
		public abstract Recipe Recipe
		{
			get;
			set;
		}

		[Parent("tip")]
		public abstract Tip Tip
		{
			get;
			set;
		}

		[Column("sent")]
		protected abstract int sent
		{
			get;
			set;
		}

		public int Sent
		{
			get
			{
				return sent;
			}
			set
			{
				if(value < 0)
					throw new ArgumentException("The number of emails sent must be more than zero");

				sent = value;
			}
		}

		[Children(typeof(EmailClickTotal), "Email")]
		public abstract ServerObjectCollection Clicks
		{
			get;
		}
	}
}
