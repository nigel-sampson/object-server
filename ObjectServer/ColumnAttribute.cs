using System;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Specifies the column which holds the value of the attributed property.
	/// </summary>
	/// <remarks>
	/// The property must be abstract. The property given in <see cref="TableAttribute">TableAttribute</see>
	/// must be read only while all others must have at least an accessor.
	/// </remarks>
	/// <example>
	/// The example shows a property attributed with <see cref="ColumnAttribute">ColumnAttribute</see>.
	/// <code>
	/// [ColumnAttribute("nameColumn")]
	/// public abstract stirng Name
	/// {
	///		get;
	///		set;
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class ColumnAttribute : Attribute
	{
		private string columnName;
		private object nullValue;

		/// <summary>
		/// Initialises a new instance of ColumnAttribute
		/// </summary>
		/// <param name="columnName">The name of the column in the database that holds the value for the attributed property.</param>
		public ColumnAttribute(string columnName)
		{
			this.columnName = columnName;
			nullValue = null;
		}

		/// <summary>
		/// Gets the name of the column in the database that holds the value for the attributed property.
		/// </summary>
		/// <value>
		/// The name of the column in the database that holds the value for the attributed property.
		/// </value>
		public string ColumnName
		{
			get
			{
				return columnName;
			}
		}

		/// <summary>
		/// Gets or sets the value to expose when the data store holds a Null value.
		/// </summary>
		/// <remarks>
		/// The value to expose when the data store holds a Null value.
		/// </remarks>
		public object NullValue
		{
			get
			{
				return nullValue;
			}
			set
			{
				nullValue = value;
			}
		}
	}
}
