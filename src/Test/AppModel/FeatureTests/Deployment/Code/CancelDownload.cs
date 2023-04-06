// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Resources;
using System.Windows;
using System.Diagnostics;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using System.Windows.Automation;
using UIA=System.Windows.Automation;
using System.Threading; using System.Windows.Threading; 
using System.IO;
using System.Windows.Input;
using Microsoft.Test.Loaders.Steps;
using MTI = Microsoft.Test.Input;
using System.Runtime.InteropServices;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Method to use for canceling app...
    /// </summary>
    public enum CancelType
    {
        /// <summary>
        /// Cancel by sending an "Esc" to the window (IE Accelerator key for "stop")
        /// </summary>
        EscapeKey,
        /// <summary>
        /// Cancel by clicking either "Stop" or the UI Cancel button
        /// </summary>
        Regular,
        /// <summary>
        /// Cancel by closing the browser window
        /// </summary>
        CloseBrowser,
        /// <summary>
        /// Cancel by navigating away to some other site.
        /// </summary>
        NavigateAway
    }


    internal class CancelDownload : UIHandler
    {
        #region Private Data
        string _appName = null;
        bool _notHandledYet = true;
        protected TestLog TestLog;
        bool _inBrowserProgress = false;
        bool _restartApp = false;
        bool _useUICancelButton = false;
        #endregion  

        #region Public Members
        /// <summary>
        /// Name of process to be monitored. Controls checking of Shortcut (for Shell-visible),
        /// name of process (for StandAlone apps), and title of Window if ExpectedWindowTitle
        /// is not provided
        /// </summary>
        public virtual string AppName
        {
            get { return _appName; }
            set { _appName = value; }
        }

        /// <summary>
        /// Determines how CancelDownload will attempt to click cancel to stop the download.
        /// If false - tries to click on the standalone "(%)Installing Company Application of..."
        /// If true - handles the in-browser dialog by either clicking on "Stop" in the browser
        /// or on the UI of the progress bar.
        /// </summary>
        public virtual bool InBrowserProgress
        {
            get { return _inBrowserProgress; }
            set { _inBrowserProgress = value; }
        }

        /// <summary>
        /// Determines if App will be restarted after being canceled. Default is False.
        /// </summary>
        public virtual bool RestartApp
        {
            get { return _restartApp; }
            set { _restartApp = value; }
        }

        /// <summary>
        /// Method to use when canceling the download of the app.
        /// Default: IE Stop Button / Progress UI Cancel Button (Depends on UseUICancelButton)
        /// </summary>
        public CancelType CancelMethod = CancelType.Regular;

        /// <summary>
        /// Determines whether the Download details checkbox should be toggled before cancelling download of the app.
        /// </summary>
        public bool ExpandProgressUIDetails = false;

        /// <summary>
        /// Determines if App will be Cancelled using the browser stop button or the cancel button contained in UI.
        /// Only affects behavior if InBrowserProgress = true.  Default: False
        /// </summary>
        public virtual bool UseUICancelButton
        {
            get { return _useUICancelButton; }
            set { _useUICancelButton = value;}
        }

        #endregion

        #region Constructor
        public CancelDownload()
		{


        }
        #endregion

        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            this.TestLog = TestLog.Current;
            if (this.TestLog == null)
            {
                throw new InvalidOperationException("Must be run in the context of a test log");
            }
            if (_notHandledYet) // Only run this code for the first time install is seen
            {
                if (ExpandProgressUIDetails)
                {
                    ExpandProgressUI(topLevelhWnd);
                }

                UIHandlerAction cancelResult = UIHandlerAction.Abort;
                switch (CancelMethod)
                {
                    case CancelType.EscapeKey:
                    case CancelType.Regular:
                        cancelResult = CancelNormal(topLevelhWnd);
                        break;
                    case CancelType.CloseBrowser:
                        cancelResult = CloseBrowser(topLevelhWnd);
                        break;
                    case CancelType.NavigateAway:
                        cancelResult = NavigateAway(topLevelhWnd);
                        break;
                }
                if (cancelResult == UIHandlerAction.Abort)
                {
                    return cancelResult;
                }

                _notHandledYet = false;

                // Sleep 10 secs to let the app start up (in case Cancel failed)
                GlobalLog.LogDebug("Sleeping 10 seconds to make sure app isn't about to run...");
                Thread.Sleep(10000);

                // No need to restart if the app ran. need to fail.
                if (RestartApp && !FusionEntriesExist())
                {
                    // THIS UI WILL CHANGE. No idea when. When it does, this test breaks.
                    AutomationElement thisWindow = AutomationElement.FromHandle(topLevelhWnd);
                    PropertyCondition isRestart = new PropertyCondition(AutomationElement.AutomationIdProperty, "RetryButton");
                    AutomationElement restart = thisWindow.FindFirst(TreeScope.Descendants, isRestart);

                    if (restart != null)
                    {
                        GlobalLog.LogEvidence("Passed Cancel part of test. App not present before clicking restart");
                        this.TestLog.Result = TestResult.Pass;
                        object patternObject;
                        restart.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                        InvokePattern ip = patternObject as InvokePattern;
                        ip.Invoke();
                        return UIHandlerAction.Unhandled;
                    }
                    else
                    {
                        GlobalLog.LogEvidence("Failed: Couldnt get restart UI element to click.");
                        this.TestLog.Result = TestResult.Fail;
                        return UIHandlerAction.Abort;
                    }
                }
                else
                {
                    // Verify app isn't in fusion store...
                    if (FusionEntriesExist() || AvalonProcessesStillRunning())
                    {
                        GlobalLog.LogEvidence("Failed: Entries for app found in the fusion store or AppName not set");
                        this.TestLog.Result = TestResult.Fail;
                    }
                    else
                    {
                        GlobalLog.LogEvidence("Passed: No entries for app found in the fusion store");
                        this.TestLog.Result = TestResult.Pass;
                    }
                    return UIHandlerAction.Abort;
                }
           }
           return UIHandlerAction.Unhandled;
        }

        #region Private Methods

        private void ExpandProgressUI(IntPtr mainWindow)
        // This only works if the download is VERY slow (i.e. throttled).  
        // WaitForInputIdle() doesnt cause it to wait until it can be actually checked.
        {
            Thread.Sleep(2000);
            AutomationElement checkBox = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "DownloadDetailsToggle"));
            if (checkBox != null)
            {
                object patternObject;
                checkBox.TryGetCurrentPattern(TogglePattern.Pattern, out patternObject);
                TogglePattern tp = patternObject as TogglePattern;
                tp.Toggle();
                GlobalLog.LogEvidence("Expanded Progress UI checkbox");
            }
            else
            {
                GlobalLog.LogEvidence("Unable to expand progress UI details button");
            }
        }

        private UIHandlerAction CloseBrowser(IntPtr topLevelhWnd)
        {
            try
            {
                // Keep alive despite IE and PH, etc closing.
                // Workaround: Load IE.  Will be killed as soon as verification is done.
                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Internet Explorer\IEXPLORE.exe");

                AutomationElement windowToClose = AutomationElement.FromHandle(topLevelhWnd);
                object patternObject;
                windowToClose.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
                WindowPattern wp = patternObject as WindowPattern;
                wp.Close();
                GlobalLog.LogDebug("Closed main window");
            }
            catch (Exception e)
            {
                GlobalLog.LogDebug("Error while trying to close IE window during install... \n" + e.ToString());
                return UIHandlerAction.Abort;
            }
            return UIHandlerAction.Unhandled;
        }

        private UIHandlerAction NavigateAway(IntPtr topLevelhWnd)
        {
            AutomationElement IEWindow = AutomationElement.FromHandle(topLevelhWnd);
            AndCondition isAddrBar = IEAutomationHelper.GetIEAddressBarAndCondition();
            AutomationElement address = IEWindow.FindFirst(TreeScope.Descendants, isAddrBar);

            if (address != null)
            {
                address.SetFocus();

                //Sleep a second to make sure that the address bar has focus
                Thread.Sleep(2000);
                // Setting Focus is no longer a guarantee of highlighting all the text... so double-click to select it all before typing.
                MTI.Input.MoveToAndClick(address);
                MTI.Input.MoveToAndClick(address);
                // MTI.Input.SendUnicodeString(@"http://www.microsoft.com", 1, 15);
                ValuePattern vp = address.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (vp == null)
                {
                    throw new System.Exception("Couldn't get the valuePattern for the IE address bar! Please contact Microsoft to fix this.");
                }
                vp.SetValue("http://www.microsoft.com");
                MTI.Input.SendKeyboardInput(Key.Enter, true);
                MTI.Input.SendKeyboardInput(Key.Enter, false);

                if ((GetKeyboardLayout(0).ToInt32() & 0xffff) == 0x0411)
                {
                    Thread.Sleep(100);
                    MTI.Input.SendKeyboardInput(Key.Enter, true);
                    MTI.Input.SendKeyboardInput(Key.Enter, false);
                    GlobalLog.LogDebug("Hit 2nd Return for IME-enabled OS... ");
                }
            }
            else
            {
                GlobalLog.LogDebug("Couldn't find the IE address bar to navigate away");
                return UIHandlerAction.Abort;
            }
            return UIHandlerAction.Unhandled;
        }

        // Import method to figure out current keyboard layout... need to hit enter twice for IME languages.
        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(int idThread);

        private UIHandlerAction CancelNormal(IntPtr topLevelhWnd)
        {
                if (_inBrowserProgress)
                {
                    AutomationElement thisWindow = AutomationElement.FromHandle(topLevelhWnd);
                    if (UseUICancelButton)
                    {
                        PropertyCondition isCancelButton = new PropertyCondition(AutomationElement.AutomationIdProperty, "Button_1");
                        AutomationElement cancelButton = thisWindow.FindFirst(TreeScope.Descendants, isCancelButton);
                        try
                        {
                            WaitForStatusBarByExceptionStringId(topLevelhWnd, "HostingStatusDownloadApp");
                            // Try again if we failed to find it before.  Don't get much time to do this though.
                            if (cancelButton == null)
                            {
                                cancelButton = thisWindow.FindFirst(TreeScope.Descendants, isCancelButton);
                            }
                            object patternObject;
                            cancelButton.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                            InvokePattern ip = patternObject as InvokePattern;
                            ip.Invoke();
                            GlobalLog.LogEvidence("Invoked In-Browser cancel button");
                        }
                        catch (Exception failureReason)
                        {
                            GlobalLog.LogEvidence("Failed to click cancel button... test failed");
                            GlobalLog.LogDebug("Exception: " + failureReason.Message);
                            this.TestLog.Result = TestResult.Fail;
                            return UIHandlerAction.Abort;
                        }
                    }
                    else if (CancelMethod == CancelType.EscapeKey)
                    {
                        try
                        {
                            thisWindow.SetFocus();
                            WaitForStatusBarByExceptionStringId(topLevelhWnd, "HostingStatusDownloadApp");
                            MTI.Input.SendKeyboardInput(Key.Escape, true);
                            MTI.Input.SendKeyboardInput(Key.Escape, false);
                            GlobalLog.LogEvidence("Sent Esc Key to IE window");
                        }
                        catch (Exception ex)
                        {
                            GlobalLog.LogEvidence("Exception thrown trying to send 'Esc' to IE window... test failed \n" + ex.InnerException.ToString());
                            this.TestLog.Result = TestResult.Fail;
                            return UIHandlerAction.Abort;
                        }
                    }
                    else
                    {
                        // 
                        OrCondition isStopButton = new OrCondition(new PropertyCondition(AutomationElement.NameProperty, "Stop"),
                                                                   new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 292"),   // Old AutoID in IE7.  Leave in for a while til we're sure this is gone in IE7 builds!
                                                                   new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 101"),   // AutoID in IE7
                                                                   new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 1016"));  // AutoID in IE6
                        AutomationElement stopButton = thisWindow.FindFirst(TreeScope.Descendants, isStopButton);
                        try
                        {
                            WaitForStatusBarByExceptionStringId(topLevelhWnd, "HostingStatusDownloadApp");
                            object patternObject;
                            stopButton.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                            InvokePattern ip = patternObject as InvokePattern;
                            ip.Invoke();
                            GlobalLog.LogEvidence("Invoked IE Stop button");
                        }
                        catch (Exception ex)
                        {
                            GlobalLog.LogEvidence("Exception thrown trying to click stop button... test failed \n" + ex.InnerException.ToString());
                            this.TestLog.Result = TestResult.Fail;
                            return UIHandlerAction.Abort;
                        }                        
                    }
                }
                else // Handle the standalone progress window...
                {
                    MTI.Input.SendKeyboardInput(Key.Space, true);
                    MTI.Input.SendKeyboardInput(Key.Space, false);
                }
                return UIHandlerAction.Unhandled;
            }

        private void WaitForStatusBarByExceptionStringId(IntPtr windowHandle, string toWaitFor)
        {
            // Get the localized text we expect in the status bar...
            // Should only need values from PresentationFramework here, could be expanded later if needed.
            ResourceManager resMan = new ResourceManager("ExceptionStringTable", typeof(System.Windows.Controls.Control).Assembly);
            WaitForStatusBar(windowHandle, resMan.GetString(toWaitFor));
        }

        private void WaitForStatusBar(IntPtr windowHandle, string toWaitFor)
        {
            GlobalLog.LogDebug("Entering wait loop for status bar value: " + toWaitFor);
            AutomationElement theWindow = AutomationElement.FromHandle(windowHandle);
            AutomationElement statusBar = theWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "StatusBar.Pane0"));
            if (statusBar == null)
            {
                GlobalLog.LogEvidence("Failed finding the status bar to wait for " + toWaitFor);
                return;
            }

            object patternObject;
            statusBar.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject);
            ValuePattern vp = (ValuePattern)patternObject;
            int totalTries = 0;
            while (!(vp.Current.Value.ToLowerInvariant().Equals(toWaitFor.ToLowerInvariant())) && (totalTries < 50))
            {
                System.Threading.Thread.Sleep(100);
                object _patternObject;
                statusBar.TryGetCurrentPattern(ValuePattern.Pattern, out _patternObject);
                vp = (ValuePattern)_patternObject;
                totalTries++;
            }
            if (totalTries < 50)
            {
                GlobalLog.LogEvidence("Found status bar, waited until it displayed " + toWaitFor);
            }
        }

        private bool FusionEntriesExist()
        {
            if (_appName == null)
            {
                GlobalLog.LogDebug("ERROR: AppName not specified in config file.  Cannot verify app presence in Fusion Cache");
                return true;
            }
            string FusionCachePath = ApplicationDeploymentHelper.FusionCachePath;
            if (FusionCachePath == null)
            {
                return false;
            }
            GlobalLog.LogDebug("Found fusion path at : " + FusionCachePath + "...");
            string[] fusionContents = Directory.GetDirectories(FusionCachePath);
            foreach (string dirName in fusionContents)
            {
                if (dirName.ToLower().Contains(this.AppName.ToLower().Substring(0, 10)))
                {
                    GlobalLog.LogEvidence("Found Directory: " + dirName);
                    return true;
                }
            }
            return false;
        }

        private bool AvalonProcessesStillRunning()
        {
            // Skip this if normal cancel method, since PresentationHost is still running
            if (CancelMethod == CancelType.Regular)
            {
                return false;
            }
            // Sleep a little bit to give processes that are exiting a chance to finish...
            Thread.Sleep(5000);
            bool foundAvalonProcess = false;
            GlobalLog.LogDebug("Checking if there are Avalon processes currently running:");
            Process[] procList = Process.GetProcesses();
            foreach (Process p in procList)
            {
                if ((p.ProcessName.ToLowerInvariant().Equals("presentationhost")) ||
                    (p.ProcessName.ToLowerInvariant().Equals("presentationstartup")))
                {
                    GlobalLog.LogEvidence("Found process " + p.ProcessName + " running where it should not be.");
                    foundAvalonProcess = true;
                }
            }
            return foundAvalonProcess;
        }
        #endregion
    }
}
