// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Threading;
using Microsoft.Test.Deployment;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using System.Runtime.InteropServices;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.Win32;

namespace Microsoft.Test.AppModel.SplashScreen
{
    public class Helpers
    {
        /// <summary>
        /// Waits up to timeout seconds for a child of the given hwnd to satisfy AndCondition ac
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="ac"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementByAndCondition(IntPtr hwnd, AndCondition ac, int timeout)
        {
            AutomationElement window = AutomationElement.FromHandle(hwnd);
            return WaitForElementByAndCondition(window, ac, timeout);
        }

        /// <summary>
        /// Waits up to timeout seconds for a child of the given hwnd to satisfy AndCondition ac
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="ac"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementByAndCondition(AutomationElement parentElement, AndCondition ac, int timeout)
        {
            AutomationElement element = null;
            do
            {
                try
                {
                    element = parentElement.FindFirst(TreeScope.Descendants, ac);
                }
                catch
                {
                    GlobalLog.LogDebug("Yet another random exception from UIAutomation...");
                } // do nothing, UIA is mind-bogglingly buggy and throws randomly

                Thread.Sleep(1000);
                timeout--;
            }
            while ((element == null) && (timeout > 0));

            return element;
        }

        /// <summary>
        /// Waits up to timeout seconds to find and invoke element with automationId that is a child of parentElement
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="AutomationId"></param>
        /// <param name="timeout"></param>
        public static void InvokeElementViaAutomationId(AutomationElement parentElement, string AutomationId, int timeout)
        {
            AutomationElement el = WaitForElementWithAutomationId(parentElement, AutomationId, timeout);
            InvokePattern ip = el.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            if ((ip == null) && (el != null))
            {
                throw new System.InvalidOperationException("Element with automation id \""+AutomationId+"\" did not return an invoke pattern!");
            }
            ip.Invoke();
        }

        /// <summary>
        /// Wait (timeout) seconds for an element with Name (name) to appear as child of HWnd (hwnd)
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="AutoID"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementWithAutomationId(IntPtr hwnd, string AutoID, int timeout)
        {
            PropertyCondition isElement = new PropertyCondition(AutomationElement.AutomationIdProperty, AutoID);
            return WaitForElementByPropertyCondition(hwnd, isElement, timeout);
        }
        
        /// <summary>
        /// Wait (timeout) seconds for an element with Name (name) to appear as child of HWnd (hwnd)
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="AutoID"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementWithAutomationId(AutomationElement parentElement, string AutoID, int timeout)
        {
            PropertyCondition isElement = new PropertyCondition(AutomationElement.AutomationIdProperty, AutoID);
            return WaitForElementByPropertyCondition(parentElement, isElement, timeout);
        }

        /// <summary>
        /// Waits up to timeout seconds for a child of the given hwnd to satisfy PropertyCondition pc
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="pc"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementByPropertyCondition(IntPtr hwnd, PropertyCondition pc, int timeout)
        {
            AutomationElement window = AutomationElement.FromHandle(hwnd);
            return WaitForElementByPropertyCondition(window, pc, timeout);
        }


        /// <summary>
        /// Waits up to timeout seconds for a child of the given AutomationElement to satisfy PropertyCondition pc
        /// </summary>
        /// <param name="parentElement"></param>
        /// <param name="pc"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static AutomationElement WaitForElementByPropertyCondition(AutomationElement parentElement, PropertyCondition pc, int timeout)
        {
            if (parentElement == null)
            {
                return null;
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
                Thread.Sleep(1000);
                timeout--;
            }
            return element;
        }
    }

    public class SplashScreenVerifier : UIHandler
    {
        
        #region Public Members
        public int ExpectedFadeTime = 3000;
        public bool AppWillForceCloseSplash = false;
        public bool CheckSplashScreenTopMost = false;

        #endregion

        #region Private Members
        private static AndCondition s_isSplashScreen = new AndCondition(
                new PropertyCondition(AutomationElement.ClassNameProperty, "SplashScreen"),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane));
        #endregion

        #region Handler Implementation

        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            bool passed = true;

            // Basic Test variation 1: Invoke Splash and have the app manually close it:
            passed &= DoSplashScreenAPITest("SplashTest1", false, AutomationElement.FromHandle(topLevelhWnd));
            // Basic Test variation 2: Invoke Splash and have the app self close it:
            passed &= DoSplashScreenAPITest("SplashTest2", true, AutomationElement.FromHandle(topLevelhWnd));
#if TESTBUILD_CLR40
            // Basic Test variation 3: Invoke Splash and have the app self close it... FROM A DIFFERENT THREAD!!!!
            // (Regression case for Dev10 502501)
            if (!AppWillForceCloseSplash)
            {
                passed &= DoSplashScreenAPITest("SplashTest4", true, AutomationElement.FromHandle(topLevelhWnd));
            }
            if (CheckSplashScreenTopMost)
            {
                passed &= DoSplashScreenTopMostTest(AutomationElement.FromHandle(topLevelhWnd));
            }
#endif 
            if (passed)
            {
                TestLog.Current.LogStatus("Success! Manual and automatic closing worked as expected for WPF Splash screen");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogStatus("Failed! Manual and automatic closing did not work as expected for WPF Splash screen");
                TestLog.Current.Result = TestResult.Fail;
            }
            return UIHandlerAction.Abort;
        }

#if TESTBUILD_CLR40
        private bool DoSplashScreenTopMostTest(AutomationElement testAppWindow)
        {
            AutomationElement topMostTestButton = testAppWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "SplashTest3"));
            ((InvokePattern)topMostTestButton.GetCurrentPattern(InvokePattern.Pattern)).Invoke();
            Thread.Sleep(2000);

            AutomationElement splashScreen = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isSplashScreen);
            
            IntPtr splashHwnd = IntPtr.Zero;

            if (splashScreen != null)
            {
                splashHwnd = (IntPtr)((int[])splashScreen.GetCurrentPropertyValue(AutomationElement.RuntimeIdProperty, true))[1];
            }

            IntPtr hasTopMostStyle = NativeMethods.GetWindowLong(splashHwnd, NativeConstants.GWL_EXSTYLE);
            bool hadTopMostWindowStyle = false;
            if ((((int)hasTopMostStyle) & NativeConstants.WS_EX_TOPMOST) == NativeConstants.WS_EX_TOPMOST)
            {
                GlobalLog.LogStatus("Saw SplashScreen have Hwnd = " + splashHwnd.ToString() + " and it had the WS_EX_TOPMOST WindowStyle (expected)");
                hadTopMostWindowStyle = true;
            }               
            else
            {
                GlobalLog.LogStatus("Saw SplashScreen have Hwnd = " + splashHwnd.ToString() + " and it DID NOT have the WS_EX_TOPMOST WindowStyle (failure)");
            }

            if (hadTopMostWindowStyle)
            {
                TestLog.Current.LogStatus("SUCCESS: Splash screen is topmost");
                return true;
            }
            else
            {
                TestLog.Current.LogStatus("ERROR: Splash screen is not topmost");
                return false;
            }
        }
#endif

        private bool DoSplashScreenAPITest(string testButtonAutoId, bool willCloseItself, AutomationElement testWindow)
        {            
            Helpers.InvokeElementViaAutomationId(testWindow, testButtonAutoId, 10);
            AutomationElement theSplashScreen = null;

            if (!AppWillForceCloseSplash)
            {

                theSplashScreen = Helpers.WaitForElementByAndCondition(AutomationElement.RootElement, s_isSplashScreen, 60);

                if (theSplashScreen != null)
                {
                    TestLog.Current.LogStatus("Successfully created splash screen, closing");
                }
                else
                {
                    TestLog.Current.LogStatus("Error:  Splash screen not seen 10 seconds after invocation!");
                    return false;
                }
                TestLog.Current.LogStatus("Test specifies that splash screen will " + (willCloseItself ? "" : "not ") + "self-close");
                if (willCloseItself)
                {
                    // Default fadeout time (i.e. the automatic one) is 300 ms.  Give it 600 for UIA to be updated
                    Thread.Sleep(600);
                }
                else
                {
                    // Fadeout time here is from 0 -> Max Timespan, so just wait 100 ms more than specified
                    Helpers.InvokeElementViaAutomationId(testWindow, "CloseSplash", 10);
                    Thread.Sleep(ExpectedFadeTime + 100);
                }
            }
            else
            {
                TestLog.Current.LogStatus("Test specifies app will forcibly tear down splash screen.  Simply checking it does not exist.");
                Thread.Sleep(100);
            }
            // Stability tweak... for some reason need a little more time for UIA to stabilize where we want it.
            Thread.Sleep(2000);
            theSplashScreen = null;
            theSplashScreen = Helpers.WaitForElementByAndCondition(AutomationElement.RootElement, s_isSplashScreen, 1);

            if (theSplashScreen == null)
            {
                TestLog.Current.LogStatus("Successfully dismissed splash screen, and it was not present 100 ms after expected timespan.");
            }
            else
            {
                TestLog.Current.LogStatus("Error:  Failed to dismiss Splash screen... aborting test");
                return false;
            }
            return true;
        }

        #endregion
    }

    public class SplashScreenVisualVerifier : UIHandler
    {
        #region Public Members
        public string ReferenceImageFile = "";
        public bool DriveTestApp = false;
        #endregion

        #region Private Members
        private static AndCondition s_isSplashScreen = new AndCondition(
                new PropertyCondition(AutomationElement.ClassNameProperty, "SplashScreen"),
                new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Pane));
        #endregion

        #region Handler Implementation

        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            // Only used in DriveTestApp case I know... but minimal perf implications
            AutomationElement testWindow = AutomationElement.FromHandle(topLevelhWnd);

            if (DriveTestApp)
            {                
                Helpers.InvokeElementViaAutomationId(testWindow, "SplashTest1", 10);
            }

            Thread.Sleep(500);
            AutomationElement theSplashScreen = AutomationElement.RootElement.FindFirst(TreeScope.Children, s_isSplashScreen);

            if (theSplashScreen == null)
            {
                GlobalLog.LogStatus("Error! Can't get the splash screen to validate.  Returning...");
                return UIHandlerAction.Abort;
            }
            GlobalLog.LogStatus("Found WPF Splash screen... ");

            Rect boundingRect = (Rect)theSplashScreen.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);

            GlobalLog.LogStatus("Taking image capture of Rect at: (X:" + boundingRect.X + ", Y:" + boundingRect.Y + ", Width:" + boundingRect.Width + " Height:" + boundingRect.Height + ")");
            System.Drawing.Bitmap splashScreenContent = ImageUtility.CaptureScreen(new System.Drawing.Rectangle((int)boundingRect.X, (int)boundingRect.Y, (int)boundingRect.Width, (int)boundingRect.Height));

            ImageComparator ic = new ImageComparator();
            ic.FilterLevel = 0;
            ImageAdapter capturedImage = new ImageAdapter(splashScreenContent);
            ImageAdapter referenceImage = new ImageAdapter(ReferenceImageFile);

            if (ic.Compare(capturedImage, referenceImage))
            {
                GlobalLog.LogStatus("Success! Detected splash screen using visual validation!");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogStatus("Error: Splash screen image did not match (Should have -> Comparing lossless images)");
                GlobalLog.LogStatus("Please rerun and observe test execution to ensure that WPF splash screen shows.");
                TestLog.Current.Result = TestResult.Fail;
            }

            if (DriveTestApp)
            {
                Helpers.InvokeElementViaAutomationId(testWindow, "CloseSplash", 10);
                Thread.Sleep(5000);
            }

            return UIHandlerAction.Abort;
        }

        #endregion
    }
}
