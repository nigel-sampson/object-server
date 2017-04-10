using System;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Specifies the a collection of child objects.
	/// </summary>
	/// <remarks>
	/// The type of the attributed property must be an IList.
	/// </remarks>
	/// <example>
	/// The example shows a property attributed with <see cref="ChildrenAttribute">ChildrenAttribute</see>.
	/// <code>
	/// [ChildrenAttribute(typeof(ParentType), "ParentProperty"]
	/// public abstract ServerObjectCollection Children
	/// {
	///		get;
	///		set;
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class ChildrenAttribute : Attribute
	{
		private Type childType;
		private string propertyName;

		/// <summary>
		/// Initialises the new instance of ChildrenAttribute.
		/// </summary>
		/// <param name="childType">The type of object that represents the child data.</param>
		/// <param name="propertyName">The name of the property in the child class definition which is attributed with the matching <see cref="ParentAttribute">ParentAttribute</see>.</param>
		public ChildrenAttribute(Type childType, string propertyName)
		{
			this.childType = childType;
			this.propertyName = propertyName;
		}

		/// <summary>
		/// Gets the type of object that represents the child data.
		/// </summary>
		/// <value>
		/// The type of object that represents the child data.
		/// </value>
		public Type ChildType
		{
			get
			{
				return childType;
			}
		}

		/// <summary>
		/// Gets the name of the property in the child class definition which is attributed with the matching <see cref="ParentAttribute">ParentAttribute</see>.
		/// </summary>
		/// <value>
		/// The name of the property in the child class definition which is attributed with the matching <see cref="ParentAttribute">ParentAttribute</see>.
		/// </value>
		public string PropertyName
		{
			get
			{
				return propertyName;
			}
		}
	}
}
