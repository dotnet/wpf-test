// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//


using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // TapPage
    public partial class NavigationTests : Application
    {
        internal enum TapPage_CurrentTest
        {
            InitialNav,
            NavigatedToSecondPage,
            NavigatedToThirdPage,
            CalledGoBack,
            CalledGoForward,
            CalledGoForward2
        }

        TapPage_CurrentTest _tapPage_curState = TapPage_CurrentTest.InitialNav;

        void TapPage_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("TapPageProperties");
            Application.Current.StartupUri = new Uri("TapPage_Page1.xaml", UriKind.RelativeOrAbsolute);

            NavigationHelper.ExpectedTestCount = 13;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"TapPage_Page3.xaml";

            NavigationHelper.Output("Registering application-level eventhandlers.");
            Application.Current.Navigating += new NavigatingCancelEventHandler(TapPage_Navigating);
            Application.Current.Navigated += new NavigatedEventHandler(TapPage_Navigated);
            //Application.Current.LoadCompleted += new LoadCompletedEventHandler(TapPage_LoadCompleted);

            NavigationHelper.SetStage(TestStage.Run);
        }

        void TapPage_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;
        }

        void TapPage_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;
        }

        void TapPage_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.ActualTestCount++;

            if (e.Uri == null)
            {
                NavigationHelper.Output("No uri here.  e.Content is " + e.Content.ToString());
            }
            else
            {
                NavigationHelper.Output("uri is: " + e.Uri.ToString());
                NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));
            }

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                switch (_tapPage_curState)
                {
                    case TapPage_CurrentTest.InitialNav:
                        _tapPage_curState = TapPage_CurrentTest.NavigatedToSecondPage;
                        NavigationHelper.Output("Calling Navigate on TapPage_secondPage.xaml ");
                        nw.Navigate(new Uri("TapPage_secondPage.xaml", UriKind.RelativeOrAbsolute));
                        break;

                    case TapPage_CurrentTest.NavigatedToSecondPage:
                        _tapPage_curState = TapPage_CurrentTest.NavigatedToThirdPage;
                        break;

                    case TapPage_CurrentTest.NavigatedToThirdPage:
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

