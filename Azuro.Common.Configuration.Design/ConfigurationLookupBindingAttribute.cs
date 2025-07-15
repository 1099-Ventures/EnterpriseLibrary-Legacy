using System;

namespace Azuro.Common.Configuration.Design
{
    public class ConfigurationLookupBindingAttribute : Attribute
    {
        private string m_displayMember;
        private string m_valueMember;
        private string m_connectionString;
        private Type m_connectionType;
        private string m_connectionTypeString;
        private bool m_returnSelectedItem;
        private bool m_returnSelectedOrdinal;
        private bool m_returnValueAndOrdinal;

        /// <summary>
        /// The value that will be displayed
        /// <para>By default the DisplayMember is the same as the ValueMember</para>
        /// </summary>
        public string DisplayMember
        {
            get { return m_displayMember ?? m_valueMember; }
            set { m_displayMember = value; }
        }

        /// <summary>
        /// The value that will be returned
        /// </summary>
        public string ValueMember
        {
            get { return m_valueMember; }
            set { m_valueMember = value; }
        }

        public string ConnectionString
        {
            get { return m_connectionString; }
            set { m_connectionString = value; }
        }

        public Type ConnectionType
        {
            get { return m_connectionType ?? Type.GetType(m_connectionTypeString); }
            set { m_connectionType = value; }
        }

        public string ConnectionTypeString
        {
            get { return m_connectionTypeString; }
            set { m_connectionTypeString = value; }
        }

        public bool ReturnSelectedItem
        {
            get { return m_returnSelectedItem; }
            set { m_returnSelectedItem = value; }
        }

        public bool ReturnSelectedOrdinal
        {
            get { return m_returnSelectedOrdinal; }
            set { m_returnSelectedOrdinal = value; }
        }

        public bool ReturnValueAndOridinal
        {
            get { return m_returnValueAndOrdinal; }
            set { m_returnValueAndOrdinal = value; }
        }
    }
}
