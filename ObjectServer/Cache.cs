using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace Nichevo.ObjectServer
{
	internal class Cache : IEnumerable
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("Cache", String.Empty);

		private Hashtable typeCache;

		public Cache()
		{
			typeCache = new Hashtable();	
		}

		public IEnumerator GetEnumerator()
		{
			return typeCache.Values.GetEnumerator();
		}

		public CacheItem GetCacheItem(Type type)
		{
			if(typeCache.ContainsKey(type.FullName))
			{
				return typeCache[type.FullName] as CacheItem;
			}

			CacheItem CacheItem = new CacheItem();
			typeCache.Add(type.FullName, CacheItem);
			return CacheItem;
		}

		public ServerObject Add(ServerObject obj)
		{
			Type type = obj.ServerObjectType;

			Trace.WriteLineIf(DebugOutput.Enabled, "Adding object of type " + type.FullName);
			
			if(typeCache.ContainsKey(type.FullName))
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "CacheItem found");
				CacheItem CacheItem = typeCache[type.FullName] as CacheItem;
				return CacheItem.Add(obj);
			}
			else
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Creating new CacheItem");
				CacheItem CacheItem = new CacheItem();
				typeCache.Add(type.FullName, CacheItem);
				return CacheItem.Add(obj);
			}
		}

		public ServerObject Get(Type type, object key)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Getting objcet of type {0} with key {1}", type.FullName, key));

			if(typeCache.ContainsKey(type.FullName))
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "CacheItem found");
				CacheItem CacheItem = typeCache[type.FullName] as CacheItem;
				return CacheItem.Get(key);
			}
			
			Trace.WriteLineIf(DebugOutput.Enabled, "No CacheItem found");

			return null;
		}
	}
}

