using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading;
using Azuro.Common;

namespace Azuro.Data
{
    /// <summary>
    /// Caches data entity inserts and inserts them into a SQL table in batches.
    /// The data is written in batches of the size specified in the ctor (auto write threshold).
    /// 
    /// Note that the acual number of items written in a batch might be larger than the 
    /// auto write threshold. This happens when data is cached while a pervious set of data
    /// is busy being written. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SqlBulkInserter
    {
        private object m_cachedSyncRoot = new object();
        private Dictionary<Type, List<DataEntity>> m_cached = new Dictionary<Type, List<DataEntity>>();
        private readonly Queue<KeyValuePair<Type, List<DataEntity>>> m_toWrite = new Queue<KeyValuePair<Type, List<DataEntity>>>();
        private readonly Dictionary<Type, string> m_typeTableNames = new Dictionary<Type, string>();
        private readonly Dictionary<Type, DataTable> m_schemaTables = new Dictionary<Type, DataTable>();
        private readonly DataObject m_dataObject;
        private readonly int m_autoWriteThreshold;
        private readonly Dictionary<Type, List<PropertyGet>> m_propertyGetters = new Dictionary<Type, List<PropertyGet>>();
        private readonly SqlBulkCopyOptions m_sqlBulkCopyOptions;
        [NonSerialized]
        private LateBound m_lateBound;

        private bool m_done = false;
        private readonly Thread m_thread;
        private readonly AutoResetEvent m_wait = new AutoResetEvent( false );

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkInserter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <param name="tableName">Name of the table insert into.</param>
        public SqlBulkInserter( DataObject dataObject, IDictionary<Type, string> typeTableNames )
            : this( dataObject, typeTableNames, 1000, SqlBulkCopyOptions.TableLock )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkInserter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <param name="tableName">Name of the table insert into.</param>
        public SqlBulkInserter( DataObject dataObject, IDictionary<Type, string> typeTableNames, int autoWriteThreshold )
            : this( dataObject, typeTableNames, autoWriteThreshold, SqlBulkCopyOptions.TableLock )
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBulkInserter&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="dataObject">The data object.</param>
        /// <param name="tableName">Name of the table insert into.</param>
        /// <param name="autoWriteThreshold">Number of items to cache before doing the bulk insert.</param>
        public SqlBulkInserter( DataObject dataObject, IDictionary<Type, string> typeTableNames, int autoWriteThreshold, SqlBulkCopyOptions sqlBulkCopyOptions )
        {
            #region param checks
            if( dataObject == null )
            {
                throw new ArgumentNullException( "dataObject" );
            }

            if( typeTableNames == null )
            {
                throw new ArgumentNullException( "typeTableNames" );
            }
            #endregion

            m_dataObject = dataObject;
            m_sqlBulkCopyOptions = sqlBulkCopyOptions;
            m_typeTableNames = new Dictionary<Type, string>( typeTableNames );
            m_lateBound = new LateBound(LateBoundType.ExpressionTrees);
            m_autoWriteThreshold = Math.Max( autoWriteThreshold, 1000 );

            foreach( var kv in m_typeTableNames )
            {
                Type type = kv.Key;
                string tableName = kv.Value;

                m_schemaTables[type] = GetSchemaTable( tableName );
            }

            CachePropertyGettersForDataColumn();

            m_thread = new Thread( ProcessCachedDataThreadMethod );
            m_thread.IsBackground = false;
            m_thread.Start();
        }

        /// <summary>
        /// Cache a item for inserting.
        /// </summary>
        /// <param name="entity">The entity.</param>
        public virtual void CacheInsert( DataEntity entity )
        {
            Type entityType = entity.GetType();

            if( !m_typeTableNames.ContainsKey( entityType ) )
            {
                throw new ArgumentOutOfRangeException( "entity", "Entity type has not been configured: " + entity.GetType() );
            }

            List<DataEntity> write = null;

            lock( m_cachedSyncRoot )
            {
                if( !m_cached.ContainsKey( entityType ) )
                {
                    m_cached[entityType] = new List<DataEntity>();
                }

                m_cached[entityType].Add( entity );

                //Are there enough items to write to the database?
                if( m_cached.Count > m_autoWriteThreshold )
                {
                    write = m_cached[entityType];
                    m_cached[entityType] = new List<DataEntity>();
                }
            }

            //Were there enough items to write to the database
            if( write != null )
            {
                lock( m_toWrite )
                {
                    //Enqueue a new collection of data to write
                    m_toWrite.Enqueue( new KeyValuePair<Type,List<DataEntity>>( entityType, write ) );
                }

                //Signal the thread so that the data will be inserted
                m_wait.Set();
            }
        }

        /// <summary>
        /// Call when all inserts have been cached.
        /// This enures that the remaining cached items are inserted and stops the insert thread.
        /// </summary>
        public void Done()
        {
            var write = new Dictionary<Type, List<DataEntity>>();

            lock( m_cachedSyncRoot )
            {
                //Are there any cached items still to be written
                foreach( var kv in m_cached )
                {
                    var type = kv.Key;
                    var items = kv.Value;

                    write[type] = items;
                }

                foreach( var kv in write )
                {
                    m_cached[kv.Key] = new List<DataEntity>();
                }
            }

            //If there are items left to write
            if( write.Count > 0 )
            {
                lock( m_toWrite )
                {
                    foreach( var kv in write )
                    {
                        if( kv.Value.Count > 0 )
                        {
                            m_toWrite.Enqueue( kv );
                        }
                    }
                }
            }

            //Inserting done
            m_done = true;

            //Signal the thread so that the data will be inserted
            m_wait.Set();

            //Wait for the inserts to complete
            m_thread.Join();
        }

        /// <summary>
        /// Method run by the insert thread.
        /// </summary>
        private void ProcessCachedDataThreadMethod()
        {
            do
            {
                if( !m_done )
                {
                    //Wait for a signal
                    m_wait.WaitOne();
                }

                Dictionary<Type, List<DataEntity>> write = new Dictionary<Type, List<DataEntity>>();

                lock( m_toWrite )
                {
                    //Add all the items read to be inserted into a single list.
                    // This means that all the cached items can be written in a single insert.
                    while( m_toWrite.Count > 0 )
                    {
                        var kv = m_toWrite.Dequeue();
                        var type = kv.Key;
                        var items = kv.Value;

                        if( !write.ContainsKey( type ) )
                        {
                            write[type] = new List<DataEntity>();
                        }

                        write[type].AddRange( items );
                    }
                }

                //If there are items to insert then run a bulk copy
                foreach( var kv in write )
                {
                    var type = kv.Key;
                    var items = kv.Value;

                    var tableName = m_typeTableNames[type];
                    var schemaTable = m_schemaTables[type];
                    var getters = m_propertyGetters[type];

                    var bulk = new SqlBulkCopy( m_dataObject.ConnectionString, m_sqlBulkCopyOptions );
                    bulk.BulkCopyTimeout = 5 * 60;
                    bulk.BatchSize = m_autoWriteThreshold;
                    bulk.DestinationTableName = tableName;
                    bulk.WriteToServer( new BulkInserterReader( schemaTable, items, getters ) );
                }

            } while( !m_done || (m_done && AreItemsCached) );
        }

        /// <summary>
        /// Gets the schema table for a database table.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        private DataTable GetSchemaTable( string tableName )
        {
            var dataTable = new DataTable( tableName );

            using( var adapter = new SqlDataAdapter( "select top 0 * from " + tableName, m_dataObject.ConnectionString ) )
            {
                adapter.Fill( dataTable );
            }

            return dataTable;
        }

        /// <summary>
        /// For each data column create a delegate that execute the property getter
        /// </summary>
        private void CachePropertyGettersForDataColumn()
        {
            foreach( var kv in m_typeTableNames )
            {
                var entityType = kv.Key;

                var props = new Dictionary<string, PropertyInfo>();

                // Get all the column names from the database column attribute, and the corresponding property info
                foreach( var pi in entityType.GetProperties() )
                {
                    var dbca = (DatabaseColumnAttribute)Attribute.GetCustomAttribute( pi, typeof( DatabaseColumnAttribute ) );

                    if( dbca != null )
                    {
                        props[dbca.ColumnName] = pi;
                    }
                }

                // Now create a list of the properties in the order that they are expected by the data table.
                foreach( DataColumn col in m_schemaTables[entityType].Columns )
                {
                    var getter = LateBound.CreatePropertyGetter( props[col.ColumnName].Name, entityType );

                    if( !m_propertyGetters.ContainsKey( entityType ) )
                    {
                        m_propertyGetters[entityType] = new List<PropertyGet>();
                    }

                    m_propertyGetters[entityType].Add( getter );
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether are any cached items
        /// </summary>
        /// <value><c>true</c> if are items cached; otherwise, <c>false</c>.</value>
        private bool AreItemsCached
        {
            get
            {
                lock( m_cachedSyncRoot )
                {
                    var numCached = (
                        from c in m_cached 
                        where (c.Value != null) && (c.Value.Count > 0) 
                        select 1).Count();

                    if( numCached > 0 )
                    {
                        return true;
                    }
                }

                lock( m_toWrite )
                {
                    if( m_toWrite.Count > 0 )
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the bumber of items cached before bulk inserts are run
        /// </summary>
        /// <value>The auto write threshold.</value>
        public int AutoWriteThreshold { get { return m_autoWriteThreshold; } }

        /// <summary>
        /// Gets or sets the late bound helper class.
        /// </summary>
        /// <value>The late bound.</value>
        public LateBound LateBound
        {
            get { return m_lateBound; }
        }
    }
}
