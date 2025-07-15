using System;
using System.Collections.Generic;
using System.Text;

namespace Azuro.Data
{
	/// <summary>
	/// An attribute that describes the stored procedures to use when interacting with
	/// DataEntity.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    [Serializable]
	public class StoredProcedureAttribute : System.Attribute
	{
		/// <summary>
		/// The procedure to use when calling DataObject.List.
		/// </summary>
		public string ListProcedure { get; set; }

		/// <summary>
		/// The procedure to use when calling DataObject.Fetch.
		/// </summary>
		public string FetchProcedure { get; set; }

		/// <summary>
		/// The procedure to use when calling DataObject.Update.
		/// </summary>
		public string UpdateProcedure { get; set; }

		/// <summary>
		/// The procedure to use when calling DataObject.Insert.
		/// </summary>
		public string InsertProcedure { get; set; }

		/// <summary>
		/// The procedure to use when calling DataObject.Delete.
		/// </summary>
		public string DeleteProcedure { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="StoredProcedureAttribute"/> class.
		/// </summary>
		public StoredProcedureAttribute()
		{
			ListProcedure
				= FetchProcedure
				= UpdateProcedure
				= InsertProcedure
				= DeleteProcedure
				= string.Empty;
		}
	}
}
