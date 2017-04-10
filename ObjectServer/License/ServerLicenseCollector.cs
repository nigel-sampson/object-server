using System;
using System.Collections;
using System.Collections.Specialized;

namespace Nichevo.ObjectServer.License
{
	internal sealed class ServerLicenseCollector 
	{

		private IDictionary collectedLicenses;

		public ServerLicenseCollector() 
		{
			collectedLicenses = new HybridDictionary();
		}

		public void AddLicense(Type objectType, ServerLicense license) 
		{
			if(objectType == null) 
				throw new ArgumentNullException("objectType");
			if(license == null) 
				throw new ArgumentNullException("objectType");

			collectedLicenses[objectType] = license;
		}

		public ServerLicense GetLicense(Type objectType) 
		{
			if(objectType == null) 
				throw new ArgumentNullException("objectType");

			if(collectedLicenses.Count == 0) 
				return null;

			return (ServerLicense)collectedLicenses[objectType];
		}
	}
}
