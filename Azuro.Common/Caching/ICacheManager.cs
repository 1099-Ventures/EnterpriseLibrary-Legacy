using System;
using System.Collections;
using System.Web.Caching;

namespace Azuro.Caching
{
    public interface ICacheManager : ICollection, IEnumerable
    {
        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> with the specified key.
        /// </summary>
        /// <value></value>
        object this[string key] { get; set; }
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void Add( string key, object value );
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        void Add( string key, object value, CacheDependency dependency, TimeSpan slidingExpiration );
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        void Add( string key, object value, CacheDependency dependency, DateTime absoluteExpiration );
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        void Add( string key, object value, CacheDependency dependency, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback removeCallback );
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        void Add( string key, object value, CacheDependency dependency, DateTime absoluteExpiration, CacheItemPriority priority, CacheItemRemovedCallback removeCallback );
        /// <summary>
        /// Adds the specified key.
        /// </summary>
        void Add( string key, object value, CacheDependency dependency, DateTime absoluteExpiration, TimeSpan slidingExpiration, CacheItemPriority priority, CacheItemRemovedCallback removeCallback );


        /// <summary>
        /// Clears this instance.
        /// </summary>
        void Clear();
        /// <summary>
        /// Determines whether [contains] [the specified key].
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified key]; otherwise, <c>false</c>.
        /// </returns>
        bool Contains( string key );
        /// <summary>
        /// Gets the keys.
        /// </summary>
        /// <value>The keys.</value>
        ICollection Keys { get; }
        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        ICollection Values { get; }
        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        new IDictionaryEnumerator GetEnumerator();
        /// <summary>
        /// Removes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        object Remove( string key );
    }
}
