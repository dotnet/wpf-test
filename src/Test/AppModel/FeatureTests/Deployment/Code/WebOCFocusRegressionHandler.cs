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
using MTI = Microsoft.Test.Input;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// UI handler
    /// </summary>
    public class WebOCFocusRegressionHandler: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public WebOCFocusRegressionHandler()
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
            AutomationElement ieWindow = AutomationElement.FromHandle(topHwnd);
            ieWindow.SetFocus();

            AutomationElement txtBox = IEAutomationHelper.WaitForElementWithAutomationId(ieWindow, "txtBox", 20);
            AutomationElement btnAfterWebOC = IEAutomationHelper.WaitForElementWithAutomationId(ieWindow, "FocusTestButton", 20);

            txtBox.SetFocus();

            if ((bool)btnAfterWebOC.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty))
            {
                GlobalLog.LogEvidence("Error: Button already had focus before starting!");
            }
            else
            {
                GlobalLog.LogEvidence("Button did not have focus before starting...");
            }

            int giveUpCount = 20;

            while (!((bool)btnAfterWebOC.GetCurrentPropertyValue(AutomationElement.HasKeyboardFocusProperty)))
            {
                Microsoft.Test.Input.Input.SendKeyboardInput(System.Windows.Input.Key.Tab, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(System.Windows.Input.Key.Tab, false);
                Thread.Sleep(500);
                giveUpCount--;
                if (giveUpCount == 0)
                {
                    GlobalLog.LogEvidence("FAIL: Hit tab 20x without getting focus around WebOC Control...1453368 may have regressed");
                    TestLog.Current.Result = TestResult.Fail;
                    return UIHandlerAction.Abort;
                }
            }
            
            GlobalLog.LogEvidence("Passed! - No exceptions thrown trying to tab focus around WebOC control (1453368 Regression Prevention)");
            TestLog.Current.Result = TestResult.Pass;
            return UIHandlerAction.Abort;
        }

    }
    
    public class WebOCInvariantAssertTestHandler : UIHandler
    {
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
            AutomationElement ieWindow = (AutomationElement)theWindow;

            PropertyCondition isComboBox = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.ComboBox);

            AutomationElement webOCControl = IEAutomationHelper.WaitForElementWithName(ieWindow, "DevDiv Bugs # 121501 Repro Page", 30);
            AutomationElement htmlSelect = webOCControl.FindFirst(TreeScope.Descendants, isComboBox);

            ieWindow.SetFocus();
            Thread.Sleep(500);

            GlobalLog.LogEvidence("Clicking HTML select tag... ");

            System.Windows.Rect r = (System.Windows.Rect)htmlSelect.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
            double horizMidPt = r.Left + ((r.Right - r.Left) / 2.0);
            double vertMidPt = r.Top + ((r.Bottom - r.Top) / 2.0);
            MTI.Input.SendMouseInput(horizMidPt, vertMidPt, 0, MTI.SendMouseInputFlags.Absolute | MTI.SendMouseInputFlags.LeftDown | MTI.SendMouseInputFlags.Move);
            MTI.Input.SendMouseInput(horizMidPt, vertMidPt, 0, MTI.SendMouseInputFlags.Absolute | MTI.SendMouseInputFlags.LeftUp | MTI.SendMouseInputFlags.Move);

            GlobalLog.LogEvidence("Waiting for Error UI... ");
            // Sleep for several seconds to allow the error page to show.  If it does, we will fail.  
            Thread.Sleep(5000);

            string windowTitle = ieWindow.GetCurrentPropertyValue(AutomationElement.NameProperty).ToString();

            if (windowTitle.ToLowerInvariant().StartsWith("navigation completed"))
            {
                GlobalLog.LogEvidence("Passed! - No exceptions thrown clicking on HTML Select tag in WebOC (DD bug 121501 regression test)");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failed - Saw HTML error page on clicking HTML select tag in WebOC");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }

}
