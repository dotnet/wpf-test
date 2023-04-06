// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  NavigationEvents tests that:
//  [1] Navigating event is fired (once for StartupURI and once for secondPage.xaml)
//  [2] Navigated event is fired (once for StartupURI and once for secondPage.xaml)
//  [3] LoadCompleted event is fired (once for StartupURI and once for secondPage.xaml)


using System;
using System.Collections;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum NavigationEvents_State
        {
            InitialNav, 
            Navigated,
            NavigateToAnchoredPage
        }

        NavigationEvents_State _navigationEvents_curState = NavigationEvents_State.InitialNav;

        #region event count
        private int _numNavigatingEvents         = 0;
        private int _numNavigatedEvents          = 0;
        private int _numNavigationProgressEvents = 0;
        private int _numLoadCompletedEvents      = 0;
        private int _numFragmentNavigationEvents = 0;
        private int _numNavigationStoppedEvents  = 0;

        private int _expectedNavigatingEvents         = 2;
        private int _expectedNavigatedEvents          = 2;
        private int _expectedNavigationProgressEvents = 3;
        private int _expectedLoadCompletedEvents      = 2;
        private int _expectedFragmentNavigationEvents = 1;
        private int _expectedNavigationStoppedEvents  = 0;
        #endregion

        #region event order
        private ArrayList _actualEvents = null;
        private ArrayList _expectedEvents = null;
        #endregion

        protected void NavigationEvents_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavigationEvents");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize);

            NavigationHelper.ExpectedTestCount = 7;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"NavigationEvents_Page3.xaml#bottom";

            //NavigationHelper.Output("Registering application-level eventhandlers");
            //this.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);

            // Create the actualEvents array
            _actualEvents = new ArrayList();

            // Build the expectedEvents array
            _expectedEvents = new ArrayList();
            _expectedEvents.Add("navigating");
            _expectedEvents.Add("navigated");
            _expectedEvents.Add("loadcompleted");
            _expectedEvents.Add("navigating");
            _expectedEvents.Add("navigationprogress");
            _expectedEvents.Add("navigationprogress");
            _expectedEvents.Add("navigationprogress");
            _expectedEvents.Add("navigated");
            _expectedEvents.Add("loadcompleted");
            _expectedEvents.Add("fragmentnavigation");

            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri("NavigationEvents_Page1.xaml", UriKind.RelativeOrAbsolute);
            //base.OnStartup(e);
        }


        public void NavigationEvents_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LOADCOMPLETED [App] event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri);
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw == null)
                NavigationHelper.Fail("Could not get NavigationWindow");

            if (_navigationEvents_curState == NavigationEvents_State.InitialNav)
            {
                NavigationHelper.Output("Registering NavigationWindow eventhandlers");
                nw.Navigating += new NavigatingCancelEventHandler(NavigationEvents_Navigating_NavWin);
                nw.Navigated += new NavigatedEventHandler(NavigationEvents_Navigated_NavWin);
                nw.NavigationProgress += new NavigationProgressEventHandler(NavigationEvents_NavigationProgress_NavWin);
                nw.LoadCompleted += new LoadCompletedEventHandler(NavigationEvents_LoadCompleted_NavWin);
                nw.FragmentNavigation += new FragmentNavigationEventHandler(NavigationEvents_FragmentNavigation_NavWin);
                nw.NavigationStopped += new NavigationStoppedEventHandler(NavigationEvents_NavigationStopped_NavWin);
            }

            switch (_navigationEvents_curState)
            {
                case NavigationEvents_State.InitialNav:
                    _navigationEvents_curState = NavigationEvents_State.Navigated;
                    NavigationHelper.Output("Calling Navigate on secondPage.xaml");
                    nw.Navigate(new Uri("NavigationEvents_Page2.xaml", UriKind.RelativeOrAbsolute));
                    break;

                case NavigationEvents_State.Navigated:
                    _navigationEvents_curState = NavigationEvents_State.NavigateToAnchoredPage;
                    NavigationHelper.Output("Navigating to a fragment on an anchored page");
                    nw.Source = new Uri("NavigationEvents_Page3.xaml#bottom", UriKind.RelativeOrAbsolute);
                    break;

                case NavigationEvents_State.NavigateToAnchoredPage:
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object ob)
                        {
                            NavigationEvents_VerifyTest(e.Uri, e.Content);
                            return null;
                        }, null);
                    break;
            }
        }


        #region NavigationWindow eventhandlers
        public void NavigationEvents_Navigating_NavWin(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("NAVIGATING event fired");
            NavigationHelper.ActualTestCount++;

            _numNavigatingEvents++;
            _actualEvents.Add("navigating");
            NavigationHelper.Output("#### Navigating events: " + _numNavigatingEvents + " ####");
        }

        public void NavigationEvents_Navigated_NavWin(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("NAVIGATED event fired");
            NavigationHelper.ActualTestCount++;

            _numNavigatedEvents++;
            _actualEvents.Add("navigated");
            NavigationHelper.Output("#### Navigated events: " + _numNavigatedEvents + " ####");
        }

        private void NavigationEvents_NavigationProgress_NavWin(object source, NavigationProgressEventArgs e)
        {
            NavigationHelper.Output("NAVIGATIONPROGRESS event fired");
            _numNavigationProgressEvents++;
            _actualEvents.Add("navigationprogress");
            NavigationHelper.Output("#### NavigationProgress events: " + _numNavigationProgressEvents + " ####");
        }

        private void NavigationEvents_LoadCompleted_NavWin(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LOADCOMPLETED [NavWin] event fired");
            _numLoadCompletedEvents++;
            _actualEvents.Add("loadcompleted");
            NavigationHelper.Output("#### LoadCompleted events: " + _numLoadCompletedEvents + " ####");
        }

        private void NavigationEvents_FragmentNavigation_NavWin(object source, FragmentNavigationEventArgs e)
        {
            NavigationHelper.Output("FRAGMENTNAVIGATION event fired");
            _numFragmentNavigationEvents++;
            _actualEvents.Add("fragmentnavigation");
            NavigationHelper.Output("#### FragmentNavigation events: " + _numFragmentNavigationEvents + " ####");
        }

        private void NavigationEvents_NavigationStopped_NavWin(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("NAVIGATIONSTOPPED event fired");
            _numNavigationStoppedEvents++;
            _actualEvents.Add("navigationstopped");
            NavigationHelper.Output("#### NavigationStopped events: " + _numNavigationStoppedEvents + " ####");
        }
        #endregion

        private void NavigationEvents_VerifyTest(Uri navUri, Object navContent)
        {
            // Check event-order and event count
            NavigationHelper.Output("Checking the event counts");
            if (_expectedNavigatingEvents != _numNavigatingEvents)
                NavigationHelper.Fail("Navigating events didn't match. EXPECTED: " + _expectedNavigatingEvents + "; ACTUAL: " + _numNavigatingEvents);
            if (_expectedNavigatedEvents != _numNavigatedEvents)
                NavigationHelper.Fail("Navigated events didn't match. EXPECTED: " + _expectedNavigatedEvents + "; ACTUAL: " + _numNavigatedEvents);
            if (_expectedNavigationProgressEvents != _numNavigationProgressEvents)
                NavigationHelper.Fail("NavigationProgress events didn't match. EXPECTED: " + _expectedNavigationProgressEvents + "; ACTUAL: " + _numNavigationProgressEvents);
            if (_expectedLoadCompletedEvents != _numLoadCompletedEvents)
                NavigationHelper.Fail("LoadCompleted events didn't match. EXPECTED: " + _expectedLoadCompletedEvents + "; ACTUAL: " + _numLoadCompletedEvents);
            if (_expectedFragmentNavigationEvents != _numFragmentNavigationEvents)
                NavigationHelper.Fail("FragmentNavigation events didn't match. EXPECTED: " + _expectedFragmentNavigationEvents + "; ACTUAL: " + _numFragmentNavigationEvents);
            if (_expectedNavigationStoppedEvents != _numNavigationStoppedEvents)
                NavigationHelper.Fail("NavigationStopped events didn't match. EXPECTED: " + _expectedNavigationStoppedEvents + "; ACTUAL: " + _numNavigationStoppedEvents);

            NavigationHelper.Output("Checking the order events arrived in");
            if (_expectedEvents.Count != _actualEvents.Count)
                NavigationHelper.Fail("Number of events received did not match. EXPECTED: " + _expectedEvents.Count + "; ACTUAL: " + _actualEvents.Count);
            for (int i = 0; i < _expectedEvents.Count; i++)
            {
                if (!(_expectedEvents[i].Equals(_actualEvents[i])))
                    NavigationHelper.Fail("Items at index " + i + " did not match. EXPECTED: " + _expectedEvents[i] + "; ACTUAL: " + _actualEvents[i]);
            }

            // If you get to this point without failing, then pass the test
            NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, navUri)) && (navContent != null));
        }
    }
}

