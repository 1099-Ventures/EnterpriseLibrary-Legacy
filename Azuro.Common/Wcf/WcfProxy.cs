using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace Azuro.Common.Wcf
{
	/// <summary>   
	/// Generic helper class for a WCF service proxy.    
	/// </summary>    
	/// <typeparam name="TProxy">The type of WCF service proxy to wrap.</typeparam>
	/// <typeparam name="TChannel">The type of WCF service interface to wrap.</typeparam>
	public class WcfProxy<TProxy, TChannel> : IDisposable
		where TProxy : ClientBase<TChannel>, new()
		where TChannel : class
	{        /// <summary>       
		/// Private instance of the WCF service proxy.
		/// </summary>        
		private TProxy _proxy;
		/// <summary>
		/// Gets the WCF service proxy wrapped by this instance.        
		/// </summary>        
		protected TProxy Proxy
		{
			get
			{
				if (_proxy != null)
				{
					return _proxy;
				}
				else
				{
					throw new ObjectDisposedException("WcfProxy");
				}
			}
		}
		/// <summary>        
		/// Constructs an instance.        
		/// </summary>
		protected WcfProxy()
		{
			_proxy = new TProxy();
		}
		/// <summary>        
		/// Disposes of this instance.        
		/// </summary>
		public void Dispose()
		{
			try
			{
				if (_proxy != null)
				{
					if (_proxy.State != CommunicationState.Faulted)
					{
						_proxy.Close();
					}
					else
					{
						_proxy.Abort();
					}
				}
			}
			catch (CommunicationException)
			{
				_proxy.Abort();
			}
			catch (TimeoutException)
			{ _proxy.Abort(); }
			catch (Exception)
			{
				_proxy.Abort();
				throw;
			}
			finally
			{
				_proxy = null;
			}
		}
	}
}

