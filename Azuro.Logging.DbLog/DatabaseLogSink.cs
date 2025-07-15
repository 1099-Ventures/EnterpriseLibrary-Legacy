using System;
using System.Collections.Generic;
using System.Text;
using Azuro.Data;
using Azuro.Logging.Entities;

namespace Azuro.Logging
{
	/// <summary>
	/// A class to log to the database.
	/// </summary>
	public class DatabaseLogSink : ILogSink
	{
		private const string DataAccessName = "DataObject";
		private const string LogAsync = "LogAsync";

		private DataObject m_do = null;
		private DataObject DataObject { get { return m_do ?? (m_do = DataObject.Create(DataObjectName)); } }
		private string DataObjectName { get; set; }

		/*
			<sink name="DatabaseSink" type="Azuro.Logging.DatabaseLogSink, Azuro.Logging.DbLog">
				<extensions key="DataObject" value="AbsaRewardsAudit" />
				<extensions key="LogAsync" value="false" />
			</sink>
		*/

		#region ILogSink Members

		/// <summary>
		/// Logs the specified logentry.
		/// </summary>
		/// <param name="logentry">The logentry.</param>
		public void Log(LogEntry logentry)
		{
			DataObjectName = Config[DataAccessName];
			if (DataObjectName == null)
				throw new ArgumentNullException("You must set an extension with a [DataObject] key to use this sink.");

			string async = Config[LogAsync];
			bool logAsync = false;
			bool.TryParse(async, out logAsync);
			if (logAsync)
				new WriteLogDelegate(WriteLog).BeginInvoke(logentry, null, null);
			else
				WriteLog(logentry);

		}

		private delegate void WriteLogDelegate(LogEntry logentry);

		private void WriteLog(LogEntry logentry)
		{
			DataObject.Insert(new LogEntryEntity(logentry));
		}

		/// <summary>
		/// Gets or sets the config.
		/// </summary>
		/// <value>The config.</value>
		public CfgLogSink Config { get; set; }

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose() { }

		#endregion
	}
}
