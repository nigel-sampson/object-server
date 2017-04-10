using System;
using System.Security;
using System.Reflection;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

[assembly: AssemblyTitle("Object Server")]
[assembly: AssemblyDescription("Object Persistence Layer")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Nichevo Software")]
[assembly: AssemblyProduct("Object Server")]
[assembly: AssemblyCopyright("2004 Nigel Sampson")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		
[assembly: AssemblyVersion("1.2.0.*")]
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile(@"..\..\..\ObjectServer.snk")]
[assembly: AssemblyKeyName("ObjectServer")]

[assembly: ComVisible(false)]
[assembly: CLSCompliant(true)]

[assembly: ReflectionPermission(SecurityAction.RequestMinimum, Unrestricted = true)]
