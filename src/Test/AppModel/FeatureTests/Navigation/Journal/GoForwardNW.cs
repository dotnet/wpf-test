// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Verify NavigationMode in navigating forward
//   
//  Step1 - Set the StartupUri to controlsPage
//  Step2 - Navigate to flowDocPage using NavigationWindow.Navigate
//  Step3 - call NavigationWindow.GoBack()
//  Step4 - call NavigationWindow.GoForward()
// 

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // GoForwardNW
    public class GoForwardNW
    {
        private enum CurrentTest
        {
            InitialNav,
            Navigated,
            CalledGoBack,
            CalledGoForward
        }

        private CurrentTest _goForwardNWTest = CurrentTest.InitialNav;
        private const String flowDocPage = "FlowDocument_Loose.xaml";
        private const String controlsPage = "ContentControls_Page.xaml";

        // expected and actual GoForward counts
        private int _expectedGoForwardCount;
        private int _actualGoForwardCount = 0;

        public void Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("GoForwardNW");
            Application.Current.StartupUri = new Uri(controlsPage, UriKind.RelativeOrAbsolute);
            NavigationHelper.ExpectedFileName = flowDocPage;

            NavigationHelper.SetStage(TestStage.Run);

            // Set the expected navigation counts
            _expectedGoForwardCount = 1;
            int navStates = (int)CurrentTest.CalledGoForward + 1; // number of navigation states
            NavigationHelper.NumExpectedNavigatingEvents = navStates;
            NavigationHelper.NumExpectedNavigatedEvents = navStates;
            NavigationHelper.NumExpectedLoadCompletedEvents = navStates;
        }

        public void Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.Output("-----State = " + _goForwardNWTest.ToString());
            NavigationHelper.NumActualNavigatingEvents++;
            if (e.NavigationMode == NavigationMode.Forward && _goForwardNWTest == CurrentTest.CalledGoForward)
            {
                NavigationHelper.Output("GoingForward NavigationMode set");
                _actualGoForwardCount++;
            }
        }

        public void Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.Output("-----State = " + _goForwardNWTest.ToString());
            NavigationHelper.NumActualNavigatedEvents++;
        }

        public void LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.Output("-----State = " + _goForwardNWTest.ToString());
            NavigationHelper.NumActualLoadCompletedEvents++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                if (_goForwardNWTest == CurrentTest.InitialNav)
                {
                    _goForwardNWTest = CurrentTest.Navigated;
                    NavigationHelper.Output("Calling Navigate on " + flowDocPage);
                    nw.Navigate(new Uri(flowDocPage, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    switch (_goForwardNWTest)
                    {
                        case CurrentTest.Navigated:
                            NavigationHelper.Output("State is Navigated: calling GoBack");
                            _goForwardNWTest = CurrentTest.CalledGoBack;
                            nw.GoBack();
                            break;

                        case CurrentTest.CalledGoBack:
                            NavigationHelper.Output("State is CalledGoBack: calling GoForward");
                            _goForwardNWTest = CurrentTest.CalledGoForward;
                            nw.GoForward();
                            break;

                        case CurrentTest.CalledGoForward:
                            // verify go forward counts
                            if (_actualGoForwardCount != _expectedGoForwardCount)
                            {
                                NavigationHelper.Fail("Actual GoForward count = " + _actualGoForwardCount + "; Expected GoForward count = " + _expectedGoForwardCount);
                            }
                            NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                            break;
                    }
                }
            }
        }
    }
}
