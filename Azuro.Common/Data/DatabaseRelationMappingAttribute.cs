using System;

namespace Azuro.Data
{
	/// <summary>
	/// Use this attribute to set mappings for the Database entity list attribute.
	/// It has no relevance other than for that attribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
	public class DatabaseRelationMappingAttribute : Attribute
	{
		/// <summary>
		/// The source property in the entity to get the value from.
		/// </summary>
		public string SourceProperty { get; set; }

		/// <summary>
		/// The target property on the destination entity to set the value on.
		/// </summary>
		public string TargetProperty { get; set; }

		/// <summary>
		/// Set the target property to an arbitrary value.
		/// </summary>
		/// <value>The target value.</value>
		public object TargetValue { get; set; }

		/// <summary>
		/// Determines whether this instance is valid.
		/// </summary>
		/// <returns>
		/// 	<c>true</c> if this instance is valid; otherwise, <c>false</c>.
		/// </returns>
		public bool IsValid()
		{
			return !string.IsNullOrEmpty(TargetProperty)
					&& (string.IsNullOrEmpty(SourceProperty) && TargetValue != null)
					|| (!string.IsNullOrEmpty(SourceProperty) && TargetValue == null);
		}
	}
}
