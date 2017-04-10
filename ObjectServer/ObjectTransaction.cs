using System;
using System.Web;
using System.Globalization;
using System.Reflection;
using System.Diagnostics;

using Nichevo.ObjectServer.Schema;
using Nichevo.ObjectServer.Queries;
using Nichevo.ObjectServer.DataAdapter;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Represent a logical transaction of object manipulations with the data source.
	/// </summary>
	/// <remarks>
	/// An ObjectTransaction allows actions such has
	/// <list type="bullet">
	/// <item>
	///	<term>Create</term>
	///	<description>Initialise a new object with default values and a generated primary key</description>
	/// </item>
	/// <item>
	///	<term>Select</term>
	///	<description>Initialise a new object or collection of objects with values from the data source</description>
	/// </item>
	/// <item>
	///	<term>Delete</term>
	///	<description>Deleting existing objects from the data source</description>
	/// </item>
	/// <item>
	///	<term>Commit</term>
	///	<description>Updating the data source with the operations performed through the transaction</description>
	/// </item>
	/// </list>
	/// An <see cref="ObjectTransaction">ObjectTransaction</see> is created by an <see cref="ObjectManager">ObjectManager</see>, 
	/// and each <see cref="ObjectTransaction">ObjectTransaction</see> has its own dedicated connection to the data source.
	/// <para>
	/// Each ObjectTransaction maintains a cache of the objects loaded so far and trys to resolve references by returning cached objects
	/// first. This means that the ObjectTransaction maintains object identity within the scope of the transaction. This allows object identity
	/// to be compared simple on object reference equality.
	/// </para>
	/// </remarks>
	public class ObjectTransaction
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("ObjectTransaction", String.Empty);

		private ObjectAdapter adapter;
		private Cache objectCache;
		private int identityKey;

		internal ObjectTransaction(ObjectAdapter adapter)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Initialising new ObjectTransaction");

			this.adapter = adapter;
			objectCache = new Cache();
			identityKey = -1;
		}

		/// <summary>
		/// Creates a new peristed object and initialises its primary key.
		/// </summary>
		/// <remarks>
		/// This overload is for objects whose PrimaryKey is of type Identity or Guid (where ObjectServer handles primary key initialisation).
		/// If the primary key type is Defined then this overload will throw an exception.
		/// <para>
		/// The actual objects returned from a call to Create are not of the defined type but of a
		/// proxy class which sub-classes the persisted object and implements the abstract properties.
		/// This is transparent as long as no access to the type occurs. If this is done then it is 
		/// nessecary to access the proxy's base type to get back to the defined type.
		/// </para>
		/// </remarks>
		/// <param name="type">The type of object to create.</param>
		/// <returns>A newly initialised proxy for the specified type.</returns>
		public ServerObject Create(Type type)
		{
			if(type == null)
				throw new ArgumentNullException("type", "type cannot be null.");

			return Create(type, null);
		}

		/// <summary>
		/// Creates a new persisted object with the specified primary key.
		/// </summary>
		/// <param name="type">The type of object to create.</param>
		/// <param name="key">The user defined primary key</param>
		/// <remarks>
		/// This overload is for objects whose PrimaryKey is of type Identity or Guid (where ObjectServer handles primary key initialisation).
		/// If the primary key type is Defined then this overload will throw an exception.
		/// <para>
		/// The actual objects returned from a call to Create are not of the defined type but of a
		/// proxy class which sub-classes the persisted object and implements the abstract properties.
		/// This is transparent as long as no access to the type occurs. If this is done then it is 
		/// nessecary to access the proxy's base type to get back to the defined type.
		/// </para>
		/// </remarks>
		/// <returns>A newly initialised proxy for the specified type.</returns>
		public ServerObject Create(Type type, object key)
		{
			if(type == null)
				throw new ArgumentNullException("type", "type cannot be null.");

			Trace.WriteLineIf(DebugOutput.Enabled, "Creating new ServerObject of type " + type.FullName);

			TypeSchema schema = SchemaCache.Current.GetSchema(type);

			ServerObject obj = Activator.CreateInstance(schema.Proxy) as ServerObject;

			switch(schema.KeyType)
			{
				case PrimaryKeyType.Defined:
					if(key == null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0} KeyType is PrimaryKey.Defined and no has been key passed to ObjectTransaction.Create", type.FullName));
					Trace.WriteLineIf(DebugOutput.Enabled, "Setting key to " + key);
					obj.Data.SetValue(schema.PrimaryKey.Property.Name, key);
					break;
				case PrimaryKeyType.Guid:
					Guid guid = Guid.NewGuid();
					Trace.WriteLineIf(DebugOutput.Enabled, "Setting key to " + guid);
					obj.Data.SetValue(schema.PrimaryKey.Property.Name, guid);
					break;
				case PrimaryKeyType.Identity:
					Trace.WriteLineIf(DebugOutput.Enabled, "Setting key to " + identityKey);
					obj.Data.SetValue(schema.PrimaryKey.Property.Name, identityKey);
					identityKey--;
					break;
				default:
					throw new ObjectServerException("Invalid PrimaryKeyType found.");
			};

			foreach(PropertySchema property in schema.PropertySchemas)
			{
				if(property.CanBeNull)
					obj.Data.SetValue(property.Property.Name, property.NullValue);
			}
	
			obj.State = ObjectState.Added;
			obj.SetTransaction(this);

			objectCache.Add(obj);

			obj.PromptEvent(EventType.Created);
			obj.PromptEvent(EventType.Initialised);

			return obj;
		}

		/// <summary>
		/// Retrieves an object from the data source of the specified type with the specified primary key.
		/// </summary>
		/// <remarks>
		/// Selects a single object from the data source using the given key to locate the values. If no values exist in the data source then Select returns null.
		/// <para>
		/// The actual objects returned from a call to Select are not of the defined type but of a
		/// proxy class which sub-classes the persisted object and implements the abstract properties.
		/// This is transparent as long as no access to the type occurs. If this is done then it is 
		/// nessecary to access the proxy's base type to get back to the defined type.
		/// </para>
		/// </remarks>
		/// <param name="type">The type of object to create.</param>
		/// <param name="key">The primary key which to select the object in the data source.</param>
		/// <returns>A newly initialised proxy for the specified type and with the values given from the data source for the specified primary key.</returns>
		public ServerObject Select(Type type, object key)
		{
			if(type == null)
				throw new ArgumentNullException("type", "type cannot be null.");

			if(key == null)
				return null;

			Trace.WriteLineIf(DebugOutput.Enabled, "Selecting ServerObject of type " + type.FullName);

			ServerObject obj = objectCache.Get(type, key);

			if(obj != null)
				return obj;

			try
			{
				adapter.Open();

				obj = adapter.Select(type, key);
			}
			finally
			{
				adapter.Close();
			}

			if(obj == null)
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "No object found returning null");
				return null;
			}

			obj.State = ObjectState.Unchanged;
			obj.SetTransaction(this);

			objectCache.Add(obj);

			obj.PromptEvent(EventType.Initialised);

			return obj;
		}

		public ServerObject SelectScalar(Type type, Query query)
		{
			query.Top = 1;

			ServerObjectCollection collection = Select(type, query);

			if(collection.Count != 1)
				return null;

			return collection[0];
		}

		public ServerObjectCollection Select(Type type)
		{
			return Select(type, new Query());
		}

		public ServerObjectCollection Select(Type type, Query query)
		{
			if(type == null)
				throw new ArgumentNullException("type", "type cannot be null.");

			if(query == null)
				throw new ArgumentNullException("query", "query cannot be null.");

			ServerObjectCollection collection = new ServerObjectCollection();

			try
			{
				adapter.Open();

				collection = adapter.Select(type, query);
			}
			finally
			{
				adapter.Close();
			}

			for(int i = 0; i < collection.Count; i++)
			{
				collection[i].State = ObjectState.Unchanged;
				collection[i].SetTransaction(this);

				collection[i] = objectCache.Add(collection[i]);

				collection[i].PromptEvent(EventType.Initialised);
			}

			return collection;
		}

		/// <summary>
		/// Marks a object for removal in the transaction.
		/// </summary>
		/// <remarks>
		/// This method can take a lengthy time dependent on its position in the relational graph due to having to 
		/// traverse an object's child objects and either delete or set to null.
		/// <para>
		/// If the object graph traversal encounters an child relation with at least one valid object and ActionOnDelete is
		/// <see cref="DeleteAction.Throw">DeleteAction.Throw</see> then an <see cref="ObjectServerException">ObjectServerException</see>
		/// will be thrown. It is likely that the object graph will be left in an invalid state and it is not advised to <see cref="ObjectTransaction.Commit">Commit</see>
		/// the transaction.
		/// </para>
		/// </remarks>
		/// <param name="obj">The object to be marked for removal.</param>
		public void Delete(ServerObject obj)
		{
			if(obj == null)
				throw new ArgumentNullException("obj", "obj cannot be null.");

			Trace.WriteLineIf(DebugOutput.Enabled, "Deleting object of type " + obj.ServerObjectType);

			DeleteImpl(obj);

			Trace.WriteLineIf(DebugOutput.Enabled, "Clearing Processed Objects");

			foreach(CacheItem cacheItem in objectCache)
			{
				foreach(ServerObject item in cacheItem)
				{
					item.Processed = false;
				}
			}
		}

		private void DeleteImpl(ServerObject obj)
		{
			if(obj.Processed)
				return;

			Trace.WriteLineIf(DebugOutput.Enabled, "Deleting ServerObject of type " + obj.ServerObjectType);

			switch(obj.State)
			{
				case ObjectState.Added:
					Trace.WriteLineIf(DebugOutput.Enabled, "Setting state to ObjectState.Detached");
					obj.State = ObjectState.Detached;
					break;
				case ObjectState.Modified:
				case ObjectState.Unchanged:
					Trace.WriteLineIf(DebugOutput.Enabled, "Setting state to ObjectState.Deleted");
					obj.State = ObjectState.Deleted;
					break;
				case ObjectState.Deleted:
				case ObjectState.Detached:
					break;
				default:
					throw new ObjectServerException("Invalid ObjectState found.");
			};

			obj.Processed = true;

			TypeSchema parentTypeSchema = SchemaCache.Current.GetSchema(obj.ServerObjectType);

			Trace.WriteLineIf(DebugOutput.Enabled, "Enumerating Childrean");

			foreach(ChildrenSchema childSchema in parentTypeSchema.ChildrenSchemas)
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Checking Children " + childSchema.Property.Name);

				ServerObjectCollection children = obj.Children[childSchema.Property.Name] as ServerObjectCollection;

				TypeSchema childTypeSchema = SchemaCache.Current.GetSchema(childSchema.ChildType);

				ParentSchema parentSchema = childTypeSchema.FindParentSchema(childSchema.PropertyName);

				int nonDeleted = 0;

				foreach(ServerObject child in children)
				{
					if(child.State != ObjectState.Deleted && child.State != ObjectState.Detached)
						nonDeleted++;
				}

				Trace.WriteLineIf(DebugOutput.Enabled, "Counted " + nonDeleted + " non-deleted children");

				if(nonDeleted > 0)
				{
					if(parentSchema.DeleteAction == DeleteAction.Throw)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not delete, {0}.{1} has DeleteAction.Throw", childTypeSchema.Type.FullName, parentSchema.Property.Name));

					if(parentSchema.DeleteAction == DeleteAction.Null)
					{
						Trace.WriteLineIf(DebugOutput.Enabled, "Changing key to DBNull.Value and updating state");

						foreach(ServerObject child in children)
						{
							child.Data.SetValue(childSchema.PropertyName, DBNull.Value);
							
							switch(child.State)
							{
								case ObjectState.Added:
								case ObjectState.Modified:
								case ObjectState.Detached:
								case ObjectState.Deleted:
									break;
								case ObjectState.Unchanged:
									child.State = ObjectState.Modified;
									break;
								default:
									throw new ObjectServerException("Invalid ObjectState found.");
							};
						}

						continue;
					}
				}

				Trace.WriteLineIf(DebugOutput.Enabled, "Processing children");

				foreach(ServerObject child in children)
				{
					DeleteImpl(child);
				}
			}
		}

		/// <summary>
		/// Commits the transaction to the data source.
		/// </summary>
		/// <remarks>
		/// Flushes all inserted, updated and deleted objects to the data source. If an error is encountered during the 
		/// object graph traversal an <see cref="ObjectServerException">ObjectServerException</see> will be thrown
		/// an all actions so far will be rolled back, this may leave the object graph in an invalid state so it is not 
		/// advised to Commit the transaction again.
		/// <para>
		/// Commit calculates the correct order of the data source operations including assigning data source assigned keys
		/// to refering child objects.
		/// </para>
		/// </remarks>
		public void Commit()
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Committing ObjectGraph");

			try
			{
				adapter.Open();
				adapter.Begin();

				Trace.WriteLineIf(DebugOutput.Enabled, "Enumerating ServerObjectCache");

				foreach(CacheItem cacheItem in objectCache)
				{
					foreach(ServerObject item in cacheItem)
					{
						Commit(item, false);
					}
				}

				Trace.WriteLineIf(DebugOutput.Enabled, "Committing ObjectGraph Transaction");
				adapter.Commit();
			}
			catch(Exception ex)
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Rolling back ObjectGraph Transaction:" + ex.Message);
				adapter.Rollback();
				throw;
			}
			finally
			{
				adapter.Close();
			}

			foreach(CacheItem cacheItem in objectCache)
			{
				foreach(ServerObject item in cacheItem)
				{
					item.PromptEvent(EventType.Committed);
					item.Processed = false;
					item.Visited = false;
				}
			}
		}

		internal bool Commit(ServerObject obj, bool ignoreVisited)
		{
			if(obj.Processed)
				return true;

			if(obj.Visited && !ignoreVisited)
				return false;

			Trace.WriteLineIf(DebugOutput.Enabled, "Commiting ServerObject of type " + obj.ServerObjectType);

			TypeSchema schema = SchemaCache.Current.GetSchema(obj.ServerObjectType);

			obj.Visited = true;

			switch(obj.State)
			{
				case ObjectState.Added:

					obj.PromptEvent(EventType.Validate);

					if(!CommitParents(obj))
						break;

					if(!obj.Processed)
					{
						Trace.WriteLineIf(DebugOutput.Enabled, "Inserting ServerObject of type " + obj.ServerObjectType);
						adapter.Insert(obj);
						obj.Processed = true;
					}

					CommitChildren(obj, obj.Data.GetValue(schema.PrimaryKey.Property.Name));

					obj.State = ObjectState.Unchanged;
					break;
				case ObjectState.Deleted:
					CommitChildren(obj, null);

					Trace.WriteLineIf(DebugOutput.Enabled, "Deleting ServerObject of type " + obj.ServerObjectType);

					obj.PromptEvent(EventType.Deleted);

					adapter.Delete(obj);
					obj.Processed = true;

					CommitParents(obj);

					obj.State = ObjectState.Detached;
					break;
				case ObjectState.Modified:

					obj.PromptEvent(EventType.Validate);

					if(!CommitParents(obj))
						break;
					
					if(!obj.Processed)
					{
						Trace.WriteLineIf(DebugOutput.Enabled, "Updating ServerObject of type " + obj.ServerObjectType);
						adapter.Update(obj);
						obj.Processed = true;
					}

					CommitChildren(obj, obj.Data.GetValue(schema.PrimaryKey.Property.Name));

					obj.State = ObjectState.Unchanged;
					break;
				case ObjectState.Unchanged:
				case ObjectState.Detached:

					obj.Processed = true;

					break;
				default:
					throw new ObjectServerException("Invalid ObjectState found.");
			};

			return obj.Processed;
		}

		private void CommitChildren(ServerObject obj, object key)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Enumerating children");

			TypeSchema schema = SchemaCache.Current.GetSchema(obj.ServerObjectType);

			if(key != null)
			{
				foreach(ChildrenSchema childSchema in schema.ChildrenSchemas)
				{
					ServerObjectCollection children = obj.Children.GetValue(childSchema.Property.Name);

					if(children == null)
						continue;

					TypeSchema childTypeSchema = SchemaCache.Current.GetSchema(childSchema.ChildType);
					ParentSchema parentSchema = childTypeSchema.FindParentSchema(childSchema.PropertyName);

					foreach(ServerObject child in children)
					{
						if(child.Parents.GetValue(parentSchema.Property.Name) != obj)
							continue;

						child.Data.SetValue(parentSchema.Property.Name, key);
					}
				}
			}

			foreach(ChildrenSchema childSchema in schema.ChildrenSchemas)
			{
				ServerObjectCollection children = obj.Children.GetValue(childSchema.Property.Name);

				if(children == null)
					continue;

				foreach(ServerObject child in children)
				{
					Commit(child, true);
				}
			}
		}

		private bool CommitParents(ServerObject obj)
		{
			bool allProcessed = true;

			Trace.WriteLineIf(DebugOutput.Enabled, "Enumerating parents");

			TypeSchema schema = SchemaCache.Current.GetSchema(obj.ServerObjectType);

			foreach(ParentSchema parentSchema in schema.ParentSchemas)
			{
				ServerObject parent = obj.Parents.GetValue(parentSchema.Property.Name);

				if(parent == null)
					continue;
				
				if(!Commit(parent, false))
					allProcessed = false;
			}

			return allProcessed;
		}

		/// <summary>
		/// Retrives objects based on primary keys in the current <see cref="System.Web.HttpRequest">HttpRequest</see>.
		/// </summary>
		/// <remarks>
		/// Fields marked with <see cref="WebObjectAttribute">WebObjectAttribute</see> must be either public or protected for
		/// them to be initialsied by a call to SelectWebObjects.
		/// </remarks>
		/// <param name="obj">The object containing the attributed fields.</param>
		public void SelectWebObjects(object obj)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Selecting Web Objects");

			if(obj == null)
				throw new ArgumentNullException("obj", "obj cannot be null.");

			if(HttpContext.Current == null)
				throw new ObjectServerException("SelectWebObjects can only be called with in a ASP.NET Web Application");

			HttpRequest request = HttpContext.Current.Request;

			foreach(FieldInfo field in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Examining field " + field.Name);

				if(!field.FieldType.IsSubclassOf(typeof(ServerObject)))
					continue;

				object[] attributes = field.GetCustomAttributes(typeof(WebObjectAttribute), false);
				
				if(attributes.Length != 1)
					continue;

				WebObjectAttribute webAttribute = (WebObjectAttribute)attributes[0];

				object key = request[webAttribute.RequestKey.Length == 0 ? field.Name : webAttribute.RequestKey];

				if(key == null)
				{
					Trace.WriteLineIf(DebugOutput.Enabled, "No value");

					if(webAttribute.DefaultValue.Length != 0)
					{
						key = webAttribute.DefaultValue;
						Trace.WriteLineIf(DebugOutput.Enabled, "Using default value");
					}
					else if(!webAttribute.IsRequired)
					{
						Trace.WriteLineIf(DebugOutput.Enabled, "Not required, ignoring");
						return;
					}
					else
						throw new ObjectServerException("Missing value for field " + field.Name);
				}

				field.SetValue(obj, Select(field.FieldType, key));
			}
		}

		internal ServerObjectCollection SelectChildren(ServerObject obj, string propertyName)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Selecting children for " + propertyName);

			TypeSchema schema = SchemaCache.Current.GetSchema(obj.ServerObjectType);
			ChildrenSchema childrenSchema = schema.FindChildrenSchema(propertyName);

			ServerObjectCollection children = null;

			object primaryKey = obj.Data.GetValue(schema.PrimaryKey.Property.Name);

			Query query = new Query(new Condition(childrenSchema.PropertyName, Expression.Equal, primaryKey));

			children = Select(childrenSchema.ChildType, query);
		
			Trace.WriteLineIf(DebugOutput.Enabled, "Checking cache for more items");

			foreach(ServerObject cacheItem in objectCache.GetCacheItem(childrenSchema.ChildType))
			{
				if(primaryKey.Equals(cacheItem.Data.GetValue(childrenSchema.PropertyName)) && !children.Contains(cacheItem))
					children.Add(cacheItem);
			}

			ServerObjectCollection toRemove = new ServerObjectCollection();

			foreach(ServerObject child in children)
			{
				if(!primaryKey.Equals(child.Data.GetValue(childrenSchema.PropertyName)))
					toRemove.Add(child);
			}

			foreach(ServerObject child in toRemove)
				children.Remove(child);

			return children;
		}
	}
}
