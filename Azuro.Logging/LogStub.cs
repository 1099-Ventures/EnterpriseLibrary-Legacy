using System;
using System.IO;
using System.Linq;
using System.Configuration;
using System.Diagnostics;

using System.Text;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using Azuro.Common;
using System.Collections;
using System.Threading;
using Azuro.Common.Configuration;

namespace Azuro.Logging
{
	// DG 14/04/07 Removed Microsoft Enterprise Library logging.
	//             Created a config section for inclusion in the
	//             server config file and/or service domain config files.
	//             Created two log sinks: Eventlog sink and Trace log sink.
	/// <summary>
	/// A static helper class to for logging messages to various logging sinks.
	/// </summary>
	/// <author>Dietloff Giliomee</author>
	public class Log
	{
		/// <summary>
		/// Log category identifiers.
		/// </summary>
		public enum Category
		{
			/// <summary>
			/// Fatal error message.
			/// </summary>
			Fatal = 0,
			/// <summary>
			/// Error message.
			/// </summary>
			Error,
			/// <summary>
			/// Warning message.
			/// </summary>
			Warn,
			/// <summary>
			/// Information message.
			/// </summary>
			Info,
			/// <summary>
			/// Debug message.
			/// </summary>
			Debug,
			/// <summary>
			/// Trace message.
			/// </summary>
			Trace
		}

		/// <summary>
		/// A struct to map EventLog Categories to PG EntLib Categories.
		/// </summary>
		private class LogCategory
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="LogCategory"/> struct.
			/// </summary>
			/// <param name="_category">The _category.</param>
			/// <param name="_eventid">The _eventid.</param>
			/// <param name="_priority">The _priority.</param>
			/// <param name="_severity">The _severity.</param>
			public LogCategory(string _category, int _eventid, int _priority, EventLogEntryType _severity)
			{
				category = _category;
				eventid = _eventid;
				priority = _priority;
				severity = _severity;
                disabled = false;
			}
			public string category;
			public int eventid;
			public int priority;
            public bool disabled;
			public EventLogEntryType severity;
		}

		/// <summary>
		/// An array of <see cref="LogCategory">Log Categories</see>.
		/// </summary>
		private static LogCategory[] m_logCategory = new LogCategory[]
		{
			new LogCategory("Fatal", 100, 1, EventLogEntryType.Error),
			new LogCategory("Error", 101, 2, EventLogEntryType.Error),
			new LogCategory("Warn",  102, 3, EventLogEntryType.Warning),
			new LogCategory("Info",  103, 4, EventLogEntryType.Information),
			new LogCategory("Debug", 104, 5, EventLogEntryType.Information),
			new LogCategory("Trace", 105, 6, EventLogEntryType.Information)
		};

		/// <summary>
		/// Logs the entry.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <returns></returns>
		private static LogEntry LogEntry(Category category)
		{
			int cat = (int)category;
			LogEntry log = new LogEntry();
			log.Category = m_logCategory[cat].category;
			log.EventId = m_logCategory[cat].eventid;
			log.Priority = m_logCategory[cat].priority;
			log.Severity = m_logCategory[cat].severity;
			log.AppDomainName = AppDomain.CurrentDomain.FriendlyName;
			log.Commandline = Environment.CommandLine;
			log.MachineName = Environment.MachineName;
            log.CategoryType = category;
			return log;
		}

		/// <summary>
		/// Gets the default log source.
		/// </summary>
		/// <value>The default log source.</value>
		private static string DefaultLogSource
		{
			get
			{
				int i = 1;
				while (true)
				{
					StackFrame sf = new StackFrame(i++);
					if (sf.GetMethod() == null)
						break;
					if (sf.GetMethod().DeclaringType != typeof(Log))
						return sf.GetMethod().DeclaringType.ToString();
				}
				return "Azuro.Logging.Log";
			}
		}

		/// <summary>
		/// Write an entry to the Log.
		/// </summary>
		/// <param name="category">The log severity category.</param>
		/// <param name="message">The message to log to the logging interface.</param>
		public static void Write(Category category, string message)
		{
            if ( m_logCategory[(int)category].disabled )
                return;

			LogEntry log = LogEntry(category);
			log.Message = message;
			Write(log);
		}

		/// <summary>
		/// Writes the specified category.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="sender">The sender.</param>
		/// <param name="message">The message.</param>
		public static void Write(Category category, Type sender, string message)
		{
            if ( m_logCategory[(int)category].disabled )
                return;
            
            LogEntry log = LogEntry( category );
			log.Message = message;
			log.Sender = sender;
			Write(log);
		}

		/// <summary>
		/// Writes the specified category.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="message">The message.</param>
		public static void Write(Category category, Exception exception, string message)
		{
            if ( m_logCategory[(int)category].disabled )
                return;
            
            LogEntry log = LogEntry( category );
			log.Message = string.Format("{0}\n{1}", message, Util.UnpackException(exception));
			Write(log);
		}

		/// <summary>
		/// Writes the specified category.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Write(Category category, string format, params object[] args)
		{
            if ( m_logCategory[(int)category].disabled )
                return;
            
            LogEntry log = LogEntry( category );
			log.Message = string.Format(format, args);
			Write(log);
		}

		/// <summary>
		/// Writes the specified category.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="sender">The sender.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Write(Category category, Type sender, string format, params object[] args)
		{
            if ( m_logCategory[(int)category].disabled )
                return;

            LogEntry log = LogEntry(category);
			log.Message = string.Format(format, args);
			log.Sender = sender;
			Write(log);
		}

		/// <summary>
		/// Writes the specified category.
		/// </summary>
		/// <param name="category">The category.</param>
		/// <param name="exception">The exception.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Write(Category category, Exception exception, string format, params object[] args)
		{
            if ( m_logCategory[(int)category].disabled )
                return;
            
            LogEntry log = LogEntry( category );
			log.Message = string.Concat(string.Format(format, args), "\n", Util.UnpackException(exception));
			Write(log);
		}

		/// <summary>
		/// Debugs the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Debug(string message)
		{
			Write(Category.Debug, message);
		}

		/// <summary>
		/// Debugs the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="message">The message.</param>
		public static void Debug(Type sender, string message)
		{
			Write(Category.Debug, sender, message);
		}

		/// <summary>
		/// Debugs the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Debug(string format, params object[] args)
		{
			Write(Category.Debug, format, args);
		}

		/// <summary>
		/// Debugs the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Debug(Type sender, string format, params object[] args)
		{
			Write(Category.Debug, sender, format, args);
		}

		/// <summary>
		/// Debugs the specified log.
		/// </summary>
		/// <param name="log">if set to <c>true</c> [log].</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Debug(bool log, string format, params object[] args)
		{
			if (log)
				Write(Category.Debug, format, args);
		}

		/// <summary>
		/// Debugs the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Debug(Exception exception, string format, params object[] args)
		{
			Write(Category.Debug, exception, format, args);
		}

		/// <summary>
		/// Infoes the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Info(string message)
		{
			Write(Category.Info, message);
		}

		/// <summary>
		/// Infoes the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="message">The message.</param>
		public static void Info(Type sender, string message)
		{
			Write(Category.Info, sender, message);
		}

		/// <summary>
		/// Infoes the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Info(string format, params object[] args)
		{
			Write(Category.Info, format, args);
		}

		/// <summary>
		/// Infoes the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Info(Type sender, string format, params object[] args)
		{
			Write(Category.Info, sender, format, args);
		}

		/// <summary>
		/// Infoes the specified log.
		/// </summary>
		/// <param name="log">if set to <c>true</c> [log].</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Info(bool log, string format, params object[] args)
		{
			if (log)
				Write(Category.Info, format, args);
		}

		/// <summary>
		/// Infoes the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Info(Exception exception, string format, params object[] args)
		{
			Write(Category.Info, exception, format, args);
		}

		/// <summary>
		/// Warns the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Warn(string message)
		{
			Write(Category.Warn, message);
		}

		/// <summary>
		/// Warns the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="message">The message.</param>
		public static void Warn(Type sender, string message)
		{
			Write(Category.Warn, sender, message);
		}

		/// <summary>
		/// Warns the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Warn(string format, params object[] args)
		{
			Write(Category.Warn, format, args);
		}

		/// <summary>
		/// Warns the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Warn(Type sender, string format, params object[] args)
		{
			Write(Category.Warn, sender, format, args);
		}

		/// <summary>
		/// Warns the specified log.
		/// </summary>
		/// <param name="log">if set to <c>true</c> [log].</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Warn(bool log, string format, params object[] args)
		{
			if (log)
				Write(Category.Warn, format, args);
		}

		/// <summary>
		/// Warns the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Warn(Exception exception, string format, params object[] args)
		{
			Write(Category.Warn, exception, format, args);
		}

		/// <summary>
		/// Errors the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Error(string message)
		{
			Write(Category.Error, message);
		}

		/// <summary>
		/// Errors the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="message">The message.</param>
		public static void Error(Type sender, string message)
		{
			Write(Category.Error, sender, message);
		}

		/// <summary>
		/// Errors the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Error(string format, params object[] args)
		{
			Write(Category.Error, format, args);
		}

		/// <summary>
		/// Errors the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Error(Type sender, string format, params object[] args)
		{
			Write(Category.Error, sender, format, args);
		}

		/// <summary>
		/// Errors the specified log.
		/// </summary>
		/// <param name="log">if set to <c>true</c> [log].</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Error(bool log, string format, params object[] args)
		{
			if (log)
				Write(Category.Error, format, args);
		}

		/// <summary>
		/// Errors the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Error(Exception exception, string format, params object[] args)
		{
			Write(Category.Error, exception, format, args);
		}

		/// <summary>
		/// Fatals the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Fatal(string message)
		{
			Write(Category.Fatal, message);
		}

		/// <summary>
		/// Fatals the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="message">The message.</param>
		public static void Fatal(Type sender, string message)
		{
			Write(Category.Fatal, sender, message);
		}

		/// <summary>
		/// Fatals the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Fatal(string format, params object[] args)
		{
			Write(Category.Fatal, format, args);
		}

		/// <summary>
		/// Fatals the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Fatal(Type sender, string format, params object[] args)
		{
			Write(Category.Fatal, sender, format, args);
		}

		/// <summary>
		/// Fatals the specified log.
		/// </summary>
		/// <param name="log">if set to <c>true</c> [log].</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Fatal(bool log, string format, params object[] args)
		{
			if (log)
				Write(Category.Fatal, format, args);
		}

		/// <summary>
		/// Fatals the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Fatal(Exception exception, string format, params object[] args)
		{
			Write(Category.Fatal, exception, format, args);
		}

		/// <summary>
		/// Traces the specified message.
		/// </summary>
		/// <param name="message">The message.</param>
		public static void Trace(string message)
		{
			Write(Category.Trace, message);
		}

		/// <summary>
		/// Traces the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="message">The message.</param>
		public static void Trace(Type sender, string message)
		{
			Write(Category.Trace, sender, message);
		}

		/// <summary>
		/// Traces the specified format.
		/// </summary>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Trace(string format, params object[] args)
		{
			Write(Category.Trace, format, args);
		}

		/// <summary>
		/// Traces the specified sender.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Trace(Type sender, string format, params object[] args)
		{
			Write(Category.Trace, sender, format, args);
		}

		/// <summary>
		/// Traces the specified log.
		/// </summary>
		/// <param name="log">if set to <c>true</c> [log].</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Trace(bool log, string format, params object[] args)
		{
			if (log)
				Write(Category.Trace, format, args);
		}

		/// <summary>
		/// Traces the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="format">The format.</param>
		/// <param name="args">The args.</param>
		public static void Trace(Exception exception, string format, params object[] args)
		{
			Write(Category.Trace, exception, format, args);
		}

		private static Dictionary<string, CategorySink> CategorySinks { get; set; }

		private static CfgLogSinks Config { get; set; }

		/// <summary>
		/// Initializes the <see cref="Log"/> class.
		/// </summary>
		static Log()
		{
			Config = ConfigurationHelper.GetSection<CfgLogSinks>(CfgLogSinks.SectionName);
			if (Config == null)
			{
				//	TODO: Setup default configuration.
				Config = new CfgLogSinks();
			}

			List<ILogSink> logSinks = new List<ILogSink>(Config.LogSinks.Count);
			foreach (CfgLogSink s in Config.LogSinks)
			{
				ILogSink i = CreateLogSink(s.Type);
				if (i != null)
				{
					i.Config = s;
					logSinks.Add(i);
				}
				else
				{
					//	TODO: Weird - Log an error while creating the log class... 
					//	This would be funny, if it wasn't so backwards... :P
				}
			}
			logSinks.TrimExcess();

			// Category sink list.
			CategorySinks = new Dictionary<string, CategorySink>();

			// Keep a list of category sinks to prevent having to wade through
			// the config object every time a log is written.
			foreach (CfgLogCategory cat in Config.LogCategories.LogCategory)
			{
				if (!CategorySinks.ContainsKey(cat.Name))
				{
					List<DestinationSink> lds = new List<DestinationSink>();
					foreach (CfgLogDestination dst in cat.LogDestinations.LogDestination)
					{
						ILogSink s = logSinks.Find(item => string.Compare(dst.Sink, item.Config.Name, true) == 0);
						if (s != null)
						{
							DestinationSink ds = new DestinationSink
							{
								Name = dst.Name,
								LogSink = s,
								Config = dst,
							};
							lds.Add(ds);
						}
					}

					// If a category has no destinations, default to the EventLog.
					if (lds.Count == 0 && Config.LogAllMessages)
						lds.Add(new DestinationSink
						{
							Name = string.Format("{0} => Default Destination", cat.Name),
							Config = null,
							LogSink = new EventlogSink(),
						});

                    LogCategory lc = (from c in m_logCategory where c.category == cat.Name select c).FirstOrDefault();
                    
                    if ( lc != null )
                    {
                        lc.disabled = cat.Disabled;
                    }
                    
					CategorySink cs = new CategorySink
					{
						Name = cat.Name,
						Config = cat,
						LogDestinations = lds,
                        Disabled = cat.Disabled
					};

					CategorySinks.Add(cat.Name, cs);
				}
			}
		}

		/// <summary>
		/// Writes the specified log.
		/// </summary>
		/// <param name="log">The log.</param>
		private static void Write(LogEntry log)
		{
            if ( m_logCategory[(int)log.CategoryType].disabled )
                return;

			//	TODO: Check security rights to interrogate Thread ID
			log.ThreadId = Thread.CurrentThread.ManagedThreadId;

			// If the log category is unconfigured, default to the EventLog.
            if ( !CategorySinks.ContainsKey( log.Category ) )
            {
                if ( Config.LogAllMessages )
                {
                    WriteEventLog( log );
                }
            }
            else
                foreach ( DestinationSink ds in CategorySinks[log.Category].LogDestinations )
                {
                    bool logMessage = Config.LogAllMessages;
                    try
                    {
                        if ( ds.Config != null && !string.IsNullOrEmpty( ds.Config.Type ) )
                        {
                            if ( log.Sender == null )
                                logMessage = false;
                            else
                            {
                                Type t = Type.ReflectionOnlyGetType( ds.Config.Type, false, true );
                                if ( t != null )
                                    logMessage = !ds.Config.Exclude && log.Sender.IsAssignableFrom( t );
                                else
                                    logMessage = !ds.Config.Exclude
                                                && ((string.Compare( log.Sender.FullName, ds.Config.Type, true ) == 0)
                                                || (string.Compare( log.Sender.Name, ds.Config.Type, true ) == 0));
                            }
                        }
                        log.Destination = ds;
                        if ( logMessage )
                        {
                            ds.LogSink.Log( log );
                        }
                    }
                    catch ( Exception ex )
                    {
                        WriteEventLog( ex, log );
                    }
                }
		}

		/// <summary>
		/// Writes the event log.
		/// </summary>
		/// <param name="logEx">The log ex.</param>
		/// <param name="log">The log.</param>
		private static void WriteEventLog(Exception logEx, LogEntry log)
		{
			string message = string.Format("An error occurred while trying to Log the following message to the EventLog:{0}[{1}] => [{2}]{0}",
							Environment.NewLine, log.Severity, log.Message);
			message += Util.UnpackException(logEx);

			try
			{
				EventLog.WriteEntry(DefaultLogSource, message, log.Severity, log.EventId);
			}
			catch (Exception exe)
			{
				if (Config.LogExceptions)
					throw new InvalidOperationException(message, exe);
			}
		}

		/// <summary>
		/// Writes the event log.
		/// </summary>
		/// <param name="log">The log.</param>
		private static void WriteEventLog(LogEntry log)
		{
			try
			{
				EventLog.WriteEntry(DefaultLogSource, log.Message, log.Severity, log.EventId);
			}
			catch (Exception ex)
			{
				if (Config.LogExceptions)
					throw new InvalidOperationException(string.Format("An error occurred while trying to Log the following message to the EventLog:{0}[{1}] => [{2}]",
								Environment.NewLine, log.Severity, log.Message), ex);
			}
		}

		/// <summary>
		/// Creates the log sink.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		private static ILogSink CreateLogSink(string type)
		{
			try
			{
				return Util.CreateObject<ILogSink>(type);
			}
			catch (Exception ex)
			{
				if (Config.LogExceptions)
					EventLog.WriteEntry(DefaultLogSource, ex.Message, EventLogEntryType.Error, m_logCategory[1].eventid, 1);
				else
					throw;
			}
			return null;
		}

		/// <summary>
		/// Unpacks the object.
		/// </summary>
		/// <param name="o">The object to unpack.</param>
		/// <returns></returns>
		[Obsolete("Use Azuro.Common.ObjectInformation.Unpack() or use the Extension method found in Azuro.Common.Extensions (3.5 only)", true)]
		public static string UnpackObject(object o)
		{
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("== Begin Unpack Object [{0}] ==", o.GetType().FullName);
			sb.AppendLine();

			sb.AppendLine("Properties");
			UnpackProperties(sb, o, 1);

			sb.AppendFormat("== End Unpack Object [{0}] ==", o.GetType().FullName);
			sb.AppendLine();

			return sb.ToString();
		}


		/// <summary>
		/// Unpacks the properties.
		/// </summary>
		/// <param name="sb">The sb.</param>
		/// <param name="o">The o.</param>
		/// <param name="depth">The maximum depth.</param>
		private static void UnpackProperties(StringBuilder sb, object o, int depth)
		{
			if (o == null)
				return;

			foreach (PropertyInfo pi in o.GetType().GetProperties())
			{
				object pVal = null;
				ParameterInfo[] parmInfo = pi.GetIndexParameters();
				if (parmInfo.Length > 0)
				{
					UnpackIndexedProperty(sb, pi, parmInfo);
					continue;
				}
				else
				{
					pVal = pi.GetValue(o, null);

					UnpackPropertyValue(sb, pi, pVal);
				}
				
				//	Arbitrary value to avoid Stack Overflow
				//	also skip nulls, strings and datetimes
				if (pVal == null || pVal is string || pVal is DateTime || depth > 3)
					continue;

				if (pVal is IEnumerable)
				{
					foreach (object element in (IEnumerable)pVal)
					{
						UnpackProperties(sb, element, depth + 1);
					}
				}
				else if (!pVal.GetType().IsPrimitive)
				{
					UnpackProperties(sb, pVal, depth + 1);
				}
			}
		}

		/// <summary>
		/// Unpacks the indexed property.
		/// </summary>
		/// <param name="sb">The sb.</param>
		/// <param name="pi">The pi.</param>
		/// <param name="parmInfo">The parm info.</param>
		private static void UnpackIndexedProperty(StringBuilder sb, PropertyInfo pi, ParameterInfo[] parmInfo)
		{
			sb.AppendFormat("[{0}] => [{1}] [", pi.Name, pi.PropertyType);
			foreach (ParameterInfo p in parmInfo)
			{
				sb.AppendFormat("{0} [{1}], ", p.Name, p.ParameterType);
			}
			sb.Remove(sb.Length - 2, 2);
			sb.AppendLine("]");
		}

		/// <summary>
		/// Unpacks the property value.
		/// </summary>
		/// <param name="sb">The sb.</param>
		/// <param name="pi">The pi.</param>
		/// <param name="pVal">The p val.</param>
		private static void UnpackPropertyValue(StringBuilder sb, PropertyInfo pi, object pVal)
		{
			sb.AppendFormat("[{0}] => [{1}]: [{2}]", pi.Name, pi.PropertyType, pVal);
			sb.AppendLine();
		}
	}
}
