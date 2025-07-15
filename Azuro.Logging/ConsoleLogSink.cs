using System;
using System.Collections.Generic;
using System.Text;

namespace Azuro.Logging
{
	class ConsoleLogSink : ILogSink
	{
		private CfgLogSink m_config;

		#region ILogSink Members
		public void Log(LogEntry logentry)
		{
			Console.WriteLine("[{0}]:[{1}]:[{2}]:\t[{3}]", DateTime.Now, logentry.Category, logentry.ThreadId, logentry.Message);
		}

		public CfgLogSink Config
		{
			get { return m_config; }
			set { m_config = value; }
		}
		#endregion

		#region IDisposable Members
		public void Dispose()
		{
			throw new Exception("The method or operation is not implemented.");
		}
		#endregion
	}
}
