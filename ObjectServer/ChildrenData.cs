using System;
using System.Collections;
using System.Diagnostics;

using Nichevo.ObjectServer.Schema;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Maintains references to a peristed objects Children collections.
	/// </summary>
	/// <remarks>
	/// This object shouldn't really be used but must be exposed by ObjectServer so that the dynamic 
	/// proxies can manipulate an objects data.
	/// </remarks>
	public class ChildrenData
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("ChildrenData", String.Empty);

		private Hashtable data;
		private ServerObject obj;

		internal ChildrenData(ServerObject obj)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Initialing ChildrenData");

			this.obj = obj;
			data = new Hashtable();	
		}

		/// <summary>
		/// Gets the child collection associated with the given property.
		/// </summary>
		/// <param name="key">The name of the property in the <see cref="ServerObject">ServerObject</see> which exposes this child collection.</param>
		/// <value>A <see cref="ServerObjectCollection">ServerObjectCollection</see> that is a child collection or if no children exists an empty collection.</value>
		public ServerObjectCollection this[string key]
		{
			get
			{
				if(data[key] == null)
				{
					Trace.WriteLineIf(DebugOutput.Enabled, "Children not found, loading");

					ServerObjectCollection children = obj.State == ObjectState.Added ? new ServerObjectCollection() : obj.Transaction.SelectChildren(obj, key);
					data[key] = children;

					return children;
				}

				return data[key] as ServerObjectCollection;
			}
		}

		internal ServerObjectCollection GetValue(string key)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Getting value for key " + key);

			ServerObjectCollection children = data[key] as ServerObjectCollection;

			if(children == null && obj.State == ObjectState.Added)
			{
				children = new ServerObjectCollection();
				data[key] = children;
			}

			return children;
		}
	}
}
