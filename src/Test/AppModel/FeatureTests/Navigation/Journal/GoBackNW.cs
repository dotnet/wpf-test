// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Verify NavigationMode in navigating back
//   
//  Step1 - Set the StartupUri to page1
//  Step2 - Navigate to page2 using NavigationWindow.Navigate
//  Step3 - call NavigationWindow.GoBack()
// 

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // GoBackNW
    public class GoBackNW
    {
        private enum CurrentTest
        {
            InitialNav,
            Navigated,
            CalledGoBack
        }

        private CurrentTest _goBackNWTest = CurrentTest.InitialNav;
        private const String flowDocPage = "FlowDocument_Loose.xaml";
        private const String controlsPage = "ContentControls_Page.xaml";

        // expected and actual GoBack counts
        private int _expectedGoBackCount;
        private int _actualGoBackCount = 0;

        public void Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("GoBackNW");
            Application.Current.StartupUri = new Uri(flowDocPage, UriKind.RelativeOrAbsolute);

            NavigationHelper.ExpectedFileName = flowDocPage;
            NavigationHelper.SetStage(TestStage.Run);

            // Set the expected navigation counts
            _expectedGoBackCount = 1;
            int navStates = (int)CurrentTest.CalledGoBack + 1; // number of navigation states
            NavigationHelper.NumExpectedNavigatingEvents = navStates; 
            NavigationHelper.NumExpectedNavigatedEvents = navStates; 
            NavigationHelper.NumExpectedLoadCompletedEvents = navStates;
        }

        public void Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            NavigationHelper.NumActualNavigatingEvents++;
            if (e.NavigationMode == NavigationMode.Back && _goBackNWTest == CurrentTest.CalledGoBack)
            {
                NavigationHelper.Output("GoingBack NavigateionMode set");
                _actualGoBackCount++;
            }
        }

        public void Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            NavigationHelper.NumActualNavigatedEvents++;
        }

        public void LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired ON APPLICATION");
            NavigationHelper.NumActualLoadCompletedEvents++;

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw == null)
            {
                NavigationHelper.Fail("Could not grab a reference to the NavigationWindow");
            }

            if (nw != null && e.Navigator is NavigationWindow)
            {
                NavigationHelper.Output("In LoadCompleted; Navigator is NavigationWindow");

                NavigationHelper.Output("uri is: " + e.Uri.ToString());
                NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

                switch (_goBackNWTest)
                {
                    case CurrentTest.InitialNav:
                        _goBackNWTest = CurrentTest.Navigated;
                        NavigationHelper.Output("Calling Navigate on " + controlsPage);
                        nw.Navigate(new Uri(controlsPage, UriKind.RelativeOrAbsolute));
                        break;

                    case CurrentTest.Navigated:
                        _goBackNWTest = CurrentTest.CalledGoBack;
                        nw.GoBack();
                        break;

                    case CurrentTest.CalledGoBack:
                        // verify go back counts
                        if (_actualGoBackCount != _expectedGoBackCount)
                        {
                            NavigationHelper.Fail("Actual GoBack count = " + _actualGoBackCount + "; Expected GoBack count = " + _expectedGoBackCount); 
                        }
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }
            }
            else
            {
                NavigationHelper.Fail("Navigator is not NavigationWindow");
            }
        }
    }
}
