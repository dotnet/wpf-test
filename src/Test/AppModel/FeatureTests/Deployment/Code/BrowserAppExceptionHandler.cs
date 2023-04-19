// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using System.Diagnostics;
using System.Windows.Automation;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Win32;
using MTI = Microsoft.Test.Input;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Diagnostics;


/*****************************************************
 * The logic in this file is maintained by the AppModel team
 * contact: Microsoft
 *****************************************************/

namespace Microsoft.Test.Deployment
{


    /// <summary>
    /// Specifies the action to take on the Deployment Maintenance dialog
    /// </summary>
    public enum BrowserAppExceptionHandlerAction
    {
        /// <summary>
        /// Log the complete contents of the text in the frame (on the browser app fail page) and fail in the current testlog if defined
        /// </summary>
        LogAndFailIfSeen,
        /// <summary>
        /// Simply log (and allow Appmonitor to continue) when this is seen.  Useful if there can be more than one.  Test will still usually fail due to timeout.
        /// </summary>
        LogIfSeen,
        /// <summary>
        /// Pass if a specific exception string is seen.  May need to modify this for localized strings if that becomes the case (not too hard)
        /// </summary>
        PassOnSpecificException
    }

    /// <summary>
    /// Handles the Maintenance/Uninstall dialog
    /// </summary>
    public class BrowserAppExceptionHandler : UIHandler
    {
        public BrowserAppExceptionHandlerAction Action
        {
            set
            {
                s_actionToTake = value;
            }
            get
            {
                return s_actionToTake;
            }
        }
        public string ExceptionString
        {
            set
            {
                s_exceptionString = value;
            }
            get
            {
                return s_exceptionString;
            }
        }

        /// <summary>
        /// Action to take when handling the window.  Can log, log and fail, or look for a particular exception.
        /// </summary>
        private static BrowserAppExceptionHandlerAction s_actionToTake = BrowserAppExceptionHandlerAction.LogIfSeen;

        /// <summary>
        /// String to look for if Action is "PassOnSpecificException"
        /// </summary>
        private static String s_exceptionString = "";

        private static UIHandlerAction s_result = UIHandlerAction.Abort;
        /// <summary>
        /// Handles Browser Application error messages.  Does work on STA thread to avoid UIAutomation bugs.
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

            return s_result;
        }


        /// <summary>
        /// Handles Browser Application error messages
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        private static void handleWindowNewThread(object theWindow)
        {
            GlobalLog.LogEvidence("Entering Browser error page handler");

            AutomationElement IEWindow = (AutomationElement)theWindow;

            AutomationElement moreInfoButton = IEWindow.FindFirst(TreeScope.Subtree,
                                                new PropertyCondition(AutomationElement.NameProperty, ApplicationDeploymentHelper.ErrorPageMoreInfo));
            if (moreInfoButton == null)
            {
                GlobalLog.LogDebug("Error page check for " + ApplicationDeploymentHelper.ErrorPageMoreInfo + " failed, moving to hard coded english version... ");
                 
                moreInfoButton = IEWindow.FindFirst(TreeScope.Subtree, new PropertyCondition(AutomationElement.NameProperty, "More Information"));

                // Last-ditch effort: if the registry key is set, the error details will be shown automatically.
                // Then the button will read "Less Information".
                if (moreInfoButton == null)
                {
                    GlobalLog.LogDebug("Error page check for hard coded english version failed, moving to 'Less Information' version... ");
                    moreInfoButton = IEWindow.FindFirst(TreeScope.Subtree,
                        new PropertyCondition(AutomationElement.NameProperty, ApplicationDeploymentHelper.ErrorPageLessInfo));
                }
            }
            if (moreInfoButton == null)
            {
                GlobalLog.LogEvidence("Could not find the More Information button.... ");
                s_result = UIHandlerAction.Unhandled;
            }
            else
            {
                switch (ApplicationDeploymentHelper.GetIEVersion())
                {
                    case 6:
                        InvokePattern ip = moreInfoButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        ip.Invoke();
                        break;
                    case 7:
                        System.Windows.Rect r = (System.Windows.Rect)moreInfoButton.GetCurrentPropertyValue(AutomationElement.BoundingRectangleProperty);
                        double horizMidPt = r.Left + ((r.Right - r.Left) / 2.0);
                        double vertMidPt = r.Top + ((r.Bottom - r.Top) / 2.0);
                        MTI.Input.MoveTo(new System.Windows.Point(horizMidPt, vertMidPt));
                        MTI.Input.SendMouseInput(horizMidPt, vertMidPt, 0, MTI.SendMouseInputFlags.Absolute | MTI.SendMouseInputFlags.LeftDown | MTI.SendMouseInputFlags.Move);
                        MTI.Input.SendMouseInput(horizMidPt, vertMidPt, 0, MTI.SendMouseInputFlags.Absolute | MTI.SendMouseInputFlags.LeftUp | MTI.SendMouseInputFlags.Move);
                        break;
                    // IE 8 and 9 are the same as 7 for now...
                    default:
                        goto case 7;
                }
                // Sleep a couple seconds to let the name on the button change...
                Thread.Sleep(2000);

                if (moreInfoButton.Current.Name != ApplicationDeploymentHelper.ErrorPageLessInfo)
                {
                    GlobalLog.LogEvidence("The More Information button did not toggle: \n");
                    GlobalLog.LogEvidence("Value: " + moreInfoButton.Current.Name + " Expected: " + ApplicationDeploymentHelper.ErrorPageMoreInfo);
                    s_result = UIHandlerAction.Unhandled;
                }
                else
                {
                    GlobalLog.LogDebug("Successfully expanded browser app error page");
                }
            }

            String errorMessage;
            AutomationElement errorDetailsElem = Microsoft.Test.Loaders.UIHandlers.BrowserAppExceptionLogger.GetErrorDetails(moreInfoButton);
            if (errorDetailsElem != null)
            {
                ValuePattern vp = (ValuePattern)errorDetailsElem.GetCurrentPattern(ValuePattern.Pattern);
                errorMessage = vp.Current.Value;
            }
            else
            {
                GlobalLog.LogEvidence("Could not find error details element.\n");
                errorMessage = "[[Could not find error details element]]";
            }

            switch (s_actionToTake)
            {
                case BrowserAppExceptionHandlerAction.LogAndFailIfSeen:
                    {
                        GlobalLog.LogEvidence("Logging (and setting result to fail) Browser Application Error... ");
                        GlobalLog.LogDebug(errorMessage);

                        TestLog tl = TestLog.Current;
                        if (tl == null)
                        {
                            throw new InvalidOperationException("Log and Fail if seen must be run in the context of a test log!");
                        }

                        tl.Result = TestResult.Fail;
                        GlobalLog.LogEvidence(" BEGIN Error Log: \n");
                        GlobalLog.LogEvidence(errorMessage);
                        GlobalLog.LogEvidence("\n END Error Log: \n");
                        GlobalLog.LogEvidence("Set result to fail and aborted.... ");
                        s_result = UIHandlerAction.Abort;
                        break;
                    }
                case BrowserAppExceptionHandlerAction.LogIfSeen:
                    {
                        GlobalLog.LogEvidence("Logging (and NOT setting result to fail) Browser Application Error... ");
                        GlobalLog.LogEvidence(" BEGIN Error Log: \n");
                        GlobalLog.LogEvidence(errorMessage);
                        GlobalLog.LogEvidence("\n END Error Log: \n");
                        GlobalLog.LogEvidence("Did not set result to fail ... ");
                        break;
                    }
                case BrowserAppExceptionHandlerAction.PassOnSpecificException:
                    {
                        GlobalLog.LogEvidence("Examining Browser App Error page for exception string \"" + s_exceptionString + "\"");
                        TestLog tl = TestLog.Current;
                        if (tl == null)
                        {
                            throw new InvalidOperationException("\"Pass on Specific Exception\" must be run in the context of a test log!");
                        }

                        if (errorMessage.Contains(s_exceptionString))
                        {
                            tl.Result = TestResult.Pass;
                            GlobalLog.LogEvidence("Found \"" + s_exceptionString + "\" in the Browser App exception page... test passes");
                        }
                        else
                        {
                            GlobalLog.LogEvidence("Could not find " + s_exceptionString + " in the Browser App exception page... logging error page for analysis.");
                            GlobalLog.LogEvidence(" BEGIN Error Log: \n");
                            GlobalLog.LogEvidence(errorMessage);
                            GlobalLog.LogEvidence("\n END Error Log: \n");
                        }
                        s_result = UIHandlerAction.Abort;
                        break;
                    }
                default:
                    s_result = UIHandlerAction.Handled;
                    break;
            }
        }
    }

    public class BrowserAppExceptionRestartHandler : UIHandler
    {
        public override UIHandlerAction HandleWindow(IntPtr topHwnd, IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            LoaderStep fileHostStep = this.Step;

            while ((fileHostStep != null) && (fileHostStep.GetType() != typeof(FileHostStep)))
            {
                fileHostStep = fileHostStep.ParentStep;
            }

            if (fileHostStep.GetType() != typeof(FileHostStep))
            {
                GlobalLog.LogEvidence("Couldnt find parent FileHostStep to get nav URIs from! Quitting out...");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }

            AutomationElement browserWindow = AutomationElement.FromHandle(topHwnd);
            browserWindow.SetFocus();

            Uri xbapPath = ((FileHostStep)fileHostStep).fileHost.GetUri("SimpleBrowserHostedApplication.xbap", FileHostUriScheme.HttpInternet);

            FireFoxAutomationHelper.NavigateFireFox(topHwnd, xbapPath.ToString());

            GlobalLog.LogEvidence("Navigated FF to " + xbapPath.ToString());

            // Crash app
            IEAutomationHelper.WaitForElementWithAutomationId(browserWindow, "XbapRelXaml", 30);
            IEAutomationHelper.ClickCenterOfElementById(browserWindow, "XbapRelXaml");

            GlobalLog.LogEvidence("Clicked intentionally bad link... exception page should come up... ");

            Thread.Sleep(1000);

            AndCondition isRestartLink = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, ApplicationDeploymentHelper.ErrorPageRestartApp), new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Hyperlink));
            IEAutomationHelper.WaitForElementByAndCondition(browserWindow, isRestartLink, 15);


            GlobalLog.LogEvidence("Error page detected, now navigating back and forward to restart the app." );

            // Successfully hit the error page.  Now go back and forward, making sure app gets relaunched...
            FireFoxAutomationHelper.ClickFireFoxBackButton(topHwnd);
            IEAutomationHelper.WaitForElementWithAutomationId(browserWindow, "refreshTestTxtBox", 10);
            FireFoxAutomationHelper.ClickFireFoxForwardButton(topHwnd);

            // Crash app again
            AutomationElement relaunchedAppElement = IEAutomationHelper.WaitForElementWithAutomationId(browserWindow, "XbapRelXaml", 30);
            if (relaunchedAppElement == null)
            {
                GlobalLog.LogEvidence("ERROR! Couldnt find the relaunched .xbap!");
                TestLog.Current.Result = TestResult.Fail;
                return UIHandlerAction.Abort;
            }
            GlobalLog.LogEvidence("Xbap relaunched successfully.  Now to crash again and restart with \"Restart\" button...");

            IEAutomationHelper.ClickCenterOfElementById(browserWindow, "XbapRelXaml");

            Thread.Sleep(1000);
            AutomationElement RestarterLink = IEAutomationHelper.WaitForElementByAndCondition(browserWindow, isRestartLink, 15);

            if (RestarterLink== null)
            {
                GlobalLog.LogEvidence("Success! Back-->Forward Restart for xbap relaunching worked as expected! (Not verifying restart button since UIA is broken here)");
                TestLog.Current.Result = TestResult.Pass;
                return UIHandlerAction.Abort;
            }

            IEAutomationHelper.ClickHTMLHyperlinkByName(browserWindow, ApplicationDeploymentHelper.ErrorPageRestartApp);


            relaunchedAppElement = IEAutomationHelper.WaitForElementWithAutomationId(browserWindow, "XbapRelXaml", 30);
            if (relaunchedAppElement == null)
            {
                GlobalLog.LogEvidence("ERROR! Couldnt find the relaunched .xbap!");
                TestLog.Current.Result = TestResult.Fail;
                return UIHandlerAction.Abort;
            }
            else
            {
                GlobalLog.LogEvidence("Success! Back-->Forward and \"Restart\" button xbap relaunching worked as expected!");
                TestLog.Current.Result = TestResult.Pass;
                return UIHandlerAction.Abort;
            }



        }
    }
}
