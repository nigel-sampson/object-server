using System;
using System.Collections;
using System.Diagnostics;

using Nichevo.ObjectServer.Schema;

namespace Nichevo.ObjectServer
{
	internal class CacheItem : IEnumerable
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("CacheItem", String.Empty);

		private Hashtable objectCache;

		public CacheItem()
		{
			objectCache = new Hashtable();	
		}

		public ServerObject Add(ServerObject obj)
		{
			Type type = obj.ServerObjectType;
			TypeSchema schema = SchemaCache.Current.GetSchema(type);
			object key = obj.Data.GetValue(schema.PrimaryKey.Property.Name);

			Trace.WriteLineIf(DebugOutput.Enabled, "Adding object with key " + key.ToString());

			if(objectCache.ContainsKey(key))
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Object already in cache, returning orginal");
				return objectCache[key] as ServerObject;
			}

			Trace.WriteLineIf(DebugOutput.Enabled, "Adding object to cache");

			objectCache.Add(key, obj);
			return obj;
		}

		public ServerObject Get(object key)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Getting object with key " + key.ToString());

			if(objectCache.ContainsKey(key))
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Found object returning");
				return objectCache[key] as ServerObject;
			}

			Trace.WriteLineIf(DebugOutput.Enabled, "Object not found");
			return null;
		}

		public IEnumerator GetEnumerator()
		{
			return objectCache.Values.GetEnumerator();
		}
	}
}
