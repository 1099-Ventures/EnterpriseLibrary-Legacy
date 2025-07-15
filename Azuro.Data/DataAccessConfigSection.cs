using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Configuration;
using Azuro.Configuration;

namespace Azuro.Data
{
	[XmlRoot("Azuro.Data")]
	public class DataAccessConfigSection : AConfigurationSection
	{
		[XmlElement("DataObject")]
		public List<DataAccessConfigObjectSection> Configs;
		[XmlIgnore]
		public DataAccessConfigObjectSection this[string index]
		{
			get { return Configs.Find(item => string.Compare(item.Name, index, true) == 0); }
		}
	}

	public class DataAccessConfigObjectSection
	{
		[XmlAttribute("name")]
		public string Name;
		[XmlAttribute("assembly")]
		public string Assembly;
		[XmlAttribute("type")]
		public string Type;
		[XmlAttribute("connectionString")]
		public string ConnectionString;
		[XmlAttribute("sqlTextCommandWrapper")]
		public SqlTextCommandType SqlTextCommandWrapper;
		[XmlAttribute("sqlTextCommandLocation")]
		public string SqlTextCommandLocation;
	}

	public class DataAccessConfigSectionHandler : ConfigurationSectionHandler<DataAccessConfigSection>
	{
	}
}
