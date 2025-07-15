using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Azuro.Common.Configuration;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Azuro.Common
{
	/// <summary>
	/// This class provides a configurable mechanism for setting up string replacements from various objects and default values.
	/// </summary>
	public class ReplacementParameters
	{
		private static ReplacementParametersConfigurationSection Config { get; set; }

		/// <summary>
		/// Initializes the <see cref="ReplacementParameters"/> class.
		/// </summary>
		static ReplacementParameters()
		{
			Config = ConfigurationHelper.GetSection<ReplacementParametersConfigurationSection>(ReplacementParametersConfigurationSection.SectionName)
				?? new ReplacementParametersConfigurationSection();

			Config.Tags.Add(new ReplacementTagConfigurationSection("{%yyyyMMdd%}", ReplacementCommonValue.CurrentDateTime, "yyyyMMdd"));
			Config.Tags.Add(new ReplacementTagConfigurationSection("{%DateTime%}", ReplacementCommonValue.CurrentDateTime, "yyyyMMdd"));
			Config.Tags.Add(new ReplacementTagConfigurationSection("{%FullDateTime%}", ReplacementCommonValue.CurrentDateTime, "yyyyMMddHHmmssfff"));
		}

		/// <summary>
		/// Replaces the specified input.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="valueObjects">The value objects.</param>
		/// <returns></returns>
		public static string Replace(string input, List<object> valueObjects)
		{
			string result = input;
			foreach (ReplacementTagConfigurationSection rtcs in Config.Tags)
			{
				result = ReplaceParameters(result, rtcs, valueObjects);
			}

			return result;
		}

		/// <summary>
		/// Replaces the parameters.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="rtcs">The RTCS.</param>
		/// <param name="valueObjects">The value objects.</param>
		/// <returns></returns>
		private static string ReplaceParameters(string input, ReplacementTagConfigurationSection rtcs, List<object> valueObjects)
		{
			string result = string.Empty;
			if (rtcs.CommonValue != ReplacementCommonValue.NotSet)
			{
				result = ReplaceCommonValues(input, rtcs);
			}
			else
			{
				result = ReplaceObjectValues(input, rtcs, valueObjects);
			}

			return result;
		}

		/// <summary>
		/// Replaces the common values.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="rtcs">The RTCS.</param>
		/// <returns></returns>
		private static string ReplaceCommonValues(string input, ReplacementTagConfigurationSection rtcs)
		{
			string result = string.Empty;

			switch (rtcs.CommonValue)
			{
				case ReplacementCommonValue.CurrentDateTime:
					result = Regex.Replace(input, rtcs.Tag, DateTime.Now.ToString(rtcs.Format ?? "yyyyMMdd"));
					break;
				case ReplacementCommonValue.RandomInt:
					{
						Random r = new Random(DateTime.Now.Millisecond);
						result = Regex.Replace(input, rtcs.Tag, r.Next().ToString(rtcs.Format ?? "0"));
						break;
					}
				case ReplacementCommonValue.RandomDecimal:
					{
						Random r = new Random(DateTime.Now.Millisecond);
						result = Regex.Replace(input, rtcs.Tag, r.NextDouble().ToString(rtcs.Format ?? "0.00"));
						break;
					}
			}

			return result;
		}

		/// <summary>
		/// Replaces the object values.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="rtcs">The RTCS.</param>
		/// <param name="valueObjects">The value objects.</param>
		/// <returns></returns>
		private static string ReplaceObjectValues(string input, ReplacementTagConfigurationSection rtcs, List<object> valueObjects)
		{
			string result = string.Empty;
			//	Dotted value indicates Object name.Property
			int idx = rtcs.Value.LastIndexOf('.');
			if (idx > 0)
			{
				string objectName = rtcs.Value.Substring(0, idx);

				object oVal = valueObjects.Find(item => item.GetType().Name == objectName || item.GetType().FullName == objectName);
				if (oVal != null)
					result = ReplaceObjectValues(input, rtcs.Tag, rtcs.Value.Substring(idx + 1), oVal);
			}
			else
			{
				result = input;
				foreach (object oVal in valueObjects)
				{
					result = ReplaceObjectValues(result, rtcs.Tag, rtcs.Value, oVal);
				}
			}
			return result;
		}

		/// <summary>
		/// Replaces the object values.
		/// </summary>
		/// <param name="input">The input.</param>
		/// <param name="tag">The tag.</param>
		/// <param name="valueName">Name of the value.</param>
		/// <param name="oVal">The o val.</param>
		/// <returns></returns>
		private static string ReplaceObjectValues(string input, string tag, string valueName, object oVal)
		{
			FieldInfo fi = oVal.GetType().GetField(valueName);
			if (fi != null)
			{
				object pVal = fi.GetValue(oVal);
				//Regex re = new Regex(ReplacementTags[i].Tag, RegexOptions.Compiled | RegexOptions.IgnoreCase);
				return Regex.Replace(input, tag, FormatField(pVal));
			}
			return input;
		}

		/// <summary>
		/// Formats the field.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <returns></returns>
		private static string FormatField(object o)
		{
			if (o is TimeSpan)
			{
				TimeSpan ts = (TimeSpan)o;
				return string.Format("{0}{1}{2}h {3}m {4}s", (ts.Days > 0) ? ts.Days.ToString() + " day" : "",
															(ts.Days > 1) ? "s " : " ",
															ts.Hours, ts.Minutes, ts.Seconds);
			}
			else
				return o.ToString();
		}
	}
}
