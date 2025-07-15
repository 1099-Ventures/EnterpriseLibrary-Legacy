using System;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;
using Azuro.Configuration;
using Azuro.Configuration.Design;
using System.Collections.Generic;

namespace Azuro.Logging
{
	/// <summary>
	/// Logging configuration section handler.
	/// </summary>
	public class LogSinkConfigSectionHandler : ConfigurationSectionHandler<CfgLogSinks>
	{
	}

	/*
	<Azuro.Logging>
	  <sink name="Event Log Sink" type="Azuro.Service.EventlogSink, ServiceLib" />
	  <sink name="Trace Log Sink" type="Azuro.Service.TracelogSink, ServiceLib">
		  <!-- "hostname" can be a comma seperated list -->
		  <extensions key="hostname" value="localhost" /> 
		  <extensions name="port" value="8088" />
	  </sink>
	  <categories>
		<category name="Debug">
		  <destinations>
			<destination name="Trace Log Destination" sink="Trace Log Sink" />
			<destination type="Azuro.Workflow.Activities.WcfOperationActivity, Azuro.Workflow.Activities" name="Log WcfOperation Trace Log Destination" sink="Trace Log Sink" />
			<destination type="Azuro.Workflow.Activities.CallSmActivity, Azuro.Workflow.Activities" exclude="true" name="Exclude Sm Trace Log Destination" />
		  </destinations>
		</category>
		<category name="Error">
		  <destinations>
			<destination name="Event Log Destination" sink="Event Log Sink" />
			<destination name="Trace Log Destination" sink="Trace Log Sink" />
		  </destinations>
		</category>
		<category name="Fatal">
		  <destinations>
			<destination name="Event Log Destination" sink="Event Log Sink" />
			<destination name="Trace Log Destination" sink="Trace Log Sink" />
		  </destinations>
		</category>
		<category name="Info">
		  <destinations>
			<destination name="Event Log Destination" sink="Event Log Sink" />
			<destination name="Trace Log Destination" sink="Trace Log Sink" />
		  </destinations>
		</category>
		<category name="Trace">
		  <destinations>
			<destination name="Trace Log Destination" sink="Trace Log Sink" />
		  </destinations>
		</category>
		<category name="Warn">
		  <destinations>
			<destination name="Event Log Destination" sink="Event Log Sink" />
			<destination name="Trace Log Destination" sink="Trace Log Sink" />
		  </destinations>
		</category>
	  </categories>
	</Azuro.Logging>
	*/

	/// <summary>
	/// Service log sinks.
	/// </summary>
	[Serializable]
	[XmlRoot(CfgLogSinks.SectionName)]
	public class CfgLogSinks : AConfigurationSection
	{
		/// <summary>
		/// The name of the configuration section.
		/// </summary>
		public const string SectionName = "Azuro.Logging";

		/// <summary>
		/// Log Sink collection.
		/// </summary>
		[Description("Log sink collection.")]
		[XmlElement("sink", Type = typeof(CfgLogSink))]
		public List<CfgLogSink> LogSinks { get; set; }

		/// <summary>
		/// Log Category collection.
		/// </summary>
		[Description("Log category collection.")]
		[XmlElement("categories", Type = typeof(CfgLogCategories))]
		public CfgLogCategories LogCategories { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to log exceptions.
		/// </summary>
		/// <value><c>true</c> if exceptions should be logged otherwise, <c>false</c> to rethrow them.</value>
		[Description("Specifies whether to log Exceptions.")]
		[XmlAttribute("logExceptions")]
		public bool LogExceptions { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether to log all messages.
		/// </summary>
		/// <value><c>true</c> if all messages are to be logged; otherwise, <c>false</c>.</value>
		[Description("Specifies whether to log all messages.")]
		[XmlAttribute("logAllMessages")]
		public bool LogAllMessages { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CfgLogSinks"/> class.
		/// </summary>
		public CfgLogSinks()
		{
			LogSinks = new List<CfgLogSink>();
			LogCategories = new CfgLogCategories();
		}

		/// <summary>
		/// Outputs a formatted string.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		[Browsable(false)]
		public string ToFormattedString()
		{
			StringBuilder sb = new StringBuilder("Log Sinks:");
			foreach (CfgLogSink p in LogSinks)
				sb.Append(p.ToFormattedString());

			foreach (CfgLogCategory p in LogCategories.LogCategory)
				sb.Append(p.ToFormattedString());

			return sb.ToString();
		}
		
		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		[Browsable(false)]
		public override string ToString()
		{
			return "Service Log Sinks";
		}
	}

	/// <summary>
	/// Log sink settings.
	/// </summary>
	[Description("Log sink settings unique to a service domain.")]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Serializable]
	public class CfgLogSink
	{
		/// <summary>
		/// Server name.
		/// </summary>
		[Category("Log Sink")]
		[Description("The name of the log sink.")]
		[XmlAttribute("name")]
		public string Name { get; set; }
		/// <summary>
		/// Log Sink Type, Assembly specifier.
		/// </summary>
		[Description("Log Sink implementation. (Type name, Assembly name).")]
		//	TODO: Because of XML Serialization and Reflection booboos, try this with the string overload
		[EditorAttribute("Azuro.Configuration.Design.TypeLoadEditor, Azuro.Configuration.Design", typeof(System.Drawing.Design.UITypeEditor))]
		//[EditorAttribute(typeof(TypeLoadEditor), typeof(System.Drawing.Design.UITypeEditor))]
		[XmlAttribute("type")]
		public string Type { get; set; }

		/// <summary>
		/// Settings collection.
		/// </summary>
		[Description("A Key/Value pair for settings data.")]
		[XmlElement("extensions", Type = typeof(KeyValueConfigurationSection))]
		public List<KeyValueConfigurationSection> Settings { get; set; }

		/// <summary>
		/// Settings indexer.
		/// </summary>
		[Browsable(false)]
		public string this[string key]
		{
			get
			{
				KeyValueConfigurationSection kvpcs = null;
				if (Settings != null)
					kvpcs = Settings.Find(item => item.Key != null && item.Key.CompareTo(key) == 0);
				return kvpcs != null ? kvpcs.Value : null;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CfgLogSink"/> class.
		/// </summary>
		public CfgLogSink()
		{
			Name = string.Empty;
			Type = string.Empty;
		}

		/// <summary>
		/// Outputs a formatted string.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		[Browsable(false)]
		public string ToFormattedString()
		{
			StringBuilder sb = new StringBuilder("Log Sink:");
			sb.AppendFormat("\nName => {0}", Name);
			sb.AppendFormat("\nType => {0}", Type);
			sb.Append("\nExtensions:");
			foreach (KeyValueConfigurationSection p in Settings)
				sb.AppendFormat("\n    key:{0} value:{1}", p.Key, p.Value);
			return sb.ToString();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		[Browsable(false)]
		public override string ToString()
		{
			return "Log Sink";
		}
	}

	/// <summary>
	/// Log sink categories.
	/// </summary>
	[Description("Log sink categories.")]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Serializable]
	public class CfgLogCategories
	{
		/// <summary>
		/// Log Category.
		/// </summary>
		[Description("Log category.")]
		[XmlElement("category", Type = typeof(CfgLogCategory))]
		public List<CfgLogCategory> LogCategory { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CfgLogCategories"/> class.
		/// </summary>
		public CfgLogCategories()
		{
			LogCategory = new List<CfgLogCategory>();
		}
	}

	/// <summary>
	/// Log sink categories.
	/// </summary>
	[Description("Log sink category.")]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Serializable]
	public class CfgLogCategory
	{
		/// <summary>
		/// Server name.
		/// </summary>
		[Category("Log Sink Category")]
		[Description("The name of the log sink.")]
		[XmlAttribute("name")]
		public string Name { get; set; }

        /// <summary>
        /// Server name.
        /// </summary>
        [Category( "Log Sink Category" )]
        [Description( "Is the category disabled?." )]
        [XmlAttribute( "disabled" )]
        public bool Disabled { get; set; }		

		/// <summary>
		/// Log Category collection.
		/// </summary>
		[Description("Log category destination collection.")]
		[XmlElement("destinations", Type = typeof(CfgLogDestinations))]
		public CfgLogDestinations LogDestinations { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CfgLogCategory"/> class.
		/// </summary>
		public CfgLogCategory()
		{
			LogDestinations = new CfgLogDestinations();
		}

		/// <summary>
		/// Outputs a formatted string.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		[Browsable(false)]
		public string ToFormattedString()
		{
			StringBuilder sb = new StringBuilder("Log Category:");
			sb.AppendFormat("\nName => {0}", Name);
			foreach (CfgLogDestination p in LogDestinations.LogDestination)
				sb.AppendFormat("\n    name:{0} sink:{1}", p.Name, p.Sink);
			return sb.ToString();
		}
		
		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		[Browsable(false)]
		public override string ToString()
		{
			return "Log Category";
		}
	}

	/// <summary>
	/// Log sink categories.
	/// </summary>
	[Description("Log sink destinations.")]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Serializable]
	public class CfgLogDestinations
	{
		/// <summary>
		/// Log Category.
		/// </summary>
		[Description("Log destination.")]
		[XmlElement("destination", Type = typeof(CfgLogDestination))]
		public List<CfgLogDestination> LogDestination { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CfgLogDestinations"/> class.
		/// </summary>
		public CfgLogDestinations()
		{
			LogDestination = new List<CfgLogDestination>();
		}
	}

	/// <summary>
	/// Log sink categories.
	/// </summary>
	[Description("Log sink category.")]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Serializable]
	public class CfgLogDestination
	{
		/// <summary>
		/// The name for this destination.
		/// </summary>
		[Category("Log Sink")]
		[Description("The name of the log destination.")]
		[XmlAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// The name of the sink to use. This sink must be found in the preceding definition of sinks.
		/// </summary>
		[Category("Log Sink")]
		[Description("The name of the destination sink.")]
		[XmlAttribute("sink")]
		public string Sink { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[Category("Log Sink")]
		[Description("The type of the destination. Use this property if a sink must only apply to a particular Type.")]
		[XmlAttribute("type")]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="CfgLogDestination"/> is exclude.
		/// </summary>
		/// <value><c>true</c> if exclude; otherwise, <c>false</c>.</value>
		[Category("Log Sink")]
		[Description("Exclude this type from logging. Setting this makes the Type property required. Use this property if a sink must not apply for a particular Type.")]
		[XmlAttribute("exclude")]
		public bool Exclude { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="CfgLogDestination"/> class.
		/// </summary>
		public CfgLogDestination()
		{
			Name = string.Empty;
			Sink = string.Empty;
			Type = string.Empty;
		}

		/// <summary>
		/// Outputs a formatted string.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		[Browsable(false)]
		public string ToFormattedString()
		{
			StringBuilder sb = new StringBuilder("Log Destination:");
			sb.AppendFormat("\nName => {0}", Name);
			sb.AppendFormat("\nSink => {0}", Sink);
			sb.AppendFormat("\nType => {0}", Type);
			sb.AppendFormat("\nExclude => {0}", Exclude);
			return sb.ToString();
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
		/// </returns>
		[Browsable(false)]
		public override string ToString()
		{
			return "Log Destination";
		}
	}
}
