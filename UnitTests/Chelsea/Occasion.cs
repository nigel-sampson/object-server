using System;
using System.Xml.Serialization;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Occasions", "Id", PrimaryKeyType.Identity, DefaultOrder="name")]
	public abstract class Occasion : ServerObject, IComparable
	{
		public const int MaxNameLength = 50;

		[XmlType("Occasion")]
		public class ServiceProxy
		{
			private int id;
			private string name;

			public ServiceProxy()
			{
				id = -1;
				name = String.Empty;
			}

			public ServiceProxy(Occasion occasion)
			{
				id = occasion.Id;
				name = occasion.Name;
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

			public string Name
			{
				get
				{
					return name;
				}
				set
				{
					name = value;
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

		public int CompareTo(object obj)
		{
			if(!(obj is Occasion))
				throw new ApplicationException("object is not an Occasion");

			Occasion occasion = obj as Occasion;

			return Name.CompareTo(occasion.Name);
		}

		[Parent("featured", CanBeNull = true, DeleteAction = DeleteAction.Null)]
		public abstract Recipe Featured
		{
			get;
			set;
		}

		[Children(typeof(RecipeOccasionMap), "Occasion")]
		public abstract ServerObjectCollection RecipeMaps
		{
			get;
		}
	}
}
