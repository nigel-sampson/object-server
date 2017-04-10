using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

using Nichevo.ObjectServer.Schema;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Maintains references to a peristed objects Parent objects.
	/// </summary>
	/// <remarks>
	/// This object shouldn't really be used but must be exposed by ObjectServer so that the dynamic 
	/// proxies can manipulate an objects data.
	/// </remarks>
	public class ParentData
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("ParentData", String.Empty);

		private Hashtable data;
		private ServerObject obj;

		internal ParentData(ServerObject obj)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Initialising ParentData");
			this.obj = obj;
			data = new Hashtable();	
		}

		private void UpdateState(string key)
		{
			switch(obj.State)
			{
				case ObjectState.Added:
					break;
				case ObjectState.Unchanged:
				case ObjectState.Modified:
					obj.State = ObjectState.Modified;
					Trace.WriteLineIf(DebugOutput.Enabled, "Setting State to ObjectState.Modified");
					break;
				case ObjectState.Deleted:
				case ObjectState.Detached:
					throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Cannot modify {0}.{1}, the object is a deleted or detached object", obj.ServerObjectType.FullName, key));
				default:
					throw new ObjectServerException("Invalid ObjectState found.");
			};
		}

		/// <summary>
		/// Gets or sets the <see cref="ServerObject">ServerObject</see> that is a Parent object.
		/// </summary>
		/// <remarks>
		/// On object access if a parent exists but has not be retrieved by ObjectServer then it will be retrieved and returned.
		/// Otherwise the existing object will be returned. If you wish a null value to be set the assign null to the parent.
		/// When an object is assigned as a parent, ObjectServer will locate the correct children collection and automatically append the current object.
		/// </remarks>
		/// <param name="key">The name of the property in the <see cref="ServerObject">ServerObject</see> which exposes this Parent.</param>
		/// <value>A <see cref="ServerObject">ServerObject</see> that is a Parent object or if no parent exists null.</value>
		public ServerObject this[string key]
		{
			get
			{
				if(data[key] == null)
				{
					Trace.WriteLineIf(DebugOutput.Enabled, "Parent not found, loading");

					TypeSchema schema = SchemaCache.Current.GetSchema(obj.ServerObjectType);
					ParentSchema parentSchema = schema.FindParentSchema(key);

					ServerObject parent = obj.Transaction.Select(parentSchema.Property.PropertyType, obj.Data.GetValue(key));

					data[key] = parent;
					return parent;
				}
				else if(Convert.IsDBNull(data[key]))
				{
					Trace.WriteLineIf(DebugOutput.Enabled, "Key Value is DBNull.Value returning null");
					return null;
				}

				return data[key] as ServerObject;
			}
			set
			{
				if(value == null)
				{
					Trace.WriteLineIf(DebugOutput.Enabled, "Parent set to null, setting key value to DBNull.Value");

					obj.Data.SetValue(key, DBNull.Value);
					data[key] = DBNull.Value;

					UpdateState(key);
				}
				else
				{
					if(!value.Equals(data[key]))
					{
						Trace.WriteLineIf(DebugOutput.Enabled, "New Parent set");

						ServerObject newParent = value as ServerObject;

						ServerObject oldParent = data[key] != null ? data[key] as ServerObject : null;

						TypeSchema schema = SchemaCache.Current.GetSchema(newParent.ServerObjectType);
						object parentKey = newParent.Data.GetValue(schema.PrimaryKey.Property.Name);

						obj.Data.SetValue(key, parentKey);

						data[key] = newParent;
						UpdateState(key);

						foreach(ChildrenSchema childSchema in schema.ChildrenSchemas)
						{
							if(childSchema.PropertyName == key && childSchema.ChildType == obj.ServerObjectType)
							{
								if(oldParent != null)
								{
									Trace.WriteLineIf(DebugOutput.Enabled, "Removing from children collection " + childSchema.Property.Name);
									ServerObjectCollection oldChildren = oldParent.Children.GetValue(childSchema.Property.Name);
									if(oldChildren != null && oldChildren.Contains(obj))
										oldChildren.Remove(obj);
								}

								Trace.WriteLineIf(DebugOutput.Enabled, "Adding to children collection " + childSchema.Property.Name);
								ServerObjectCollection newChildren = newParent.Children.GetValue(childSchema.Property.Name);
								if(newChildren != null && !newChildren.Contains(obj))
									newChildren.Add(obj);

								break;
							}
						}
					}
				}
			}
		}

		internal ServerObject GetValue(string key)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Getting value for key " + key);
			return data[key] as ServerObject;
		}
	}
}
