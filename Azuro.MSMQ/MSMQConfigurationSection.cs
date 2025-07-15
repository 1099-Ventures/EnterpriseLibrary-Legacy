using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Azuro.Configuration;
using System.Xml.Serialization;

namespace Azuro.MSMQ
{
	[XmlRoot(MSMQConfigurationSection.SectionName)]
	public class MSMQConfigurationSection : AConfigurationSection
	{
		[XmlElement("Queue")]
		public List<QueueProcessorConfigurationSection> QueueProcessorConfigurations { get; set; }

		public const string SectionName = "Azuro.MSMQ";
	}

	public class MSMQConfigurationSectionHandler : ConfigurationSectionHandler<MSMQConfigurationSection> { }
}
