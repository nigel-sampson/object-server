using System;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;

using Nichevo.ObjectServer.Schema;
using Nichevo.ObjectServer.License;
using Nichevo.ObjectServer.DataAdapter;

namespace Nichevo.ObjectServer
{
	/// <summary>
	/// Component used to create transactions to the data source.
	/// </summary>
	/// <remarks>
	/// The ObjectManager is the equivalent of a database server. Each transaction created by
	/// the ObjectManager has its own dedicated connection to the data source.
	/// </remarks>
	[ToolboxItem(true)]
	[LicenseProvider(typeof(ServerLicenseProvider))]
	public class ObjectManager : Component
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("ObjectServer", String.Empty);

		private ServerType serverType;
		private string connectionString;
		private System.ComponentModel.License license;

		/// <summary>
		/// Initialises a new instance of ObjectManager.
		/// </summary>
		public ObjectManager()
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Initialising ObjectServer");

			ValidateLicense();
		}

		/// <summary>
		/// nitialises a new instance of ObjectManager of the specified ServerType using the given connection string.
		/// </summary>
		/// <param name="serverType">The type of data source to use.</param>
		/// <param name="connectionString">String containing connection info to the data source.</param>
		public ObjectManager(ServerType serverType, string connectionString)
		{
			Trace.WriteLineIf(DebugOutput.Enabled, "Initialising ObjectServer");

			ValidateLicense();

			this.serverType = serverType;
			this.connectionString = connectionString;
		}

		[Conditional("RELEASE")]
		private void ValidateLicense()
		{
			if(license == null)
				license = LicenseManager.Validate(typeof(ObjectManager), this);

			if(license == null)
				throw new LicenseException(typeof(ObjectManager), this, "Your license is invalid");
		}

		/// <summary>
		/// Releases the resources used by the <see cref="ObjectManager">ObjectManager</see>.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(license != null)
				{
					license.Dispose();
					license = null;
				}
			}
		}

		/// <summary>
		/// Gets and sets the type of data source being connected to.
		/// </summary>
		/// <value>
		/// The type of data source being connected to, the default is <see cref="ServerType.SqlServer">SqlServer</see>
		/// </value>
		[Category("Data")]
		[Description("The type of data source being connected to")]
		[DefaultValue(ServerType.SqlServer)]
		public ServerType ObjectServerType
		{
			get
			{
				return serverType;
			}
			set
			{
				serverType = value;
			}
		}

		/// <summary>
		/// Gets and sets the information used to connecto to the data source.
		/// </summary>
		/// <value>
		/// The information used to connecto to the data source.
		/// </value>
		[Category("Data")]
		[Description("Information used to connect to the data source")]
		[RecommendedAsConfigurable(true)]
		[DefaultValue("")]
		public string ConnectionString
		{
			get
			{
				return connectionString;
			}
			set
			{
				connectionString = value;
			}
		}

		/// <summary>
		/// Creates a new transaction for the data source.
		/// </summary>
		/// <remarks>
		/// Once the transaction has been completed you must explcitly commit the transaction by
		/// using the <see cref="ObjectTransaction.Commit">Commit</see> method.
		/// </remarks>
		/// <returns>
		/// A newly created <see cref="ObjectTransaction">ObjectTransaction</see>.
		/// </returns>
		public ObjectTransaction BeginTransaction()
		{
			ValidateLicense();

			Trace.WriteLineIf(DebugOutput.Enabled, "Creating new ObjectTransaction");
		
			return new ObjectTransaction(new ObjectAdapter(serverType, connectionString));
		}

		/// <summary>
		/// Scans the given assembly for suitable types and preloads the schema information.
		/// </summary>
		/// <remarks>
		/// The given assembly will have to be already loaded in the AppDomain for PreloadSchemas to
		/// detect it.
		/// </remarks>
		/// <param name="assemblyName">The name of the assembly to scan.</param>
		public static void PreloadSchemas(string assemblyName)
		{	
			Trace.WriteLineIf(DebugOutput.Enabled, "Preloading Schemas");

			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Checking Assembly " + assembly.GetName().Name);
				if(assembly.GetName().Name != assemblyName)
					continue;

				foreach(Type type in assembly.GetTypes())
				{
					Trace.WriteLineIf(DebugOutput.Enabled, "Checking Type " + type.FullName);
					if(!type.IsSubclassOf(typeof(ServerObject)))
						continue;

					SchemaCache.Current.LoadSchema(type);
				}
			}
		}
	}
}
