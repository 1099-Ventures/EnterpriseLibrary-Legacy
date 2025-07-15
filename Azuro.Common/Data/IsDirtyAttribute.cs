using System;

namespace Azuro.Data
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class IsDirtyAttribute : Attribute
    {
        private bool m_useIsDirty = true;

        public bool UseIsDirty
        {
            get { return m_useIsDirty; }
            set { m_useIsDirty = value; }
        }
    }
}
