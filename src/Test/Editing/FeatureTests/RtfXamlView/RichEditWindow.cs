// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//              Creates form with RichEdit control
//              (for rendering text displayed in Rtf Text Panel)
//

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace RtfXamlView
{
    class RichTextBoxV5 : RichTextBox
    {
        const string DLL_RICHEDIT = "riched20.dll";
        const string WC_RICHEDITW = "RICHEDIT60W";

        private IntPtr _moduleHandle;
        private bool _attemptedLoad;

        [DllImport("Kernel32.dll")]
        internal static extern IntPtr LoadLibrary(string libname);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

        public RichTextBoxV5()
        {
            // This is where we store the riched library.
            _moduleHandle = IntPtr.Zero;
            _attemptedLoad = false;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                AttemptToLoadNewRichEdit();
                if (_moduleHandle != IntPtr.Zero)
                {
                    cp.ClassName = WC_RICHEDITW;
                }
                return cp;
            }
        }

        void AttemptToLoadNewRichEdit()
        {
            // Check for library
            if (false == _attemptedLoad)
            {
                _attemptedLoad = true;
                string szPath = System.Windows.Forms.Application.StartupPath;
                szPath += "\\" + DLL_RICHEDIT;
                _moduleHandle = LoadLibrary(szPath);
            }
        }
    }

    class RichEditWindow : Form//, IFormattedViewWindow 
    {
        public RichEditWindow()
        {
            // Set the form
            this.Name = "RTFViewWindow";
            this.Text = "RTF View Window";
            this.Size = new Size(450, 450);
            this.WindowState = FormWindowState.Normal;
            this.BackColor = Color.FromArgb(153, 198, 241);
            this.ControlBox = false;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;

            // Set RichEdit control
            _richTextBox = new RichTextBoxV5();
            _richTextBox.Size = this.ClientSize;
            _richTextBox.Location = new Point(0, 0);

            this.Controls.Add(_richTextBox);

            // Add Resize event
            this.Resize += new EventHandler(OnResize);
        }

        #region Public Properties
        public RichTextBox RtfTextBox
        {
            get
            {
                return _richTextBox;
            }
        }
        #endregion

        #region IFormattedViewWindow Members
        public string ContentAsText
        {
            get
            {
                return _richTextBox.Rtf;
            }
        }

        public object Content
        {
            set
            {
                _richTextBox.Rtf = (string)value;
                //Clear the Undo stack
                _richTextBox.ClearUndo();
                Refresh();
            }
        }
        #endregion IFormattedViewWindow Members  
      
        #region Events
        void OnResize(object sender, EventArgs e)
        {
            _richTextBox.Size = this.ClientSize;
            Refresh();
        }
        #endregion

        private RichTextBox _richTextBox;
    }       
}