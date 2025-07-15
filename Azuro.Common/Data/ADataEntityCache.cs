using System;
using System.Collections.Generic;
using System.Reflection;
using Azuro.Common;
using Azuro.Common.Collections;
using Azuro.Caching;
using System.Runtime.Serialization;

namespace Azuro.Data
{
   public partial class DataEntity
   {
      private static Dictionary<Type, List<KeyColumn>> g_keyColumnCache;
      private static Dictionary<int, List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertySet>>> g_fillCache;

      protected class ADataEntityCache
      {
         private DataEntity m_adataEntity;
         private bool m_optimiseForBulk = true; //TODO from config
         [NonSerialized]
         private LateBound m_lateBound;

         public ADataEntityCache( DataEntity adataEntity )
         {
            m_adataEntity = adataEntity;
            InitLateBound();
         }        
         
         private void InitLateBound()
         {
            m_lateBound = new LateBound( m_optimiseForBulk ? LateBoundType.ExpressionTrees : LateBoundType.Reflection );
         }

         public List<KeyColumn> GetKeyColumns( Type entityType )
         {
            List<KeyColumn> keyColumns;

            if( KeyColumnCache.TryGetValue( entityType, out keyColumns ) )
            {
               return keyColumns;
            }
            else
            {
               return null;
            }
         }

         public void UpdateKeyColumns( Type entityType, List<KeyColumn> keyColumns )
         {
            KeyColumnCache[ entityType ] = keyColumns;
         }

         public List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertySet>> GetFillCache(
             string sqlCommand,
             DataEntity entity )
         {
             if ( sqlCommand == null )
             {
                 return null;
             }

            int h1 = sqlCommand.GetHashCode();
            int h2 = entity.GetType().GetHashCode();
            int hash = (((h1 << 5) + h1) ^ h2);

            List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertySet>> fillCache = null;

            lock( FillCache )
            {
               FillCache.TryGetValue( hash, out fillCache );
            }

            return fillCache;
         }

         public void UpdateFillCache(
             string sqlCommand,
             DataEntity entity,
             List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertySet>> cache )
         {
            int h1 = sqlCommand.GetHashCode();
            int h2 = entity.GetType().GetHashCode();
            int hash = (((h1 << 5) + h1) ^ h2);

            lock( FillCache )
            {
               FillCache[ hash ] = cache;
            }
         }


         private Dictionary<Type, List<KeyColumn>> KeyColumnCache
         {
            get
            {
               if( g_keyColumnCache == null )
               {
                  var hash = "ADataEntity.KeyColumnCache";

                  lock( CacheManager.Instance )
                  {
                     var cache = (Dictionary<Type, List<KeyColumn>>)CacheManager.Instance[ hash ];

                     if( cache == null )
                     {
                        cache = new Dictionary<Type, List<KeyColumn>>();
                        CacheManager.Instance[ hash ] = cache;
                     }

                     g_keyColumnCache = cache;
                  }
               }

               return g_keyColumnCache;
            }
         }

         private Dictionary<int, List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertySet>>> FillCache
         {
            get
            {
               if( g_fillCache == null )
               {
                  var hash = "ADataEntity.FillCache";

                  lock( CacheManager.Instance )
                  {
                     var cache = (Dictionary<int, List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertySet>>>)CacheManager.Instance[ hash ];

                     if( cache == null )
                     {
                        cache = new Dictionary<int, List<Triplet<PropertyInfo, DatabaseColumnAttribute, PropertySet>>>();
                        CacheManager.Instance[ hash ] = cache;
                     }

                     g_fillCache = cache;
                  }
               }

               return g_fillCache;
            }
         }

         public LateBound LateBound { get { return m_lateBound; } }
      }
   }
}
