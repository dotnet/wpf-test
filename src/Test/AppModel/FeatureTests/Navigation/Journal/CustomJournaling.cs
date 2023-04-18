// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: The CustomJournaling BVT suite checks the functionality 
//  of the new journaling extensibility  
//

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // CustomJournaling
    public partial class NavigationTests : Application
    {
        internal enum CustomJournaling_CurrentTest
        {
            JournalEachChange,
            ReloadCurrentPage,
            GoBack,
            GoForward,
            NavigateAwayAndReturn,
            StopNavigationAndReload,
            RemoveBackEntryAndGoBack,
            NavigateToFragment
        }

        #region CustomJournaling globals
        private NavigationWindow _navWindow = null;
        private NavigationMode _navMode;

        private CustomJournaling_CurrentTest currentTest;
        private bool _firstTime = true;
        private int _whichSubTest = 0;

        #region variables keeping track of journal/page state
        private ArrayList _backStack = null;
        private ArrayList _fwdStack = null;
        private String _currLbxItem = String.Empty;
        private Page _pageOne = null;
        #endregion

        private const string CUSTOMJOURNALINGPAGE1 = "Journal_Page1.xaml";
        private const string CUSTOMJOURNALINGPAGE2 = "Journal_Page2.xaml";
        #endregion

        /// <summary>
        /// [1] Sets up TestHelper logging,
        /// [2] Initializes the private variables and grabs references to their values and 
        /// [3] Registers eventhandlers for the BVT
        /// </summary>
        void CustomJournaling_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("CustomJournaling");
            Application.Current.StartupUri = new Uri(CUSTOMJOURNALINGPAGE1, UriKind.RelativeOrAbsolute);

            // Set values of the private member variables
            NavigationHelper.Output("Initializing private member variables");
            _fwdStack = new ArrayList();
            if (_fwdStack == null)
            {
                NavigationHelper.Output("Could not create ArrayList for fwdStack");
                NavigationHelper.Fail("fwdStack successfully created", "fwdStack is NULL");
            }

            _backStack = new ArrayList();
            if (_backStack == null)
            {
                NavigationHelper.Output("Could not create ArrayList for backStack");
                NavigationHelper.Fail("backStack successfully created", "backStack is NULL");
            }

            NavigationHelper.SetStage(TestStage.Run);
            NavigationHelper.Output("Running CustomJournaling test.");
        }

        private void CustomJournaling_LoadCompleted(object source, NavigationEventArgs e)
        {
            if (_firstTime)
            {
                _navWindow = Application.Current.MainWindow as NavigationWindow;
                if (_navWindow == null)
                {
                    NavigationHelper.Output("Could not grab reference to current NavigationWindow");
                    NavigationHelper.Fail("navWindow reference grabbed successfully", "navWindow is NULL");
                }

                // Set the first item to be the currently selected ListBoxItem
                _pageOne = _navWindow.Content as Page;

                ListBox lbx = LogicalTreeHelper.FindLogicalNode(_pageOne, "userList") as ListBox;
                _currLbxItem = ((ListBoxItem)lbx.SelectedItem).Content.ToString();
            }
            else
            {
                String whichSubTestString = String.Empty;
                if (_navWindow.Source.Equals(CUSTOMJOURNALINGPAGE1))
                    UpdateLbxReference();

                // Verify stuff here!
                #region ReloadCurrentPage test case
                if (currentTest == CustomJournaling_CurrentTest.ReloadCurrentPage && 
                    _navMode == NavigationMode.Refresh)
                {
                    // Refreshing will lose "Bashful" state. Journal stacks should be the same
                    NavigationHelper.Output("Refreshed navWindow.  Currently selected item should be:  Sneezy");
                    VerifyJournalState("Sneezy");

                    NavigationHelper.Pass("[TEST 2] :: Reload current page, go back and go forward");
                }
                #endregion

                #region GoBack test case
                if (currentTest == CustomJournaling_CurrentTest.GoBack && 
                    _navMode == NavigationMode.Back)
                {
                    switch (_whichSubTest)
                    {
                        case 0: whichSubTestString = "Dopey"; break;
                        case 1: whichSubTestString = "Happy"; break;
                        case 2: whichSubTestString = "Sleepy"; break;
                        case 3: whichSubTestString = "Doc"; break;
                    }

                    NavigationHelper.Output("navWindow.GoBack() completed.  Currently selected item should be: " + whichSubTestString);
                    VerifyJournalState(whichSubTestString);

                    if (_whichSubTest == 3)
                        NavigationHelper.Pass("[TEST 3] :: Go back once and multiple times on custom journaled page");
                }
                #endregion

                #region GoForward test case
                if (currentTest == CustomJournaling_CurrentTest.GoForward && 
                    _navMode == NavigationMode.Forward)
                {
                    switch (_whichSubTest)
                    {
                        case 0: whichSubTestString = "PrinceCharming"; break;
                        case 1: whichSubTestString = "Doc"; break;
                        case 2: whichSubTestString = "Sleepy"; break;
                        case 3: whichSubTestString = "Happy"; break;
                    }

                    NavigationHelper.Output("navWindow.GoForward() completed.  Currently selected item should be: " + whichSubTestString);
                    VerifyJournalState(whichSubTestString);

                    if (_whichSubTest == 3)
                        NavigationHelper.Pass("[TEST 4] :: Go forward once and multiple times on custom journaled page");
                }
                #endregion

                #region NavigateAwayAndReturn test case
                if (currentTest == CustomJournaling_CurrentTest.NavigateAwayAndReturn && 
                    _navMode == NavigationMode.Back)
                {
                    NavigationHelper.Output("Checking that we're back at " + CUSTOMJOURNALINGPAGE1);
                    if (!_navWindow.CurrentSource.ToString().EndsWith(CUSTOMJOURNALINGPAGE1))
                    {
                        NavigationHelper.Fail("navWindow should be back at " + CUSTOMJOURNALINGPAGE1,
                            "navWindow is at " + _navWindow.CurrentSource.ToString());
                    }

                    VerifyJournalState("Bashful");
                    NavigationHelper.Pass("[TEST 5] :: Navigate away from page and GoBack");
                }

                #endregion

                #region StopNavigationAndReload test case
                if (currentTest == CustomJournaling_CurrentTest.StopNavigationAndReload)
                {
                    // Check that we are at page 1 still after canceling navigation and refreshing
                    if (!(_navWindow.CurrentSource.ToString().EndsWith(CUSTOMJOURNALINGPAGE1)))
                        NavigationHelper.Fail("navWindow should be at " + CUSTOMJOURNALINGPAGE1, "navWindow is at " + _navWindow.CurrentSource.ToString());

                    // Check that we've gone back to the default start state
                    VerifyJournalState("Sneezy");
                    NavigationHelper.Pass("[TEST 6] :: Cancel mid-navigation and reload page");
                }
                #endregion

                #region RemoveBackEntryAndGoBack test case
                if (currentTest == CustomJournaling_CurrentTest.RemoveBackEntryAndGoBack && 
                    _navMode == NavigationMode.Back)
                {
                    VerifyJournalState("Grumpy");
                    NavigationHelper.Pass("[TEST 7] :: Remove most recent back entry and GoBack");
                }
                #endregion

                #region NavigateToFragment
                if (currentTest == CustomJournaling_CurrentTest.NavigateToFragment && 
                    _navMode == NavigationMode.New)
                {
                    switch (_whichSubTest)
                    {
                        case 1: whichSubTestString = BOTTOMFRAGMENT; break;
                        case 2: whichSubTestString = TOPFRAGMENT; break;
                    }

                    // Check that we are on the proper fragment
                    if (_navWindow.CurrentSource.ToString().EndsWith(whichSubTestString))
                    {
                        NavigationHelper.Output("CurrentSource is at " + CUSTOMJOURNALINGPAGE1 + whichSubTestString + ".  Currently selected ListBoxItem: Bashful");
                        VerifyJournalState("Bashful");

                        if (whichSubTestString.Equals(BOTTOMFRAGMENT))
                        {
                            // Try to navigate to #top anchor on the current page
                            NavigationHelper.Output("Navigate to " + TOPFRAGMENT + " anchor on " + CUSTOMJOURNALINGPAGE1);
                            NavigateAndJournal(TOPFRAGMENT);
                        }
                        else if (whichSubTestString.Equals(TOPFRAGMENT))
                        {
                            NavigationHelper.Pass("[TEST 8] :: Navigate to fragment URI and GoBack");
                        }
                    }
                    else
                    {
                        NavigationHelper.Fail("navWindow CurrentSource should be " + CUSTOMJOURNALINGPAGE1 + whichSubTestString,
                            "navWindow CurrentSource is " + _navWindow.CurrentSource.ToString());
                    }
                }
                #endregion
            }
        }

        private void CustomJournaling_Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Saving NavigationMode");
            _navMode = e.NavigationMode;

            if (currentTest == CustomJournaling_CurrentTest.StopNavigationAndReload && 
                _navMode == NavigationMode.New)
            {
                // if we are here via initial navigation and not refresh,
                // pretend to cancel the navigation, then reload navWindow
                NavigationHelper.Output("Cancelling navigation and refreshing NavigationWindow");
                e.Cancel = true;
                _navWindow.Refresh();

                // WORKAROUND: Remove last entry in backStack, since we added it BEFORE navigating
                _backStack.RemoveAt(_backStack.Count - 1);
            }
        }


        private void CustomJournaling_ContentRendered(object sender, EventArgs e)
        {
            if (_firstTime)
            {
                _firstTime = false;

                #region route tests
                // Snag command line argument and route test based on that
                NavigationHelper.Output("Test to run: " + DriverState.DriverParameters["CustomJournalingTest"] );
                switch (DriverState.DriverParameters["CustomJournalingTest"])
                {
                    case "JournalEachChange":
                        NavigationHelper.Output("[TEST 1] :: Journal each ListBoxItem selection change on Page1");
                        currentTest = CustomJournaling_CurrentTest.JournalEachChange;
                        TestAndVerifyCustomJournaling();
                        break;

                    case "ReloadCurrentPage":
                        NavigationHelper.Output("[TEST 2] :: Reload current page, go back and go forward");
                        currentTest = CustomJournaling_CurrentTest.ReloadCurrentPage;
                        TestAndVerifyReloadingCurrentPage();
                        break;

                    case "GoBack":
                        NavigationHelper.Output("[TEST 3] :: Go back once and multiple times on custom journaled page");
                        currentTest = CustomJournaling_CurrentTest.GoBack;
                        TestAndVerifyGoBack();
                        break;

                    case "GoForward":
                        NavigationHelper.Output("[TEST 4] :: Go forward once and multiple times on custom journaled page");
                        currentTest = CustomJournaling_CurrentTest.GoForward;
                        TestAndVerifyGoForward();
                        break;

                    case "NavigateAwayAndReturn":
                        NavigationHelper.Output("[TEST 5] :: Navigate away from page and GoBack");
                        currentTest = CustomJournaling_CurrentTest.NavigateAwayAndReturn;
                        TestAndVerifyNavigateAndGoBack();
                        break;

                    case "StopNavigationAndReload":
                        NavigationHelper.Output("[TEST 6] :: Cancel mid-navigation and reload page");
                        currentTest = CustomJournaling_CurrentTest.StopNavigationAndReload;
                        TestAndVerifyStopAndReload();
                        break;

                    case "RemoveBackEntryAndGoBack":
                        NavigationHelper.Output("[TEST 7] :: Remove most recent back entry and GoBack");
                        currentTest = CustomJournaling_CurrentTest.RemoveBackEntryAndGoBack;
                        TestAndVerifyRemoveBackEntry();
                        break;

                    case "NavigateToFragment":
                        NavigationHelper.Output("[TEST 8] :: Navigate to fragment URI");
                        currentTest = CustomJournaling_CurrentTest.NavigateToFragment;
                        TestAndVerifyFragmentNavigation();
                        break;

                    default:
                        NavigationHelper.Output("Test is not defined.  Exiting.");
                        break;
                }
                #endregion
            }
            else
            {
                #region NavigateAwayAndReturn test case
                // Start second part of this test here
                if (currentTest == CustomJournaling_CurrentTest.NavigateAwayAndReturn && 
                    _navMode == NavigationMode.New)
                {
                    // Check that we are now on the second page
                    if (_navWindow.Source.ToString().Equals(CUSTOMJOURNALINGPAGE2))
                    {
                        NavigationHelper.Output("Navigation to " + CUSTOMJOURNALINGPAGE2 + " was successful.  Return to " + CUSTOMJOURNALINGPAGE1);
                        GoBackAndJournal();
                    }
                }
                #endregion
            }

            return;
        }

        #region general test helpers
        private void SetupTest()
        {
            // Set up test
            VerifyJournalState("Sneezy");
            JournalAndChangeItem("PrinceCharming");
            JournalAndChangeItem("Doc");
            JournalAndChangeItem("Sleepy");
            JournalAndChangeItem("Happy");
            JournalAndChangeItem("Dopey");
            JournalAndChangeItem("Bashful");

            NavigationHelper.Output("Checking that the currently selected item is Bashful");
            VerifyJournalState("Bashful");
        }


        private void UpdateLbxReference()
        {
            Page p1 = _navWindow.Content as Page;

            ListBox lbx = LogicalTreeHelper.FindLogicalNode(p1, "userList") as ListBox;
            _currLbxItem = ((ListBoxItem)lbx.SelectedItem).Content.ToString();
        }
        #endregion


        #region TEST 1 :: JournalEachChange
        /// <summary>
        /// Add a new journal entry for each state/ListBox selection
        /// </summary>
        private void TestAndVerifyCustomJournaling()
        {
            try
            {
                VerifyJournalState("Sneezy");

                NavigationHelper.Output("Try to RemoveBackEntry when back stack is empty");
                _navWindow.RemoveBackEntry();
                VerifyJournalState("Sneezy");

                NavigationHelper.Output("Changing ListBox's selected item 1x");
                JournalAndChangeItem("PrinceCharming");
                VerifyJournalState("PrinceCharming");

                NavigationHelper.Output("Changing ListBox's selected item 5x");
                JournalAndChangeItem("Doc");
                JournalAndChangeItem("Sleepy");
                JournalAndChangeItem("Happy");
                JournalAndChangeItem("Dopey");
                JournalAndChangeItem("Bashful");
                VerifyJournalState("Bashful");

                NavigationHelper.Output("Remove back entry");
                RemoveBackEntry("Dopey");
                VerifyJournalState("Bashful");

                NavigationHelper.Pass("[TEST 1] :: Journal each ListBoxItem selection change on Page1");
            }
            catch (Exception exp)
            {
                NavigationHelper.Fail("No exceptions should be thrown", "Caught unexpected exception:  " + exp.ToString());
            }
        }
        #endregion JournalEachChange

        #region TEST 2 :: ReloadCurrentPage
        private void TestAndVerifyReloadingCurrentPage()
        {
            try
            {
                NavigationHelper.Output("Setting up ReloadCurrentPage test");
                SetupTest();

                // Reload page -- turns control over to OnNavWindowNavigating, OnNavWindowContentRendered
                // and OnLoadCompleted_NavApp eventhandlers
                NavigationHelper.Output("Calling NavigationWindow.Refresh");
                _navWindow.Refresh();
            }
            catch (Exception exp)
            {
                NavigationHelper.Fail("No exceptions should be thrown", "Caught unexpected exception:  " + exp.ToString());
            }
        }
        #endregion ReloadCurrentPage

        #region TEST 3 :: GoBack
        private void TestAndVerifyGoBack()
        {
            try
            {
                NavigationHelper.Output("Setting up GoBack test.");
                SetupTest();

                // Go back once
                NavigationHelper.Output("GoBack called 1x");
                GoBackAndJournal();

                // Go back multiple times 
                NavigationHelper.Output("GoBack called 3x");
                GoBackAndJournal();
                GoBackAndJournal();
                GoBackAndJournal();
            }
            catch (Exception exp)
            {
                NavigationHelper.Fail("No exceptions should be thrown", "Caught unexpected exception:  " + exp.ToString());
            }
        }
        #endregion

        #region TEST 4 :: GoForward
        private void TestAndVerifyGoForward()
        {
            try
            {
                NavigationHelper.Output("Setting up GoForward test.");
                SetupTest();

                // Go back all the way
                while (_navWindow.CanGoBack)
                    GoBackAndJournal();

                // Reset whichSubTest
                _whichSubTest = 0;

                // Go forward once
                NavigationHelper.Output("GoForward called 1x");
                GoForwardAndJournal();

                // Go forward multiple times 
                NavigationHelper.Output("GoForward called 3x");
                GoForwardAndJournal();
                GoForwardAndJournal();
                GoForwardAndJournal();
            }
            catch (Exception exp)
            {
                NavigationHelper.Fail("No exceptions should be thrown", "Caught unexpected exception:  " + exp.ToString());
            }
        }
        #endregion

        #region TEST 5 :: NavigateAwayAndReturn
        private void TestAndVerifyNavigateAndGoBack()
        {
            try
            {
                NavigationHelper.Output("Setting up NavigateAwayAndReturn test.");
                SetupTest();

                NavigationHelper.Output("Navigating to " + CUSTOMJOURNALINGPAGE2);
                NavigateAndJournal(CUSTOMJOURNALINGPAGE2);
            }
            catch (Exception exp)
            {
                NavigationHelper.Fail("No exceptions should be thrown", "Caught unexpected exception:  " + exp.ToString());
            }
        }
        #endregion NavigateAwayAndReturn

        #region TEST 6 :: StopNavigationAndReload
        private void TestAndVerifyStopAndReload()
        {
            try
            {
                NavigationHelper.Output("Setting up StopNavigationAndReload test.");
                SetupTest();

                // Navigate to page 2, abort and then reload page 1
                NavigationHelper.Output("Navigating to " + CUSTOMJOURNALINGPAGE2);
                NavigateAndJournal(CUSTOMJOURNALINGPAGE2);

                // Control gets transferred to Navigating, ContentRendered eventhandlers
            }
            catch (Exception exp)
            {
                NavigationHelper.Fail("No exceptions should be thrown", "Caught unexpected exception:  " + exp.ToString());
            }

        }
        #endregion StopNavigationAndReload

        #region TEST 7 :: RemoveBackEntryAndGoBack
        private void TestAndVerifyRemoveBackEntry()
        {
            try
            {
                NavigationHelper.Output("Setting up RemoveEntryAndGoBack test");
                SetupTest();

                // Add a few more journal entries
                JournalAndChangeItem("Grumpy");
                JournalAndChangeItem("SnowWhite");
                JournalAndChangeItem("EvilStepmother");

                NavigationHelper.Output("Remove back entry");
                RemoveBackEntry("SnowWhite");

                NavigationHelper.Output("Attempt to GoBack");
                GoBackAndJournal();
            }
            catch (Exception exp)
            {
                NavigationHelper.Fail("No exceptions should be thrown", "Caught unexpected exception:  " + exp.ToString());
            }
        }
        #endregion RemoveBackEntryAndGoBack

        #region TEST 8 :: NavigateToFragment
        private void TestAndVerifyFragmentNavigation()
        {
            try
            {
                NavigationHelper.Output("Setting up NavigateToFragment test");
                SetupTest();

                // Try to navigate to #bottom anchor on the current page
                NavigationHelper.Output("Navigate to " + BOTTOMFRAGMENT + " anchor on " + CUSTOMJOURNALINGPAGE1);
                NavigateAndJournal(BOTTOMFRAGMENT);
            }
            catch (Exception exp)
            {
                NavigationHelper.Fail("No exceptions should be thrown", "Caught unexpected exception:  " + exp.ToString());
            }
        }
        #endregion


        #region Navigation-Journaling helpers
        private void RemoveBackEntry(String lastEntry)
        {
            // Remove last entry from the NavigationWindow's journal
            _navWindow.RemoveBackEntry();

            // Reflect this by removing the last entry in the backStack
            _backStack.Remove(lastEntry);
        }

        private void JournalAndChangeItem(String newLbxItem)
        {
            // Add current selection to backStack and update currLbxItem
            _backStack.Add(_currLbxItem);
            _currLbxItem = newLbxItem;

            // Change the current ListBoxItem selected
            Page1 p1 = _navWindow.Content as Page1;
            p1.SelectUser(newLbxItem);
        }


        private void GoBackAndJournal()
        {
            if (_navWindow.CurrentSource.ToString().Contains(CUSTOMJOURNALINGPAGE1))
                _fwdStack.Add(_currLbxItem);
            else
                _fwdStack.Add(_navWindow.CurrentSource.ToString());

            _backStack.RemoveAt(_backStack.Count - 1);

            _navWindow.GoBack();
            _whichSubTest++;
        }


        private void GoForwardAndJournal()
        {
            if (_navWindow.CurrentSource.ToString().Contains(CUSTOMJOURNALINGPAGE1))
                _backStack.Add(_currLbxItem);
            else
                _backStack.Add(_navWindow.CurrentSource.ToString());

            _fwdStack.RemoveAt(_fwdStack.Count - 1);

            _navWindow.GoForward();
            _whichSubTest++;
        }


        private void NavigateAndJournal(string destination)
        {
            if (_navWindow.CurrentSource.ToString().Contains(CUSTOMJOURNALINGPAGE1))
                _backStack.Add(_currLbxItem);
            else
                _backStack.Add(_navWindow.CurrentSource.ToString());

            _whichSubTest++;
            _navWindow.Navigate(new Uri(destination, UriKind.RelativeOrAbsolute));
        }


        private void VerifyJournalState(String lbxSelectedItem)
        {
            String back = UnpackJournalStack(_backStack);
            String fwd = UnpackJournalStack(_fwdStack);

            // We can only verify journal state with this method if we are currently on Page1.xaml
            if (!(_navWindow.Content is Page) ||
                !(_navWindow.CurrentSource.ToString().Contains(CUSTOMJOURNALINGPAGE1)))
            {
                NavigationHelper.Output("Cannot verify journal state");
                NavigationHelper.Fail("navWindow.Content is Page, navWindow.CurrentSource is Page1.xaml",
                    "navWindow.CurrentSource is " + _navWindow.CurrentSource.ToString());
            }

            // We must be on Page1.xaml, so check that current ListBoxItem equals the ListBoxItem we expect
            if (!(_currLbxItem.Equals(lbxSelectedItem)))
                NavigationHelper.Fail("Selected ListBoxItem should be " + lbxSelectedItem, "Current ListBoxItem is " + _currLbxItem);
            else
                NavigationHelper.Output("YES! Selected ListBoxItem is: " + _currLbxItem);

            // Check that backStack contains the expected entries
            if (!back.Equals(GetNavWinBackStack()))
                NavigationHelper.Fail("Back stack should contain " + back, "Back stack contains " + GetNavWinBackStack());
            else
                NavigationHelper.Output("YES! Back stack correctly contains: " + GetNavWinBackStack());

            // Check that fwdStack contains the expected entries
            if (!fwd.Equals(GetNavWinFwdStack()))
                NavigationHelper.Fail("Forward stack should contain " + fwd, "Forward stack contains " + GetNavWinFwdStack());
            else
                NavigationHelper.Output("YES! Forward stack correctly contains: " + GetNavWinFwdStack());
        }
        #endregion

        #region stack helpers
        /// <summary>
        /// Prints out all the contents of the back or forward stack, with the most recently
        /// visited entries listed as the first items in the string and the stalest entries
        /// listed as the last items in the string.
        /// </summary>
        /// <param name="stack">Either the fwdStack or backStack private member variable</param>
        /// <returns>String with the back/forward stack entries printed out, 
        /// from most to least recent</returns>
        private string UnpackJournalStack(ArrayList stack)
        {
            string poppedStack = String.Empty;
            foreach (String journalItem in stack)
            {
                // Prepend journalItem to the front of poppedStack
                poppedStack = journalItem + " " + poppedStack;
            }

            return poppedStack.Trim();
        }


        /// <summary>
        /// Prints out the ForwardStackProperty DP contents
        /// </summary>
        /// <returns>Forward stack contents, from most to least recently visited</returns>
        private string GetNavWinFwdStack()
        {
            return GetStack((IEnumerable)_navWindow.GetValue(NavigationWindow.ForwardStackProperty));
        }


        /// <summary>
        /// Prints out the BackStackProperty DP contents
        /// </summary>
        /// <returns>Back stack contents, from most to least recently visited</returns>
        private string GetNavWinBackStack()
        {
            return GetStack((IEnumerable)_navWindow.GetValue(NavigationWindow.BackStackProperty));
        }


        /// <summary>
        /// From Chango's DRT.
        /// This unpacks the Forward/BackStackProperty into a string, similar to 
        /// what UnpackJournalStack does for our private "accounting" copies of
        /// these stacks.
        /// </summary>
        /// <param name="entries"></param>
        /// <returns>String containing journal stack contents</returns>
        private string GetStack(IEnumerable entries)
        {
            StringBuilder stack = new StringBuilder();
            IEnumerator entryEnumerator = entries.GetEnumerator();
            bool first = true;
            while (entryEnumerator.MoveNext())
            {
                if (!first)
                {
                    stack.Append(' ');
                }
                JournalEntry entry = (JournalEntry)entryEnumerator.Current;
                stack.Append(entry.Name);
                first = false;
            }

            return stack.ToString();
        }
        #endregion
    }
}
