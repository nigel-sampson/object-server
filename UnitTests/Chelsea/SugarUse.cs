using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Uses", "Id", PrimaryKeyType.Identity)]
	public abstract class SugarUse : ServerObject
	{
		[Column("id")]
		public abstract int Id
		{
			get;
		}

		[Column("name")]
		public abstract string Name
		{
			get;
			set;
		}

		[Children(typeof(UserSugerUseMap), "Use")]
		public abstract ServerObjectCollection UserMaps
		{
			get;
		}
	}
}
