using System;
using System.Text.RegularExpressions;
using Azuro.Common.Validation;

namespace Azuro.Data
{
	/// <summary>
	/// An implementation of AValidatorAttribute that checks whether a string conforms
	/// to a specific length requirement.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class RegularExpressionValidatorAttribute : AValidatorAttribute
	{
		private string m_regExp;

		/// <summary>
		/// Constructor.
		/// </summary>
		public RegularExpressionValidatorAttribute()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="regExp">The regular expression.</param>
		public RegularExpressionValidatorAttribute(string regExp)
		{
			m_regExp = regExp;
		}

		/// <summary>
		/// Regular Expression to parse.
		/// </summary>
		public string RegExp
		{
			get { return m_regExp; }
			set { m_regExp = value; }
		}

		/// <summary>
		/// Implement the abstract IsValid method. It is called by applications using the validation strategy
		/// and in this case will check whether the string conforms to the regular expression.
		/// </summary>
		/// <param name="value">The value for which the RegExp must be checked.</param>
		/// <returns>True if it complies with the regular expression specification, else false.</returns>
		public override bool IsValid(object value)
		{
			Regex re = new Regex(RegExp);
			return re.IsMatch(value.ToString());
		}
	}
}
