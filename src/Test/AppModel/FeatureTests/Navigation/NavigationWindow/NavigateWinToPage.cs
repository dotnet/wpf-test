// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: 
//

using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        private void NavigateWinToPage_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavigateWinToPage");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize);

            NavigationHelper.ExpectedTestCount = 6;
            NavigationHelper.ActualTestCount = 0;
            NavigationHelper.ExpectedFileName = @"NavigateWinToPage_Page2.xaml";

            //NavigationHelper.Output("Registering application-level eventhandlers");
            //this.Navigating += new NavigatingCancelEventHandler(OnNavigating);
            //this.Navigated += new NavigatedEventHandler(OnNavigated);
            //this.LoadCompleted += new LoadCompletedEventHandler(OnLoadCompletedAPP);

            NavigationHelper.SetStage(TestStage.Run);
            NavigationWindow nwFirst = new NavigationWindow();
            nwFirst.Visibility = Visibility.Visible;
            nwFirst.Navigate(new Uri(@"NavigateWinToPage_Page1.xaml", UriKind.RelativeOrAbsolute));
        }


        public void NavigateWinToPage_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.ActualTestCount++;

            Type t = e.Navigator.GetType();
            NavigationHelper.Output("Type is " + t.ToString());
        }

        public void NavigateWinToPage_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.ActualTestCount++;
        }

        public void NavigateWinToPage_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationWindow nwSource = e.Navigator as NavigationWindow;
            Window wSource = e.Navigator as Window;

            NavigationHelper.Output("LoadCompleted event fired on application");
            NavigationHelper.ActualTestCount++;

            NavigationHelper.Output("There are this many windows " + this.Windows.Count.ToString());
            if (this.Windows.Count != 2)
                NavigationHelper.Fail("Wrong number of windows");

            // source window should be a navigationwindow
            if (nwSource == null)
            {
                if (wSource == null)
                    NavigationHelper.Fail("Source was a Window, not a NavigationWindow");
                else 
                    NavigationHelper.Fail("Source was neither window nor NavigationWindow");
            }

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationHelper.Output("e.Navigator is navwindow");
            NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
        }
    }
}

