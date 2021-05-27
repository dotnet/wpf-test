// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: class to host Avalon controls in WinForm
 * 
 * Creation time: 11/15/2005
 * Creator: adamshao
********************************************************************/

using System.Reflection;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Interop;


namespace Microsoft.Test.WindowsForms
{
    ///<summary>
    /// This class enable hosting Avalon controls in Winform
    /// Avalon controls are added to ElementHost thru the RootVisual property.
    ///</summary>
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    [System.ComponentModel.DesignerCategory("code")]
    public class WindowsFormSource : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public static WindowsFormSource MainWindow
        {
            get
            {
                return _mainWindow;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            _ctrlHost = null;
            _hwndSource = null;
            if (disposing && (_components != null))
            {
                _components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public WindowsFormSource()
        {
            _mainWindow = this;
            InitializeComponent();
        }

              
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        new public void Resize(int width, int height)
        {
            this.Width = width;
            this.Height = height;

            System.Drawing.Size size = this.ClientSize;

            AvalonHost.Width = size.Width;
            AvalonHost.Height = size.Height;
        }


        /// <summary>
        /// 
        /// </summary>
        public HwndSource CurrentHwndSource
        {
            get
            {
                if (_ctrlHost != null)
                {
                    if (_hwndSource == null)
                    {
                        FieldInfo field = _ctrlHost.GetType().GetField("_hwndSource",
                            BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance);

                        object o = field.GetValue(_ctrlHost);

                        if (o != null && o is HwndSource)
                        {
                            _hwndSource = (HwndSource)o;
                        }
                    }

                    return _hwndSource;
                }

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ElementHost AvalonHost
        {
            get
            {
                return _ctrlHost;
            }
        }

        /// <summary>
        /// Property RootVisual set/get.
        /// Right now ElementHost only takes UIElements
        /// </summary>
        public UIElement RootVisual
        {
            set
            {
                UIElement visual = value;

                if (_ctrlHost.Child == visual)
                {
                    return;
                }

                System.Security.Permissions.UIPermission p = new System.Security.Permissions.UIPermission(PermissionState.Unrestricted);
                p.Assert();
                _ctrlHost.Child = value;


            }
            get
            {
                return (UIElement)_ctrlHost.Child;
            }
        }


        /// <summary>
        /// initialize the winform and the element host, add element host to winform
        /// </summary>
        private void InitializeComponent()
        {
            this._ctrlHost = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();

            // 
            // _ctrlHost
            // 
            this._ctrlHost.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._ctrlHost.BackColor = System.Drawing.SystemColors.ControlLight;
            this._ctrlHost.BackColorTransparent = false;
            this._ctrlHost.Location = new System.Drawing.Point(0, 0);
            this._ctrlHost.Name = "_ctrlHost";
            this._ctrlHost.Size = new System.Drawing.Size(455, 336);
            this._ctrlHost.TabIndex = 0;
            this._ctrlHost.TabStop = false;
            this._ctrlHost.Text = "_ctrlHost";

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 375);
            this.Controls.Add(this._ctrlHost);
            this.Name = "WindowsFormSource";
            this.Text = "WindowsFormSource";
            this.ResumeLayout(false);

        }

        //Cache the HwndSource
        HwndSource _hwndSource = null;
        private ElementHost _ctrlHost;
        private static WindowsFormSource _mainWindow = null;
        private System.ComponentModel.IContainer _components = null;

    }
}

