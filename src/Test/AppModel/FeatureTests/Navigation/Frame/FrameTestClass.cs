// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// The FrameBaseClass is instantiated by each specific Frame navigation test
    /// and provides the following members and methods common to all tests that 
    /// exercise Frame API.
    /// </summary>
    public class FrameTestClass : NavigationBaseClass
    {
        public enum FrameType
        {
            Frame,
            IslandFrame
        }
        
        public enum NavEvent
        {
            FragmentNavigation,
            LoadCompleted,
            Navigating,
            NavigationProgress,
            NavigationStopped
        }

        #region globals
        protected NavigationWindow  navWin         = null;
        protected Frame             frame          = null;
        protected Frame             islandFrame_local    = null;

        private const String        FRAMENAME       = "testFrame";
        private const String        ISLANDFRAMENAME = "testIslandFrame";
        private const String        FRAMEPAGE = @"FrameTestPage.xaml";

        private bool                _registerNavEventHandlers   = true;
        private bool                _recordEventOrder           = true;
        private FrameType           _frameType                  = FrameType.Frame;
        #endregion


        public FrameTestClass(String userGivenTestName) : base(userGivenTestName)
        {
            Application.Current.StartupUri = new Uri(FRAMEPAGE, UriKind.RelativeOrAbsolute);

            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);
        }

        public FrameTestClass() : base("DefaultFrameTest")
        {
        }

        public void SetupTest()
        {
            if (this.IsFirstRun)
            {
                Output("Grabbing reference to NavigationWindow.");
                navWin = Application.Current.MainWindow as NavigationWindow;

                Output("Grabbing reference to Frame.");
                frame = LogicalTreeHelper.FindLogicalNode(navWin.Content as DependencyObject, FRAMENAME) as Frame;

                if (_registerNavEventHandlers)
                {
                    Output("Registering Frame eventhandlers.");
                    frame.Navigating += new NavigatingCancelEventHandler(OnFrameNavigating);
                    frame.Navigated += new NavigatedEventHandler(OnFrameNavigated);
                    frame.FragmentNavigation += new FragmentNavigationEventHandler(OnFrameFragmentNavigation);
                    frame.NavigationProgress += new NavigationProgressEventHandler(OnFrameNavigationProgress);
                    frame.NavigationStopped += new NavigationStoppedEventHandler(OnFrameNavigationStopped);
                    frame.LoadCompleted += new LoadCompletedEventHandler(OnFrameLoadCompleted);
                    frame.ContentRendered += new EventHandler(OnFrameContentRendered);
                    frame.NavigationFailed += new NavigationFailedEventHandler(OnFrameNavigationFailed);
                }

                Output("Grabbing reference to IslandFrame.");
                islandFrame_local = LogicalTreeHelper.FindLogicalNode(navWin.Content as DependencyObject, ISLANDFRAMENAME) as Frame;

                if (_registerNavEventHandlers)
                {
                    Output("Registering IslandFrame eventhandlers.");
                    islandFrame_local.Navigating += new NavigatingCancelEventHandler(OnIslandFrameNavigating);
                    islandFrame_local.Navigated += new NavigatedEventHandler(OnIslandFrameNavigated);
                    islandFrame_local.FragmentNavigation += new FragmentNavigationEventHandler(OnIslandFrameFragmentNavigation);
                    islandFrame_local.NavigationProgress += new NavigationProgressEventHandler(OnIslandFrameNavigationProgress);
                    islandFrame_local.NavigationStopped += new NavigationStoppedEventHandler(OnIslandFrameNavigationStopped);
                    islandFrame_local.LoadCompleted += new LoadCompletedEventHandler(OnIslandFrameLoadCompleted);
                    islandFrame_local.ContentRendered += new EventHandler(OnIslandFrameContentRendered);
                    islandFrame_local.NavigationFailed += new NavigationFailedEventHandler(OnIslandFrameNavigationFailed);
                }
            }
        }

        // Unregister the given event handler
        public void UnregisterNavEventHandler(NavEvent navEvent)
        {
            switch (navEvent)
            {
                case NavEvent.FragmentNavigation:
                    frame.FragmentNavigation -= new FragmentNavigationEventHandler(OnFrameFragmentNavigation);
                    islandFrame_local.FragmentNavigation -= new FragmentNavigationEventHandler(OnIslandFrameFragmentNavigation);
                    break;
                case NavEvent.LoadCompleted:
                    frame.LoadCompleted -= new LoadCompletedEventHandler(OnFrameLoadCompleted);
                    islandFrame_local.LoadCompleted -= new LoadCompletedEventHandler(OnIslandFrameLoadCompleted);
                    break;
                case NavEvent.Navigating:
                    frame.Navigating -= new NavigatingCancelEventHandler(OnFrameNavigating);
                    islandFrame_local.Navigating -= new NavigatingCancelEventHandler(OnIslandFrameNavigating);
                    break;
                case NavEvent.NavigationProgress:
                    frame.NavigationProgress -= new NavigationProgressEventHandler(OnFrameNavigationProgress);
                    islandFrame_local.NavigationProgress -= new NavigationProgressEventHandler(OnIslandFrameNavigationProgress);
                    break;
                case NavEvent.NavigationStopped:
                    frame.NavigationStopped -= new NavigationStoppedEventHandler(OnFrameNavigationStopped);
                    islandFrame_local.NavigationStopped -= new NavigationStoppedEventHandler(OnIslandFrameNavigationStopped);
                    break;
            }
        }

        // Register the given event handler
        public void RegisterNavEventHandler(NavEvent navEvent)
        {
            switch (navEvent)
            {
                case NavEvent.FragmentNavigation:
                    frame.FragmentNavigation += new FragmentNavigationEventHandler(OnFrameFragmentNavigation);
                    islandFrame_local.FragmentNavigation += new FragmentNavigationEventHandler(OnIslandFrameFragmentNavigation);
                    break;
                case NavEvent.LoadCompleted:
                    frame.LoadCompleted += new LoadCompletedEventHandler(OnFrameLoadCompleted);
                    islandFrame_local.LoadCompleted += new LoadCompletedEventHandler(OnIslandFrameLoadCompleted);
                    break;
                case NavEvent.Navigating:
                    frame.Navigating += new NavigatingCancelEventHandler(OnFrameNavigating);
                    islandFrame_local.Navigating += new NavigatingCancelEventHandler(OnIslandFrameNavigating);
                    break;
                case NavEvent.NavigationProgress:
                    frame.NavigationProgress += new NavigationProgressEventHandler(OnFrameNavigationProgress);
                    islandFrame_local.NavigationProgress += new NavigationProgressEventHandler(OnIslandFrameNavigationProgress);
                    break;
                case NavEvent.NavigationStopped:
                    frame.NavigationStopped += new NavigationStoppedEventHandler(OnFrameNavigationStopped);
                    islandFrame_local.NavigationStopped += new NavigationStoppedEventHandler(OnIslandFrameNavigationStopped);
                    break;
            }
        }

        #region properties
        public bool RegisterNavEventHandlers
        {
            get
            {
                return _registerNavEventHandlers;
            }
            set
            {
                _registerNavEventHandlers = value;
            }
        }

        public Frame StdFrame
        {
            get
            {
                return frame;
            }
        }

        public Frame IslandFrame
        {
            get
            {
                return islandFrame_local;
            }
        }

        public NavigationWindow NavigationWindow
        {
            get
            {
                return navWin;
            }
        }

        public FrameType FrameTestType
        {
            get
            {
                return _frameType;
            }
            set
            {
                _frameType = value;
            }
        }
        #endregion

        #region Frame eventhandlers : counters

        protected void OnFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Output("Frame :: NAVIGATING event caught");
            NavigationHelper.NumActualNavigatingEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigating");
        }

        protected void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            Output("Frame :: NAVIGATED event caught");
            NavigationHelper.NumActualNavigatedEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigated");
        }

        protected void OnFrameFragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            Output("Frame :: FRAGMENTNAVIGATION event caught");
            NavigationHelper.NumActualFragmentNavigationEvents++;

            if (_recordEventOrder)
                AddActualEvent("fragmentnavigation");
        }

        /// <summary>
        /// OnFrameNavigationProgress gets triggered by each Navigation progressed Event 
        /// </summary>
        /// <param name="sender">object can be System.Windows.Controls.Frame</param>
        /// <param name="e">NavigationProgressEventArgs</param>
        protected void OnFrameNavigationProgress(object sender, NavigationProgressEventArgs e)
        {
            Output(string.Format("Frame :: NAVIGATIONPROGRESS caught: URI: {0} ", e.Uri));
            Output(string.Format("Frame :: NAVIGATIONPROGRESS event caught {0} of {1} bytes retrieved", e.BytesRead, e.MaxBytes));

            /// Increment the event count only if the event is raised while navigating to the frame 
            /// we are trying to navigate to - NavigationHelper.ExpectedFileName
            if (true == NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri))
            {
                NavigationHelper.NumActualNavigationProgressEvents++;
                NavigationHelper.NumMaxBytesExpectedNavigationProgressEvent = (int)e.MaxBytes;
            }

            if (_recordEventOrder)
                AddActualEvent("navigationprogress");
        }

        protected void OnFrameNavigationStopped(object sender, NavigationEventArgs e)
        {
            Output("Frame :: NAVIGATIONSTOPPED event caught");
            NavigationHelper.NumActualNavigationStoppedEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigationstopped");
        }

        protected void OnFrameLoadCompleted(object sender, NavigationEventArgs e)
        {
            Output("Frame :: LOADCOMPLETED event caught");
            NavigationHelper.NumActualLoadCompletedEvents++;

            if (_recordEventOrder)
                AddActualEvent("loadcompleted");
        }

        protected void OnFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Output("Frame :: NAVIGATIONFAILED event caught");
            NavigationHelper.NumActualNavigationFailedEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigationfailed");
        }

        protected void OnFrameContentRendered(object sender, EventArgs e)
        {
            Output("Frame :: CONTENTRENDERED event caught");
            // we don't keep track of ContentRendered events fired

            if (_recordEventOrder)
                AddActualEvent("contentrendered");
        }

        #endregion

        #region IslandFrame eventhandlers : counters

        protected void OnIslandFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            Output("IslandFrame :: NAVIGATING event caught");
            NavigationHelper.NumActualNavigatingEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigating");
        }

        protected void OnIslandFrameNavigated(object sender, NavigationEventArgs e)
        {
            Output("IslandFrame :: NAVIGATED event caught");
            NavigationHelper.NumActualNavigatedEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigated");
        }

        protected void OnIslandFrameFragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            Output("IslandFrame :: FRAGMENTNAVIGATION event caught");
            NavigationHelper.NumActualFragmentNavigationEvents++;

            if (_recordEventOrder)
                AddActualEvent("fragmentnavigation");
        }

        protected void OnIslandFrameNavigationProgress(object sender, NavigationProgressEventArgs e)
        {
            Output("IslandFrame :: NAVIGATIONPROGRESS event caught");
            NavigationHelper.NumActualNavigationProgressEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigationprogress");
        }

        protected void OnIslandFrameNavigationStopped(object sender, NavigationEventArgs e)
        {
            Output("IslandFrame :: NAVIGATIONSTOPPED event caught");
            NavigationHelper.NumActualNavigationStoppedEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigationstopped");
        }

        protected void OnIslandFrameLoadCompleted(object sender, NavigationEventArgs e)
        {
            Output("IslandFrame :: LOADCOMPLETED event caught");
            NavigationHelper.NumActualLoadCompletedEvents++;

            if (_recordEventOrder)
                AddActualEvent("loadcompleted");
        }

        protected void OnIslandFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Output("IslandFrame :: NAVIGATIONFAILED event caught");
            NavigationHelper.NumActualNavigationFailedEvents++;

            if (_recordEventOrder)
                AddActualEvent("navigationfailed");
        }

        protected void OnIslandFrameContentRendered(object sender, EventArgs e)
        {
            Output("IslandFrame :: CONTENTRENDERED event caught");
            // we don't keep track of ContentRendered events fired

            if (_recordEventOrder)
                AddActualEvent("contentrendered");
        }

        #endregion


    }
}
