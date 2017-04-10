using System;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("FactSheets", "Id", PrimaryKeyType.Identity)]
	public abstract class FactSheet : ServerObject
	{
		public const int MaxNameLength = 50;
		public const string DocumentDir = "~/shared/documents/factsheets/";

		[Column("id")]
		public abstract int Id
		{
			get;
			set;
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

		[Column("document")]
		protected abstract string document
		{
			get;
			set;
		}

		public string Document
		{
			get
			{
				return document;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Document cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Document cannot be an empty string");

				document = value;
			}
		}

		public string DocumentPath
		{
			get
			{
				return DocumentDir + Document;
			}
		}
	}
}
