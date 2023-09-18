// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides types to implement test cases.

namespace Test.Uis.TestTypes
{
    #region Namespaces.

    using System;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.KoKoMo;
    using Microsoft.Test.Loaders;
    using Microsoft.Test.Logging;
    using Test.Uis.IO;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>Provides a base class for test cases.</summary>
    /// <remarks>
    /// <p>
    /// Using CustomTestCase enables the tester to quickly adapt
    /// test cases to run under a variety of execution environments.
    /// </p><p>
    /// The following methods can be overriden to control the
    /// application behavior.
    /// </p><p>
    /// <b>DoCreated</b> is executed right after the main window has
    /// been created. This is only executed if the application is running
    /// stand-alone, i.e., if it's not run in a container. Here you can
    /// create the Avalon tree that would have otherwise been built
    /// by reading a .xaml file.
    /// </p><p>
    /// <b>DoLoadCompleted</b> is run when the main window has
    /// navigated to its contents. Here the test case has access to all
    /// the elements loaded form a .xaml file. Note that if multiple files
    /// are loaded, this fires multiple times.
    /// </p><p>
    /// <b>RunTestCase</b> is executed to run the
    /// code that interacts with the application window and contains
    /// the main logic for the test case.
    /// </p><p>
    /// <b>Dispose</b> is executed to clean up resources
    /// deterministically. Call the base implementation to make
    /// sure that all resources are disposed of appropriately.</p>
    /// </remarks>
    [TestArgument("InputLogger",
                  "Off by default, may be set to All, Mouse or Keyboard to activate logger")]
    //[PermissionSet(SecurityAction.Assert, Name = "FullTrust")] //Removing as a fix for 
    [TestDefaults(DefaultMethodName = "StiEntryPoint")]
    public abstract class CustomTestCase : Model, System.IDisposable
    {
        #region Public methods.

        /// <summary>
        /// ProcessArgs
        /// </summary>
        /// <param name="commandline"></param>
        /// <returns></returns>
        public static ProcessStartInfo ProcessArgs(string commandline)
        {
            string[] args = commandline.Split(' ');
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = args[0];
            startInfo.UseShellExecute = true;

            if (args.Length > 1)
                startInfo.Arguments = String.Join(" ", args, 1, args.Length - 1);

            return startInfo;
        }

        /// <summary>
        /// Entry Point of Sti.exe
        /// </summary>
        public void StiEntryPoint(string commandline)
        {
            try
            {
                bool runInPartialTrust = false;
                string methodParams = DriverState.DriverParameters["MethodParams"];
                string[] args = methodParams.Split(' ');
                foreach (string arg in args)
                {
                    if (arg.ToLower().Contains("xbapname"))
                    {
                        runInPartialTrust = true;
                        string[] xbapArgs = arg.Split(new char[] { '=', ':' });
                        commandline = xbapArgs[1] + ".xbap " + commandline;
                        break;
                    }
                }    
                                              
                if (runInPartialTrust)
                {                                   
                    ExecutePartialTrust(commandline);
                }
                else
                {
                    TestRunner.DoMain(((string)commandline).Split(' '));
                }
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence(String.Format("Unexpected exception-------:\r\n{0}", exception));
                throw exception; 
            }
        }

        /// <summary>
        /// ExecutePartialTrust
        /// </summary>
        /// <param name="commandline"></param>
        public static void ExecutePartialTrust(string commandline)
        {
            GlobalLog.LogEvidence("Executing Partial Trust");
            ProcessStartInfo pInfo = ProcessArgs(commandline);

            //Publish the Harness so that we recieve the Cross-Process Notification
            
            ApplicationMonitor appMon = new ApplicationMonitor();

            // if we're launching a ClickOnce application, clean the cache
            if (pInfo.FileName.ToLowerInvariant().EndsWith(ApplicationDeploymentHelper.STANDALONE_APPLICATION_EXTENSION) || pInfo.FileName.ToLowerInvariant().EndsWith(ApplicationDeploymentHelper.BROWSER_APPLICATION_EXTENSION))
            {
                ApplicationDeploymentHelper.CleanClickOnceCache();
            }

            // shell exec the app
            appMon.StartProcess(pInfo);
            appMon.WaitForUIHandlerAbort();
            appMon.Close();
            // throw new NotImplementedException("Harness.Unpublish() no longer exists. Figure out the right thing to do.");
            //Harness.Current.Unpublish();
        }

        /// <summary>Starts the test case.</summary>
        /// <remarks>
        /// If running standalone, this starts the main application.
        /// </remarks>
        [TestEntryPoint]
        public void RunStandalone()
        {
            if (Application.Current == null)
            {
                _isApplicationOwned = true;

                if (ConfigurationSettings.Current.GetArgumentAsBool("NavigationWindow"))
                {
                    _application = new Application();
                }
                else
                {
                    _application = new Application();
                }
                _application.Startup += new System.Windows.StartupEventHandler(OnStartup);
                SetupTestCaseExceptionHandler();
                new System.Security.Permissions.SecurityPermission(System.Security.Permissions.PermissionState.Unrestricted).Assert();
                _application.Run();
            }
            else
            {
                _isApplicationOwned = false;
                if (IsNavigationStyle)
                {
                    _mainWindow = Application.Current.MainWindow;
                }
                else
                {
                    _mainWindow = CreateTestWindow();
                }
                MainWindowCreated();
                HookMainContentIfPending();
                ProcessStartupPage();
            }
        }

        /// <summary>Runs the test case.</summary>
        public abstract void RunTestCase();

        #region IDisposable implementation.

        /// <summary>Releases all resources.</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>Finalizes the custom application.</summary>
        ~CustomTestCase()
        {
            Dispose(false);
        }

        #endregion IDisposable implementation.

        #region Convenience methods.

        /// <summary>
        /// Retrieves the XAML for a file with replacements from
        /// the current configuration settings.
        /// </summary>
        /// <param name='fileName'>File name with XAML to replace.</param>
        /// <remarks><p>
        /// Creates the XAML from a page name replacing strings between
        /// '$$' characters with the value of the strings in
        /// the ConfigurationSettings object.
        /// </p><p>
        /// Original contents are cached in the main AppDomain, so they
        /// can be restored after running.
        /// </p>
        /// </remarks>
        public string GetProcessedXaml(string fileName)
        {
            string text = XamlUtils.GetXamlFileContents(fileName);
            int replacementCount = XamlUtils.ReplaceEscapedXaml(ref text);
            if (replacementCount == 0)
            {
                Logger.Current.Log("No replacements on xaml file [{0}]", fileName);
            }
            else
            {
                Logger.Current.Log("XAML file [{0}] after {1} replacements:{2}{3}",
                    fileName, replacementCount, Environment.NewLine, text);
            }
            return text;
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="text">Message to log.</param>
        public void Log(string text)
        {
            Logger.Current.Log(text);
        }

        /// <summary>Queues a simple delegate.</summary>
        /// <param name='handler'>Delegate to queue.</param>
        /// <example>The following example shows how to use this method.<code>...
        /// public class MyTestClass: CustomTestCase {
        ///   public void MyMethod() {
        ///     QueueDelegate(new SimpleHandler(OtherMethod));
        ///   }
        ///   public void OtherMethod() {
        ///     Log("Application is idle. Proceeding with test...");
        ///   }
        /// }</code></example>
        public void QueueDelegate(SimpleHandler handler)
        {
            QueueHelper.Current.QueueDelegate(handler);
        }

        #endregion Convenience methods.

        #endregion Public methods.

        #region Public properties.

        /// <summary>Determines whether we prefer navigating to creating new windows.</summary>
        public bool IsNavigationStyle
        {
            get { return (Application.Current != null) && (Application.Current.MainWindow is NavigationWindow); }
        }

        /// <summary>Main window.</summary>
        public Window MainWindow
        {
            get
            {
                if (this._isWindowless)
                {
                    throw new InvalidOperationException("Unable to obtain MainWindow in a windowless test case.");
                }

                return this._mainWindow;
            }
            set
            {
                //when you already have a window somewhere, You can set to this case.
                //Combined Custom test need this setter.
                //This help a CustomTestCase running in another CustomTestCase
                this._mainWindow = value;
            }
        }

        /// <summary>Startup page for the application.</summary>
        /// <remarks>
        /// If non-blank, the application will navigate to this page after
        /// creating the main window.
        /// </remarks>
        public string StartupPage
        {
            get { return this._startupPage; }
            set { this._startupPage = (value == null) ? String.Empty : value; }
        }

        /// <summary>Test window as a NavigationWindow instance.</summary>
        public NavigationWindow TestNavigationWindow
        {
            get { return TestWindow as NavigationWindow; }
        }

        /// <summary>Main test window, currently an alias for MainWindow.</summary>
        public Window TestWindow
        {
            get { return this._mainWindow; }
        }

        #region Convenience properties.

        /// <summary>
        /// Returns whether the test is running in automation mode.
        /// It is manual (not automated) mode if:
        /// 1. It is running under a debugger, or
        /// 2. It is not running under piper and manual switch is true (default).
        /// </summary>
        /// <remarks>
        /// If you need manual running under piper a command line switch
        /// /manual:true is needed. This can be added to the Tactics database
        /// or to the local piper.cfg configuration file.
        /// </remarks>
        public bool IsAutomation
        {
            get
            {
                if (System.Diagnostics.Debugger.IsAttached)
                    return false;
                bool inPiper = !Coordinator.IsStandaloneMode;
                bool manualOverride =
                    ConfigurationSettings.Current.GetArgumentAsBool("manual");
                return (inPiper && !manualOverride);
            }
        }

        /// <summary>Current configuration settings.</summary>
        public ConfigurationSettings Settings
        {
            get { return ConfigurationSettings.Current; }
        }

        #endregion Convenience properties.

        #endregion Public properties.

        #region Protected methods.

        #region Lifetime management.

        /// <summary>Releases all resources.</summary>
        /// <param name="disposing">
        /// Whether the call is invoked through IDispose.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            //This seems to be old fail save code which is causing problems while
            //test case closes down in amd64 machines for xbap cases. Commenting it
            //out resulted in no changes. Verified running all cases in amd64 and x86 
            //machines.

            //new System.Security.Permissions.UIPermission(
            //    System.Security.Permissions.PermissionState.Unrestricted)
            //    .Assert();

            //if (Application.Current != null)
            //{
            //    // If we are being disposed on a regular thread, we have
            //    // access to the dispatcher objects; otherwise, we need
            //    // to schedule the shutdown on the appropriate thread.
            //    if (disposing)
            //    {
            //        if (Logger.Current.TestLog != null)
            //        {
            //            Logger.Current.TestLog.Close();
            //            Logger.Current.TestLog = null;
            //        }
            //        Application.Current.Shutdown();
            //    }
            //    else
            //    {
            //        if (Application.Current.Dispatcher != null)
            //        {
            //            Application.Current.Dispatcher.BeginInvokeShutdown(DispatcherPriority.Send);
            //        }
            //    }
            //}            
        }

        /// <summary>
        /// Allows subclasses to take action after any contents have
        /// been loaded, but before they have been rendered.
        /// </summary>
        protected virtual void DoLoadCompleted()
        {
        }

        /// <summary>
        /// Allows subclasses to take action after the main window is created.
        /// </summary>
        protected virtual void DoMainWindowCreated()
        {
        }

        /// <summary>
        /// Allows subclasses to take action after the main window is shown.
        /// </summary>
        protected virtual void DoMainWindowShown()
        {
        }

        #endregion Lifetime management.

        /// <summary>
        /// Sets up an application-level exception handler for the test case.
        /// </summary>
        /// <remarks>
        /// By default, the TestRunner exception handler is used, to log out
        /// the exception, fail the test case and terminate the applicaiton.
        /// Subclasses may choose to change this behavior and set up their
        /// own exception handlers.
        /// </remarks>
        protected virtual void SetupTestCaseExceptionHandler()
        {
            TestRunner.SetupApplicationExceptionHandler();
        }

        #endregion Protected methods.

        #region Private methods.

        /// <summary>
        /// Creates and sets up a window suitable for running test cases in it.
        /// </summary>
        /// <returns>The created window.</returns>
        private Window CreateTestWindow()
        {
            Window result;
            System.Security.PermissionSet pset = new System.Security.PermissionSet(System.Security.Permissions.PermissionState.Unrestricted);
            pset.AddPermission(new System.Security.Permissions.UIPermission(System.Security.Permissions.PermissionState.Unrestricted));
            pset.AddPermission(new System.Security.Permissions.UIPermission(System.Security.Permissions.PermissionState.Unrestricted));
            pset.Assert();

            if (ConfigurationSettings.Current.GetArgumentAsBool("NavigationWindow"))
            {
                result = new NavigationWindow();
            }
            else
            {
                result = new Window();

            }

            result.Left = 0;
            result.Top = 0;
            result.Show();
            return result;
        }

        /// <summary>
        /// Called after the main window is created.
        /// </summary>
        private void MainWindowCreated()
        {
            DoMainWindowCreated();
        }

        /// <summary>
        /// Hooks this instance to the MainContent.EnterView event
        /// if required.
        /// </summary>
        private void HookMainContentIfPending()
        {
            if (_enterViewPending)
            {
                if (!this._isWindowless)
                {
                    QueueDelegate(MainWindowShown);
                }
                else
                {
                    QueueDelegate(RunTestCase);
                }

                _enterViewPending = false;
            }
        }

        /// <summary>
        /// Called after the main window has been shown.
        /// </summary>
        private void MainWindowShown()
        {
            //System.Security.PermissionSet pset = new System.Security.PermissionSet(
            //    System.Security.Permissions.PermissionState.Unrestricted);
            //pset.AddPermission(new System.Security.Permissions.UIPermission(System.Security.Permissions.PermissionState.Unrestricted));
            //pset.AddPermission(new System.Security.Permissions.UIPermission(System.Security.Permissions.PermissionState.Unrestricted));
            //pset.Assert();

            SetupLoggers();

            // This is a static class to grab static objects.
            GlobalCachedObjects.Current.Init();

            // Start static InputMonitorManager.
            InputMonitorManager.Initialize(null);

            // BEWARE! InputMonitorDisabled is initialized to false by default
            // it will be set to true *only* if it is explicitly set in
            // InputMonitorEnabled. 
            if (ConfigurationSettings.Current.HasArgument("InputMonitorEnabled")
                && ConfigurationSettings.Current.GetArgumentAsBool("InputMonitorEnabled"))
            {
                InputMonitorManager.Current.IsEnabled = true;
            }

            ElementUtils.BringToTop(MainWindow);

            MouseClickToActivateMainWindow();

            // HACK! If the mouse is at {0, 0} mouse click event missing problem
            // will not repro. see Regression_Bug891
            MouseInput.MouseMove(0, 0);

            // Before the test case is run, ensure that the IME state is consistent
            // across all platforms - default to IME turned off, matching English defaults.
            InputMethod.SetPreferredImeState(MainWindow, InputMethodState.Off);
            KeyboardInput.SetActiveInputLocale(Test.Uis.Data.InputLocaleData.EnglishUS.Identifier);
            DoMainWindowShown();
            QueueDelegate(RunTestCase);
        }

        /// <summary>
        /// Mouse click at the middle of the title bar to activate the window.
        /// </summary>
        private void MouseClickToActivateMainWindow()
        {

            if (MainWindow != null && !System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted && MainWindow.WindowStyle != WindowStyle.None)
            {
                int x;

                x = (int)(MainWindow.Left + MainWindow.Width) / 2;
                //we want to click at the middle of the title bar
                MouseInput.MouseClick(x, 20);
            }
        }

        /// <summary>
        /// Takes care of loading a startup page into the test window if the
        /// StartupPage property is not empty.
        /// </summary>
        private void ProcessStartupPage()
        {
            if (StartupPage.Length == 0)
            {
                return;
            }
            string content = GetProcessedXaml(StartupPage);

            StringStream stream = new StringStream(content);
            FrameworkElement root = (FrameworkElement)
                System.Windows.Markup.XamlReader.Load(stream);
            if (TestNavigationWindow != null)
            {
                TestNavigationWindow.Navigate(root);
            }
            else
            {
                TestWindow.Content = root;
            }
            DoLoadCompleted();
        }

        /// <summary>
        /// Sets up any loggers after the main window has been shown.
        /// </summary>
        /// <remarks>
        /// Currently the only logger set up automatically is the input
        /// logger, controlled through the InputLogger configuration setting.
        /// </remarks>
        private void SetupLoggers()
        {
            string logger = Settings.GetArgument("InputLogger");
            if (logger == "All")
            {
                InputLogger.EnableInputLogging(null);
            }
            else if (logger == "Mouse")
            {
                InputLogger.EnableInputLogging("Mouse");
            }
            else if (logger == "Keyboard")
            {
                InputLogger.EnableInputLogging("Keyboard");
            }
        }

        #region Private methods - event handlers.

        /// <summary>Handles the OnStartup event.</summary>
        /// <remarks>This is only called when the application is owned. Unlike
        /// the previous implementation, the application *can* have no
        /// startup page, in which case the test case can build its tree.</remarks>
        private void OnStartup(object sender, StartupEventArgs e)
        {
            Logger.Current.Log("CustomTestCase.OnStartup event called.");

            this._isWindowless = WindowlessTestAttribute.IsTestWindowless(this.GetType());

            // Allow per-test data to override type attribute setting.
            if (ConfigurationSettings.Current.HasArgument("Windowless"))
            {
                this._isWindowless = ConfigurationSettings.Current.GetArgumentAsBool("Windowless");
            }

            if (_isApplicationOwned)
            {
                if (!this._isWindowless)
                {
                    this._mainWindow = CreateTestWindow();
                    System.Diagnostics.Debug.Assert(_application != null);
                    System.Diagnostics.Debug.Assert(Application.Current != null);
                    Application.Current.MainWindow = _mainWindow;
                    MainWindowCreated();
                }

                HookMainContentIfPending();
            }
            ProcessStartupPage();
        }

        #endregion Private methods - event handlers.

        #endregion Private methods.

        #region Private fields.

        /// <summary>Main application.</summary>
        private Application _application;

        /// <summary>Whether the EnterView event is still pending to be hooked.</summary>
        private bool _enterViewPending = true;

        /// <summary>Whether the test case owns destroying the window.</summary>
        private bool _isApplicationOwned;

        /// <summary>Startup page to load.</summary>
        private string _startupPage = String.Empty;

        /// <summary>Main window.</summary>
        private Window _mainWindow;

        /// <summary>Whether the test case can run without a window.</summary>
        private bool _isWindowless;

        #endregion Private fields.
    }
}