using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

namespace Azuro.Logging
{
	/// <summary>
	/// A Log sink that writes to file.
	/// </summary>
	public class FileLogSink : ILogSink
	{
		private CfgLogSink m_cfg;
		private static Mutex m_fileLock = new Mutex();
		#region ILogSink Members

		/// <summary>
		/// Logs the specified logentry.
		/// </summary>
		/// <param name="logentry">The logentry.</param>
		public void Log(LogEntry logentry)
		{
			try
			{
				m_fileLock.WaitOne();
				string mask = Config["Filename"] ?? "log_{%yyyyMMdd%}.log";
				string path = Config["Path"] ?? ".";

				path = Path.Combine(path, mask.Replace("{%yyyyMMdd%}", DateTime.Now.ToString("yyyyMMdd")));

				using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read))
				{
					using (StreamWriter sw = new StreamWriter(fs))
					{
						sw.WriteLine("[{0}]:[{1}]:[{2}]:\t[{3}]", DateTime.Now, logentry.Category, logentry.ThreadId, logentry.Message);
					}
					fs.Close();
				}
			}
			finally
			{
				m_fileLock.ReleaseMutex();
			}
		}

		/// <summary>
		/// Gets or sets the config.
		/// </summary>
		/// <value>The config.</value>
		public CfgLogSink Config
		{
			get { return m_cfg; }
			set { m_cfg = value; }
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
		}

		#endregion
	}
}
