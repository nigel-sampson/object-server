using System;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Specifies the column which holds the primary key for the objects parent.
	/// </summary>
	/// <remarks>
	/// The type of the attributed property must be a valid object for persistance and also the property must be abstract.
	/// </remarks>
	/// <example>
	/// The example shows a property attributed with <see cref="ParentAttribute">ParentAttribute</see>.
	/// <code>
	/// [ParentAttribute("parentColumn")]
	/// public abstract ParentType Parent
	/// {
	///		get;
	///		set;
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class ParentAttribute : Attribute
	{
		private string columnName;
		private bool canBeNull;
		private DeleteAction deleteAction;

		/// <summary>
		/// Initialises a new instance of ParentAttribute
		/// </summary>
		/// <param name="columnName">The name of the column in the database that holds the primary key to the parent object.</param>
		public ParentAttribute(string columnName)
		{
			this.columnName = columnName;
			canBeNull = false;
			deleteAction = DeleteAction.Throw;
		}

		/// <summary>
		/// Gets the name of the column in the database that holds the primary key to the parent object.
		/// </summary>
		/// <value>
		/// The name of the column in the database that holds the primary key to the parent object.
		/// </value>
		public string ColumnName
		{
			get
			{
				return columnName;
			}
		}

		/// <summary>
		/// Gets and sets the action an object should take with regard to its children when deleted.
		/// </summary>
		/// <value>
		/// The action an object should take with regard to its children when deleted, the default is DeleteAction.Throw.
		/// </value>
		public DeleteAction DeleteAction
		{
			get
			{
				return deleteAction;
			}
			set
			{
				deleteAction = value;
			}
		}

		public bool CanBeNull
		{
			get
			{
				return canBeNull;
			}
			set
			{
				canBeNull = value;
			}
		}
	}
}
