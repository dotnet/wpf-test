// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test;
using Microsoft.Test.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// The NavigationBaseClass is subclassed by each individual Navigation test case
    /// and is responsible for providing the following common functionality:
    /// * logging output, test results
    /// * verifying Source/CurrentSource for a specific NavigationWindow/Frame
    /// * invoking a Button/Hyperlink
    /// * clicking on a Hyperlink via raising RequestNavigateEventArgs
    /// * keeping track of number of events fired and event order
    /// * monitor journal button (Back/Fwd) state and journal drop-down menu contents
    /// * verify the exceptions caught to see if these match expected exceptions
    /// * simple journal navigations, such as going back/fwd N times (N >= 1)
    /// </summary>
    public class NavigationBaseClass
    {
        #region globals : used by all Navigation test cases
        
        protected String                    testName_local     = String.Empty;
        protected bool                      isFirstRun_local   = true;
        protected List<String>              eventsOrder  = null;

        protected DispatcherSignalHelper    signalHelper   = new DispatcherSignalHelper();
        protected JournalHelper             journalHelper  = new JournalHelper();
        #endregion


        /// <summary>
        /// This starts off all of the navigation tests.  Each subclass derived from 
        /// NavigationBaseClass will call this method first in their own OnStartup override
        /// and then register their own application-level eventhandlers and start running
        /// the actual test.
        /// </summary>
        /// <param name="e">Arguments for the StartupUri event</param>
        public NavigationBaseClass(String userGivenTestName)
        {
            if (String.IsNullOrEmpty(userGivenTestName))
                NavigationHelper.Fail("TestName has not been set.");
            else
            {
                testName_local = userGivenTestName;
                NavigationHelper.CreateLog(testName_local);
                NavigationHelper.SetStage(TestStage.Initialize);
            }
        }

        public NavigationBaseClass() : this("DefaultNavigationTestName")
        {
            Output("No test name supplied - using DefaultNavigationTestName as name of test");
        }


        #region helpers : logging, output

        /// <summary>
        /// Writes out the specified status message to the console/testlog
        /// </summary>
        /// <param name="statusMsg">String to output to console/testlog</param>
        public void Output(String statusMsg)
        {
            NavigationHelper.Output(statusMsg);
        }


        /// <summary>
        /// Logs a fail for the test into the logging mechanism, and 
        /// writes out the specified failure message to the console/testlog
        /// </summary>
        /// <param name="failMsg">Failure message to output to console/testlog</param>
        public void Fail(String failMsg)
        {
            NavigationHelper.Fail(failMsg);
        }


        /// <summary>
        /// Logs a fail for the test into the logging mechanism, and 
        /// writes out the expected and actual results for the test to 
        /// the console/testlog
        /// </summary>
        /// <param name="expected">Expected result for the test/subtest</param>
        /// <param name="actual">Actual result for the test/subtest</param>
        public void Fail(String expected, String actual)
        {
            NavigationHelper.Fail(expected, actual);
        }

        /// <summary>
        /// Logs a fail for the test into the logging mechanism, and 
        /// writes out the expected and actual results for the test to 
        /// the console/testlog
        /// </summary>
        /// <param name="expected">Expected result for the test/subtest</param>
        /// <param name="actual">Actual result for the test/subtest</param>
        public void CacheFail(String expected, String actual)
        {
            NavigationHelper.CacheTestResult(Result.Fail, "Expected " +  expected + " saw " + actual);
        }

        /// <summary>
        /// Logs a pass for the test into the logging mechanism, and
        /// writes out the specified pass message to the console/testlog
        /// </summary>
        /// <param name="passMsg">Success message to output to console/testlog</param>
        public void Pass(String passMsg)
        {
            NavigationHelper.Pass(passMsg);
        }

        /// <summary>
        /// Caches a pass for the test into the logging mechanism, and
        /// writes out the specified pass message to the console/testlog
        /// </summary>
        /// <param name="passMsg">Success message to output to console/testlog</param>
        public void CachePass(String passMsg)
        {
            NavigationHelper.CacheTestResult(Result.Pass, passMsg);
        }
       
        #endregion

        #region helpers : journal navigation/management

        /// <summary>
        /// Calls the GoBack method on the Frame/NavigationWindow.
        /// </summary>
        /// <param name="c">Navigation container to invoke GoBack() on</param>
        public void GoBack(ContentControl c)
        {
            if (c is Frame)
            {
                Frame f = c as Frame;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate(object ob)
                    {
                        f.GoBack();
                        return null;
                    }, null);
            }
            else if (c is NavigationWindow)
            {
                NavigationWindow nw = c as NavigationWindow;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate(object ob)
                    {
                        nw.GoBack();
                        return null;
                    }, null);
            }
            else
                Fail("Cannot call GoBack on a non-Frame, non-NavigationWindow control");

        }


        /// <summary>
        /// Calls the GoBack method on the Frame/NavigationWindow a given number of times
        /// </summary>
        /// <param name="c">Navigation container to invoke GoBack() on</param>
        /// <param name="numBackNavigations">Number of back navigations to perform</param>
        public void GoBack(ContentControl c, int numBackNavigations)
        {
            for (int i = 0; i < numBackNavigations; i++)
                GoBack(c);
        }


        /// <summary>
        /// Calls the GoForward method on the Frame/NavigationWindow
        /// </summary>
        /// <param name="c">Navigation container to invoke GoForward() on</param>
        public void GoForward(ContentControl c)
        {
            if (c is Frame)
            {
                Frame f = c as Frame;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate(object ob)
                    {
                        f.GoForward();
                        return null;
                    }, null);
            }
            else if (c is NavigationWindow)
            {
                NavigationWindow nw = c as NavigationWindow;
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate(object ob)
                    {
                        nw.GoForward();
                        return null;
                    }, null);
            }
            else
                Fail("Cannot call GoForward on a non-Frame, non-NavigationWindow control");
        }


        /// <summary>
        /// Calls the GoForward method on the Frame/NavigationWindow a given number of times
        /// </summary>
        /// <param name="c">NavigationContainer to invoke GoForward() on</param>
        /// <param name="numFwdNavigations">Number of forward navigations to perform</param>
        public void GoForward(ContentControl c, int numFwdNavigations)
        {
            for (int i = 0; i < numFwdNavigations; i++)
                GoForward(c);
        }

        #endregion

        #region helpers : verify expected results

        /// <summary>
        /// Checks the Source property of the given navigation container against the 
        /// expected Source string
        /// </summary>
        /// <param name="c">Navigation container we're checking Source for</param>
        /// <param name="expectedSource">Expected value of the Source property</param>
        public bool VerifySource(ContentControl c, String expectedSource)
        {
            if (c is Frame)
            {
                Frame f = c as Frame;
                if (expectedSource.Equals(f.Source.ToString()))
                {
                    Output("Expected and actual values for Frame's Source property match.  Source = " + expectedSource);
                    return true;
                }
                else
                {
                    Output("Expected and actual values for Frame's Source property do not match.");
                    Fail("Source = " + expectedSource, "Source = " + f.Source.ToString());
                    return false;
                }
            }
            else if (c is NavigationWindow)
            {
                NavigationWindow nw = c as NavigationWindow;
                if (expectedSource.Equals(nw.Source.ToString()))
                {
                    Output("Expected and actual values for NavigationWindow's Source property match.  Source = " + expectedSource);
                    return true;
                }
                else
                {
                    Output("Expected and actual values for NavigationWindow's Source property do not match.");
                    Fail("Source = " + expectedSource, "Source = " + nw.Source.ToString());
                    return false;
                }
            }
            else
            {
                Fail("Cannot verify the Source property of a non-Frame, non-NavigationWindow control");
                return false;
            }
        }


        /// <summary>
        /// Checks the CurrentSource property of the given navigation container against the 
        /// expected CurrentSource string
        /// </summary>
        /// <param name="c">Navigation container we're checking CurrentSource for</param>
        /// <param name="expectedCurrentSource">Expected value of the CurrentSource property</param>
        public bool VerifyCurrentSource(ContentControl c, String expectedCurrentSource)
        {
            if (c is Frame)
            {
                Frame f = c as Frame;
                if (expectedCurrentSource.Equals(f.CurrentSource.ToString()))
                {
                    Output("Expected and actual values for Frame's CurrentSource property match.  CurrentSource = " + expectedCurrentSource);
                    return true;
                }
                else
                {
                    Output("Expected and actual values for Frame's CurrentSource property do not match.");
                    Fail("CurrentSource = " + expectedCurrentSource, "CurrentSource = " + f.CurrentSource.ToString());
                    return false;
                }
            }
            else if (c is NavigationWindow)
            {
                NavigationWindow nw = c as NavigationWindow;
                if (expectedCurrentSource.Equals(nw.CurrentSource.ToString()))
                {
                    Output("Expected and actual values for NavigationWindow's CurrentSource property match.  CurrentSource = " + expectedCurrentSource);
                    return true;
                }
                else
                {
                    Output("Expected and actual values for NavigationWindow's CurrentSource property do not match.");
                    Fail("CurrentSource = " + expectedCurrentSource, "CurrentSource = " + nw.CurrentSource.ToString());
                    return false;
                }
            }
            else
            {
                Fail("Cannot verify the CurrentSource property of a non-Frame, non-NavigationWindow control");
                return false;
            }
        }


        /// <summary>
        /// Compares the expected exception against the actual exception thrown by the framework
        /// </summary>
        /// <param name="expectedException">Exception we were expecting to get</param>
        /// <param name="actualException">Exception that was thrown</param>
        public void VerifyExpectedException(Exception expectedException, Exception actualException)
        {
            if (expectedException.Equals(actualException))
                Output("Expected and actual exceptions match:  " + expectedException.ToString());
            else
            {
                Output("Expected and actual exceptions do not match.");
                Fail(expectedException.ToString(), actualException.ToString());
            }
        }

        #endregion

        #region helpers : journal button, drop-down menu state

        /// <summary>
        /// Returns the current state of the NavigationWindow/Frame's back button.
        /// </summary>
        /// <param name="c">NavigationWindow or Frame whose journal is being checked</param>
        /// <returns>true if back button is enabled, false if it is disabled</returns>
        public bool IsBackButtonEnabled(ContentControl c)
        {
            return journalHelper.IsBackEnabled(c);
        }

        /// <summary>
        /// Returns the current state of the NavigationWindow/Frame's fwd button
        /// </summary>
        /// <param name="c">NavigationWindow or Frame whose journal is being checked</param>
        /// <returns>true if back button is enabled, false if it is disabled</returns>
        public bool IsFwdButtonEnabled(ContentControl c)
        {
            return journalHelper.IsForwardEnabled(c);
        }

        // for IE7, standalone apps
        public bool VerifyDropDownContents(String[] journalContents, ContentControl c)
        {
            // Open up menu and grab the journal entries
            if (c is NavigationWindow)
            {
                NavigationWindow nw = c as NavigationWindow;
                Menu backMenu = NavigationUtilities.GetBackMenu(nw);
                Menu fwdMenu = NavigationUtilities.GetForwardMenu(nw);

                // Standalone: Both menus are the same, so arbitrarily pick 
                // the back menu as the application's journal
                if (!BrowserInteropHelper.IsBrowserHosted)
                {
                    MenuItem journal = backMenu.Items[0] as MenuItem;
                    return CompareJournals(journalContents, journal);
                }
                else // browser-hosted test case
                {
                    Output("VerifyDropDownContents is not used for non-browser-hosted test cases");
                    return false;
                }
            }
            else // not a NavigationWindow
            {
                Output("VerifyDropDownContents is not used for non-NavigationWindow test cases.");
                return false;
            }
        }


        private bool CompareJournals(String[] expectedJournalContents, MenuItem actualJournal)
        {
            if (actualJournal == null)
            {
                Output("Could not grab a reference to the journal drop-down menu");
                return false;
            }

            // Open the submenu
            actualJournal.IsSubmenuOpen = true;
            ItemContainerGenerator journalICG = actualJournal.ItemContainerGenerator;
            int journalEntryCount = actualJournal.Items.Count;

            // Compare the actual and expected journal entry values
            for (int i = 0; i < journalEntryCount; i++)
            {
                MenuItem currentEntry = (MenuItem)journalICG.ContainerFromIndex(i);
                // Check that current (actual) entry is non-null
                if (currentEntry == null)
                {
                    Output("Current journal entry is null.  Cannot compare to expected");
                    return false;               // kill loop
                }

                // Check that actual and expected counts match
                if (journalEntryCount != expectedJournalContents.Length)
                {
                    Output("Expected and actual journal counts do not match.");
                    return false;
                }

                // Check that expected entry is non-null and non-empty, then compare the 
                // actual and expected entries
                if (String.IsNullOrEmpty(expectedJournalContents[i]) ||
                    !(expectedJournalContents[i].Equals((String)currentEntry.Header)))
                {
                    Output("Expected and actual journal entries don't match");
                    Output("EXPECTED: " + expectedJournalContents[i] + "; ACTUAL: " + currentEntry.Header);
                    return false;
                }
            }

            // if we get to this point without any failures, it means everything else matched
            Output("Expected and actual journal entries match for all contents in the journal");
            return true;
        }


        // IE6 only
        public bool VerifyBackStackContents(String[] expectedBackStackContents, ContentControl c)
        {
            String[] actualBackStackContents = journalHelper.GetBackMenuItems(c);

            if (actualBackStackContents.Length != expectedBackStackContents.Length)
            {
                Output("Actual and expected back stacks do not have matching counts.");
                return false;
            }
            else
            {
                // Compare each entry in the arrays and see if they match
                for (int i = 0; i < expectedBackStackContents.Length; i++)
                {
                    if (!(expectedBackStackContents[i].Equals(actualBackStackContents[i])))
                    {
                        Output("Item [" + i + "] in the back stack doesn't match expected");
                        Output("EXPECTED: " + expectedBackStackContents[i] + "; ACTUAL: " + actualBackStackContents[i]);
                        return false;
                    }
                }

                // if we get to this point without returning a false, it means everything matched
                Output("All back stack entries matched the expected back stack contents");
                return true;
            }
        }

        public bool VerifyFwdStackContents(String[] expectedFwdStackContents, ContentControl c)
        {
            String[] actualFwdStackContents = journalHelper.GetForwardMenuItems(c);

            if (actualFwdStackContents.Length != expectedFwdStackContents.Length)
            {
                Output("Actual and expected forward stacks do not have matching counts.");
                return false;
            }
            else
            {
                // Compare each entry in the arrays and see if they match
                for (int i = 0; i < expectedFwdStackContents.Length; i++)
                {
                    if (!(expectedFwdStackContents[i].Equals(actualFwdStackContents[i])))
                    {
                        Output("Item [" + i + "] in the forward stack doesn't match expected");
                        Output("EXPECTED: " + expectedFwdStackContents[i] + "; ACTUAL: " + actualFwdStackContents[i]);
                        return false;
                    }
                }

                // if we get to this point without returning a false, it means everything matched
                Output("All forward stack entries matched the expected forward stack contents");
                return true;
            }
        }

        #endregion

        #region helpers : asynchronous navigation

        public void NavigateToUri(ContentControl c, Uri targetUri)
        {
            if (targetUri == null)
                Output("targetUri is null.");

            if (c is NavigationWindow || c is Frame)
            {
                Output("Navigating to " + targetUri.ToString());
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate(object obj)
                    {
                        if (c is NavigationWindow)
                            ((NavigationWindow)c).Navigate(targetUri);
                        else if (c is Frame)
                            ((Frame)c).Navigate(targetUri);

                        return null;
                    }, null);
            }
            else
                Output("Cannot navigate a non-Frame, non-NavigationWindow object");
        }

        public void NavigateToObject(ContentControl c, object target)
        {
            if (target == null)
                Output("target object is null");

            if (c is NavigationWindow || c is Frame)
            {
                Output("Navigating to " + target.ToString());
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                    (DispatcherOperationCallback)delegate(object ob)
                    {
                        if (c is NavigationWindow)
                            ((NavigationWindow)c).Navigate(target);
                        else if (c is Frame)
                            ((Frame)c).Navigate(target);

                        return null;
                    }, null);
            }
            else
                Output("Cannot navigate a non-Frame, non-NavigationWindow object");
        }

        #endregion

        #region helpers : event order

        public void AddActualEvent(String eventName)
        {
            if (eventsOrder == null)
            {
                eventsOrder = new List<String>();
                if (eventsOrder == null)
                    NavigationHelper.Fail("Cannot initialize the expected events list.");
            }

            eventsOrder.Add(eventName);
        }

        public bool VerifyEventOrder(List<String> expectedEvents)
        {
            if (expectedEvents == null)
            {
                Output("List of expected events is null.  Cannot compare actual vs expected event order.");
                return false;
            }

            if (expectedEvents.Count != eventsOrder.Count)
            {
                Output("Number of expected and actual events do not match.  Cannot compare actual vs expected event order.");
                return false;
            }

            for (int i = 0; i < expectedEvents.Count; i++)
            {
                if (!(expectedEvents[i].Equals(eventsOrder[i])))
                {
                    Output("Expected and actual event do not match at slot " + i);
                    Output("EXPECTED: " + expectedEvents[i] + "; ACTUAL: " + eventsOrder[i]);
                    return false;
                }
            }

            // if we've looped through everything and all slots matched, 
            // then the actual event order is correct so return true
            Output("Actual event order matches the expected event order.");
            return true;
        }

        public bool VerifyEventOrder(ArrayList expectedEvents)
        {
            if (expectedEvents == null)
            {
                Output("List of expected events is null.  Cannot compare actual vs expected event order.");
                return false;
            }

            if (expectedEvents.Count != eventsOrder.Count)
            {
                Output("Number of expected and actual events do not match.  Cannot compare actual vs expected event order.");
                return false;
            }

            for (int i = 0; i < expectedEvents.Count; i++)
            {
                if (!(expectedEvents[i].Equals(eventsOrder[i])))
                {
                    Output("Expected and actual event do not match at slot " + i);
                    Output("EXPECTED: " + expectedEvents[i] + "; ACTUAL: " + eventsOrder[i]);
                    return false;
                }
            }

            // if we've looped through everything and all slots matched, 
            // then the actual event order is correct so return true
            Output("Actual event order matches the expected event order.");
            return true;
        }

        public bool VerifyEventCount()
        {
            return NavigationHelper.CompareEventCounts();
        }

        #endregion

        #region properties

        public String TestName
        {
            get
            {
                return testName_local;
            }

            set
            {
                testName_local = value;
            }
        }


        public bool IsFirstRun
        {
            get
            {
                return isFirstRun_local;
            }

            set
            {
                isFirstRun_local = value;
            }
        }

        #endregion
    }
}
