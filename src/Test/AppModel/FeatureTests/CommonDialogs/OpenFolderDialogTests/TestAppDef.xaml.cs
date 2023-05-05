using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;
using Microsoft.Test;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;


namespace Microsoft.Wpf.AppModel.CommonDialogs
{
    public partial class OpenFolderDialogTests
    {
        private Application       currNavApp      = null;
        private NavigationWindow  currNavWin      = null;
        private String            currentTest     = null;
        private TestLog           log             = null;

        private bool isInitialLoad   = true;

        #region event handlers
        /// <summary>
        /// Initializes the test by:
        /// [1] instantiating the TestLog
        /// [2] grabbing the currentTest type, so we can set the Application StartupUri
        /// [3] grabbing a reference to the Application (so we can later grab a reference to the NavWin)
        /// [4] registering application-level eventhandlers
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            log = TestLog.Current == null ? new TestLog("OpenFolderDialog test") : TestLog.Current;

            // Cannot create/grab reference to TestLog.  Exiting test.
            if (log == null)
            {
                ApplicationMonitor.NotifyStopMonitoring();
            }

            // Grab argument passed into AppMonitor to use as the Application's StartupUri
            currentTest = DriverState.DriverParameters["TestStartupURI"];
            if (currentTest == null || currentTest.Equals(String.Empty))
            {
                Common.ExitWithError("Usage: need to pass TestStartupURI in the AppMonitor config file for this test");
            }

            currNavApp = Application.Current;
            currNavApp.StartupUri = new Uri(currentTest + ".xaml", UriKind.RelativeOrAbsolute);

            // Register the test's eventhandlers
            log.LogEvidence("Registering LoadCompleted event handler for Application");
            LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedAPP);
        }


        /// <summary>
        /// Further initializes test by:
        /// [1] Grabbing a reference to the NavigationWindow
        /// [2] Registering NavigationWindow-level event handlers
        /// </summary>
        private void OnLoadCompletedAPP(object sender, NavigationEventArgs e)
        {
            if (isInitialLoad)
            {
                if (e.Navigator is NavigationWindow)
                {
                    log.LogEvidence("Grabbing reference to NavigationWindow.");
                    currNavWin = currNavApp.MainWindow as NavigationWindow;

                    log.LogEvidence("Registering ContentRendered event handler for NavigationWindow");
                    currNavWin.ContentRendered += new EventHandler(OnContentRendered_NavWin);

                    isInitialLoad = false;
                }
            }
        }

        private void OnContentRendered_NavWin(object sender, EventArgs e)
        {
            log.LogEvidence("CommonFileDialogTest NavigationWindow content rendered.");
        }
        #endregion
    }
}
