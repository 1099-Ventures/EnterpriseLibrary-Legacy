using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web;
using System.Collections;
using Azuro.Common;
using System.Web.Caching;
using Azuro.Caching;

namespace Azuro.Caching
{
	public class HttpCache : ICacheManager
	{
		#region ICacheManager Members

		public object this[ string key ]
		{
			get
			{
				return HttpRuntime.Cache[ key ];
			}
			set
			{
				HttpRuntime.Cache[ key ] = value;
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
            HttpRuntime.Cache.Add(
                            key,
                            value,
                            dependency,
                            absoluteExpiration,
                            slidingExpiration,
                            priority,
                            removeCallback );
        }

		public void Clear()
		{
			for( IDictionaryEnumerator en = HttpRuntime.Cache.GetEnumerator(); en.MoveNext(); )
				HttpRuntime.Cache.Remove( (string)en.Key );
		}

		public bool Contains( string key )
		{
			return (HttpRuntime.Cache[ key ] != null);
		}

		public ICollection Keys
		{
			get
			{
				ArrayList al = new ArrayList( HttpRuntime.Cache.Count );
				for( IDictionaryEnumerator en = HttpRuntime.Cache.GetEnumerator(); en.MoveNext(); )
					al.Add( en.Key );
				return al;
			}
		}

		public ICollection Values
		{
			get
			{
				ArrayList al = new ArrayList( HttpRuntime.Cache.Count );
				for( IDictionaryEnumerator en = HttpRuntime.Cache.GetEnumerator(); en.MoveNext(); )
					al.Add( en.Value );
				return al;
			}
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			return HttpRuntime.Cache.GetEnumerator();
		}

        public object Remove(string key)
        {
            return HttpRuntime.Cache.Remove(key);
        }

	    #endregion

		#region ICollection Members

		public void CopyTo( Array array, int index )
		{
			ArrayList al = new ArrayList( HttpRuntime.Cache.Count );
			for( IDictionaryEnumerator en = HttpRuntime.Cache.GetEnumerator(); en.MoveNext(); )
				al.Add( en.Value );
			al.CopyTo( array, index );
		}

		public int Count
		{
			get { return HttpRuntime.Cache.Count; }
		}

		public bool IsSynchronized
		{
            get { throw new NotImplementedException( "The method or operation is not implemented." ); }
		}

		public object SyncRoot
		{
            get { throw new NotImplementedException( "The method or operation is not implemented." ); }
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
