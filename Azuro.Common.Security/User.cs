using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Security.Principal;

namespace Azuro.Security
{
	/// <summary>
	/// Summary description for User.
	/// </summary>

	public class User : IDisposable
	{
		private IntPtr	m_token = IntPtr.Zero;

		private string						m_userName,
											m_password,
											m_domain;
		private bool						m_loggedOn;
		private LogonTypes					m_logonType = LogonTypes.NewCredentials;
		private WindowsImpersonationContext m_impCtxt;

		public string Name
		{
			get { return m_userName; }
			set { m_userName = value; }
		}

		public string Password
		{
			get { return m_password; }
			set { m_password = value; }
		}

		public string Domain
		{
			get { return m_domain; }
			set { m_domain = value; }
		}

		public LogonTypes LogonType
		{
			get { return m_logonType; }
			set { m_logonType = value; }
		}

		public User(string userName, string password, string domain)
		{
			m_userName = userName;
			m_password = password;
			m_domain = domain;
		}

		public void Logon()
		{
			m_loggedOn = LogonUser(m_userName, m_domain, m_password, LogonType, LogonProviders.Default, out m_token);
			if ( !m_loggedOn )
				throw new Win32Exception(Marshal.GetLastWin32Error());
		}

		public void Impersonate()
		{
			if ( !m_loggedOn )
				Logon();
			m_impCtxt = WindowsIdentity.Impersonate(m_token);
		}

		public void Dispose()
		{
			if ( m_token != IntPtr.Zero )
			{
				CloseHandle(m_token);
				m_token = IntPtr.Zero;
				m_impCtxt.Undo();
			}
		}

		#region Impersonation Import
		[DllImport("advapi32.dll", SetLastError=true)]
		static extern bool LogonUser(string principal,
								string authority,
								string password,
								LogonTypes logonType,
								LogonProviders logonProvider,
								out IntPtr token);

		[DllImport("kernel32.dll", SetLastError=true)]
		static extern bool CloseHandle(IntPtr handle);
		
		public enum LogonTypes : uint 
		{
			Interactive = 2,
			Network,
			Batch,
			Service,
			NetworkCleartext = 8,
			NewCredentials
		}

		public enum LogonProviders : uint 
		{
			Default = 0, // default for platform (use this!)
			WinNT35,     // sends smoke signals to authority
			WinNT40,     // uses NTLM
			WinNT50      // negotiates Kerberos or NTLM
		}
		#endregion
	}
}
