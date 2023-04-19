// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Verify the journal retains the initial nav page after Navigating to NULL
//  Step 1 - Navigate to controlsPage
//  Step 2 - Navigate to NULL (In one variation call NavigationWindow.RemoveBackEntry())
//  Step 3 - Navigate to hlinkPage
//  Step 4 - Go back
//  Step 5 - Verify the page is controlsPage and compare navigation event counts
//  Note - Navigating to NULL should still raise events  
//
//

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class NavigateToNull
    {
        private enum State
        {
            FirstNav,
            NavigateToNull,
            SecondNav,
            EndNav
        }

        private State _testState = State.FirstNav;
        private const String hlinkPage = "HyperlinkPage_Loose.xaml";
        private const String controlsPage = "ContentControls_Page.xaml";
        private bool _removeBackEntry; // if true call RemoveBackEntry
        private string _nullVariation = ""; // identifies how to implement navigate to null

        public void Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("NavigateToNull");
            NavigationHelper.Output("Initializing TestLog...");
            NavigationHelper.SetStage(TestStage.Run);

            // retrive the arguments and set the nullVariation and removeBackEntry
            NavigationHelper.Output("Testing for the null variation of " + DriverState.DriverParameters["NullVariation"]);
            _nullVariation = DriverState.DriverParameters["NullVariation"];

            NavigationHelper.Output("Testing for RemoveBackEntry = " + DriverState.DriverParameters["RemoveBackEntry"]);
            if (DriverState.DriverParameters["RemoveBackEntry"] == "True")
            {
                _removeBackEntry = true;
            }
            else
            {
                _removeBackEntry = false;
            }

            NavigationHelper.ExpectedFileName = controlsPage;
            Application.Current.StartupUri = new Uri(controlsPage, UriKind.RelativeOrAbsolute);

            // Set the expected navigation counts
            int navStates = (int)State.EndNav + 1; // number of navigation states
            NavigationHelper.NumExpectedNavigatedEvents = navStates;
            NavigationHelper.NumExpectedLoadCompletedEvents = navStates;
        }

        public void Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired. State - " + _testState);
            NavigationHelper.NumActualNavigatedEvents++;

            if (_testState == State.NavigateToNull)
            {
                if (e.Content != null)
                {
                    NavigationHelper.Fail("Found content after navigating to NULL");
                }
            }
        }

        public void LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired. State - " + _testState);
            NavigationHelper.NumActualLoadCompletedEvents++;

            if (e.Uri != null)
            {
                NavigationHelper.Output("Uri is: " + e.Uri.ToString());
            }
            else
            {
                NavigationHelper.Output("Uri is null ");
            }

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                switch (_testState)
                {
                    case State.FirstNav:
                        _testState = State.NavigateToNull;
                        NavigationHelper.Output("Navigating to NULL");
    
                        // Navigate to NULL depending on the given variation
                        switch (_nullVariation)
                        {
                            case "Uri":
                                NavigationHelper.Output("Setting nw.Navigate((Uri)null)");
                                nw.Navigate((Uri)null);
                                break;
                            case "Object":
                                NavigationHelper.Output("Setting nw.Navigate((object)null)");
                                nw.Navigate((object)null);
                                break;
                            case "Source":
                                NavigationHelper.Output("Setting nw.Source = null");
                                nw.Source = null;
                                break;
                        }

                        break;

                    case State.NavigateToNull:
                        _testState = State.SecondNav;
                        // call RemoveBackEntry
                        if (_removeBackEntry == true)
                        {
                            NavigationHelper.Output("Calling nw.RemoveBackEntry()");
                            nw.RemoveBackEntry();
                        }

                        NavigationHelper.Output("Navigating to hlinkPage");
                        nw.Navigate(new Uri(hlinkPage, UriKind.RelativeOrAbsolute));

                        break;

                    case State.SecondNav:
                        _testState = State.EndNav;
                        NavigationHelper.Output("Calling GoBack from NavigationWindow");
                        nw.GoBack();
                        break;

                    case State.EndNav:
                        NavigationHelper.FinishTest((NavigationHelper.CompareFileNames(NavigationHelper.ExpectedFileName, e.Uri)) && (e.Content != null));
                        break;
                }

            }
            else
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }

        // we expect to catch System.InvalidOperationException when RemoveBackEntry is called
        public void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Output("State = " + _testState);
            e.Handled = true;

            if (_testState == State.EndNav)
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
