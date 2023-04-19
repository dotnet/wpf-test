// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    public class NavigationWindowTestClass : NavigationBaseClass
    {
        #region globals
        private NavigationWindow _navWin = null;

        private bool _registerNavEventHandlers  = true;
        private bool _recordEventOrder          = true;
        #endregion


        public NavigationWindowTestClass(String userGivenTestName) : base(userGivenTestName)
        {
            //Output("Registering application-level eventhandlers");
            //LoadCompleted += new LoadCompletedEventHandler(OnAppLoadCompleted);

            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);

            SetupTest();
        }

        protected void SetupTest()
        {
             if (this.IsFirstRun)
            {
                NavigationHelper.Output("Grabbing reference to NavigationWindow.");
                _navWin = Application.Current.MainWindow as NavigationWindow;

                if (_registerNavEventHandlers)
                {
                    NavigationHelper.Output("Registering NavigationWindow eventhandlers.");
                    _navWin.Navigating += new NavigatingCancelEventHandler(OnNavWinNavigating);
                    _navWin.Navigated += new NavigatedEventHandler(OnNavWinNavigated);
                    _navWin.FragmentNavigation += new FragmentNavigationEventHandler(OnNavWinFragmentNavigation);
                    _navWin.NavigationProgress += new NavigationProgressEventHandler(OnNavWinNavigationProgress);
                    _navWin.NavigationStopped += new NavigationStoppedEventHandler(OnNavWinNavigationStopped);
                    _navWin.LoadCompleted += new LoadCompletedEventHandler(OnNavWinLoadCompleted);
                    _navWin.ContentRendered += new EventHandler(OnNavWinContentRendered);
                    _navWin.NavigationFailed += new NavigationFailedEventHandler(OnNavWinNavigationFailed);
                }
            }
        }

        #region eventhandlers : counters

        protected void OnNavWinNavigating(object sender, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("NavigationWindow :: NAVIGATING event caught");
            NavigationHelper.NumActualNavigatingEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigating");
        }

        protected void OnNavWinNavigated(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("NavigationWindow :: NAVIGATED event caught");
            NavigationHelper.NumActualNavigatedEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigated");
        }

        protected void OnNavWinFragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            NavigationHelper.Output("NavigationWindow :: FRAGMENTNAVIGATION event caught");
            NavigationHelper.NumActualFragmentNavigationEvents++;

            if (_recordEventOrder)
                AddActualEvent("fragmentnavigation");
        }

        protected void OnNavWinNavigationProgress(object sender, NavigationProgressEventArgs e)
        {
            NavigationHelper.Output("NavigationWindow :: NAVIGATIONPROGRESS event caught");
            NavigationHelper.NumActualNavigationProgressEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigationprogress");
        }

        protected void OnNavWinNavigationStopped(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("NavigationWindow :: NAVIGATIONSTOPPED event caught");
            NavigationHelper.NumActualNavigationStoppedEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigationstopped");
        }

        protected void OnNavWinLoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("NavigationWindow :: LOADCOMPLETED event caught");
            NavigationHelper.NumActualLoadCompletedEvents++;

            if (_recordEventOrder)
                AddActualEvent("loadcompleted");
        }

        protected void OnNavWinNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            NavigationHelper.Output("NavigationWindow :: NAVIGATIONFAILED event caught");
            NavigationHelper.NumActualNavigationFailedEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigationfailed");
        }

        protected void OnNavWinContentRendered(object sender, EventArgs e)
        {
            Output("NavigationWindow :: CONTENTRENDERED event caught");
            // we don't keep track of ContentRendered events fired

            if (_recordEventOrder)
                AddActualEvent("contentrendered");
        }

        #endregion

        /// <summary>
        /// Returns the number of windows "owned" by this particular application.
        /// </summary>
        /// <returns>Number of windows in the Application's window collection</returns>
        public int GetWindowCount()
        {
            return Application.Current.Windows.Count;
        }
    }
}
