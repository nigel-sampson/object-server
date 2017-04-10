using System;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Desribes the action a ServerObject will take when a parent object is marked for deletetion.
	/// </summary>
	public enum DeleteAction
	{
		/// <summary>
		/// Sets the parent reference in each child to a NULL value.
		/// </summary>
		Null,
		/// <summary>
		/// Cascades the delete to each child. 
		/// </summary>
		Cascade,
		/// <summary>
		/// Throws a <see cref="ObjectServer.ObjectServerException">ObjectServerException</see> if children exist.
		/// </summary>
		Throw
	}
}
