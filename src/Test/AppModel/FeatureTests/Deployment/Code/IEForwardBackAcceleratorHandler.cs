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
using System.Windows.Input;
using Microsoft.Test.Deployment;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// UI handler
    /// </summary>
    public class IEForwardBackAcceleratorHandler: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IEForwardBackAcceleratorHandler()
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
            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);
            IEWindow.SetFocus();
             
            bool success = true;

            // Go to Page 2...
            success &= GoToPage2(IEWindow);

            // Hit backspace
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Back, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Back, false);

            // Stability tweak.  Since navigation in IE is always asynchronous, this can confuse UIA (which updates asynchronously itself) if we go too fast.
            Thread.Sleep(600);

            // Wait til we get back to the main page
            if (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "btnSecurityTester", 10) != null)
            {
                GlobalLog.LogEvidence("Success: Backspace key causes back navigation with .xbap");
            }
            else
            {
                success = false; // something went wrong
                GlobalLog.LogEvidence("Uh oh: Issue seen with Backspace...");
            }

            // Go to Page 2 again...
            success &= GoToPage2(IEWindow);

            // Hit Alt-Back
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, false);

            // Stability tweak.  Since navigation in IE is always asynchronous, this can confuse UIA (which updates asynchronously itself) if we go too fast.
            Thread.Sleep(600);

            // Wait til we get back to the main page
            if (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "btnSecurityTester", 10) != null)
            {
                GlobalLog.LogEvidence("Success: \"Alt-Left Arrow\" causes back navigation with .xbap");
            }
            else
            {
                success = false; // something went wrong
                GlobalLog.LogEvidence("Uh oh: Issue seen with Alt-Left...");
            }

            // Go to Page 2 again...
            success &= GoToPage2(IEWindow);

            if (ApplicationDeploymentHelper.GetIEVersion() == 7)
            {
                IEAutomationHelper.ClickIEBackButton(IEWindow);
            }
            else
            {
                // Hit Alt-Back, since IE's back will be disabled here in IE6
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, false);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, false);
            }

            // Stability tweak.  Since navigation in IE is always asynchronous, this can confuse UIA (which updates asynchronously itself) if we go too fast.
            Thread.Sleep(600);

            // Wait til we get back to the main page
            if (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "btnSecurityTester", 10) != null)
            {
                GlobalLog.LogEvidence("Success: IE Back button causes back navigation with .xbap");
            }
            else
            {
                success = false; // something went wrong
                GlobalLog.LogEvidence("Uh oh: Issue seen with IE Back button...");
            }

            // Stability tweak.  Since navigation in IE is always asynchronous, this can confuse UIA (which updates asynchronously itself) if we go too fast.
            try
            {
                Thread.Sleep(1000);
                // Give focus to the DocObj here, or this navigation will be DOA... 
                // In real-world scenarios this would happen automatically due to user interaction.
                IEAutomationHelper.InvokeElementViaAutomationId(IEWindow, "btnSecurityTester", 10);
                IEWindow.SetFocus();
            }
            catch { }; // do nothing... test will fail if these are unsuccessful anyways.

            // Needed for stability, below input gets lost on some machines.
            Thread.Sleep(500);

       // Now for forward tests:

            // Hit Shift-backspace
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.RightShift, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Back, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Back, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.RightShift, false);

            // Stability tweak.  Since navigation in IE is always asynchronous, this can confuse UIA (which updates asynchronously itself) if we go too fast.
            Thread.Sleep(1000);

            // Wait til we get back to the main page
            if (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page2Button", 10) != null)
            {
                GlobalLog.LogEvidence("Success: Shift-Backspace causes Fwd navigation with .xbap");
            }
            else
            {
                success = false; // something went wrong
                GlobalLog.LogEvidence("Uh oh: Issue seen with Shift-Backspace...");
            }

            if (ApplicationDeploymentHelper.GetIEVersion() == 7)
            {
                IEAutomationHelper.ClickIEBackButton(IEWindow);
            }
            else
            {
                // Hit Alt-Back, since IE's back will be disabled here in IE6
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, false);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, false);
            }

            // Stability tweak.  Since navigation in IE is always asynchronous, this can confuse UIA (which updates asynchronously itself) if we go too fast.
            Thread.Sleep(600);

            success &= (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "btnSecurityTester", 10) != null);

            // Hit Alt-Left
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Right, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.Right, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, false);

            // Wait til we get back to the main page
            if (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page2Button", 10) != null)
            {
                GlobalLog.LogEvidence("Success: Alt-Right causes Fwd navigation with .xbap");
            }
            else
            {
                success = false; // something went wrong
                GlobalLog.LogEvidence("Uh oh: Issue seen with Alt-Right Button...");
            }

            if (ApplicationDeploymentHelper.GetIEVersion() == 7)
            {
                IEAutomationHelper.ClickIEBackButton(IEWindow);
            }
            else
            {
                // Hit Alt-Back, since IE's back will be disabled here in IE6
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Left, false);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, false);
            }

            // Stability tweak.  Since navigation in IE is always asynchronous, this can confuse UIA (which updates asynchronously itself) if we go too fast.
            Thread.Sleep(600);

            success &= (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "btnSecurityTester", 10) != null);

            // Hit IE Fwd Button
            if (ApplicationDeploymentHelper.GetIEVersion() == 7)
            {
                IEAutomationHelper.ClickIEFwdButton(IEWindow);
            }
            else
            {
                // Hit Alt-Back, since IE's back will be disabled here in IE6
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Right, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Right, false);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftAlt, false);
            }

            // Stability tweak.  Since navigation in IE is always asynchronous, this can confuse UIA (which updates asynchronously itself) if we go too fast.
            Thread.Sleep(600);

            // Wait til we get back to the main page
            if (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page2Button", 10) != null)
            {
                GlobalLog.LogEvidence("Success: IE Fwd Button causes Fwd navigation with .xbap");
            }
            else
            {
                success = false; // something went wrong
                GlobalLog.LogEvidence("Uh oh: Issue seen with IE Fwd Button...");
            }

            if (success)
            {
                GlobalLog.LogEvidence("Success! All IE back/forward accelerators and buttons working properly with .xbap");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failure: One or more IE Forward/Back accelerator keys or buttons not working with .xbap content");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }

        private static bool GoToPage2(AutomationElement IEWindow)
        {
            // Go to Page 2 ...
            IEAutomationHelper.InvokeElementViaAutomationId(IEWindow, "page2Link", 10);
            // Wait til we see it...
            if (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "Page2Button", 10) != null)
            {
                GlobalLog.LogDebug("Successfully navigated to page 2...");
                return true;
            }
            else
            {
                GlobalLog.LogDebug("Error attempting to navigate to page 2...");
                return false;
            }
        }
    }
}
