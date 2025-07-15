using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Azuro.Common.Configuration.Design
{
    //  TODO: Need to fix the loading capability here. Is this still relevant?
    public class ConfigurationLookupEditor<T> : UITypeEditor //where T : DataEntity, new()
    {
        private object m_value = null;
        //private object m_display = null;

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            m_value = value;
            ConfigurationLookupBindingAttribute clba =
                (ConfigurationLookupBindingAttribute)
                context.PropertyDescriptor.Attributes[typeof(ConfigurationLookupBindingAttribute)];
            if (clba == null)
                throw new InvalidOperationException("The type must have a [ConfigurationLookupBindingAttribute] defined.");

            IWindowsFormsEditorService svc = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            ListBox lb = new ListBox();
            if (svc != null)
            {
				//	TODO: Figure out what I was trying to do here
				//IConfigurationLookupConnection clc =
				//	Util.CreateObject<IConfigurationLookupConnection>(clba.ConnectionType, null);
				//lb.DataSource = clc.GetData<T>(context, provider, value);
                lb.DisplayMember = clba.DisplayMember;
                lb.ValueMember = clba.ValueMember;
                lb.Tag = svc;
                lb.SelectedValue = m_value; //TODO: Not Working, want to actually show the item in the dropdown
                lb.SelectedValueChanged += lb_SelectedValueChanged;
                svc.DropDownControl(lb);
            }
            if (lb.SelectedIndex != -1) // Check that an item has been selected else keep the original value
            {
                if (clba.ReturnSelectedItem)
                    m_value = lb.SelectedItem;
                else if (clba.ReturnSelectedOrdinal)
                    m_value = lb.SelectedIndex + 1;
                else if (clba.ReturnValueAndOridinal) //TODO: might need to look at a cleaner way of doing the following
                    m_value = string.Format("{0}|{1}", m_value, lb.SelectedIndex + 1);
            }
            return m_value;
        }

        void lb_SelectedValueChanged(object sender, EventArgs e)
        {
            ListBox lb = (ListBox)sender;
            if (lb.SelectedIndex != -1) // Check that an item has been selected else keep the original value
                m_value = lb.SelectedValue;
            ((IWindowsFormsEditorService)lb.Tag).CloseDropDown();
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            Console.WriteLine(e.Value);
            base.PaintValue(e);
        }
    }
}
