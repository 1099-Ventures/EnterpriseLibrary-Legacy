using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Configuration.Install;
using System.Management;

namespace Azuro.Common.WindowsService
{
	public class WindowsServiceInstaller<T> : Installer where T : ServiceBase
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		private System.ServiceProcess.ServiceInstaller m_svcInstaller;
		private System.ServiceProcess.ServiceProcessInstaller m_svcProcInstaller;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		public WindowsServiceInstaller()
		{
			this.m_svcInstaller = new System.ServiceProcess.ServiceInstaller();
			this.m_svcProcInstaller = new System.ServiceProcess.ServiceProcessInstaller();
			// 
			// m_svcInstaller
			// 
			this.m_svcInstaller.Description = AttributeHelper.Description(typeof(T));
			this.m_svcInstaller.DisplayName = AttributeHelper.DisplayName(typeof(T));
			this.m_svcInstaller.ServiceName = AttributeHelper.ServiceName(typeof(T));
			this.m_svcInstaller.StartType = AttributeHelper.StartType(typeof(T));
			this.m_svcInstaller.AfterInstall += new InstallEventHandler(SvcInstaller_AfterInstall);
			// 
			// m_svcProcInstaller
			//
			this.m_svcProcInstaller.Account = AttributeHelper.ServiceAccount(typeof(T));
			if (m_svcProcInstaller.Account == ServiceAccount.User)
			{
				//	TODO: Add functionality for custom action to prompt for service user id and password
				throw new NotImplementedException("The ability to set a User Account has not yet been implemented in the installer.");
			}
			else
			{
				this.m_svcProcInstaller.Password = null;
				this.m_svcProcInstaller.Username = null;
			}

			// 
			// Installer Helper
			// 
			this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.m_svcInstaller,
            this.m_svcProcInstaller});
		}

		void SvcInstaller_AfterInstall(object sender, InstallEventArgs e)
		{
			if (AttributeHelper.CanInteractWithDesktop(typeof(T)))
			{
				try
				{
					ConnectionOptions coOptions = new ConnectionOptions();
					coOptions.Impersonation = ImpersonationLevel.Impersonate;
					ManagementScope mgmtScope = new System.Management.ManagementScope(@"root\CIMV2", coOptions);
					mgmtScope.Connect();
					ManagementObject wmiService;
					wmiService = new ManagementObject("Win32_Service.Name='" + m_svcInstaller.ServiceName + "'");
					ManagementBaseObject InParam = wmiService.GetMethodParameters("Change");
					InParam["DesktopInteract"] = true;
					ManagementBaseObject OutParam = wmiService.InvokeMethod("Change", InParam, null);
				}
				catch (Exception ex)
				{
					//	TODO: Log Error
					Console.WriteLine("Now what: [{0}]", ex);
				}
			}
			if(AttributeHelper.StartOnInstall(typeof(T)))
			{
				try
				{
					ServiceController scm = new ServiceController(AttributeHelper.ServiceName(typeof(T)));
					scm.Start();
				}
				catch (Exception ex)
				{
					//	TODO: Log Error
					Console.WriteLine("Now what: [{0}]", ex);
				}
			}
		}
	}
}
