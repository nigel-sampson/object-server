using System;

namespace Nichevo.ObjectServer.License
{
	internal sealed class ServerLicense : System.ComponentModel.License
	{
		private Type type;

		public ServerLicense(Type type)
		{
			if ( type == null ) 
				throw new ArgumentNullException("The licensed type reference may not be null.");

			this.type = type;
		}

		public override string LicenseKey 
		{
			get 
			{
				return type.FullName;
			}
		}
		public override void Dispose() 
		{
		}
	}
}
