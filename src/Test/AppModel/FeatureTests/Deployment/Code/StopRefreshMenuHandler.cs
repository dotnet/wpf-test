// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using System.Windows.Automation;
using System.Diagnostics;
using MTI = Microsoft.Test.Input;
using System.Windows.Input;
using System.Collections;
using Microsoft.Test.Deployment;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// UI handler
    /// </summary>
    public class StopRefreshMenuHandler: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public StopRefreshMenuHandler() { }

        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            string stopButtonName = "";
            string refreshButtonName = "";
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    stopButtonName = "Stop"; // !!! Replace these with actual strings!
                    refreshButtonName = "Refresh";
                    break;
                case 7:
                    stopButtonName = "Stop"; 
                    refreshButtonName = "Refresh";
                    break;
                case 8:
                    stopButtonName = "Stop";
                    refreshButtonName = "Refresh";
                    break;
                case 9:
                    stopButtonName = "Stop";
                    refreshButtonName = "Refresh";
                    break;
                default:
                    throw new InvalidOperationException("Don't know what to do with IE Version " + ApplicationDeploymentHelper.GetIEVersion());
            }

            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);



            IEAutomationHelper.WaitForElementWithAutomationId(topHwnd, "MenuBar", 2);

            IEAutomationHelper.ShowIEViewMenu(IEWindow);
            Thread.Sleep(2000);
            AutomationElement visibleMenu = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Menu));
            AutomationElement StopMenu = visibleMenu.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, stopButtonName));
            AutomationElement RefreshMenu = visibleMenu.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, refreshButtonName));

            bool passed = true;

            if (passed &= (bool)(StopMenu.GetCurrentPropertyValue(AutomationElement.IsEnabledProperty)))
            {
                GlobalLog.LogEvidence("Stop Menu was enabled... ");
            }
            if (passed &= (bool)RefreshMenu.GetCurrentPropertyValue(AutomationElement.IsEnabledProperty))
            {
                GlobalLog.LogEvidence("Refresh Menu was enabled... ");
            }
            if (passed)
            {
                GlobalLog.LogEvidence("PASSED: Stop and refresh buttons in IE menus still enabled with WinFX content.");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failed: One or both of Stop and refresh buttons in IE menus disabled with WinFX content.");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }
    }

    /// <summary>
    /// UI handler
    /// </summary>
    public class CtrlNHandler : UIHandler
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("user32.dll")]
        static extern IntPtr LoadMenu(IntPtr hInstance, IntPtr resourceIDHex);

        [DllImport("user32.dll")]
        static extern IntPtr GetSubMenu(IntPtr hMenu, int nPos);

        [DllImport("user32.dll")]
        static extern int GetMenuString(IntPtr hMenu, uint uIDItem,
           [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder lpString, int nMaxCount, uint uFlag);


        private const uint MF_BYPOSITION = 0x00000400;

        private static Key IECtrlNString()
        {
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    return IE6CtrlNString();
                case 7:
                    return IE7CtrlNString();
                case 8:
                    return IE8CtrlNString();
                default:
                    goto case 8;
            }
        }

        private static Key IE8CtrlNString()
        {
            string ieMenuDLL = Environment.SystemDirectory + @"\" + CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui";
            if (!File.Exists(ieMenuDLL))
            {
                ieMenuDLL = Environment.GetEnvironmentVariable("SystemRoot") + @"\System32\en-us\ieframe.dll.mui";
            }
            string newMenuString = IEAutomationHelper.GetUnmanagedSubMenuResourceString(ieMenuDLL, 333, 0, 2);

            return (Key)(new KeyConverter().ConvertFromString(newMenuString.Substring(newMenuString.IndexOf("+")).Substring(1)));
        }

        private static Key IE7CtrlNString()
        {
            string ieMenuDLL = Environment.SystemDirectory + @"\" + CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui";
            if (!File.Exists(ieMenuDLL))
            {
                ieMenuDLL = Environment.GetEnvironmentVariable("SystemRoot") + @"\System32\en-us\ieframe.dll.mui";
            }
            string newMenuString = IEAutomationHelper.GetUnmanagedSubMenuResourceString(ieMenuDLL, 267, 0, 1);

            return (Key)(new KeyConverter().ConvertFromString(newMenuString.Substring(newMenuString.IndexOf("+")).Substring(1)));
        }

        private static Key IE6CtrlNString()
        {
            string dllPath = null;
            int ResourceId = 0;
            int MenuID = 0;
            uint SubId = 0;

            IntPtr hExe = LoadLibrary(dllPath);
            if (hExe.ToInt32() == 0)
            {
                GlobalLog.LogEvidence("Uh oh, something terrible happened trying to get the Ctrl-N string!... falling back to english");
                return Key.N;
            }

            IntPtr theMenu = LoadMenu(hExe, new IntPtr(ResourceId));
            IntPtr subMenu = GetSubMenu(theMenu, MenuID);
            IntPtr subMenu2 = GetSubMenu(subMenu, MenuID);
            StringBuilder loadedStringBuilder = new StringBuilder(260);

            if (GetMenuString(subMenu2, SubId, loadedStringBuilder, loadedStringBuilder.Capacity, MF_BYPOSITION) > 0x00000000)
            {
                string wholeString = loadedStringBuilder.ToString();
                return (Key)(new KeyConverter().ConvertFromString(wholeString.Substring(wholeString.IndexOf("+")).Substring(1)));
            }
            else
            {
                GlobalLog.LogEvidence("Uh oh, something terrible happened trying to get the Ctrl-N string!... falling back to english");
                return Key.N;
            }
        }

        /// <summary>
        /// Presses "Ctrl-N" to pop a new IE Window ...
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            Key LocalizedCtrlN = IECtrlNString();
            Thread.Sleep(2000);
            GlobalLog.LogDebug("Pressing Ctrl-N to create a new IE window ... ");
            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);
            IEWindow.SetFocus();
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, true);
            MTI.Input.SendKeyboardInput(LocalizedCtrlN, true);
            Thread.Sleep(500);
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, false);
            MTI.Input.SendKeyboardInput(LocalizedCtrlN, false);
            return UIHandlerAction.Unhandled;
        }
    }

    /// <summary>
    /// UI handler
    /// </summary>
    public class RefreshAccelKeyHandler : UIHandler
    {

        /// <summary>
        /// Exercises refresh accelerator keys for IE.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            GlobalLog.LogEvidence("Starting Refresh accel-key test...");
            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);

            bool testSucceeded = true;

            IEWindow.SetFocus();

            testSucceeded &= SetupForRefresh(IEWindow);

            GlobalLog.LogEvidence("Pressing F5...");
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.F5, true);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.F5, false);

            testSucceeded &= EnsureRefreshCleared(IEWindow);

            GlobalLog.LogEvidence("Pressing Ctrl-F5...");
            testSucceeded &= SetupForRefresh(IEWindow);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, true);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.F5, true);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.F5, false);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, false);
            testSucceeded &= EnsureRefreshCleared(IEWindow);

            GlobalLog.LogEvidence("Pressing Ctrl-R...");
            testSucceeded &= SetupForRefresh(IEWindow);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, true);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.R, true);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.R, false);
            MTI.Input.SendKeyboardInput(System.Windows.Input.Key.LeftCtrl, false);
            testSucceeded &= EnsureRefreshCleared(IEWindow);

            if (testSucceeded)
            {
                if (TestLog.Current != null)
                {
                    TestLog.Current.Result = TestResult.Pass;
                }
                GlobalLog.LogEvidence("F5, Ctrl-F5, and Ctrl-R test passed!!!");
            }
            else
            {
                if (TestLog.Current != null)
                {
                    TestLog.Current.Result = TestResult.Fail;
                }
                GlobalLog.LogEvidence("F5, Ctrl-F5, and Ctrl-R test failed!!!");
            }

            return UIHandlerAction.Abort;
        }

        private static bool SetupForRefresh(AutomationElement refElement)
        {
            bool success = true;

            success = success && txtBoxHasText(refElement, false);

            if (txtBoxHasText(refElement, false))
                GlobalLog.LogDebug("Didn't have text in the textbox before setting...");

            setTxtBoxText(refElement, "RefreshTest");

            success = success && txtBoxHasText(refElement, true);

            if (txtBoxHasText(refElement, true))
                GlobalLog.LogDebug("Had text in the textbox after setting...");

            return success;
        }

        private static bool EnsureRefreshCleared(AutomationElement refElement)
        {
            if (txtBoxHasText(refElement, false))
            {
                GlobalLog.LogDebug("Refresh cleared the text box!");
            }
            return txtBoxHasText(refElement, false);
        }

        private static void setTxtBoxText(AutomationElement refElement, string TextToSet)
        {
            if (refElement == null)
            {
                refElement = AutomationElement.RootElement;
                GlobalLog.LogDebug("Had to use root element since refElement was null!");
            }
            AutomationElement txtBox = IEAutomationHelper.WaitForElementWithAutomationId(refElement, "refreshTestTxtBox", 45);

            ValuePattern vp = txtBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            vp.SetValue(TextToSet);
            
        }

        private static bool txtBoxHasText(AutomationElement refElement, bool shouldHaveText)
        {
            if (refElement == null)
            {
                refElement = AutomationElement.RootElement;
                GlobalLog.LogDebug("Had to use root element since refElement was null!");
            }
            AutomationElement txtBox = IEAutomationHelper.WaitForElementWithAutomationId(refElement, "refreshTestTxtBox", 45);
            ValuePattern vp = txtBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            if (shouldHaveText)
                return (vp.Current.Value.Length > 0);
            else
                return (vp.Current.Value.Length == 0);
        }
    }

    /// <summary>
    /// UI handler
    /// </summary>
    public class FireFoxRefreshHandler : UIHandler
    {
        /// <summary>
        /// Exercises refresh accelerator keys for FireFox.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            GlobalLog.LogEvidence("Starting FireFox Refresh behavior test...");

            AutomationElement ffWindow = AutomationElement.FromHandle(topHwnd);
            ffWindow.SetFocus();

            // Set a value into the textbox
            AutomationElement txtBox = IEAutomationHelper.WaitForElementWithAutomationId(ffWindow, "refreshTestTxtBox", 45);
            ValuePattern vp = txtBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            vp.SetValue("refresh test");

            // Find the PresentationHost process... Sleep a bit first to let any old instances die if they're going to.
            Thread.Sleep(4000);
            Process[] presHosts = Process.GetProcessesByName("presentationhost");
            if (presHosts.Length != 1)
            {
                throw new InvalidOperationException("Error! Should only be one instance of PresentationHost running at this point");
            }
            int firstPresHostId = presHosts[0].Id;

            // Refresh then give UIA a second to catch up... 
            FireFoxAutomationHelper.ClickFireFoxRefreshButton(topHwnd);            
            Thread.Sleep(1000);

            // Find the text box again since the app will have restarted
            txtBox = IEAutomationHelper.WaitForElementWithAutomationId(ffWindow, "refreshTestTxtBox", 45);

            // Sleep a bit first to let any old instances die if they're going to.
            Thread.Sleep(4000);
            presHosts = Process.GetProcessesByName("presentationhost");
            bool gotNewProcess = false;
            bool refreshEmptiedTextBox = false;

            if (presHosts.Length != 1)
            {
                throw new InvalidOperationException("Error! Should only be one instance of PresentationHost running at this point");
            }

            if (firstPresHostId == presHosts[0].Id)
            {
                GlobalLog.LogEvidence("Error! Same instance of PresentationHost process present after refresh in FireFox (restart expected)");
            }
            else
            {
                GlobalLog.LogEvidence("Success... saw new instance of PresentationHost process present after refresh in FireFox (restart expected)");
                gotNewProcess = true;
            }

            vp = txtBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;


            if (vp.Current.Value.Length == 0)
            {
                GlobalLog.LogEvidence("Success ... refresh button restarted .xbap in FireFox");
                refreshEmptiedTextBox = true;
            }
            else
            {
                GlobalLog.LogEvidence("Failure ... refresh button appears to have not restarted .xbap in FireFox");
            }

            if (refreshEmptiedTextBox && gotNewProcess)
            {
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.Result = TestResult.Fail;
            }

            return UIHandlerAction.Abort;
        }
    }
}
