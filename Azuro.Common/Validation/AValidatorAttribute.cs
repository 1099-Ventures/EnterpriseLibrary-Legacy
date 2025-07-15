using System;

namespace Azuro.Common.Validation
{
	/// <summary>
	/// This abstract class forms the base interface for a validator attribute.
	/// It can be applied to any property that requires validation. The Azuro
	/// Data Access Layer already validates any entities for correctness before 
	/// updating the database with the entity.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public abstract class AValidatorAttribute : System.Attribute
	{
		/// <summary>
		/// The message to use when validation fails.
		/// </summary>
		protected string m_message;

		/// <summary>
		/// The message to use when validation fails.
		/// </summary>
		public string Message
		{
			get { return m_message; }
			set { m_message = value; }
		}
		/// <summary>
		/// This method must be implemented in child classes and should contain the logic
		/// for the validation.
		/// </summary>
		/// <param name="value">The value to check for validity.</param>
		/// <returns>True if the validation passed, else false.</returns>
		public abstract bool IsValid(object value);
	}

	/// <summary>
	/// This exception is thrown when validation fails.
	/// </summary>
	public class ValidationException : System.Exception
	{
		private AValidatorAttribute m_va;

		/// <summary>
		/// The validator attribute that caused the exception.
		/// </summary>
		public AValidatorAttribute ValidatorAttribute
		{
			get { return m_va; }
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">A message for the exception.</param>
		/// <param name="va">The <see cref="AValidatorAttribute">AValidatorAttribute</see> that caused the exception.</param>
		public ValidationException(string message, AValidatorAttribute va) : base(message)
		{
			m_va = va;
		}
	}
}
