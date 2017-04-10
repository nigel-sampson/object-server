using System;
using System.Collections;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Represents a collection of <see cref="ServerObject">ServerObject</see> objects.
	/// </summary>
	/// <remarks>
	/// Provides a simple collection object that can represent a set of <see cref="ServerObject">ServerObject</see> objects.
	/// </remarks>
	public class ServerObjectCollection : CollectionBase
	{
		/// <summary>
		/// Initialises a new instances of <see cref="ServerObjectCollection">ServerObjectCollection</see>.
		/// </summary>
		public ServerObjectCollection() : base()
		{			
		}

		/// <summary>
		/// Adds the specified <see cref="ServerObject">ServerObject</see> to the collection.
		/// </summary>
		/// <param name="obj">The <see cref="ServerObject">ServerObject</see> to add.</param>
		/// <returns>The index at which the new element was inserted.</returns>
		public int Add(ServerObject obj)
		{
			return List.Add(obj);
		}

		/// <summary>
		/// Removes the specified <see cref="ServerObject">ServerObject</see> from the collection.
		/// </summary>
		/// <param name="obj">The <see cref="ServerObject">ServerObject</see> to remove from the collection.</param>
		/// <exception cref="ArgumentException">
		/// The specified object is not found in the collection.
		/// </exception>
		public void Remove(ServerObject obj)
		{
			List.Remove(obj);
		}

		/// <summary>
		/// Gets or sets the <see cref="ServerObject">ServerObject</see> at the specified index in the collection.
		/// </summary>
		/// <param name="index">The zero-based index of the collection to access.</param>
		/// <value>A <see cref="ServerObject">ServerObject</see> at each valid index.</value>
		/// <exception cref="ArgumentOutOfRangeException">The index parameter is outside the valid range of indices for the collection.</exception>
		public ServerObject this[int index]
		{
			get
			{
				return List[index] as ServerObject;
			}
			set
			{
				List[index] = value;
			}
		}

		/// <summary>
		/// Copies the collection objects to a one-dimensional <see cref="Array">Array</see> instance beginning at the specified index
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="Array">Array</see> that is the destination of the values copied from the collection. </param>
		/// <param name="index">The index of the array at which to begin inserting. </param>
		public void CopyTo(ServerObject[] array, int index)
		{
			List.CopyTo(array, index);
		}

		/// <summary>
		/// Gets a value indicating whether the collection contains the specified <see cref="ServerObject">ServerObject</see>.
		/// </summary>
		/// <param name="obj">The <see cref="ServerObject">ServerObject</see> to search for in the collection. </param>
		/// <returns><see langword="true">true</see> if the collection contains the specified object; otherwise, <see langword="false">false</see>.</returns>
		public bool Contains(ServerObject obj)
		{
			return List.Contains(obj);
		}

		/// <summary>
		/// Gets the index in the collection of the specified <see cref="ServerObject">ServerObject</see>, if it exists in the collection.
		/// </summary>
		/// <param name="obj">The <see cref="ServerObject">ServerObject</see> to locate in the collection.</param>
		/// <returns>The index in the collection of the specified object, if found; otherwise, -1.</returns>
		public int IndexOf(ServerObject obj)
		{
			return List.IndexOf(obj);
		}

		/// <summary>
		/// Inserts the specified <see cref="ServerObject">ServerObject</see> into the collection at the specified index.
		/// </summary>
		/// <param name="index">The zero-based index where the specified object should be inserted.</param>
		/// <param name="obj">The <see cref="ServerObject">ServerObject</see> to insert. </param>
		public void Insert(int index, ServerObject obj)
		{
			List.Insert(index, obj);
		}
	}
}
