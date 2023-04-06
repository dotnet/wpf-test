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
//


using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Navigation;
using Microsoft.Test;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        private NavigationWindow  _openSaveFileDialog_currNavWin      = null;
        private String            _openSaveFileDialog_currentTest     = null;

        //private bool OpenSaveFileDialog_isBrowserHosted = false;
        private bool _openSaveFileDialog_isInitialLoad   = true;

        #region event handlers
        /// <summary>
        /// Initializes the test by:
        /// [1] instantiating the TestLog
        /// [2] grabbing the OpenSaveFileDialog_currentTest type, so we can set the Application StartupUri
        /// [3] grabbing a reference to the Application (so we can later grab a reference to the NavWin)
        /// [4] registering application-level eventhandlers
        /// </summary>
        void OpenSaveFileDialog_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("CommonFileDialog test");

            // Grab argument passed into AppMonitor to use as the Application's StartupUri
            _openSaveFileDialog_currentTest = DriverState.DriverParameters["CommonFileDialogTest"];
            if (String.IsNullOrEmpty(_openSaveFileDialog_currentTest))
                NavigationHelper.Fail("Usage: need to pass CommonFileDialogTest URI in the AppMonitor config file for this test");

            Application.Current.StartupUri = new Uri(_openSaveFileDialog_currentTest + ".xaml", UriKind.RelativeOrAbsolute);

            // Register the test's eventhandlers
            //NavigationHelper.Output("Registering LoadCompleted event handler for Application");
            //LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedAPP);
        }


        /// <summary>
        /// Further initializes test by:
        /// [1] Grabbing a reference to the NavigationWindow
        /// [2] Registering NavigationWindow-level event handlers
        /// [3] setting global flags indicating whether or not the test is browser-hosted,
        ///     and whether we've completed the initial load sequence for the test
        /// </summary>
        private void OpenSaveFileDialog_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_openSaveFileDialog_isInitialLoad)
            {
                if (e.Navigator is NavigationWindow)
                {
                    NavigationHelper.Output("Grabbing reference to NavigationWindow.");
                    _openSaveFileDialog_currNavWin = Application.Current.MainWindow as NavigationWindow;

                    NavigationHelper.Output("Registering ContentRendered event handler for NavigationWindow");
                    _openSaveFileDialog_currNavWin.ContentRendered += new EventHandler(OpenSaveFileDialog_OnContentRendered_NavWin);

                    // commenting out due to warning in devdiv. This var was not used
                    //if (BrowserInteropHelper.IsBrowserHosted)
                        //OpenSaveFileDialog_isBrowserHosted = true;

                    _openSaveFileDialog_isInitialLoad = false;
                }
            }
        }

        private void OpenSaveFileDialog_OnContentRendered_NavWin(object sender, EventArgs e)
        {
            NavigationHelper.Output("CommonFileDialogTest NavigationWindow content rendered.");
        }
        #endregion

    }
}
