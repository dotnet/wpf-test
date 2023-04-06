// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: FrameNavigationEvents tests that we receive:
//  [1] the Navigating event prior to a navigation to another page 
//  [2] the Navigated event after the target page was found and 
//      the download has begun
//  [3] NavigationProgress events periodically while we are downloading
//  [4] the LoadCompleted event once the entire target page has been loaded
//      (but not necessarily rendered onscreen)
//  [5] the FragmentNavigation event when we navigate to a URI fragment
//      located on the current page
//  [6] the NavigationStopped event when the user clicks the Stop button or
//      programmatically invokes the StopLoading method
//
//
//

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// Verify navigation event counts and the order in navigations in a frame
    /// </summary>

    public class FrameNavigationEvents
    {
        private enum CurrentTest
        {
            UnInit,
            NavigateToAnchoredPage,
            NavigateToFragment,
            NavigateToRegularPage1,
            NavigateToRegularPage2,
            NavigateBackToFlowdocPage,
            End
        }

        #region FrameNavigationEvents privates

        private CurrentTest _frameNavEventsTest = CurrentTest.UnInit;
        private List<String> _frameNavEventsExpectedEvents = new List<String>();
        private const String flowDocPage = @"FlowDocument_Loose.xaml";
        private const String bottomFragment = @"#bottom";
        private const String topFragment = @"#top";
        private const String anchoredPage = @"AnchoredPage_Loose.xaml";
        private const String flowDocPageSourceStandalone = @"NavigationTests_Standalone;component/FlowDocument_Loose.xaml";
        private const String flowDocPageSourceBrowser = @"NavigationTests_Browser;component/FlowDocument_Loose.xaml";
        private String _flowDocPageSource = "";
        private Frame _homeFrame = null;
        private FrameTestClass _frameTest = null;

        #endregion

        public void Startup(object sender, StartupEventArgs e)
        {
            _frameTest = new FrameTestClass("FrameNavigationEvents");
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = true;

            // determine if standalone or browser hosted
            if (AppDomain.CurrentDomain.FriendlyName.ToString().Contains(ApplicationDeploymentHelper.BROWSER_APPLICATION_EXTENSION))
            {
                _flowDocPageSource = flowDocPageSourceBrowser;
            }
            else
            {
                _flowDocPageSource = flowDocPageSourceStandalone;
            }

            // Set the expected navigation counts
            NavigationHelper.NumExpectedNavigatingEvents = 4;
            NavigationHelper.NumExpectedNavigatedEvents = 3;
            NavigationHelper.NumExpectedNavigationStoppedEvents = 1;
            NavigationHelper.NumExpectedFragmentNavigationEvents = 1;
            NavigationHelper.NumExpectedLoadCompletedEvents = 3;

            /// NumExpectedNavigationProgressEvents have to be calculated dynamically 
            /// Refer Variable definition in NavigationHelper.cs
            NavigationHelper.NumExpectedNavigationProgressEvents = 8;

            // Construct the expected event order list
            if (_frameNavEventsExpectedEvents == null)
            {
                _frameTest.Fail("Could not create expected event order list.  Aborting test.");
            }
            else
            {
                _frameNavEventsExpectedEvents.Add("navigating");
                _frameNavEventsExpectedEvents.Add("navigationprogress");
                _frameNavEventsExpectedEvents.Add("navigationprogress");
                _frameNavEventsExpectedEvents.Add("navigationprogress");
                _frameNavEventsExpectedEvents.Add("navigationprogress");
                _frameNavEventsExpectedEvents.Add("navigated");
                _frameNavEventsExpectedEvents.Add("loadcompleted");
                _frameNavEventsExpectedEvents.Add("contentrendered");
                _frameNavEventsExpectedEvents.Add("navigating");
                _frameNavEventsExpectedEvents.Add("navigationprogress");
                _frameNavEventsExpectedEvents.Add("navigationprogress");
                _frameNavEventsExpectedEvents.Add("navigationprogress");
                _frameNavEventsExpectedEvents.Add("navigationprogress");
                _frameNavEventsExpectedEvents.Add("navigated");
                _frameNavEventsExpectedEvents.Add("loadcompleted");
                _frameNavEventsExpectedEvents.Add("contentrendered");
                _frameNavEventsExpectedEvents.Add("navigating");
                _frameNavEventsExpectedEvents.Add("fragmentnavigation");
                _frameNavEventsExpectedEvents.Add("navigated");
                _frameNavEventsExpectedEvents.Add("loadcompleted");
                _frameNavEventsExpectedEvents.Add("navigating");
                _frameNavEventsExpectedEvents.Add("navigationstopped");
            }
        }

        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            _frameTest.Output("In LoadCompleted - State = " + _frameNavEventsTest.ToString());
            _frameTest.Output("Application.LoadCompleted:  e.Navigator = " + e.Navigator.ToString());

            if (_frameTest.IsFirstRun &&
                _frameNavEventsTest == CurrentTest.UnInit)
            {
                _frameTest.SetupTest();

                _frameTest.StdFrame.Source = new Uri(flowDocPage, UriKind.RelativeOrAbsolute);
                _frameTest.StdFrame.ContentRendered += new EventHandler(FrameContentRendered);

                _homeFrame = LogicalTreeHelper.FindLogicalNode(((NavigationWindow)Application.Current.MainWindow).Content as DependencyObject, "testFrame") as Frame;
                _homeFrame.ContentRendered += new EventHandler(FrameContentRendered);

                _frameNavEventsTest = CurrentTest.NavigateToAnchoredPage;
            }
            // After fragment navigation, navigate back to the page we started from
            else if (_frameNavEventsTest == CurrentTest.NavigateToRegularPage1 &&
                _frameTest.VerifySource(_frameTest.StdFrame, anchoredPage))
            {
                _frameTest.Output("Navigating via Uri to " + anchoredPage);

                // Unregister some of the navigation-related eventhandlers, then navigate elsewhere
                _frameTest.UnregisterNavEventHandler(FrameTestClass.NavEvent.FragmentNavigation);
                _frameTest.UnregisterNavEventHandler(FrameTestClass.NavEvent.LoadCompleted);
                _frameTest.UnregisterNavEventHandler(FrameTestClass.NavEvent.Navigating);
                _frameTest.UnregisterNavEventHandler(FrameTestClass.NavEvent.NavigationProgress);
                _frameTest.UnregisterNavEventHandler(FrameTestClass.NavEvent.NavigationStopped);

                _frameNavEventsTest = CurrentTest.NavigateToRegularPage2;
            }
        }

        public void ContentRendered(object sender, EventArgs e)
        {
            _frameTest.Output("In ContentRendered - State = " + _frameNavEventsTest.ToString());
            if (sender is NavigationWindow)
            {
                NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
                _frameTest.Output("NavigationWindow.Source = " + navWin.Source);
            }
            else if (sender is Frame)
            {
                _frameTest.Output("Frame.Source = " + _frameTest.StdFrame.Source);
            }

            if (_frameNavEventsTest == CurrentTest.NavigateToAnchoredPage &&
                _frameTest.VerifySource(_frameTest.StdFrame, _flowDocPageSource))
            {
                _frameTest.Output("Navigating frame to a page with anchors.");
                _frameTest.NavigateToUri(_frameTest.StdFrame, new Uri(anchoredPage, UriKind.RelativeOrAbsolute));
                _frameNavEventsTest = CurrentTest.NavigateToFragment;
            }
            else
            {
                _frameTest.Fail("Unexpected to be in ContentRendered - State = " + _frameNavEventsTest.ToString());
            }
        }

        // Frame ContentRendered event handler
        private void FrameContentRendered(object sender, EventArgs e)
        {
            _frameTest.Output("In FrameContentRendered - State = " + _frameNavEventsTest.ToString());
            if (sender is NavigationWindow)
            {
                NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
                _frameTest.Output("NavigationWindow.Source = " + navWin.Source);
            }
            else if (sender is Frame)
            {
                _frameTest.Output("Frame.Source = " + _frameTest.StdFrame.Source);
            }

            switch (_frameNavEventsTest)
            {
                case CurrentTest.NavigateToFragment:
                    if (_frameTest.VerifySource(_frameTest.StdFrame, _flowDocPageSource))
                    {
                        _frameTest.Output("Navigating to bottom region of " + anchoredPage);
                        _frameTest.NavigateToUri(_frameTest.StdFrame, new Uri(bottomFragment, UriKind.RelativeOrAbsolute));
                        _frameNavEventsTest = CurrentTest.NavigateToRegularPage1;
                    }
                    break;

                case CurrentTest.NavigateToRegularPage2:
                    if (_frameTest.VerifySource(_frameTest.StdFrame, anchoredPage))
                    {
                        // Re-register some of the navigation-related eventhandlers
                        _frameTest.RegisterNavEventHandler(FrameTestClass.NavEvent.FragmentNavigation);
                        _frameTest.RegisterNavEventHandler(FrameTestClass.NavEvent.LoadCompleted);
                        _frameTest.RegisterNavEventHandler(FrameTestClass.NavEvent.Navigating);
                        _frameTest.RegisterNavEventHandler(FrameTestClass.NavEvent.NavigationProgress);
                        _frameTest.RegisterNavEventHandler(FrameTestClass.NavEvent.NavigationStopped);

                        // Navigate to a page
                        _frameTest.NavigateToUri(_frameTest.StdFrame, new Uri(flowDocPage, UriKind.RelativeOrAbsolute));
                        _frameNavEventsTest = CurrentTest.NavigateBackToFlowdocPage;
                    }
                    break;
            }
        }

        public void Navigating(object source, NavigatingCancelEventArgs e)
        {
            _frameTest.Output("In Navigating - State = " + _frameNavEventsTest.ToString());
            // Once download has begun, we want to call StopLoading to fire NavigationStopped
            if (_frameNavEventsTest == CurrentTest.NavigateBackToFlowdocPage &&
                e.Uri.ToString().Equals(flowDocPage))
            {
                // Abort the download
                _frameTest.Output("- - - - ABORTING the download - - - -");
                _frameNavEventsTest = CurrentTest.End;
                _frameTest.StdFrame.StopLoading();
            }
        }

        public void NavigationStopped(object source, NavigationEventArgs e)
        {
            _frameTest.Output("In NavigationStopped - State = " + _frameNavEventsTest.ToString());

            if (_frameNavEventsTest == CurrentTest.End)
            {
                // Queue this in the Dispatcher as Background priority so the message
                // queue will be empty when we count the events that have occurred.
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate(object ob)
                    {
                        if (_frameTest.VerifyEventCount() && _frameTest.VerifyEventOrder(_frameNavEventsExpectedEvents))
                        {
                            _frameTest.Pass("All states matched.  Event counts and event order matched.");
                        }
                        else
                        {
                            _frameTest.Fail("All states did not match.");
                        }
                        return null;
                    }, null);
            }
        }

        public void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            _frameTest.Output("In DispatcherException - State = " + _frameNavEventsTest.ToString());
            _frameTest.Output("Dispatcher exception = " + e.Exception);

            _frameTest.Fail("Caught exception in dispatcher");
        }

        public void NavigationProgress(object sender, NavigationProgressEventArgs e)
        {
            _frameTest.Output("In NavigationProgress - State = " + _frameNavEventsTest.ToString());
            // if you try to call StopLoading() in the frame here you get an exception
        }
    }
}
