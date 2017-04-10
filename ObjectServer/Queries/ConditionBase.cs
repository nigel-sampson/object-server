using System;
using System.Globalization;

using Nichevo.ObjectServer.Schema;
using Nichevo.ObjectServer.DataAdapter;

namespace Nichevo.ObjectServer.Queries
{
	public abstract class ConditionBase
	{
		protected ConditionBase()
		{
			
		}

		internal virtual string BuildQuery(IQueryBuilder query)
		{
			return String.Empty;
		}

		internal static Type DetermineChildType(IQueryBuilder query, string propertyName)
		{
			string[] properties = propertyName.Split('.');

			TypeSchema schema = SchemaCache.Current.GetSchema(query.QueriedType);

			for(int i = 0; i < properties.Length; i++)
			{
				if(i == properties.Length - 1)
				{
					ChildrenSchema childSchema = schema.FindChildrenSchema(properties[i]);

					if(childSchema == null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate schema for {0}.{1}", schema.Type.FullName, properties[i]));

					return childSchema.ChildType;
				}
				else
				{
					ParentSchema parentSchema = schema.FindParentSchema(properties[i]);

					if(parentSchema == null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate schema for {0}.{1}", schema.Type.FullName, properties[i]));

					schema = SchemaCache.Current.GetSchema(parentSchema.Property.PropertyType);
				}
			}

			return null;
		}

		internal string DeterminePrimaryKey(IQueryBuilder query, string propertyName)
		{
			string[] properties = propertyName.Split('.');
			string table = String.Empty;
			string column = String.Empty;
			
			TypeSchema schema = SchemaCache.Current.GetSchema(query.QueriedType);

			for(int i = 0; i < properties.Length; i++)
			{
				if(i == properties.Length - 1)
				{
					ChildrenSchema childSchema = schema.FindChildrenSchema(properties[i]);

					if(childSchema == null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate schema for {0}.{1}", schema.Type.FullName, properties[i]));

					column = String.Format(CultureInfo.CurrentCulture, query.Context.ColumnFormat, schema.PrimaryKey.ColumnName);
				
					table = String.Format(CultureInfo.CurrentCulture, query.Context.TableFormat, schema.TableName);
				}
				else
				{
					ParentSchema parentSchema = schema.FindParentSchema(properties[i]);

					if(parentSchema == null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate schema for {0}.{1}", schema.Type.FullName, properties[i]));

					schema = SchemaCache.Current.GetSchema(parentSchema.Property.PropertyType);

					query.AddParentJoin(parentSchema);
				}
			}

			return String.Format(CultureInfo.CurrentCulture, query.Context.TableColumnFormat, table, column);
		}

		internal static string DetermineForeginKey(IQueryBuilder query, string propertyName)
		{
			string[] properties = propertyName.Split('.');
			string table = String.Empty;
			string column = String.Empty;
			
			TypeSchema schema = SchemaCache.Current.GetSchema(query.QueriedType);

			for(int i = 0; i < properties.Length; i++)
			{
				if(i == properties.Length - 1)
				{
					ChildrenSchema childSchema = schema.FindChildrenSchema(properties[i]);

					if(childSchema == null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate schema for {0}.{1}", schema.Type.FullName, properties[i]));

					TypeSchema childTypeSchema = SchemaCache.Current.GetSchema(childSchema.ChildType);
					ParentSchema parentSchema = childTypeSchema.FindParentSchema(childSchema.PropertyName);

					if(parentSchema == null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate schema for {0}.{1}", schema.Type.FullName, childSchema.PropertyName));

					column = String.Format(CultureInfo.CurrentCulture, query.Context.ColumnFormat, parentSchema.ColumnName);
				
					table = String.Format(CultureInfo.CurrentCulture, query.Context.TableFormat, childTypeSchema.TableName);
				}
				else
				{
					ParentSchema parentSchema = schema.FindParentSchema(properties[i]);

					if(parentSchema == null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate schema for {0}.{1}", schema.Type.FullName, properties[i]));

					schema = SchemaCache.Current.GetSchema(parentSchema.Property.PropertyType);
				}
			}

			return String.Format(CultureInfo.CurrentCulture, query.Context.TableColumnFormat, table, column);
		}

		internal static string DetermineColumnName(IQueryBuilder query, string propertyName)
		{
			string[] properties = propertyName.Split('.');
			string table = String.Empty;
			string column = String.Empty;

			TypeSchema schema = SchemaCache.Current.GetSchema(query.QueriedType);

			for(int i = 0; i < properties.Length; i++)
			{
				if(i == properties.Length - 1)
				{
					PropertySchema propertySchema = schema.FindPropertySchema(properties[i]);

					if(propertySchema != null)
						column = String.Format(CultureInfo.CurrentCulture, query.Context.ColumnFormat, propertySchema.ColumnName);
					else
					{
						ParentSchema parentSchema = schema.FindParentSchema(properties[i]);

						if(parentSchema != null)
							column = String.Format(CultureInfo.CurrentCulture, query.Context.ColumnFormat, parentSchema.ColumnName);
						else
							throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate schema for {0}.{1}", schema.Type.FullName, properties[i]));
					}

					table = String.Format(CultureInfo.CurrentCulture, query.Context.TableFormat, schema.TableName);
				}
				else
				{
					ParentSchema parentSchema = schema.FindParentSchema(properties[i]);

					if(parentSchema == null)
						throw new ObjectServerException(String.Format(CultureInfo.CurrentCulture, "Could not locate schema for {0}.{1}", schema.Type.FullName, properties[i]));

					schema = SchemaCache.Current.GetSchema(parentSchema.Property.PropertyType);

					query.AddParentJoin(parentSchema);
				}
			}

			return String.Format(CultureInfo.CurrentCulture, query.Context.TableColumnFormat, table, column);
		}
	}
}
