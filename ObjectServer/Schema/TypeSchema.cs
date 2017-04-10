using System;
using System.Reflection;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace Nichevo.ObjectServer.Schema
{
	internal class TypeSchema
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("TypeSchema", String.Empty);
		
		private TableAttribute tableData;
		private Hashtable propertySchemas;
		private Hashtable parentSchemas;
		private Hashtable childSchemas;
		private Type proxyType;
		private Type type;

		public TypeSchema(Type type)
		{			
			this.type = type;
			propertySchemas = new Hashtable();
			parentSchemas = new Hashtable();
			childSchemas = new Hashtable();

			Trace.WriteLineIf(DebugOutput.Enabled, "Creating TypeSchema for " + type.FullName);

			if(!type.IsSubclassOf(typeof(ServerObject)))
				throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0} is not a subclass of ServerObject.", type.FullName));

			if(type.GetCustomAttributes(typeof(TableAttribute), false).Length != 1)
				throw new ObjectServerException("Could not locate TableAttribute on type " + type.FullName);

			if(!type.IsAbstract)
				throw new ObjectServerException(type.FullName + " is not an abstract class");

			tableData = (TableAttribute)type.GetCustomAttributes(typeof(TableAttribute), false)[0];

			Debug.Indent();
			Trace.WriteLineIf(DebugOutput.Enabled, "TableName = " + tableData.TableName);
			Trace.WriteLineIf(DebugOutput.Enabled, "PrimaryKey = " + tableData.PrimaryKey);
			Trace.WriteLineIf(DebugOutput.Enabled, "PrimaryKeyType = " + tableData.KeyType);
			Trace.WriteLineIf(DebugOutput.Enabled, "DefaultOrder = " + tableData.DefaultOrder);
			Debug.Unindent();

			PropertyInfo keyProperty = type.GetProperty(tableData.PrimaryKey, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

			if(keyProperty == null)
				throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate the property {0} which is defined as the property for {1}", tableData.PrimaryKey, type.FullName));

			if(tableData.KeyType == PrimaryKeyType.Identity && keyProperty.PropertyType != typeof(int))
				throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "KeyType equals PrimaryKeyType.Identity and the property {0}.{1} is not System.Int32", type.FullName, keyProperty.Name));

			if(tableData.KeyType == PrimaryKeyType.Guid && keyProperty.PropertyType != typeof(Guid))
				throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "KeyType equals PrimaryKeyType.Guid and the property {0}.{1} is not System.Guid", type.FullName, keyProperty.Name));

			foreach(PropertyInfo propInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Checking property " + propInfo.Name);
				
				if(propInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Length == 1)
				{
					if(propInfo.GetIndexParameters().Length != 0)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Index properties ({0}.{1}) cannot be attributed with ColumnAttribute", type.FullName, propInfo.Name));

					if(!propInfo.CanRead)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} must have an accessor", type.FullName, propInfo.Name));

					if(!propInfo.GetGetMethod(true).IsAbstract)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} must be abstract", type.FullName, propInfo.Name));

					ColumnAttribute columnData = (ColumnAttribute)propInfo.GetCustomAttributes(typeof(ColumnAttribute), false)[0];

					Debug.Indent();
					Trace.WriteLineIf(DebugOutput.Enabled, "ColumnName = " + columnData.ColumnName);;
					Trace.WriteLineIf(DebugOutput.Enabled, "NullValue = " + columnData.NullValue);
					Debug.Unindent();

					if(propInfo.Name == tableData.PrimaryKey && columnData.NullValue != null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} is marked as the Primary Key and cannot have a NullValue", type.FullName, propInfo.Name));

					propertySchemas.Add(propInfo.Name, new PropertySchema(this, propInfo, columnData));
				}
				else if(propInfo.GetCustomAttributes(typeof(ParentAttribute), false).Length == 1)
				{
					if(propInfo.GetIndexParameters().Length != 0)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Index properties ({0}.{1}) cannot be attributed with ParentAttribute", type.FullName, propInfo.Name));

					if(!propInfo.CanRead)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} must have an accessor", type.FullName, propInfo.Name));

					if(!propInfo.GetGetMethod(true).IsAbstract)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} must be abstract", type.FullName, propInfo.Name));

					ParentAttribute parentData = (ParentAttribute)propInfo.GetCustomAttributes(typeof(ParentAttribute), false)[0];

					if(parentData.DeleteAction == DeleteAction.Null && !parentData.CanBeNull)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} has DeleteAction.Null and CanBeNull is false", type.FullName, propInfo.Name));

					Debug.Indent();
					Trace.WriteLineIf(DebugOutput.Enabled, "ColumnName = " + parentData.ColumnName);
					Trace.WriteLineIf(DebugOutput.Enabled, "ActionOnDelete = " + parentData.DeleteAction);
					Debug.Unindent();

					parentSchemas.Add(propInfo.Name, new ParentSchema(this, propInfo, parentData));
				}
				else if(propInfo.GetCustomAttributes(typeof(ChildrenAttribute), false).Length == 1)
				{
					if(propInfo.GetIndexParameters().Length != 0)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Index properties ({0}.{1}) cannot be attributed with ChildrenAttribute", type.FullName, propInfo.Name));

					if(!propInfo.CanRead)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} must have an accessor", type.FullName, propInfo.Name));

					if(propInfo.CanWrite)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} must be read only", type.FullName, propInfo.Name));

					if(!propInfo.GetGetMethod(true).IsAbstract)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} must be abstract", type.FullName, propInfo.Name));

					if(propInfo.PropertyType != typeof(ServerObjectCollection))
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "{0}.{1} must be of type ServerObjectCollection", type.FullName, propInfo.Name));
					
					ChildrenAttribute childData = (ChildrenAttribute)propInfo.GetCustomAttributes(typeof(ChildrenAttribute), false)[0];

					Debug.Indent();
					Trace.WriteLineIf(DebugOutput.Enabled, "ChildType = " + childData.ChildType.FullName);
					Trace.WriteLineIf(DebugOutput.Enabled, "PropertyName = " + childData.PropertyName);
					Debug.Unindent();

					childSchemas.Add(propInfo.Name, new ChildrenSchema(this, propInfo, childData));
				}
			}

			proxyType = ProxyBuilder.BuildProxy(type);
		}

		public string TableName
		{
			get
			{
				return tableData.TableName;
			}
		}

		public PrimaryKeyType KeyType
		{
			get
			{
				return tableData.KeyType;
			}
		}

		public PropertySchema PrimaryKey
		{
			get
			{
				return propertySchemas[tableData.PrimaryKey] as PropertySchema;
			}
		}

		public string DefaultOrder
		{
			get
			{
				if(tableData.DefaultOrder.Length == 0)
				{
					return String.Format(CultureInfo.CurrentCulture, "{0} ASC", PrimaryKey.Property.Name);
				}
				else
					return tableData.DefaultOrder;
			}
		}

		public ICollection PropertySchemas
		{
			get
			{
				return propertySchemas.Values;
			}
		}

		public PropertySchema FindPropertySchema(string propertyName)
		{
			return propertySchemas[propertyName] as PropertySchema;
		}

		public ICollection ParentSchemas
		{
			get
			{
				return parentSchemas.Values;
			}
		}

		public ParentSchema FindParentSchema(string propertyName)
		{
			return parentSchemas[propertyName] as ParentSchema;
		}

		public ICollection ChildrenSchemas
		{
			get
			{
				return childSchemas.Values;
			}
		}

		public ChildrenSchema FindChildrenSchema(string propertyName)
		{
			return childSchemas[propertyName] as ChildrenSchema;
		}

		public Type Proxy
		{
			get
			{
				return proxyType;
			}
		}

		public Type Type
		{
			get
			{
				return type;
			}
		}
	}
}
