using System;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Specifies the table in the system that holds the attribute class's data.
	/// </summary>
	/// <remarks>
	/// Classes attributed with <see cref="TableAttribute">TableAttribute</see> should be abstract and be a sub-class of <see cref="ServerObject">ServerObject</see>.
	/// </remarks>
	/// <example>
	/// The example shows a simple object to be persisted marked with <see cref="TableAttribute">TableAttribute</see>
	/// <code>
	/// [Table("TableName", "PrimaryKeyProperty", PrimaryKeyType.Guid)]
	/// public class MyObject : ServerObject
	/// {
	///		...
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class TableAttribute : Attribute
	{
		private string tableName;
		private string primaryKey;
		private string defaultOrder;
		private PrimaryKeyType keyType;

		/// <summary>
		/// Initialises a new instance of TableAttribute with the specified table name, primary key property and the primary key type.
		/// </summary>
		/// <param name="tableName">The name of the table in the system which holds the data for the attributed class.</param>
		/// <param name="primaryKey">The name of the property in the attributed class which provides a unique identifier for this class.</param>
		/// <param name="keyType">The type of primary key used to identify unique instances of this class.</param>
		public TableAttribute(string tableName, string primaryKey, PrimaryKeyType keyType)
		{
			this.tableName = tableName;
			this.primaryKey = primaryKey;
			this.keyType = keyType;
			defaultOrder = String.Empty;		
		}

		/// <summary>
		/// Gets the name of the table in the system which holds the data for the attributed class.
		/// </summary>
		/// <value>
		///	The name of the table in the system which holds the data for the attributed class
		/// </value>
		public string TableName
		{
			get
			{
				return tableName;
			}
		}

		/// <summary>
		/// Gets the name of the property in the attribute class which provides a unique identifier for this class.
		/// </summary>
		/// <value>
		/// The name of the property in the attribute class which provides a unique identifier for this class.
		/// </value>
		public string PrimaryKey
		{
			get
			{
				return primaryKey;
			}
		}

		/// <summary>
		/// Gets the type of primary key used to identify unique instances of this class.
		/// </summary>
		/// <value>
		/// The type of primary key used to identify unique instances of this class.
		/// </value>
		public PrimaryKeyType KeyType
		{
			get
			{
				return keyType;
			}
		}

		/// <summary>
		/// Gets and sets the string representation of the default order for a collection of the attributed class.
		/// </summary>
		/// <value>
		/// The string representation of the default order for a collection of the attributed class.
		/// </value>
		public string DefaultOrder
		{
			get
			{
				return defaultOrder;
			}
			set
			{
				defaultOrder = value;
			}
		}
	}
}
