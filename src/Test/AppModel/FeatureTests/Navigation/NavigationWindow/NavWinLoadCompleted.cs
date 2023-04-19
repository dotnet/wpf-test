// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 


using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum NavWinLoadCompleted_State
        {
            InitialNav, 
            Navigated
        }

        NavWinLoadCompleted_State _navWinLoadCompleted_curState = NavWinLoadCompleted_State.InitialNav;

        void NavWinLoadCompleted_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavWinLoadCompleted");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize);

            NavigationHelper.ExpectedTestCount = 3;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"secondPage.xaml";

            //NavigationHelper.Output("Registering application-level eventhandlers");
            //this.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedAPP);

            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri("NavWinLoadCompleted_Page1.xaml", UriKind.RelativeOrAbsolute);
            //base.OnStartup(e);
        }


        public void NavWinLoadCompleted_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;
        }

        public void NavWinLoadCompleted_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired on application");
            NavigationHelper.ActualTestCount++;

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                if ((_navWinLoadCompleted_curState == NavWinLoadCompleted_State.InitialNav) && (e.IsNavigationInitiator))
                {
                    nw.LoadCompleted += new LoadCompletedEventHandler(NavWinLoadCompleted_LoadCompletedWin);

                    _navWinLoadCompleted_curState = NavWinLoadCompleted_State.Navigated;
                    NavigationHelper.Output("Calling Navigate on NavWinLoadCompleted_Page2.xaml");
                    nw.Navigate(new Uri("NavWinLoadCompleted_Page2.xaml", UriKind.RelativeOrAbsolute));
                }
                else if (_navWinLoadCompleted_curState == NavWinLoadCompleted_State.Navigated)
                {
                    NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                }
                else if (_navWinLoadCompleted_curState != NavWinLoadCompleted_State.Navigated)
                {
                    NavigationHelper.Fail("curState is unknown: " + _navWinLoadCompleted_curState.ToString());
                }
            }
            else 
            {
                NavigationHelper.Fail("Could not get navigationwindow");
            }
        }

        public void NavWinLoadCompleted_LoadCompletedWin(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));
        }
    }
}

