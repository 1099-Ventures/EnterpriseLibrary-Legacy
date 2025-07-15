using System;

namespace Azuro.Data
{
    /// <summary>
    /// This entity is intended to read the information out of the database,
    /// the stored procedures still need to be completed
    /// </summary>
    [Serializable]
    [StoredProcedure(
       FetchProcedure = "FetchProcedure",
       ListProcedure = "ListProcedure",
       UpdateProcedure = "UpdateProcedure",
       InsertProcedure = "InsertProcedure")]
    //TODO: The stored procedures still need to be completed
    public class DataAccessConfigObjectSectionEntity : DataEntityWithIdentityId<int>
    {
        private string m_name;
        private string m_assembly;
        private string m_type;
        private string m_connectionString;
        private SqlTextCommandType m_sqlTextCommandWrapper;
        private string m_sqlTextCommandLocation;

        [DatabaseColumn("Name")]
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        [DatabaseColumn("Assembly")]
        public string Assembly
        {
            get { return m_assembly; }
            set { m_assembly = value; }
        }

        [DatabaseColumn("Type")]
        public string Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        [DatabaseColumn("ConnectionString")]
        public string ConnectionString
        {
            get { return m_connectionString; }
            set { m_connectionString = value; }
        }

        [DatabaseColumn("SqlTextCommandWrapper")]
        public SqlTextCommandType SqlTextCommandWrapper
        {
            get { return m_sqlTextCommandWrapper; }
            set { m_sqlTextCommandWrapper = value; }
        }

        [DatabaseColumn("SqlTextCommandLocation")]
        public string SqlTextCommandLocation
        {
            get { return m_sqlTextCommandLocation; }
            set { m_sqlTextCommandLocation = value; }
        }

        public static implicit operator DataAccessConfigObjectSection(DataAccessConfigObjectSectionEntity dacose)
        {
            DataAccessConfigObjectSection dacos = new DataAccessConfigObjectSection();
            dacos.Name = dacose.Name;
            dacos.Assembly = dacose.Assembly;
            dacos.ConnectionString = dacose.ConnectionString;
            dacos.SqlTextCommandLocation = dacose.SqlTextCommandLocation;
            dacos.SqlTextCommandWrapper = dacose.SqlTextCommandWrapper;
            dacos.Type = dacose.Type;
            return dacos;
        }
    }
}
