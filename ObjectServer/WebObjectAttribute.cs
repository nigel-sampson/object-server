using System;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Adding this attribute to a field allows the field to be loaded by <see cref="ObjectTransaction.SelectWebObjects">ObjectTransaction.SelectWebObjects</see>
	/// directly from a <see cref="System.Web.HttpRequest">HttpRequest</see>
	/// </summary>
	/// <remarks>
	///	If applied to private fields then a call to <see cref="ObjectTransaction.SelectWebObjects">ObjectTransaction.SelectWebObjects</see> will leave the attribute field as untouched.
	///	Care must be taken not to use ServerObject's from different transactions using this method. 
	/// </remarks>
	/// <example>
	///	In the example below a simple web form contains a <see cref="ServerObject">ServerObject</see> marked with <see cref="WebObjectAttribute">WebObjectAttribute</see>
	///	<code>
	///	public class MyPage : System.Web.UI.Page
	///	{
	///		[WebObject]
	///		protected ServerObject myObject;
	///	}
	///	</code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
	public sealed class WebObjectAttribute : Attribute
	{
		private string requestKey;
		private string defaultValue;
		private bool isRequired;

		/// <summary>
		/// Initialises a new instance of WebObjectAttribute
		/// </summary>
		public WebObjectAttribute()
		{
			requestKey = String.Empty;
			defaultValue = String.Empty;
			isRequired = true;
		}

		/// <summary>
		/// The unique key in the <see cref="System.Web.HttpRequest">HttpRequest</see> that contains
		/// the primary key used to select the attribute field.
		/// </summary>
		/// <remarks>
		/// If the value is the default value <see cref="String.Empty">String.Empty</see> then the name of the 
		/// attribted field is used.
		/// </remarks>
		/// <value>
		/// The unique key in the <see cref="System.Web.HttpRequest">HttpRequest</see> that contains
		/// the primary key used to select the attribute field
		/// The default value is <see cref="String.Empty">String.Empty</see>.
		/// </value>
		public string RequestKey
		{
			get
			{
				return requestKey;
			}
			set
			{
				requestKey = value;
			}
		}

		/// <summary>
		/// Gets or sets the string representation of the value to be used if the unique key is not found in the <see cref="System.Web.HttpRequest">HttpRequest</see>.
		/// </summary>
		/// <value>
		/// The string representation of the value to be used if the unique key is not found in the <see cref="System.Web.HttpRequest">HttpRequest</see>; otherwise <see cref="String.Empty">String.Empty</see>.
		/// The default value is <see cref="String.Empty">String.Empty</see>.
		/// </value>
		public string DefaultValue
		{
			get
			{
				return defaultValue;
			}
			set
			{
				defaultValue = value;
			}
		}

		/// <summary>
		/// Gets or sets whether an <see cref="ObjectServerException">ObjectServerException</see> should be thrown if the <see cref="WebObjectAttribute.RequestKey">RequestKey</see> is not located.
		/// </summary>
		/// <remarks>
		/// If IsRequired is <see langword="true">true</see> then and an the <see cref="WebObjectAttribute.RequestKey">RequestKey</see> is not located and no <see cref="WebObjectAttribute.DefaultValue">DefaultValue</see> then the <see cref="ObjectServerException">ObjectServerException</see> will be thrown.
		/// If IsRequired is <see langword="false">false</see> then the attribute field will be left to its original value.
		/// </remarks>
		/// <value>
		/// <see langword="true">true</see> if an <see cref="ObjectServerException">ObjectServerException</see> should be thrown if the <see cref="WebObjectAttribute.RequestKey">RequestKey</see> is not located; otherwise, <see langword="false">false</see>
		/// The default value is <see langword="true">true</see>
		/// </value>
		public bool IsRequired
		{
			get
			{
				return isRequired;
			}
			set
			{
				isRequired = value;
			}
		}
	}
}
