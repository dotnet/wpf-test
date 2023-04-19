// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.AddIn;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using WinForms=System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;

namespace Microsoft.Test.AddIn
{
    [AddIn("WinFormsHost", Version = "1.0.0.0")]
    public class WinFormsHostAddInCode : AddInWinFormsHostView
    {
        #region Private Members

        private StackPanel _panel;
        private WinForms.TextBox _tb;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public WinFormsHostAddInCode()
        {
            //Not sure where the right place is to put this, but it needs to be pretty early on, and only for this addin, so let's try here.
            //If we're on Japanese, we need to shut off the IME by switching to English input locale, so text entry works.
            //Also note that I had to add the PInvokes to the addin since I didn't want the addin to have to have a dependency on TestRuntime.
            if (System.Globalization.CultureInfo.CurrentCulture.Name.ToLower().Contains("ja"))
            {
                //change to English input locale
                IntPtr layout;
                IntPtr oldLayout;

                //409 is English
                layout = LoadKeyboardLayout("00000409", 0);

                //other comments said the return value from LoadKeyboardLayout wasn't reliable, so set to English.
                layout = (IntPtr)0x409;
                oldLayout = ActivateKeyboardLayout(layout, 0);
            }

            _panel = new StackPanel();
            _panel.Name = "RootPanel";
            _panel.Height = 300;
            _panel.Width = 300;
            _panel.Background = new SolidColorBrush(Colors.CornflowerBlue);

            WindowsFormsHost host = new WindowsFormsHost();

            _tb = new WinForms.TextBox();
            _tb.Width = 300;
            _tb.Height = 22;

            host.Child = _tb;
                        
            _panel.Children.Add(host);

        }

        #endregion

        #region Public Overrides

        public override void Initialize(string addInParameters)
        {
            //
        }

        public override FrameworkElement GetAddInUserInterface()
        {
            return _panel;
        }

        public override string GetTextBoxText()
        {
            return _tb.Text;            
        }

        #endregion

        /// <summary>
        /// Loads a new input locale identifier (formerly called the keyboard layout) into the system.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr LoadKeyboardLayout(string pwszKLID, int flags);

        /// <summary>
        /// Sets the input locale identifier (formerly called the keyboard layout handle) for the calling thread or the current process.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, int uFlags);

    }
}
