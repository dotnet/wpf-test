// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Verify that frame journal retains the initial nav page after Navigating to NULL
//  Step 1 - Instantiate a page with a frame where startup Uri is SingleFrameTestPage.xaml
//  Step 2 - Navigate frame to controlsPage
//  Step 3 - Navigate frame to NULL 
//          (In one variation call NavigationWindow.RemoveBackEntry() that should throw
//           an exception when go back is called)
//  Step 4 - Navigate frame to hlinkPage
//  Step 5 - Go back on frame
//  Step 6 - Verify the frame page is controlsPage
//  Step 7 - Navigate parent to NULL and verify the Uri is NULL
//           Verify that parent can also navigate to NULL after frame has navigated to NULL
//  Step 8 - Compare the frame event counts against expected
//  Note - Navigating to NULL should still raise events  
//
//  Creator: Gamage
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.Logging;           

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class FrameNavigateToNull
    {
        // state of the test
        private enum State
        {
            FirstNav,
            FrameNavigateToNull,
            SecondNav,
            GoBack,
            ParentNavigateToNull,
            EndNav
        }

        private State _testState = State.FirstNav;
        // Parent start page is SingleFrameTestPage.xaml
        // navigation pages for the frame
        private const String hlinkPage = "HyperlinkPage_Loose.xaml";
        private const String controlsPage = "ContentControls_Page.xaml";

        private SingleFrameTestClass _frameTest = null;
        private string _nullVariation = ""; // identifies how to implement navigate to null
        private bool _removeBackEntry; // if true call RemoveBackEntry

        public void Startup(object sender, StartupEventArgs e)
        {
            // instantiate the frameTest object
            _frameTest = new SingleFrameTestClass("FrameNavigateToNull");

            // retrive the arguments and set the nullVariation and removeBackEntry
            NavigationHelper.Output("Testing for the null variation of " + DriverState.DriverParameters["NullVariation"]);
            _nullVariation = DriverState.DriverParameters["NullVariation"];

            NavigationHelper.Output("Testing for RemoveBackEntry = " + DriverState.DriverParameters["RemoveBackEntry"]);
            if (DriverState.DriverParameters["RemoveBackEntry"].ToLower() == "true")
            {
                _removeBackEntry = true;
            }
            else
            {
                _removeBackEntry = false;
            }

            // Set the expected navigation counts
            // only in four states we expect frame events getting fired
            NavigationHelper.NumExpectedNavigatedEvents = 4;
            NavigationHelper.NumExpectedLoadCompletedEvents = 4;
        }

        // LoadCompleted event handler
        public void LoadCompleted(object source, NavigationEventArgs e)
        {
            Frame frame = null;
            NavigationHelper.Output("-----State = " + _testState.ToString());
            NavigationHelper.Output("LoadCompleted event fired");

            if (e.Uri != null)
            {
                NavigationHelper.Output("uri is: " + e.Uri.ToString());
            }

            // after parent navigating to null, we don't have any content 
            if (_testState == State.EndNav)
            {
                if (_frameTest.NavigationWindow.Content != null)
                {
                    NavigationHelper.Fail("Found content after parent navigating to null");
                }
                else
                {
                    // verify event counts and exit
                    NavigationHelper.FinishTest(true);
                    return;
                }
            }

            frame = _frameTest.GetFrame(); // get the frame object
            if (frame == null)
            {
                NavigationHelper.Fail("Could not get frame element");
            }

            switch (_testState)
            {
                case State.FirstNav:
                    // set the JournalOwnership to the frame, this helps with visual verification
                    frame.JournalOwnership = JournalOwnership.OwnsJournal;

                    // register for frame event handlers
                    frame.LoadCompleted += new LoadCompletedEventHandler(FrameLoadCompleted);
                    frame.Navigated += new NavigatedEventHandler(FrameNavigated);

                    // setting the Source property should navigate to the page
                    frame.Source = new Uri(controlsPage, UriKind.RelativeOrAbsolute);
                    NavigationHelper.Output("Setting the frame.Source to " + controlsPage);
                    _testState = State.FrameNavigateToNull;
                    break;

                case State.FrameNavigateToNull:
                    NavigationHelper.Output("Navigating frame to NULL");

                    // Navigate to NULL depending on the given variation
                    switch (_nullVariation)
                    {
                        case "Uri":
                            NavigationHelper.Output("Calling frame.Navigate((Uri)null)");
                            frame.Navigate((Uri)null);
                            break;
                        case "Object":
                            NavigationHelper.Output("Calling frame.Navigate((object)null)");
                            frame.Navigate((object)null);
                            break;
                        case "Source":
                            NavigationHelper.Output("Setting frame.Source = null");
                            frame.Source = null;
                            break;
                    }
                    _testState = State.SecondNav;

                    break;

                case State.SecondNav:
                    NavigationHelper.Output("Navigating to hlinkPage");
                    // call RemoveBackEntry
                    if (_removeBackEntry == true)
                    {
                        NavigationHelper.Output("Calling frame.RemoveBackEntry()");
                        frame.RemoveBackEntry();
                    }
                    frame.Navigate(new Uri(hlinkPage, UriKind.RelativeOrAbsolute));
                    _testState = State.GoBack;
                    break;

                case State.GoBack:
                    NavigationHelper.Output("Calling GoBack on the frame");
                    frame.GoBack();
                    _testState = State.ParentNavigateToNull;
                    break;

                case State.ParentNavigateToNull:
                    NavigationHelper.Output("Navigating parent to NULL");

                    NavigationWindow nw = _frameTest.NavigationWindow;
                    if (nw == null)
                    {
                        NavigationHelper.Fail("Could not get navigation window");
                    }

                    // Navigate to NULL depending on the given variation
                    switch (_nullVariation)
                    {
                        case "Uri":
                            NavigationHelper.Output("Calling nw.Navigate((Uri)null)");
                            nw.Navigate((Uri)null);
                            break;
                        case "Object":
                            NavigationHelper.Output("Calling nw.Navigate((object)null)");
                            nw.Navigate((object)null);
                            break;
                        case "Source":
                            NavigationHelper.Output("Setting nw.Source = null");
                            nw.Source = null;
                            break;
                    }
                    _testState = State.EndNav;

                    break;
            }
        }

        // LoadCompleted event handler for the frame
        protected void FrameLoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("FrameLoadCompleted event fired. State - " + _testState);
            NavigationHelper.NumActualLoadCompletedEvents++;
        }

        public void Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired. State - " + _testState);
        }

        // Navigated event handler for the frame
        protected void FrameNavigated(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("FrameNavigated event fired. State - " + _testState);
            NavigationHelper.NumActualNavigatedEvents++;

            // validate the frame content
            Frame frame = _frameTest.GetFrame();

            if (frame == null)
            {
                NavigationHelper.Fail("Could not get frame element");
            }

            if (_testState == State.SecondNav) 
            {
                // if frame navigated to NULL, don't expect content
                if (frame.Content != null)
                {
                    NavigationHelper.Fail("Found content in the frame after navigating to NULL");
                }
            }
            else
            {
                // in all other cases we want to see content
                if (frame.Content == null)
                {
                    NavigationHelper.Fail("Found no content in the frame");
                }
            }

            // verify the Uri after GoBack is called
            if (_testState == State.ParentNavigateToNull)
            {
                if (String.Compare(controlsPage, e.Uri.ToString()) != 0)
                {
                    NavigationHelper.Fail("Unexpected Uri found - " + e.Uri.ToString());
                }
            }
        }

        // we expect to catch System.InvalidOperationException when RemoveBackEntry is called
        public void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Output("State = " + _testState);
            e.Handled = true;

            if (_testState == State.GoBack)
            {
                if (_removeBackEntry == true && e.Exception is System.InvalidOperationException)
                {
                    NavigationHelper.Pass("Expected System.InvalidOperationException caught");
                }
                else
                {
                    NavigationHelper.Fail("Failed due to unexpected dispatcher exception");
                }
            }
            else
            {
                NavigationHelper.Fail("Failed due to unexpected dispatcher exception");
            }
        }
    }
}

