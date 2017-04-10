using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Types", "Id", PrimaryKeyType.Identity)]
	public abstract class SugarType : ServerObject
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

		[Children(typeof(UserSugarTypeMap), "Type")]
		public abstract ServerObjectCollection UserMaps
		{
			get;
		}
	}
}
