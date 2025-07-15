using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Azuro.Logging
{
	/// <summary>
	/// The LogEntry class contains information pertaining to a particular log event.
	/// </summary>
	public class LogEntry
	{
		/// <summary>
		/// Gets or sets the sender.
		/// </summary>
		/// <value>The sender.</value>
		public Type Sender { get; set; }
		
		/// <summary>
		/// Gets or sets the category.
		/// </summary>
		/// <value>The category.</value>
		public string Category { get; set; }
		
		/// <summary>
		/// Gets or sets the event id.
		/// </summary>
		/// <value>The event id.</value>
		public int EventId { get; set; }
		
		/// <summary>
		/// Gets or sets the priority.
		/// </summary>
		/// <value>The priority.</value>
		public int Priority { get; set; }
		
		/// <summary>
		/// Gets or sets the severity.
		/// </summary>
		/// <value>The severity.</value>
		public EventLogEntryType Severity { get; set; }

		/// <summary>
		/// Gets or sets the commandline.
		/// </summary>
		/// <value>The commandline for the application that raised the log entry.</value>
		public string Commandline { get; set; }

		/// <summary>
		/// Gets or sets the machine name.
		/// </summary>
		/// <value>The name of the machine that the log was generated on.</value>
		public string MachineName { get; set; }

		/// <summary>
		/// Gets or sets the name of the app domain.
		/// </summary>
		/// <value>The name of the app domain.</value>
		public string AppDomainName { get; set; }

		/// <summary>
		/// Gets or sets the thread id.
		/// </summary>
		/// <value>The thread id.</value>
		public int ThreadId { get; set; }

		/// <summary>
		/// Gets or sets the message.
		/// </summary>
		/// <value>The message.</value>
		public string Message { get; set; }

		/// <summary>
		/// Gets or sets the destination.
		/// </summary>
		/// <value>The destination.</value>
		public DestinationSink Destination { get; set; }

        public Azuro.Logging.Log.Category CategoryType { get; set; }
	}
}
