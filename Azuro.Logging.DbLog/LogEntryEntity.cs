using System;
using System.Collections.Generic;
using System.Text;
using Azuro.Data;
using System.Diagnostics;

namespace Azuro.Logging.Entities
{
	/// <summary>
	/// The entity used to insert the database logs.
	/// </summary>
	[StoredProcedure(ListProcedure = "log.ListLogEntry",
					FetchProcedure = "",
					InsertProcedure = "log.InsertLogEntry",
					UpdateProcedure = "",
					DeleteProcedure = "")]
	public class LogEntryEntity : DataEntityWithIdentityId<long>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="LogEntryEntity"/> class.
		/// </summary>
		/// <param name="le">The le.</param>
		public LogEntryEntity(LogEntry le)
		{
			Category = le.Category;
			AppDomain = le.AppDomainName;
			Message = le.Message;
			EventId = le.EventId;
			Priority = le.Priority;
			Severity = le.Severity;
			Sender = le.Sender != null ? le.Sender.FullName : null;
			ThreadId = le.ThreadId;
			Commandline = le.Commandline;
			MachineName = le.MachineName;
			LogName = le.Destination.Name;
		}

		/// <summary>
		/// Gets or sets the sender.
		/// </summary>
		/// <value>The sender.</value>
		[DatabaseColumn("Sender")]
		public string Sender { get; set; }

		/// <summary>
		/// The category of the entry.
		/// <list type="enum">
		/// <item>Info</item>
		/// <item>Debug</item>
		/// <item>Trace</item>
		/// <item>Error</item>
		/// <item>Warn</item>
		/// <item>Fatal</item>
		/// </list>
		/// </summary>
		[DatabaseColumn("Category")]
		public string Category { get; set; }

		/// <summary>
		/// The EventId of the log entry.
		/// </summary>
		[DatabaseColumn("EventId")]
		public int EventId { get; set; }

		/// <summary>
		/// The priority of the entry.
		/// </summary>
		[DatabaseColumn("Priority")]
		public int Priority { get; set; }

		/// <summary>
		/// The severity of the entry. See <see cref="EventLogEntryType"/> for details.
		/// </summary>
		[DatabaseColumn("Severity")]
		public EventLogEntryType Severity { get; set; }

		/// <summary>
		/// The name of the <see cref="AppDomain"/> the entry executed under.
		/// </summary>
		[DatabaseColumn("AppDomain")]
		public string AppDomain { get; set; }

		/// <summary>
		/// Gets or sets the commandline.
		/// </summary>
		/// <value>The commandline for the application that raised the log entry.</value>
		[DatabaseColumn("Commandline")]
		public string Commandline { get; set; }

		/// <summary>
		/// Gets or sets the machine name.
		/// </summary>
		/// <value>The name of the machine that the log was generated on.</value>
		[DatabaseColumn("MachineName")]
		public string MachineName { get; set; }

		/// <summary>
		/// gets or sets the log sink name.
		/// </summary>
		/// <value>The name of the log sink that generated this call.</value>
		[DatabaseColumn("LogName")]
		public string LogName { get; set; }

		/// <summary>
		/// Gets or sets the thread id.
		/// </summary>
		/// <value>The thread id.</value>
		[DatabaseColumn("ThreadId")]
		public int? ThreadId { get; set; }

		/// <summary>
		/// The log entry message.
		/// </summary>
		[DatabaseColumn("Message")]
		public string Message { get; set; }
	}
}
