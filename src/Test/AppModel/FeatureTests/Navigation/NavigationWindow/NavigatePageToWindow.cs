// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//

using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        internal enum NavigatePageToWindow_State 
        {
            FirstNav, 
            SecondNav
        }
        NavigatePageToWindow_State _navigatePageToWindow_curState = NavigatePageToWindow_State.FirstNav;

        protected void NavigatePageToWindow_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavigatePageToWindow");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize);

            NavigationHelper.ExpectedTestCount = 8;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"NavigatePageToWindow_Page2.xaml";

            //NavigationHelper.Output("Registering application-level eventhandlers.");
            //this.Navigating += new NavigatingCancelEventHandler(OnNavigating);
            //this.Navigated += new NavigatedEventHandler(OnNavigated);
            //this.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedAPP);

            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri(@"NavigatePageToWindow_Page1.xaml", UriKind.RelativeOrAbsolute);
            //base.OnStartup(e);
        }

        public void NavigatePageToWindow_ContentRendered(object source, EventArgs e)
        {
            NavigationHelper.Output("ContentRendered event fired");
            NavigationHelper.ActualTestCount++;
        }

        public void NavigatePageToWindow_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;

            if (source != null)
            {
                Type t = source.GetType();
                if (t != null)
                    NavigationHelper.Output("Type is " + t.ToString());
                else
                    NavigationHelper.Output("Type is null");
            }
            else
            {
                NavigationHelper.Output("source is null");
            }
        }

        public void NavigatePageToWindow_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;
        }

        public void NavigatePageToWindow_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired on application");
            NavigationHelper.ActualTestCount++;

            if (_navigatePageToWindow_curState == NavigatePageToWindow_State.FirstNav)
            {
                this._navigatePageToWindow_curState = NavigatePageToWindow_State.SecondNav;
                NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
                if (nw != null)
                {
                    NavigationHelper.Output("Navigating to secondWindowPageCB.xaml");
                    nw.Navigate(new Uri( "secondWindowPageCB.xaml", UriKind.RelativeOrAbsolute));
                }
                else 
                    NavigationHelper.Fail("Could not get NavigationWindow");
            }
            else if (_navigatePageToWindow_curState == NavigatePageToWindow_State.SecondNav)
            {
                if (this.Windows.Count == 1)
                    NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                    // NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.expectedFileName, new Uri("secondNavWindowPage.xaml",UriKind.RelativeOrAbsolute))) && (e.Content != null));
                else 
                    NavigationHelper.Fail("Wrong number of windows: " + this.Windows.Count.ToString());
            }
        }
    }
}

