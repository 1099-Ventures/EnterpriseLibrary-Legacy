using System;
using System.Collections;
using System.Web.Caching;
using System.Configuration;
using Azuro.Common;

namespace Azuro.Caching
{
    public class CacheManager
    {
        private static ICacheManager m_instance;
        private static System.Threading.Mutex m_singletonMutex = new System.Threading.Mutex();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ICacheManager Instance
        {
            get
            {
                m_singletonMutex.WaitOne();
                if( m_instance == null )
                {
                    CacheConfigSection cc = (CacheConfigSection)ConfigurationManager.GetSection( "Azuro.Caching" );

                    if( cc != null )
                    {
                        switch( cc.Strategy )
                        {
                            case Strategy.Custom:
                                m_instance = Util.CreateObject<ICacheManager>( cc.Assembly, cc.Type );
                                break;
                            case Strategy.Http:
                                m_instance = CreateHttpCache();
                                break;
                            case Strategy.Memory:
                                m_instance = CreateInMemoryCache();
                                break;
                        }
                    }
                    else
                    {
                        m_instance = CreateInMemoryCache();
                    }
                }
                m_singletonMutex.ReleaseMutex();
                return m_instance;
            }
        }

        private static ICacheManager CreateHttpCache()
        {
            Type cacheType = Type.GetType( "Azuro.Caching.HttpCache, Azuro.Caching" );
            return (ICacheManager)Activator.CreateInstance( cacheType );
        }

        private static ICacheManager CreateInMemoryCache()
        {
            Type cacheType = Type.GetType("Azuro.Caching.InMemoryCache, Azuro.Caching");
            return (ICacheManager)Activator.CreateInstance( cacheType );
        }
    }
}
