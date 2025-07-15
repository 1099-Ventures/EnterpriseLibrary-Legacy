using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Data;

namespace Azuro.Data
{
	[XmlRoot("SqlCommands")]
	public class SqlTextCommand
	{
		[XmlIgnore]
		public Dictionary<string, SqlTextProcedure> Procedures = new Dictionary<string, SqlTextProcedure>();
		[XmlArray("Procedures")]
		[XmlArrayItem("Procedure", Type = typeof(SqlTextProcedure))]
		public SqlTextProcedure[] __Procedures
		{
			get
			{
				SqlTextProcedure[] retVal = new SqlTextProcedure[Procedures.Count];
				int i = 0;
				foreach (KeyValuePair<string, SqlTextProcedure> kv in Procedures)
				{
					retVal[i++] = kv.Value;
				}
				return retVal;
			}
			set
			{
				Procedures.Clear();
				foreach (SqlTextProcedure xsp in value)
				{
					Procedures.Add(xsp.ProcedureName, xsp);
				}
			}
		}
	}

	public class SqlTextProcedure
	{
		[XmlAttribute("name")]
		public string								ProcedureName;
		[XmlIgnore]
		public Dictionary<string, SqlTextParameter> Parameters = new Dictionary<string, SqlTextParameter>();
		[XmlArray("Parameters")]
		[XmlArrayItem("Parameter", Type = typeof(SqlTextParameter))]
		public SqlTextParameter[] __Parameters
		{
			get
			{
				SqlTextParameter[] retVal = new SqlTextParameter[Parameters.Count];
				int i = 0;
				foreach (KeyValuePair<string, SqlTextParameter> kv in Parameters)
				{
					retVal[i++] = kv.Value;
				}
				return retVal;
			}
			set
			{
				Parameters.Clear();
				foreach (SqlTextParameter xsp in value)
				{
					Parameters.Add(xsp.Name, xsp);
				}
			}
		}
		[XmlElement("CommandText")]
		public string								CommandText;
	}

	public class SqlTextParameter
	{
		[XmlAttribute("name")]
		public string	Name;
		[XmlIgnore]
		public Type DataType = typeof(string);
		[XmlAttribute("direction")]
		public ParameterDirection Direction = ParameterDirection.Input;
		[XmlAttribute("type")]
		public string __DataType
		{
			get { return DataType.ToString(); }
			set { DataType = Type.GetType(value); }
		}
	}
}
