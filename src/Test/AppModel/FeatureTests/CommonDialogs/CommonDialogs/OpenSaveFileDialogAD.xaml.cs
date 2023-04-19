// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: OpenSaveFileDialog tests that we are able to:
//  [1] open an existing file located in the same directory as the app 
//      using the OpenFileDialog 
//  [2] save a file in the same directory as the app using the SaveFileDialog 
//  [3] open a non-existing file, supply an existing filename including extension
//  [4] save to an existing file, supply a filename including extension
//
//  Creator: Microsoft
//
//


using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;
using Microsoft.Test;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;


namespace Microsoft.Wpf.AppModel.CommonDialogs
{
    public partial class CommonFileDialogTest
    {
        private Application       _currNavApp      = null;
        private NavigationWindow  _currNavWin      = null;
        private String            _currentTest     = null;
        private TestLog           _log             = null;

        //private bool isBrowserHosted = false;
        private bool _isInitialLoad   = true;

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
            _log = TestLog.Current == null ? new TestLog("CommonFileDialog test") : TestLog.Current;

            // Cannot create/grab reference to TestLog.  Exiting test.
            if (_log == null)
            {
                ApplicationMonitor.NotifyStopMonitoring();
            }

            // Grab argument passed into AppMonitor to use as the Application's StartupUri
            //currentTest = Harness.Current["TestStartupURI"];
            _currentTest = DriverState.DriverParameters["TestStartupURI"];
            if (_currentTest == null || _currentTest.Equals(String.Empty))
            {
                Common.ExitWithError("Usage: need to pass TestStartupURI in the AppMonitor config file for this test");
            }

            _currNavApp = Application.Current;
            _currNavApp.StartupUri = new Uri(_currentTest + ".xaml", UriKind.RelativeOrAbsolute);

            // Register the test's eventhandlers
            _log.LogEvidence("Registering LoadCompleted event handler for Application");
            LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedAPP);
        }


        /// <summary>
        /// Further initializes test by:
        /// [1] Grabbing a reference to the NavigationWindow
        /// [2] Registering NavigationWindow-level event handlers
        /// [3] setting global flags indicating whether or not the test is browser-hosted,
        ///     and whether we've completed the initial load sequence for the test
        /// </summary>
        private void OnLoadCompletedAPP(object sender, NavigationEventArgs e)
        {
            if (_isInitialLoad)
            {
                if (e.Navigator is NavigationWindow)
                {
                    _log.LogEvidence("Grabbing reference to NavigationWindow.");
                    _currNavWin = _currNavApp.MainWindow as NavigationWindow;

                    _log.LogEvidence("Registering ContentRendered event handler for NavigationWindow");
                    _currNavWin.ContentRendered += new EventHandler(OnContentRendered_NavWin);

                    //if (BrowserInteropHelper.IsBrowserHosted)
                    //{
                    //    isBrowserHosted = true;
                    //}

                    _isInitialLoad = false;
                }
            }
        }

        private void OnContentRendered_NavWin(object sender, EventArgs e)
        {
            _log.LogEvidence("CommonFileDialogTest NavigationWindow content rendered.");
        }
        #endregion
    }
}
