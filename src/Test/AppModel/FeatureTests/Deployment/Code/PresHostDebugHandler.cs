// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Loaders.Steps;
using System.Windows.Automation;
using System.Diagnostics;
using Microsoft.Test.Deployment;
using Microsoft.Test.Diagnostics;

namespace Microsoft.Windows.Test.Client.AppSec.Deployment.CustomUIHandlers
{
    /// <summary>
    /// UI handler
    /// </summary>
    public class PresHostDebugHandler : UIHandler
    {
        /// <summary>
        /// constructor
        /// </summary>
        public PresHostDebugHandler()
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
            if ((Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null) &&
                (Environment.OSVersion.Version.Major < 6) && // If we're running on a 64-bit pre-vista OS,
                (this.Step.GetType() == typeof(ActivationStep)) &&  // And our parent step is an Activation
                (((ActivationStep)this.Step).PresentationHostDebugMode) && // And it's launching an xbap in debug mode
                (IEAutomationHelper.Is64BitProcess(Process.GetCurrentProcess().Id))) // and we're a 64-bit process, then
            {
                // Abort the test, since this scenario is invalid (DevEnv.exe is a 32-bit process, 
                // so will not launch 64-bit PH debug mode on a system that will start 32-bit PH
                GlobalLog.LogEvidence("Setting result to ignore and returning... cannot perform PresentationHost.exe debug mode tests on 64-bit Pre-vista OS");
                TestLog.Current.Result = TestResult.Ignore;
                return UIHandlerAction.Abort;
            }

            // We just need to launch a different app here... if it succeeds in deploying it'll invoke a different UIHandler.
            TestLog log = TestLog.Current;

            IEAutomationHelper.WaitForElementWithAutomationId(topHwnd, "btnSecurityTester", 60);

            Process.Start("iexplore.exe", Directory.GetCurrentDirectory() + "\\different\\SimpleBrowserHostedApplication.xbap");

            return UIHandlerAction.Handled;
        }
    }

    public class PresHostDebugSOOUrlStep : LoaderStep
    {
        /// <summary>
        /// Whether to use the -debugsecurityzoneurl flag.  If false, we expect failure and this will be handled by the generic
        /// browser error logger
        /// </summary>
        public bool UseDebugSOOFlag = true;

        /// <summary>
        /// Make sure that the Site Of Origin can be spoofed with the -debugsecurityzoneurl flag
        /// </summary>
        /// <returns>true</returns>
        public override bool DoStep()
        {
            string currDir = Environment.CurrentDirectory;

            // Workaround for DD bugs #129532
            if ((SystemInformation.Current.IsServer) && (SystemInformation.Current.OSVersion.StartsWith("6")))
            {
                ApplicationDeploymentHelper.AddUrlToZone(IEUrlZone.URLZONE_TRUSTED, "about:security_PresentationHost.exe");
            }

            GlobalLog.LogDebug("Current Dir: " + currDir);
            ProcessStartInfo psi;
            string argsToUse = "";
            string SOODir = currDir + "\\SimpleBrowserHostedApplication.xbap";
            currDir = currDir + "\\subdir";

            // Positive + Negative tests.  First branch = Spoof the site of origin.  Second = Don't spoof.
            if (UseDebugSOOFlag)
            {
                GlobalLog.LogDebug("SOO Spoofing Dir: " + SOODir);
                argsToUse = "-debugsecurityzoneurl \"" + SOODir + "\" -debug \"" + currDir + "\\SimpleBrowserHostedApplication.xbap\"";
            }
            else
            {
                argsToUse = " -debug \"" + currDir + "\\SimpleBrowserHostedApplication.xbap\"";
            }

            GlobalLog.LogDebug("Starting " + IEAutomationHelper.PresentationHostExePath + " " + argsToUse);
            psi = new ProcessStartInfo(IEAutomationHelper.PresentationHostExePath, argsToUse);

            psi.UseShellExecute = false;

            Process PHost = Process.Start(psi);

            // Start a monitor thread to handle trust manager window
            Thread handleDialogThread = new Thread(new ThreadStart(TrustManagerHandler));
            handleDialogThread.Start();

            Process[] IEInstances = Process.GetProcessesByName("iexplore");

            while (IEInstances.Length == 0)
            {
                Thread.Sleep(4000);
                IEInstances = Process.GetProcessesByName("iexplore");
            }
            AutomationElement IEWindow = null;

            //// Workaround:  In IE8, only one IE process has an automation element tree.  Solution: Get them all and try til you find a good one.
            if (ApplicationDeploymentHelper.GetIEVersion() >= 8)
            {
                int index = 0;
                bool foundAUsableWindow = false;

                while ((index < IEInstances.Length) && !foundAUsableWindow)
                {
                    try
                    {
                        IEWindow = AutomationElement.FromHandle(IEInstances[index].MainWindowHandle);
                        // Needs to have a viable Hwnd and also have an xbap in it... 
                        if (IEAutomationHelper.WaitForElementByPropertyCondition(IEWindow, new PropertyCondition(AutomationElement.ClassNameProperty, "RootBrowserWindow"), 30) != null)
                        {
                            foundAUsableWindow = true;
                        }
                        else
                        {
                            index++;
                        }
                    }
                    catch
                    {
                        index++;
                    }
                }
            }
            else
            {
                IEWindow = AutomationElement.FromHandle(IEInstances[0].MainWindowHandle);
            }
            AutomationElement actUriElement = IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "actUriTxt", 20);

            if (actUriElement == null)
            {
                actUriElement = IEAutomationHelper.WaitForElementWithAutomationId(AutomationElement.RootElement, "actUriTxt", 20);
            }

            string detectedActivationUri = actUriElement.Current.Name;
            bool ActUriCheckPassed = true;

            IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "sooDebugTestBtn", 15);
            IEAutomationHelper.InvokeElementViaAutomationId(IEWindow, "sooDebugTestBtn", 15);
            GlobalLog.LogEvidence("Invoked SOO content button ... ");

            if (UseDebugSOOFlag)
            {
#if TESTBUILD_CLR40
                if (!detectedActivationUri.ToLowerInvariant().EndsWith("subdir/simplebrowserhostedapplication.xbap"))
                {
                    GlobalLog.LogEvidence("BrowserInteropHelper.Source was set to -debugSecurityZoneUrlValue! (v4 only)");
                }
                else
                {
                    GlobalLog.LogEvidence("Error: BrowserInteropHelper.Source was not set to -debugSecurityZoneUrlValue! (v4 only)");
                    ActUriCheckPassed = false;
                }
#endif
                if (ActUriCheckPassed && (IEAutomationHelper.WaitForElementWithAutomationId(IEWindow, "xamlTestButton", 45) != null))
                {
                    GlobalLog.LogEvidence("Passed: Site of Origin with -debugSecurityZoneUrl test");
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Error: May not have been able to load SOO content using -debugSecurityZoneUrl");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }    // Otherwise just wait for the browser exception page and fail if we see that       
            else // Just sleep for a bit and let the other UIHandler fail this test if it will.
            {
                GlobalLog.LogEvidence("Waiting for error page ...");
                Thread.Sleep(5000);

#if TESTBUILD_CLR40
                if (detectedActivationUri.ToLowerInvariant().EndsWith("subdir/simplebrowserhostedapplication.xbap"))
                {
                    GlobalLog.LogEvidence("Success: BrowserInteropHelper.Source was expected value. (v4 only)");
                }
                else
                {
                    GlobalLog.LogEvidence("Error: BrowserInteropHelper.Source was not set to expected value! (v4 only)");
                    ActUriCheckPassed = false;
                }
#endif
                if (ActUriCheckPassed && (IEAutomationHelper.WaitForElementWithName(IEWindow, ApplicationDeploymentHelper.ErrorPageMoreInfo, 15) != null))
                {
                    GlobalLog.LogEvidence("Passed: Site of Origin with -debugSecurityZoneUrl test (Negative) ");
                    TestLog.Current.Result = TestResult.Pass;
                }
                // Workaround for incorrect localization issue on certain languages.
                // There's no need for this test to fail owing to incorrect localization, so check for the english version after the correct one fails.
                else if (ActUriCheckPassed && (IEAutomationHelper.WaitForElementWithName(IEWindow, "More Information", 15) != null))
                {
                    GlobalLog.LogEvidence("Passed: Site of Origin with -debugSecurityZoneUrl test (Negative) ... but had to fall back to the English \"More Information\" button. ");
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Error: Didn't see error page for non-SOO content");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }

            foreach (Process p in Process.GetProcessesByName("iexplore"))
            {
                p.Kill();
            }

            // Dispose the monitor thread until vairation finished.
            handleDialogThread.Abort();

            Thread.Sleep(4000);

            return true;
        }


        /// <summary>
        /// Workaround: Can't get the AutomationElement via UIAutomation from the SecurityDialog
        ///             that is poped up after starting an xbap with -debug/-debugsecurityzoneurl mode.
        ///             Not ensure that accesskey worked correctly on non-en OS.
        /// Solution: Hitting the accsee key of the "Run" button instead. 
        ///           Mointoring the PresentationHost until the call thread abort.
        ///           Obtain the mainwindow of PrestentationHost,Focus on it, bring it to forground and Send the accessKey.
        /// </summary>
        public static void TrustManagerHandler()
        {
            Process[] PHInstances = null;
            AutomationElement PHWindow = null;
            int index = 0;
            while (true)
            {
                PHInstances = Process.GetProcessesByName("PresentationHost");
                while (PHInstances.Length == 0)
                {
                    Thread.Sleep(4000);
                    PHInstances = Process.GetProcessesByName("PresentationHost");
                }
                while (index < PHInstances.Length)
                {
                    GlobalLog.LogEvidence("PresenttationHost.exe process Found!\n");
                    try
                    {
                        GlobalLog.LogEvidence("Try to get the main handle of security dialog from PresentationHost process \n");
                        PHWindow = AutomationElement.FromHandle(PHInstances[index].MainWindowHandle);

                        if (PHWindow != null)
                        {
                            GlobalLog.LogEvidence("Security Dialog found!\n Try to handle the Security Dialog\n");
                            IEAutomationHelper.HandleSecDialogByAccessKey(PHInstances[index].MainWindowHandle);
                        }
                        else
                        {
                            index++;
                        }
                    }
                    catch
                    {
                        index++;
                    }
                }
                Thread.Sleep(1000);
            }
        }
    }
}
