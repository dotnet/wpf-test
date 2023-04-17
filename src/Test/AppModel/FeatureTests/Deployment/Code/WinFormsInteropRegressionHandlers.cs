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
using MTI = Microsoft.Test.Input;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// UI handler
    /// </summary>
    public class WinFormsInteropInputProcessingHandler: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public WinFormsInteropInputProcessingHandler()
        {
        }

        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            AutomationElement theWindow = AutomationElement.FromHandle(topHwnd);
            // Click button to show a dialog
            IEAutomationHelper.InvokeElementViaAutomationId(theWindow, "OpenDetailPageCommand", 15);

            // Text box in the WPF control for later use
            AutomationElement inputTextBox = IEAutomationHelper.WaitForElementWithAutomationId(theWindow, "InputTextBox", 15);
            // Wait for the dialog 
            AutomationElement DialogWindow = IEAutomationHelper.WaitForElementWithName(AutomationElement.RootElement, "DetailWindow", 15);

            // Dismiss with Esc (causes key char loss pre-fix)
            MTI.Input.SendKeyboardInput(Key.Escape, true);
            MTI.Input.SendKeyboardInput(Key.Escape, false);
            Thread.Sleep(1000);

            // Type some stuff in the box.  
            // There's a tiny chance of losing a char or two, so dont validate explicitly.
            inputTextBox.SetFocus();
            MTI.Input.SendUnicodeString("TestInputString");

            // Let UIA info propagate, then check the text box's contents.  If we have anything, the fix is still holding.
            Thread.Sleep(1000);
            
            ValuePattern vp = inputTextBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;

            if (vp.Current.Value.Length > 1)
            {
                GlobalLog.LogEvidence("Success! Text box still accepting input (Saw " + vp.Current.Value + ") after escape-cancelled dialog");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Fail! Text box no longer accepting input after escape-cancelled dialog ");
                TestLog.Current.Result = TestResult.Fail;
            }
                 
            return UIHandlerAction.Abort;
        }
    }
}
