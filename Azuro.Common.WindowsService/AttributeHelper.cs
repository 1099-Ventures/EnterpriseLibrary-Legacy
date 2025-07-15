using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Azuro.Common.WindowsService
{
	internal static class AttributeHelper
	{
		internal static string Description(Type type)
		{
			DescriptionAttribute da = Azuro.Common.AttributeHelper.GetCustomAttribute<DescriptionAttribute>(type, true);
			return (da != null) ? da.Description : null;
		}

		internal static string DisplayName(Type type)
		{
			DisplayNameAttribute dna = Azuro.Common.AttributeHelper.GetCustomAttribute<DisplayNameAttribute>(type, true);
			return (dna != null) ? dna.DisplayName : null;
		}

		internal static string ServiceName(Type type)
		{
			ServiceNameAttribute sna = Azuro.Common.AttributeHelper.GetCustomAttribute<ServiceNameAttribute>(type, true);
			return (sna != null) ? sna.ServiceName : null;
		}

		internal static System.ServiceProcess.ServiceAccount ServiceAccount(Type type)
		{
			ServiceAccountAttribute saa = Azuro.Common.AttributeHelper.GetCustomAttribute<ServiceAccountAttribute>(type);
			return (saa != null) ? saa.ServiceAccount : System.ServiceProcess.ServiceAccount.LocalSystem;
		}

		internal static System.ServiceProcess.ServiceStartMode StartType(Type type)
		{
			ServiceStartTypeAttribute ssta = Azuro.Common.AttributeHelper.GetCustomAttribute<ServiceStartTypeAttribute>(type);
			return (ssta != null) ? ssta.StartType : System.ServiceProcess.ServiceStartMode.Manual;
		}

		internal static bool CanInteractWithDesktop(Type type)
		{
			ServiceInteractWithDesktopAttribute siwda = Azuro.Common.AttributeHelper.GetCustomAttribute<ServiceInteractWithDesktopAttribute>(type);
			return (siwda != null) ? siwda.InteractWithDesktop : false;
		}

		internal static bool StartOnInstall(Type type)
		{
			ServiceStartOnInstallAttribute ssoi = Azuro.Common.AttributeHelper.GetCustomAttribute<ServiceStartOnInstallAttribute>(type);
			return (ssoi != null) ? ssoi.Start : false;
		}
	}
}
