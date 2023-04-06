// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define use_tools
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Media;
using System.Windows.Markup;
using System.Collections;
using System.Windows.Input;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Automation.Provider;
using System.Windows.Automation;

#if use_tools
using Microsoft.Test.Logging;
using Microsoft.Windows.Test.Client.AppSec.P1;
#endif

namespace Microsoft.Windows.Test.Client.AppSec.P1
{
    public class DialogUtilities
    {
        private static Microsoft.Windows.Test.Client.AppSec.FrameworkLoggerWrapper s_fw = new Microsoft.Windows.Test.Client.AppSec.FrameworkLoggerWrapper();
        #region Logging
        public static void LogStatus(string message)
        {
#if use_tools
            s_fw.LogEvidence(message);
#else 
                        Console.WriteLine (message);
#endif
        }

        public static void LogPass(string message)
        {
#if use_tools
            s_fw.LogEvidence(message);
            s_fw.Result = TestResult.Pass;
#else 
                        Console.WriteLine ("Pass: " + message);
#endif

        }

        public static void LogFail(string message)
        {
#if use_tools
            s_fw.LogEvidence(message);
            s_fw.Result = TestResult.Fail;
#else 
                        Console.WriteLine ("Fail: " + message);
#endif
        }

        #endregion

        public static bool CheckModality(Window[] zorderarray)
        {
            LogStatus("Checking modality of dialog");
            if (!CheckForegroundWindow(zorderarray[0]))
            {
                LogFail("Foreground window check failed");
                return false;
            }
            else
            {
                DialogUtilities.LogStatus("Foreground test passed");
            }

            if (!CheckZOrder(zorderarray))
            {
                LogFail("zorder of windows is incorrect for the dialog");
                return false;
            }

            return true;
        }

        private static bool CheckForegroundWindow(Window expectedfgwindow)
        {
            IntPtr expected_hwnd = IntPtr.Zero, actual_hwnd = IntPtr.Zero;
            WindowInteropHelper wih_hwnd = new WindowInteropHelper(expectedfgwindow);

            expected_hwnd = wih_hwnd.Handle;
            actual_hwnd = GetForegroundWindow();
            LogStatus("HWND of expected foreground window is: " + expected_hwnd);
            LogStatus("HWND of actual foreground window is: " + actual_hwnd);
            //            PresentationSource src = PresentationSource.FromVisual (expectedfgwindow);
            //            IWin32Window w = (IWin32Window)src;
            //
            //            if (w == null)
            //                LogStatus ("Unsuccessful cast");
            //            else
            //                LogStatus ("Successful cast: " + w.Handle);

            //WindowInteropHelper wih = expectedfgwindow
            if (actual_hwnd != expected_hwnd)
                return false;

            return true;
        }

        private static bool CheckZOrder(Window[] zorderarray)
        {
            return true;
        }

        #region Imports
        [DllImport("user32.dll")]
        protected static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        protected static extern void GetWindow();

        #endregion
    }
}
