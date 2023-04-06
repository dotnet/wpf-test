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
using System.Windows.Input;
using Microsoft.Test.Loaders.Steps;
using System.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Loader Step that is used to activate the browser
    /// </summary>
    public class HyperlinkNavigationHandlerBrowser : Microsoft.Test.Loaders.UIHandler
    {
        private static String s_text = "HyperLinks"; // text element on the page

        /// <summary>
        /// Click on hyperlinks and verify the navigation
        ///     - Navigate to fragment on a frame
        ///     - Navigate to fragment with Uri notation pack://siteoforigin:,,,/
        ///     - Navigate to a frame
        ///     - Click on hyperlinks in a frame and navigate
        ///          a) Navigate to another XAML file
        ///          b) Navigate to http://www.microsoft.com without TargetName
        ///          c) Navigate to another XAML file with TargetName=_blank
        /// This handler uses the content in file HlinkNavigation_Page1_Loose.xaml
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

            if (ApplicationDeploymentHelper.GetIEVersion() >= 9)
            {            
                Log.Current.CurrentVariation.LogMessage("Journal issue with IE9+, Ignore'ing test");
                NavigationHelper.CacheTestResult(Result.Ignore);
                return UIHandlerAction.Abort;
            }

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

            Log.Current.CurrentVariation.LogMessage("Waiting for element HyperLinks");
            if (IEAutomationHelper.WaitForElementWithName(IEWindow, s_text, 30) == null)
            {
                Log.Current.CurrentVariation.LogMessage("Did not find the element : " + s_text);
                NavigationHelper.CacheTestResult(Result.Fail);
                return;
            }

            // look for the first hyperlink and ignore the test if it cannot be found
            if (IEAutomationHelper.FindHTMLHyperlinkByName(IEWindow, "HlinkNavigation_Frame1_fragment2") == null)
            {
                Log.Current.CurrentVariation.LogMessage("Whoops, hit UI Automation bug with downlevel platforms + IE (WOSB 1883785 or 1985683, both not likely to ever be fixed) .  Setting result to Ignore and returning, since failure occurred before WPF even loaded.");
                NavigationHelper.CacheTestResult(Result.Ignore);
                return;
            }

            // Click on the given hyperlink and verify the navigated page
            ClickHyperlinkAndVerifyNavigation(IEWindow, "HlinkNavigation_Frame1_fragment2", s_text, false, false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "HlinkNavigation_Frame1_fragment1", s_text, false, false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "HlinkNavigation_Frame2_fragment5", s_text, false, false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "HlinkNavigation_Frame2_fragment4", s_text, false, false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "HlinkNavigation_hlinkFrame1", s_text, false, false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "HlinkNavigation_hlinkFrame2", s_text, false, false);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "HlinkNavigation_Page1", s_text, false, true);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "HlinkNavigation_Page2", s_text, false, true);
            ClickHyperlinkAndVerifyNavigation(IEWindow, "HlinkNavigation_TargetBlank", s_text, true, false);

            // when you click on a link without TargetName and Uri is outside of SOO
            // security exception will be thrown
            IEAutomationHelper.ClickCenterOfElementByName(IEWindow, "HlinkNavigation_NotargetName", 30);

            // verify the error page
            VerifyErrorPage(IEWindow);
        }

        /// <summary>
        /// Click on the given hyperlink and verify the navigated page
        /// </summary>
        /// <param name="IEWindow">IE window</param>
        /// <param name="hyperlink">Name of the hyperlink</param>
        /// <param name="element">Element on the navigated page</param>
        private static void ClickHyperlinkAndVerifyNavigation(AutomationElement IEWindow, 
            String hyperlink, String element, bool clickOnly, bool goBack)
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
                    Log.Current.CurrentVariation.LogMessage(string.Format("Verifying that \"{0}\" was launched in new Ie window", hyperlink));
                    bool newIEWindowNavigatedCorrectly = false;
                    for (int i = 0; i < IEInstances.Length; i++)
                    {
                        // Ignore IE8 instance that does not have WindowHandle 
                        if (IEInstances[i].MainWindowHandle == IntPtr.Zero) { continue; }

                        newIEWindow = AutomationElement.FromHandle(IEInstances[i].MainWindowHandle);
                        Log.Current.CurrentVariation.LogMessage(newIEWindow.Current.Name);
                        // Ignore this handle if it belongs to the parent IE instance 
                        if (IEWindow == newIEWindow) { continue; }

                        Log.Current.CurrentVariation.LogMessage("Verify that \"" + newIEWindow.Current.Name + "\" navigated to HlinkNavigation_Page4_Loose.xaml");
                        if (null == IEAutomationHelper.WaitForElementWithAutomationId(newIEWindow, "HlinkNavigation_Page4_Loose_TextBox", 30))
                        {
                            Log.Current.CurrentVariation.LogMessage("Did not find known element \"HlinkNavigation_Page4_Loose_TextBox\" on\" " + newIEWindow.Current.Name + "\" ");
                        }
                        else 
                        {
                            Log.Current.CurrentVariation.LogMessage("Found known element \"HlinkNavigation_Page4_Loose_TextBox\" on\" " + newIEWindow.Current.Name + "\" ");
                            newIEWindowNavigatedCorrectly = true;
                            break;
                        }
                    }
                    NavigationHelper.CacheTestResult(newIEWindowNavigatedCorrectly ? Result.Pass : Result.Fail);

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

                    if (goBack == true)
                    {
                        Log.Current.CurrentVariation.LogMessage("Go back on the browser");

                        if (ApplicationDeploymentHelper.GetIEVersion() > 6)
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

                            Thread.Sleep(1000);
                        }

                        if (IEAutomationHelper.WaitForElementWithName(IEWindow, s_text, 30) == null)
                        {
                            Log.Current.CurrentVariation.LogMessage("Did not find the element : " + s_text);
                            NavigationHelper.CacheTestResult(Result.Fail);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                /// Test infrastructure is not able to log all exceptions. No point fixing it 
                /// one month before v-Next. Am adding this exception handler to log the exception
                GlobalLog.LogDebug(e.ToString());
                throw e; 
            }
        }

        /// <summary>
        /// Verify we navigated to the error page
        /// </summary>
        /// <param name="IEWindow"></param>
        private static void VerifyErrorPage(AutomationElement IEWindow)
        {
            AutomationElement moreInfoButton = IEWindow.FindFirst(TreeScope.Subtree,
                                                new PropertyCondition(AutomationElement.NameProperty, ApplicationDeploymentHelper.ErrorPageMoreInfo));

            // If the registry key is set, the error details will be shown automatically. 
            // Then the button will read "Less Information".
            if (moreInfoButton == null)
            {
                moreInfoButton = IEWindow.FindFirst(TreeScope.Subtree,
                    new PropertyCondition(AutomationElement.NameProperty, ApplicationDeploymentHelper.ErrorPageLessInfo));
                if (moreInfoButton == null)
                {
                    Log.Current.CurrentVariation.LogMessage("Could not find the More Information button.... ");
                    NavigationHelper.CacheTestResult(Result.Fail);
                }
                else
                {
                    Log.Current.CurrentVariation.LogMessage("Found the More Information button.... ");
                    NavigationHelper.CacheTestResult(Result.Pass);
                }
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Found the More Information button.... ");
                NavigationHelper.CacheTestResult(Result.Pass);
            }
        }
    }

}
