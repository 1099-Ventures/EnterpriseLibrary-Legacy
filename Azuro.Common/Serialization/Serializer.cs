using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Azuro.Common.Serialization
{
	public class Serializer
	{
		private static Encoding _defaultEncoding = Encoding.Default;

		/// <summary>
		/// This method will serialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="input">The object to serialize.</param>
		/// <returns>An XmlNode containing the serialized data.</returns>
		public static XmlNode SerializeToXmlNode<T>(T input)
		{
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(SerializeToXml(input));
			return doc.DocumentElement;
		}

		/// <summary>
		/// This method will serialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="input">The object to serialize.</param>
		/// <returns>A string containing the serialized data.</returns>
		public static string SerializeToXml<T>(T input)
		{
			return SerializeToXml(input, _defaultEncoding);
		}

		/// <summary>
		/// This method will serialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="input">The type of object to serialize.</param>
		/// <param name="encoding">The encoding to be used for serialization.</param>
		/// <returns>A string containing the serialized data.</returns>
		public static string SerializeToXml<T>(T input, Encoding encoding)
		{
			return SerializeToXml(input, encoding, null);
		}

		/// <summary>
		/// This method will serialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="input">The type of object to serialize.</param>
		/// <param name="extraTypes">Extra types that have to be considered.</param>
		/// <returns>A string containing the serialized data.</returns>
		public static string SerializeToXml<T>(T input, Type[] extraTypes)
		{
			return SerializeToXml(input, _defaultEncoding, extraTypes);
		}

		/// <summary>
		/// This method will serialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="input">The type of object to serialize.</param>
		/// <param name="encoding">The encoding to be used for serialization.</param>
		/// <param name="extraTypes">Extra types that have to be considered.</param>
		/// <returns>A string containing the serialized data.</returns>
		public static string SerializeToXml<T>(T input, Encoding encoding, Type[] extraTypes)
		{
			return SerializeToXml((object)input, encoding, extraTypes);
		}

		/// <summary>
		/// This method will serialise an object using XmlSerializer.
		/// </summary>
		/// <param name="input">The object to serialize.</param>
		/// <param name="encoding">The encoding to be used for serialization.</param>
		/// <param name="extraTypes">Extra types that have to be considered.</param>
		/// <returns>A string containing the serialized data.</returns>
		public static string SerializeToXml(object input, Encoding encoding, Type[] extraTypes)
		{
			XmlSerializer xs = extraTypes == null
							   ? new XmlSerializer(input.GetType())
							   : new XmlSerializer(input.GetType(), extraTypes);
			using (MemoryStream ms = new MemoryStream())
			{
				using (XmlTextWriter xw = new XmlTextWriter(ms, encoding))
				{
					xs.Serialize(xw, input);
					return encoding.GetString(ms.ToArray()).Trim();
				}
			}
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="input">A string representation to deserialize.</param>
		/// <returns>An object of type T.</returns>
		public static T DeserializeXml<T>(string input)
		{
			return (T)DeserializeXml(input, typeof(T), null);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="inStream">A stream to deserialize.</param>
		/// <returns>An object of type T.</returns>
		public static T DeserializeXml<T>(Stream inStream)
		{
			return (T)DeserializeXml(inStream, typeof(T), null);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="inNode">An XmlNode to deserialize.</param>
		/// <returns>An object of type T.</returns>
		public static T DeserializeXml<T>(XmlNode inNode)
		{
			return (T)DeserializeXml(inNode, typeof(T), null);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="tr">A TextReader to deserialize.</param>
		/// <returns>An object of type T.</returns>
		public static T DeserializeXml<T>(TextReader tr)
		{
			return (T)DeserializeXml(tr, typeof(T), null);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="input">A string representation to deserialize.</param>
		/// <param name="extraTypes">The extra types to be evaluated.</param>
		/// <returns>An object of type T.</returns>
		public static T DeserializeXml<T>(string input, Type[] extraTypes)
		{
			return (T)DeserializeXml(input, typeof(T), extraTypes);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="inStream">A stream to deserialize.</param>
		/// <param name="extraTypes">The extra types to be evaluated.</param>
		/// <returns>An object of type T.</returns>
		public static T DeserializeXml<T>(Stream inStream, Type[] extraTypes)
		{
			return (T)DeserializeXml(inStream, typeof(T), extraTypes);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="extraTypes">The extra types to be evaluated.</param>
		/// <param name="inNode">An XmlNode to deserialize.</param>
		/// <returns>An object of type T.</returns>
		public static T DeserializeXml<T>(XmlNode inNode, Type[] extraTypes)
		{
			return (T)DeserializeXml(inNode, typeof(T), extraTypes);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <typeparam name="T">The type of object to serialize.</typeparam>
		/// <param name="tr">A TextReader to deserialize.</param>
		/// <param name="extraTypes">The extra types to be evaluated.</param>
		/// <returns>An object of type T.</returns>
		public static T DeserializeXml<T>(TextReader tr, Type[] extraTypes)
		{
			return (T)DeserializeXml(tr, typeof(T), extraTypes);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <param name="input">A string representation to deserialize.</param>
		/// <param name="type">The type to evaluate.</param>
		/// <returns>An object of type T.</returns>
		public static object DeserializeXml(string input, Type type)
		{
			return DeserializeXml(input, type, null);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <param name="inStream">A stream to deserialize.</param>
		/// <param name="type">The type to evaluate.</param>
		/// <returns>An object of type T.</returns>
		public static object DeserializeXml(Stream inStream, Type type)
		{
			return DeserializeXml(inStream, type, null);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <param name="inNode">An XmlNode to deserialize.</param>
		/// <param name="type">The type to evaluate.</param>
		/// <returns>An object of type T.</returns>
		public static object DeserializeXml(XmlNode inNode, Type type)
		{
			return DeserializeXml(inNode, type, null);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <param name="tr">A TextReader to deserialize.</param>
		/// <param name="type">The type to evaluate.</param>
		/// <returns>An object of type T.</returns>
		public static object DeserializeXml(TextReader tr, Type type)
		{
			return DeserializeXml(tr, type, null);
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <param name="input">A string representation to deserialize.</param>
		/// <param name="type">The type to evaluate.</param>
		/// <param name="extraTypes">The extra types to be evaluated.</param>
		/// <returns>An object of type T.</returns>
		public static object DeserializeXml(string input, Type type, Type[] extraTypes)
		{
			using (StreamReader sr = new StreamReader(new MemoryStream(_defaultEncoding.GetBytes(input)), _defaultEncoding))
			{
				return DeserializeXml(sr, type, extraTypes);
			}
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <param name="inStream">A stream to deserialize.</param>
		/// <param name="type">The type to evaluate.</param>
		/// <param name="extraTypes">The extra types to be evaluated.</param>
		/// <returns>An object of type T.</returns>
		public static object DeserializeXml(Stream inStream, Type type, Type[] extraTypes)
		{
			using (StreamReader sr = new StreamReader(inStream, _defaultEncoding))
			{
				return DeserializeXml(sr, type, extraTypes);
			}
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <param name="inNode">An XmlNode to deserialize.</param>
		/// <param name="type">The type to evaluate.</param>
		/// <param name="extraTypes">The extra types to be evaluated.</param>
		/// <returns>An object of type T.</returns>
		public static object DeserializeXml(XmlNode inNode, Type type, Type[] extraTypes)
		{
			using (StreamReader sr = new StreamReader(inNode.OuterXml, _defaultEncoding))
			{
				return DeserializeXml(sr, type, extraTypes);
			}
		}

		/// <summary>
		/// This method will deserialise an object using XmlSerializer.
		/// </summary>
		/// <param name="tr">A TextReader to deserialize.</param>
		/// <param name="type">The type to evaluate.</param>
		/// <param name="extraTypes">The extra types to be evaluated.</param>
		/// <returns>An object of type T.</returns>
		public static object DeserializeXml(TextReader tr, Type type, Type[] extraTypes)
		{
			XmlSerializer xs = extraTypes == null
				   ? new XmlSerializer(type)
				   : new XmlSerializer(type, extraTypes);

			return xs.Deserialize(tr);
		}
	}
}
