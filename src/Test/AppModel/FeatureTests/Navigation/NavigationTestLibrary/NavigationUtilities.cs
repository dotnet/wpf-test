// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define TRACE

#region Using directives
using System;
using System.Collections;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
//using System.Diagnostics;
using Microsoft.Test.Logging;
#endregion

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Contains common utility functions for Navigation / Journaling
    /// Will add more functionality to this class as reqd
    /// </summary>
    public class NavigationUtilities
    {
        static TreeUtilities s_treeUtils = new TreeUtilities();

        public NavigationUtilities()
        {   
        }

        /// <summary>
        /// Returns the Enumerator for the Forward Stack of the Navigation Window
        /// </summary>
        /// <param name="c">NavigationWindow/Frame we're inspecting journal of</param>
        /// <returns>IEnumerator containing the contents of the NavigationWindow/Frame's fwd stack</returns>
        public static IEnumerator GetForwardStack(ContentControl c)
        {
            if (c is NavigationWindow)
            {
                NavigationWindow nw = c as NavigationWindow;
                return (nw == null) ?
                    null :
                    ((IEnumerable)nw.GetValue(NavigationWindow.ForwardStackProperty)).GetEnumerator();
            }
            else if (c is Frame)
            {
                Frame f = c as Frame;
                return (f == null) ?
                    null :
                    ((IEnumerable)f.GetValue(Frame.ForwardStackProperty)).GetEnumerator();
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Cannot inspect fwd stack of non-NavigationWindow, non-Frame object");
                return null;
            }
        }

        /// <summary>
        /// Returns the Enumerator for the Back Stack of the Navigation Window
        /// </summary>
        /// <param name="c">NavigationWindow/Frame we're inspecting journal of</param>
        /// <returns>IEnumerator containing the contents of the the NavigationWindow/Frame's fwd stack</returns>
        public static IEnumerator GetBackStack(ContentControl c)
        {
            if (c is NavigationWindow)
            {
                NavigationWindow nw = c as NavigationWindow;
                return (nw == null) ?
                    null :
                    ((IEnumerable)nw.GetValue(NavigationWindow.BackStackProperty)).GetEnumerator();
            }
            else if (c is Frame)
            {
                Frame f = c as Frame;
                return (f == null) ?
                    null :
                    ((IEnumerable)f.GetValue(Frame.BackStackProperty)).GetEnumerator();
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Cannot inspect back stack of non-NavigationWindow, non-Frame object");
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nw"></param>
        /// <returns></returns>
        public static Menu GetBackMenu(NavigationWindow nw)
        {            
            Menu menu = (Menu)s_treeUtils.FindVisualByType(typeof(Menu), GetBackButton(nw));

            // if we can't find it, try to find the IE7 style menu drop-down
            if (menu == null)
            {
                menu = (Menu)s_treeUtils.FindVisualByType(typeof(Menu), nw);
            }

            return menu;
        }

        public static Button GetBackButton(NavigationWindow nw)
        {
            Button backButton = (Button)s_treeUtils.FindVisualByPropertyValue(Button.CommandProperty, NavigationCommands.BrowseBack, nw);
            return backButton;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nw"></param>
        /// <returns></returns>
        public static Menu GetForwardMenu(NavigationWindow nw)
        {
            Menu menu = (Menu)s_treeUtils.FindVisualByType(typeof(Menu), GetForwardButton(nw));

            // if we can't find it, try to find the IE7 style menu drop-down
            if (menu == null)
            {
                menu = (Menu)s_treeUtils.FindVisualByType(typeof(Menu), nw);
            }
 
            return menu;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nw"></param>
        /// <returns></returns>
        public static Button GetForwardButton(NavigationWindow nw)
        {
            Button forwardButton = (Button)s_treeUtils.FindVisualByPropertyValue(Button.CommandProperty, NavigationCommands.BrowseForward, nw);
            return forwardButton;
        }

        public static void InvokeBackMenuEntry(NavigationWindow navWin, int dropDownIndex)
        {
            Menu backMenu = GetBackMenu(navWin);
            InvokeMenuItem(backMenu, dropDownIndex, navWin.GetValue(NavigationWindow.BackStackProperty) as IEnumerable);    
        }

        public static void InvokeForwardMenuEntry(NavigationWindow navWin, int dropDownIndex)
        {
            Menu forwardMenu = GetForwardMenu(navWin);
            InvokeMenuItem(forwardMenu, dropDownIndex, navWin.GetValue(NavigationWindow.ForwardStackProperty) as IEnumerable);
        }

        public static void InvokeMenuItem(Menu menu, int menuItemIndex, IEnumerable stack)
        {
            MenuItem menuItem = (MenuItem)menu.Items[0];
            if (menuItem == null ||  (menuItem.Items.Count == 0))
                return;
            
            menuItem.IsSubmenuOpen = true;            
            Thread.Sleep(250);
            JournalEntry entry = GetJournalEntry(stack, menuItemIndex);
            if (entry != null)
            {
                MenuItem navItem = (MenuItem)menuItem.ItemContainerGenerator.ContainerFromItem(entry);
                if (navItem != null)
                {
                    AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(navItem);
                    IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                    iip.Invoke();
                }
            }
        }
        
        private static JournalEntry GetJournalEntry(IEnumerable stack, int index)
        {
            IEnumerator enumerator = stack.GetEnumerator();
            enumerator.Reset();
            int i = 0;
            while (enumerator.MoveNext())
            {
                JournalEntry je = enumerator.Current as JournalEntry;
                if (i == index)
                    return je;
                i++;
            }
            return null;
        }
        /// <summary>
        /// Compares the Journal Name entries of the actual and expected journal stacks
        /// Returns true if all match, false otherwise
        /// </summary>
        /// <param name="actual"></param>
        /// <param name="expected"></param>
        public static bool CompareJournalStacks(IEnumerator actual, IEnumerator expected)
        {
            lock (actual)
            {
                lock (expected)
                {
                    int actualSize = 0;
                    int expectedSize = 0;
                    JournalEntry actualEntry = null;
                    VerificationJournalEntry expectedEntry = null;
                    bool actualEntriesPresent = false;
                    bool expectedEntriesPresent = false;

                    while (true)
                    {
                        actualEntriesPresent = actual.MoveNext();
                    
                        if (actualEntriesPresent)
                        {
                            actualSize++;
                            actualEntry = (JournalEntry)actual.Current;
                            if (actualEntry == null)
                            {
                                Log.Current.CurrentVariation.LogMessage("Actual Entry is null");
                                actualEntriesPresent = true;
                            }
                            else
                            {
                                Log.Current.CurrentVariation.LogMessage("Actual Journal Entry: Name = "
                                + actualEntry.Name);
                            }
                        }
                        
                        expectedEntriesPresent = expected.MoveNext();
                        
                        if (expectedEntriesPresent)
                        {
                            expectedSize++;
                            expectedEntry = (VerificationJournalEntry)expected.Current;
                            if (expectedEntry == null)
                            {
                                Log.Current.CurrentVariation.LogMessage("Expected Entry is null");
                            }
                            else
                            {
                                Log.Current.CurrentVariation.LogMessage("Expected Journal Entry: Name = "
                                                         + expectedEntry.Name);
                            }
                        }

                        if (actualEntriesPresent && !expectedEntriesPresent)
                        {
                            Log.Current.CurrentVariation.LogMessage("Expected Stack Size < Actual Stack Size\n");
                            return false;
                        }
                        else if (!actualEntriesPresent && !expectedEntriesPresent)
                        {
                            Log.Current.CurrentVariation.LogMessage("End of both Stacks. Equal Size");
                            return true;
                        }
                        else if (!actualEntriesPresent && expectedEntriesPresent)
                        {
                            Log.Current.CurrentVariation.LogMessage("Expected Stack Size > Actual Stack Size\n");
                            return false;
                        }
            
                        if (!actualEntry.Name.Equals(expectedEntry.Name))
                        {
                            Log.Current.CurrentVariation.LogMessage("Actual and Expected Journal Entries are different");
                            Log.Current.CurrentVariation.LogMessage("EXPECTED: " + expectedEntry.Name + "; ACTUAL: " + actualEntry.Name);
                            return false;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Prints Journal Name, Source and ToString() for all the journal
        /// entries in the IEnumerator passed
        /// </summary>
        /// <param name="stack"></param>
        public static void PrintJournalEntries(IEnumerator stack)
        {
            if (stack == null)
                return;

            while (stack.MoveNext())
            {
                JournalEntry entry = stack.Current as JournalEntry;
                if (entry == null)
                {
                    Log.Current.CurrentVariation.LogMessage("JournalEntry is null");
                    return;
                }
                Log.Current.CurrentVariation.LogMessage("JournalEntry Name = " + entry.Name
                                + " Source = " + entry.Source
                                + " ToString = " + entry);
            }
        }

        /// <summary>
        /// Prints the ForwardStack journal entries for the Navigation Window
        /// </summary>
        /// <param name="navWin"></param>
        public static void PrintForwardStack(NavigationWindow navWin)
        {
            Log.Current.CurrentVariation.LogMessage("Forward Stack");
            IEnumerator forwardStack = GetForwardStack(navWin);
            PrintJournalEntries(forwardStack);
        }

        /// <summary>
        /// Prints the BackStack journal entries for the Navigation Window
        /// </summary>
        /// <param name="navWin">NavigationWindow whose journal we're inspecting</param>
        public static void PrintBackStack(NavigationWindow navWin)
        {
            Log.Current.CurrentVariation.LogMessage("Back Stack");
            IEnumerator backStack = GetBackStack(navWin);
            PrintJournalEntries(backStack);
        }

        /// <summary>
        /// Prints all the journal entries for the NavigationWindow
        /// </summary>
        /// <param name="navWin">NavigationWindow whose journal we're inspecting</param>
        public static void PrintJournalEntries(NavigationWindow navWin)
        {
            PrintBackStack(navWin);
            PrintForwardStack(navWin);
        }

        /// <summary>
        /// Compares actual and expected back, forward stacks for the NavigationWindow
        /// and returns true on a complete match, otherwise false
        /// </summary>
        /// <param name="expectedBackStack">Enumerated back stack entries</param>
        /// <param name="expectedForwardStack">Enumerated fwd stack entries</param>
        /// <param name="navWin">NavigationWindow whose journal we're inspecting</param>
        public static bool VerifyJournalEntries(IEnumerator expectedBackStack, IEnumerator expectedForwardStack,
                                    NavigationWindow navWin)
        {
            Log.Current.CurrentVariation.LogMessage("Verifying Journal Entries");
            IEnumerator actualBackStack = GetBackStack(navWin);
            IEnumerator actualForwardStack = GetForwardStack(navWin);
            
            Log.Current.CurrentVariation.LogMessage("Comparing Back Stacks");
            bool backMatch = CompareJournalStacks(actualBackStack, expectedBackStack);
            Log.Current.CurrentVariation.LogMessage("Comparing Forward Stacks");
            bool forwardMatch = CompareJournalStacks(actualForwardStack, expectedForwardStack);
            Log.Current.CurrentVariation.LogMessage("backMatch = " + backMatch + " forwardMatch = " + forwardMatch);
            return (backMatch && forwardMatch);
        }

        /// <summary>
        /// verify the states of forward and back buttons
        /// verify the window title
        /// verify the chevron items (recent pages)
        /// </summary>
        /// <param name="isBackEnabled">Is the back button enabled</param>
        /// <param name="isFwdEnabled">Is the forward button enabled</param>
        /// <param name="expectedJournal">Expected journal entries</param>
        /// <param name="windowTitle">Window title</param>
        /// <returns></returns>
        public static bool VerifyIE7BrowserState(bool isBackEnabled, bool isFwdEnabled,
            String[] expectedJournal, String windowTitle)
        {

            AutomationElement automationElementBack = BrowserHelper.GetChromeBackButton();

            if (!(BrowserHelper.IsEnabledPropertySet(automationElementBack).Equals(isBackEnabled)))
            {
                NavigationHelper.Output("Browser back button did not match expected value");
                return false;
            }

            AutomationElement automationElementForward = BrowserHelper.GetChromeForwardButton();

            if (!(BrowserHelper.IsEnabledPropertySet(automationElementForward).Equals(isFwdEnabled)))
            {
                NavigationHelper.Output("Browser forward button did not match expected value");
                return false;
            }

            String ieWindowTitle = BrowserHelper.GetMainBrowserWindow(Application.Current).WindowTitle;
            if (!ieWindowTitle.Equals(windowTitle))
            {
                StringBuilder output = new StringBuilder();
                output.Append("Browser window title did not match expected value. Actual = ");
                output.Append(ieWindowTitle);
                output.Append(", Expected = ");
                output.Append(windowTitle);
                NavigationHelper.Output(output.ToString());
                return false;
            }

            // verify the journal items if the expectedJournal is not null 
            if (expectedJournal != null)
            {
                String[] backItems = BrowserHelper.GetChevronItems(BrowserHelper.GetJournalDropdownButton());
                if (backItems == null)
                {
                    NavigationHelper.Output("Couldn't retrieve chevron items");
                    return false;
                }

                // match the stacks for the given length because we cannot always predict exact expected length
                if (BrowserHelper.Match(backItems, expectedJournal, 25))
                {
                    NavigationHelper.Output("Expected and actual journals matched");
                    return true;
                }
                else
                {
                    NavigationHelper.Output("Expected and actual journals do not match");
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// verify the journal properties against the JournalHelper object
        /// </summary>
        /// <param name="isBackEnabled">Is back button enabled</param>
        /// <param name="isFwdEnabled">Is forward button enabled</param>
        /// <param name="expectedBackJournal">Expected back journal (back stack)</param>
        /// <param name="expectedForwardJournal">Expected forward journal (forward stack)</param>
        /// <param name="windowTitle">Window title</param>
        /// <param name="journalHelper">Journal Helper object that contains journal info</param>
        /// <returns></returns>
        public static bool VerifyJournalEntries(bool isBackEnabled, bool isFwdEnabled,
            String[] expectedBackJournal, String[] expectedForwardJournal, String windowTitle, JournalHelper journalHelper)
        {
            if (journalHelper == null)
            {
                NavigationHelper.Output("journalHelper is null in VerifyJournalEntries");
                return false;
            }

            // match for back button enabled property
            if (!(journalHelper.IsBackEnabled().Equals(isBackEnabled)))
            {
                NavigationHelper.Output("journalHelper back button did not match expected value");
                return false;
            }

            // match for forward button enabled property
            if (!(journalHelper.IsForwardEnabled().Equals(isFwdEnabled)))
            {
                NavigationHelper.Output("journalHelper forward button did not match expected value");
                return false;
            }

            // match the window title
            if (!journalHelper.WindowTitle.Equals(windowTitle))
            {
                StringBuilder output = new StringBuilder();
                output.Append("journalHelper window title did not match expected value. Actual = ");
                output.Append(journalHelper.WindowTitle);
                output.Append(", Expected = ");
                output.Append(windowTitle);
                NavigationHelper.Output(output.ToString());
                return false;
            }

            // match the back journal items
            if (expectedBackJournal != null)
            {
                // match the stacks for the given length because we cannot always predict exact expected length
                if (BrowserHelper.Match(journalHelper.GetBackMenuItems(), expectedBackJournal, 25))
                {
                    NavigationHelper.Output("Expected back stack properties and actual journalHelper properties matched");
                    return true;
                }
                else
                {
                    NavigationHelper.Output("Expected back stack properties and actual journalHelper properties do not match");
                    return false;
                }
            }

            // match the forward journal items
            if (expectedForwardJournal != null)
            {
                // match the stacks for the given length because we cannot always predict exact expected length
                if (BrowserHelper.Match(journalHelper.GetForwardMenuItems(), expectedForwardJournal, 25))
                {
                    NavigationHelper.Output("Expected forward stack properties and actual journalHelper properties matched");
                    return true;
                }
                else
                {
                    NavigationHelper.Output("Expected forward stack properties and actual journalHelper properties do not match");
                    return false;
                }
            }

            return true;
        }
    }
	
	    public class VerificationJournalEntry
        {
            String _name = String.Empty;

            public VerificationJournalEntry()
            {
            }

            public String Name
            {
                get { return _name; }
                set { _name = value; }
            }
        }

}
