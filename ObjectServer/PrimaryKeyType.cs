using System;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Describes the type of Primary Key an object uses to identify itself in the database.
	/// </summary>
	public enum PrimaryKeyType
	{
		/// <summary>
		/// The object uses an integer as an indentifier and is system defined.
		/// </summary>
		Identity,
		/// <summary>
		/// The object uses a Globally Unique Indentifer and is system defined.
		/// </summary>
		Guid,
		/// <summary>
		/// The object uses a user provided indentifer.
		/// </summary>
		Defined
	}
}
