using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace Azuro.Data
{
	public class SqlTextCommandHandler
	{
		public static SqlTextCommand Load(SqlTextCommandType t, string location)
		{
			switch (t)
			{
				case SqlTextCommandType.Xml:
				{
					SqlTextCommand xsct = null;
					XmlSerializer xs = new XmlSerializer(typeof(SqlTextCommand));
					using (StreamReader sr = File.OpenText(location))
					{
						xsct = (SqlTextCommand)xs.Deserialize(sr);
					}
					return xsct;
				}
				case SqlTextCommandType.Database:
				{
					return null;
				}
				default:
					return null;
			}
		}
	}
}
