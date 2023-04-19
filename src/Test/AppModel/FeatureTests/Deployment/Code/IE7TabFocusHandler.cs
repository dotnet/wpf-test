// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
using Microsoft.Test.Diagnostics;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// UI handler
    /// </summary>
    public class IE7TabFocusHandler: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IE7TabFocusHandler()
        {
        }

        /// <summary>
        /// IE7 only.  When triggered, focuses a particular element in a test xbap, opens a new tab and navigates it to microsoft.com, then comes back to the original tab, verifying remaining focus on element.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            if (ApplicationDeploymentHelper.GetIEVersion() < 7)
            {
                GlobalLog.LogEvidence("PASS: Test only runs on IE7 or greater");
                TestLog.Current.Result = TestResult.Pass;
                return UIHandlerAction.Abort;
            }

            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);

            AutomationElement xbapButton = IEWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "btnInfiniteLoop"));
            xbapButton.SetFocus();

            GlobalLog.LogDebug("Focused button on Xbap...");

            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.T, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.T, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, false);

            
            //// Find the part of the window with all the tabs ... 
            //PropertyCondition isTabBand = new PropertyCondition(AutomationElement.ClassNameProperty, "DirectUIHWND");
            //AutomationElement tabBand = IEWindow.FindFirst(TreeScope.Descendants, isTabBand);

            //// Get all the buttons from here... 
            //PropertyCondition isBtn = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
            //PropertyCondition isTab = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem);
            //AutomationElement newTabBtn = tabBand.FindFirst(TreeScope.Descendants, isBtn);
            //AutomationElement lastBrowserTab = tabBand.FindFirst(TreeScope.Descendants, isTab);

            //// ... and invoke the tab button
            //InvokePattern ip = newTabBtn.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            //ip.Invoke();

            Thread.Sleep(2000);
            GlobalLog.LogDebug("Popped new IE Tab... ");

            // Navigate this tab somewhere... 
            AndCondition isAddrBar = IEAutomationHelper.GetIEAddressBarAndCondition();
            AutomationElement addrBar = IEWindow.FindFirst(TreeScope.Descendants, isAddrBar);
            ValuePattern vp = addrBar.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            vp.SetValue("about:blank");
            addrBar.SetFocus();
            // Double-press enter becos it doesnt matter, and for IME languages this is required.  Give it 10 seconds to load content.
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            Thread.Sleep(10);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, false);

            Thread.Sleep(3000);

            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, true);
            Thread.Sleep(10);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Enter, false);
            Thread.Sleep(10000);

            GlobalLog.LogEvidence("Navigated new IE Tab to html content... ");

            // Go back to the first tab

            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftShift, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Tab, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Tab, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftShift, false);            
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, false);

            GlobalLog.LogDebug("Went back to xbap tab... ");

            xbapButton = IEWindow.FindFirst(TreeScope.Descendants,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "btnInfiniteLoop"));

            if ((bool)xbapButton.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty, true))
            {
                GlobalLog.LogEvidence("PASS: Button still had focus after navigating other frame to html content");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("FAIL: Button lost focus after navigating other frame to html content");
                TestLog.Current.Result = TestResult.Fail;
            }

            // Revert Enhanced security mode workaround
            if (SystemInformation.Current.IsServer)
            {
                try
                {
                    ApplicationDeploymentHelper.RemoveUrlFromZone(IEUrlZone.URLZONE_TRUSTED, "http://*.microsoft.com");
                }
                catch { } // Do nothing.  Fails on some odd configurations
            }

            return UIHandlerAction.Abort;
        }
    }

    /// <summary>
    /// UI handler
    /// </summary>
    public class IE7DocObjFocusHandler : UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IE7DocObjFocusHandler()
        {
        }

        /// <summary>
        /// Automation ID to try to tab to.  Needs to be inside the docobj though.
        /// </summary>
        public string TabElementId = "";

        /// <summary>
        /// IE7 only.  When triggered, focuses a particular element in a test xbap, opens a new tab and navigates it to microsoft.com, then comes back to the original tab, verifying remaining focus on element.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            if (ApplicationDeploymentHelper.GetIEVersion() < 7)
            {
                GlobalLog.LogEvidence("PASS: Test only runs on IE7 or greater");
                TestLog.Current.Result = TestResult.Pass;
                return UIHandlerAction.Abort;
            }

            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);
            IEWindow.SetFocus();

            // Test Stability... for Loose Xaml case, this click happens to fast and messes up addr bar focus.  Sleep for a bit first.
            Thread.Sleep(5000);

            AndCondition isAddrBar = IEAutomationHelper.GetIEAddressBarAndCondition();
            AutomationElement addressBar = IEWindow.FindFirst(TreeScope.Descendants, isAddrBar);
            AutomationElement tabElement = IEWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, TabElementId));
            
            IEAutomationHelper.ClickCenterOfAutomationElement(addressBar);
            IEAutomationHelper.ClickCenterOfAutomationElement(addressBar);

            if (addressBar.Current.HasKeyboardFocus)
            {
                GlobalLog.LogEvidence("Address Bar started with keyboard focus ...");
            }

            if (!tabElement.Current.HasKeyboardFocus)
            {
                GlobalLog.LogEvidence("Tab element did not start with keyboard focus ...");
            }

            // Now press tab until it goes from the address bar to the chosen element...
            // Assume that the element is < 30 tab stops from address bar (will be for MY tests :) )
            int failureCountdown = 30;
            while ((!tabElement.Current.HasKeyboardFocus) && (failureCountdown > 0))
            {
                failureCountdown--;
                MTI.Input.SendKeyboardInput(Key.Tab, true);
                MTI.Input.SendKeyboardInput(Key.Tab, false);
                Thread.Sleep(750);
            }

            if (tabElement.Current.HasKeyboardFocus)
            {
                GlobalLog.LogEvidence("PASS: in-docobj element eventually got keyboard focus starting outside ...");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("FAIL: Could not tab to in-docobj element from outside the docobj!!!");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }
    }

    /// <summary>
    /// Tests Alt+Key, Ctrl+Key, F6/Shift-F6, Arrow keys for correct focus and handling
    /// </summary>
    public class IE7InputFilteringHandler : UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IE7InputFilteringHandler()
        {
        }

        /// <summary>
        /// IE7 only.  When triggered, focuses a particular element in a test xbap, opens a new tab and navigates it to microsoft.com, then comes back to the original tab, verifying remaining focus on element.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            if (ApplicationDeploymentHelper.GetIEVersion() < 7)
            {
                GlobalLog.LogEvidence("IGNORE: Test only runs on IE7 or greater");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }
            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpperInvariant() == "HU")
            {
                GlobalLog.LogEvidence("IGNORE: Due to weird collision with non-localized WPF and Hungarian IE Menus (Both register for ALT E), setting result to ignore and ending test.");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }

            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);

            bool succeeded = true;

            // ** Begin Alt-E section

            GlobalLog.LogEvidence("Begin Test: Making sure app access key (alt-E) prevents this from reaching the browser");

            IEWindow.SetFocus();

            Thread.Sleep(2000);
            MTI.Input.SendKeyboardInput(Key.LeftAlt, true);
            Thread.Sleep(200);
            MTI.Input.SendKeyboardInput(Key.E, true);            
            MTI.Input.SendKeyboardInput(Key.E, false);
            MTI.Input.SendKeyboardInput(Key.LeftAlt, false);
            Thread.Sleep(500);

            // Try to find the "IE Edit" menu ... if we find it, then this Alt-E has bubbled up... 
            AutomationElement IEEdit = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Menu));

            if (IEEdit == null)
            {
                GlobalLog.LogEvidence("Success: Alt-E did not bring up edit menu... (Mapped in App)");
            }
            else
            {
                GlobalLog.LogEvidence("Error... Initial Alt-E brought up edit menu!");
                succeeded = false;
            }

            // ** End Alt-E section

            // ** Begin Ctrl-F section
            GlobalLog.LogEvidence("Begin Test: Making sure app access key (ctrl-F) does not reach the browser");
            AutomationElement ctrlBtn = IEWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnInputGesture"));
            ctrlBtn.SetFocus();

            Thread.Sleep(2000);
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, true);
            MTI.Input.SendKeyboardInput(Key.F, true);
            MTI.Input.SendKeyboardInput(Key.F, false);
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, false);
            Thread.Sleep(500);

            AutomationElement IEFind = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "Internet Explorer_TridentDlgFrame"));

            if (IEFind == null)
            {
                GlobalLog.LogEvidence("Initial Ctrl-F did not bring up Find menu... ");
                if (ctrlBtn.Current.Name == "Ctrl-F Captured!")
                {
                    GlobalLog.LogEvidence("Success:  App captured the input!");
                }
                else
                {
                    GlobalLog.LogEvidence("Failure:  App failed to capture the input!");
                    succeeded = false;
                }
            }
            else
            {
                GlobalLog.LogEvidence("Error... Initial Ctrl-F brought up find dialog!");
            }
            // ** End Ctrl-F section

            // ** Begin F6 section

            GlobalLog.LogEvidence("**** Begin F6 test \n   (make sure focus can cycle through browser and app w/ F6 & Shift-F6)");
            IEWindow.SetFocus();
            AndCondition isAddrBar = IEAutomationHelper.GetIEAddressBarAndCondition();
            AutomationElement addrBar = IEAutomationHelper.WaitForElementByAndCondition(topHwnd, isAddrBar, 10);
            AutomationElement inAppElement = IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "refreshTestTxtBox", 10);

            // Get focus to the address bar...
            IEAutomationHelper.ClickCenterOfAutomationElement(addrBar);

            int countDown = 30;
            while ((!inAppElement.Current.HasKeyboardFocus) && countDown > 0)
            {
                MTI.Input.SendKeyboardInput(Key.F6, true);
                MTI.Input.SendKeyboardInput(Key.F6, false);
                Thread.Sleep(350);
                countDown--;
            }

            if (countDown > 0)
            {
                GlobalLog.LogEvidence("Success: Focus traveled from address bar to in app via F6!");
            }
            else
            {
                GlobalLog.LogEvidence("Failure:  Focus didn't travel from address bar to in app in 30 F6's!");
                succeeded = false;
            }

            countDown = 30;
            while ((!addrBar.Current.HasKeyboardFocus) && countDown > 0)
            {
                MTI.Input.SendKeyboardInput(Key.F6, true);
                MTI.Input.SendKeyboardInput(Key.F6, false);
                Thread.Sleep(350);
                countDown--;
            }

            if (countDown > 0)
            {
                GlobalLog.LogEvidence("Success: Focus traveled back to address bar from app via F6!");
            }
            else
            {
                GlobalLog.LogEvidence("Failure:  Focus didn't travel to address bar from in app in 30 F6's!");
                succeeded = false;
            }

            // Now repeat with Shift-F6 (backwards version of the same thing)

            countDown = 30;
            while ((!inAppElement.Current.HasKeyboardFocus) && countDown > 0)
            {
                MTI.Input.SendKeyboardInput(Key.LeftShift, true);
                MTI.Input.SendKeyboardInput(Key.F6, true);
                MTI.Input.SendKeyboardInput(Key.F6, false);
                MTI.Input.SendKeyboardInput(Key.LeftShift, false);
                Thread.Sleep(350);
                countDown--;
            }

            if (countDown > 0)
            {
                GlobalLog.LogEvidence("Success: Focus traveled from address bar to in app via Shift-F6!");
            }
            else
            {
                GlobalLog.LogEvidence("Failure:  Focus didn't travel from address bar to in app in 30 Shift-F6's!");
                succeeded = false;
            }

            countDown = 30;
            while ((!inAppElement.Current.HasKeyboardFocus) && countDown > 0)
            {
                MTI.Input.SendKeyboardInput(Key.LeftShift, true);
                MTI.Input.SendKeyboardInput(Key.F6, true);
                MTI.Input.SendKeyboardInput(Key.F6, false);
                MTI.Input.SendKeyboardInput(Key.LeftShift, false);
                Thread.Sleep(350);
                countDown--;
            }

            if (countDown > 0)
            {
                GlobalLog.LogEvidence("Success: Focus traveled back to address bar from app via Shift-F6!");
            }
            else
            {
                GlobalLog.LogEvidence("Failure:  Focus didn't traveled back to address bar in 30 Shift-F6's");
                succeeded = false;
            }
            // ** End F6 section

            // ** Begin Arrow Keys section

            GlobalLog.LogEvidence("Begin Arrow keys test: \n  (Makes sure focus loops within the docobj and does not leave for arrow keys)");
            IEWindow.SetFocus();

            IEAutomationHelper.ClickCenterOfElementById("focusPageLink");

            AutomationElement firstBtn = IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "FirstButton", 10);
            AutomationElement lastBtn = IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "LastButton", 10);

            lastBtn.SetFocus();
            
            MTI.Input.SendKeyboardInput(Key.Down, true);
            MTI.Input.SendKeyboardInput(Key.Down, false);

            Thread.Sleep(5000);

            if (firstBtn.Current.HasKeyboardFocus)
            {
                GlobalLog.LogEvidence("Success: Arrow keys did not move focus outside of the app!");
            }
            else
            {
                GlobalLog.LogEvidence("Failure:  Arrow keys appear to have moved focus outside of the app!");
                succeeded = false;
            }

            if (succeeded)
            {
                GlobalLog.LogEvidence("PASS: All browser-input-filtering tests passed.");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("FAIL: One or more input filtering tests failed");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }
    }
}
