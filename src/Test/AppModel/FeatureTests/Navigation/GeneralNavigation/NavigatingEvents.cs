// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  NavingEvents BVT verifies the following NavigationWindow events
//  1) Navigating  
//  2) NavigationProgress events
//  Also removes the event handlers (and checks to see that these are not called
//  after being removed)
//


// 

using System;
using System.Collections;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Class to handle NavigatingEvents test
    /// </summary>
    public class NavigatingEvents
    {
        private NavigationWindow _navWin = null;
        private Application _navApp = null;

        enum State
        {
            UnInit,
            Init,
            NavingToPage2,
            NavingBackToPage1,
            PreEmptNavigation
        };

        private int _prevNavigationProgressHitCount = 0;
        private int _prevNavigatingHitCount = 0;

        private State _currentState = State.UnInit;
        private int _navigationProgressHitCount = 0;
        private int _navigatingHitCount = 0;
        private int _loadCompletedHitCount = 0;
        private int _navigatedHitCount = 0;
        private int _contentRenderedHitCount = 0;

        private ArrayList _actualEvents = null;
        private ArrayList _expectedEvents = null;

        // pages
        private const string Page1 = "NavigatingEvents_Page1.xaml"; // page 1 
        private const string Page2 = "NavigatingEvents_Page2.xaml"; // page 2 
        private const string Page3 = "NavigatingEvents_Page3.xaml"; // page 3 

        /// <summary>
        /// Constructor
        /// </summary>
        public NavigatingEvents()
        {
        }

        /// <summary>
        /// Startup event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavigatingEvents");
            NavigationHelper.SetStage(TestStage.Run);

            NavigationHelper.Output("In Startup.");
            _currentState = State.Init;
            _navApp = Application.Current as Application;

            // Build expected events (only for the PreEmptNavigation test)
            _expectedEvents = new ArrayList();
            _expectedEvents.Add("navigated");            // navigated event for navigation to NavigatingEvents_Page2.xaml
            _expectedEvents.Add("navigated");            // navigated event for navigation to NavigatingEvents_Page3.xaml
            _expectedEvents.Add("loadcompleted");        // loadcompleted event for NavigatingEvents_Page3.xaml
            _expectedEvents.Add("contentrendered");      // contentrendered event for NavigatingEvents_Page3.xaml

            // Get actual event array ready
            _actualEvents = new ArrayList();

            Application.Current.StartupUri = new Uri(Page1, UriKind.RelativeOrAbsolute);

        }

        #region NavigationWindow eventhandlers
        private void OnNavigated(object sender, NavigationEventArgs e)
        {
            switch (_currentState)
            {
                case State.PreEmptNavigation:
                    NavigationHelper.Output("Logging Navigated event fired");
                    ++_navigatedHitCount;
                    _actualEvents.Add("navigated");
                    if (e.Uri.ToString().Equals(Page2))
                    {
                        String webRespContentType = e.WebResponse.ContentType;
                        NavigationHelper.Output("Starting a new navigation via Source property in Navigated eventhandler");
                        _navWin.Source = new Uri(Page3, UriKind.RelativeOrAbsolute);

                        // Check that WebResponse.ContentType property is now null.
                        try
                        {
                            if (e.WebResponse.ContentType != null || e.WebResponse.ContentType.Equals(webRespContentType))
                            {
                                NavigationHelper.Fail("WebResponse of pre-empted navigation to " + e.Uri + " was not disposed of");
                            }
                        }
                        catch (ObjectDisposedException ode)
                        {
                            NavigationHelper.Output("Correctly disposed of pre-empted navigation's WebResponse\n" + ode.ToString());
                        }
                        catch (Exception exp)
                        {
                            NavigationHelper.Fail("Accessing property of WebResponse of pre-empted navigation to " + e.Uri + " didn't throw ObjectDisposedException, instead threw " + exp.ToString());
                        }
                    }
                    break;
            }
        }

        private void OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_currentState == State.PreEmptNavigation)
            {
                NavigationHelper.Output("Logging LoadCompleted event fired");
                ++_loadCompletedHitCount;
                _actualEvents.Add("loadcompleted");
            }
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            if (_currentState == State.PreEmptNavigation)
            {
                NavigationHelper.Output("Logging ContentRendered event fired");
                ++_contentRenderedHitCount;
                _actualEvents.Add("contentrendered");

                // Verify stuff here after emptying message queue
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                (DispatcherOperationCallback)delegate(object ob)
                                {
                                    VerifyTest();
                                    return null;
                                }, null);
            }
        }

        private void OnNavigating(object sender, NavigatingCancelEventArgs e)
        {            
            switch (_currentState)
            {
                case State.NavingToPage2:
                case State.NavingBackToPage1:
                    NavigationHelper.Output("Logging Navigating event fired");
                    ++_navigatingHitCount;
                    break;
            }
        }

        private void OnNavigationProgress(object sender, NavigationProgressEventArgs e)
        {
            switch (_currentState)
            {
                case State.NavingToPage2:
                case State.NavingBackToPage1:
                    NavigationHelper.Output("Logging NavigationProgress event fired");
                    ++_navigationProgressHitCount;
                    break;
            }
        }

        #endregion

        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            switch (_currentState)
            {
                case State.Init:
                    _navWin = _navApp.MainWindow as NavigationWindow;
                    if (_navWin == null)
                    {
                        NavigationHelper.Fail("NavWin is null!!");
                    }

                    NavigationHelper.Output("Registering NavigationWindow level eventhandlers");
                    _navWin.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted);
                    _navWin.Navigated += new NavigatedEventHandler(OnNavigated);
                    _navWin.Navigating += new NavigatingCancelEventHandler(OnNavigating);
                    _navWin.NavigationProgress += new NavigationProgressEventHandler(OnNavigationProgress);
                    _navWin.ContentRendered += new EventHandler(OnContentRendered);

                    // Start first subtest
                    _currentState = State.NavingToPage2;
                    NavigationHelper.Output("Navigate to NavigatingEvents_Page2.xaml");
                    _navWin.Navigate(new Uri(Page2, UriKind.RelativeOrAbsolute));
                    break;

                case State.NavingToPage2:
                    _prevNavigatingHitCount = _navigatingHitCount;
                    _prevNavigationProgressHitCount = _navigationProgressHitCount;

                    if (_navigatingHitCount != 1)
                    {
                        NavigationHelper.Fail("Navigating event fired " + _navigatingHitCount + " times");
                    }

                    if (_navigationProgressHitCount < 1)
                    {
                        NavigationHelper.Fail("NavigationProgress event fired " + _navigationProgressHitCount + " times");
                    }

                    NavigationHelper.Output("Un-registering NavigationWindow level eventhandlers");
                    _navWin.Navigating -= new NavigatingCancelEventHandler(OnNavigating);
                    _navWin.NavigationProgress -= new NavigationProgressEventHandler(OnNavigationProgress);

                    NavigationHelper.Output("GoBack to NavigatingEvents_Page1.xaml");
                    _currentState = State.NavingBackToPage1;
                    _navWin.GoBack();
                    break;
                
                case State.NavingBackToPage1:
                    NavigationHelper.Output("Verifying the number of Navigating/NavigationProgress events fired");
                    NavigationHelper.Output("Navigating fired " + _navigatingHitCount + " times");
                    NavigationHelper.Output("NavigationProgress fired " + _navigationProgressHitCount + " times");

                    if ((_prevNavigatingHitCount != _navigatingHitCount) ||
                        (_prevNavigationProgressHitCount != _navigationProgressHitCount))
                    {
                        NavigationHelper.Output("Hits counts did not match");
                        NavigationHelper.Output("Navigating cur = " + _navigatingHitCount
                                        + " prev = " + _prevNavigatingHitCount);
                        NavigationHelper.Output("NavigationProgress cur = " + _navigationProgressHitCount
                                        + " prev = " + _prevNavigationProgressHitCount);
                        NavigationHelper.Fail("Event hit counts do not match expected");
                    }
                    else
                    {
                        NavigationHelper.Output("Starting a new navigation [via Source property] to NavigatingEvents_Page2.xaml");
                        _currentState = State.PreEmptNavigation;
                        _navWin.Source = new Uri(Page2, UriKind.RelativeOrAbsolute);
                    }
                    break;

                case State.PreEmptNavigation:
                    break;
            }
        }

        private void VerifyTest()
        {
            NavigationHelper.Output("Checking that we received the correct number of events");
            if (_expectedEvents.Count != _actualEvents.Count)
            {
                NavigationHelper.Fail("Did not get the expected # of events. EXPECTED: " + _expectedEvents.Count + "; ACTUAL: " + _actualEvents.Count);
            }
            else
            {
                NavigationHelper.Output("Checking the order that events arrived in");
                for (int i = 0; i < _expectedEvents.Count; i++)
                {
                    if (!_expectedEvents[i].Equals(_actualEvents[i]))
                        NavigationHelper.Fail("Items at index " + i + " didn't match. EXPECTED: " + _expectedEvents[i] + "; ACTUAL: " + _actualEvents[i]);
                }

                NavigationHelper.Pass("Event hit counts match the expected number of events fired");
            }
        }
   
    }
}
