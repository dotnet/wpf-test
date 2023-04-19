// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  BackForwardApp tests that we can navigate to a second page
//                defined in XAML, GoBack() to the initial page, and then
//                GoForward() to return to the second page.  This test verifies
//                that we receive the correct number of events during these
//                navigations, that we end up at the correct location and that//                the correct NavigationModes are set when we navigate fwd/back. 


using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // BackForwardApp
    public partial class NavigationTests : Application
    {
        internal enum BackForwardApp_CurrentTest
        {
            InitialNav,
            Navigated,
            CalledGoBack,
            CalledGoForward
        }

        #region BackForwardApp globals
        BackForwardApp_CurrentTest _backFwdTest = BackForwardApp_CurrentTest.InitialNav;
        #endregion

        void BackForwardApp_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("BackForwardApp");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize);

            NavigationHelper.ExpectedTestCount = 14;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"secondPage.xaml";

            NavigationHelper.SetStage(TestStage.Run);
            this.StartupUri = new Uri("page1.xaml", UriKind.RelativeOrAbsolute);
            base.OnStartup(e);
        }

        void BackForwardApp_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
            if (e.NavigationMode == NavigationMode.Forward)
            {
                NavigationHelper.Output("GoingForward NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationHelper.Output("GoingBack NavigationMode set");
                NavigationHelper.ActualTestCount++;
            }
        }

        void BackForwardApp_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;
        }

        void BackForwardApp_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                switch (_backFwdTest)
                {
                    case BackForwardApp_CurrentTest.InitialNav:
                        _backFwdTest = BackForwardApp_CurrentTest.Navigated;
                        NavigationHelper.Output("Calling Navigate on secondPage.xaml");
                        nw.Navigate(new Uri("secondPage.xaml", UriKind.RelativeOrAbsolute));
                        break;

                    case BackForwardApp_CurrentTest.Navigated:
                        _backFwdTest = BackForwardApp_CurrentTest.CalledGoBack;
                        NavigationHelper.Output("Calling GoBack from NavigationWindow");
                        nw.GoBack();
                        break;

                    case BackForwardApp_CurrentTest.CalledGoBack:
                        _backFwdTest = BackForwardApp_CurrentTest.CalledGoForward;
                        NavigationHelper.Output("Calling GoForward from NavigationWindow");
                        nw.GoForward();
                        break;

                    case BackForwardApp_CurrentTest.CalledGoForward:
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }

            }
            else
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }

    }
}
