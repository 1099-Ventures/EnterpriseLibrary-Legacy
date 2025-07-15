using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azuro.Common;
using System.Reflection;

namespace Azuro.Common
{
	public class GenericTypeCreator
	{
		public static T CreateObject<T>(string typeName, Type[] types, params object[] args)
		{
			Type t = Type.GetType(typeName);
			Type gt = t.MakeGenericType(types);
			T ot = Util.CreateObject<T>(gt, args);

			return ot;
		}

		public static object CreateObject(string typeName, Type[] types, params object[] args)
		{
			Type t = Type.GetType(typeName);
			Type gt = t.MakeGenericType(types);
			object o = Util.CreateObject(gt, args);

			return o;
		}

		public static object Execute(string typeName, string methodName, Type[] types, object[] args, object[] methodArgs)
		{
			object gt = GenericTypeCreator.CreateObject(typeName, types, args);

			MethodInfo mi = gt.GetType().GetMethod(methodName);

			return mi.Invoke(gt, methodArgs);
		}
	}
}
