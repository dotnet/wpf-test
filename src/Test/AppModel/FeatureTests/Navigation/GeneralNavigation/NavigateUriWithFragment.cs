// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//

using System;
using System.Windows;
using System.Windows.Controls;                  // Frame, Button
using System.Windows.Navigation;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum NavigateUriWithFragment_State
        {
            InitialNav, 
            Navigated
        }

        NavigateUriWithFragment_State _navigateUriWithFragment_curState = NavigateUriWithFragment_State.InitialNav;

        void NavigateUriWithFragment_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavigateUriWithFragment");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize);

            NavigationHelper.ExpectedTestCount = 5;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"NavigateUriWithFragment_Page1.xaml#anchorTag5";

            //NavigationHelper.Output("Registering application-level eventhandlers");
            //this.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedAPP);

            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri(@"NavigateUriWithFragment_Page1.xaml", UriKind.RelativeOrAbsolute);
            //base.OnStartup(e);
        }

        public void NavigateUriWithFragment_LoadCompleted(object source, NavigationEventArgs e)
        {
            FrameworkElement el;
            Frame f = e.Navigator as Frame;
            NavigationWindow nwSource = e.Navigator as NavigationWindow;

            NavigationHelper.Output("LoadCompleted event fired on application");
            NavigationHelper.ActualTestCount++;

            if (f != null)
            {
                // do nothing for frame navigations
                return;
            }
            if ((nwSource != null) && (_navigateUriWithFragment_curState == NavigateUriWithFragment_State.Navigated))
                NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));

            if ((nwSource == null) && (f == null))
                NavigationHelper.Fail("Could not get NavigationWindow nor Frame as e.Navigator");

            el = nwSource.Content as FrameworkElement;
            if (el == null)
                NavigationHelper.Fail("Could not get NavigationWindow.Content as FrameworkElement - it is null");

            if ((_navigateUriWithFragment_curState == NavigateUriWithFragment_State.InitialNav) &&
                (e.IsNavigationInitiator) && (NavigationHelper.getFileName(e.Uri) == "NavigateUriWithFragment_Page1.xaml"))
            {
                // attach events to navwin
                NavigationHelper.Output("Attaching NavigationWindow-level eventhandlers");
                nwSource.Navigating += new NavigatingCancelEventHandler(OnNavigating_NavigateUriWithFragment_nw);
                nwSource.Navigated += new NavigatedEventHandler(OnNavigated_NavigateUriWithFragment_nw);
                nwSource.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompleted_NavigateUriWithFragment_nw);

                _navigateUriWithFragment_curState = NavigateUriWithFragment_State.Navigated;
                NavigationHelper.Output("Creating Uri with just a fragment and Navigating to it");
                Uri newUri = new Uri("#anchorTag5", UriKind.RelativeOrAbsolute);
                NavigationHelper.Output("Calling Navigate on NavigationWindow with anchor uri");
                nwSource.Navigate(newUri);
            }
            else if (_navigateUriWithFragment_curState != NavigateUriWithFragment_State.Navigated)
                NavigationHelper.Fail("NavigateUriWithFragment_curState is unknown: " + _navigateUriWithFragment_curState.ToString());

            // otherwise, something else was the loadcompleted source - this is fine
        }

        public void OnNavigating_NavigateUriWithFragment_nw(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
        }

        public void OnNavigated_NavigateUriWithFragment_nw(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));
            NavigationWindow nwSource = e.Navigator as NavigationWindow;

            /* if ((nwSource != null) && (NavigateUriWithFragment_curState == State.Navigated))
            {
                NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.expectedFileName, e.Uri)) && (e.Content != null));
            } */
        }

        public void OnLoadCompleted_NavigateUriWithFragment_nw(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));
        }

        
    }
}

