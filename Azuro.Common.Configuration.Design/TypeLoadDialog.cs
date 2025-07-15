using System;
using System.IO;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Azuro.Configuration.Design
{
	/// <summary>
	/// Type load edit form.
	/// </summary>
	internal class TypeLoadDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ComboBox cbTypeName;
		private System.Windows.Forms.Button cmdBrowseAssembly;
		private System.Windows.Forms.TextBox txtServiceAssembly;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Button cmdOK;
		private System.Windows.Forms.Button cmdCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// ctor.
		/// </summary>
		public TypeLoadDialog(string type_assembly)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			TypeAssembly = type_assembly;
		}

		/// <summary>
		/// Type, Assembly
		/// </summary>
		internal string TypeAssembly
		{
			get{return string.Format("{0}, {1}", (string)cbTypeName.SelectedItem, Path.GetFileNameWithoutExtension(txtServiceAssembly.Text));}
			set
			{
				if (value != null)
				{
					string[] type_assembly = value.Split(',');
					txtServiceAssembly.Text = type_assembly[1].Trim();
					cbTypeName.Items.Add(type_assembly[0]);
					cbTypeName.SelectedIndex = 0;
				}
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(TypeLoadDialog));
			this.cbTypeName = new System.Windows.Forms.ComboBox();
			this.cmdBrowseAssembly = new System.Windows.Forms.Button();
			this.txtServiceAssembly = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.cmdOK = new System.Windows.Forms.Button();
			this.cmdCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// cbTypeName
			// 
			this.cbTypeName.Location = new System.Drawing.Point(112, 32);
			this.cbTypeName.Name = "cbTypeName";
			this.cbTypeName.Size = new System.Drawing.Size(216, 21);
			this.cbTypeName.TabIndex = 35;
			// 
			// cmdBrowseAssembly
			// 
			this.cmdBrowseAssembly.Image = ((System.Drawing.Image)(resources.GetObject("cmdBrowseAssembly.Image")));
			this.cmdBrowseAssembly.Location = new System.Drawing.Point(336, 8);
			this.cmdBrowseAssembly.Name = "cmdBrowseAssembly";
			this.cmdBrowseAssembly.Size = new System.Drawing.Size(24, 20);
			this.cmdBrowseAssembly.TabIndex = 34;
			this.cmdBrowseAssembly.Click += new System.EventHandler(this.cmdBrowseAssembly_Click);
			// 
			// txtServiceAssembly
			// 
			this.txtServiceAssembly.Location = new System.Drawing.Point(112, 8);
			this.txtServiceAssembly.Name = "txtServiceAssembly";
			this.txtServiceAssembly.Size = new System.Drawing.Size(216, 20);
			this.txtServiceAssembly.TabIndex = 38;
			this.txtServiceAssembly.Text = "";
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label12.Location = new System.Drawing.Point(8, 32);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(100, 16);
			this.label12.TabIndex = 40;
			this.label12.Text = "Type Name";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label11.Location = new System.Drawing.Point(8, 8);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(100, 16);
			this.label11.TabIndex = 39;
			this.label11.Text = "Assembly";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// cmdOK
			// 
			this.cmdOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.cmdOK.Location = new System.Drawing.Point(208, 64);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.TabIndex = 2;
			this.cmdOK.Text = "O&K";
			// 
			// cmdCancel
			// 
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.Location = new System.Drawing.Point(288, 64);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.TabIndex = 3;
			this.cmdCancel.Text = "C&ancel";
			// 
			// TypeLoadDialog
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(370, 96);
			this.ControlBox = false;
			this.Controls.Add(this.cbTypeName);
			this.Controls.Add(this.cmdBrowseAssembly);
			this.Controls.Add(this.txtServiceAssembly);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.cmdCancel);
			this.Controls.Add(this.cmdOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "TypeLoadDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Type Load Dialog";
			this.ResumeLayout(false);

		}
		#endregion

		private void cmdBrowseAssembly_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Assemblies (*.dll)|*.dll";

			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txtServiceAssembly.Text = Path.GetFileName(openFileDialog.FileName);
				Assembly assem = Assembly.LoadFile(openFileDialog.FileName);
				if (assem!=null)
				{
					cbTypeName.Items.Clear();
					try
					{
						Type[] types = assem.GetTypes();
						foreach (Type type in types)
							cbTypeName.Items.Add(type.FullName);
					}
					catch (Exception ex)
					{
						MessageBox.Show(this, ex.Message);
					}
				}

				if (cbTypeName.Items.Count>0)
					cbTypeName.SelectedIndex = 0;
			}		
		}
	}

	/// <summary>
	/// Type load editor implementation.
	/// </summary>
	public class TypeLoadEditor : UITypeEditor
	{
		/// <exclude/>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			if (context!=null && context.Instance!=null && provider!=null) 
			{
				// Attempt to obtain an IWindowsFormsEditorService.
				IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if (edSvc==null) return null;

				// Displays a TypeLoadDialog Form to get a Type to load.
				TypeLoadDialog dlg = new TypeLoadDialog((string)value);
				if (edSvc.ShowDialog(dlg)==DialogResult.OK)
					return dlg.TypeAssembly;
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
