using System;
using System.Collections.Generic;
using System.Text;

namespace Azuro.Configuration
{
	/// <summary>
	/// An exception to represent configuration validation errors.
	/// </summary>
	public class ConfigurationValidationException : Exception
	{
		private object m_cfg;
		private List<string> m_errors;

		/// <summary>
		/// Return the failed configuration object.
		/// </summary>
		public object ConfigurationSection
		{
			get { return m_cfg; }
		}

		/// <summary>
		/// Return a List of validation errors.
		/// </summary>
		public List<string> Errors
		{
			get { return m_errors; }
		}

		/// <summary>
		/// ctor.
		/// </summary>
		/// <param name="message">The exception message.</param>
		/// <param name="cfg">The configuration object that failed validation.</param>
		/// <param name="errors">The list of errors returned by the Validate method.</param>
		public ConfigurationValidationException(string message, object cfg, List<string> errors) : base(message)
		{
			m_cfg = cfg;
			m_errors = errors;
		}
	}

	/// <summary>
	/// This class will validate that a configuration element is valid.
	/// </summary>
	public class ConfigurationValidator
	{
		/// <summary>
		/// Validate the passed in configuration element.
		/// </summary>
		/// <param name="cfg">The configuration element to check.</param>
		/// <returns>A List&lt;string&gt; containing any validation errors.</returns>
		public static List<string> Validate(object cfg)
		{
			return new List<string>();
		}
	}
}
