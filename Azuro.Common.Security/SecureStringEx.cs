using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Azuro.Common.Security
{
	public class SecureStringEx:IDisposable
	{
		private SecureString _secureString;

		public SecureString SecureString { get { return _secureString; } }

		public SecureStringEx(string s)
		{
			_secureString = new SecureString();
			foreach (char c in s)
			{
				_secureString.AppendChar(c);
			}
		}
		public SecureStringEx(SecureString ss)
		{
			//	Should I copy or not?
			_secureString = ss;
			//_secureString = ss.Copy();
		}

		[SecurityCritical]
		unsafe public override string ToString()
		{
			string str;
			using (SecureString str2 = _secureString.Copy())
			{
				IntPtr s = Marshal.SecureStringToBSTR(str2);
				try
				{
					str = new string((char*)s);
				}
				finally
				{
					Marshal.ZeroFreeBSTR(s);
				}
			}
			return str;
		}

		public static implicit operator SecureString(SecureStringEx ssex)
		{
			return ssex._secureString.Copy();
		}

		public void Dispose()
		{
			_secureString.Clear();
			_secureString.Dispose();
		}
	}
}
