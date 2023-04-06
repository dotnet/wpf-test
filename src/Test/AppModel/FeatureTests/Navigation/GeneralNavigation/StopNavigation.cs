// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Testing stop navigation scenarios
//  
//  Test for following page navigations by calling StopLoading or setting e.Cancel = true
//
//  a) Compiled XAML page
//  b) loose XAML page at site of origin
//  c) HTML page at site of origin
//  d) Web Site (http://www.google.com) in standalone application case
//  e) Loose XAML page in a component
//  f) Loose XAML content page (compiled with build type Content)
//  

using System;
using System.Windows;
using System.Windows.Navigation;
using Microsoft.Test; 
using Microsoft.Test.Logging;               
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class StopNavigation
    {
        private enum State
        {
            InitialNav, 
            EndNav
        }

        private State _curState = State.InitialNav;
        private const String initialPage = "StopNavigation_Page1.xaml";
        private const String compiledXamlPage = "StopNavigation_Page2.xaml";
        private const String xamlSiteOfOrigin = "pack://siteoforigin:,,,/Page1_Loose.xaml";
        private const String xamlComponent = @"XamlComponent;component/ComponentPages/PageA.xaml";
        private const String xamlLooseContent = "pack://application:,,,/FlowDocument_Loose.xaml";
        private const String htmlSiteOfOrigin = "pack://siteoforigin:,,,/WebPage1_Loose.html";
        private const String google = "http://www.google.com/";

        private String _navigationPage = String.Empty; // Navigation page variation to run
        private bool _stopLoading; // if true call StopLoading else set e.Cancel = true

        public void Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("StopNavigation");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Run);

            NavigationHelper.ExpectedFileName = initialPage;
            Application.Current.StartupUri = new Uri(initialPage, UriKind.RelativeOrAbsolute);

            // retrieve the test variation to run
            _navigationPage = DriverState.DriverParameters["NavigationPage"];
            NavigationHelper.Output("Testing for the navigation variation " + _navigationPage);

            if (String.Compare(DriverState.DriverParameters["StopLoading"].ToLower(), "true") == 0)
            {
                _stopLoading = true;
            }
            else
            {
                _stopLoading = false;
            }

            // expected navigation event counts
            NavigationHelper.NumExpectedLoadCompletedEvents = 1;
            NavigationHelper.NumExpectedNavigatedEvents = 1;
            NavigationHelper.NumExpectedNavigatingEvents = 2;
            if (_stopLoading)
            {
                // NavigationStopped is fired when StopLoading is called
                NavigationHelper.NumExpectedNavigationStoppedEvents = 1;
            }
            else
            {
                NavigationHelper.NumExpectedNavigationStoppedEvents = 0;
            }
        }

        public void Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired. State = " + _curState);
            NavigationHelper.NumActualNavigatingEvents++;

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                // Navigation can be stopped either by calling StopLoading or setting e.Cancel = true
                if (_curState == State.EndNav)
                {
                    if (_stopLoading)
                    {
                        NavigationHelper.Output("Calling StopLoading");
                        nw.StopLoading();
                    }
                    else
                    {
                        NavigationHelper.Output("Setting e.Cancel = true");
                        e.Cancel = true;
                    }
                }
            }
        }

        public void Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired. State = " + _curState);
            NavigationHelper.NumActualNavigatedEvents++;
        }

        public void NavigationStopped(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("NavigationStopped event fired. State = " + _curState);
            NavigationHelper.NumActualNavigationStoppedEvents++;

            if (this._curState == State.EndNav)
            {
                NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
            }
            else
            {
                NavigationHelper.Fail("NavigationStopped with invalid state: " + _curState.ToString());
            }
        }

        public void ContentRendered(object sender, EventArgs e)
        {
            NavigationHelper.Output("ContentRendered event fired. State = " + _curState);

            if (_stopLoading == false)
            {
                // when you set e.Cancel = true, ContentRendered event will get fired
                // and NavigationStopped event doesn't get fired
                if (this._curState == State.EndNav)
                {
                    NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
                    Uri uri = BaseUriHelper.GetBaseUri(nw.Content as DependencyObject);
                    NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, uri)) && (nw.Content != null));
                }
                else
                {
                    NavigationHelper.Fail("ContentRendered with invalid state: " + _curState.ToString());
                }
            }
        }

        public void LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired. State = " + _curState);
            NavigationHelper.NumActualLoadCompletedEvents++;
            NavigationHelper.Output("uri is: " + e.Uri.ToString());

            if (_curState == State.InitialNav)
            {
                _curState = State.EndNav;
                switch (_navigationPage.ToLower())
                {
                    case "compiledxamlpage": // compiled XAML page
                        NavigateToPage(compiledXamlPage);
                        break;

                    case "xamlsiteoforigin": // loose XAML at site of origin
                        NavigateToPage(xamlSiteOfOrigin);
                        break;

                    case "xamlcomponent": // loose XAML page in a component
                        NavigateToPage(xamlComponent);
                        break;

                    case "xamlloosecontent": // loose XAML content page (compiled with build type Content)
                        NavigateToPage(xamlLooseContent);
                        break;

                    case "htmlsiteoforigin": // html file at site of origin
                        NavigateToPage(htmlSiteOfOrigin);
                        break;

                    case "http": // navigate to a web site
                        NavigateToPage(google);
                        break;

                    default:
                        NavigationHelper.Fail("Unsupported navigation page type - " + _navigationPage.ToLower());
                        break;
                }
            }
        }

        // Navigate to the given page
        private void NavigateToPage(String page)
        {
            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                NavigationHelper.Output("Calling Navigate on " + page);
                nw.Navigate(new Uri(page, UriKind.RelativeOrAbsolute));
            }
            else
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }
    }
}

