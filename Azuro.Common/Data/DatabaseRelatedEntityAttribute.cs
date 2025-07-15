using System;

namespace Azuro.Data
{
	public enum EntityRelationType
	{
		Fetch,
		List
	}
	/// <summary>
	/// An attribute that describes an embedded list of entities in another entity
	/// for the Data Access Layer.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class DatabaseRelatedEntityAttribute : Attribute
	{
		private Type m_entityType;
		private bool m_fillByDefault;
		private EntityRelationType m_entityRelation;

		/// <summary>
		/// ctor.
		/// </summary>
		/// <param name="entityType">The type of the embedded entity to list.</param>
		/// <param name="entityRelationType">The type of relationship operation to perform.</param>
		public DatabaseRelatedEntityAttribute(Type entityType, EntityRelationType entityRelationType)
		{
			m_entityType = entityType;
			m_entityRelation = entityRelationType;
		}

		/// <summary>
		/// The type of the embedded entity.
		/// </summary>
		public Type EntityType
		{
			get { return m_entityType; }
			set { m_entityType = value; }
		}

		/// <summary>
		/// Set this property to true if you want the fetch to fill this property by default.
		/// </summary>
		public bool FillByDefault
		{
			get { return m_fillByDefault; }
			set { m_fillByDefault = value; }
		}

		/// <summary>
		/// The type of relationship, be it single fetch or list.
		/// </summary>
		public EntityRelationType EntityRelationType
		{
			get { return m_entityRelation; }
			set { m_entityRelation = value; }
		}
	}
}
