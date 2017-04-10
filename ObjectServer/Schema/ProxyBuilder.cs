using System;
using System.Collections;
using System.Reflection;
using System.Diagnostics;
using System.Reflection.Emit;

namespace Nichevo.ObjectServer.Schema
{
	internal sealed class ProxyBuilder
	{
		private static BooleanSwitch DebugOutput = new BooleanSwitch("ProxyBuilder", "");
		private static AssemblyBuilder assembly;
		private static ModuleBuilder module;

		private ProxyBuilder()
		{
			
		}

		static ProxyBuilder()
		{
			AssemblyName name = new AssemblyName();
			name.Name = "ObjectServerProxies";
			assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
			module = assembly.DefineDynamicModule("Proxies");
		}

		public static Type BuildProxy(Type type)
		{	
			Trace.WriteLineIf(DebugOutput.Enabled, "Building proxy for type " + type.FullName);

			TypeBuilder proxy = module.DefineType(String.Format("Nichevo.ObjectServer.Proxies.{0}", type.FullName), TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit, type);	

			MethodAttributes ma = MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.Virtual; 

			foreach(PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				Trace.WriteLineIf(DebugOutput.Enabled, "Examining Property " + propertyInfo.Name);

				string method = String.Empty;
				Type data;

				if(propertyInfo.GetCustomAttributes(typeof(ColumnAttribute), false).Length == 1)
				{
					method = "get_Data";
					data = typeof(ObjectData);
				}
				else if(propertyInfo.GetCustomAttributes(typeof(ParentAttribute), false).Length == 1)
				{
					method = "get_Parents";
					data = typeof(ParentData);
				}
				else if(propertyInfo.GetCustomAttributes(typeof(ChildrenAttribute), false).Length == 1)
				{
					method = "get_Children";
					data = typeof(ChildrenData);
				}
				else
					continue;


				if(propertyInfo.CanRead)
				{
					MethodInfo getMethodInfo = propertyInfo.GetGetMethod(true);
					MethodBuilder getMethod = proxy.DefineMethod(getMethodInfo.Name, getMethodInfo.IsPublic ? ma | MethodAttributes.Public : ma, propertyInfo.PropertyType, new Type[]{});

					Trace.WriteLineIf(DebugOutput.Enabled, "Building " + getMethod.Name);

					ILGenerator getIL = getMethod.GetILGenerator();
				
					getIL.DeclareLocal(propertyInfo.PropertyType);
					getIL.Emit(OpCodes.Ldarg_0);
					getIL.Emit(OpCodes.Call, typeof(ServerObject).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic));
					getIL.Emit(OpCodes.Ldstr, propertyInfo.Name);
					getIL.Emit(OpCodes.Callvirt, data.GetMethod("get_Item"));

					if(propertyInfo.PropertyType.IsValueType)
					{
						getIL.Emit(OpCodes.Unbox, propertyInfo.PropertyType);
						getIL.Emit(OpCodes.Ldobj, propertyInfo.PropertyType);	
					}
					else
					{
						getIL.Emit(OpCodes.Castclass, propertyInfo.PropertyType);
					}

					getIL.Emit(OpCodes.Stloc_0);
					getIL.Emit(OpCodes.Br_S, (byte)0); 
					getIL.Emit(OpCodes.Ldloc_0);
					getIL.Emit(OpCodes.Ret);
				}

				if(propertyInfo.CanWrite)
				{
					MethodInfo setMethodInfo = propertyInfo.GetSetMethod(true);
					MethodBuilder setMethod = proxy.DefineMethod(setMethodInfo.Name, setMethodInfo.IsPublic ? ma | MethodAttributes.Public : ma, typeof(void), new Type[]{propertyInfo.PropertyType});

					Trace.WriteLineIf(DebugOutput.Enabled, "Building " + setMethod.Name);

					ILGenerator setIL = setMethod.GetILGenerator();		
					setIL.Emit(OpCodes.Ldarg_0);
					setIL.Emit(OpCodes.Call, typeof(ServerObject).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic));
					setIL.Emit(OpCodes.Ldstr, propertyInfo.Name);
					setIL.Emit(OpCodes.Ldarg_1);

					if(propertyInfo.PropertyType.IsValueType)
						setIL.Emit(OpCodes.Box, propertyInfo.PropertyType);
					
					setIL.Emit(OpCodes.Callvirt, data.GetMethod("set_Item"));
					setIL.Emit(OpCodes.Ret);
				}
			}
	
			return proxy.CreateType();;
		}
	}
}
