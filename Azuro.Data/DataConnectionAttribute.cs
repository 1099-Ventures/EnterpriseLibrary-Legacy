using System;
using System.Collections.Generic;
using System.Text;

namespace Azuro.Data
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class DataConnectionAttribute : Attribute
	{
		private string m_cfg;

		public string Configuration
		{
			get { return m_cfg; }
			set { m_cfg = value; }
		}

		public DataConnectionAttribute(string configuration)
		{
			m_cfg = configuration;
		}
	}
}
