using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;

namespace Azuro.Configuration
{
	//	TODO: See SectionInformation.Source

	[Serializable]
	public abstract class AConfigurationSection
	{
		private string m_fileName;

		/// <summary>
		/// A file to load this config section from.
		/// </summary>
		[XmlAttribute("file")]
		[Description("Set this value if the configuration is to be loaded from a file.")]
		public string FileName
		{
			get { return m_fileName; }
			set { m_fileName = value; }
		}
	}
}
