// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Test case for NavigationFailed event
//
//  Step1 : Navigate to NavigationFailed_Page1.xaml
//  Step2 : Navigate to nonexisting page (NavigationFailed_NoneExistingPage.xaml), 
//            Verify NavigationFailed event fired
//  Step3 : Navigate to a page with a frame (NavigationFailed_Frame.xaml)
//  Step4 : In the frame navigate to a non existing page and Verify NavigationFailed event fired
//  Step5 : In the frame navigate to a fragment of a page (FragmentNavigation_Page2.xaml#fragment2)
//  Step6 : In the frame navigate to a non existing fragment (FragmentNavigation_Page2.xaml#nonexisting)
//  Step7 : Navigate to a non existing external Uri and Verify NavigationFailed event fired
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;   // TestLog, TestStage
using System.Net;
using System.IO.Packaging;   

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        // NavigationFailed States
        internal enum NavigationFailed_State 
        {
            Page,
            PageNonExisting,
            Frame,
            FrameNonExisting,
            Fragment,
            FragmentNonExisting,
            ExternalNonExisting,
            End
        }

        private NavigationFailed_State _navigationFailed_curState = NavigationFailed_State.Page;
        private const string NavigationFailed_TESTFRAME = "testFrame"; // name of the frame in NavigationFailed_Frame.xaml 
        private const string NavigationFailed_Page1 = "NavigationFailed_Page1.xaml"; // page 1 
        private const string NavigationFailed_Frame = "NavigationFailed_Frame.xaml"; // Frame 
        private const string NavigationFailed_Fragment2 = "FragmentNavigation_Page2.xaml#fragment2"; // Fragment2 in FragmentNavigation_Page2.xaml
        private const string NavigationFailed_NonExistingFragment = "FragmentNavigation_Page2.xaml#nonexisting"; // NonExisting Fragment in FragmentNavigation_Page2.xaml
        private const string NavigationFailed_NonExisting = "NavigationFailed_NoneExistingPage.xaml"; // Non existing page 
        private const string NavigationFailed_Page3 = "page3.xaml"; // page inside NavigationFailed_Frame.xaml
        private string _navigationFailed_ExternalUri = ""; // external non existing page 
        private bool _browserHostedApp = false; 

        // Startup event handler
        public void NavigationFailed_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavigationFailed");
            NavigationHelper.Output("Initializing TestLog...");

            NavigationHelper.SetStage(TestStage.Run);

            // determine if standalone or browser hosted
            if (AppDomain.CurrentDomain.FriendlyName.ToString().Contains(Microsoft.Test.Loaders.ApplicationDeploymentHelper.BROWSER_APPLICATION_EXTENSION))
            {
                _browserHostedApp = true;
            }

            // Actual event counts
            NavigationHelper.NumActualNavigatingEvents = 0;
            NavigationHelper.NumActualNavigatedEvents = 0;
            NavigationHelper.NumActualNavigationFailedEvents = 0;
            NavigationHelper.NumActualLoadCompletedEvents = 0;

            // Expected event counts
            if (_browserHostedApp == true)
            {
                NavigationHelper.NumExpectedNavigatingEvents = 7;
            }
            else
            {
                NavigationHelper.NumExpectedNavigatingEvents = 8;
            }
            NavigationHelper.NumExpectedNavigatedEvents = 5;
            if (_browserHostedApp == true)
            {
                NavigationHelper.NumExpectedNavigationFailedEvents = 3;
            }
            else
            {
                NavigationHelper.NumExpectedNavigationFailedEvents = 4;
            }
            NavigationHelper.NumExpectedLoadCompletedEvents = 5;

            // set the external URI
            _navigationFailed_ExternalUri = Microsoft.Test.Loaders.FileHost.HttpInternetBaseUrl + @"NonExisting.xaml";
            this.StartupUri = new Uri(NavigationFailed_Page1, UriKind.RelativeOrAbsolute);
        }

        // Navigating event handler
        public void NavigationFailed_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired - State = " + _navigationFailed_curState);
            NavigationHelper.NumActualNavigatingEvents++;
        }

        // Navigated event handler
        public void NavigationFailed_Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired - State = " + _navigationFailed_curState);
            NavigationHelper.NumActualNavigatedEvents++;

            NavigationHelper.Output("Uri at state " + _navigationFailed_curState + " is " + e.Uri.ToString());

            // verify Uri after navigation 
            switch (_navigationFailed_curState)
            {
                case NavigationFailed_State.Page:
                    if (String.Compare(e.Uri.ToString(), NavigationFailed_Page1) != 0)
                    {
                        NavigationHelper.Fail("Unexpected Uri after Page");
                    }
                    break;

                case NavigationFailed_State.Frame:
                    // we are navigating to a page with a frame
                    if (e.Navigator is NavigationWindow)
                    {
                        if (String.Compare(e.Uri.ToString(), NavigationFailed_Frame) != 0)
                        {
                            NavigationHelper.Fail("Unexpected Uri after Frame");
                        }
                    }
                    else if (e.Navigator is Frame)
                    {
                        if (String.Compare(e.Uri.ToString(), NavigationFailed_Page3) != 0)
                        {
                            NavigationHelper.Fail("Unexpected Uri after Frame");
                        }
                    }
                    break;

                case NavigationFailed_State.Fragment:
                    if (String.Compare(e.Uri.ToString(), NavigationFailed_Fragment2) != 0)
                    {
                        NavigationHelper.Fail("Unexpected Uri after Fragment");
                    }
                    break;

                case NavigationFailed_State.FragmentNonExisting:
                    if (String.Compare(e.Uri.ToString(), NavigationFailed_Fragment2) != 0)
                    {
                        NavigationHelper.Fail("Unexpected Uri after FragmentNonExisting");
                    }
                    break;
            }
        }

        // LoadCompleted event handler
        public void NavigationFailed_LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.NumActualLoadCompletedEvents++;
            NavigationHelper.Output("LoadCompleted event fired on application - State = " + _navigationFailed_curState);

            if (_navigationFailed_curState == NavigationFailed_State.Page)
            {
                _navigationFailed_curState = NavigationFailed_State.PageNonExisting;
                NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
                if (nw != null)
                {
                    NavigationHelper.Output("Navigating nw to none existing page");
                    nw.Navigate(new Uri(NavigationFailed_NonExisting, UriKind.RelativeOrAbsolute));
                }
                else 
                {
                    NavigationHelper.Fail("Couldn't retrieve the navigation window (null)");
                }
            }
            else if (_navigationFailed_curState == NavigationFailed_State.Frame)
            {
                // we have navigated to a page with a frame
                Frame frame = null;
                NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
                if (nw != null)
                {
                    frame = LogicalTreeHelper.FindLogicalNode(nw.Content as DependencyObject, NavigationFailed_TESTFRAME) as Frame;
                }
                else
                {
                    NavigationHelper.Fail("Couldn't retrieve the navigation window (null)");
                }

                // register with ContentRendered and NavigationFailed event handler
                if (frame != null)
                {
                    frame.ContentRendered += new EventHandler(NavigationFailed_OnFrameContentRendered);
                    frame.NavigationFailed += new NavigationFailedEventHandler(NavigationFailed_OnFrameNavigationFailed);
                }
                else
                {
                    NavigationHelper.Fail("Couldn't retrieve the frame (null)");
                }
            }
        }

        // Navigation failed event handler
        public void NavigationFailed_NavigationFailed(object source, NavigationFailedEventArgs e)
        {
            NavigationHelper.NumActualNavigationFailedEvents++;
            NavigationHelper.Output("NavigationFailed event fired");

            // check the EventArgs
            NavigationFailed_CheckEventArgs(e);
            e.Handled = true;

            // navigation failure is expected, now navigate to a different page
            if (_navigationFailed_curState == NavigationFailed_State.PageNonExisting)
            {
                // Navigate to a page with a frame
                _navigationFailed_curState = NavigationFailed_State.Frame;
                NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
                if (nw != null)
                {
                    NavigationHelper.Output("Navigating nw to a page with a frame");
                    nw.Navigate(new Uri(NavigationFailed_Frame, UriKind.RelativeOrAbsolute));
                }
                else
                {
                    NavigationHelper.Fail("Could not get NavigationWindow");
                }
            }
            else if (_navigationFailed_curState == NavigationFailed_State.End)
            {
                NavigationHelper.FinishTest(true);
            }
        }

        // Frame ContentRendered event handler
        private void NavigationFailed_OnFrameContentRendered(object sender, EventArgs e)
        {
            NavigationHelper.Output("Frame::ContentRendered event fired - State = " + _navigationFailed_curState);

            switch (_navigationFailed_curState)
            {
                case NavigationFailed_State.Frame:
                    // content is rendered successfully in the frame
                    // now try to navigate to a non existing page
                    if (sender is Frame)
                    {
                        Frame frame = sender as Frame;
                        if (frame != null)
                        {
                            // Navigate to a none existing page
                            _navigationFailed_curState = NavigationFailed_State.FrameNonExisting;
                            frame.Navigate(new Uri(NavigationFailed_NonExisting, UriKind.RelativeOrAbsolute));
                        }
                    }
                    else
                    {
                        NavigationHelper.Fail("sender is not Frame after Frame");
                    }
                    break;

                case NavigationFailed_State.Fragment:
                    // content is rendered successfully in the frame
                    // now try to navigate to a non existing fragment
                    if (sender is Frame)
                    {
                        Frame frame = sender as Frame;
                        if (frame != null)
                        {
                            // Navigate to a none existing fragment
                            // Navigation shouldn't fail even if the fragment doesn't exist
                            _navigationFailed_curState = NavigationFailed_State.FragmentNonExisting;
                            frame.Navigate(new Uri(NavigationFailed_NonExistingFragment, UriKind.RelativeOrAbsolute));
                        }
                    }
                    else
                    {
                        NavigationHelper.Fail("sender is not Frame after Fragment");
                    }
                    break;

                case NavigationFailed_State.FragmentNonExisting:
                    // now try to navigate to a non existing external Uri
                    NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
                    if (nw != null)
                    {
                        NavigationHelper.Output("Navigating nw to none existing external Uri " + _navigationFailed_ExternalUri);
                        _navigationFailed_curState = NavigationFailed_State.End;
                        nw.Navigate(new Uri(_navigationFailed_ExternalUri, UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        NavigationHelper.Fail("Couldn't retrieve the navigation window (null)");
                    }
                    break;
            }
        }

        // Frame NavigationFailed event handler
        private void NavigationFailed_OnFrameNavigationFailed(object source, NavigationFailedEventArgs e)
        {
            NavigationHelper.NumActualNavigationFailedEvents++;
            NavigationHelper.Output("Frame::NavigationFailed event fired - State = " + _navigationFailed_curState);

            // check the EventArgs
            NavigationFailed_CheckEventArgs(e);
            e.Handled = true;

            // navigation failure is expected, now navigate to a different page
            // You get into this event handler twice because of the frame, so take the next action based on NumActualNavigationFailedEvents
            if (_navigationFailed_curState == NavigationFailed_State.FrameNonExisting && NavigationHelper.NumActualNavigationFailedEvents == 3)
            {            
                if (e.Navigator is Frame)
                {
                    Frame frame = e.Navigator as Frame;
                    if (frame != null)
                    {
                        // Navigate to a fragement
                        _navigationFailed_curState = NavigationFailed_State.Fragment;
                        frame.Navigate(new Uri(NavigationFailed_Fragment2, UriKind.RelativeOrAbsolute));
                    }
                }
                else
                {
                    NavigationHelper.Fail("e.Navigator is not Frame after FrameNonExisting");
                }
            }
        }

        // DispatcherUnhandledException
        void NavigationFailed_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if (_browserHostedApp == true)
            {
                NavigationHelper.Output("In DispatcherUnhandledException");
                if (_navigationFailed_curState == NavigationFailed_State.End &&
                    e.Exception is System.Security.SecurityException)
                {
                    // browser hosted app should fail navigating to an external Uri
                    NavigationHelper.Output("Got expected SecurityException on navigating to an external Uri");
                    e.Handled = true;
                    NavigationHelper.FinishTest(true);
                }
                else
                {
                    NavigationHelper.Fail("Got unexpected exception. Test fail");
                }

                Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
            }
            else
            {
                NavigationHelper.Fail("Don't expect DispatcherUnhandledException from Standalone");
            }
        }

        // Check the EventArgs
        private void NavigationFailed_CheckEventArgs(NavigationFailedEventArgs e)
        {
            NavigationHelper.Output("  *NavigationFailedEventArgs* - State = " + _navigationFailed_curState);
            if (e != null)
            {
                NavigationHelper.Output("    Navigator is: " + ((e.Navigator != null) ? e.Navigator.ToString() : "null"));
                NavigationHelper.Output("    Uri is: " + ((e.Uri != null) ? e.Uri.ToString() : "null"));
                NavigationHelper.Output("    Exception Type is: " + ((e.Exception.GetType() != null) ? e.Exception.GetType().ToString() : "null"));
                NavigationHelper.Output("    Exception Source is: " + ((e.Exception.Source != null) ? e.Exception.Source.ToString() : "null"));
                NavigationHelper.Output("    Exception Message is: " + ((e.Exception.Message != null) ? e.Exception.Message.ToString() : "null"));
                NavigationHelper.Output("    WebRequest is       : " + ((e.WebRequest != null) ? e.WebRequest.ToString() : "null"));
                NavigationHelper.Output("    WebResponse is      : " + ((e.WebResponse!= null) ? e.WebResponse.ToString() : "null"));
                NavigationHelper.Output("    ExtraData is        : " + ((e.ExtraData!= null) ? e.ExtraData.ToString() : "null"));
            }
            else
            {
                NavigationHelper.Fail("NavigationFailedEventArgs is null");
            }

            // verify the NavigationFailedEventArgs for each state
            switch (_navigationFailed_curState)
            {
                case NavigationFailed_State.PageNonExisting:
                    if (_browserHostedApp == true)
                    {
                        if (String.Compare(e.Navigator.ToString(), "MS.Internal.AppModel.RootBrowserWindow") != 0)
                        {
                            NavigationHelper.Fail("Unexpected Navigator, expected: MS.Internal.AppModel.RootBrowserWindow");
                        }
                    }
                    else
                    {
                        if (String.Compare(e.Navigator.ToString(), "System.Windows.Navigation.NavigationWindow") != 0)
                        {
                            NavigationHelper.Fail("Unexpected Navigator, expected: System.Windows.Navigation.NavigationWindow");
                        }
                    }
                    if (String.Compare(e.Uri.ToString(), "pack://application:,,,/NavigationFailed_NoneExistingPage.xaml") != 0)
                    {
                        NavigationHelper.Fail("Unexpected Uri, expected: pack://application:,,,/NavigationFailed_NoneExistingPage.xaml");
                    }
                    if (e.Exception.GetType() != typeof(System.IO.IOException))
                    {
                        NavigationHelper.Fail("Unexpected Exception Type, expected: System.IO.IOException");
                    }
                    if (String.Compare(e.Exception.Source.ToString(), "PresentationFramework") != 0)
                    {
                        NavigationHelper.Fail("Unexpected Exception Source, expected: PresentationFramework");
                    }
                    if (System.Globalization.CultureInfo.CurrentCulture.Name == "en-US") // checking only for en-US should be sufficient
                    {
                        if (String.Compare(e.Exception.Message.ToString(), "Cannot locate resource 'navigationfailed_noneexistingpage.xaml'.") != 0)
                        {
                            NavigationHelper.Fail("Unexpected Exception Message, expected: Cannot locate resource 'navigationfailed_noneexistingpage.xaml'.");
                        }
                    }
                    break;

                case NavigationFailed_State.FrameNonExisting:
                    if (String.Compare(e.Navigator.ToString(), "System.Windows.Controls.Frame: NavigationFailed_NoneExistingPage.xaml") != 0)
                    {
                        NavigationHelper.Fail("Unexpected Navigator, expected: System.Windows.Controls.Frame: NavigationFailed_NoneExistingPage.xaml");
                    }
                    if (String.Compare(e.Uri.ToString(), "pack://application:,,,/NavigationFailed_NoneExistingPage.xaml") != 0)
                    {
                        NavigationHelper.Fail("Unexpected Uri, expected: pack://application:,,,/NavigationFailed_NoneExistingPage.xaml");
                    }
                    if (e.Exception.GetType() != typeof(System.IO.IOException))
                    {
                        NavigationHelper.Fail("Unexpected Exception Type, expected: System.IO.IOException");
                    }
                    if (String.Compare(e.Exception.Source.ToString(), "PresentationFramework") != 0)
                    {
                        NavigationHelper.Fail("Unexpected Exception Source, expected: PresentationFramework");
                    }
                    if (System.Globalization.CultureInfo.CurrentCulture.Name == "en-US") // checking only for en-US should be sufficient
                    {
                        if (String.Compare(e.Exception.Message.ToString(), "Cannot locate resource 'navigationfailed_noneexistingpage.xaml'.") != 0)
                        {
                            NavigationHelper.Fail("Unexpected Exception Message, expected: Cannot locate resource 'navigationfailed_noneexistingpage.xaml'.");
                        }
                    }
                    break;

                case NavigationFailed_State.End:
                    if (String.Compare(e.Navigator.ToString(), "System.Windows.Navigation.NavigationWindow") != 0)
                    {
                        NavigationHelper.Fail("Unexpected Navigator, expected: System.Windows.Navigation.NavigationWindow");
                    }
                    if (String.Compare(e.Uri.ToString(), "http://wpf.redmond.corp.microsoft.com/testscratch/NonExisting.xaml") != 0)
                    {
                        NavigationHelper.Fail("Unexpected Uri, expected: http://wpf.redmond.corp.microsoft.com/testscratch/NonExisting.xaml");
                    }
                    if (e.Exception.GetType() != typeof(System.Net.WebException))
                    {
                        NavigationHelper.Fail("Unexpected Exception Type, expected: System.Net.WebException");
                    }
                    if (String.Compare(e.Exception.Source.ToString(), "System") != 0)
                    {
                        NavigationHelper.Fail("Unexpected Exception Source, expected: System");
                    }
                    if (System.Globalization.CultureInfo.CurrentCulture.Name == "en-US") // checking only for en-US should be sufficient
                    {
                        if (String.Compare(e.Exception.Message.ToString(), "The remote server returned an error: (404) Not Found.") != 0)
                        {
                            NavigationHelper.Fail("Unexpected Exception Message, expected: The remote server returned an error: (404) Not Found.");
                        }
                    }
                    /// Ensure that this NavigationFailed Event was raised by the last request we made 
                    HttpWebRequest httpWebRequest = (HttpWebRequest)e.WebRequest;
                    if (String.Compare(httpWebRequest.RequestUri.ToString(), "http://wpf.redmond.corp.microsoft.com/testscratch/NonExisting.xaml") != 0)
                    {
                        NavigationHelper.Fail("Unexpected WebRequest.RequestUri: " + httpWebRequest.RequestUri.ToString());
                    }
                    if (e.WebResponse != null)
                    {
                        NavigationHelper.Output("Unexpected: WebResponse. It should be NULL When navigation fails"); 
                    }
		    if (e.ExtraData != null) 
		    {
                        NavigationHelper.Output("Unexpected: ExtraData" + e.ExtraData.ToString()); 
		    }
                    break;

                default:
                    NavigationHelper.Fail("Don't expect navigation to fail at state = " + _navigationFailed_curState);
                    break;
            }
        }
    }
}

