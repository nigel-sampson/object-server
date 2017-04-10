using System;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

using Nichevo.ObjectServer;

namespace UnitTests.Chelsea
{
	[Table("Users", "Id", PrimaryKeyType.Identity)]
	public abstract class User : ServerObject
	{
		public const int MaxTitleLength = 8;
		public const int MaxNameLength = 50;
		public const int MaxAddressLength = 255;
		public const int MaxPostcodeLength = 10;
		public const int MaxPhoneFaxLength = 50;
		public const int MaxEmailLength = 128;
		public const int MaxBirthMonthLength = 3;
		public const int MaxPasswordLength = 50;

		public const int MaxBookTitleLength = 128;
		public const int MaxBookImageLength = 50;
		
		public const string DateRegisteredNullValue = "01/01/1900";
		public const string BookImagesPath = "~/shared/images/book/";
		public const string StyleImagesPath = "~/shared/images/styles/";

		public const string EmailValidationExpression = Constants.EmailValidationExpression;

		protected User()
		{
			Gender = Gender.Unknown;
			Registered = DateTime.Now;
			IsConfigRegistration = false;
			IsDeleted = false;
			TypeofBirthdayCard = 2;
			IsActivated = false;
			WantsHtmlEmail = true;
			Style = RecipeBookStyle.Traditional;
			BookTitle = String.Empty;
			BookImage = String.Empty;
		}

		[XmlType("User")]
		public class ServiceProxy
		{
			private int id;
			private RecipeBookStyle style;
			private string title;
			private string image;

			public ServiceProxy()
			{
				id = -1;
				style = RecipeBookStyle.Traditional;
				title = String.Empty;
				image = String.Empty;
			}

			public ServiceProxy(User user)
			{
				id = user.Id;
				style = user.Style;
				title = user.BookTitle;
				image = user.BookImage;
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

			public RecipeBookStyle Style
			{
				get
				{
					return style;
				}
				set
				{
					style = value;
				}
			}

			public string Title
			{
				get
				{
					return title;
				}
				set
				{
					title = value;
				}
			}

			public string Image
			{
				get
				{
					return image;
				}
				set
				{
					image = value;
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

		[Column("title")]
		protected abstract string title
		{
			get;
			set;
		}

		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Title cannot be null");

				value = value.Trim();

				if(value.Length > MaxTitleLength)
					throw new ArgumentException(String.Format("Title cannot be more than {0} characters", MaxTitleLength));

				title = value;
			}
		}

		[Column("firstname")]
		protected abstract string firstname
		{
			get;
			set;
		}

		public string Firstname
		{
			get
			{
				return firstname;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Firstname cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Firstname cannot be an empty string");

				if(value.Length > MaxNameLength)
					throw new ArgumentException(String.Format("Firstname cannot be more than {0} characters", MaxNameLength));

				firstname = value;
			}
		}

		[Column("lastname")]
		protected abstract string surname
		{
			get;
			set;
		}

		public string Surname
		{
			get
			{
				return surname;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Surname cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Surname cannot be an empty string");

				if(value.Length > MaxNameLength)
					throw new ArgumentException(String.Format("Surname cannot be more than {0} characters", MaxNameLength));

				surname = value;
			}
		}

		[Column("address1")]
		protected abstract string address1
		{
			get;
			set;
		}

		public string Address1
		{
			get
			{
				return address1;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Address1 cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Address1 cannot be an empty string");

				if(value.Length > MaxAddressLength)
					throw new ArgumentException(String.Format("Address1 cannot be more than {0} characters", MaxAddressLength));

				address1 = value;
			}
		}

		[Column("address2", NullValue = "")]
		protected abstract string address2
		{
			get;
			set;
		}

		public string Address2
		{
			get
			{
				return address2;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Address2 cannot be null");

				value = value.Trim();

				if(value.Length > MaxAddressLength)
					throw new ArgumentException(String.Format("Address2 cannot be more than {0} characters", MaxAddressLength));

				address2 = value;
			}
		}

		[Column("postcode", NullValue = "")]
		protected abstract string postcode
		{
			get;
			set;
		}

		public string Postcode
		{
			get
			{
				return postcode;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Postcode cannot be null");

				value = value.Trim();

				if(value.Length > MaxPostcodeLength)
					throw new ArgumentException(String.Format("Postcode cannot be more than {0} characters", MaxPostcodeLength));

				postcode = value;
			}
		}

		[Parent("country")]
		public abstract Country Country
		{
			get;
			set;
		}

		[Column("homephone", NullValue = "")]
		protected abstract string homephone
		{
			get;
			set;
		}

		public string HomePhone
		{
			get
			{
				return homephone;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "HomePhone cannot be null");

				value = value.Trim();

				if(value.Length > MaxPhoneFaxLength)
					throw new ArgumentException(String.Format("HomePhone cannot be more than {0} characters", MaxPhoneFaxLength));

				homephone = value;
			}
		}

		[Column("workphone", NullValue = "")]
		protected abstract string workphone
		{
			get;
			set;
		}

		public string WorkPhone
		{
			get
			{
				return workphone;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "WorkPhone cannot be null");

				value = value.Trim();

				if(value.Length > MaxPhoneFaxLength)
					throw new ArgumentException(String.Format("WorkPhone cannot be more than {0} characters", MaxPhoneFaxLength));

				workphone = value;
			}
		}

		[Column("fax", NullValue = "")]
		protected abstract string fax
		{
			get;
			set;
		}

		public string Fax
		{
			get
			{
				return fax;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Fax cannot be null");

				value = value.Trim();

				if(value.Length > MaxPhoneFaxLength)
					throw new ArgumentException(String.Format("Fax cannot be more than {0} characters", MaxPhoneFaxLength));

				fax = value;
			}
		}

		[Column("email", NullValue = "")]
		protected abstract string email
		{
			get;
			set;
		}

		public string Email
		{
			get
			{
				return email;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Email cannot be null");

				value = value.Trim();

				if(value.Length > MaxEmailLength)
					throw new ArgumentException(String.Format("Email cannot be more than {0} characters", MaxEmailLength));

				if(value.Length > 0)
				{
					Regex expression = new Regex(EmailValidationExpression);

					if(!expression.IsMatch(value))
						throw new ArgumentException("Email is not a valid email address");
				}

				email = value;
			}
		}

		[Column("reg_date", NullValue = DateRegisteredNullValue)]
		public abstract DateTime Registered
		{
			get;
			set;
		}

		[Column("gender")]
		public abstract Gender Gender
		{
			get;
			set;
		}

		[Column("birthday", NullValue = "")]
		protected abstract string birthday
		{
			get;
			set;
		}

		public string BirthDay
		{
			get
			{
				return birthday;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "BirthDay cannot be null");

				value = value.Trim();

				if(value.Length > 0)
				{
					int bd = Convert.ToInt32(value);

					if(bd < 1 || bd > 31)
						throw new ArgumentOutOfRangeException("value", bd, "BirthDay must be between 1 and 31");
				}

				birthday = value;
			}
		}

		[Column("birthmonth", NullValue = "")]
		protected abstract string birthmonth
		{
			get;
			set;
		}

		public string BirthMonth
		{
			get
			{
				return birthmonth;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "BirthMonth cannot be null");

				value = value.Trim();

				if(value.Length > MaxBirthMonthLength)
					throw new ArgumentException(String.Format("BirthMonth cannot be more than {0} characters", MaxBirthMonthLength));

				birthmonth = value;
			}
		}

		[Column("birthyear", NullValue = "")]
		protected abstract string birthyear
		{
			get;
			set;
		}

		public string BirthYear
		{
			get
			{
				return birthyear;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "BirthYear cannot be null");

				value = value.Trim();

				if(value.Length > 0)
				{
					int by = Convert.ToInt32(value);

					if(by < 1900 || by > 2000)
						throw new ArgumentOutOfRangeException("value", by, "BirthYear must be between 1900 and 2000");
				}

				birthyear = value;
			}
		}

		[Column("password")]
		protected abstract string password
		{
			get;
			set;
		}

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "Password cannot be null");

				value = value.Trim();

				if(value.Length == 0)
					throw new ArgumentException("Password cannot be an empty string");

				if(value.Length > MaxPasswordLength)
					throw new ArgumentException(String.Format("Password cannot be more than {0} characters", MaxPasswordLength));

				password = value;
			}
		}

		[Column("freerecipe")]
		public abstract bool WantsMail
		{
			get;
			set;
		}

		[Column("promotions")]
		public abstract bool WantsPromotions
		{
			get;
			set;
		}

		[Column("householdsize", NullValue = "")]
		protected abstract string householdSize
		{
			get;
			set;
		}

		public string HouseholdSize
		{
			get
			{
				return householdSize;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "HouseholdSize cannot be null");

				value = value.Trim();

				switch(value)
				{	
					case "1":
					case "2-4":
					case "5+":
						break;
					default:
						throw new ArgumentException("Invalid HouseholdSize");
				};

				householdSize = value;
			}
		}

		[Column("blnConfigRegistration")]
		public abstract bool IsConfigRegistration
		{
			get;
			set;
		}

		[Column("isDeleted")]
		public abstract bool IsDeleted
		{
			get;
			set;
		}

		[Column("birthdayCardTypeToGet")]
		protected abstract int typeofBirthdayCard
		{
			get;
			set;
		}

		public int TypeofBirthdayCard
		{
			get
			{
				return typeofBirthdayCard;
			}
			set
			{
				if(value > 5 || value < 1)
					throw new ArgumentOutOfRangeException("value", "TypeofBirthdayCard must be between 1 and 5");

				typeofBirthdayCard = value;
			}
		}

		[Parent("city")]
		public abstract City City
		{
			get;
			set;
		}

		[Column("blnHTMLEmail")]
		public abstract bool WantsHtmlEmail
		{
			get;
			set;
		}

		[Column("activate")]
		public abstract bool IsActivated
		{
			get;
			set;
		}

		public DateTime DeactivateOn
		{
			get
			{
				return Registered.AddMonths(1);
			}
		}

		[Children(typeof(Recipe), "Author")]
		public abstract ServerObjectCollection Recipes
		{
			get;
		}

		[Children(typeof(Tip), "Author")]
		public abstract ServerObjectCollection Tips
		{
			get;
		}

		[Children(typeof(UserSugerUseMap), "User")]
		public abstract ServerObjectCollection UseMaps
		{
			get;
		}

		[Children(typeof(UserBrandMap), "User")]
		public abstract ServerObjectCollection BrandMaps
		{
			get;
		}

		[Children(typeof(UserSugarTypeMap), "User")]
		public abstract ServerObjectCollection TypeMaps
		{
			get;
		}

		[Children(typeof(SweetnessEntry), "Entrant")]
		public abstract ServerObjectCollection SweetnessEntries
		{
			get;
		}

		[Children(typeof(RecipeBookItem), "User")]
		public abstract ServerObjectCollection RecipeBookItems
		{
			get;
		}

		[Column("bookstyle")]
		public abstract RecipeBookStyle Style
		{
			get;
			set;
		}

		[Column("booktitle")]
		protected abstract string booktitle
		{
			get;
			set;
		}

		public string BookTitle
		{
			get
			{
				return booktitle;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "BookTitle cannot be null");

				value = value.Trim();

				if(value.Length > MaxBookTitleLength)
					throw new ArgumentException(String.Format("BookTitle cannot be more than {0} characters", MaxBookTitleLength));

				booktitle = value;
			}
		}

		[Column("bookimage")]
		protected abstract string bookimage
		{
			get;
			set;
		}

		public string BookImage
		{
			get
			{
				return bookimage;
			}
			set
			{
				if(value == null)
					throw new ArgumentNullException("value", "BookImage cannot be null");

				value = value.Trim();

				if(value.Length > MaxBookImageLength)
					throw new ArgumentException(String.Format("BookImage cannot be more than {0} characters", MaxBookImageLength));

				bookimage = value;
			}
		}
	}
}
