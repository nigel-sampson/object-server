using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("SweetnessFriends", "Id", PrimaryKeyType.Identity)]
	public abstract class SweetnessFriend : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}
		
		[Parent("entrant")]
		public abstract SweetnessEntry Entrant
		{
			get;
			set;
		}

		[Column("email")]
		public abstract string Email
		{
			get;
			set;
		}
	}
}
