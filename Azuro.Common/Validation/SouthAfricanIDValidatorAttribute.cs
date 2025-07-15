using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using Azuro.Data;

namespace Azuro.Common.Validation

{
	/// <summary>
	/// This class will validate a property for conformance to the South African ID
	/// number check digit standard.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class SouthAfricanIDValidatorAttribute : AValidatorAttribute
	{
		public SouthAfricanIDValidatorAttribute()
		{
			Message = "Not a valid South African ID number";
		}

		/// <summary>
		/// Validates the ID number for conformance to the check digit only. For more
		/// comprehensive validation, call the Azuro.Common.GenericValidation.ValidateID()
		/// method directly.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <returns>True if its a valid ID number.</returns>
		public override bool IsValid(object value)
		{
			return GenericValidation.ValidateID(DateTime.MinValue, ' ', value.ToString());
		}
	}
}
