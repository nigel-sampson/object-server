using System;
using System.IO;
using System.Web;
using System.Xml;
using System.Security;
using System.Collections;
using System.Globalization;
using System.ComponentModel;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Security.Cryptography.Xml;

using Microsoft.Win32;

namespace Nichevo.ObjectServer.License
{
	internal sealed class ServerLicenseProvider : LicenseProvider
	{
		private static readonly ServerLicenseCollector LicenseCollector = new ServerLicenseCollector();

		public ServerLicenseProvider()
		{
	
		}

		public override System.ComponentModel.License GetLicense(LicenseContext context, Type type, object instance, bool allowExceptions) 
		{
			if(context.UsageMode == LicenseUsageMode.Designtime)
				return new ServerLicense(type);

			ServerLicense license = LicenseCollector.GetLicense(type);

			if(license != null)
				return license;

			if(HttpContext.Current != null)
			{
				if(HttpContext.Current.Request.Url.IsLoopback)
					return new ServerLicense(type);
			}

			RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\.NETFramework\AssemblyFolders\ObjectServer");

			if(key == null)
				throw new LicenseException(type, instance, @"Could not locate registry key Software\Microsoft\.NETFramework\AssemblyFolders\ObjectServer");


			if(key.GetValue(String.Empty) == null)
				throw new LicenseException(type, instance, @"Could not retrieve default value for key Software\Microsoft\.NETFramework\AssemblyFolders\ObjectServer");

			string path = key.GetValue(String.Empty) + @"License.xml";

			if(!File.Exists(path))
				throw new LicenseException(type, instance, "Could not locate file " + path);

			XmlDocument xmldoc = new XmlDocument();
			xmldoc.Load(path);

			bool valid = ValidateSignature(xmldoc);

			valid = valid && ValidateLicense(xmldoc);

			if(valid)
				license = new ServerLicense(type);

			if(license != null)
			{
				LicenseCollector.AddLicense(type, license);
				return license;
			}

			if (allowExceptions) 
				throw new LicenseException(type, instance, "Your license is invalid");

			return null;
		}

		private bool ValidateLicense(XmlDocument xmldoc)
		{
			bool validServer = false;
			bool validDomain = false;

			foreach(XmlNode node in xmldoc.SelectNodes("/license/servers/server"))
			{	
				if(String.Compare(node.InnerText, Environment.MachineName, true, CultureInfo.CurrentCulture) == 0)
					validServer = true;
			}

			if(HttpContext.Current != null)
			{
				foreach(XmlNode node in xmldoc.SelectNodes("/license/domains/domain"))
				{	
					if(HttpContext.Current.Request.Url.Host.ToLower(CultureInfo.CurrentCulture).IndexOf(node.InnerText.ToLower(CultureInfo.CurrentCulture)) >= 0)
						validDomain = true;
				}
			}

			return validDomain || validServer;
		}

		private bool ValidateSignature(XmlDocument xmldoc)
		{
			Stream s = null;
			string xmlkey = String.Empty;
			try
			{
				s = typeof(ObjectManager).Assembly.GetManifestResourceStream("Nichevo.ObjectServer.License.Key.xml");

				StreamReader reader = new StreamReader(s);
				xmlkey = reader.ReadToEnd();
				reader.Close();
			}
			catch(Exception ex)
			{
				throw new Exception("Error: could not import public key", ex);
			}

			return true;

//			CspParameters cspParams = new CspParameters(1);
//			cspParams.Flags = CspProviderFlags.UseMachineKeyStore;
//			cspParams.KeyContainerName = "ObjectServerLicense";
//			cspParams.KeyNumber = 2;
//
//			RSACryptoServiceProvider csp = new RSACryptoServiceProvider(cspParams);
//			csp.FromXmlString(xmlkey);
//
//			SignedXml sxml = new SignedXml(xmldoc);
//
//			try
//			{
//				XmlNode dsig = xmldoc.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0];
//				sxml.LoadXml((XmlElement)dsig);
//			}
//			catch
//			{
//				throw new Exception("Error: could not import public key");
//			}
//
//			return sxml.CheckSignature(csp);
		}
	}
}
