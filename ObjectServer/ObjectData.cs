using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Maintains simple data values for the persisted object.
	/// </summary>
	/// <remarks>
	/// This object shouldn't really be used but must be exposed by ObjectServer so that the dynamic 
	/// proxies can manipulate an objects data.
	/// </remarks>
	public class ObjectData
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("ObjectData", String.Empty);

		private Hashtable data;
		private ServerObject obj;

		internal ObjectData(ServerObject obj)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Initialising ObjectData");
			this.obj = obj;
			data = new Hashtable();	
		}

		/// <summary>
		/// Gets or sets the value for the given property.
		/// </summary>
		/// <remarks>
		/// To assign a null value use value defined in the <see cref="ColumnAttribute">ColumnAttribute</see>.
		/// </remarks>
		/// <param name="key">The name of the property in the <see cref="ServerObject">ServerObject</see> which exposes this value.</param>
		/// <value>The value for the property specified by the key</value>
		public object this[string key]
		{
			get
			{
				return data[key];
			}
			set
			{
				if(value == null)
					throw new ObjectServerException(String.Format("Cannot set {0}.{1} to null, use NullValue instead", obj.ServerObjectType.FullName, key));
				
				if(!value.Equals(data[key]))
				{
					Trace.WriteLineIf(DebugOutput.Enabled, "New Property value");

					data[key] = value;

					switch(obj.State)
					{
						case ObjectState.Added:
						case ObjectState.Modified:
							break;
						case ObjectState.Unchanged:
							Trace.WriteLineIf(DebugOutput.Enabled, "Setting State to ObjectState.Modified");
							obj.State = ObjectState.Modified;
							break;
						case ObjectState.Deleted:
						case ObjectState.Detached:
							throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Cannot modify {0}.{1}, the object is a deleted or detached object", obj.ServerObjectType.FullName, key));
						default:
							throw new ObjectServerException("Invalid ObjectState found.");
					};
				}
			}
		}

		internal object GetValue(string key)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Getting value for key " + key);
			return data[key];
		}

		internal void SetValue(string key, object val)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Setting value for key " + key + " to " + val);
			data[key] = val;
		}
	}
}
