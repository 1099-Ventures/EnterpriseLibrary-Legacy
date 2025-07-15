using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Azuro.Configuration
{
	//	TODO: See SectionInformation.Source [JU: What was I thinking here?]
	//	TODO: Add Option to Watch for Config Changes

	/// <summary>
	/// Implement the IConfigurationSectionHandler for any Configuration type.
	/// </summary>
	/// <typeparam name="T">The configuration section type to attempt to deserialize.</typeparam>
	public class ConfigurationSectionHandler<T> : IConfigurationSectionHandler where T : AConfigurationSection
	{

		#region IConfigurationSectionHandler Members

		/// <summary>
		/// Create a Configuration Section of type T. At this stage, allow T to be of any type,
		/// but possibly in the future, implement a Config Section with base members etc.
		/// </summary>
		/// <param name="parent">The parent of this configuration section.</param>
		/// <param name="configContext">The context of this configuration section.</param>
		/// <param name="section">The section to deserialize as a config element.</param>
		/// <returns>A configuration section of type T.</returns>
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			if (section == null) return Activator.CreateInstance<T>();
			XmlSerializer xs = new XmlSerializer(typeof(T));
			T cfg = (T)xs.Deserialize(new XmlTextReader(new StringReader(section.OuterXml)));
			return LoadConfigFromFile(cfg, xs);
		}

		/// <summary>
		/// Update the server config file.
		/// </summary>
		/// <param name="config">Configuration object.</param>
		public static void UpdateConfiguration(T config)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));

			Stream ms = new MemoryStream();
			XmlWriter xmlWriter = new XmlTextWriter(ms, Encoding.UTF8);
			xmlSerializer.Serialize(xmlWriter, config);
			ms.Position = 0;

			XmlDocument udoc = new XmlDocument();
			udoc.Load(ms);
			xmlWriter.Close();

			XmlDocument doc = new XmlDocument();
			doc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

			//	TODO: Check for Xml Filename set
			XmlRootAttribute[] xra = (XmlRootAttribute[])typeof(T).GetCustomAttributes(typeof(XmlRootAttribute), false);
			if (xra.Length > 0)
			{
				XmlNode cfgNode = doc.DocumentElement.SelectSingleNode(xra[0].ElementName);
				XmlNode newCfgNode = doc.ImportNode(udoc.DocumentElement, true);
				doc.DocumentElement.ReplaceChild(newCfgNode, cfgNode);
				doc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
			}
			else
				throw new InvalidOperationException("Can only serialize a Config section that has the XmlRootAttribute set.");
		}

		/// <summary>
		/// Loads the config from file.
		/// </summary>
		/// <param name="cfg">The CFG.</param>
		/// <param name="xs">The xs.</param>
		/// <returns></returns>
		private T LoadConfigFromFile(T cfg, XmlSerializer xs)
		{
			AConfigurationSection cs = cfg as AConfigurationSection;
			if (cs != null)
			{
				if (!string.IsNullOrEmpty(cs.FileName))
				{
					StreamReader sr = null;
					foreach (string path in GetExecutionPathList())
					{
						string filePath = Path.Combine(path, cs.FileName);
						sr = SafeFileOpen(filePath);

						if (sr != null)
							break;
					}

					if (sr == null)
						throw new ArgumentException(string.Format("The configuration file [{0}] specified for section [{1}] could not be found.",
										cs.FileName, typeof(T)));

					cfg = (T)xs.Deserialize(sr);
				}
			}
			return cfg;
		}

		/// <summary>
		/// Gets the execution path list.
		/// </summary>
		/// <returns></returns>
		private List<string> GetExecutionPathList()
		{
			List<string> path = new List<string>();
			//	Current execution path
			path.Add(".\\");
			//	Configuration path root
			path.Add(Path.GetDirectoryName(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile));
			//	Application base directory
			path.Add(AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
			//	Application Private Binary Paths
			if (!string.IsNullOrEmpty(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath))
				path.AddRange(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath.Split(';'));

			return path;
		}

		/// <summary>
		/// Safely opens a file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns></returns>
		private StreamReader SafeFileOpen(string filePath)
		{
			if (File.Exists(filePath))
				return File.OpenText(filePath);
			return null;
		}

		#endregion
	}
}
