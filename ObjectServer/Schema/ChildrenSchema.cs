using System;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

namespace Nichevo.ObjectServer.Schema
{
	internal class ChildrenSchema
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("ChildrenSchema", String.Empty);

		private TypeSchema schema;
		private PropertyInfo propInfo;
		private ChildrenAttribute childData;

		public ChildrenSchema(TypeSchema schema, PropertyInfo propInfo, ChildrenAttribute childData)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Creating ChildrenSchema for property {0} and child property {1}", propInfo.Name, childData.PropertyName));		
	
			this.schema = schema;
			this.propInfo = propInfo;
			this.childData = childData;
		}

		public TypeSchema Schema
		{
			get
			{
				return schema;
			}
		}

		public PropertyInfo Property
		{
			get
			{
				return propInfo;
			}
		}

		public string PropertyName
		{
			get
			{
				return childData.PropertyName;
			}
		}

		public Type ChildType
		{
			get
			{
				return childData.ChildType;
			}
		}
	}
}
