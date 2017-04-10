using System;
using System.Collections;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Objects for persistence must sub-class this object.
	/// </summary>
	/// <remarks>
	/// For an object to be persisted by ObjectServer it must inherit from <see cref="ServerObject">ServerObject</see>.
	/// </remarks>
	public class ServerObject
	{
		private ObjectData objectData;
		private ParentData parentData;
		private ChildrenData childrenData;
		private ObjectState state;
		private ObjectTransaction transaction;
		private bool processed;
		private bool visited;

		public event EventHandler Created;
		public event EventHandler Initialised;
		public event EventHandler Validate;
		public event EventHandler Committed;
		public event EventHandler Deleted;

		protected virtual void OnCreated(EventArgs e)
		{
			if(Created != null)
				Created(this, e);
		}

		protected virtual void OnInitialised(EventArgs e)
		{
			if(Initialised != null)
				Initialised(this, e);
		}

		protected virtual void OnValidate(EventArgs e)
		{
			if(Validate != null)
				Validate(this, e);
		}

		protected virtual void OnCommitted(EventArgs e)
		{
			if(Committed != null)
				Committed(this, e);
		}

		protected virtual void OnDeleted(EventArgs e)
		{
			if(Deleted != null)
				Deleted(this, e);
		}

		internal void PromptEvent(EventType eventType)
		{
			switch(eventType)
			{
				case EventType.Created:
					OnCreated(EventArgs.Empty);
					break;
				case EventType.Initialised:
					OnInitialised(EventArgs.Empty);
					break;
				case EventType.Validate:
					OnValidate(EventArgs.Empty);
					break;
				case EventType.Committed:
					OnCommitted(EventArgs.Empty);
					break;
				case EventType.Deleted:
					OnDeleted(EventArgs.Empty);
					break;
			};
		}

		/// <summary>
		/// Initialises a new instance of <see cref="ServerObject">ServerObject</see>.
		/// </summary>
		public ServerObject()
		{
			objectData = new ObjectData(this);
			parentData = new ParentData(this);
			childrenData = new ChildrenData(this);
			processed = false;
			visited = false;
		}

		/// <summary>
		/// Gets a reference to the <see cref="ObjectData">ObjectData</see> object which contains the simple data values for the <see cref="ServerObject">ServerObject</see>.
		/// </summary>
		/// <value>
		/// A reference to the <see cref="ObjectData">ObjectData</see> object which contains the simple data values for the <see cref="ServerObject">ServerObject</see>.
		/// </value>
		protected internal ObjectData Data
		{
			get
			{
				return objectData;
			}
		}

		/// <summary>
		/// Gets a reference to the <see cref="ParentData">ParentData</see> object which contains the parent objects for the <see cref="ServerObject">ServerObject</see>.
		/// </summary>
		/// <value>
		/// A reference to the <see cref="ParentData">ParentData</see> object which contains the parent objects for the <see cref="ServerObject">ServerObject</see>.
		/// </value>
		protected internal ParentData Parents
		{
			get
			{
				return parentData;
			}
		}

		/// <summary>
		/// Gets a reference to the <see cref="ChildrenData">ChildrenData</see> object which contains the child collections for the <see cref="ServerObject">ServerObject</see>.
		/// </summary>
		/// <value>
		/// A reference to the <see cref="ChildrenData">ChildrenData</see> object which contains the child collections for the <see cref="ServerObject">ServerObject</see>.
		/// </value>
		protected internal ChildrenData Children
		{
			get
			{
				return childrenData;
			}
		}

		internal ObjectState State
		{
			get
			{
				return state;
			}
			set
			{
				state = value;
			}
		}

		/// <summary>
		/// Gets a reference to the <see cref="ObjectTransaction">ObjectTransaction</see> reponsible for this <see cref="ServerObject">ServerObject</see>.
		/// </summary>
		/// <value>
		/// A reference to the <see cref="ObjectTransaction">ObjectTransaction</see> reponsible for this <see cref="ServerObject">ServerObject</see>.
		/// </value>
		protected internal ObjectTransaction Transaction
		{
			get
			{
				return transaction;
			}
		}

		internal void SetTransaction(ObjectTransaction transaction)
		{
			this.transaction = transaction;
		}

		internal bool Visited
		{
			get
			{
				return visited;
			}
			set
			{
				visited = value;
			}
		}

		internal bool Processed
		{
			get
			{
				return processed;
			}
			set
			{
				processed = value;
			}
		}

		internal Type ServerObjectType
		{
			get
			{
				return GetType().BaseType;
			}
		}
	}
}
