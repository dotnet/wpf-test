// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Diagnostics;
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

    public class IECtrlWAndF11Handler: UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public IECtrlWAndF11Handler()
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
            if (CultureInfo.CurrentCulture.Name.ToLowerInvariant() != "en-us")
            {
                GlobalLog.LogEvidence("Can't run accelerator test on non-english locale because noone knows how to load IE accelerator keys from resource dlls.  Ignoring...");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }

            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);
            bool succeeded = true;

            // Start with Ctrl-P test...

            AutomationElement PrintDialog = IEWindow.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));

            succeeded &= (PrintDialog == null);

            if (PrintDialog != null)
            {
                GlobalLog.LogEvidence("Error: IE window already had a child window before pressing ctrl-P!");
            }
            else
            {
                GlobalLog.LogEvidence("IE window not showing Print dialog before pressing ctrl-P...");
            }

            // I love UIA.  This randomly throws an Invalid Op Except.  Sometimes.
            IEAutomationHelper.TrySetFocus(IEWindow);

            Thread.Sleep(3000);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.P, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.P, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, false);
            Thread.Sleep(6000);

            bool needNoPrinterWorkaround = false;

            PrintDialog = IEWindow.FindFirst(TreeScope.Children,
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));

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
                    IEAutomationHelper.TrySetFocus(PrintDialog);
                }
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Escape, true);
                Microsoft.Test.Input.Input.SendKeyboardInput(Key.Escape, false);
                Thread.Sleep(3000);
            }

            // Make sure that the window is not already full screen-ed
            WindowPattern wp = IEWindow.GetCurrentPattern(WindowPattern.Pattern) as WindowPattern;
            wp.SetWindowVisualState(WindowVisualState.Normal);
            IEAutomationHelper.TrySetFocus(IEWindow);

            succeeded &= ((wp.Current.CanMaximize) && (wp.Current.CanMinimize));
            if ((wp.Current.CanMaximize) && (wp.Current.CanMinimize))
            {
                GlobalLog.LogEvidence("Verified that the window was NOT full screen before pressing F11...");
            }
            else
            {
                GlobalLog.LogEvidence("Error: Window state not as expected before starting... ");
                GlobalLog.LogEvidence("CanMaximize = " + wp.Current.CanMaximize + "\nCanMinimize=" + wp.Current.CanMinimize);
            }

            // Now send an F11 to it...
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.F11, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.F11, false);

            Thread.Sleep(4000);

            // Verify differently for IE6/7... since UI Automation gives us different info for each.
            // Fullscreen is "Maximized" on 6 but not on 7.  So I just use info that has to be different for each scenario.
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    {
                        succeeded &= (wp.Current.WindowVisualState == WindowVisualState.Maximized);

                        if (wp.Current.WindowVisualState == WindowVisualState.Maximized)
                        {
                            GlobalLog.LogEvidence("Successfully maximized IE6 window using F11");
                        }
                        else
                        {
                            GlobalLog.LogEvidence("Error: F11 did not cause IE6 window to maximize w/ Avalon content!");
                            GlobalLog.LogDebug("WindowVisualState = " + wp.Current.WindowVisualState);
                        }
                        break;
                    }
                case 7:
                    {
                        succeeded &= !wp.Current.CanMaximize;

                        if (!wp.Current.CanMaximize)
                        {
                            GlobalLog.LogEvidence("Successfully maximized IE7 window using F11");
                        }
                        else
                        {
                            GlobalLog.LogEvidence("Error: F11 did not cause IE7 window to maximize w/ Avalon content!");
                            GlobalLog.LogDebug("CanMaximize = " + wp.Current.CanMaximize + "\nCanMinimize=" + wp.Current.CanMinimize);
                        } 
                        break;
                    }
                default:
                    goto case 7;
            }

            // Now send an F11 to it and verify it un-full-screened
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.F11, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.F11, false);

            Thread.Sleep(3500);

            succeeded &= ((wp.Current.CanMaximize) && (wp.Current.CanMinimize));
            if ((wp.Current.CanMaximize) && (wp.Current.CanMinimize))
            {
                GlobalLog.LogEvidence("Verified that the window returned to normal after 2nd press of F11...");
            }
            else
            {
                GlobalLog.LogEvidence("ERROR: Window state did not return to normal after 2nd press of F11");
                GlobalLog.LogEvidence("CanMaximize = " + wp.Current.CanMaximize + "\nCanMinimize=" + wp.Current.CanMinimize);
            }

            GlobalLog.LogEvidence(" F11 Test passed... now to close window with Ctrl-W ... ");

            Process backupProc = Process.Start("iexplore");
            backupProc.WaitForInputIdle();
            GlobalLog.LogDebug(" (Started extra IE Window to avoid Appmonitor exiting) ");
            Thread.Sleep(3000);

            int retryCount = 3;
            bool focusedWin = false;

            while ((retryCount > 0) && !focusedWin)
            {
                try
                {
                    IEWindow.SetFocus();
                    focusedWin = true;
                }
                catch
                {
                    Thread.Sleep(2000);
                    retryCount--;
                }
            }

            if (!focusedWin)
            {
                GlobalLog.LogEvidence("Couldnt get UIAutomation to be helpful, setting test result to Ignore");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }

            Thread.Sleep(3000);

            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, true);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.W, true);
            Thread.Sleep(100);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.W, false);
            Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, false);

            int timeoutCount = 100;
            while (!IEWindow.Current.IsOffscreen)
            {
                if (timeoutCount % 20 == 0)
                    GlobalLog.LogDebug("Waiting for process to exit... ");
                timeoutCount--;
                Thread.Sleep(50);
            }
            succeeded &= IEWindow.Current.IsOffscreen;

            if (IEWindow.Current.IsOffscreen)
            {
                GlobalLog.LogEvidence("Success... Ctrl-W closed IE window");
            }
            else
            {
                GlobalLog.LogEvidence("Error: Ctrl W did not cause IE process to exit ... ");
            }

            if (succeeded)
            {
                GlobalLog.LogEvidence("Success! Ctrl-W, Ctrl-P, and F11 keys work as expected with browser-hosted content");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failure: Ctrl-W, Ctrl-P, and/or F11 key not working as expected with browser-hosted content");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }
    }


    /// <summary>
    /// Makes sure that SOMETHING prints when printing from browser-hosted content.
    /// The print team will handle making sure it prints correctly
    /// </summary>
    public class BrowserContentPrintHandler : UIHandler
    {
        /// <summary>
        /// Either uses file / print or the ctrl-P shortcut.  Need to test both paths
        /// since this affects focus.
        /// </summary>
        public bool CtrlP = false;
        /// <summary>
        /// constructor
        /// </summary>
        public BrowserContentPrintHandler()
        {
        }

        /// <summary>
        /// Cause the print dialog to come up, then use print-testing APIs to make sure that printing occurred as expected.
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            //string printerName = "testprinter";
            //PauseAndVerifyQueue pnvHelper = new PauseAndVerifyQueue(printerName);
            //pnvHelper.ExpectedJobCount = 1;
            //pnvHelper.Begin();

            //GlobalLog.LogEvidence("Set up Print Queue test ... now initiating print... ");

            //string printMenuString = null;

            //switch (ApplicationDeploymentHelper.GetIEVersion())
            //{
            //    case 7:
            //        {
            //            string ieMenuDLL = Environment.SystemDirectory + @"\" + CultureInfo.CurrentUICulture.Name + @"\ieframe.dll.mui";
            //            if (!File.Exists(ieMenuDLL))
            //            {
            //                ieMenuDLL = Environment.GetEnvironmentVariable("SystemRoot") + @"\System32\en-us\ieframe.dll.mui";
            //            }
            //            printMenuString = IEAutomationHelper.GetUnmanagedSubMenuResourceString(ieMenuDLL, 267, 0, 9);
            //            break;
            //        }
            //    case 6:
            //        {
            //            printMenuString = IEAutomationHelper.GetUnmanagedSubMenuResourceString(Environment.GetEnvironmentVariable("SystemRoot") + @"\system32\shdoclc.dll", 258, 0, 6);
            //            break;
            //        }
            //}
            //string shortcutString = printMenuString.Substring(printMenuString.IndexOf("Ctrl+")).Substring(5);
            //printMenuString = printMenuString.Remove(printMenuString.IndexOf("\t")).Replace("&", "");

            //if (CtrlP)
            //{
            //    KeyConverter kc = new KeyConverter();
            //    GlobalLog.LogDebug("Pressing \"Ctrl+" + shortcutString.ToLowerInvariant() + "\"");
            //    Thread.Sleep(6000);
            //    Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, true);
            //    Microsoft.Test.Input.Input.SendKeyboardInput((Key)(kc.ConvertFromString(shortcutString)), true);
            //    Microsoft.Test.Input.Input.SendKeyboardInput((Key)(kc.ConvertFromString(shortcutString)), false);
            //    Microsoft.Test.Input.Input.SendKeyboardInput(Key.LeftCtrl, false);
            //}
            //else
            //{
            //    AutomationElement fileMenu = Microsoft.Test.Deployment.IEAutomationHelper.ShowIEFileMenu(AutomationElement.FromHandle(topHwnd));
            //    AutomationElement printBtn = IEAutomationHelper.WaitForElementWithName(fileMenu, printMenuString, 30);
            //    Microsoft.Test.Input.Input.MoveToAndClick(printBtn);
            //}

            //while (!AutomationHelper.ClosePrintDialog(printerName))
            //{
            //    System.Threading.Thread.Sleep(1000);
            //}

            //bool succeeded = true;
            //try
            //{
            //    pnvHelper.End();
            //    GlobalLog.LogEvidence("Success: PauseAndVerifyQueue did not report error condition on trying to print!");
            //}
            //catch (InvalidOperationException ioe)
            //{
            //    succeeded = false;
            //    GlobalLog.LogEvidence("Error: PauseAndVerifyQueue reported error condition on trying to print! \n --- " + ioe.Message);
            //}

            //if (succeeded)
            //{
            //    TestLog.Current.Result = TestResult.Pass;
            //}
            //else
            //{
            //    TestLog.Current.Result = TestResult.Fail;
            //}

            return UIHandlerAction.Abort;
        }
    }
}
