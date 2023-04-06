// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Diagnostics;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Deployment;
using System.Windows.Automation;
using Microsoft.Test.Loaders.Steps;
using System.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Loader Step that is used to activate the browser
    /// </summary>
    public class HyperlinkTargetingHandlerBrowser : Microsoft.Test.Loaders.UIHandler
    {
        private static String s_text = "Hyperlink Targeting"; // text element on the page

        /// <summary>
        /// Navigate to hyperlinks with different TargetNames
        /// This handler uses the content in file HlinkTargeting_Loose.xaml
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topHwnd, System.IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            Log.Current.CurrentVariation.LogMessage("Starting HyperlinkTargetingOutOfProcBrowser test ... ");
            AutomationElement IEWindow = AutomationElement.FromHandle(topHwnd);
            IEWindow.SetFocus();

            ParameterizedThreadStart workerThread = new ParameterizedThreadStart(HandleWindowNewThread);
            Thread thread = new Thread(workerThread);
            thread.SetApartmentState(ApartmentState.STA);

            thread.Start((object) IEWindow);

            thread.Join();

            return UIHandlerAction.Abort;
        }

        private static void HandleWindowNewThread(object theWindow)
        {
            Log.Current.CurrentVariation.LogMessage("In HandleWindowNewThread");
            AutomationElement IEWindow = (AutomationElement)theWindow;

            if (Log.Current == null)
            {
                Log.Current.CurrentVariation.LogMessage("Couldn't retrieve Log.Current");
                throw new ApplicationException("Couldn't retrieve Log.Current");
            }

            Log.Current.CurrentVariation.LogMessage("Waiting for element Hyperlink Targeting");
            if (IEAutomationHelper.WaitForElementWithName(IEWindow, s_text, 30) == null)
            {
                Log.Current.CurrentVariation.LogMessage("Did not find the element : " + s_text);
                NavigationHelper.CacheTestResult(Result.Fail);
                return;
            }

            // look for the first hyperlink and ignore the test if it cannot be found
            if (IEAutomationHelper.FindHTMLHyperlinkByName(IEWindow, "DoClickNoTargetNameXaml") == null)
            {
                Log.Current.CurrentVariation.LogMessage("Whoops, hit UI Automation bug with downlevel platforms + IE (WOSB 1883785 or 1985683, both not likely to ever be fixed) .  Setting result to Ignore and returning, since failure occurred before WPF even loaded.");
                NavigationHelper.CacheTestResult(Result.Ignore);
                return;
            }

            // Click on the given hyperlink and verify the navigated page
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickNoTargetNameXaml", "FramePage_Embedded", false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickParentXaml", "FramePage_Embedded", false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickSelfXaml", "FramePage_Embedded", false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickTopXaml", "FramePage_Embedded", false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickNoTargetNameHtml", "HTML Page", false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickParentHtml", "HTML Page", false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickSelfHtml", "HTML Page", false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickTopHtml", "HTML Page", false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickBlankXaml", "FramePage_Embedded", true);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "DoClickBlankHtml", "HTML Page", true);

            // A little hacky... we need to set SOME result here but the encompassing TestLogStep is going
            // to want to set a result and close the log, so just use the old API for now.  Eventually, we
            // could fully remove the legacy API wrappers.
            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Click on the given hyperlink and verify the navigated page
        /// </summary>
        /// <param name="IEWindow">IE window</param>
        /// <param name="hyperlink">Name of the hyperlink</param>
        /// <param name="element">Element on the navigated page</param>
        private static void ClickHyperlinkAndVerifyNavigation(AutomationElement IEWindow,
            String hyperlink, String element, bool clickOnly)
        {
            Log.Current.CurrentVariation.LogMessage("Clicking on hyperlink " + hyperlink);

            try
            {
                // we already verified hyperlink should be found, else let the exception to be thrown
                IEAutomationHelper.ClickCenterOfElementByName(IEWindow, hyperlink, 30);

                if (clickOnly == true)
                {
                    Thread.Sleep(4000);
                    Process[] IEInstances = Process.GetProcessesByName("iexplore");
                    AutomationElement newIEWindow = null;
                    // you will see only one process for iexplore uptill ie7. For Ie8, you will see 2 procs
                    // Ignore the 2nd Process (MainWindowHandle == IntPtr.Zero) because it has no Window or Window Handle
                    GlobalLog.LogEvidence(string.Format("Verifying that \"{0}\" was launched in new Ie window", hyperlink));
                    bool newIEWindowNavigatedCorrectly = false;
                    for (int i = 0; i < IEInstances.Length; i++)
                    {
                        // Ignore IE8 instance that does not have WindowHandle
                        if (IEInstances[i].MainWindowHandle == IntPtr.Zero) { continue; }

                        newIEWindow = AutomationElement.FromHandle(IEInstances[i].MainWindowHandle);
                        GlobalLog.LogEvidence(newIEWindow.Current.Name);
                        
                        if (IEWindow != newIEWindow)
                        {
                            // newIEWindow should be different because it is a pop-up and active
                            newIEWindowNavigatedCorrectly = true; 
                            GlobalLog.LogEvidence("Found new IE window");
                        }
                        else
                        {
                            GlobalLog.LogEvidence("Did not find a new IE Window as expected");
                            break;
                        }
                    }
                    if (newIEWindowNavigatedCorrectly)
                    {
                        NavigationHelper.CacheTestResult(Result.Pass);
                    }
                    else
                    {
                        NavigationHelper.CacheTestResult(Result.Fail);
                    }
                    // give the focus back to the old IE window
                    IEWindow.SetFocus();
                }
                else
                {
                    // verify the page we navigated after the click
                    Log.Current.CurrentVariation.LogMessage("Verify the element " + element);
                    if (IEAutomationHelper.WaitForElementWithName(IEWindow, element, 30) == null)
                    {
                        Log.Current.CurrentVariation.LogMessage("Did not find the element after navigation : " + element);
                        NavigationHelper.CacheTestResult(Result.Fail);
                        return;
                    }
                    else
                    {
                        Log.Current.CurrentVariation.LogMessage("Found the element " + element);
                    }

                    // go back to the previous page
                    Log.Current.CurrentVariation.LogMessage("Go back on the browser");
                    IEAutomationHelper.ClickIEBackButton(IEWindow);

                    if (IEAutomationHelper.WaitForElementWithName(IEWindow, s_text, 30) == null)
                    {
                        Log.Current.CurrentVariation.LogMessage("Did not find the element : " + s_text);
                        NavigationHelper.CacheTestResult(Result.Fail);
                    }
                }
            }
            catch (Exception e)
            {
                GlobalLog.LogDebug(string.Format("Unexpected Exception: {0}", e.ToString())); 
            }
        }
    }

}
