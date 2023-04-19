// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows.Automation;
using System.Threading;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders.Steps;
using MTI = Microsoft.Test.Input;
using System.Runtime.InteropServices;
using Microsoft.Test.Deployment;
using System.Resources;

namespace Microsoft.Test.Windows.Client.AppSec.Deployment
{
    public class StandaloneCancelHandler : UIHandler
    {
        public StandaloneCancelHandler()
        {
            GlobalLog.LogDebug("Registered Standalone App Canceler for " + this.WindowTitle + ", " + this.ProcessName);
        }

        public override UIHandlerAction HandleWindow(IntPtr topLevelhWnd, IntPtr hwnd, System.Diagnostics.Process process, string title, UIHandlerNotification notification)
        {
            MTI.Input.SendKeyboardInput(Key.Space, true);
            MTI.Input.SendKeyboardInput(Key.Space, false);
            // Sleep for a while to let the UI pop if it will.  If it does, the above failed and we will fail using a different handler
            Thread.Sleep(6000);
            TestLog.Current.Result = TestResult.Pass;
            return UIHandlerAction.Abort;
        }
    }
    /// <summary>
    /// Relaunches an installed .application, then verifies it is not in ClickOnceCache.  
    /// Needs to be run in the context of a TestLogStep
    /// </summary>    
    public class CancelDownloadStep : LoaderStep
    {
        #region private data
        ApplicationMonitor _appMonitor;
        TestLog _testLog;
        UIHandler[] _uiHandlers = new UIHandler[0];
        #endregion

        #region Public members

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
            StopButton,
            /// <summary>
            /// Cancel by clicking either "Stop" or the UI Cancel button
            /// </summary>
            UICancelButton,
            /// <summary>
            /// Cancel by closing the browser window
            /// </summary>
            CloseBrowser,
            /// <summary>
            /// Cancel by navigating away to some other site.
            /// </summary>
            NavigateAway,
            /// <summary>
            /// Cancel by sending F5 to the browser window... 
            /// </summary>
            RefreshKey,
            /// <summary>
            /// Same as RefreshKey, but first calls SetFocus on the browser.
            /// </summary>
            RefreshKeyFocusBrowser

        }

        /// <summary>
        /// Method of cancelling
        /// </summary>
        public CancelType Method = CancelType.UICancelButton;

        /// <summary>
        /// gets or sets an array of UIHandlers that will be registered with Activation Host
        /// </summary>
        public UIHandler[] UIHandlers
        {
            get { return _uiHandlers; }
            set
            {
                foreach (UIHandler handler in value)
                    handler.Step = this;
                _uiHandlers = value;
            }
        }

        /// <summary>
        /// Name of .xbap to cancel
        /// </summary>
        public string FileName;

        /// <summary>
        /// Bool representing whether app should be restarted or not.  
        /// </summary>
        public bool RestartDownload = false;

        /// <summary>
        /// Gets or sets the Scheme to when activating the application
        /// </summary>
        public FileHostUriScheme Scheme = FileHostUriScheme.Local;

        /// <summary>
        /// Gets the ApplicationMonitor that is monitoring the target application
        /// </summary>
        /// <value>the ApplicationMonitor that is monitoring the target application</value>
        public ApplicationMonitor Monitor
        {
            get { return _appMonitor; }
        }

        /// <summary>
        /// Application name string to verify on progress page
        /// </summary>
        public string ExpectedAppName = "";

        /// <summary>
        /// Publisher name string to verify on progress page
        /// </summary>
        public string ExpectedPublisherName = "";

        #endregion

        #region Step Implementation
        // Import method to figure out current keyboard layout... need to hit enter twice for IME languages.
        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(int idThread);

        /// <summary>
        /// Invokes app (assumed to be installed and shell-visible), then checks property bag for value left by 
        /// TrustManager dialog handler to see if the trustmanager ran more than once.
        /// </summary>
        /// <returns>true</returns>
        protected override bool BeginStep()
        {
            #region Setup
            // Get current Test Log
            this._testLog = TestLog.Current;
            if (this._testLog == null)
            {
                throw new InvalidOperationException("Relaunch step must be created in the context of a test log");
            }

            if (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.ToUpperInvariant() != "EN")
            {
                if (Method == CancelType.RefreshKey || Method == CancelType.RefreshKeyFocusBrowser ||
                    Method == CancelType.StopButton)
                {
                    this._testLog.LogEvidence("Ignoring due to inability to load string resources for browser stop/refresh buttons.");
                    this._testLog.Result = TestResult.Ignore;
                    ApplicationMonitor.NotifyStopMonitoring();
                    return false;
                }
            }

            //Create ApplicationMonitor
            _appMonitor = new ApplicationMonitor();

            LoaderStep parent = this.ParentStep;
            FileHostStep parentFileHost = null;

            string UriToNavigate = string.Empty;

            while (parent != null)
            {
                if (parent.GetType() == typeof(Microsoft.Test.Loaders.Steps.FileHostStep))
                {
                    parentFileHost = ((FileHostStep)parent);
                    break;
                }
                // Failed to find it in the immediate parent: try til we hit null or the right one
                parent = parent.ParentStep;
            }

            if (parentFileHost != null)
            {
                UriToNavigate = parentFileHost.fileHost.GetUri(FileName, Scheme).ToString();
                GlobalLog.LogEvidence("Launching IE with argument \"" + FileName + "\"");
            }

            #endregion

            // Due to the complexity of cancelling a quick, asynchronous action, we'll give it several tries to succeed
            int tryCount = 7;
            bool succeeded = false;

            while (tryCount > 0 && !succeeded)
            {
                succeeded = false;
                tryCount--;

                // Clean up... 
                ApplicationDeploymentHelper.CleanClickOnceCache();

                while ((Process.GetProcessesByName("iexplore").Length > 0) &&
                       (Process.GetProcessesByName("presentationhost").Length > 0))
                {
                    CleanupHostingProcesses();
                }

                Process ieProc = Process.Start("iexplore.exe", "about:NavigateIE");
                while (!ieProc.HasExited && (ieProc.MainWindowHandle == IntPtr.Zero))
                {
                    Thread.Sleep(500);
                    ieProc.Refresh();
                }

                if (ieProc.HasExited)
                {
                    ieProc = Process.GetProcessesByName("iexplore")[0];
                    while (!ieProc.HasExited && (ieProc.MainWindowHandle == IntPtr.Zero))
                    {
                        Thread.Sleep(500);
                        ieProc.Refresh();
                    }
                }
                AutomationElement ieWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame"));

                // Get a reference the address bar...
                int findAddrBarTimeOut = 15;
                AutomationElement addressBar;

                do
                {
                    GlobalLog.LogDebug("Finding the IE address bar");
                    AndCondition isAddrBar = new AndCondition(new PropertyCondition(ValuePattern.ValueProperty, "about:NavigateIE"),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));
                    addressBar = ieWindow.FindFirst(TreeScope.Descendants, isAddrBar);
                    findAddrBarTimeOut--;
                    Thread.Sleep(1000);
                }
                while ((findAddrBarTimeOut >= 0) && addressBar == null);

                if (addressBar == null)
                {
                    GlobalLog.LogEvidence("Error, couldnt get the AutomationElement for the address bar!");
                    _testLog.Result = TestResult.Ignore;
                    return false;
                }

                // Type the URI and press return
                GlobalLog.LogDebug("Typing " + UriToNavigate.ToString() + " into the address bar");

                try
                {
                    MTI.Input.MoveToAndClick(addressBar);
                }
                catch (System.Windows.Automation.NoClickablePointException)
                {
                    // Do nothing... we'll try again in a little bit hoping this has fixed itself.
                }

                ValuePattern vp = addressBar.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (vp == null)
                {
                    GlobalLog.LogEvidence("Couldn't get the valuePattern for the IE address bar! Please contact Microsoft to fix this.");
                    _testLog.Result = TestResult.Ignore;
                    return false;
                }      

                int valueSetCountdown = 5;
                do
                {
                    vp.SetValue(UriToNavigate.ToString());

                    valueSetCountdown--;
                    Thread.Sleep(1500);
                }
                while ((vp.Current.Value != UriToNavigate.ToString()) && (valueSetCountdown > 0));

                if (vp.Current.Value != UriToNavigate.ToString())
                {
                    GlobalLog.LogEvidence("Couldn't set the value on the IE address bar! (Test bug, NOT product bug) Please contact Microsoft to fix this.");
                    _testLog.Result = TestResult.Ignore;
                    return false;
                }      

                // Can't trust setfocus to work, so also click on the address bar... 
                MTI.Input.MoveToAndClick(addressBar);

                MTI.Input.SendKeyboardInput(Key.Enter, true);
                MTI.Input.SendKeyboardInput(Key.Enter, false);

                // Send a 2nd Enter to the address bar if we're Japanese, as IME is on by default and requires this.
                // If this UIHandler fails for other languages, add their LCID's to the if statement.
                if ((GetKeyboardLayout(0).ToInt32() & 0xffff) == 0x0411)
                {
                    MTI.Input.SendKeyboardInput(Key.Enter, true);
                    MTI.Input.SendKeyboardInput(Key.Enter, false);
                    GlobalLog.LogDebug("Hit 2nd Return for IME-enabled OS... ");
                }

                AutomationElement cancelButton;
                OrCondition isCancelButton = new OrCondition(new PropertyCondition(AutomationElement.NameProperty, ApplicationDeploymentHelper.CancelPageUIButtonName),
                                                             new PropertyCondition(AutomationElement.AutomationIdProperty, "Button_1"));
                GlobalLog.LogDebug("Cancel button detected name is " + ApplicationDeploymentHelper.CancelPageUIButtonName);


                switch (Method)
                {
                    case CancelType.UICancelButton:
                    case CancelType.StopButton:
                        if (Method == CancelType.UICancelButton)
                        {
                            GlobalLog.LogStatus("Cancel button: " + ApplicationDeploymentHelper.CancelPageUIButtonName);
                            WaitForStatusBarByExceptionStringId(ieWindow, "HostingStatusDownloadApp");
                            cancelButton = ieWindow.FindFirst(TreeScope.Descendants, isCancelButton);
                        }
                        else
                        {
                            if (System.Globalization.CultureInfo.InstalledUICulture.TextInfo.IsRightToLeft)
                            {
                                GlobalLog.LogDebug("Using alternate AutoID due to RTL Language OS");
                                isCancelButton = new OrCondition(new PropertyCondition(AutomationElement.NameProperty, "Stop"),
                                                                          new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 101"),   // AutoID in IE7
                                                                          new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 1027"), // AutoID in IE6
                                                                          new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 1014")); // AutoID in IE6 (Randomly see both)
                            }
                            else
                            {
                                isCancelButton = new OrCondition(new PropertyCondition(AutomationElement.NameProperty, "Stop"),
                                                                          new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 292"),   // Old AutoID in IE7.  Leave in for a while til we're sure this is gone in IE7 builds!
                                                                          new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 101"),   // AutoID in IE7
                                                                          new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 1014"),
                                                                          new PropertyCondition(AutomationElement.AutomationIdProperty, "Item 1027")); // AutoIDs in IE6 (With / without Avalon)
                            }
                            WaitForStatusBarByExceptionStringId(ieWindow, "HostingStatusDownloadApp");
                            cancelButton = ieWindow.FindFirst(TreeScope.Descendants, isCancelButton);
                        }
                        try
                        {
                            // Try again if we failed to find it before.  Don't get much time to do this though.
                            if (cancelButton == null)
                            {
                                cancelButton = ieWindow.FindFirst(TreeScope.Descendants, isCancelButton);
                            }

                            // This uses UIA and can be slow, so only try it for the first two tries.
                            if (tryCount > 5)
                            {
                                VerifyProgressUIValues(ieWindow);
                            }
                            object patternObject;
                            cancelButton.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                            InvokePattern ip = patternObject as InvokePattern;
                            ip.Invoke();
                            GlobalLog.LogEvidence("Invoked In-Browser cancel button");
                        }
                        catch (Exception cancelErr)
                        {
                            GlobalLog.LogDebug("Failed to click cancel button... Will retry");
                            GlobalLog.LogDebug("(" + cancelErr.Message + ")");
                        }
                        break;

                    case CancelType.EscapeKey:
                        try
                        {
                            ieWindow.SetFocus();
                            WaitForStatusBarByExceptionStringId(ieWindow, "HostingStatusDownloadApp");
                            MTI.Input.SendKeyboardInput(Key.Escape, true);
                            MTI.Input.SendKeyboardInput(Key.Escape, false);
                            GlobalLog.LogDebug("Sent Esc Key to IE window");
                        }
                        catch
                        {
                            GlobalLog.LogDebug("Exception thrown trying to send 'Esc' to IE window... will retry. ");
                        }
                        break;

                    case CancelType.NavigateAway:
                        WaitForStatusBarByExceptionStringId(ieWindow, "HostingStatusDownloadApp");
                        try
                        {
                            MTI.Input.MoveToAndClick(addressBar);
                        }
                        catch (System.Windows.Automation.NoClickablePointException)
                        {
                            // Do nothing... we'll try again in a little bit hoping this has fixed itself.
                        }
                        //addressBar.SetFocus();
                        //Sleep a second to make sure that the address bar has focus
                        Thread.Sleep(200);
                        vp.SetValue("http://www.microsoft.com");
                        MTI.Input.MoveToAndClick(addressBar);
                        MTI.Input.SendKeyboardInput(Key.Enter, true);
                        MTI.Input.SendKeyboardInput(Key.Enter, false);

                        if ((GetKeyboardLayout(0).ToInt32() & 0xffff) == 0x0411)
                        {
                            Thread.Sleep(100);
                            MTI.Input.SendKeyboardInput(Key.Enter, true);
                            MTI.Input.SendKeyboardInput(Key.Enter, false);
                            GlobalLog.LogEvidence("Hit 2nd Return for IME-enabled OS... ");
                        }

                        break;
                    case CancelType.CloseBrowser:
                        try
                        {
                            // Keep alive despite IE and PH, etc closing.
                            // Workaround: Load IE.  Will be killed as soon as verification is done.
                            // Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Internet Explorer\IEXPLORE.exe");
                            WaitForStatusBarByExceptionStringId(ieWindow, "HostingStatusDownloadApp");
                            object patternObject;
                            ieWindow.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
                            WindowPattern wp = patternObject as WindowPattern;
                            wp.Close();
                            GlobalLog.LogEvidence("Closed main window");
                        }
                        catch
                        {
                            GlobalLog.LogDebug("Error while trying to close IE window during install... will try again");
                        }
                        break;
                    case CancelType.RefreshKeyFocusBrowser:
                        ieWindow.SetFocus();
                        goto case CancelType.RefreshKey;
                    case CancelType.RefreshKey:
                        
                        int timeoutCount = 1000;
                        while ((ieWindow.FindFirst(TreeScope.Descendants, isCancelButton) == null) && (timeoutCount > 0))
                        {
                            timeoutCount--;
                            Thread.Sleep(10);
                        }

                        Process[] PHosts = Process.GetProcessesByName("PresentationHost");
                        if (PHosts.Length != 1)
                        {
                            GlobalLog.LogEvidence("Error! Expected 1 but saw " + PHosts.Length + " instances of PresentationHost.exe");
                            TestLog.Current.Result = TestResult.Fail;
                            return false;
                        }
                        int previousId = PHosts[0].Id;

                        MTI.Input.SendKeyboardInput(Key.F5, true);
                        MTI.Input.SendKeyboardInput(Key.F5, false);
                        Thread.Sleep(500);

                        PHosts = Process.GetProcessesByName("PresentationHost");

                        // This will cause the test to time out if the progress page doesnt come back.
                        timeoutCount = 1000;
                        while ((ieWindow.FindFirst(TreeScope.Descendants, isCancelButton) == null) && (timeoutCount > 0))
                        {
                            timeoutCount--;
                            Thread.Sleep(10);
                        }

                        // It's normal to see the old instance around for a while
                        // This is covered by another test, so just make sure there arent > 2
                        if ((PHosts[0].Id != previousId) && PHosts.Length < 3)
                        {
                            GlobalLog.LogEvidence("Success! Refresh caused download of .xbap to restart!");
                            TestLog.Current.Result = TestResult.Pass;
                            return false;
                        }
                        break;
                }

                switch (Method)
                {
                    case CancelType.EscapeKey:
                    case CancelType.StopButton:
                    case CancelType.UICancelButton:
                        if (IEAutomationHelper.WaitForElementWithAutomationId(ieWindow, "RetryButton", 10) != null)
                        {
                            GlobalLog.LogEvidence("Successfully cancelled .xbap with " + tryCount.ToString() + " tries left");
                            succeeded = true;

                            if (RestartDownload)
                            {
                                GlobalLog.LogEvidence("Restart was selected, clicking restart ... ");

                                // register UIHandlers (The trick is to not do this until we've successfully cancelled!)
                                foreach (UIHandler handler in UIHandlers)
                                {
                                    if (handler.NamedRegistration != null)
                                        _appMonitor.RegisterUIHandler(handler, handler.NamedRegistration, handler.Notification);
                                    else
                                        _appMonitor.RegisterUIHandler(handler, handler.ProcessName, handler.WindowTitle, handler.Notification);
                                }
                                _appMonitor.StartMonitoring(false);
                                IEAutomationHelper.InvokeElementViaAutomationId(ieWindow, "RetryButton", 10);
                            }
                            else
                            {
                                if (IsProcessRunning("PresentationHost", 1))
                                {
                                    _testLog.Result = TestResult.Pass;
                                }
                                else
                                {
                                    _testLog.Result = TestResult.Fail;
                                }
                            }
                        }
                        else
                        {
                            if (tryCount == 0 && !succeeded)
                            {
                                GlobalLog.LogEvidence("Still Failing to cancel download of .xbap. Quitting with FAIL");
                                _testLog.Result = TestResult.Fail;
                            }
                            else
                            {
                                GlobalLog.LogEvidence("Failed to cancel download of .xbap. " + tryCount.ToString() + " tries left");
                            }
                        }
                        break;
                    case CancelType.CloseBrowser:
                    case CancelType.NavigateAway:
                        Thread.Sleep(4000);
                        if (Process.GetProcessesByName("presentationhost").Length == 0)
                        {
                            succeeded = true;
                            GlobalLog.LogEvidence("Successfully cancelled xbap install!");

                            if (IsProcessRunning("PresentationHost", 0))
                            {
                                _testLog.Result = TestResult.Pass;
                            }
                            else
                            {
                                _testLog.Result = TestResult.Fail;
                            }
                        }
                        else
                        {
                            if (tryCount == 0 && !succeeded)
                            {
                                GlobalLog.LogEvidence("Still Failing to cancel download of .xbap. Quitting with FAIL");
                                _testLog.Result = TestResult.Fail;
                            }
                            else
                            {
                                GlobalLog.LogEvidence("Failed to close browser during xbap install ... Retries remaining: " + tryCount);
                            }
                        }
                        break;
                }
            }
            return true;
        }

        private void VerifyProgressUIValues(AutomationElement IEWindow)
        {
            PropertyCondition isTextElement = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text);
            AutomationElementCollection aec = IEWindow.FindAll(TreeScope.Descendants, isTextElement);

            if (aec.Count < 10)
            {
                // We expect at least 10 total text elements; if there aren't that many, then the progress page isn't showing.
                GlobalLog.LogEvidence("Progress Page not showing in UI Verification");
                return;
            }

            AutomationElement AppNameTag = aec[3];
            AutomationElement PubNameTag = aec[4];
            AutomationElement ProgressKB = aec[7];
            AutomationElement TotalKB    = aec[9];

            if (ExpectedAppName != "")
            {
                if (AppNameTag.Current.Name == ExpectedAppName)
                {
                    GlobalLog.LogEvidence("Expected Application name (" + ExpectedAppName + ")  seen... ");
                }
                else
                {
                    // It turns out this only happens in practice when we're not actually LOOKING at the progress page.
                    // So quit as rapidly as possible if that is the case so cancel will succeed.
                    GlobalLog.LogEvidence("Expected Application name (" + ExpectedAppName + ")  was not seen. (Most likely progress page has gone away)");
                    return;
                }
            }
            if (ExpectedPublisherName != "")
            {
                if (PubNameTag.Current.Name == ExpectedPublisherName)
                {
                    GlobalLog.LogEvidence("Expected Publisher name (" + ExpectedPublisherName + ")  seen... ");
                }
                else
                {
                    GlobalLog.LogEvidence("Expected Publisher name (" + ExpectedPublisherName + ")  was not seen!!! ");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            int sleepCount = 1000;
            while ((ProgressKB.Current.Name.Split(' ')[0] == "0") && (sleepCount > 0))
            {
                sleepCount--;
                Thread.Sleep(10);
            }
            if (sleepCount > 0)
            {
                GlobalLog.LogEvidence("Saw changed value (" + ProgressKB.Current.Name + ") in Progress KB value!");
            }
            else
            {
                GlobalLog.LogEvidence("Didnt see any change to KB progress value in 10 seconds!");
                TestLog.Current.Result = TestResult.Ignore;
            }
            if (TotalKB.Current.Name.Split(' ')[0] != "0")
            {
                GlobalLog.LogEvidence("Saw a value (" + TotalKB.Current.Name + ") in Total KB to download!");
            }
            else
            {
                GlobalLog.LogEvidence("ERROR: Didn't see any value in Total KB to download!");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        private static void CleanupHostingProcesses()
        {
            Process[] allIEs = Process.GetProcessesByName("iexplore");
            foreach (Process p in allIEs)
            {
                try
                {
                    if (!p.HasExited)
                    {
                        p.Kill();
                    }
                }
                catch (Exception)
                {
                    // Do nothing. 
                }
            }

            // Give PresentationHost instances a chance to die naturally...
            Thread.Sleep(100);

            Process[] allPHosts = Process.GetProcessesByName("presentationhost");
            foreach (Process p in allPHosts)
            {
                try
                {
                    if (!p.HasExited)
                    {
                        p.Kill();
                    }
                }
                catch (Exception)
                {
                    // Do nothing. 
                }
            }
        }

        private static bool IsProcessRunning(string processName, int numProcesses)
        {
            // processes don't always appear or disappear immediately
            // this gives them 10 seconds to appear
            int tries = 0;
            Process[] processes = Process.GetProcessesByName(processName);

            while ((processes == null || processes.Length != numProcesses) && tries <= 10)
            {
                tries++;
                Thread.Sleep(1000);
                processes = Process.GetProcessesByName(processName);
            }

            if (processes != null && processes.Length == numProcesses)
            {
                GlobalLog.LogEvidence("Found expected " + numProcesses + " " + processName + " process(es)");
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Found " + processes.Length + " " + processName + " process(es) where there should have been " + numProcesses);
                return false;
            }
        }

        /// <summary>
        /// Process Cleanup
        /// </summary>
        /// <returns>true</returns>
        protected override bool EndStep()
        {
            GlobalLog.LogDebug("Finished test ... cleaning up");
            //Wait for the application to be done, IFF there are UIHandlers (not necessary)
            if (UIHandlers.Length > 0)
            {
                _appMonitor.WaitForUIHandlerAbort();
            }
            ApplicationMonitor.NotifyStopMonitoring();

            // Clean up since we didnt start the process with AppMonitor (since it wont give us the Process object)
            CleanupHostingProcesses();

            return true;
        }
        #endregion

        private static void WaitForStatusBarByExceptionStringId(AutomationElement theWindow, string toWaitFor)
        {
            // Get the localized text we expect in the status bar...
            // Should only need values from PresentationFramework here, could be expanded later if needed.
            ResourceManager resMan = new ResourceManager("ExceptionStringTable", typeof(System.Windows.Controls.Control).Assembly);
            WaitForStatusBar(theWindow, resMan.GetString(toWaitFor));
        }

        private static void WaitForStatusBar(AutomationElement theWindow, string toWaitFor)
        {
            if (ApplicationDeploymentHelper.GetIEVersion() >= 9)
            {
                GlobalLog.LogDebug("WaitForStatusBar skipped for IE9");
                DateTime in15Seconds = DateTime.Now.Add(TimeSpan.FromSeconds(15));
                while ((Process.GetProcessesByName("PresentationHost").Length == 0) && (in15Seconds > DateTime.Now))
                {
                    // spin.
                }
                // Ugly but needed, don't have anything else to key on in IE9.  This starts cancel attempts after PH.exe has run for 300 ms.
                Thread.Sleep(300);
                return;
            }
            GlobalLog.LogDebug("Entering wait loop for status bar value: " + toWaitFor);

            if (theWindow == null)
            {
                GlobalLog.LogDebug("WaitForStatusBar called but with no base UIA element to search from.  Trying from RootElement...");
                theWindow = AutomationElement.RootElement;
            }
        RetryThis:
            try
            {
            
                AutomationElement statusBar = theWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "StatusBar.Pane0"));
                if (statusBar == null)
                {
                    GlobalLog.LogDebug("Failed finding the status bar to wait for " + toWaitFor);
                    return;
                }

                object patternObject;
                statusBar.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject);
                ValuePattern vp = (ValuePattern)patternObject;
                int totalTries = 0;
                while (!(vp.Current.Value.ToLowerInvariant().Equals(toWaitFor.ToLowerInvariant())) && (totalTries < 500))
                {
                    System.Threading.Thread.Sleep(100);
                    object _patternObject;
                    statusBar.TryGetCurrentPattern(ValuePattern.Pattern, out _patternObject);
                    vp = (ValuePattern)_patternObject;
                    totalTries++;
                }
                if (totalTries < 500)
                {
                    GlobalLog.LogDebug("Found status bar, waited until it displayed " + toWaitFor);
                }
            }
            catch (ElementNotAvailableException)
            {
                goto RetryThis;
            }
            catch (Exception exc)
            {
                GlobalLog.LogEvidence("Error! Exception thrown while waiting for status bar value.  \n" + exc.Message + "\n" + exc.StackTrace);
                throw exc;
            }
        }
    }
}
