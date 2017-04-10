using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("EmailClickTotals", "Id", PrimaryKeyType.Identity)]
	public abstract class EmailClickTotal : ServerObject, IComparable
	{
		protected EmailClickTotal()
		{
			total = 0;
		}

		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Parent("link")]
		public abstract EmailLink Link
		{
			get;
			set;
		}

		public int CompareTo(object obj)
		{
			if(!(obj is EmailClickTotal))
				throw new ApplicationException("object is not a EmailClickTotal");

			EmailClickTotal total = obj as EmailClickTotal;

			return Link.Ordering.CompareTo(total.Link.Ordering);
		}

		[Parent("recipe")]
		public abstract WeeklyRecipe Email
		{
			get;
			set;
		}

		[Column("total")]
		protected abstract int total
		{
			get;
			set;
		}

		public int Total
		{
			get
			{
				return total;
			}
			set
			{
				if(value < total)
					throw new ArgumentException("The total clicks can never decrease");

				total = value;

				last = DateTime.Now;
			}
		}

		[Column("final")]
		protected abstract DateTime last
		{
			get;
			set;
		}

		public DateTime Last
		{
			get
			{
				return last;
			}
		}
	}
}

