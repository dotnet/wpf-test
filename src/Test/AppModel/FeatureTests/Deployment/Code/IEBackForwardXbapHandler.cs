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
    public class IE7BackForwardHandler: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IE7BackForwardHandler() {}

        /// <summary>
        /// Prevents regression of WinOS.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);
            IEWindow.SetFocus();

            ParameterizedThreadStart workerThread = new ParameterizedThreadStart(handleWindowNewThread);
            Thread thread = new Thread(workerThread);
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start((object)IEWindow);

            thread.Join();

            return UIHandlerAction.Abort;
        }

        private static void handleWindowNewThread(object theWindow)
        {
            AutomationElement IEWindow = (AutomationElement)theWindow;
            // Sometimes this seems to hang.  Focusing the window a second time after waiting for a bit seems to help.
            Thread.Sleep(3000);
            IEWindow.SetFocus();
            IEAutomationHelper.ClickHTMLHyperlinkByName(IEWindow, "HtmlRelXbap");
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "btnSecurityTester", 15);
            GlobalLog.LogEvidence("Sucessfully navigated HTML -> XBAP");
            IEAutomationHelper.ClickIEBackButton(IEWindow);
            IEAutomationHelper.WaitForElementByAndCondition(IEWindow, new AndCondition(
                new PropertyCondition(AutomationElement.NameProperty, "HtmlRelXbap"),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink)), 15);
            GlobalLog.LogEvidence("Sucessfully navigated using back button XBAP -> HTML");
            IEAutomationHelper.ClickIEFwdButton(IEWindow);
            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "btnSecurityTester", 15);
            GlobalLog.LogEvidence("PASS: Sucessfully navigated using Fwd button HTML -> XBap ");
            TestLog.Current.Result = TestResult.Pass;
        }
    }


    public class IE7CanceledNavigationJournalHandler : UIHandler
    {

        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            if (SystemInformation.Current.IEVersion.StartsWith("6"))
            {
                // No Journal integration Pre-IE7... so set result to Ignore and return.
                GlobalLog.LogEvidence("120574 Regression test cannot run on IE6, ignoring result and returning...");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }

            AutomationElement ieWindow = AutomationElement.FromHandle(topLevelhWnd);
            ieWindow.SetFocus();

            // Get pages on both the forward stack and back stack... 
            IEAutomationHelper.InvokeElementViaAutomationId(ieWindow, "page1Link", 20);
            IEAutomationHelper.InvokeElementViaAutomationId(ieWindow, "goXbapMainPage", 20);
            IEAutomationHelper.InvokeElementViaAutomationId(ieWindow, "page1Link", 20);
            IEAutomationHelper.ClickIEBackButton(AutomationElement.RootElement);
            // Set handler to cancel all navigations in app
            IEAutomationHelper.InvokeElementViaAutomationId(ieWindow, "cancelNavigatingEnabler", 20);
            // Click Fwd button twice.  If the journal is getting out of sync, the 2nd one will throw an exception since there's only 1 page on the fwd stack.
            IEAutomationHelper.ClickIEFwdButton(AutomationElement.RootElement);
            Thread.Sleep(1000);
            bool forwardStackPreserved = true;

            try
            {
                IEAutomationHelper.ClickIEFwdButton(AutomationElement.RootElement);
                GlobalLog.LogEvidence("Did not hit exception trying to click Forward a second time.  Cancelled navigations not affecting the journal.");
            }
            catch (System.OperationCanceledException)
            {
                GlobalLog.LogEvidence("Hit exception trying to click Forward a second time.  Cancelled navigations are affecting the journal!");
                forwardStackPreserved = false;
            }

            if (forwardStackPreserved)
            {
                GlobalLog.LogEvidence("Success... forward button enabled after canceled forward navigation... Journal entries were preserved!");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Fail... forward button disabled after canceled forward navigation... Journal entries were lost");
                TestLog.Current.Result = TestResult.Fail;
            }

            return UIHandlerAction.Abort;
        }
    }


    public class IE7RapidNavigationHandler : UIHandler
    {
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            if (SystemInformation.Current.IEVersion.StartsWith("6"))
            {
                // No navigation integration Pre-IE7... so set result to Ignore and return.
                GlobalLog.LogEvidence(" Regression test 1 cannot run on IE6, ignoring result and returning...");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }

            AutomationElement ieWindow = AutomationElement.FromHandle(topLevelhWnd);
            ieWindow.SetFocus();

            DoFastNavTest(topLevelhWnd);

            GlobalLog.LogEvidence("Success... 50 rapid forward/back IE7 navigations performed within Xbap");
            TestLog.Current.Result = TestResult.Pass;

            return UIHandlerAction.Abort;
        }

        private void DoFastNavTest(IntPtr ieWindowHwnd)
        {
            AutomationElement ieWindow = AutomationElement.FromHandle(ieWindowHwnd);
            PropertyCondition isPage1 = new PropertyCondition(AutomationElement.NameProperty, "This is Page 1");
            PropertyCondition isPage2 = new PropertyCondition(AutomationElement.NameProperty, "This is Page 2");
            PropertyCondition isPage3 = new PropertyCondition(AutomationElement.NameProperty, "This is Page 3");

            GlobalLog.LogDebug("Debug 1");
            IEAutomationHelper.InvokeElementViaAutomationId(ieWindow, "page1Link", 20);
            IEAutomationHelper.WaitForElementByPropertyCondition(ieWindow, isPage1, 20);
            IEAutomationHelper.InvokeElementViaAutomationId(ieWindow, "goPage2", 20);
            IEAutomationHelper.WaitForElementByPropertyCondition(ieWindow, isPage2, 20);
            IEAutomationHelper.InvokeElementViaAutomationId(ieWindow, "goPage3", 20);
            IEAutomationHelper.WaitForElementByPropertyCondition(ieWindow, isPage3, 20);

            IEAutomationHelper.ClickIEBackButton(ieWindow);

            for (int count = 0; count < 50; count++)
            {
                IEAutomationHelper.ClickIEBackButton(ieWindow);
                WaitForPageElement(ieWindow, isPage1, 40);
                IEAutomationHelper.ClickIEFwdButton(ieWindow);
                WaitForPageElement(ieWindow, isPage2, 40);
            }
        }

        /// <summary>
        /// Waits up to (timeout * 50) milliseconds for a child of the given AutomationElement to satisfy PropertyCondition pc
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="pc"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        private static void WaitForPageElement(AutomationElement parentElement, PropertyCondition pc, int timeout)
        {
            if (parentElement == null)
            {
                return;
            }

            AutomationElement element = null;

            while ((element == null) && (timeout >= 0))
            {
                try
                {
                    element = parentElement.FindFirst(TreeScope.Descendants, pc);
                }
                catch (System.InvalidOperationException)
                {
                    // Ignore this... it happens if we're navigating away and the app is being disposed.
                }
                Thread.Sleep(50);
                timeout--;
            }
        }
    }
}
