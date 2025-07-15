using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Web.Caching;
using Azuro.Caching;

namespace Azuro.Caching
{
	public class InMemoryCache : ICacheManager
	{
		private ManualResetEvent m_cacheExpiryWait = new ManualResetEvent( false );
        private Hashtable m_cacheItems;

        private Hashtable CacheItems
		{
            get { return m_cacheItems ?? (m_cacheItems = new Hashtable( 20 )); }
		}

		public object this[ string key ]
		{
			get
			{
				CacheItem item;

				lock( CacheItems )
				{
					if( CacheItems.ContainsKey( key ) )
					{
						item = (CacheItem)CacheItems[ key ];

						if( item.CacheDependency != null )
						{
							if( item.CacheDependency.HasChanged )
							{
								CacheItems.Remove( key );
							}
						}

						return item.Value;
					}
					else
					{
						return null;
					}
				}
			}
			set
			{
				if( CacheItems.ContainsKey( key ) )
				{
					Set( (CacheItem)CacheItems[ key ], value );
				}
				else
				{
					Add( key, value );
				}
			}
		}

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        public void Add( string key, object value )
		{
			Add( key, value, null, Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes( 30 ), CacheItemPriority.Default, null );
		}

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        public void Add( string key, object value, CacheDependency dependency, TimeSpan slidingExpiration )
		{
			Add( key, value, dependency, Cache.NoAbsoluteExpiration, slidingExpiration, CacheItemPriority.Default, null );
		}

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        public void Add( string key, object value, CacheDependency dependency, DateTime absoluteExpiration )
		{
			Add( key, value, dependency, absoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null );
		}

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        public void Add( string key, object value, CacheDependency dependency, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback removeCallback )
		{
			Add( key, value, dependency, Cache.NoAbsoluteExpiration, slidingExpiration, priority, removeCallback );
		}

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        public void Add( string key, object value, CacheDependency dependency, DateTime absoluteExpiration, CacheItemPriority priority, CacheItemRemovedCallback removeCallback )
		{
			Add( key, value, dependency, absoluteExpiration, Cache.NoSlidingExpiration, priority, removeCallback );
		}

        /// <summary>
        /// Adds the specified key.
        /// </summary>
        public void Add(
			string key,
			object value,
			CacheDependency dependency,
			DateTime absoluteExpiration,
			TimeSpan slidingExpiration,
			CacheItemPriority priority,
			CacheItemRemovedCallback removeCallback )
		{
			CacheItem ci = new CacheItem();
			ci.Key = key;
			ci.Value = value;
			ci.CacheDependency = dependency;
			ci.AbsoluteExpiry = absoluteExpiration;
			ci.SlidingExpiry = slidingExpiration;
			ci.CacheItemPriority = priority;
			ci.RemovedCallback = removeCallback;

			lock( CacheItems )
			{
				if( CacheItems.ContainsKey( key ) )
				{
					Set( (CacheItem)CacheItems[ key ], ci );
				}
				else
				{
					CacheItems.Add( key, ci );
					SetCacheItemManagement( ci );
				}
			}
		}

		public object Remove( string key )
		{
			lock( CacheItems )
			{
				if( CacheItems.ContainsKey( key ) )
				{
					CacheItem retVal = (CacheItem)CacheItems[ key ];
					CacheItems.Remove( key );
					return retVal.Value;
				}
				else
				{
					return null;
				}
			}
		}

		public void Clear()
		{
			lock( CacheItems )
			{
				CacheItems.Clear();
			}
		}

		public bool Contains( string key )
		{
			return CacheItems.ContainsKey( key );
		}

		public ICollection Keys
		{
			get
			{
				lock( CacheItems )
				{
					return new ArrayList( CacheItems.Keys );
				}
			}
		}

		public ICollection Values
		{
			get
			{
				lock( CacheItems )
				{
					return new ArrayList( CacheItems.Values );
				}
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return CacheItems.GetEnumerator();
		}

		public int Count
		{
			get { return CacheItems.Count; }
		}

		private void Set( CacheItem cacheItem, object value )
		{
			if( cacheItem.RegisteredWaitHandle != null )
			{
				cacheItem.RegisteredWaitHandle.Unregister( null );
			}

			cacheItem.Value = value;

			TimeSpan timeout = cacheItem.SlidingExpiry;
			cacheItem.RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject( m_cacheExpiryWait, CacheItemExpired, cacheItem.Key, timeout, true );
		}

		private void Set( CacheItem origValue, CacheItem newValue )
		{
			if( origValue.RegisteredWaitHandle != null )
			{
				origValue.RegisteredWaitHandle.Unregister( null );
			}

			origValue.Value = newValue.Value;
			origValue.AbsoluteExpiry = newValue.AbsoluteExpiry;
			origValue.CacheDependency = newValue.CacheDependency;
			origValue.CacheItemPriority = newValue.CacheItemPriority;
			origValue.Key = newValue.Key;
			origValue.SlidingExpiry = newValue.SlidingExpiry;

			TimeSpan timeout = origValue.SlidingExpiry;
			origValue.RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject( m_cacheExpiryWait, CacheItemExpired, origValue.Key, timeout, true );
		}

		private void SetCacheItemManagement( CacheItem ci )
		{
			TimeSpan timeout = ci.SlidingExpiry;
			ci.RegisteredWaitHandle = ThreadPool.RegisterWaitForSingleObject( m_cacheExpiryWait, CacheItemExpired, ci.Key, timeout, true );
		}

		/// <summary>
		/// This is the callback called by the registered wait object when a cache item expires.
		/// It will also be called when the WaitHandle is signalled, so one must check for that 
		/// the item timed out before attempting to clear the cache. 
		/// </summary>
		/// <param name="state">This contains the key value for the cache item.</param>
		/// <param name="timedOut">Boolean to indicate that the item timed out or the wait handle was signalled.</param>
		private void CacheItemExpired( object state, bool timedOut )
		{
			//	check if timedout or set, to see whether to kill cache item or ignore
			if( timedOut && CacheItems.ContainsKey( state as string ) )
			{
				Remove( state as string );
			}
		}

		#region ICollection Members

		public void CopyTo( Array array, int index )
		{
			m_cacheItems.CopyTo( array, index );
		}

		public bool IsSynchronized
		{
			get { return m_cacheItems.IsSynchronized; }
		}

		public object SyncRoot
		{
			get { return m_cacheItems.SyncRoot; }
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException( "The method or operation is not implemented." );
		}

		#endregion
	}

}
