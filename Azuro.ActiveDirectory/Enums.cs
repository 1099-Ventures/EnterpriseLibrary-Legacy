using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Azuro.ActiveDirectory
{
	[Flags]
	public enum PasswordFlags
	{
		Script = 0x0001,
		AccountDisabled = 0x0002,
		HomeDirRequired = 0x0008,
		Lockout = 0x0010,
		PasswordNotRequired = 0x0020,
		PasswordCantChange = 0x0040,
		EncryptedTextPasswordAllowed = 0x0080,
		TempDuplicateAccount = 0x0100,
		NormalAccount = 0x0200,
		InterdomainTrustAccount = 0x0800,
		WorkstationTrustAccount = 0x1000,
		ServerTrustAccount = 0x2000,
		DontExpirePassword = 0x10000,
		MnsLogonAccount = 0x20000,
		SmartcardRequired = 0x40000,
		TrustedForDelegation = 0x80000,
		NotDelegated = 0x100000,
		UseDesKeyOnly = 0x200000,
		DontRequirePreAuth = 0x400000,
		PasswordExpired = 0x800000,
		TrustedToAuthForDelegation = 0x1000000,
	}

	public enum ObjectClass
	{
		User,
		Group,
		Computer,
		OU,
	}

	public enum ReturnType
	{
		DistinguishedName,
		ObjectGUID,
	}
}
