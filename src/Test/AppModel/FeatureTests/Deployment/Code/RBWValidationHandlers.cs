// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Diagnostics;
using System.Windows.Automation;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using Microsoft.Test.Deployment;
using Microsoft.Test.Diagnostics;
using System.Windows.Input;
using MTI = Microsoft.Test.Input;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Validates resize behavior of Browser apps 
    /// </summary>
    public class RootBrowserWindowResizeHandler : UIHandler
    {
        #region Public Members
        
        /// <summary>
        /// Validates window resizing code, while also exercising various browser interop codepaths.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement browserWindow = AutomationElement.FromHandle(topLevelhWnd);

            // Make sure it's not max-/mini-mized
            WindowPattern wp = browserWindow.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            wp.SetWindowVisualState(WindowVisualState.Normal);

            // Resize to a known, reasonably small value
            TransformPattern tp = browserWindow.GetCurrentPattern(TransformPattern.Pattern) as TransformPattern;
            tp.Resize(700, 550);

            // Check that our resize worked... 
            Thread.Sleep(1000);
            System.Windows.Rect resultingRect = (System.Windows.Rect)browserWindow.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty, true);
            GlobalLog.LogEvidence("Browser window before resize --> Width: " + resultingRect.Width + " Height: " + resultingRect.Height);

            if (!((resultingRect.Width <= 700) && (resultingRect.Height <= 550)))
            {
                GlobalLog.LogEvidence("Error! Could not set window size for browser resize test.  Aborting... ");
            }

            // Click the button to tell the app to resize itself, then wait a little bit...
            IEAutomationHelper.InvokeElementViaAutomationId(browserWindow, "resizeBrowserWindowTestBtn", 20);
            Thread.Sleep(1000);

            // Validate results
            resultingRect = (System.Windows.Rect)browserWindow.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty, true);
            int maxWidth = Microsoft.Test.Display.Monitor.GetPrimary().Area.Width;
            int maxHeight = Microsoft.Test.Display.Monitor.GetPrimary().Area.Height;

            GlobalLog.LogEvidence("Primary Display Info        --> Width: " + maxWidth + " Height: " + maxHeight);
            GlobalLog.LogEvidence("Browser window after resize --> Width: " + resultingRect.Width + " Height: " + resultingRect.Height);

            if ((maxWidth >= resultingRect.Width) && (maxHeight >= resultingRect.Height))
            {
                GlobalLog.LogEvidence("Pass! Browser app able to resize window up to but not greater than size of primary display");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Fail! Browser app was able to resize window beyond the size of the primary display");
                TestLog.Current.Result = TestResult.Fail;
            }

            return UIHandlerAction.Abort;
        }

        #endregion
    }

    /// <summary>
    /// Simply navigates .xbap --> HTML to exercise browser navigation code 
    /// </summary>
    public class NavigateAwayToHTMLHandler : UIHandler
    {
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement browserWindow = AutomationElement.FromHandle(topLevelhWnd);

            IEAutomationHelper.WaitForElementWithAutomationId(browserWindow, "btnInfiniteLoop", 20);

            browserWindow.SetFocus();

            IEAutomationHelper.ClickCenterOfElementById(browserWindow, "XbapRelHtml");

            return UIHandlerAction.Handled;
        }
    }

    /// <summary>
    /// Exercises various input bubbling scenarios.
    /// </summary>
    public class FireFoxInputBubblingHandler : UIHandler
    {
        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement browserWindow = AutomationElement.FromHandle(topLevelhWnd);
            TrySetFocus(browserWindow, 5);

            bool succeeded = true;

            // Start with Ctrl-P test...

            PropertyCondition isPrintDialog = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window);

            TrySetFocus(browserWindow,5);

            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.P, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.P, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, false);

            Thread.Sleep(5000);

            bool needNoPrinterWorkaround = false;

            AutomationElement PrintDialog = browserWindow.FindFirst(TreeScope.Children, isPrintDialog);

            if ((PrintDialog != null) && (((bool)PrintDialog.GetCurrentPropertyValue(AutomationElement.IsKeyboardFocusableProperty)) == false))
            {
                // On some client skus, test machines will have no printer installed.  Dismiss the dialog that comes up for this.
                // First we find the first child window of the original dialog:
                GlobalLog.LogStatus("Dismissing dialog for test machine with no printer");
                needNoPrinterWorkaround = true;
            }

            succeeded &= (PrintDialog != null);

            if (PrintDialog == null)
            {
                GlobalLog.LogEvidence("Error: Ctrl-P doesn't seem to have spawned a print window!");
            }
            else
            {
                GlobalLog.LogEvidence("Success: Ctrl-P spawned a child print window ... closing print dialog");
                if (needNoPrinterWorkaround)
                {
                    Microsoft.Test.Input.Input.SendKeyboardInput(Key.N, true);
                    Microsoft.Test.Input.Input.SendKeyboardInput(Key.N, false);
                    Thread.Sleep(1000);
                }
                else
                {
                    TrySetFocus(PrintDialog, 5);
                }                                
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Escape, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Escape, false);
                Thread.Sleep(3000);
            }

            // ** Begin Ctrl-F section
            GlobalLog.LogEvidence("Begin Test: Making sure app access key (ctrl-F) does not reach the browser");

            AutomationElement ctrlBtn = browserWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "btnInputGesture"));
            // Click instead of set focus... this fails on some SKUs for no good reason.
            IEAutomationHelper.ClickCenterOfElementById(browserWindow, "btnInputGesture");
            

            Thread.Sleep(2000);
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, true);
            MTI.Input.SendKeyboardInput(Key.F, true);
            MTI.Input.SendKeyboardInput(Key.F, false);
            MTI.Input.SendKeyboardInput(Key.LeftCtrl, false);
            Thread.Sleep(500);

            if (ctrlBtn.Current.Name == "Ctrl-F Captured!")
            {
                GlobalLog.LogEvidence("Success:  App captured the input!");
            }
            else
            {
                GlobalLog.LogEvidence("Failure:  App failed to capture the input!");
                succeeded = false;
            }

            // ** End Ctrl-F section

            // ** Make sure we can get to the FireFox browser menus when the app has focus (accessibility issue)
            // Since we can't get UIA info for the file menu, just use Alt-F, Alt-X and make sure the app exited.
            // This wouldnt happen unless the menu got focus :)

            TrySetFocus(ctrlBtn,5);

            MTI.Input.SendKeyboardInput(Key.LeftAlt, true);
            MTI.Input.SendKeyboardInput(Key.F, true);
            MTI.Input.SendKeyboardInput(Key.F, false);

            Thread.Sleep(2000);

            MTI.Input.SendKeyboardInput(Key.X, true);
            MTI.Input.SendKeyboardInput(Key.X, false);
            MTI.Input.SendKeyboardInput(Key.LeftAlt, false);

            bool processSelfExited = false;

            for (int i = 0; i < 15; i++)
            {
                Thread.Sleep(1000);
                if (process.HasExited)
                {
                    GlobalLog.LogEvidence("FireFox process exited in response to menu commands!");
                    processSelfExited = true;
                    break;
                }
            }

            if (!processSelfExited)
            {
                GlobalLog.LogEvidence("Error: FireFox process did not exit in response to Alt-F/Alt-X combo.  WPF app may be blocking input.");
            }

            succeeded &= processSelfExited;

            if (succeeded)
            {
                GlobalLog.LogEvidence("Success... FireFox Input bubbling test passed (Print, app-captured, bubbled to browser).");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failure... FireFox Input bubbling test failed (Print, app-captured, bubbled to browser).");
                TestLog.Current.Result = TestResult.Fail;
            }

            return UIHandlerAction.Abort;
        }

        private void TrySetFocus(AutomationElement element, int timeout)
        {
            while (timeout > 0)
            {
                try
                {
                    element.SetFocus();
                    return;
                }
                catch
                {
                    timeout--;
                    Thread.Sleep(1000);
                }
            }
        }

    }
}
