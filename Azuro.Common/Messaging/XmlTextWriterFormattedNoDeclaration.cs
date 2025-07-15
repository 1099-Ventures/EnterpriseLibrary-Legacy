using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Serialization;

namespace Azuro.Common
{
	public class XmlTextWriterFormattedNoDeclaration : XmlTextWriter
	{
		public XmlTextWriterFormattedNoDeclaration(TextWriter w)
			: base(w)
		{
			Formatting = System.Xml.Formatting.Indented;
		}

		public static string Serialize(object o)
		{
			// send clean Xml with no headers or namespaces
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");

			XmlSerializer serializer = new XmlSerializer(o.GetType());
			StringWriter stringWriter = new StringWriter();
			using (XmlWriter writer = new XmlTextWriterFormattedNoDeclaration(stringWriter))
			{
				serializer.Serialize(writer, o, ns);
			}
			return stringWriter.ToString();
		}
	}
}
