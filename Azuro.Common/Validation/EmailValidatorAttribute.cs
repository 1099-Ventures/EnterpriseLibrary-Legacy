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
	public class EmailValidatorAttribute : AValidatorAttribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		public EmailValidatorAttribute()
		{
		}

		/// <summary>
		/// Implement the abstract IsValid method. It is called by applications using the validation strategy
		/// and in this case will check whether the string conforms to the regular expression.
		/// </summary>
		/// <param name="value">The value for which the RegExp must be checked.</param>
		/// <returns>True if it complies with the regular expression specification, else false.</returns>
		public override bool IsValid(object value)
		{
			Regex re = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
			return re.IsMatch(value.ToString());
		}
	}
}
