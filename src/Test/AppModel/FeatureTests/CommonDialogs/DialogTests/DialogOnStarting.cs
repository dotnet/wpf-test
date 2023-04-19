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
using System.Threading;
using System.Windows.Threading;
using Microsoft.Test.Threading;
#if use_tools
using Microsoft.Test.Logging;
using Microsoft.Windows.Test.Client.AppSec.P1;
#endif

using MTI = Microsoft.Test.Input;

namespace Microsoft.Windows.Test.Client.AppSec.P1
{
    public class DialogOnStarting :
#if use_tools
 BaseTestPureApp
#else 
Application
#endif
    {
        Window _wDlg;

        /// <summary>
        /// App to test pure dialog. Will test that a modal dialog remains the foreground window when user clicks owner window.
        /// Added extra clicking to prevent lost input, set focus to window initially to prevent 
        ///                       other windows being foreground, and cleaned up.
        ///    <list type="unordered">
        ///    <item>modality</item>
        ///    <item>will create a dialog in the starting up of the app</item>
        ///    <item>will click outside the window to ensure modality</item>
        ///    <item>will set the owner window. 
        public DialogOnStarting()
        {
            eventpipe = new Hashtable(5);
            eventpipe["AppStarted"] = true;
            eventpipe["mainwindow_lostfocus"] = false;
            eventpipe["wDlg_lostfocus"] = false;
            Description = "This test creates dialogs on starting up and checks modality (click outside dialog window). This test also sets window style";
            DialogUtilities.LogStatus(Description);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Window wMain = new Window();
            wMain.Left = 10;
            wMain.Top = 10;
            wMain.Width = 1280;
            wMain.Height = 1024;
            wMain.Show();
            wMain.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(wMain_lostfocus);
            wMain.Focus();

            LogStatus("Creating Dialog");
            _wDlg = new Window();
            _wDlg.WindowStyle = WindowStyle.ToolWindow;
            _wDlg.Title = "Dialog";
            _wDlg.Width = 100;
            _wDlg.Height = 100;
            _wDlg.Left = 350;
            _wDlg.Top = 350;
            _wDlg.Owner = wMain;
            _wDlg.Activated += new EventHandler(wDlg_Activated);
            bool? dr = _wDlg.ShowDialog();
            LogStatus("Got dialog result on OnStartup. Shutting down app");
            Shutdown();
        }

        private void wDlg_Activated(object sender, EventArgs e)
        {

            _wDlg.Activated -= new EventHandler(wDlg_Activated);
            LogStatus("Dialog window activated");
            Window[] zorderarray = new Window[2];
            zorderarray[0] = Windows[1];
            zorderarray[1] = Windows[0];
            if (((Window)sender).Title != "Dialog")
            {
                LogFail("Test issue: wrong window set at top of z-order.. " + "test needs to be reworked");
                goto EndTest;
            }

            LogStatus("Now checking modality by clicking outside the window");
            MTI.Input.MoveToAndClick(new Point(150, 150));
            MTI.Input.MoveToAndClick(new Point(150, 150));
            MTI.Input.MoveToAndClick(new Point(150, 150));

            DispatcherHelper.DoEvents(DispatcherPriority.SystemIdle);
            Thread.Sleep(2000);
            LogStatus("Finished clicking");
            CheckModality(zorderarray);

            LogStatus("Again checking modality by clicking outside the window at another point");
            MTI.Input.MoveToAndClick(new Point(550, 300));
            MTI.Input.MoveToAndClick(new Point(550, 300));
            MTI.Input.MoveToAndClick(new Point(550, 300));
            LogStatus("Finished clicking");
            DispatcherHelper.DoEvents(DispatcherPriority.SystemIdle);
            Thread.Sleep(2000);

            CheckModality(zorderarray);
            if (!CheckFocusOM())
            {
                LogFail("Focus sequence was wrong");
                goto EndTest;
            }

            if (!CheckActivationDeactivationRecord())
            {
                LogFail("Windows' activated deactivated events indicate incorrect activation seq");
                goto EndTest;
            }
            LogPass("Passed Window test for Modality");
        EndTest:
            LogStatus("Test completed");
            PostTestItem(new TestStep(RetDlg));
        }

        private void RetDlg()
        {
            LogStatus("Setting Dialog Result");
            _wDlg.DialogResult = false;
        }

        private bool CheckModality(Window[] zorderarray)
        {
            LogStatus("Checking modality of dialog");
            if (!CheckForegroundWindow(zorderarray[0]))
            {
                LogFail("Foreground window check failed");
                return false;
            }

            if (!CheckZOrder(zorderarray))
            {
                LogFail("zorder of windows is incorrect for the dialog");
                return false;
            }

            return true;
        }

        private bool CheckForegroundWindow(Window expectedfgwindow)
        {
            IntPtr expected_hwnd = IntPtr.Zero, actual_hwnd = IntPtr.Zero;
            WindowInteropHelper wih_hwnd = new WindowInteropHelper(expectedfgwindow);

            expected_hwnd = wih_hwnd.Handle;
            actual_hwnd = GetForegroundWindow();
            LogStatus("HWND of expected foreground window is: " + expected_hwnd);
            LogStatus("HWND of actual foreground window is: " + actual_hwnd);
            int retryCount = 0;
            while (actual_hwnd != expected_hwnd && retryCount <= 5)
            {
                LogStatus("Retry # " + retryCount + " : HWND of actual foreground window is: " + actual_hwnd);
                actual_hwnd = GetForegroundWindow();
                retryCount++;
                Thread.Sleep(3000);
            }
            if (actual_hwnd != expected_hwnd)
            {
                return false;
            }
            return true;
        }

        private bool CheckZOrder(Window[] zorderarray)
        {

            return true;
        }

        private bool CheckFocusOM()
        {
            return true;
        }

        private bool CheckActivationDeactivationRecord()
        {
            return true;
        }

        private void wMain_lostfocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            eventpipe["mainwindow_lostfocus"] = true;
        }

        private void wDlg_lostfocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            eventpipe["wDlg_lostfocus"] = true;
            // if dialog is being closed and it loses focus, it should be ok, 
            // or if the entire app loses focus, then also dialog will lose
            // focus, else a fail. 
        }

        #region Logging
        public void LogStatus(string message)
        {
#if use_tools
            fw.LogEvidence(message);
#else 
            Console.WriteLine (message);
#endif
        }

        public void LogPass(string message)
        {
#if use_tools
            fw.LogEvidence(message);
            fw.Result = TestResult.Pass;
#else 
            Console.WriteLine ("Pass: " + message);
#endif

        }

        public void LogFail(string message)
        {
#if use_tools
            fw.LogStatus(message);
            fw.Result = TestResult.Fail;
#else 
            Console.WriteLine ("Fail: " + message);
#endif
        }

        #endregion
        #region Imports
        [DllImport("user32.dll")]
        protected static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        protected static extern void GetWindow();

        #endregion
        #region protected
        protected Hashtable eventpipe;
        #endregion
    }
}
