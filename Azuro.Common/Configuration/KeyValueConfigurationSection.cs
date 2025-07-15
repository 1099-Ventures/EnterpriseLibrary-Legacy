using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.ComponentModel;

namespace Azuro.Configuration
{
	/// <summary>
	/// This class represents a default KeyValue pair for Xml configuration.
	/// </summary>
	[Description("Key/Value pairs for settings data.")]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Serializable]
	public class KeyValueConfigurationSection
	{
		private string m_key;
		private string m_value;

		/// <summary>
		/// An overloaded operator to create a KeyValuePair from this Config section.
		/// </summary>
		/// <param name="kvcs">The configuration section to convert.</param>
		/// <returns>A KeyValuePair&lt;string, string&gt;</returns>
		public static implicit operator KeyValuePair<string, string>(KeyValueConfigurationSection kvcs)
		{
			return new KeyValuePair<string, string>(kvcs.Key, kvcs.Value);
		}
		/// <summary>
		/// The key field.
		/// </summary>
		[Description("The key name of Key/Value pair.")]
		[XmlAttribute("key")]
		public string Key
		{
			get { return m_key; }
			set { m_key = value; }
		}
		/// <summary>
		/// The value field.
		/// </summary>
		[Description("The value of Key/Value pair.")]
		[XmlAttribute("value")]
		public string Value
		{
			get { return m_value; }
			set { m_value = value; }
		}

		/// <exclude/>
		[Browsable(false)]
		public override string ToString()
		{
			return string.Format("[{0}]: Key/Value Settings", GetType());
		}
	}
}
