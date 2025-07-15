using System;
using System.Collections.Generic;
using System.Text;
using Azuro.Configuration;
using System.Xml.Serialization;

namespace Azuro.Common
{
	[XmlRoot(SectionName)]
	[Serializable]
	public class ReplacementParametersConfigurationSection : AConfigurationSection
	{
		public const string SectionName = "Azuro.ReplacementParameters";

		[XmlArrayItem("Replacement")]
		public List<ReplacementTagConfigurationSection> Tags { get; set; }

		public ReplacementParametersConfigurationSection()
		{
			Tags = new List<ReplacementTagConfigurationSection>();
		}
	}

	public enum ReplacementCommonValue
	{
		NotSet			= 0,
		CurrentDateTime,
		RandomInt,
		RandomDecimal
	}

	[Serializable]
	public class ReplacementTagConfigurationSection
	{
		[XmlAttribute("tag")]
		public string Tag { get; set; }
		[XmlAttribute("value")]
		public string Value { get; set; }
		[XmlAttribute("format")]
		public string Format { get; set; }
		[XmlAttribute("commonValue")]
		public ReplacementCommonValue CommonValue { get; set; }

		public ReplacementTagConfigurationSection()
		{
		}

		public ReplacementTagConfigurationSection(string tag, string val)
		{
			Tag = tag;
			Value = val;
		}

		public ReplacementTagConfigurationSection(string tag, ReplacementCommonValue commonValue, string format)
		{
			Tag = tag;
			Value = null;
			Format = format;
			CommonValue = commonValue;
		}
	}
}
