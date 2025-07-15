using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Azuro.Common.Configuration.Design
{
	public interface IConfigurationEditor
	{
		object ConfigurationValue { get; set; }
	}

	/// <summary>
	/// An implementation of UITypeEditor
	/// </summary>
	public class ConfigurationEditor<T> : UITypeEditor where T : System.Windows.Forms.Form, IConfigurationEditor
	{
		/// <exclude/>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context != null && context.Instance != null && provider != null)
			{
				// Attempt to obtain an IWindowsFormsEditorService.
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc == null)
					return null;

				// Displays a Dialog Form to allow for user configuration.
				T dlg = Activator.CreateInstance<T>();
				// the type that T1 is derived from must implement the IConfigurationEditor interface.
				dlg.ConfigurationValue = value;
				if (edSvc.ShowDialog(dlg) == DialogResult.OK)
					return dlg.ConfigurationValue;
			}

			// If OK was not pressed, return the original value
			return value;
		}

		/// <exclude/>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			// Indicates that this editor can display a Form-based interface.
			return UITypeEditorEditStyle.Modal;
		}
	}
}
