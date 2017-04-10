using System;
using System.Reflection;
using System.Diagnostics;
using System.Globalization;

namespace Nichevo.ObjectServer.Schema
{
	internal class PropertySchema
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("PropertySchema", String.Empty);

		private TypeSchema schema;
		private PropertyInfo propInfo;
		private ColumnAttribute columnData;

		public PropertySchema(TypeSchema schema, PropertyInfo propInfo, ColumnAttribute columnData)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, String.Format(CultureInfo.CurrentCulture, "Creating PropertySchema for property {0} and column {1}", propInfo.Name, columnData.ColumnName));		
	
			this.schema = schema;
			this.propInfo = propInfo;
			this.columnData = columnData;
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
				return columnData.ColumnName;
			}
		}

		public bool CanBeNull
		{
			get
			{
				return columnData.NullValue != null;
			}
		}

		public object NullValue
		{
			get
			{	
				if(typeof(IConvertible).IsAssignableFrom(columnData.NullValue.GetType()))
					return Convert.ChangeType(columnData.NullValue, propInfo.PropertyType, CultureInfo.CurrentCulture);
				else
					return columnData.NullValue;
			}
		}
	}
}
