using System;
using System.Collections.Generic;
using System.Text;
using Azuro.Common;

namespace Azuro.Data
{
	/// <summary>
	/// Use this attribute to map a Lookup Enum to codes on the database.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class LookupCodeMappingAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the code.
		/// </summary>
		/// <value>The code.</value>
		public string Code { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="LookupCodeMappingAttribute"/> class.
		/// </summary>
		/// <param name="code">The code.</param>
		public LookupCodeMappingAttribute(string code)
		{
			Code = code;
		}

		/// <summary>
		/// Gets the lookup code.
		/// </summary>
		/// <param name="t">The t.</param>
		/// <param name="enumField">The enum field.</param>
		/// <returns></returns>
		public static string GetLookupCode(Type t, string enumField)
		{
			return AttributeHelper.GetFieldAttribute<LookupCodeMappingAttribute>(t, enumField).Code;			
		}

		/// <summary>
		/// Gets the lookup code.
		/// </summary>
		/// <param name="o">The o.</param>
		/// <param name="enumField">The enum field.</param>
		/// <returns></returns>
		public static string GetLookupCode(object o, string enumField)
		{
			return AttributeHelper.GetFieldAttribute<LookupCodeMappingAttribute>(o, enumField).Code;
		}
	}
}
