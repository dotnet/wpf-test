// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Interop;

using DRT;

namespace DrtJournal
{
    //[Suite split off ..\DrtJournal.cs]
    /// <summary>
    /// This suite is designed to test basic journal navigation. It will navigate to a bunch of new
    /// pages, go back all the way by pressing the back button, go forward all the way by
    /// pressing the forward button, jump around using the menus on the back/forward buttons,
    /// and do new navigations that should wipe out the forward stack.
    ///
    /// See the Verify method for a complete description of the tests made after each navigation.
    /// </summary>
    public class BasicJournalNavigationSuite : JournalTestSuite
    {
        public BasicJournalNavigationSuite()
            : base("Basic Journal Navigation")
        {
            this.Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] {
                (DrtTest) delegate()
                {
                    (DRT as DrtJournalBase).UseNewNavigationWindow();
                    WireNavigationEvents();

                    // Perform the initial navigation to PageA.xaml
                    base.NavWin.Navigate(new Uri(@"Basic\PageA.xaml", UriKind.RelativeOrAbsolute));
                    DRT.Suspend(); // OnLoadCompleted() will resume. (Navigation is asynchronous.)
                },
                (DrtTest) delegate() { this.Verify('A', "", "", "After initial navigation to A"); },
                (DrtTest) delegate() { this.NavigateTo('B'); },
                (DrtTest) delegate() { this.Verify('B', "A", "", "After going to B"); },

                // Interrupting the linear navigation sequence for a test that requires a fresh
                // and really clean Journal in the current state (no Pruned entries, in particular)...
                // Test GoBack() GoForward() with no forward stack, a single entry in the back stack
                // (there were failing assertions in this case), and no "current" journal
                // entry to go forward to. (The same test but in the middle of the journal is later on.)
                (DrtTest) delegate() { NavWin.GoBack(); DRT.Assert(NavWin.CanGoForward); NavWin.GoForward(); },
                (DrtTest) delegate() { this.Verify('B', "A", "", "GoBack,GoForward should result in same location"); },

                (DrtTest) delegate() { this.NavigateTo('C'); },
                (DrtTest) delegate() { this.Verify('C', "BA", "", "After going to C"); },
                (DrtTest) delegate() { this.NavigateTo('D'); },
                (DrtTest) delegate() { this.Verify('D', "CBA", "", "After going to D"); },
                (DrtTest) delegate() { this.NavigateTo('E'); },
                (DrtTest) delegate() { this.Verify('E', "DCBA", "", "After going to E"); },
                (DrtTest) delegate() { this.NavigateTo('F'); },
                (DrtTest) delegate() { this.Verify('F', "EDCBA", "", "After going to F"); },
                (DrtTest) delegate() { this.NavigateTo('G'); },
                (DrtTest) delegate() { this.Verify('G', "FEDCBA", "", "After going to G"); },
                (DrtTest) delegate() { this.NavigateTo('H'); },
                (DrtTest) delegate() { this.Verify('H', "GFEDCBA", "", "After going to H"); },
                (DrtTest) delegate() { this.NavigateTo('I'); },
                (DrtTest) delegate() { this.Verify('I', "HGFEDCBA", "", "After going to I"); },
                (DrtTest) delegate() { this.NavigateTo('J'); },
                (DrtTest) delegate() { this.Verify('J', "IHGFEDCBA", "", "After going to J"); },
                (DrtTest) delegate() { this.NavigateTo('K'); },
                (DrtTest) delegate() { this.Verify('K', "JIHGFEDCBA", "JIHGFEDCB", "", "", "After going to K, should trim back stack display"); },

                // Try Refresh of the current page, for which there is no journal entry yet.
                (DrtTest) delegate() { NavWin.Refresh(); DRT.Suspend(); },
                (DrtTest) delegate() { this.Verify('K', "JIHGFEDCBA", "JIHGFEDCB", "", "", "After Refresh"); },

                // Now go back to the beginning
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('J', "IHGFEDCBA", "K", "Going back to J"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('I', "HGFEDCBA", "JK", "Going back to I"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('H', "GFEDCBA", "IJK", "Going back to H"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('G', "FEDCBA", "HIJK", "Going back to G"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('F', "EDCBA", "GHIJK", "Going back to F"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('E', "DCBA", "FGHIJK", "Going back to E"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('D', "CBA", "EFGHIJK", "Going back to D"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('C', "BA", "DEFGHIJK", "Going back to C"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('B', "A", "CDEFGHIJK", "Going back to B"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('A', "", "", "BCDEFGHIJK", "BCDEFGHIJ", "Going back to A, should trim forward stack"); },

                (DrtTest) delegate() { NavWin.Refresh(); DRT.Suspend(); },

                // Now go forward to the end
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('B', "A", "CDEFGHIJK", "Going forward to B"); },
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('C', "BA", "DEFGHIJK", "Going forward to C"); },
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('D', "CBA", "EFGHIJK", "Going forward to D"); },
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('E', "DCBA", "FGHIJK", "Going forward to E"); },
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('F', "EDCBA", "GHIJK", "Going forward to F"); },
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('G', "FEDCBA", "HIJK", "Going forward to G"); },
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('H', "GFEDCBA", "IJK", "Going forward to H"); },
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('I', "HGFEDCBA", "JK", "Going forward to I"); },
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('J', "IHGFEDCBA", "K", "Going forward to J"); },
                (DrtTest) delegate() { this.GoForward(); },
                (DrtTest) delegate() { this.Verify('K', "JIHGFEDCBA", "JIHGFEDCB", "", "", "Going forward to K, should trim back stack"); },

                // Test GoBack and GoForward multiple times, without waiting for intermediate 
                // navigations to complete. There was a problem with *more than one* page skipped 
                // that way. For variety, also mix different ways of initiating the navigations...
                (DrtTest) delegate() { for(int i=0; i < 3; i++) { this.GoBack(); } NavWin.GoBack(); },
                    //[No need to call DRT.Suspend() here since this.GoBack() calls it (and it's not
                    // reference-counted).]
                (DrtTest) delegate() { this.Verify('G', "FEDCBA", "HIJK", "Four pages back, w/o waiting for intermediate navigations"); },
                (DrtTest) delegate() { for(int i=0; i < 3; i++) { this.GoForward(); } NavWin.GoForward(); },
                (DrtTest) delegate() { this.Verify('K', "JIHGFEDCBA", "JIHGFEDCB", "", "", "Four pages forward, w/o waiting for intermediate navigations"); },

                 // Now jump back to B
                (DrtTest) delegate() { this.JumpBack('B'); },
                (DrtTest) delegate() { this.Verify('B', "A", "CDEFGHIJK", "Jumped back to B"); },
                // Now jump forward to I
                (DrtTest) delegate() { this.JumpForward('I'); },
                (DrtTest) delegate() { this.Verify('I', "HGFEDCBA", "JK", "Jumped forward to I"); },
                // GoForward should simply cancel the previous GoBack (Not waiting for either GoBack
                // to complete.)
                (DrtTest) delegate() { this.GoBack(); this.GoBack(); this.GoForward(); },
                (DrtTest) delegate() { this.Verify('H', "GFEDCBA", "IJK", "GoBack+GoBack+GoForward ?= GoBack (w/o waiting)"); },
                // Opposite test
                (DrtTest) delegate() { this.GoForward();  this.GoForward(); this.GoBack(); },
                (DrtTest) delegate() { this.Verify('I', "HGFEDCBA", "JK", "GoFwd+GoFwd+GoBack ?= GoFwd (w/o waiting)"); },
                // Now do a new navigation to Z
                (DrtTest) delegate() { this.NavigateTo('Z'); },
                (DrtTest) delegate() { this.Verify('Z', "IHGFEDCBA", "", "Navigated to Z, forward stack should be empty"); },
                // Jump to A
                (DrtTest) delegate() { this.JumpBack('A'); },
                (DrtTest) delegate() { this.Verify('A', "", "BCDEFGHIZ", "Jumped back to A"); },
                // Now do a new navigation to C
                (DrtTest) delegate() { this.NavigateTo('C'); },
                (DrtTest) delegate() { this.Verify('C', "A", "", "Navigated to C, forward stack should be empty"); },

                // Test for bugs 1187603 & 1187613: Going back/fwd to a different instance of the current page
                (DrtTest) delegate() { this.NavigateTo('A'); }, // Has to be a separate DrtTest to complete loading.
                (DrtTest) delegate() { this.JumpBack('A'); },
                (DrtTest) delegate() { this.Verify('A', "", "CA", "Back to previous instance of current page (A)"); },
                (DrtTest) delegate() { this.NavigateTo('C'); }, // This just restores the state for the following tests.

                // Jump back to A - this exercises the ItemContainerGenerator (
                (DrtTest) delegate() { this.JumpBack('A'); },
                // Test enumerators - exercises 
                (DrtTest) delegate() { CreateForwardStackEnumerator(); },
                (DrtTest) delegate() { GoForward(); },
                (DrtTest) delegate() { TestForwardStackEnumeratorInvalidated(); },
                (DrtTest) delegate() { CreateBackStackEnumerator(); },
                (DrtTest) delegate() { GoBack(); },
                (DrtTest) delegate() { TestBackStackEnumeratorInvalidated(); },
                // Cancel navigation in OnNavigating event using NavigationCancelEventArgs
                (DrtTest) delegate() { this.NavigateTo('B'); },
                (DrtTest) delegate() { _cancelNextNavigation = true; GoBack(); DRT.Resume();},
                (DrtTest) delegate() { this.Verify('B', "A", "", "Canceled backward journal navigation using EventArgs"); },
                // "cancel" a navigation by throwing an exception in OnNavigating
                (DrtTest) delegate() { _throwOnNextNavigation = true; try { NavWin.GoBack(); } catch{ /* expected path */ } },
                (DrtTest) delegate() { this.Verify('B', "A", "", "Exception on back journal navigation"); },
                 //Cancel a (non-journal) navigation by starting a new one
                (DrtTest) delegate() { this.NavigateTo('C'); this.NavigateTo('D'); },
                (DrtTest) delegate() { this.Verify('D', "BA", "", "Canceled a non-journal navigation by starting a new one (to D)"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify('B', "A", "D", "Went back."); },
               // Cancel a backward navigation
                (DrtTest) delegate() { this.GoBack(); this.StopLoading(); },
                (DrtTest) delegate() { this.Verify('B', "A", "D", "Canceled backward journal navigation with StopLoading"); },
                // Cancel a forward navigation
                (DrtTest) delegate() { this.NavigateTo('C'); },
                (DrtTest) delegate() { this.Verify('C', "BA", "", "Navigated to C"); },
                (DrtTest) delegate() { GoBack(); },
                (DrtTest) delegate() { this.Verify('B', "A", "C", "Navigate back to B puts C on forward stack"); },
                (DrtTest) delegate() { GoForward(); StopLoading(); },
                (DrtTest) delegate() { this.Verify('B', "A", "C", "Canceled forward journal navigation"); },
                // Remove the back entry
                (DrtTest) delegate() { NavWin.RemoveBackEntry(); },
                (DrtTest) delegate() { this.Verify('B', "", "C", "Back entry removed."); },
                (DrtTest) delegate() { GoForward(); },
                (DrtTest) delegate() { this.Verify('C', "B", "", "Went forward."); },

                // Test GoBack() GoBack() without waiting for first to finish
                (DrtTest) delegate() { this.NavigateTo('D'); },
                (DrtTest) delegate() { this.Verify('D',"CB","", "Verify starting state"); },
                (DrtTest) delegate() { GoBack(); DRT.Assert(NavWin.CanGoForward); GoBack(); }, 
                (DrtTest) delegate() { this.Verify('B',"","CD", "Went back twice to B without finishing C"); },
                // Test GoForward() GoForward() without waiting for first to finish
                (DrtTest) delegate() { GoForward(); DRT.Assert(NavWin.CanGoBack); GoForward(); }, 
                (DrtTest) delegate() { this.Verify('D',"CB","", "Went Forward twice to D without finishing C"); },
                // test GoBack() GoBack() with cancel
                (DrtTest) delegate() { GoBack(); GoBack(); StopLoading(); }, 
                (DrtTest) delegate() { this.Verify('D',"CB","", "GoBack twice was stopped"); },
                // test canceling a JumpBack
                (DrtTest) delegate() { _cancelNextNavigation = true; JumpBack('B'); DRT.Resume();},
                (DrtTest) delegate() { this.Verify('D',"CB","", "Jump back was canceled"); },
                // test navigating to null
                (DrtTest) delegate() { NavWin.Navigate(null); },
                (DrtTest) delegate() { DRT.Assert(NavWin.Content == null); },
                (DrtTest) delegate() { GoBack(); },
                (DrtTest) delegate() { this.Verify('D',"CB","", "Null should not be journaled"); },
                // test GoBack() GoForward() with no forward stack and no "current" journal entry to go forward to
                (DrtTest) delegate() { NavigateTo('E'); },
                (DrtTest) delegate() { NavWin.GoBack(); DRT.Assert(NavWin.CanGoForward); NavWin.GoForward(); },
                (DrtTest) delegate() { Verify('E',"DCB","", "Should result in same location"); },
                // Test GoBack() GoForward() in the middle of the journal, without waiting
                (DrtTest) delegate() { NavWin.GoBack(); DRT.Suspend(); },
                (DrtTest) delegate() { NavWin.GoBack(); NavWin.GoForward(); },
                (DrtTest) delegate() { Verify('D',"CB","E", "Quick Back-Fwd in the middle of the journal"); },
                // Test GoForward() GoBack() in the middle of the journal, without waiting
                (DrtTest) delegate() { NavWin.GoForward(); NavWin.GoBack(); },
                (DrtTest) delegate() { Verify('D',"CB","E", "Quick Fwd-Back in the middle of the journal"); },
                
                // Verify Journal serialization is not broken
                // Keep-alive journal entries should be pruned when serializing the journal into
                // the browser's TravelLog. This includes journal entries for child frame navigations.
                // The test below creates non-serializable entries and verifies pruning.
                (DrtTest) delegate() 
                { 
                    // PageD.xaml has a child frame. 
                    Page page = (Page)NavWin.Content;
                    Frame f = (Frame)page.FindName("ChildFrame");
                    f.Content = "Frame content"; // This will be considered "keep-alive" content.

                    //[DRT.Suspend() not needed here because the object navigation callback has
                    // higher priority than the next DrtTest. Besides, there is no LoadCompleted
                    // handler attached to the frame.]
                },
                (DrtTest) delegate() 
                { 
                    DRT.Assert(NavWin.Source.ToString().EndsWith("PageD.xaml"));
                    Page page = (Page)NavWin.Content;

                    // Now replace the page containing the frame with another page. 
                    // A normal JournalEntryUri will be created for the old page, but a JournalEntryKeepAlive 
                    // should be stored in its JournalDataStreams to remember the frame's content.
                    // (Done in DataStreams.SaveState.)
                    NavWin.Content = new Page();
                },
                // This causes a JournalEntryKeepAlive to be created for the new Page object.
                (DrtTest) delegate() { NavWin.Navigate(null); },
                (DrtTest) delegate() 
                { 
                    bool failed = false;
                    try { SerializeJournal(); }
                    catch(System.Runtime.Serialization.SerializationException) { failed = true; }
                    DRT.Assert(failed);

                    PropertyInfo info = NavWin.GetType().GetProperty("Journal", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
                    object journal = info.GetValue(NavWin, null);
                    journal.GetType().InvokeMember("PruneKeepAliveEntries", 
                        BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, 
                        null, journal, null);

                    SerializeJournal(); // Should succeed now.
                },                

            };
        }

        private void NavigateTo(char page)
        {
            string partialPath = @"Basic\Page" + page + ".xaml";
            Console.WriteLine("Navigating to " + partialPath);
            this.NavWin.Navigate(new Uri(partialPath, UriKind.RelativeOrAbsolute));
            DRT.Suspend();
        }

        private void GoBack()
        {
            // Go back by invoking the NavigationWindow's back button
            Console.WriteLine("Clicking the Back button");
            DoClick(this.GetBackButton());
            DRT.Suspend();
        }

        private void GoForward()
        {
            // Go forward by invoking the NavigationWindow's forward button
            Console.WriteLine("Clicking the Forward button");
            DoClick(this.GetForwardButton());
            DRT.Suspend();
        }

        private void StopLoading()
        {
            this.NavWin.StopLoading();
        }

        /// <summary>
        /// Returns the actual journal entry name with which the given page was supposedly journaled.
        /// This is what the corresponding drop-down menu item shows.
        /// </summary>
        private string JournalEntryName(char page)
        {
            // ~ "Page A (Basic/PageA.xaml)".  In this pattern "Page A" comes from Page.WindowTitle.
            return "Page " + page + " (Basic/Page" + page + ".xaml)";
        }

        private void JumpBack(char page)
        {
            base.JumpBack(JournalEntryName(page));
        }

        private void JumpForward(char page)
        {
            base.JumpForward(JournalEntryName(page));
        }

        private void Verify(char page, string backStack, string forwardStack, string text)
        {
            Verify(page, backStack, backStack, forwardStack, forwardStack, text);           
        }
        
        private void Verify(char page, string backStack, string visualBackStack, string forwardStack, string visualForwardStack, string text)
        {
            // Test that the correct page is being displayed
            DRT.AssertEqual(page, this.GetName(this.NavWin.CurrentSource.ToString()), text + " (Content)");
            
            // Test the contents of the back stack
            DRT.AssertEqual(backStack, this.GetBackStack(), text + " (BackStack)");
            // Test the contents of the back button menu
            DRT.AssertEqual(visualBackStack, this.GetBackMenu(), text + " (Back Menu)");
            
            // Test the contents of the forward stack
            DRT.AssertEqual(forwardStack, this.GetForwardStack(), text + " (ForwardStack)");
            // Test the contents of the forward menu
            DRT.AssertEqual(visualForwardStack, this.GetForwardMenu(), text + " (Forward Menu)");
            
            // Test the state of NavigationWindow.CanGoBack
            DRT.AssertEqual(backStack.Length != 0, this.NavWin.CanGoBack, text + " (NavigationWindow.CanGoBack)");
            // Test the IsEnabled property of the back button
            DRT.AssertEqual(backStack.Length != 0, this.GetBackButton().IsEnabled, text + " (Back button IsEnabled)");
            
            // Test the state of NavigationWindow.CanGoForward
            DRT.AssertEqual(forwardStack.Length != 0, this.NavWin.CanGoForward, text + " (NavigationWindow.CanGoForward)");
            // Test the IsEnabled property of the forward button
            DRT.AssertEqual(forwardStack.Length != 0, this.GetForwardButton().IsEnabled, text + " (Forward button IsEnabled)");

            // make sure that we do not expose the absolute uri
            DRT.Assert(this.NavWin.CurrentSource.IsAbsoluteUri == false, "NavigationWindow.CurrentSource should not expose an absolute Uri");
        }

        protected override string GetStack(IEnumerable entries)
        {
            StringBuilder stack = new StringBuilder();
            IEnumerator entryEnumerator = entries.GetEnumerator();
            while (entryEnumerator.MoveNext())
            {
                JournalEntry entry = (JournalEntry)entryEnumerator.Current;
                stack.Append(this.GetName(entry.Name));
            }

            return stack.ToString();
        }

        private string GetBackMenu()
        {
            MenuItem journalEntryMenu = GetJournalEntryMenu();
            StringBuilder stack = new StringBuilder();

            bool passedCurrent = false;
            for (int index = 0; index < journalEntryMenu.Items.Count; ++index)
            {
                object item = journalEntryMenu.Items[index];
                if (!(item is JournalEntry))
                {
                    DependencyObject curr = item as DependencyObject;
                    DRT.Assert(curr != null);
                    DRT.Assert((string)curr.GetValue(JournalEntry.NameProperty) == "Current Page");
                    passedCurrent = true;
                }

                else if (passedCurrent)
                {
                    stack.Append(this.GetName(((JournalEntry)item).Name));
                }
            }

            return stack.ToString();
        }

        private string GetForwardMenu()
        {
            MenuItem journalEntryMenu = GetJournalEntryMenu();
            StringBuilder stack = new StringBuilder();

            bool passedCurrent = false;
            for (int index = journalEntryMenu.Items.Count - 1; index >= 0; --index)
            {
                object item = journalEntryMenu.Items[index];
                if (!(item is JournalEntry))
                {
                    DependencyObject curr = item as DependencyObject;
                    DRT.Assert(curr != null);
                    DRT.Assert((string)curr.GetValue(JournalEntry.NameProperty) == "Current Page");
                    passedCurrent = true;
                }
                else if (passedCurrent)
                {
                    stack.Append(this.GetName(((JournalEntry)item).Name));
                }
            }

            return stack.ToString();
        }
        

        private char GetName(string entryName)
        {
            int index = entryName.IndexOf("Page");

            if (index == -1)
                return '?';

            if (entryName[index + 4] != ' ')
                return entryName[index + 4];
            else
                return entryName[index + 5];
        }

        private IEnumerator _stackEnumerator = null;

        private void CreateForwardStackEnumerator()
        {
            IEnumerable forwardStack = (IEnumerable)NavWin.GetValue(NavigationWindow.ForwardStackProperty);
            _stackEnumerator = forwardStack.GetEnumerator();
            _stackEnumerator.MoveNext(); // ok
        }

        private void TestForwardStackEnumeratorInvalidated()
        {
            try
            {
                _stackEnumerator.MoveNext(); // error
                DRT.Fail("Forward stack enumerator not invalidated");
            }
            catch (InvalidOperationException)
            {
                // expected code path
            }
            catch (Exception e)
            {
                DRT.Fail("Unexpected Exception was thrown: {0}", e.Message);
            }
            _stackEnumerator = null;
        }

        private void CreateBackStackEnumerator()
        {
            IEnumerable backStack = (IEnumerable) NavWin.GetValue(NavigationWindow.BackStackProperty);
            _stackEnumerator = backStack.GetEnumerator();
            _stackEnumerator.MoveNext(); // ok
        }

        private void TestBackStackEnumeratorInvalidated()
        {
            try
            {
                _stackEnumerator.MoveNext(); // error
                DRT.Fail("Back stack enumerator not invalidated");
            }
            catch (InvalidOperationException)
            {
                // expected code path
            }
            catch (Exception e)
            {
                DRT.Fail("Unexpected Exception was thrown: {0}", e.Message);
            }
            _stackEnumerator = null;
        }

        private void SerializeJournal()
        {
            PropertyInfo info = NavWin.GetType().GetProperty("Journal", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
            object journal = info.GetValue(NavWin, null);
            
            MemoryStream saveStream = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(saveStream, journal);
        }

        /// <summary>
        /// Helper. Invokes Click on a given ButtonBase. 
        /// </summary>
        private void DoClick(ButtonBase b)
        {
            MethodInfo info = typeof(ButtonBase).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            if (info == null) throw new Exception("Could not find ButtonBase.OnClick method");
            info.Invoke(b, new object[] { });
        }
    }
}
