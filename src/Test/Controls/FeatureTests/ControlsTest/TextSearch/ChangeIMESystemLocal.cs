using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using System.Windows.Threading;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security.Permissions;
//using Microsoft.Win32;




namespace Avalon.Test.ComponentModel.Actions
{
    public class ChangeIMESystemLocal : IAction
    {
        /// <summary>
        /// Sets the input locale identifier (formerly called the keyboard
        /// layout handle) for the calling thread.
        /// </summary>
	    /// <example>
        /// <Action Name="ChangeIMESystemLocal">
        ///	<Parameter Value="00000409"/>	
        /// </Action>
	    ///<code>
        /// Set input locale to: Arabic (Saudi Arabia) - Arabic (101) <Parameter Value = "00000401">
        /// Set input locale to: English (United States) - US <Parameter Valu = "00000409">
        /// Set input locale to: Hebrew - Hebrew <Parameter Value = "0000040d">
       	/// Set input locale to: Japanese - Japanese Input System (MS-IME2002) <Parameter Value = "e0010411">
       	/// Set input locale to: Spanish (Argentina) - Latin American <Parameter Value = "00002c0a">
       	/// </code></example> 
        /// </summary>       
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {

            GlobalLog.LogEvidence("Changing the keyboard to English.");
            string inputLocale = actionParams[0] as String;        
            IntPtr hkl;     // New layout handle.
            IntPtr hklOld;  // Old layout handle.
            if (inputLocale == null)
            {
                throw new ArgumentNullException("inputLocale");
            }
            if (inputLocale.Length != 8)
            {
                throw new ArgumentException("Input locale values should be 8 hex digits.", "inputLocale");

            }
       
            hkl = SetKeyboardLayout.SafeLoadKeyboardLayout(inputLocale, 0);
            if (hkl == IntPtr.Zero)
            {
                throw new Exception("Unable to set input locale " + inputLocale);
            }

            hkl = (IntPtr)(uint.Parse(inputLocale, NumberStyles.HexNumber, CultureInfo.InvariantCulture));

            hklOld = SetKeyboardLayout.SafeActivateKeyboardLayout(hkl, 0);
            if (hklOld == IntPtr.Zero)
            {
                throw new Exception("Locale " + inputLocale + " loaded, " + "but activation failed.");
            }

            if (hkl == hklOld)
            {
                System.Diagnostics.Debug.WriteLine("WARNING: changing keyboard layout " + "to " + inputLocale + " keeps the same layout " +
                    "(input locale " + ((uint)hkl).ToString("x8") + ")");
            }
        }        
    }
}

