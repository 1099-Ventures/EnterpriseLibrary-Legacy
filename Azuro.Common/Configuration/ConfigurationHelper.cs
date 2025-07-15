using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Reflection;
using System.Xml;

namespace Azuro.Common.Configuration
{
	public class ConfigurationHelper
	{
		public static T GetSection<T>(string section)
		{
			T t = (T)ConfigurationManager.GetSection(section);
			if (t == null)
			{
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				var executionPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().ManifestModule.FullyQualifiedName);
				foreach (Assembly a in assemblies)
				{
					if (a != null)
					{
						WriteLogEntry("[{0}] - [{1}]", a.ManifestModule.FullyQualifiedName, a.GlobalAssemblyCache);
						if (!System.IO.File.Exists(a.ManifestModule.FullyQualifiedName))
						{
							WriteLogEntry("Filename for assembly [{0}] is not valid", a.FullName);
							continue;
						}

						if (a.GlobalAssemblyCache)
						{
							var path = System.IO.Path.Combine(executionPath, a.ManifestModule.Name + ".config");
							WriteLogEntry("Path = [{0}]", path);
							t = GetExeSection<T>(path, section);
						}
						else
						{
							t = GetSection<T>(a, section);
						}
						if (t != null)
							break;
					}
					else
						WriteLogEntry("Assembly is NULL, this shouldn't happen");
				}
				WriteLogEntry("t==null [{0}]", t == null);
			}

			return t;
		}

		private static T GetExeSection<T>(string path, string section)
		{
			System.Configuration.Configuration cfg = System.Configuration.ConfigurationManager.OpenMappedExeConfiguration(new System.Configuration.ExeConfigurationFileMap { ExeConfigFilename = path, }, System.Configuration.ConfigurationUserLevel.None);
			return LoadConfiguration<T>(cfg, section);
		}

		private static T GetSection<T>(Assembly assembly, string section)
		{
			System.Configuration.Configuration cfg = ConfigurationManager.OpenExeConfiguration(assembly.ManifestModule.FullyQualifiedName);
			return LoadConfiguration<T>(cfg, section);
		}

		private static T LoadConfiguration<T>(System.Configuration.Configuration cfg, string section)
		{
			return cfg.HasFile ? (T)LoadXmlConfiguration(cfg.GetSection(section)) : default(T);
		}

		private static object LoadXmlConfiguration(System.Configuration.ConfigurationSection cs)
		{
			if (cs == null)
			{
				WriteLogEntry("Configuration Section is NULL");
				return null;
			}
			IConfigurationSectionHandler csh = Util.CreateObject<IConfigurationSectionHandler>(cs.SectionInformation.Type);
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(cs.SectionInformation.GetRawXml());
			return csh.Create(null, null, doc.FirstChild);
		}

		[System.Diagnostics.Conditional("DEBUG")]
		private static void WriteLogEntry(string message, params object[] args)
		{
			if (!System.Diagnostics.EventLog.SourceExists("Azuro.EnterpriseLibrary"))
				System.Diagnostics.EventLog.CreateEventSource("Azuro.EnterpriseLibrary", "Application");
			System.Diagnostics.EventLog.WriteEntry("Azuro.EnterpriseLibrary", string.Format(message, args), System.Diagnostics.EventLogEntryType.Information);
		}
	}
}
