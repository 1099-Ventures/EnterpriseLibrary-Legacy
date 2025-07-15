using System;
using Azuro.Common.Validation;

namespace Azuro.Data
{
	/// <summary>
	/// An implementation of AValidatorAttribute that checks whether a string conforms
	/// to a specific length requirement.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class IntegerValidatorAttribute : AValidatorAttribute
	{
		private int m_min,
					m_max = Int32.MaxValue;

		/// <summary>
		/// Constructor.
		/// </summary>
		public IntegerValidatorAttribute()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="min">The minimum allowed length of a string.</param>
		/// <param name="max">The maximum allowed length of a string.</param>
		public IntegerValidatorAttribute(int min, int max)
		{
			m_min = min;
			m_max = max;
		}

		/// <summary>
		/// Minimum length.
		/// </summary>
		public int MinValue
		{
			get { return m_min; }
			set { m_min = value; }
		}

		/// <summary>
		/// Maximum length.
		/// </summary>
		public int MaxValue
		{
			get { return m_max; }
			set { m_max = value; }
		}

		/// <summary>
		/// Implement the abstract IsValid method. It is called by applications using the validation strategy
		/// and in this case will check the string for a valid length.
		/// </summary>
		/// <param name="value">The value for which length must be checked. While length can be checked for any type, using
		/// ToString, it seems an arbitrary test to do on, for example, an integer value.</param>
		/// <returns>True if it complies with the length specification, else false.</returns>
		public override bool IsValid(object value)
		{
			int p;
			bool l = int.TryParse(value.ToString(), out p);
			if (!l) return false;
			return (p >= m_min && p <= m_max);
		}
	}
}
