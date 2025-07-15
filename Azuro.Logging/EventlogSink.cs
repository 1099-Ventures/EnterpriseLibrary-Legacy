using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;

namespace Azuro.Logging
{
	class EventlogSink : ILogSink
	{
		private CfgLogSink m_config;

		public void Log(LogEntry logentry)
		{
			EventLog.WriteEntry(LogName(logentry), logentry.Message, logentry.Severity, logentry.EventId);
		}

		private string LogName(LogEntry logentry)
		{
			return (logentry.Destination != null && !string.IsNullOrEmpty(logentry.Destination.Name))
				? logentry.Destination.Name :
				!string.IsNullOrEmpty(Config.Name)
				? Config.Name
				: "Azuro.Logging";
		}

		public void Dispose()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public CfgLogSink Config
		{
			get { return m_config ?? (m_config = new CfgLogSink()); }
			set { m_config = value; }
		}
	}
}
