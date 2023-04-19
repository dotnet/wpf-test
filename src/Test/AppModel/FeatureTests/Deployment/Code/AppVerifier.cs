// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Automation;
using Microsoft.Test.Loaders;
using System.Windows.Input;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Logging;
using MTI = Microsoft.Test.Input;
using Microsoft.Win32;
using Microsoft.Test.Diagnostics;
using Microsoft.Test.CrossProcess;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Summary description for AppVerifier.
    /// </summary>
    public class AppVerifier : UIHandler
    {
        #region Private Members
        /// <summary>
        /// Testlog instance for Appverifier.  
        /// </summary>
        protected TestLog TestLog;

        /// <summary>
        /// Array of all processes that should be running when App is fully running.
        /// Verified in HandleWindow()
        /// </summary>
        protected ArrayList ProcessesToCheck;

        int _presHostInstances = 1;

        string _appName = null;
        #endregion

        #region Public Members

        /// <summary>
        /// Determines whether appVerifier should check to see that the app it is verifying is actually 
        /// cached in the AppStore.  Default - True.
        /// </summary>
        public bool AppShouldBeInStore = true;

        /// <summary>
        /// Determines whether the HandleWindow() method will add this app as a bookmark
        /// (Ctrl-D method used).  IE window must be generated on app running, or this will do nothing.
        /// Default: False
        /// </summary>
        public bool AddAsFavorite = false;

        /// <summary>
        /// Determines whether the HandleWindow() method will look for shortcut and support
        /// link when verifying the window.
        /// </summary>
        public bool IsAppShellVisible = false;

        /// <summary>
        /// Lets user specify whether to ignore the trust dialog in verifying app correctness.
        /// Only matters if using .container or .application in the context of an ActivationStep
        /// </summary>
        public bool IgnoreTrustDialog = false;

        /// <summary>
        /// Lets user specify cases where the TM dialog needs to NOT show up.
        /// Only matters if using .container or .application in the context of an ActivationStep
        /// (As this is overriding default behavior) 
        /// </summary>
        public bool ShouldNotSeeTM = false;

        /// <summary>
        /// If defined, is used to verify the title of window.  (For when process name
        /// is different from window title)
        /// </summary>
        public string ExpectedWindowTitle = null;

        /// <summary>
        /// For special cases for verifying that an app is granted only a certain set of permissions.
        /// Appverifier will try to press a button named "TestSecurity", then search for text with this string.
        /// Disabled by default.
        /// </summary>
        public string ZoneVerificationString = null;

        /// <summary>
        /// Constructor for AppVerifier.  Instantiates new list of ProcessesToCheck.
        /// </summary>
        public AppVerifier()
        {
            this.ProcessesToCheck = new ArrayList();
        }

        /// <summary>
        /// Disables check of IE zone for mixed mode scenarios, i.e. when the zone will expectedly not match the stated scheme.
        /// </summary>
        public bool IgnoreIEZoneString = false;

        /// <summary>
        /// Only meaningful on browser apps on AMD64 OSes.  Checks that 64-bit PresentationHost is always started on 64-bit Vista+
        /// and that 32-bit PresentationHost.exe is started in Server 2k3 64-bit
        /// </summary>
        public bool CheckPresentationHostBitness = false;

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
        /// Number of PresentationHost processes to allow.  Defaults to 1, but for special cases
        /// where multiple processes can be needed (ex: multiple avalon elements in HTML Frames)
        /// </summary>
        public int PresHostInstances
        {
            get { return _presHostInstances; }
            set { _presHostInstances = value; }
        }

        /// <summary>
        /// Checks the zone string in the IE window
        /// </summary>
        /// <param name="scheme">Scheme used in app launch</param>
        /// <param name="topLevelHwnd">Hwnd of the top level browser window</param>
        /// <returns></returns>
        public static bool CheckIEZone(ActivationScheme scheme, IntPtr topLevelHwnd)
        {
            return checkIEZone(scheme, topLevelHwnd);
        }
        #endregion

        #region Handler Implementation
        /// <summary>
        /// Handler to verify that the properties we are interested in for a particular window
        /// are correct, specifically window title, processes running, and shell visibility.
        /// When finishes, closes testlog and application's main window.  
        /// </summary>
        /// <param name="topLevelhWnd"></param>
        /// <param name="hwnd"></param>
        /// <param name="process"></param>
        /// <param name="title"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public override UIHandlerAction HandleWindow(System.IntPtr topLevelhWnd, System.IntPtr hwnd, Process process, string title, UIHandlerNotification notification)
        {
            this.TestLog = TestLog.Current;
            if (this.TestLog == null)
            {
                throw new InvalidOperationException("Must be run in the context of a test log");
            }

            #region PresentationHost.exe Bitness check (32 vs 64-bit process on 64-bit OS)

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

            bool presentationHostBitnessCorrect = true;
            if ((CheckPresentationHostBitness) && (Environment.GetEnvironmentVariable("ProgramFiles(x86)") != null))
            {
                GlobalLog.LogEvidence("CheckPresentationHostBitness specified: Now attempting to validate correct PresentationHost.exe bitness was launched");

                // We no longer test on any environment where PH is "always 64-bit" or "always 32-bit"
                bool shouldBe64Bit = bool.Parse(DictionaryStore.Current["PresentationHostShouldBe64Bit"]);
                // Downlevel (Pre-Vista): always 32-bit (for now...)
                if (SystemInformation.Current.MajorVersion < 6)
                {
                    GlobalLog.LogEvidence("Checking v3.5 SP1/v4.0 Pre-Vista Presentationhost bitness behavior (always 32-bit)");
                    shouldBe64Bit = false;
                }
                else
                {
                    GlobalLog.LogEvidence("Checking v3.5 SP1/v4.0 Vista+ Presentationhost bitness behavior (always match caller)");
                }

                if (IEAutomationHelper.Is64BitProcess(process.Id) == shouldBe64Bit)
                {
                    GlobalLog.LogEvidence("PresentationHost.exe was " + (shouldBe64Bit ? "64" : "32") + " bit (correct)");
                }
                else
                {
                    presentationHostBitnessCorrect = false;
                    GlobalLog.LogEvidence("PresentationHost.exe was not " + (shouldBe64Bit ? "64" : "32") + " bit (incorrect)");
                }
            }
            #endregion

            // Bookmark this in any available IE window...
            if (AddAsFavorite)
            {
                addBookmark(title);
            }

            TestSecurity(ZoneVerificationString, topLevelhWnd);

            #region Trust Dialog behavior verification
            bool trustDialogCorrect = true;
            
            GlobalLog.LogEvidence("Skipping any Trust Dialog verification due to different behavior for HTTP Internet in patched /unpatched setups.  ");
            GlobalLog.LogEvidence("Test coverage for this exists in CLR / Clickonce tests ");
            DictionaryStore.Current["HandledTrustManagerDialog"] = null;

            #endregion

            MachineStateVerifier machineStateVerifier = new MachineStateVerifier(this.AppName);

            bool runningFromExpectedZone = true;
            bool startMenuItemsExist = false;
            bool processesRunning = true;
            bool windowTitleCorrect = false;
            bool appStoreStateCorrect = false;
            string genericTitle;

            if (this.ExpectedWindowTitle != null)
            {
                genericTitle = this.ExpectedWindowTitle.ToLowerInvariant();
            }
            else
            {
                // If not verifying a specific title, set this to the actual title so it will never fail
                genericTitle = title.ToLowerInvariant();
            }

            // If the parent step is the commonly used ActivationStep, and we're running in an IE window 
            // we can verify that the zone we're in matches what we should be seeing.
            // This will catch lab issues where the "Internet" zone is actually being Intranet.
            // Dont do the check if we've launched preshost in debug mode, as it will get no zone.
            // Don't check on Server since the zone must be "Trusted" for HTTP Internet schemes to work in "IE Enhanced Security Mode"
            if ((this.Step.GetType() == typeof(Microsoft.Test.Loaders.Steps.ActivationStep)) &&
                 !(((ActivationStep)this.Step).PresentationHostDebugMode) &&
                 !SystemInformation.Current.IsServer)
            {
                bool needToCheckIEZone = false;
                foreach (UIHandler handler in ((ActivationStep)this.Step).UIHandlers)
                {
                    // If we're doing simple Xaml or Browser-App verification, figure out the scheme the appmonitor is using 
                    // and make sure that IE has the same value.
                    // This mitigates: 1) Lab misconfiguration that causes what we consider Internet zones to display as Local Intranet
                    //                    Caused by "Automatically detect settings" being checked.
                    //                 2) Bugs where the zone does not display for XAML or Browser apps

                    if (handler.GetType() == typeof(Microsoft.Test.Deployment.BrowserHostedApplicationVerifier))
                    {
                        needToCheckIEZone = true;
                    }
                }
                if (needToCheckIEZone && !IgnoreIEZoneString)
                {
                    runningFromExpectedZone = checkIEZone(((ActivationStep)this.Step).Scheme, topLevelhWnd);
                }
            }
            if (title.ToLowerInvariant().Contains(genericTitle) || title.ToLowerInvariant().Equals(genericTitle))
            {
                windowTitleCorrect = true;
                GlobalLog.LogEvidence("Window title is correct. (" + title + ")");
            }
            if (!windowTitleCorrect)
            {
                GlobalLog.LogEvidence("Window title is incorrect.\n");
                GlobalLog.LogEvidence("The window title should contain " + genericTitle + ".  The actual title of the window is " + title + ".");
            }

            // make sure all processes that should be running are running
            foreach (string name in ProcessesToCheck)
            {
                if (name.ToLowerInvariant().Equals("presentationhost"))
                {
                    if ((this.GetType() == typeof(FireFoxBrowserHostedApplicationVerifier)) ||
                         (this.GetType() == typeof(FireFoxXAMLVerifier)))
                    {
                        // WORKAROUND: FireFox has a random chance of starting apps with 8.3 filenames, i.e. PresentationHost can be Presen~1.
                        // If the initial process check fails, log this then try the 8.3 version.
                        bool processSuccess = machineStateVerifier.IsProcessRunning(name, _presHostInstances);

                        if (!processSuccess && name.Length > 8)
                        {
                            string shortName = name.Substring(0, 6) + "~1";
                            GlobalLog.LogEvidence("Didn't find process " + name + ", but re-trying short name (" + shortName + ") since FireFox randomly starts processes with 8.3 names");
                            processSuccess = machineStateVerifier.IsProcessRunning(shortName, _presHostInstances);
                        }
                        processesRunning = processesRunning && processSuccess;
                    }
                    else
                    {
                        processesRunning = processesRunning && machineStateVerifier.IsProcessRunning(name, _presHostInstances);
                    }
                }
                else if ((name.ToLowerInvariant().Contains("iexplore")) && // If we're looking at the IE Count
                         (Environment.OSVersion.Version.Major == 6) &&     // on Vista
                         (this.Step.GetType() == typeof(Microsoft.Test.Loaders.Steps.ActivationStep)) && // For Activation
                         (((ActivationStep)this.Step).Method == ActivationMethod.Navigate) && // Via navigation
                         (((ActivationStep)this.Step).Scheme == ActivationScheme.Local))     // Over the local file system
                {
                    // Then ignore the process count as it will vary from 1/2 based on whether we're properly running with low-rights.
                    GlobalLog.LogEvidence("Navigate from Local selected on Vista ... ignoring IE process count since this pops a new window");
                }
                else // Regular process verification, no special casing.
                {
                    if ((ApplicationDeploymentHelper.GetIEVersion() >= 8) &&
                        (name.ToLowerInvariant() == "iexplore"))
                    {
                        // LCIE (Loosely Coupled IE) causes 2 IExplore processes to exist for a single-tab single browser session
                        // May need to make this do some UIAutomation trickery to count browser tabs if this does not work for all cases.
                        GlobalLog.LogEvidence("Ignoring count of IExplore processes (LCIE can arbitrarily cause 2-3 Iexplore processes per window)");
                    }
                    else
                    {
                        processesRunning = processesRunning && machineStateVerifier.IsProcessRunning(name, 1);
                        break;
                    }
                }
            }

            // Check to see if the app is committed to the store or not 
            // (Most cases will want it to be there)
            appStoreStateCorrect = CheckAppStoreState();

            if (this.IsAppShellVisible)
            {
                startMenuItemsExist = machineStateVerifier.StartMenuShortcutsExist();

                if (startMenuItemsExist && processesRunning && windowTitleCorrect && trustDialogCorrect && appStoreStateCorrect && runningFromExpectedZone)
                {
                    GlobalLog.LogEvidence("Passed: Start menu, running processes, window title, AppStore state, and Trust Dialog behavior are correct");
                    this.TestLog.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Failed due to incorrect start menu, running processes, window title, AppStore state, or Trust Dialog behavior");
                    this.TestLog.Result = TestResult.Fail;
                }
            }
            else
            {
                if (processesRunning && windowTitleCorrect && trustDialogCorrect && appStoreStateCorrect && runningFromExpectedZone && presentationHostBitnessCorrect)
                {
                    GlobalLog.LogEvidence("Passed: Running processes, window title, AppStore state, and Trust Dialog behavior are correct");
                    this.TestLog.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Failed due to incorrect running processes, window title, AppStore state, or Trust Dialog behavior (see log!)");
                    this.TestLog.Result = TestResult.Fail;
                }
            }

            // Special case: Navigation Steps reuse the same IE window, so don't close it
            if (DictionaryStore.Current["NavigationStepRunning"] == null)
            {
                // Get the highest level window (may be same as current) and close it
                AutomationElement windowToClose = AutomationElement.FromHandle(topLevelhWnd);
                object patternObject;
                windowToClose.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
                WindowPattern wp = patternObject as WindowPattern;
                wp.Close();
                GlobalLog.LogDebug("Closed main window");
            }
            else
            {
                GlobalLog.LogDebug("Did not close main window (in navigation step)");
            }
            return UIHandlerAction.Handled;
        }
        #endregion

        #region Private Methods
        private void addBookmark(string title)
        {
            if ((this.GetType().Equals(typeof(StandAloneApplicationVerifier))) &&
                this.Step.GetType().Equals(typeof(ActivationStep)))
            {
                GlobalLog.LogDebug("Adding bookmark for standalone app scenario...");
                string shortcutFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Favorites);
                TextWriter tw = new StreamWriter(shortcutFolderPath + @"\\" + this.AppName + ".url");
                try
                {
                    tw.Write("[InternetShortcut]\nURL=" + DictionaryStore.Current["ActivationStepUri"]);
                }
                finally
                {
                    tw.Close();
                }
            }
            else
            {
                GlobalLog.LogDebug("Adding bookmark for browser-hosted content scenario...");

                if (title.IndexOf(" -") > 0)
                {
                    title = title.Remove(title.IndexOf(" -"));
                }

                string existingFavorite = Environment.GetFolderPath(Environment.SpecialFolder.Favorites) + "\\" + title + ".url";
                if (File.Exists(existingFavorite))
                {
                    GlobalLog.LogEvidence("Deleting already existing file " + existingFavorite);
                    File.Delete(existingFavorite);
                }
                else
                {
                    GlobalLog.LogEvidence("Shortcut with same name didn't already exist (" + existingFavorite + ")");
                }

                // Give it plenty of time for the title to settle... 
                Thread.Sleep(5000);

                MTI.Input.SendKeyboardInput(Key.LeftCtrl, true);
                MTI.Input.SendKeyboardInput(Key.D, true);
                MTI.Input.SendKeyboardInput(Key.LeftCtrl, false);
                MTI.Input.SendKeyboardInput(Key.D, false);

                // Click the new IE7 UI for add to favorites (it's no longer silent)
                if (ApplicationDeploymentHelper.GetIEVersion() == 7)
                {
                    Thread.Sleep(2000);
                    MTI.Input.SendKeyboardInput(Key.LeftAlt, true);
                    MTI.Input.SendKeyboardInput(Key.A, true);
                    MTI.Input.SendKeyboardInput(Key.LeftAlt, false);
                    MTI.Input.SendKeyboardInput(Key.A, false);
                }

                GlobalLog.LogEvidence("Added bookmark of this app for relaunch via favorites...");
                Thread.Sleep(750);
            }
        }

        private void TestSecurity(string zoneString, System.IntPtr windowHandle)
        {
            // if the user doesnt specify this, don't do anything.
            // This is for custom cases where verifying what app permissions were granted is necessary.
            if (zoneString == null)
                return;
            else
            {
                AutomationElement parentWindow = AutomationElement.FromHandle(windowHandle);
                PropertyCondition isTestSecButton = new PropertyCondition(AutomationElement.AutomationIdProperty, "btnSecurityTester");
                AutomationElement testSecButton = null;
                try
                {
                    testSecButton = parentWindow.FindFirst(TreeScope.Descendants, isTestSecButton);
                }
                catch (System.InvalidOperationException)
                {
                    // Do nothing.  This exception sometimes happens mysteriously on BVTs.
                }

                int triesRemaining = 10;
                while ((testSecButton == null) && (triesRemaining > 0))
                {
                    Thread.Sleep(10000);
                    triesRemaining--;
                    GlobalLog.LogDebug("Waiting to see security test button...");
                    try
                    {
                        testSecButton = parentWindow.FindFirst(TreeScope.Descendants, isTestSecButton);
                    }
                    catch (System.InvalidOperationException)
                    {
                        // Do nothing.  This exception sometimes happens mysteriously on BVTs.
                    }
                }
                if (testSecButton != null)
                {
                    object patternObject;
                    testSecButton.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                    InvokePattern ip = patternObject as InvokePattern;
                    ip.Invoke();
                    Thread.Sleep(1000);
                    string newValue = testSecButton.GetCurrentPropertyValue(AutomationElement.NameProperty) as string;

                    if (newValue.ToLowerInvariant() == zoneString.ToLowerInvariant())
                    {
                        GlobalLog.LogEvidence("(" + zoneString + ") App given correct level of trust.");
                    }
                    else // Fail case... 
                    {
                        GlobalLog.LogEvidence("App given incorrect level of trust: \n Expected: " + zoneString + ", saw " + newValue);
                        this.TestLog.Result = TestResult.Fail;
                    }
                }
                else
                {
                    GlobalLog.LogEvidence("Attempted to activate TestSecurity button and it didn't exist... failing");
                    this.TestLog.Result = TestResult.Fail;
                }
            }
        }

        private bool CheckAppStoreState()
        {
            string fusionPath = "";

            try
            {
                fusionPath = ApplicationDeploymentHelper.GetAppFusionPath(_appName);
            }
            catch (DirectoryNotFoundException)
            {
                GlobalLog.LogEvidence("App store not created,  Ignoring check store state... This should only be seen on Xaml test cases, 3.5 SP1 or later.");
                return true;
            }

            if ((AppShouldBeInStore) && (fusionPath != null))
            {
                GlobalLog.LogEvidence("App present in the store. (Expected)");
                return true;
            }
            if ((!AppShouldBeInStore) && (fusionPath == null))
            {
                GlobalLog.LogEvidence("App not present in the store. (Expected)");
                return true;
            }

            if (AppShouldBeInStore)
            {
                GlobalLog.LogEvidence("ERROR: App should be in store but was not found.");
            }
            else
            {
                GlobalLog.LogEvidence("ERROR: App should not be in store but was found.");
            }
            return false;
        }

        private static string TryLoadUnmanagedStringMultiLocale(string toTry, int StringID, int SubID)
        {
            GlobalLog.LogDebug("Trying to load resource string.  Current Culture = " + System.Globalization.CultureInfo.CurrentCulture.Name + " Current UICulture = " + System.Globalization.CultureInfo.CurrentUICulture);

            string toReturn = UnmanagedStringHelper.LoadUnmanagedResourceString(
                toTry.Replace("<locale>", System.Globalization.CultureInfo.CurrentUICulture.Name), StringID, SubID);

            if (toReturn == null)
            {
                toReturn = UnmanagedStringHelper.LoadUnmanagedResourceString(
                toTry.Replace("<locale>", "en-us"), StringID, SubID);
            }
            return toReturn;
        }

        private static bool checkIEZone(ActivationScheme scheme, IntPtr topLevelHwnd)
        {
            AutomationElement IEWindow = AutomationElement.FromHandle(topLevelHwnd);
            PropertyCondition isZonePanel;
            GlobalLog.LogDebug("Beginning IE Zone Verification");

            // Figure out if we're running on IE6, 7, or 8... throw an exception if none of the above.
            // (IE's continued lack of realAutomationIds means we'll have to keep adapting this code to every release)
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    {
                        isZonePanel = new PropertyCondition(AutomationElement.AutomationIdProperty, "StatusBar.Pane7");
                        break;
                    }
                case 7:
                    {
                        isZonePanel = new PropertyCondition(AutomationElement.AutomationIdProperty, "StatusBar.Pane9");
                        break;
                    }
                case 8:
                    {
                        isZonePanel = new PropertyCondition(AutomationElement.AutomationIdProperty, "StatusBar.Pane10");
                        break;
                    }
                // For whatever reason there's no zone panel on IE9, so just pass this part.
                case 9:
                    {
                        GlobalLog.LogDebug("No Zone panel on IE9, skipping zone check");
                        return true;
                    }
                default:
                    {
                        throw new System.NotSupportedException("Don't know how to deal with validating IE zone string for version " + ApplicationDeploymentHelper.GetIEVersion());
                    }
            }
            AutomationElement zonePanel = IEWindow.FindFirst(TreeScope.Descendants, isZonePanel);

            if (zonePanel == null)
            {
                GlobalLog.LogEvidence("Failed to find the automation element for the IE Zone indicator!");
                return false;
            }

            // Get the value from the zone bar
            object patternObject;
            if (!zonePanel.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject))
            {
                int tries = 5;
                while ((tries > 0) && !zonePanel.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject))
                {
                    tries--;
                    Thread.Sleep(200);
                }
            }
            ValuePattern vp = (ValuePattern)patternObject;
            string ExpectedZoneString;

            RegistryKey zoneNameKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones");

            string subkeyName = "";
            string unmanagedResourcePath = "";
            int StringID = 0;
            int SubID = 0;

            // Figure out what we expect the value from the zone bar to be
            switch (scheme)
            {
                case ActivationScheme.HttpInternet:
                case ActivationScheme.HttpInternetExternal:
                case ActivationScheme.HttpsInternet:
                    {
                        switch (ApplicationDeploymentHelper.GetIEVersion())
                        {
                            case 6:
                                unmanagedResourcePath = @"@%SystemRoot%\system32\inetcplc.dll";
                                StringID = 294;
                                SubID = 3;
                                break;
                            case 7:
                                unmanagedResourcePath = @"@%SystemRoot%\system32\<locale>\urlmon.dll.mui";
                                StringID = 259;
                                SubID = 1;
                                break;
                            // IE 8 and 9 are the same as 7 for now... 
                            default:
                                goto case 7;
                        }
                        subkeyName = "3";
                        break;
                    }
                case ActivationScheme.HttpIntranet:
                case ActivationScheme.HttpsIntranet:
                case ActivationScheme.Unc:
                    {
                        switch (ApplicationDeploymentHelper.GetIEVersion())
                        {
                            case 6:
                                unmanagedResourcePath = @"@%SystemRoot%\system32\inetcplc.dll";
                                StringID = 294;
                                SubID = 1;
                                break;
                            case 7:
                                unmanagedResourcePath = @"@%SystemRoot%\system32\<locale>\urlmon.dll.mui";
                                StringID = 258;
                                SubID = 15;
                                break;
                            // IE 8 and 9 are the same as 7 for now... 
                            default:
                                goto case 7;
                        }
                        subkeyName = "1";
                        break;
                    }
                case ActivationScheme.Local:
                    {
                        switch (ApplicationDeploymentHelper.GetIEVersion())
                        {
                            case 6:
                                unmanagedResourcePath = @"@%SystemRoot%\system32\inetcplc.dll";
                                StringID = 294;
                                SubID = 0;
                                break;
                            case 7:
                                unmanagedResourcePath = @"@%SystemRoot%\system32\<locale>\urlmon.dll.mui";
                                StringID = 258;
                                SubID = 14;
                                break;
                            // IE 8 and 9 are the same as 7 for now... 
                            default:
                                goto case 7;
                        }
                        subkeyName = "0";
                        break;
                    }
            }

            ExpectedZoneString = TryLoadUnmanagedStringMultiLocale(unmanagedResourcePath, StringID, SubID);

            // Fall back to this only if the above fails... its localization is VERY unreliable 
            if (ExpectedZoneString == null)
            {
                GlobalLog.LogDebug("Falling back to registry for IE zone string...");
                try
                {
                    zoneNameKey = zoneNameKey.OpenSubKey(subkeyName);
                    ExpectedZoneString = zoneNameKey.GetValue("DisplayName").ToString();
                }
                catch (System.NullReferenceException)
                {
                    GlobalLog.LogDebug("ERROR: Can't get IE Zone string from HKCU\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Zones\\" + subkeyName + "\\DisplayName");
                    ExpectedZoneString = null;
                }
            }

            if ((ExpectedZoneString == null) || (vp.Current.Value == ""))
            {
                GlobalLog.LogDebug("Zone check failed, but not failing test because this is an IE issue... please contact Microsoft to fix this");
                return true;
            }

            bool correctZone = false;
            int attemptCount = 20;

            // Workaround for new issue seen w/ Native Progress page: Stuff happens faster, so IE is ending up displaying "Unknown" as the zone by the time we check
            // This mostly happens for frame scenarios because the app cannot change to title of the window, which is used for timing elsewhere.
            // A 10-second timeout
            while ((attemptCount > 0) && (!vp.Current.Value.Trim().Contains(ExpectedZoneString)))
            {
                Thread.Sleep(500);
                attemptCount--;
            }

            // Only do substring match here because in Vista there is a new "| Protected Mode:On/Off thing that gets appended here
            if (vp.Current.Value.Trim().Contains(ExpectedZoneString))
            {
                GlobalLog.LogEvidence("Expected (and saw) " + ExpectedZoneString + " as the IE zone for this content.");
                correctZone = true;
            }
            else
            {
                GlobalLog.LogEvidence("Expected " + ExpectedZoneString + " to be the zone this came from, but saw " + vp.Current.Value);
                correctZone = false;
            }

            // Make sure the IE Gold bar is NOT showing.  This should never happen for Avalon content, even in local HTML.
            OrCondition isGoldBarDialog = new OrCondition(new PropertyCondition(AutomationElement.AutomationIdProperty, "10711"),   // AutoID in IE6
                                                          new PropertyCondition(AutomationElement.AutomationIdProperty, "37425"));  // AutoID in IE7,8 (may occasionally change)
            // Try to find the Gold bar... unfortunately if the AutomationID changes we may miss this.
            AutomationElement GoldBar = IEWindow.FindFirst(TreeScope.Descendants, isGoldBarDialog);

            if (GoldBar == null)
            {
                GlobalLog.LogEvidence("No Goldbar found in IE Window");
                return correctZone;
            }
            else
            {
                GlobalLog.LogEvidence("ERROR! IE Goldbar is present and should not be");
                // For the time being this will not fail the test, only log the presence of the gold bar.  
                // When the bar is fixed, the commented out line needs to replace this, so we can fail.

                // return false;
                return correctZone;
            }
        }
        #endregion
    }
}
