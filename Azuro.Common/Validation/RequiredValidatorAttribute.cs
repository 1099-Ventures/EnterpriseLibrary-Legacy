using System;
using System.Collections.Generic;
using System.Text;

using Azuro.Data;

namespace Azuro.Common.Validation

{
	[AttributeUsage(AttributeTargets.Property)]
	public class RequiredValidatorAttribute : AValidatorAttribute
	{
		public RequiredValidatorAttribute()
		{
			Message = "Value is required";
		}

		public override bool IsValid( object value )
		{
			if( value == null )
			{
				return false;
			}

			return string.IsNullOrEmpty( value.ToString() );
		}
	}
}
