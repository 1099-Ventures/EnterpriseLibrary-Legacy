using System;
using System.Collections.Generic;
using System.Text;

namespace Azuro.Logging
{

	/// <summary>
	/// An interface describing the Log Sink. New types of Logs must implement this interface.
	/// </summary>
	public interface ILogSink : IDisposable
	{
		/// <summary>
		/// Logs the specified logentry.
		/// </summary>
		/// <param name="logentry">The logentry.</param>
		void Log(LogEntry logentry);
		/// <summary>
		/// Gets or sets the config.
		/// </summary>
		/// <value>The config.</value>
		CfgLogSink Config { get; set; }
	}
}
