namespace Azuro.Caching
{
	using System;
	using System.Web.Caching;
	using System.Threading;

	internal class CacheItem
	{
		private string m_key;
		private object m_value;
		private DateTime m_absoluteExpiry;
		private TimeSpan m_slidingExpiry;
		private CacheDependency m_dependency;
		private CacheItemPriority m_priority;
		private CacheItemRemovedCallback m_removedCallback;
		private RegisteredWaitHandle m_rwh = null;

		public string Key
		{
			get { return m_key; }
			set { m_key = value; }
		}

		public object Value
		{
			get { return m_value; }
			set { m_value = value; }
		}

		public DateTime AbsoluteExpiry
		{
			get { return m_absoluteExpiry; }
			set { m_absoluteExpiry = value; }
		}

		public TimeSpan SlidingExpiry
		{
			get { return m_slidingExpiry; }
			set { m_slidingExpiry = value; }
		}

		public CacheDependency CacheDependency
		{
			get { return m_dependency; }
			set { m_dependency = value; }
		}

		public CacheItemPriority CacheItemPriority
		{
			get { return m_priority; }
			set { m_priority = value; }
		}

		public RegisteredWaitHandle RegisteredWaitHandle
		{
			get { return m_rwh; }
			set { m_rwh = value; }
		}

		public CacheItemRemovedCallback RemovedCallback
		{
			get { return m_removedCallback; }
			set { m_removedCallback = value; }
		}
	}
}
