// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using System.Windows.Automation;
using System.Threading;
using System.Runtime.InteropServices;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// AppVerifier for Browser-Hosted Applications
    /// </summary>
    public class BrowserHostedApplicationVerifier : AppVerifier
    {
        /// <summary>
        /// Adds iexplore and presentationhost processes to list of processes to check
        /// </summary>
        public BrowserHostedApplicationVerifier()
        {
            GlobalLog.LogDebug("Entering constructor for  " + this.ToString());
            // check for these processes
            this.ProcessesToCheck.Add("iexplore");
            this.ProcessesToCheck.Add("presentationhost");
        }
        /// <summary>
        /// Calls base HandleWindow()
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            base.HandleWindow(topLevelhWnd, hwnd, process, title, notification);
            // Once the window has been handled, we're done... abort out
            return UIHandlerAction.Abort;
        }
    }

    public class FireFoxBrowserHostedApplicationVerifier : AppVerifier
    {
        /// <summary>
        /// Adds FireFox,presentationhost and Presen~1 (odd name seen when launched by Firefox) processes to list of processes to check
        /// Also since this is not the BrowserHostedApplicationVerifier class, automatically precludes check of IE zone, other IE specific checks
        /// </summary>
        public FireFoxBrowserHostedApplicationVerifier()
        {
            GlobalLog.LogDebug("Entering constructor for  " + this.ToString());
            // check for these processes
            this.ProcessesToCheck.Add("firefox");
            this.ProcessesToCheck.Add("presentationhost");
        }
        /// <summary>
        /// Calls base HandleWindow()
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            base.HandleWindow(topLevelhWnd, hwnd, process, title, notification);
            // Once the window has been handled, we're done... abort out
            return UIHandlerAction.Abort;
        }
    }

    public class UserAgentStringTestVerifier : UIHandler
    {
        public string UserAgentTestValue = "";


        /// <summary>
        /// Calls base HandleWindow()
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement thisWindow = AutomationElement.FromHandle(topLevelhWnd);

            AutomationElement stringTestButton = IEAutomationHelper.WaitForElementWithAutomationId(thisWindow, "userAgentStringTestBtn", 30);
            GlobalLog.LogEvidence("Invoking UserAgent string test button... this will attempt to cause an HTTP navigation and then output the associated UserAgent string...");
            IEAutomationHelper.InvokeElementViaAutomationId(thisWindow, "userAgentStringTestBtn", 10);

            // Let UIAutomation info catch up so we get the correct value...
            Thread.Sleep(1000);

            string receivedUserAgentString = stringTestButton.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;

            if (receivedUserAgentString == "User Agent String test")
            {
                GlobalLog.LogEvidence("Error in test... user agent string did not get set to button value.  Returning w/ failure...");
                TestLog.Current.Result = TestResult.Fail;
                return UIHandlerAction.Abort;
            }
            else
            {
                GlobalLog.LogEvidence("Got user agent string: " + receivedUserAgentString);
            }

            if (receivedUserAgentString.ToLowerInvariant().Contains(UserAgentTestValue.ToLowerInvariant()))
            {
                GlobalLog.LogEvidence("Success: User agent string contained expected substring (" + UserAgentTestValue + "). ");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failure: Expected User agent string to contain substring (" + UserAgentTestValue + ") but it did not.");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }
    }

    public class TrustDialogModalityHandler : UIHandler
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool IsWindowEnabled(IntPtr hWnd);

        private static readonly int s_GWL_HWNDPARENT = -8;
        
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            bool firstTestSucceeded = false;
            GlobalLog.LogEvidence("Testing that ClickOnce security prompt is parented by a disabled window (the browser)");

            IntPtr trustDialogParent = (IntPtr) Microsoft.Test.Win32.NativeMethods.GetWindowLong(topLevelhWnd, s_GWL_HWNDPARENT);

            if (trustDialogParent == IntPtr.Zero)
            {
                GlobalLog.LogEvidence("Couldn't get parent of hWnd " + topLevelhWnd + ", GetParent() returned " + trustDialogParent.ToString() + ", failing...");
                TestLog.Current.Result = TestResult.Fail;
                return UIHandlerAction.Abort;
            }

            if (!IsWindowEnabled(trustDialogParent))
            {
                firstTestSucceeded = true;
                GlobalLog.LogEvidence("Success: Parent window was NOT enabled (Dialog modality succeeded)");
            }
            else
            {
                GlobalLog.LogEvidence("Failure: Parent window was enabled (Dialog modality failed)");
                TestLog.Current.Result = TestResult.Fail;
                return UIHandlerAction.Abort;
            }

            AutomationElement browserWindow = AutomationElement.FromHandle(trustDialogParent);

            AndCondition isWpfCancelButton = new AndCondition(
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                new PropertyCondition(AutomationElement.NameProperty, ApplicationDeploymentHelper.CancelPageUIButtonName));

            AutomationElement cancelButton = IEAutomationHelper.WaitForElementByAndCondition(browserWindow, isWpfCancelButton, 5);

            // On many setups, UIAutomation can no longer see the cancel button on the HTML progress page.
			// If can't find the cancel button, Ignore this part of test
            if (cancelButton != null)
            {
                System.Windows.Rect boundingRect = (System.Windows.Rect)cancelButton.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                
                AutomationElement theTrustDialog = AutomationElement.FromHandle(topLevelhWnd);
                TransformPattern moveTheTrustDialogOutOfTheWay = (TransformPattern)theTrustDialog.GetCurrentPattern(TransformPattern.Pattern);
                // We're going to use the all-powerful Invoke() here so moving it down a lot is fine... 

                moveTheTrustDialogOutOfTheWay.Move(boundingRect.Bottom + 25.0, boundingRect.Right + 25.0);

                // Ariel out anyways, even if I can't Invoke()... 
                IEAutomationHelper.ClickCenterOfAutomationElement(cancelButton);
                IEAutomationHelper.ClickCenterOfAutomationElement(cancelButton);
                IEAutomationHelper.ClickCenterOfAutomationElement(cancelButton);

                IEAutomationHelper.InvokeElementViaAutomationId(theTrustDialog, "btnInstall", 10);                

                AutomationElement buttonFromRunningApp = IEAutomationHelper.WaitForElementWithAutomationId(browserWindow, "btnSecurityTester", 35);

                if (buttonFromRunningApp != null)
                {
                    TestLog.Current.Result = TestResult.Pass;
                    GlobalLog.LogEvidence("Success: Clicking \"Cancel\" on disabled browser window was ignored, and app allowed to run");
                }
                else
                {
                    TestLog.Current.Result = TestResult.Fail;
                    GlobalLog.LogEvidence("Failure: Clicking \"Cancel\" on disabled browser window was not honored, as app still ran");
                }
            }
            // UIA has failed us... 
            else
            {
                if (firstTestSucceeded)
                {
                    TestLog.Current.Result = TestResult.Pass;
                    GlobalLog.LogEvidence("Success: Couldn't find \"Cancel\" but browser window was disabled for Xbap Trust dialog (UIA bug)");
                }
                else
                {
                    TestLog.Current.Result = TestResult.Fail;
                    GlobalLog.LogEvidence("Failure: Browser window not disabled when showing Clickonce trust prompt!");
                }
            }
            return UIHandlerAction.Abort;
        }
    }
}

