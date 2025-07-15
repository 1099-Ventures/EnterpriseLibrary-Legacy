using System;
using System.Collections.Generic;
using System.Text;

namespace Azuro.Common.WindowsService
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ServiceNameAttribute : Attribute
	{
		private string m_serviceName;

		public ServiceNameAttribute(string serviceName)
		{
			m_serviceName = serviceName;
		}

		public string ServiceName
		{
			get { return m_serviceName; }
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ServiceAccountAttribute : Attribute
	{
		private System.ServiceProcess.ServiceAccount m_svcAccount;

		public ServiceAccountAttribute(System.ServiceProcess.ServiceAccount svcAccount)
		{
			m_svcAccount = svcAccount;
		}

		public System.ServiceProcess.ServiceAccount ServiceAccount
		{
			get { return m_svcAccount; }
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ServiceStartTypeAttribute : Attribute
	{
		private System.ServiceProcess.ServiceStartMode m_startMode;

		public ServiceStartTypeAttribute(System.ServiceProcess.ServiceStartMode startMode)
		{
			m_startMode = startMode;
		}

		public System.ServiceProcess.ServiceStartMode StartType
		{
			get { return m_startMode; }
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ServiceInteractWithDesktopAttribute : Attribute
	{
		private bool m_canInteract;

		public ServiceInteractWithDesktopAttribute(bool canInteract)
		{
			m_canInteract=canInteract;
		}

		public bool InteractWithDesktop
		{
			get { return m_canInteract; }
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public sealed class ServiceStartOnInstallAttribute : Attribute
	{
		public ServiceStartOnInstallAttribute(bool start)
		{
			Start = start;
		}

		public bool Start { get; set; }
	}
}
