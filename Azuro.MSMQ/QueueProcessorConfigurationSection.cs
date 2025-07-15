using Azuro.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Azuro.MSMQ
{
	public class QueueProcessorConfigurationSection
	{
		[XmlAttribute("queueName")]
		public string Name { get; set; }
		public string ErrorQueueName { get; set; }
		[XmlAttribute("messageType")]
		public string __MessageType { get; set; }
		[XmlAttribute("handlerType")]
		public string __HandlerType { get; set; }
		[XmlIgnore]
		public Type MessageType { get { return Util.SafeTypeLoad(__MessageType, Name); } }
		[XmlIgnore]
		public Type HandlerType { get { return Util.SafeTypeLoad(__HandlerType, Name); } }
	}
}
