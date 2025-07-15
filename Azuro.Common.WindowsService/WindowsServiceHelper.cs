using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using NLog;

namespace Azuro.Common.WindowsService
{
	[Description("Azuro Windows Service Helper")]
	[DisplayName("Azuro Windows Service Helper")]
	[ServiceName("Azuro.Windows.Service.Helper")]
	[ServiceStartType(ServiceStartMode.Manual)]
	[ServiceInteractWithDesktop(false)]
	public abstract class WindowsServiceHelper<T> : ServiceBase where T : WindowsServiceHelper<T>
	{
		private static T m_instance = null;
		private static Logger Logger = LogManager.GetCurrentClassLogger();

		public Azuro.Common.CommandLine.Arguments CmdArgs { get; set; }

		static WindowsServiceHelper()
		{
			m_instance = Activator.CreateInstance<T>();
		}

		public static string ServiceDescription
		{
			get { return AttributeHelper.Description(typeof(T)); }
		}

		//	TODO: JU - 2008/10/27 - Hiding base class property, have to fix this
		public static string ServiceInstallationName
		{
			get { return AttributeHelper.ServiceName(typeof(T)); }
		}

		public static void Start(string[] args)
		{
			if (System.Environment.UserInteractive)
			{
				if (args != null && args.Length > 0)
					if (m_instance.ProcessCommandLine(args))
						return;

				if (!CheckIsRunning())
				{
					m_instance.OnStart(null);

					m_instance.PrintConsoleCommands();

					while (m_instance.ProcessConsoleCommands()) ;

					m_instance.OnStop();
				}
				else
				{
					Logger.Fatal("[{0}] service already running... Cannot continue.", ServiceDescription);
				}
			}
			else
			{
				try
				{
					using (ServiceController sc = new ServiceController(ServiceInstallationName))
					{
						if (sc.Status == ServiceControllerStatus.StartPending)
						{
							ServiceBase[] ServicesToRun;

							ServicesToRun = new ServiceBase[] { m_instance };

							ServiceBase.Run(ServicesToRun);
						}
						else if (sc.Status == ServiceControllerStatus.Running)
						{
							Logger.Fatal("[{0}] service already running... Cannot continue.", ServiceDescription);
							return;
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error(ex, "Failed to start service");
				}
			}
		}

		/// <summary>
		/// Print out a menu in the console.
		/// </summary>
		protected abstract void PrintConsoleCommands();

		/// <summary>
		/// Implement this method to handle messages from the console.
		/// Make sure it returns false when the application must exit
		/// </summary>
		/// <returns>true if the application must continue running, else false.</returns>
		protected abstract bool ProcessConsoleCommands();

		protected virtual bool ProcessCommandLine(string[] args)
		{
			//	Check for the following switches
			CmdArgs = new Azuro.Common.CommandLine.Arguments(args);

			if (CmdArgs["i"] != null || CmdArgs["install"] != null) //	/I to install
			{
				if (!CheckIsInstalled())
				{
					System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { GetStartAssemblyLocation() });
					StartService();
					return true;
				}
			}
			else if (CmdArgs["u"] != null || CmdArgs["uninstall"] != null)  //	/U to uninstall
			{
				if (CheckIsInstalled()) //	TODO: Check this code, doesn't seem to be working.
				{
					System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/u", GetStartAssemblyLocation() });
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Determine whether the service is running
		/// </summary>
		/// <returns>true if running</returns>
		protected static bool CheckIsRunning()
		{
			bool isRunning = false;
			if (CheckIsInstalled())
			{
				System.ServiceProcess.ServiceController controller = GetController();

				if (controller != null)
				{
					System.ServiceProcess.ServiceControllerStatus status = controller.Status;
					isRunning = (status == System.ServiceProcess.ServiceControllerStatus.Running);
				}
			}
			return isRunning;
		}

		/// <summary>
		/// determine if the service is installed
		/// </summary>
		/// <returns>
		/// true if the service is installed
		/// </returns>
		protected static bool CheckIsInstalled()
		{
			System.ServiceProcess.ServiceController[] controllers = System.ServiceProcess.ServiceController.GetServices();

			foreach (var c in controllers)
			{
#if DEBUG
				Logger.Trace("{0} - {1}", c.DisplayName, ServiceInstallationName);
#endif
				if (c.DisplayName.Equals(ServiceInstallationName))
					return true;
			}

			return false;
		}

		/// <summary>
		/// get the service controller for this service instance
		/// </summary>
		/// <returns>the service controller</returns>
		protected static System.ServiceProcess.ServiceController GetController()
		{
			return new System.ServiceProcess.ServiceController(ServiceInstallationName);
		}

		/// <summary>
		/// start the service 
		/// </summary>
		public static void StartService()
		{
			if (CheckIsInstalled() && !CheckIsRunning())
			{
				System.ServiceProcess.ServiceController controller = GetController();

				if (controller != null)
				{
					controller.Start();
				}
			}
		}

		/// <summary>
		/// stop the service 
		/// </summary>
		public static void StopService()
		{
			if (CheckIsInstalled())
			{
				System.ServiceProcess.ServiceController controller = GetController();

				if (controller != null)
				{
					if (controller.Status == System.ServiceProcess.ServiceControllerStatus.Running ||
						controller.Status == System.ServiceProcess.ServiceControllerStatus.StartPending)
					{
						controller.Stop();
					}
				}
			}
		}

		/// <summary>
		/// get the location of the start assembly
		/// </summary>
		/// <returns>
		/// the location of the start assembly
		/// </returns>
		private static string GetStartAssemblyLocation()
		{
			//	TODO: JU - 2011/11/29 - This is not in the right place, and may in fact exist somewhere else too
			string location = string.Empty;

			System.Reflection.Assembly entryAssembly = System.Reflection.Assembly.GetEntryAssembly();

			if (entryAssembly != null)
			{
				location = entryAssembly.Location;
			}

			return location;
		}
	}

}