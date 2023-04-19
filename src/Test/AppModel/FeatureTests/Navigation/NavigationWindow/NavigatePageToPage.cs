// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Verify page to page navigation
//   
//  Step1 - Set the StartupUri to page1
//  Step2 - Navigate to page2 using NavigationWindow.Navigate
// 
using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test.Logging;               // TestLog, TestStage

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class NavigatePageToPage
    {
        private enum State 
        {
            FirstNav, 
            SecondNav
        }

        private State _curState = State.FirstNav;
        private const String page1 = "NavigatePageToPage_Page1.xaml";
        private const String page2 = "NavigatePageToPage_Page2.xaml";

        public void Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavigatePageToPage");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Initialize);

            NavigationHelper.ExpectedFileName = page2;

            NavigationHelper.SetStage(TestStage.Run);
            Application.Current.StartupUri = new Uri(page1, UriKind.RelativeOrAbsolute);

            // Set the expected navigation counts
            int navStates = (int)State.SecondNav + 1; // number of navigation states
            NavigationHelper.NumExpectedNavigatingEvents = navStates; 
            NavigationHelper.NumExpectedNavigatedEvents = navStates; 
            NavigationHelper.NumExpectedLoadCompletedEvents = navStates;
        }


        public void Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.NumActualNavigatingEvents++;

            if (source != null)
            {
                Type t = source.GetType();
                if (t != null)
                {
                    NavigationHelper.Output("Type is " + t.ToString());
                }
                else
                {
                    NavigationHelper.Output("Type is null");
                }
            }
            else
            {
                NavigationHelper.Output("source is null");
            }
        }

        public void Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.NumActualNavigatedEvents++;
        }

        public void LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired on application");
            NavigationHelper.NumActualLoadCompletedEvents++;
            NavigationHelper.Output("Number of windows is: " + Application.Current.Windows.Count.ToString());
            if (_curState == State.FirstNav)
            {
                this._curState = State.SecondNav;
                NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
                if (nw != null)
                {
                    NavigationHelper.Output("Navigating nw to " + page2);
                    nw.Navigate(new Uri(page2, UriKind.RelativeOrAbsolute));
                }
                else 
                {
                    NavigationHelper.Fail("Could not get NavigationWindow");
                }
            }
            else if (_curState == State.SecondNav)
            {
                if (Application.Current.Windows.Count == 1)
                {
                    NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                }
                else
                {
                    NavigationHelper.Fail("Wrong number of windows: " + Application.Current.Windows.Count.ToString());
                }
            }
        }
    }
}

