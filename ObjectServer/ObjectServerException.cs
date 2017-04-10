using System;
using System.Runtime.Serialization;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// The exception that is thrown when ObjectServer returns a warning or error.
	/// </summary>
	[Serializable]
	public class ObjectServerException : ApplicationException
	{
		/// <summary>
		/// Initialises a new instance of the ObjectServerException class.
		/// </summary>
		public ObjectServerException() : base()
		{		
		}

		/// <summary>
		/// Initialises a new instance of the ObjectServerException class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ObjectServerException(string message) : base(message)
		{
		}

		/// <summary>
		/// Initialses a new instance of the ObjectServerException class with a specified error message and
		/// a reference to the inner exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">The error message that explains the reason for this exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception.</param>
		public ObjectServerException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Initialises a new instance of the ObjectServerException class with serialised data.
		/// </summary>
		/// <param name="info">The SerializationInfo that holds the serialised object data about the exception being thrown.</param>
		/// <param name="context">The StreamingContext that contains contextual information about the source or destination.</param>
		protected ObjectServerException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		
		}
	}
}
