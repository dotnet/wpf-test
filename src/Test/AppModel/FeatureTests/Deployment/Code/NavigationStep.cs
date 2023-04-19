// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Deployment;
using System.Windows.Automation;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;
using System.Diagnostics;
using Microsoft.Test.Input;
using MTI = Microsoft.Test.Input;
using Microsoft.Win32;
using Microsoft.Test.CrossProcess;


namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Different methods of navigation that navigationStep can use
    /// </summary>
    public enum NavigationMethod
    {
        /// <summary>
        /// Navigate by typing the path into the IE address bar
        /// </summary>
        IENavBar,
        /// <summary>
        /// Navigate by clicking a hyperlink specified by "Filename"
        /// </summary>
        InterAppClick,
        /// <summary>
        /// Navigate by entering URL into textbox and clicking it (special case)
        /// </summary>
        InterAppSpecial,
        /// <summary>
        /// Navigate by either creating a new window or creating a new browser tab  
        /// </summary>
        IENewBrowserTab
    }

    /// <summary>
    /// Loader Step that can be used to navigate between different Avalon application types
    /// if Method is "Launch", it opens an IE window and navigates using it.
    /// if Method is "Navigate", it looks for an existing IE window and navigates using it.
    /// </summary>
    
    public class NavigationStep : LoaderStep 
    {
        #region private data
        FileHost _fileHost;
        ApplicationMonitor _appMonitor;
        UIHandler[] _uiHandlers = new UIHandler[0];
        // Keep track of whether this is a standalone app.  If a standalone app verifier is used, set it to true.
        // (Lets us turn back on window closing for this case, as a standalone app will open in a new window)
        bool _standAloneApp = false;
        bool _usingSeparateFileHost = false;
        bool _createNewBrowserTab = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for Navigation step.
        /// Puts value into property bag to prevent AppVerifier from closing windows in navigation steps.
        /// </summary>
        public NavigationStep()
        {
            DictionaryStore.Current["NavigationStepRunning"] = "true";
        }
        #endregion
        
        #region Public members

        /// <summary>
        /// Gets or sets the File that should be navigated to
        /// </summary>
        public string FileName = null;

        /// <summary>
        /// Gets or sets method to use to open file.  Navigate by default,
        /// should only be changed for the first step in a chain of NavigateSteps
        /// </summary>
        public ActivationMethod Method = ActivationMethod.Navigate;

        /// <summary>
        /// Gets or sets method to use for navigation, be it clicking links in one app to another
        /// or typing into the IE nav. bar.
        /// </summary>
        public NavigationMethod NavigationType = NavigationMethod.IENavBar;

        /// <summary>
        /// Gets or sets the Scheme to use when activating the application
        /// </summary>
        public ActivationScheme Scheme = ActivationScheme.Local;

        /// <summary>
        /// Gets or sets whether the Fusion Cache should be cleared before
        /// activating the application. Default: false.  Use with first .application
        /// in a sequence of navigations.
        /// </summary>
        public bool ClearFusionCache = false;
        
        /// <summary>
        /// Gets or sets an array of SupportFiles that should be remotely hosted
        /// </summary>
        public SupportFile[] SupportFiles = new SupportFile[0];
        
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
        /// Gets the ApplicationMonitor that is monitoring the target application
        /// </summary>
        /// <value>the ApplicationMonitor that is monitoring the target application</value>
        public ApplicationMonitor Monitor 
        {
            get { return _appMonitor; }
        }

        /// <summary>
        /// Gets or sets a boolean value representing if this is the last navigation step to run.
        /// The last navigation step in an indefinite list of these steps needs to close the final 
        /// window, thus must know this.
        /// </summary>
        public bool IsFinalStep = false;

        #endregion

        #region Step Implementation

        /// <summary>
        /// Performs the Navigation step
        /// </summary>
        /// <returns>returns true if the rest of the steps should be executed, otherwise, false</returns>
        protected override bool BeginStep() 
        {
            GlobalLog.LogDebug("Starting navigation step to : " + FileName);
            //Create ApplicationMonitor
            _appMonitor = new ApplicationMonitor();

            UploadSupportFiles();

            RegisterUIHandlers();

            string param = FileName;
            if (((Scheme != ActivationScheme.Local)&&(SupportFiles.Length > 0))  || _usingSeparateFileHost)
            {
                GlobalLog.LogDebug("FileName = " + param);
                FileHostUriScheme hostScheme = (FileHostUriScheme)Enum.Parse(typeof(FileHostUriScheme), Scheme.ToString());
                param = _fileHost.GetUri(FileName, hostScheme).AbsoluteUri;
                GlobalLog.LogDebug("Post Host update file name = " + param);
            }

            //Clear the fusion cache if requested
            if (ClearFusionCache)
                ApplicationDeploymentHelper.CleanClickOnceCache();

            // if this is the last step in the navigation sequence, or a standalone app,
            // get rid of prop bag value so that appverifier will close the window.
            if (IsFinalStep || (_standAloneApp && (NavigationType == NavigationMethod.IENavBar)))
            {
                DictionaryStore.Current["NavigationStepRunning"] = null;
            }

            //if local... need to fully qualify the path
            if (Scheme == ActivationScheme.Local)
                param = Path.GetFullPath(param);

            // Fail to IE, since it has far more tests.  
            string defaultBrowserExecutable = "iexplore.exe";

            try
            {
                defaultBrowserExecutable = Registry.GetValue(@"HKEY_CURRENT_USER\Software\Clients\StartMenuInternet", null, "iexplore.exe").ToString();
            }
            catch (Exception)
            {
                try
                {
                    defaultBrowserExecutable = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Clients\StartMenuInternet", null, "iexplore.exe").ToString();
                }
                catch (Exception)
                {
                    // Do nothing, some machines have been seen in weird states where this is undefined.  Log it anyways.
                    GlobalLog.LogDebug("Unable to get StartMenuInternet key, FireFox or other non-standard browser tests may be affected.  Contact Microsoft if this is the case");
                }
            }
            // Handle the case where this value exists but isnt set to anything usable.  IE is far more common so fall back to it.
            if (string.IsNullOrEmpty(defaultBrowserExecutable))
            {
                defaultBrowserExecutable = "iexplore.exe";
            }

            // Launch IE if we need to
            if (Method == ActivationMethod.Launch)
            {
                // start the default browser... currently just FF or IE.
                if (defaultBrowserExecutable.ToLowerInvariant().Contains("iexplore"))
                {
                    Process.Start("IEXPLORE", "about:Nothing");
                    // Let IE load up...
                    Thread.Sleep(3000);
                    // Start monitoring for registered UIHandlers
                    _appMonitor.StartMonitoring(false);
                    // Use the address bar to navigate to it, other methods wont work.
                    return NavigateUsingIEBar(param);
                }
                else if (defaultBrowserExecutable.ToLowerInvariant().Contains("firefox"))
                {
                    Process waitProcess = Process.Start("FireFox.exe");
                    IntPtr fireFoxHwnd = IntPtr.Zero;

                    // Let FF load up...
                    do
                    {
                        if (!waitProcess.HasExited)
                        {
                            fireFoxHwnd = waitProcess.MainWindowHandle;
                        }

                        Thread.Sleep(300);
                        waitProcess.Refresh();
                    }
                    while ((fireFoxHwnd.Equals(IntPtr.Zero)) && (!waitProcess.HasExited));

                    // Loop again if it exited.  Behavior is different on XP + Vista.
                    if (waitProcess.HasExited)
                    {
                        GlobalLog.LogDebug("First FireFox process exited, waiting for second to be ready...");
                        Thread.Sleep(250);
                        Process[] procs = Process.GetProcessesByName("firefox");
                        if (procs.Length == 1)
                        {
                            waitProcess = procs[0];

                            do
                            {
                                if (!waitProcess.HasExited)
                                {
                                    fireFoxHwnd = waitProcess.MainWindowHandle;
                                }

                                Thread.Sleep(300);
                                waitProcess.Refresh();
                            }
                            while (fireFoxHwnd.Equals(IntPtr.Zero));
                            GlobalLog.LogDebug("Found FireFox process with handle " + fireFoxHwnd.ToString());
                        }
                        else
                        {
                            GlobalLog.LogEvidence("Saw unexpected # of FF processes trying to navigate them.  Ignoring result...");
                            TestLog.Current.Result = TestResult.Ignore;
                            return true;
                        }
                    }

                    // Start monitoring for registered UIHandlers
                    _appMonitor.StartMonitoring(false);
                    // Use the address bar to navigate to it, other methods wont work.
                    return NavigateUsingFFBar(param, waitProcess.MainWindowHandle);
                }
                else
                {
                    throw new InvalidOperationException("Don't know how to navigate browser " + defaultBrowserExecutable);
                }
            }

            // Start monitoring for registered UIHandlers
            _appMonitor.StartMonitoring(false);

            switch (NavigationType)
            {
                case (NavigationMethod.IENavBar):
                    {
                        if (defaultBrowserExecutable.ToLowerInvariant().Contains("iexplore"))
                        {
                            return NavigateUsingIEBar(param);
                        }
                        else if (defaultBrowserExecutable.ToLowerInvariant().Contains("firefox"))
                        {
                            return NavigateUsingFFBar(param);
                        }
                        else
                        {
                            GlobalLog.LogEvidence("ERROR: Hit unknown browser " + defaultBrowserExecutable);
                            return false;
                        }
                    }
                case (NavigationMethod.InterAppClick):
                    {
                        bool result = NavigateUsingInterAppClick(FileName);
                        DictionaryStore.Current["NavigationStepRunning"] = "true";
                        return result;
                    }                    
                case (NavigationMethod.InterAppSpecial):
                    {
                        bool result = NavigateUsingInterApp(param);
                        DictionaryStore.Current["NavigationStepRunning"] = "true";
                        return result;
                    }
                case (NavigationMethod.IENewBrowserTab):
                    {
                        _createNewBrowserTab = true;
                        bool result = NavigateUsingIEBar(param);
                        DictionaryStore.Current["NavigationStepRunning"] = "true";
                        _createNewBrowserTab = false;
                        return result;
                    }                    
            }
            return true;
        }

        /// <summary>
        /// Waits for the ApplicationMonitor to Abort and Closes any remaining
        /// processes
        /// </summary>
        /// <returns>true</returns>
        protected override bool EndStep() {
            //Wait for the application to be done
            _appMonitor.WaitForUIHandlerAbort();

            if (_standAloneApp && (NavigationType == NavigationMethod.IENavBar))
            {
                DictionaryStore.Current["NavigationStepRunning"] = "true";
            }

            if (IsFinalStep)
            {
                if (_standAloneApp)
                {
                    killAllIEWindows();
                }
                _appMonitor.Close();
            }
            else
            {
                _appMonitor.StopMonitoring();
            }

            // close the fileHost if one was created within ActivationStep. 
            // Don't close if the filehost is in the context of a FileHostStep
            if ((_fileHost != null) &&  (SupportFiles.Length > 0))
                _fileHost.Close();

            return true;
        }
        #endregion

        #region Private Methods

        private void killAllIEWindows()
        {
            GlobalLog.LogDebug("Killing IE windows (navigation ending with standalone app)");
            Process[] procList = Process.GetProcesses();
            foreach (Process p in procList)
            {
                if (p.ProcessName.ToLowerInvariant().Equals("iexplore"))
                {
                    p.CloseMainWindow();
                }
            }
        }

        private void UploadSupportFiles()
        {
            if (Scheme != ActivationScheme.Local)
            {
                if (SupportFiles.Length > 0)
                {
                    _fileHost = new FileHost();
                    foreach (SupportFile suppFile in SupportFiles)
                    {
                        if (suppFile.IncludeDependencies)
                            _fileHost.UploadFileWithDependencies(suppFile.Name);
                        else
                            _fileHost.UploadFile(suppFile.Name);
                    }
                }
                else
                {
                    LoaderStep parent = this.ParentStep;
                    _usingSeparateFileHost = true;

                    while (parent != null)
                    {
                        if (parent.GetType() == typeof(Microsoft.Test.Loaders.Steps.FileHostStep))
                        {
                            this._fileHost = ((FileHostStep)parent).fileHost;
                            break;
                        }
                        // Failed to find it in the immediate parent: try til we hit null or the right one
                        parent = parent.ParentStep;
                    }
                }
            }
        }
        private void RegisterUIHandlers()
        {
            foreach (UIHandler handler in UIHandlers)
            {
                _standAloneApp = _standAloneApp || (handler.GetType() == typeof(Microsoft.Test.Deployment.StandAloneApplicationVerifier));

                if (handler.NamedRegistration != null)
                    _appMonitor.RegisterUIHandler(handler, handler.NamedRegistration, handler.Notification);
                else
                    _appMonitor.RegisterUIHandler(handler, handler.ProcessName, handler.WindowTitle, handler.Notification);
            }
        }

        // Import method to figure out current keyboard layout... need to hit enter twice for IME languages.
        [DllImport("user32.dll")]
        private static extern IntPtr GetKeyboardLayout(int idThread);

        // Case where we don't know the HWnd already
        private bool NavigateUsingFFBar(string param)
        {
            PropertyCondition isFireFoxWindow = new PropertyCondition(AutomationElement.ClassNameProperty, "MozillaUIWindowClass");
            AutomationElement fireFoxWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children, isFireFoxWindow);
            return NavigateUsingFFBar(param, (new IntPtr((int)fireFoxWindow.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty, true))));
        }

        private bool NavigateUsingFFBar(string param, IntPtr fireFoxHwnd)
        {
            FireFoxAutomationHelper.NavigateFireFox(fireFoxHwnd, param);
            return true;
        }

        private bool NavigateUsingIEBar(string param)
        {
            PropertyCondition isIE7Window = new PropertyCondition(AutomationElement.ClassNameProperty, "BrowserFrameClass");
            PropertyCondition isIE6Window = new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame");

            OrCondition isOneOfTheWindows = new OrCondition(isIE6Window, isIE7Window);

            AutomationElement IEWindow = AutomationElement.RootElement.FindFirst(TreeScope.Children, isOneOfTheWindows);

            if (IEWindow == null)
            {
                GlobalLog.LogDebug("Couldn't find an IE Window to navigate");
                return false;
            }

            // Invoking this button currently automatically focuses the address bar in IE7...
            // so if we're doing that, we don't have to click there the first time.
            if (_createNewBrowserTab)
            {
                if (ApplicationDeploymentHelper.GetIEVersion() < 7)
                {
                    GlobalLog.LogEvidence("Setting PASS and returning... : this test only works on IE7 or greater");
                    TestLog.Current.Result = TestResult.Pass;
                    ApplicationMonitor.NotifyStopMonitoring();
                    return true ;
                }

                // Find the part of the window with all the tabs ... 
                PropertyCondition isTabBand = new PropertyCondition(AutomationElement.ClassNameProperty, "DirectUIHWND");
                AutomationElement tabBand = IEWindow.FindFirst(TreeScope.Descendants, isTabBand);

                // Get all the buttons from here... 
                PropertyCondition isBtn = new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button);
                AutomationElementCollection aec = tabBand.FindAll(TreeScope.Descendants, isBtn);

                // Get the last element in the collection
                IEnumerator tabEnum = aec.GetEnumerator();
                AutomationElement newTabBtn = null;
                while (tabEnum.MoveNext() == true)
                {
                    newTabBtn = tabEnum.Current as AutomationElement;
                }

                try
                {
                    // ... and click it (Cos it's the tab button)
                    InvokePattern ip = newTabBtn.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    ip.Invoke();
                    // Wait a bit before letting other UIHandlers at this.
                    Thread.Sleep(3000);
                }
                catch (Exception e)
                {
                    GlobalLog.LogDebug("Hit exception trying to hit new tab in IE:");
                    GlobalLog.LogDebug(e.StackTrace.ToString());
                    return false;
                }
            }

            AndCondition isAddrBar = IEAutomationHelper.GetIEAddressBarAndCondition();
            AutomationElement address = IEWindow.FindFirst(TreeScope.Descendants, isAddrBar);

            if (address != null)
            {
                // Double Click the addr. bar to guarantee (hopefully) focus...
                IEAutomationHelper.ClickCenterOfAutomationElement(address);
                IEAutomationHelper.ClickCenterOfAutomationElement(address);
                
                //Sleep a second to make sure that the address bar has focus
                Thread.Sleep(2000);

                ValuePattern vp = address.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
                if (vp == null)
                {
                    throw new System.Exception("Couldn't get the valuePattern for the IE address bar! Test will fail until this is fixed.");
                }
                vp.SetValue(param.ToString());

                MTI.Input.SendKeyboardInput(Key.Enter, true);
                MTI.Input.SendKeyboardInput(Key.Enter, false);

                // For IME enabled OS'es like Japanese.  Need to hit enter twice if we're in one.
                if ((GetKeyboardLayout(0).ToInt32() & 0xffff) == 0x0411)
                {
                    MTI.Input.SendKeyboardInput(Key.Enter, true);
                    MTI.Input.SendKeyboardInput(Key.Enter, false);
                    GlobalLog.LogDebug("Hit 2nd Return for IME-enabled OS... ");
                }

                if ((Method == ActivationMethod.Launch) && _standAloneApp)
                {
                    object patternObject;
                    IEWindow.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
                    WindowPattern wp = patternObject as WindowPattern;
                    WaitNavigationWindowCreated();
                    wp.Close();
                }
            }
            else
            {
                GlobalLog.LogDebug("Couldn't find the IE address bar to navigate");
                return false;
            }
            return true;
        }

        private bool NavigateUsingInterApp(string param)
        {
            PropertyCondition isURLTextBox = new PropertyCondition(AutomationElement.AutomationIdProperty, "theURL");
            AutomationElement URLBox = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isURLTextBox);

            AutomationElement BHNavBtn = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "bhNavBtn"));
            AutomationElement SANavBtn = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.AutomationIdProperty, "saNavBtn"));

            
            bool newWindowCreated = (Path.GetExtension(param) == ApplicationDeploymentHelper.STANDALONE_APPLICATION_EXTENSION);

            // Type in the new URL to navigate to...
            URLBox.SetFocus();
            Thread.Sleep(300);
            MTI.Input.SendUnicodeString(param.ToLowerInvariant(), 1, 20);
            Thread.Sleep(200);

            GlobalLog.LogDebug("Typed " + param + " into the app's nav text box...");

            if ((BHNavBtn == null) && SANavBtn != null)
            // We're navigating within a standalone app.  That means, additionally, we will want to
            // 1) Close this app window when we leave (no matter what)
            {
                AutomationElement standAloneWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "NavigationWindow"));

                if (standAloneWindow == null)
                {
                    GlobalLog.LogDebug("Standalone window was null... problems with UIA");
                }
                if (newWindowCreated)
                {
                    MTI.Input.MoveToAndClick(SANavBtn);
                    WindowPattern wp = WaitNewNavigationWindowCreated(standAloneWindow);
                    if (wp != null)
                    {
                        Thread.Sleep(300);
                        wp.Close();
                    }
                }
                else
                {

                    if (standAloneWindow != null)
                    {
                        object patternObject;
                        standAloneWindow.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
                        WindowPattern wp = patternObject as WindowPattern;
                        MTI.Input.MoveToAndClick(SANavBtn);
                        WaitIEWindowCreated();
                        wp.Close();
                    }
                    else
                    {
                        GlobalLog.LogDebug("Couldnt find the standalone app window...");
                    }
                }
            }
            else
            // We're navigating within an Express app.  That means, additionally, we will want to
            // 1) Close the IE window IFF a new window gets created.  This only happens for standalone apps.
            {

                if (newWindowCreated)
                {
                    // Get a reference to the old window
                    AutomationElement IEWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame"));
                    // and close it.
                    object patternObject;
                    IEWindow.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
                    WindowPattern wp = patternObject as WindowPattern;

                    // Special case for Longhorn IE, which has different class name
					if (wp == null)
					{
						// Get a reference to the old window IN LONGHORN
						IEWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "BrowserFrameClass"));
						// and close it.
						object _patternObject;
						IEWindow.TryGetCurrentPattern(WindowPattern.Pattern, out _patternObject);
						wp = _patternObject as WindowPattern;
					}

                    MTI.Input.MoveToAndClick(BHNavBtn);
                    // Give the other app plenty of time to finish loading...
                    WaitNavigationWindowCreated();
                    wp.Close();
                }
                else
                {
                    MTI.Input.MoveToAndClick(BHNavBtn);
                }
            }
            return true;
        }

        private void WaitIEWindowCreated()
        {
            // Loop waiting for a new IE Window to be created.
            // If an exception is thrown we know the window has already been closed...
            // try/catch loop ----s exceptions as exception only hit when what we want is already done.
            // This case is for detecting and timing standalone -> non-standalone navigation.
            try
            {
                AutomationElement newIEWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame"));

                while (newIEWindow == null)
                {
                    Thread.Sleep(500);
                    newIEWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame"));
                }
            }
            catch
            {
                // Do nothing
            }
        }

        private void WaitNavigationWindowCreated()
        {
            // Loop waiting for a new NavigationWindow (root element of standalone apps)
            // If an exception is thrown we know the window has already been closed...
            // try/catch loop ----s exceptions as exception only hit when what we want is already done.
            // This case is for detecting and timing non-standalone -> standalone navigation.
            try
            {
                AutomationElement standAloneWindow;
                standAloneWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "NavigationWindow"));

                while (standAloneWindow == null)
                {
                    Thread.Sleep(5000);
                    standAloneWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "NavigationWindow"));
                }
            }
            catch 
            {
              // Do nothing
            }
        }
        private WindowPattern WaitNewNavigationWindowCreated(AutomationElement currentWindow)
        {
            // Loop waiting for a new NavigationWindow (root element of standalone apps)
            // If an exception is thrown we know the window has already been closed...
            // try/catch loop ----s exceptions as exception only hit when what we want is already done.
            // This case makes sure the runtime ID of the element found is NOT that of the automationElement passed in to method.
            try
            {
                AutomationElementCollection standAloneWindows;
                standAloneWindows = AutomationElement.RootElement.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "NavigationWindow"));

                while (standAloneWindows.Count < 2) 
                {
                    Thread.Sleep(2000);
                    standAloneWindows = AutomationElement.RootElement.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ClassNameProperty, "NavigationWindow"));
                }

                foreach (AutomationElement ae in standAloneWindows)
                {
                    if (  (((int[])(ae.GetCurrentPropertyValue(AutomationElement.RuntimeIdProperty)))[1]) ==
                        ((int[])(currentWindow.GetCurrentPropertyValue(AutomationElement.RuntimeIdProperty)))[1])
                    {
                        object _patternObject;
                        ae.TryGetCurrentPattern(WindowPattern.Pattern, out _patternObject);
                        return _patternObject as WindowPattern;
                    }
                }
            }
            catch 
            {
                
            }
            return null;
        }
        private bool NavigateUsingInterAppClick(string param)
        {
            // Tab through an HTML document and click when what's listed in param is a substring of the bottom left text.
            // UIAutomation doesnt currently allow seeing HTML hyperlinks in the automation.
            // BUT... You can see the pane that displays the link.  Thus I set focus on the window,
            // and hit Tab until the right link is selected.  Then Enter is pressed to navigate to the link.

            bool newWindowCreated = (Path.GetExtension(param) == ApplicationDeploymentHelper.STANDALONE_APPLICATION_EXTENSION);

            string lookingToClick = param.ToLowerInvariant();

            PropertyCondition isIE7Window = new PropertyCondition(AutomationElement.ClassNameProperty, "BrowserFrameClass");
            PropertyCondition isIE6Window = new PropertyCondition(AutomationElement.ClassNameProperty, "IEFrame");

            OrCondition isOneOfTheWindows = new OrCondition(isIE6Window, isIE7Window);

            AutomationElement IEWindow = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, isOneOfTheWindows);
            AndCondition isAddrBar = IEAutomationHelper.GetIEAddressBarAndCondition();
            AutomationElement address = IEWindow.FindFirst(TreeScope.Descendants, isAddrBar);

            address.SetFocus();
            PropertyCondition isTheLink = new PropertyCondition(AutomationElement.AutomationIdProperty, "StatusBar.Pane0");
            AutomationElement theLink = IEWindow.FindFirst(TreeScope.Descendants, isTheLink);

            if (theLink != null)
            {
                object patternObject;
                theLink.TryGetCurrentPattern(ValuePattern.Pattern, out patternObject);
                ValuePattern vp = (ValuePattern)patternObject;

                int numTries = 0;
                while (!(vp.Current.Value.ToLowerInvariant().Contains(lookingToClick)) && numTries < 50)
                {
                    MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Tab, true);
                    Thread.Sleep(300);
                    object _patternObject;
                    theLink.TryGetCurrentPattern(ValuePattern.Pattern, out _patternObject);
                    vp = (ValuePattern)_patternObject;
                    numTries++;
                }
                if (numTries < 50)
                {
                    GlobalLog.LogEvidence("Found a link containing " + lookingToClick + ", clicking... ");
                    if (newWindowCreated)
                    {
                        patternObject = null;
                        IEWindow.TryGetCurrentPattern(WindowPattern.Pattern, out patternObject);
                        WindowPattern wp = patternObject as WindowPattern;
                        MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Enter, true);
                        // Give the other app plenty of time to finish loading...
                        Thread.Sleep(12000);
                        try
                        {
                            wp.Close();
                        }
                        catch (System.Windows.Automation.ElementNotAvailableException)
                        {
                            // Do nothing... this sometimes happens at the end of the test.
                        }
                    }
                    else
                        MTI.Input.SendKeyboardInput(System.Windows.Input.Key.Enter, true);
                }
                else
                {
                    GlobalLog.LogDebug("Failed to find a link containing " + lookingToClick + "\nAborting...");
                    return false;
                }
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Failed to find the status bar pane in IE... Aborting... ");
                return false;
            }
        }


        #endregion
    }

    public class ServerTokenReplacementStep : LoaderStep
    {
        #region Public Members

        /// <summary>
        /// Text-based file to replace tokens inside
        /// </summary>
        public string FileToModify = "";

        /// <summary>
        /// File to be used for getting the URI scheme.  Can be anything that has been uploaded to the filehost.
        /// </summary>
        public string TargetFile = "";

        /// <summary>
        /// Whether to upload to the filehost used or not.  Used for "Local / HTTP mixed content" scenarios
        /// </summary>
        public bool UploadFileAfterModify = false;

        #endregion

        public override bool DoStep()
        {

            LoaderStep fileHostStep = this;

            while ((fileHostStep.GetType() != typeof(FileHostStep)) && fileHostStep.ParentStep != null)
            {
                fileHostStep = fileHostStep.ParentStep;
            }
            if ((fileHostStep == null) || (fileHostStep.GetType() != typeof(FileHostStep)))
            {
                throw new InvalidOperationException("ServerTokenReplacementStep must be run inside a FileHostStep!");
            }

            FileHost fh = ((FileHostStep)fileHostStep).fileHost;

            string fileContents = File.ReadAllText(FileToModify);

            for (FileHostUriScheme scheme = FileHostUriScheme.Local; scheme <= FileHostUriScheme.HttpsInternet; scheme++)
            {
                if (fileContents.Contains("[FileHost:" + scheme.ToString() + "]"))
                {
                    string replacementUri = fh.GetUri(TargetFile, scheme).ToString();
                    if (scheme == FileHostUriScheme.Local)
                    {
                        replacementUri = replacementUri.Substring(0, replacementUri.LastIndexOf("\\"));
                    }
                    else
                    {
                        replacementUri = replacementUri.Substring(0, replacementUri.LastIndexOf("/"));
                    }

                    fileContents = fileContents.Replace("[FileHost:" + scheme.ToString() + "]", replacementUri);
                }
            }

            File.WriteAllText(FileToModify, fileContents);

            if (UploadFileAfterModify)
            {
                fh.UploadFile(FileToModify);
            }
            else
            {
                GlobalLog.LogDebug("Not uploading " + FileToModify + " since this was not specified.");
            }

            return true;
        }
    }
}
