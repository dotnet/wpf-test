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
using System.Net;

namespace Microsoft.Test.Deployment
{

    /// <summary>
    /// Relaunches an installed .application, then verifies it is not in ClickOnceCache.  
    /// Needs to be run in the context of a TestLogStep
    /// </summary>

    public class RelaunchStep : AppMaintenanceStep
    {
        #region private data
        ApplicationMonitor _appMonitor;
        TestLog _testLog;
        UIHandler[] _uiHandlers = new UIHandler[0];
        private static readonly string s_MY_COMPUTER_NAME = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\CLSID\\{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
        #endregion

        #region public members
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
        /// String representing method used to relaunch app.  
        /// Can be via Shortcut, Favorites, or History
        /// Default: Shortcut
        /// </summary>
        public string RelaunchMethod = "Shortcut";

        /// <summary>
        /// Argument to pass to IE when starting it for relaunch via history / favorites.  Can be used to launch xaml / xbap before the navigation to history so as to validate that this works with Avalon content showing.
        /// </summary>
        public string IERelaunchArgument = "";

        /// <summary>
        /// Gets or sets the Scheme to when activating the application
        /// </summary>
        public ActivationScheme Scheme = ActivationScheme.Local;

        /// <summary>
        /// Gets the ApplicationMonitor that is monitoring the target application
        /// </summary>
        /// <value>the ApplicationMonitor that is monitoring the target application</value>
        public ApplicationMonitor Monitor
        {
            get { return _appMonitor; }
        }
        #endregion

        #region Step Implementation
        /// <summary>
        /// Invokes app (assumed to be installed and shell-visible), then checks property bag for value left by 
        /// TrustManager dialog handler to see if the trustmanager ran more than once.
        /// </summary>
        /// <returns>true</returns>
        protected override bool BeginStep()
        {
            // Get current Test Log
            this._testLog = TestLog.Current;
            if (this._testLog == null)
            {
                throw new InvalidOperationException("Relaunch step must be created in the context of a test log");
            }
            //Create ApplicationMonitor
            _appMonitor = new ApplicationMonitor();

            // register UIHandlers (for app type being verified)
            foreach (UIHandler handler in UIHandlers)
            {
                if (handler.NamedRegistration != null)
                    _appMonitor.RegisterUIHandler(handler, handler.NamedRegistration, handler.Notification);
                else
                    _appMonitor.RegisterUIHandler(handler, handler.ProcessName, handler.WindowTitle, handler.Notification);
            }

            LoaderStep parent = this.ParentStep;
            FileHostStep parentFileHost = null;

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
                bool fileInHost = false;

                // Only allow this if the file is already in the FileHostStep, since this will throw otherwise.
                foreach (SupportFile sf in parentFileHost.SupportFiles)
                {
                    if (sf.Name.ToLowerInvariant() == IERelaunchArgument.ToLowerInvariant())
                    {
                        fileInHost = true;
                    }
                }

                if (fileInHost)
                {
                    IERelaunchArgument = parentFileHost.fileHost.GetUri(IERelaunchArgument, (FileHostUriScheme)Enum.Parse(typeof(FileHostUriScheme), Scheme.ToString())).ToString();
                    GlobalLog.LogDebug("Relaunching IE with argument \"" + IERelaunchArgument + "\"");
                }
            }

            switch (RelaunchMethod.ToLowerInvariant())
            {
                case "shortcut":
                    return LaunchViaShortcut();
                case "favorites":
                    return LaunchViaFavorites();
                case "history":
                    return LaunchViaHistory();
                default:
                    GlobalLog.LogEvidence("Attempted to use relaunch step with unrecognized method: " + RelaunchMethod);
                    return false;
            }
        }

        /// <summary>
        /// Checks to see how many times the trustmanager dialog was handled
        /// </summary>
        /// <returns>true</returns>
        protected override bool EndStep()
        {
            GlobalLog.LogDebug("Waiting for UI Handler Abort signal(s)");
            //Wait for the application to be done
            _appMonitor.WaitForUIHandlerAbort();
            _appMonitor.Close();
            return true;
        }
        #endregion

        #region Private Methods

        private bool LaunchViaShortcut()
        {
            // construct the path to the app's shortcut
            string appShortcutPath = ApplicationDeploymentHelper.GetAppShortcutPath(AppName);
            GlobalLog.LogDebug("Launching appmonitor with shortcut path:");
            GlobalLog.LogDebug(appShortcutPath);
            try
            {
                _appMonitor.StartProcess(appShortcutPath);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                GlobalLog.LogEvidence("Application shortcut not present to relaunch - failed to install properly");
                this._testLog.Result = TestResult.Fail;
                return false;
            }
            return true;
        }

        private bool LaunchViaFavorites()
        {
            GlobalLog.LogEvidence("Starting IE...");
            // Issue : IE7 isn't in the PATH so iexplore doesnt work, and now -nohome doesnt launch a browser.
            // Working around for time being because this doesnt really matter
            _appMonitor.StartProcess(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + @"\Internet Explorer\IEXPLORE.exe");

            Thread.Sleep(5000);
            PropertyCondition isIEWindow = new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame");
            AutomationElement IEWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isIEWindow);

            // Expand by the correct name based on the locale
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    AutomationElement ie6FavButton = IEWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\shdoclc.dll", 188, 12)));
                    InvokePattern ip = ie6FavButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    ip.Invoke();
                    break;
                case 7:
                    //expandByName(historyPane, UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\" + CultureInfo.CurrentUICulture.ToString() + @"\IEFRAME.dll.mui", 18, 7));
                    AutomationElement favCenter = showHistoryWindow();
                    // Click the "Favorites" button within Favorites center 
                    AutomationElement ie7FavButton = IEWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\SHELL32.dll", 794, 5)));
                    MTI.Input.MoveToAndClick(ie7FavButton);
                    //InvokePattern ip2 = ie7FavButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    //ip2.Invoke();
                    break;
                default:
                    goto case 7;
            }

            Thread.Sleep(1000);

            GlobalLog.LogDebug("Opened the favorites menu, trying to click on " + AppName);
            PropertyCondition isMyBookmark = new PropertyCondition(AutomationElement.NameProperty, AppName);
            AutomationElement myBookmark = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isMyBookmark);
            if (myBookmark != null)
            {
                GlobalLog.LogEvidence("Found favorites entry for app. Clicking...");
                MTI.Input.MoveToAndClick(myBookmark);
                Thread.Sleep(500);
                string favoritePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Favorites) + "\\" + AppName + ".url";
                if (File.Exists(favoritePath))
                    File.Delete(favoritePath);
                else
                    GlobalLog.LogDebug("Unable to delete shortcut after clicking it: \n " + favoritePath);
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Unable to find app's entry in shortcut.");
                return false;
            }
        }

        private bool LaunchViaHistory()
        {
            bool isStandAloneApp = false;
            foreach (UIHandler handler in UIHandlers)
            {
                isStandAloneApp = isStandAloneApp || (handler.GetType() == typeof(Microsoft.Test.Deployment.StandAloneApplicationVerifier));
            }

            GlobalLog.LogEvidence("Starting IE...");
            if (IERelaunchArgument == "")
            {
                _appMonitor.StartProcess("IEXPLORE", "about:Nothing");
            }
            else
            {
                if ((IERelaunchArgument.Length > 8) && (IERelaunchArgument.ToLowerInvariant().StartsWith("samedir:")))
                {
                    IERelaunchArgument = IERelaunchArgument.Replace("samedir:", "");
                    IERelaunchArgument = Directory.GetCurrentDirectory() + "\\" + IERelaunchArgument;
                }

                _appMonitor.StartProcess("IEXPLORE", IERelaunchArgument);
            }
            Thread.Sleep(6000);

            AutomationElement historyPane = showHistoryWindow();
            if (historyPane == null)
            {
                return false;
            }

            string serverString = GetServerString(isStandAloneApp);

            GlobalLog.LogEvidence("Clicking on server " + serverString + " in History...");

            // Expand by the correct name based on the locale
            switch (ApplicationDeploymentHelper.GetIEVersion())
            {
                case 6:
                    expandByName(historyPane, UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\shlwapi.dll", 18, 7));
                    break;
                case 7:
                    {
                        string toExpand = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\" + CultureInfo.CurrentUICulture.ToString() + @"\IEFRAME.dll.mui", 18, 7);
                        // Second chance... fall back to english directory
                        if (toExpand == null)
                        {
                            toExpand = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\en-us\IEFRAME.dll.mui", 18, 7);
                        }
                        expandByName(historyPane, toExpand);
                        break;
                    }
                default:
                    goto case 7;
            }

            expandByName(historyPane, serverString);

            GlobalLog.LogDebug("Looking for links to " + AppName + " in the History folder...");

            string[] appLinkNames = AppName.Split('|');

            AutomationElementCollection appLinks = null;

            foreach (string appLinkName in appLinkNames)
            {
                GlobalLog.LogDebug("Looking for History link with name: " + appLinkName);
                Thread.Sleep(1200);
                appLinks = historyPane.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, appLinkName));
                if (appLinks.Count == 0)
                {
                    string appLinkNameWithExt = appLinkName;
                    // Some versions of windows show the extension by default. Some Dont.  Instead of switching by version, just try both.
                    Thread.Sleep(1500);
                    if (isStandAloneApp)
                    {
                        appLinkNameWithExt += ApplicationDeploymentHelper.STANDALONE_APPLICATION_EXTENSION;
                    }
                    else
                    {
                        appLinkNameWithExt += ApplicationDeploymentHelper.BROWSER_APPLICATION_EXTENSION;
                    }
                    appLinks = historyPane.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, appLinkNameWithExt));
                }
                // if we found an appropriate link, stop ... 
                if (appLinks.Count > 0)
                {
                    break;
                }
            }

            GlobalLog.LogEvidence("Found " + appLinks.Count.ToString() + " entries matching " + AppName + ". Clicking...");

            if (appLinks.Count > 0)
            {
                AutomationElement lastEntry = appLinks[appLinks.Count - 1];
                MTI.Input.MoveToAndClick(lastEntry);
            }
            else
            {
                GlobalLog.LogEvidence("Couldnt find an entry for this app in the History...");
                return false;
            }

            // Put the history pane away...
            showHistoryWindow(true);
            Thread.Sleep(200);
            return true;
        }

        private void expandByName(AutomationElement someElement, string itemToExpand)
        {
            Thread.Sleep(1000);
            AutomationElement itemLink = someElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, itemToExpand));
            if (itemLink != null)
            {
                object patternObject;
                itemLink.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out patternObject);
                ExpandCollapsePattern ecp = patternObject as ExpandCollapsePattern;
                ecp.Expand();
            }
            else
            {
                GlobalLog.LogDebug("Automation attempted to but failed to expand item: " + itemToExpand);
            }
        }

        private string GetServerString(bool isStandAloneApp)
        {
            string serverString = "";
            if ((this.Scheme == ActivationScheme.HttpsInternet) ||
                (this.Scheme == ActivationScheme.HttpsIntranet))
            {
                if (this.ParentStep.GetType() == typeof(Microsoft.Test.Loaders.Steps.FileHostStep))
                {
                    FileHostStep fhs = this.ParentStep as FileHostStep;
                    serverString = fhs.fileHost.GetUri(fhs.SupportFiles[0].Name, FileHostUriScheme.Unc).ToString();
                }
                else // Hard coded in case upper part fails... 
                {
                    serverString = @"\\wpf\";
                }
            }
            else
            {
                serverString = DriverState.DriverParameters["TestScratch_RW"];
            }

            if (Scheme == ActivationScheme.Local)
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(s_MY_COMPUTER_NAME);
                if (key == null)
                {
                    // On test machines, this value can be null.
                    // If so, machine name defaults to being "Computer" or "My Computer",
                    // based on the version.  
                    switch (Environment.OSVersion.Version.Major)
                    {
                        case 5:
                            serverString = "My Computer";
                            break;
                        case 6:
                            serverString = "Computer";
                            break;
                    }
                }
                else
                {
                    serverString = (string)key.GetValue(null);
                }
            }
            else if (serverString == null)
            {
                serverString = "wpf";
            }
            else
            {
                Uri tempURI = new Uri(serverString);
                serverString = tempURI.Host;
            }
            if ((Scheme == ActivationScheme.HttpsInternet) || (Scheme == ActivationScheme.HttpInternet))
            {
                serverString = Dns.GetHostEntry(serverString).HostName.Replace(".com", "") + " (" + Dns.GetHostEntry(serverString).HostName + ")";
            }
            return serverString;
        }

        private AutomationElement showHistoryWindow()
        {
            return showHistoryWindow(false);
        }

        private AutomationElement showHistoryWindow(bool hide)
        {
            try
            {
                // Do nothing for IE7 as it will auto-hide
                if (hide && (ApplicationDeploymentHelper.GetIEVersion() == 7))
                {
                    return null;
                }

                PropertyCondition isIEWindow = new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame");
                AutomationElement ieWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children, isIEWindow);

                GlobalLog.LogDebug("Found IE window...");

                // Check to see if history pane is visible
                PropertyCondition historyVisible = new PropertyCondition(AutomationElement.AutomationIdProperty, "StartMenu");

                if (ApplicationDeploymentHelper.GetIEVersion() == 7)
                {
                    historyVisible = new PropertyCondition(AutomationElement.ClassNameProperty, "Links Explorer");
                }

                AutomationElement historyPane = ieWindow.FindFirst(TreeScope.Descendants, historyVisible);
                string IEHistWindowButtonName = "";
                switch (ApplicationDeploymentHelper.GetIEVersion())
                {
                    case 6:
                        IEHistWindowButtonName = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\shdoclc.dll", 188, 13);
                        break;
                    case 7:
                        {
                            IEHistWindowButtonName = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\" + CultureInfo.CurrentUICulture.ToString() + @"\IEFRAME.dll.mui", 1105, 10);
                            // Second chance... load it from english since IE may or may not be localized.
                            if (IEHistWindowButtonName == null)
                            {
                                IEHistWindowButtonName = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\en-us\IEFRAME.dll.mui", 1105, 10);
                            }
                            break;
                        }
                    default:
                        goto case 7;
                }

                GlobalLog.LogDebug("Attempting to click History button (" + IEHistWindowButtonName + ")");
                AutomationElement histButton = ieWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, IEHistWindowButtonName));
                InvokePattern ip = histButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                GlobalLog.LogDebug("Got invoke pattern from History button...");
                string historyButtonName = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\" + CultureInfo.CurrentUICulture.ToString() + @"\IEFRAME.dll.mui", 788, 11);
                // Second chance... fall back to english
                if (historyButtonName == null)
                {
                    historyButtonName = UnmanagedStringHelper.LoadUnmanagedResourceString(@"@%SystemRoot%\system32\en-us\IEFRAME.dll.mui", 788, 11);
                }
                GlobalLog.LogDebug("IE7 History Button name to use: " + historyButtonName);

                // if it isnt, show it and get a reference to it
                int timesTried = 0;
                while ((((historyPane == null) && !hide) ||
                        ((historyPane != null) && hide))
                        && (timesTried < 30))
                {
					// ip.Invoke();
                    MTI.Input.MoveToAndClick(histButton);
                    Thread.Sleep(1000);
                    historyPane = ieWindow.FindFirst(TreeScope.Descendants, historyVisible);

                    // Click the "History" button within Favorites center if we're in IE7
                    if ((ApplicationDeploymentHelper.GetIEVersion() == 7) && historyPane != null)
                    {
                        AutomationElement histButton2 = ieWindow.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, historyButtonName));
                        MTI.Input.MoveToAndClick(histButton2);
                        //InvokePattern ip2 = histButton2.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                        //ip2.Invoke();                    
                    }
                    timesTried++;
                }

                if ((historyPane == null) && !hide)
                {
                    GlobalLog.LogEvidence("Could not display the history pane. ");
                    return null;
                }
                return historyPane;
            }
            catch
            {
                GlobalLog.LogDebug("Hit exception when manipulating IE hist window... ignoring as this may still be fine");
                return null;
            }
        }

        #endregion
    }

}
