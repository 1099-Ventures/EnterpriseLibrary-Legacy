using System.Collections.Generic;
using Azuro.Data;

namespace Azuro.Data
{
    public class DataAccessConfigObjectHelper
    {
        private DataObject m_do;
        private string m_primary;
        private readonly Dictionary<string, DataAccessConfigObjectSectionEntity> m_conn = new Dictionary<string, DataAccessConfigObjectSectionEntity>();
        private readonly Dictionary<string, DataObject> m_dataObjects = new Dictionary<string, DataObject>();

        public DataObject DO
        {
            get { return m_do = (m_do ?? DataObject.Create<DataObject>(m_primary)); }
        }

        /// <summary>
        /// Constructor for creating the helper class
        /// </summary>
        /// <param name="primaryConfigName">The name of the Azuro.Data config element in the config file
        /// <para>that consist of the main database config info</para></param>
        public DataAccessConfigObjectHelper(string primaryConfigName)
        {
            m_primary = primaryConfigName;
        }

        public DataAccessConfigObjectSectionEntity GetConnectionInfo(string primaryConfigName, string name)
        {
            m_primary = primaryConfigName;
            return GetConnectionInfo(name);
        }

        public DataAccessConfigObjectSectionEntity GetConnectionInfo(string name)
        {
            DataAccessConfigObjectSectionEntity dacose;
            if (!m_conn.TryGetValue(name, out dacose))
            {
                dacose = new DataAccessConfigObjectSectionEntity();
                dacose.Name = name;
                //DO.Fetch(dacose);
                List<DataAccessConfigObjectSectionEntity> list = DO.List<DataAccessConfigObjectSectionEntity>("FetchDataAccessConfigObject", dacose);
                if (list.Count > 0)
                    dacose = list[0];
                m_conn.Add(name, dacose);
            }
            return dacose;
        }

        public DataObject DBDataObject(string primaryConfigName, string name)
        {
            m_primary = primaryConfigName;
            return DBDataObject(name);
        }

        public DataObject DBDataObject(string name)
        {
            DataObject dataObject;
            if (!m_dataObjects.TryGetValue(name, out dataObject))
            {
                dataObject = DataObject.Create<DataObject>(GetConnectionInfo(name));
                m_dataObjects.Add(name, dataObject);
            }
            return dataObject;
        }
    }
}