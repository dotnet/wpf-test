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
using Microsoft.Test.Deployment;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// UI handler
    /// </summary>
    public class WebOCFileLockingHandler: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public WebOCFileLockingHandler()
        {
        }

        /// <summary>
        /// HandleWindow
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            TestLog log = TestLog.Current;
            bool hitException = false;

            AutomationElement navBtn = IEAutomationHelper.WaitForElementWithName(topHwnd, "Navigate WebOC", 45);

            if (navBtn == null)
            {
                GlobalLog.LogEvidence("Couldn't find the navigate button!");
                log.Result = TestResult.Fail;
                return UIHandlerAction.Abort;
            }

            try
            {
                FileStream fs = File.Open(Directory.GetCurrentDirectory() + "\\" + "deploy_htmlmarkup.htm", FileMode.Open);
                GlobalLog.LogEvidence("Successfully opened a handle to the HTML file (Not Locked)...");
                fs.Close();
            }
            catch 
            {
                hitException = true;
                GlobalLog.LogEvidence("Hit an exception trying to open the file...");
            }

            PropertyCondition isTxtBox = new PropertyCondition(AutomationElement.AutomationIdProperty, "txtBox");
            AutomationElement window = AutomationElement.FromHandle(topHwnd);
            AutomationElement element = window.FindFirst(TreeScope.Descendants, isTxtBox);
            ValuePattern vp = element.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            vp.SetValue(vp.Current.Value.Replace("deploy_htmlmarkup.htm", "deploy_markup1.xaml"));

            InvokePattern ip = navBtn.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            ip.Invoke();

            Thread.Sleep(5000);

            try
            {
                FileStream fs = File.Open(Directory.GetCurrentDirectory() + "\\" + "deploy_markup1.xaml", FileMode.Open);
                GlobalLog.LogEvidence("Successfully opened a handle to the XAML file (Not Locked)...");
                fs.Close();
            }
            catch 
            {
                hitException = true;
                GlobalLog.LogEvidence("Hit an exception trying to open the file...");
            }

            if (hitException)
            {
                GlobalLog.LogEvidence("Failed - Hit exception trying to access file opened by WebOC control.");
                log.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("Passed! - No exceptions thrown trying to access files opened by WebOC control.");
                log.Result = TestResult.Pass;
            }
            return UIHandlerAction.Abort;
        }
    }

    public class WebOCDisposalRegressionHandler : UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public WebOCDisposalRegressionHandler()
        {
        }

        /// <summary>
        /// HandleWindow
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement theWindow = AutomationElement.FromHandle(topHwnd);
            theWindow.SetFocus();
            bool succeeded = true;

            IEAutomationHelper.WaitForElementWithAutomationId(theWindow, "xamlTestButton", 10);

            succeeded &= CheckPresentationHostCount(2);

            IEAutomationHelper.InvokeElementViaAutomationId(theWindow, "page1Link", 10);
            GlobalLog.LogEvidence("Clicked link to go to 2nd page...");

            IEAutomationHelper.WaitForElementWithAutomationId(theWindow, "Page2TextBox", 10);

            Thread.Sleep(4000);

            succeeded &= CheckPresentationHostCount(0);

            IEAutomationHelper.InvokeElementViaAutomationId(theWindow, "goWebOCMainPage", 10);
            GlobalLog.LogEvidence("Clicked link to go back to 1st page...");
            Thread.Sleep(4000);

            succeeded &= CheckPresentationHostCount(2);

            if (succeeded)
            {
                GlobalLog.LogEvidence("Success: PresentationHost.exe instances disposed after navigating away from WebOC Xaml page (1521096 Regression prevention)");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("FAILURE: PresentationHost.exe instances not disposed after navigating away from WebOC Xaml page (1521096 Regression prevention)");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }

        private static bool CheckPresentationHostCount(int count)
        {
            System.Diagnostics.Process[] runningPhosts = System.Diagnostics.Process.GetProcessesByName("PresentationHost");

            if (runningPhosts.Length == count)
            {
                GlobalLog.LogEvidence("Found " + runningPhosts.Length + " running PresentationHost processes (from WebOC)...");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("ERROR: Expected " + count + ", but Found " + runningPhosts.Length + " running PresentationHost processes (from WebOC)...");
                return false;
            }
        } 
    }
}
