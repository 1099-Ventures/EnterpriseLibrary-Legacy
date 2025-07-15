using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Azuro.Common;
using Azuro.Common.Collections;
using Azuro.Caching;

namespace Azuro.Data
{
    public partial class DataObject
    {
        protected class DataObjectCache
        {
            private DataObject m_dataObject;
            private bool m_optimiseForBulk = true;
            private Dictionary<Type, FieldInfo> m_fieldInfoCache;
            private Dictionary<int, List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>>> m_propCache;
            [NonSerialized]
            private LateBound m_lateBound;


            public DataObjectCache( DataObject dataObject )
            {
                m_dataObject = dataObject;
                m_lateBound = new LateBound(m_optimiseForBulk ? LateBoundType.ExpressionTrees : LateBoundType.Reflection);
            }

            public List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>> GetPropertyCache(
                string spName,
                DataEntity entity )
            {
                int h1 = spName.GetHashCode();
                int h2 = (entity != null) ? entity.GetType().GetHashCode() : 0;
                int hash = (((h1 << 5) + h1) ^ h2);

                List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>> props = null;

                lock( PropCache )
                {
                    PropCache.TryGetValue( hash, out props );
                }

                return props;
            }
            
            public void CacheProperties( 
                string spName,
                DataEntity entity, 
                List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>> props )
            {
                int h1 = spName.GetHashCode();
                int h2 = (entity != null) ? entity.GetType().GetHashCode() : 0;
                int hash = (((h1 << 5) + h1) ^ h2);

                lock( PropCache )
                {
                    PropCache[hash] = props;
                }
            }

            public FieldInfo GetFieldInfo( Type entityType )
            {
                FieldInfo fieldInfo = null;

                lock( FieldInfoCache )
                {
                    FieldInfoCache.TryGetValue( entityType, out fieldInfo );
                }

                if( fieldInfo == null )
                {
                    fieldInfo = new FieldInfo( this, entityType, m_dataObject.GetProperties( entityType ), m_dataObject );

                    lock( FieldInfoCache )
                    {
                        FieldInfoCache[entityType] = fieldInfo;
                    }
                }

                return fieldInfo;
            }

            private Dictionary<Type, FieldInfo> FieldInfoCache
            {
                get
                {
                    if( m_fieldInfoCache == null )
                    {
                        var hash = Util.Hash( m_dataObject.ConnectionString, "DataObject.FieldInfoCache" ).ToString();

                        lock( CacheManager.Instance )
                        {
                            var cache = (Dictionary<Type, FieldInfo>)CacheManager.Instance[hash];

                            if( cache == null )
                            {
                                cache = new Dictionary<Type, FieldInfo>();
                                CacheManager.Instance[hash] = cache;
                            }

                            m_fieldInfoCache = cache;
                        }
                    }

                    return m_fieldInfoCache;
                }
            }

            private Dictionary<int, List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>>> PropCache
            {
                get
                {
                    if( m_propCache == null )
                    {
                        var hash = Util.Hash( m_dataObject.ConnectionString, "DataObject.Cache" ).ToString();

                        lock( CacheManager.Instance )
                        {
                            var cache = (Dictionary<int, List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>>>)CacheManager.Instance[hash];

                            if( cache == null )
                            {
                                cache = new Dictionary<int, List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertyGet>>>();
                                CacheManager.Instance[hash] = cache;
                            }

                            m_propCache = cache;
                        }
                    }

                    return m_propCache;
                }
                set
               { 
                    var hash = Util.Hash( m_dataObject.ConnectionString, "DataObjectCache" ).ToString();
                    CacheManager.Instance[hash] = value;
                }
            }

            public LateBound LateBound
            {
                get { return m_lateBound; }
            }
        }

        [Serializable]
        protected class FieldInfo
        {
            public FieldInfo( DataObjectCache cache, Type type, List<PropertyInfo> properties, DataObject dataObject )
            {
                m_cache = cache;
                Type = type;
                Properties = properties;
                m_dataObject = dataObject;
            }

            private DataObjectCache m_cache;
            private DataObject m_dataObject;
            private List<KeyValuePair<DatabaseRelatedEntityAttribute, PropertyInfo>> m_databaseRelatedEntityAttributes;
            private Dictionary<string, DataTable> m_fetchSchemas = new Dictionary<string, DataTable>();
            public Type Type { get; private set; }
            public List<PropertyInfo> Properties { get; private set; }
            public MethodInfo ListMethodMethodInfo { get; private set; }
            public CallMethodWithResult ListMethod { get; private set; }
            public DataTable FetchSchemaTable { get; set; }

            public List<KeyValuePair<DatabaseRelatedEntityAttribute, PropertyInfo>> DatabaseRelatedEntityAttributes
            {
                get
                {
                    if( m_databaseRelatedEntityAttributes == null )
                    {
                        CacheDatabaseRelatedEntityAttributes();
                    }

                    return m_databaseRelatedEntityAttributes;
                }
            }

            private void CacheDatabaseRelatedEntityAttributes()
            {
                if( m_databaseRelatedEntityAttributes == null )
                {
                    m_databaseRelatedEntityAttributes = new List<KeyValuePair<DatabaseRelatedEntityAttribute, PropertyInfo>>();

                    lock( DatabaseRelatedEntityAttributes )
                    {
                        foreach( PropertyInfo pi in Properties )
                        {
                            DatabaseRelatedEntityAttribute dbrea = AttributeHelper.GetCustomAttribute<DatabaseRelatedEntityAttribute>( pi );

                            if( dbrea != null )
                            {
                                DatabaseRelatedEntityAttributes.Add(
                                    new KeyValuePair<DatabaseRelatedEntityAttribute, PropertyInfo>(
                                        dbrea,
                                        pi ) );
                            }
                        }
                    }
                }
                else
                {
                    lock( DatabaseRelatedEntityAttributes )
                    {
                        //Lock here to ensure that m_databaseRelatedEntityAttributes is 
                        // populated by the code above
                    }
                }
            }

            public void CacheListMethod()
            {
                if( ListMethod == null )
                {
                    MethodInfo mi = m_dataObject.GetMethod( "List", Type, new Type[] { typeof( DataEntity ), typeof( int ) }, true );

                    if( mi != null )
                    {
                        ListMethod = m_cache.LateBound.CreateMethodCallWithResult( mi );
                    }
                }
            }

            public DataTable GetFetchSchemaTable( string fetchProcedure )
            {
                DataTable schema = null;

                lock( m_fetchSchemas )
                {
                    m_fetchSchemas.TryGetValue( fetchProcedure, out schema );
                }

                return schema;
            }

            public void UpdateFetchSchemaTable( string fetchProcedure, DataTable schema )
            {
                lock( m_fetchSchemas )
                {
                    m_fetchSchemas[ fetchProcedure ] = schema;
                }
            }
        }
    }
}
