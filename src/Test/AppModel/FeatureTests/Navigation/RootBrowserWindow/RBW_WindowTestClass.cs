// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Logging;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// The RootBrowserWindowBaseClass is instantiated by each specific Frame navigation test
    /// and provides the following members and methods common to all tests that 
    /// exercise Frame API.
    /// </summary>
    public class RootBrowserWindowTestClass : NavigationBaseClass
    {
        #region globals
        private const String ROOTBROWSERWINDOWPAGE = @"RBW_WindowTestPage.xaml";
        protected NavigationWindow navWin_Local = null;
        #endregion

        public RootBrowserWindowTestClass(String userGivenTestName) : base (userGivenTestName)
        {
            Application.Current.StartupUri = new Uri(ROOTBROWSERWINDOWPAGE, UriKind.RelativeOrAbsolute);

            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);
        }

        public RootBrowserWindowTestClass()
            : this("DefaultRootBrowserWindowTest")
        {
        }

        public void SetupTest()
        {
            if (this.IsFirstRun)
            {
                Output("Grabbing reference to NavigationWindow.");
                navWin_Local = Application.Current.MainWindow as NavigationWindow;

                if (navWin_Local == null)
                    Fail("Could not grab a reference to the NavigationWindow. Exiting test");
            }
        }

        #region properties
        public NavigationWindow NavWin
        {
            get
            {
                return navWin_Local;
            }
        }
        #endregion
    }
}
