using System;
using System.Collections;
using System.Diagnostics;

namespace Nichevo.ObjectServer.Schema
{
	internal sealed class SchemaCache
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("SchemaCache", String.Empty);
		private static readonly SchemaCache current = new SchemaCache();

		private Hashtable cache;

		private SchemaCache()
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Initialising SchemaCache");
			
			cache = Hashtable.Synchronized(new Hashtable());
		}

		public static SchemaCache Current
		{
			get
			{
				return current;
			}
		}

		public void LoadSchema(Type type)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Loading TypeSchema for " + type.FullName);

			lock(cache.SyncRoot)
			{
				if(!cache.ContainsKey(type.FullName))
				{
					Trace.WriteLineIf(DebugOutput.Enabled, "TypeSchema not located, constructing and adding to cache");
					cache.Add(type.FullName, new TypeSchema(type));
				}
			}
		}

		public TypeSchema GetSchema(Type type)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Locating TypeSchema for " + type.FullName);

			lock(cache.SyncRoot)
			{
				if(!cache.ContainsKey(type.FullName))
				{
					Trace.WriteLineIf(DebugOutput.Enabled, "TypeSchema not located, constructing and adding to cache");
					cache.Add(type.FullName, new TypeSchema(type));
				}

				return cache[type.FullName] as TypeSchema;
			}
		}
	}
}
