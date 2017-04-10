using System;
using System.Data;
using System.Globalization;

using Nichevo.ObjectServer.Queries;
using Nichevo.ObjectServer.Schema;

namespace Nichevo.ObjectServer.DataAdapter
{
	internal class ObjectAdapter
	{
		private CommandFactory cmdFac;
		private IDataContext context;
		private IDbTransaction trans;

		public ObjectAdapter(ServerType server, string connectionString)
		{
			switch(server)
			{
				case ServerType.SqlServer:
					context = new SqlServerDataContext(connectionString);
					break;
				default:
					throw new ObjectServerException("Invalid ServerType");
			};

			cmdFac = new CommandFactory(context);	
		}

		public void Open()
		{
			context.Connection.Open();
		}

		public void Close()
		{
			context.Connection.Close();
		}

		public void Begin()
		{
			trans =  context.Connection.BeginTransaction(IsolationLevel.ReadCommitted);
		}

		public void Commit()
		{
			if(trans != null)
				trans.Commit();

			trans = null;
		}


		public void Rollback()
		{
			if(trans != null)
				trans.Rollback();

			trans = null;
		}

		public ServerObject Select(Type type, object key)
		{
			IDbCommand cmd = cmdFac.Select(type, key);		
			cmd.Transaction = trans;

			return ObjectFactory.ToObject(type, cmd.ExecuteReader());
		}

		public ServerObjectCollection Select(Type type, Query query)
		{
			IDbCommand cmd = query.CreateCommand(context, type);
			cmd.Transaction = trans;

			return ObjectFactory.ToCollection(type, cmd.ExecuteReader());
		}

		public void Insert(ServerObject obj)
		{
			TypeSchema schema = SchemaCache.Current.GetSchema(obj.ServerObjectType);

			IDbCommand cmd = cmdFac.Insert(obj);
			cmd.Transaction = trans;
			
			if(schema.KeyType == PrimaryKeyType.Identity)
			{
				cmd.ExecuteNonQuery();

				cmd.Parameters.Clear();
				cmd.CommandText = context.IdentitySelect;

				int id = Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.CurrentCulture);
				obj.Data.SetValue(schema.PrimaryKey.Property.Name, id);
			}
			else
				cmd.ExecuteNonQuery();
		}

		public void Update(ServerObject obj)
		{
			IDbCommand cmd = cmdFac.Update(obj);
			cmd.Transaction = trans;
			
			cmd.ExecuteNonQuery();
		}

		public void Delete(ServerObject obj)
		{
			IDbCommand cmd = cmdFac.Delete(obj);
			cmd.Transaction = trans;
			
			cmd.ExecuteNonQuery();
		}
	}
}
