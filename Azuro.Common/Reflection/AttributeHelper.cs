using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Azuro.Common
{
	/// <summary>
	/// Static helper class for extracting custom Attributes from classes, properties and fields.
	/// </summary>
	public static class AttributeHelper
	{
		/// <summary>
		/// Gets the custom attribute.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">The t.</param>
		/// <returns></returns>
		public static T GetCustomAttribute<T>(Type t) where T : Attribute
		{
			return GetCustomAttribute<T>(t, false);
		}

		/// <summary>
		/// Gets the custom attribute.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">The t.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		public static T GetCustomAttribute<T>(Type t, bool inherit) where T : Attribute
		{
			T[] a = (T[])t.GetCustomAttributes(typeof(T), inherit);
			if (a != null && a.Length == 1)
				return a[0];

			return null;
		}

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">The t.</param>
		/// <returns></returns>
		public static T[] GetCustomAttributes<T>(Type t) where T : Attribute
		{
			return GetCustomAttributes<T>(t, false);
		}

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">The t.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		public static T[] GetCustomAttributes<T>(Type t, bool inherit) where T : Attribute
		{
			T[] a = (T[])t.GetCustomAttributes(typeof(T), inherit);
			return a;
		}

		/// <summary>
		/// Changed - 2014/12/18 - JU - This looks like it might not be threadsafe, or hashes are not unique
		/// </summary>
		//private static Dictionary<int, object> _attribs = new Dictionary<int, object>();

		/// <summary>
		/// Gets the custom attribute.
		/// </summary>
		/// <typeparam name="T">The type of Attribute to get.</typeparam>
		/// <param name="cap">The cap.</param>
		/// <returns>The Attribute requested, or null if not found.</returns>
		public static T GetCustomAttribute<T>(ICustomAttributeProvider cap) where T : Attribute
		{
			return GetCustomAttribute<T>(cap, false);
		}

		/// <summary>
		/// Gets the custom attribute.
		/// </summary>
		/// <typeparam name="T">The type of Attribute to get.</typeparam>
		/// <param name="cap">The cap.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns>The Attribute requested, or null if not found.</returns>
		public static T GetCustomAttribute<T>(ICustomAttributeProvider cap, bool inherit) where T : Attribute
		{
			//int h1 = typeof(T).GetHashCode();
			//int h2 = cap.GetHashCode();
			//int hash = (((h1 << 5) + h1) ^ h2) + (inherit ? 1 : 0);

			//object val;

			//if (_attribs.TryGetValue(hash, out val))
			//{
			//	return (T)val;
			//}
			//else
			//{
				T tVal;

				T[] a = (T[])cap.GetCustomAttributes(typeof(T), inherit);
				if (a != null && a.Length == 1)
				{
					tVal = a[0];
				}
				else
				{
					tVal = null;
				}

				//_attribs[hash] = tVal;

				return tVal;
			//}
		}

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		/// <typeparam name="T">The type of Attribute to get.</typeparam>
		/// <param name="cap">The cap.</param>
			/// <param name="inherit">if set to <c>true</c> [inherit].</param>
	/// <returns>The Attributes requested, or null if not found.</returns>
		public static T[] GetCustomAttributes<T>(ICustomAttributeProvider cap) where T : Attribute
		{
			return GetCustomAttributes<T>(cap, false);
		}

		/// <summary>
		/// Gets the custom attributes.
		/// </summary>
		/// <typeparam name="T">The type of Attribute to get.</typeparam>
		/// <param name="cap">The cap.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns>The Attributes requested, or null if not found.</returns>
		public static T[] GetCustomAttributes<T>(ICustomAttributeProvider cap, bool inherit) where T : Attribute
		{
			T[] a = (T[])cap.GetCustomAttributes(typeof(T), inherit);
			return a;
		}

		///// <summary>
		///// Gets the custom attributes.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="pi">The pi.</param>
		///// <returns></returns>
		//public static T[] GetCustomAttributes<T>(PropertyInfo pi) where T : Attribute
		//{
		//	return GetCustomAttributes<T>(pi, false);
		//}

		///// <summary>
		///// Gets the custom attributes.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="pi">The pi.</param>
		///// <param name="inherit">if set to <c>true</c> [inherit].</param>
		///// <returns></returns>
		//public static T[] GetCustomAttributes<T>(PropertyInfo pi, bool inherit) where T : Attribute
		//{
		//	T[] a = (T[])pi.GetCustomAttributes(typeof(T), inherit);
		//	return a;
		//}

		///// <summary>
		///// Gets the custom attribute.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="fi">The fi.</param>
		///// <returns></returns>
		//public static T GetCustomAttribute<T>(FieldInfo fi) where T : Attribute
		//{
		//	return GetCustomAttribute<T>(fi, false);
		//}

		///// <summary>
		///// Gets the custom attribute.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="fi">The fi.</param>
		///// <param name="inherit">if set to <c>true</c> [inherit].</param>
		///// <returns></returns>
		//public static T GetCustomAttribute<T>(FieldInfo fi, bool inherit) where T : Attribute
		//{
		//	T[] a = (T[])fi.GetCustomAttributes(typeof(T), inherit);
		//	if (a != null && a.Length == 1)
		//		return a[0];

		//	return null;
		//}

		///// <summary>
		///// Gets the custom attributes.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="fi">The fi.</param>
		///// <returns></returns>
		//public static T[] GetCustomAttributes<T>(FieldInfo fi) where T : Attribute
		//{
		//	return GetCustomAttributes<T>(fi, false);
		//}

		///// <summary>
		///// Gets the custom attributes.
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="fi">The fi.</param>
		///// <param name="inherit">if set to <c>true</c> [inherit].</param>
		///// <returns></returns>
		//public static T[] GetCustomAttributes<T>(FieldInfo fi, bool inherit) where T : Attribute
		//{
		//	T[] a = (T[])fi.GetCustomAttributes(typeof(T), inherit);
		//	return a;
		//}

		///// <summary>
		///// 
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="cap"></param>
		///// <returns></returns>
		//public static T GetCustomAttribute<T>(ICustomAttributeProvider cap) where T : Attribute
		//{
		//	return GetCustomAttribute<T>(cap, false);
		//}

		///// <summary>
		///// 
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="cap"></param>
		///// <param name="inherit"></param>
		///// <returns></returns>
		//public static T GetCustomAttribute<T>(ICustomAttributeProvider cap, bool inherit) where T : Attribute
		//{
		//	T[] a = GetCustomAttributes<T>(cap, inherit);
		//	if (a != null && a.Length == 1)
		//		return a[0];

		//	return null;
		//}

		///// <summary>
		///// 
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="cap"></param>
		///// <returns></returns>
		//public static T[] GetCustomAttributes<T>(ICustomAttributeProvider cap) where T : Attribute
		//{
		//	return GetCustomAttributes<T>(cap, false);
		//}

		///// <summary>
		///// 
		///// </summary>
		///// <typeparam name="T"></typeparam>
		///// <param name="cap"></param>
		///// <param name="inherit"></param>
		///// <returns></returns>
		//public static T[] GetCustomAttributes<T>(ICustomAttributeProvider cap, bool inherit) where T : Attribute
		//{
		//	T[] a = (T[])cap.GetCustomAttributes(typeof(T), inherit);
		//		return a;
		//}

		/// <summary>
		/// Gets the field attribute.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o">The o.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		public static T GetFieldAttribute<T>(object o, string fieldName) where T : Attribute
		{
			return GetFieldAttribute<T>(o, fieldName, false);
		}

		/// <summary>
		/// Gets the field attribute.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o">The o.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		public static T GetFieldAttribute<T>(object o, string fieldName, bool inherit) where T : Attribute
		{
			FieldInfo fi = o.GetType().GetField(fieldName);
			if (fi != null)
				return GetCustomAttribute<T>(fi, inherit);
			return null;
		}

		/// <summary>
		/// Gets the field attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o">The o.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		public static T[] GetFieldAttributes<T>(object o, string fieldName) where T : Attribute
		{
			return GetFieldAttributes<T>(o, fieldName, false);
		}

		/// <summary>
		/// Gets the field attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="o">The o.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		public static T[] GetFieldAttributes<T>(object o, string fieldName, bool inherit) where T : Attribute
		{
			FieldInfo fi = o.GetType().GetField(fieldName);
			if (fi != null)
				return GetCustomAttributes<T>(fi, inherit);
			return null;
		}

		/// <summary>
		/// Gets the field attribute.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">The t.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		public static T GetFieldAttribute<T>(Type t, string fieldName) where T : Attribute
		{
			return GetFieldAttribute<T>(t, fieldName, false);
		}

		/// <summary>
		/// Gets the field attribute.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">The t.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		public static T GetFieldAttribute<T>(Type t, string fieldName, bool inherit) where T : Attribute
		{
			FieldInfo fi = t.GetField(fieldName);
			if (fi != null)
				return GetCustomAttribute<T>(fi, inherit);
			return null;
		}

		/// <summary>
		/// Gets the field attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">The t.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <returns></returns>
		public static T[] GetFieldAttributes<T>(Type t, string fieldName) where T : Attribute
		{
			return GetFieldAttributes<T>(t, fieldName, false);
		}

		/// <summary>
		/// Gets the field attributes.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t">The t.</param>
		/// <param name="fieldName">Name of the field.</param>
		/// <param name="inherit">if set to <c>true</c> [inherit].</param>
		/// <returns></returns>
		public static T[] GetFieldAttributes<T>(Type t, string fieldName, bool inherit) where T : Attribute
		{
			FieldInfo fi = t.GetField(fieldName);
			if (fi != null)
				return GetCustomAttributes<T>(fi, inherit);
			return null;
		}
	}
}
