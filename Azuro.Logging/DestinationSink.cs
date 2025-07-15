using System;
using System.Collections.Generic;
using System.Text;

namespace Azuro.Logging
{
	/// <summary>
	/// 
	/// </summary>
	public class DestinationSink
	{
		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the log sink.
		/// </summary>
		/// <value>The log sink.</value>
		public ILogSink LogSink { get; set; }

		/// <summary>
		/// Gets or sets the config.
		/// </summary>
		/// <value>The config.</value>
		public CfgLogDestination Config { get; set; }
	}
}
