using System;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

namespace Nichevo.ObjectServer.Schema
{
	internal class ParentSchema
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("ParentSchema", String.Empty);

		private TypeSchema schema;
		private PropertyInfo propInfo;
		private ParentAttribute parentData;

		public ParentSchema(TypeSchema schema, PropertyInfo propInfo, ParentAttribute parentData)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Creating ParentSchema for property {0} and column {1}", propInfo.Name, parentData.ColumnName));		
	
			this.schema = schema;
			this.propInfo = propInfo;
			this.parentData = parentData;
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

		public string ColumnName
		{
			get
			{
				return parentData.ColumnName;
			}
		}

		public DeleteAction DeleteAction
		{
			get
			{
				return parentData.DeleteAction;
			}
		}

		public bool CanBeNull
		{
			get
			{
				return parentData.CanBeNull;
			}
		}
	}
}
