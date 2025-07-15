using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.DirectoryServices;
using System.DirectoryServices.ActiveDirectory;
using System.Security;
using System.DirectoryServices.AccountManagement;

namespace Azuro.ActiveDirectory
{
	public class UserEntity
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string FirstName { get; set; }
		public string Surname { get; set; }
		public string EmailAddress { get; set; }
	}

	public class ADHelper
	{
		private string _ldapDomain;
		private string LdapDomain
		{
			get { return _ldapDomain ?? (_ldapDomain = Environment.UserDomainName); }
			set { _ldapDomain = value; }
		}

		private string AccountPassword { get; set; }
		private string AccountUserName { get; set; }

		public ADHelper()
		{
			LdapDomain = Environment.UserDomainName;
		}

		public ADHelper(string ldapDomain, string username, string password)
		{
			LdapDomain = ldapDomain;
			AccountPassword = password;
			AccountUserName = username;
		}

		public void ResetPassword(string user, string password)
		{
			ResetPassword(user, password, LdapDomain);
		}

		public void ResetPassword(string user, string password, string ldapDomain)
		{
			using (DirectoryEntry de = GetDirectoryObject(ObjectClass.User, user, ldapDomain))
			{
				de.Invoke("SetPassword", new object[] { password });
				de.Properties["LockOutTime"].Value = 0; //unlock account
				de.CommitChanges();
				de.Close();
			}
		}

		public string CreateUserAccount(UserEntity ue)
		{
			return CreateUserAccount(ue, LdapDomain);
		}

		public string CreateUserAccount(UserEntity ue, string ldapPath)
		{
			string guid = string.Empty;

			using (DirectoryEntry dirEntry = new DirectoryEntry(ldapPath, AccountUserName, AccountPassword))
			{
				using (DirectoryEntry newUser = dirEntry.Children.Add("CN=" + ue.UserName, "user"))
				{
					newUser.Properties["samAccountName"].Value = ue.UserName;
					newUser.Properties["mail"].Value = ue.EmailAddress;
					newUser.Properties["givenName"].Value = ue.FirstName;
					newUser.Properties["sn"].Value = ue.Surname;
					newUser.Properties["displayName"].Value = ue.FirstName + " " + ue.Surname;
					newUser.CommitChanges();
					guid = newUser.Guid.ToString();

					newUser.Invoke("SetPassword", new object[] { ue.Password });
					newUser.CommitChanges();

					UnsetAccountFlags(newUser, PasswordFlags.AccountDisabled);
					dirEntry.Close();
					newUser.Close();
				}
			}
			return guid;
		}

		public DirectoryEntry GetDirectoryObject(ObjectClass objectClass, string objectName)
		{
			return GetDirectoryObject(objectClass, objectName, LdapDomain);
		}

		public DirectoryEntry GetDirectoryObject(ObjectClass objectClass, string objectName, string ldapDomain )
		{
			string connectionPrefix;
			if (ldapDomain.IndexOf("LDAP://") >= 0)
			{
				connectionPrefix = ldapDomain;
			}
			else
			{
				connectionPrefix = "LDAP://" + ldapDomain;
			}

			//	Environment.Username, _SPAccPwd

			using (DirectoryEntry entry = new DirectoryEntry(connectionPrefix, AccountUserName, AccountPassword))
			{
				using (DirectorySearcher mySearcher = new DirectorySearcher(entry))
				{
					switch (objectClass)
					{
						case ObjectClass.User:
							mySearcher.Filter = "(&(objectClass=user)(|(cn=" + objectName + ")(sAMAccountName=" + objectName + ")))";
							break;
						case ObjectClass.Group:
							mySearcher.Filter = "(&(objectClass=group)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
							break;
						case ObjectClass.Computer:
							mySearcher.Filter = "(&(objectClass=computer)(|(cn=" + objectName + ")(dn=" + objectName + ")))";
							break;
						case ObjectClass.OU:
							mySearcher.Filter = "(&(objectClass=organizationalUnit)(|(OU=" + objectName + ")(dn=" + objectName + ")))";
							break;
					}
					SearchResult result = mySearcher.FindOne();

					if (result == null)
					{
						throw new NullReferenceException("unable to locate the distinguishedName for the object " + objectName + " in the " + ldapDomain + " domain");
					}
					DirectoryEntry directoryObject = result.GetDirectoryEntry();

					entry.Close();

					return directoryObject;
				}
			}
		}

		public string GetObjectDistinguishedName(ObjectClass objectClass, string objectName, string ldapDomain)
		{
			using (DirectoryEntry entry = GetDirectoryObject(objectClass, objectName, ldapDomain))
			{
				string distinguishedName = "LDAP://" + entry.Properties["distinguishedName"].Value;
				entry.Close();
				return distinguishedName;
			}
		}

		public PasswordFlags SetAccountFlags(string user, PasswordFlags flags)
		{
			return SetAccountFlags(user, Environment.UserDomainName, flags);
		}

		public PasswordFlags SetAccountFlags(string user, string ldapDomain, PasswordFlags flags)
		{
			using (DirectoryEntry de = GetDirectoryObject(ObjectClass.User, user, ldapDomain))
			{
				return SetAccountFlags(de, flags);
			}
		}

		private PasswordFlags SetAccountFlags(DirectoryEntry de, PasswordFlags flags)
		{
			int val = (int)de.Properties["userAccountControl"].Value;
			val |= (int)flags;
			de.Properties["userAccountControl"].Value = val;
			de.CommitChanges();
			de.Close();

			return (PasswordFlags)val;
		}

		public PasswordFlags UnsetAccountFlags(string user, string ldapDomain, PasswordFlags flags)
		{
			using (DirectoryEntry de = GetDirectoryObject(ObjectClass.User, user, ldapDomain))
			{
				return UnsetAccountFlags(de, flags);
			}
		}

		private PasswordFlags UnsetAccountFlags(DirectoryEntry de, PasswordFlags flags)
		{
			int val = (int)de.Properties["userAccountControl"].Value;
			val &= (int)~flags;
			de.Properties["userAccountControl"].Value = val;
			de.CommitChanges();
			de.Close();

			return (PasswordFlags)val;
		}

		public string GetDistinguishedName(string name)
		{
			var o = GetDirectoryObject(ObjectClass.User, name, Environment.UserDomainName);

			return o.Path;
		}

		//	This is a cute idea, but SecureString is not the best implementation for it
		//public bool Authenticate(string userName, SecureString password)
		//{
		//	using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, LdapDomain))
		//	{
		//		using (var ssex = new Azuro.Common.Security.SecureStringEx(password))
		//		{
		//			return pc.ValidateCredentials(userName, ssex.ToString());
		//		}
		//	}
		//}

		public bool Authenticate(string userName, string password)
		{
			using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, LdapDomain))
			{
				return pc.ValidateCredentials(userName, password);
			}
		}
	}
}
