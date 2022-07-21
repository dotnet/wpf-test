// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Text;
using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using System.Windows.Media;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Windows.Interop;

using DRT;

namespace DrtJournal
{
    /// <summary>
    /// See the comment for class PageA for a description of the test.
    /// </summary>
    public class CustomJournalingSuite : JournalTestSuite
    {
        public CustomJournalingSuite()
            : base("Custom Navigation/Journaling")
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
                    this.NavWin.Navigate(new Uri(@"Custom\PageA.xaml", UriKind.Relative));
                    // Wait for navigation to complete (it's asynchronous). The LoadCompleted
                    // handler will call DRT.Resume().
                    DRT.Suspend(); 
                },
                (DrtTest) delegate() 
                {
                    _pageA = (CustomJournaling.PageA)NavWin.Content;
                    this.Verify(Colors.Black, "", ""); // initial color from XAML definition
                    NavWin.RemoveBackEntry(); // should just do nothing
                    _pageA.JournalAndChangeBlockColor(Colors.Blue);
                    Verify(Colors.Blue, "Black", "");
                    _pageA.JournalAndChangeBlockColor(Colors.Red);
                    _pageA.JournalAndChangeBlockColor(Colors.DeepPink);
                    Verify(Colors.DeepPink, "Red Blue Black", "");
                    NavWin.GoBack();
                    Verify(Colors.Red, "Blue Black", "DeepPink");
                    NavWin.GoForward();
                    Verify(Colors.DeepPink, "Red Blue Black", "");
                    _pageA.JournalAndChangeBlockColor(Colors.Green);
                    Verify(Colors.Green, "DeepPink Red Blue Black", "");
                    NavWin.RemoveBackEntry();
                    Verify(Colors.Green, "Red Blue Black", "");
                    NavWin.GoBack();
                    Verify(Colors.Red, "Blue Black", "Green");
                    NavWin.RemoveBackEntry(); // test in the middle of the journal
                    Verify(Colors.Red, "Black", "Green");
                    _pageA.JournalAndChangeBlockColor(Colors.Green);
                    Verify(Colors.Green, "Red Black", ""); // Fwd stack cleared
                    _pageA.JournalAndChangeBlockColor(Colors.Lavender);
                    Verify(Colors.Lavender, "Green Red Black", ""); 

                    // Change the page state directly. The changed state should be recorded on
                    // the next navigation. Since we go back, Firebrick should be in the Fwd stack.
                    _pageA.BlockColor = Colors.Firebrick;
                    NavWin.GoBack();
                    Verify(Colors.Green, "Red Black", "Firebrick"); 
                    NavWin.GoBack();
                    NavWin.RemoveBackEntry();
                    Verify(Colors.Red, "", "Green Firebrick"); 
                    NavWin.RemoveBackEntry(); 
                    Verify(Colors.Red, "", "Green Firebrick"); 

                    // Refresh() should reload the whole page. Because the operation is async,
                    // we suspend and wait for completion.
                    NavWin.Refresh(); DRT.Suspend(); // The LoadedComleted handler will Resume.
                },
                (DrtTest) delegate() 
                {
                    //[Continued]
                    // Since the page is not KeepAlive, the reference to it needs to be reset.
                    UpdatePageReference(true/*assertDifferent*/);
                    // The expected state is Black because Refresh() reloads the page.
                    Verify(Colors.Black, "", "Green Firebrick"); 

                    _pageA.JournalAndChangeBlockColor(Colors.RoyalBlue);
                    Verify(Colors.RoyalBlue, "Black", ""); 
                    _pageA.JournalAndChangeBlockColor(Colors.Yellow);
                    _pageA.JournalAndChangeBlockColor(Colors.Maroon);
                    Verify(Colors.Maroon, "Yellow RoyalBlue Black", ""); 
                },
                (DrtTest) delegate() 
                {
                    // Do a little jumping around
                    JumpBack("Black"); 
                    // MenuItem clicks are posted as Dispatcher operations, so we need to interrupt
                    // the current DrtTest. 
                },
                (DrtTest) delegate() 
                {
                    //[Continued]
                    Verify(Colors.Black, "", "RoyalBlue Yellow Maroon"); 
                    _pageA.BlockColor = Colors.SeaGreen; // This will replace Black in the journal.
                    JumpForward("Yellow");
                },
                (DrtTest) delegate() 
                {
                    //[Continued]
                    Verify(Colors.Yellow, "RoyalBlue SeaGreen", "Maroon"); 

                    // Try another Refresh. The "Yellow" state will be lost.
                    NavWin.Refresh(); DRT.Suspend(); 
                },
                (DrtTest) delegate() 
                {
                    //[Continued]
                    UpdatePageReference();
                    Verify(Colors.Black, "RoyalBlue SeaGreen", "Maroon"); 

                    // Test when IProvideCustomContentState.GetContentState() is automatically 
                    // called by the framework
                    _pageA.BlockColor = Colors.Violet; // set new page state
                    NavWin.Navigate(new Uri(@"Custom\PageB.xaml", UriKind.Relative));
                    DRT.Suspend();
                },
                (DrtTest) delegate() 
                {
                    // We are at PageB now. But since we still have the reference to PageA, the 
                    // current color test will succeed.
                    // Only a single item will appear in the back stack because only the exit
                    // journal entry for the previous content is shown.
                    Verify(Colors.Violet, "Violet", ""); 

                    NavWin.GoBack(); DRT.Suspend();   
                },
                (DrtTest) delegate() 
                {
                    UpdatePageReference();
                    Verify(Colors.Violet, "RoyalBlue SeaGreen", "Page B"); 

                    // Similar test as the one just completed, but with GoForward...
                    _pageA.BlockColor = Colors.Navy; // set new page state
                    NavWin.GoForward(); DRT.Suspend();
                },
                (DrtTest) delegate() 
                {
                    Verify(Colors.Navy, "Navy", ""); 
                    NavWin.GoBack(); DRT.Suspend();   
                },
                (DrtTest) delegate() 
                {
                    UpdatePageReference();
                    Verify(Colors.Navy, "RoyalBlue SeaGreen", "Page B"); 

                    // Navigating to a null object should update the entry for the current page
                    // Then, going back should not create en entry for the null content.
                    NavWin.GoBack();
                    _pageA.BlockColor = Colors.Moccasin; 
                    NavWin.Navigate(null);
                },
                (DrtTest) delegate() 
                {
                    DRT.Assert(NavWin.Content == null);
                    //(Again, only one entry for PageA will appear on the back stack.)
                    Verify(Colors.Moccasin, "Moccasin", ""); 
                    NavWin.GoBack(); DRT.Suspend();   
                },
                (DrtTest) delegate() 
                {
                    UpdatePageReference();
                    Verify(Colors.Moccasin, "SeaGreen", ""); 

                    // Finally, test the interplay with fragment navigation. The current framework
                    // support is not optimal for all applications and might be changed. It does
                    // not really distinguish between fragment and CustomContentState navigations.
                    // CustomContentState is requested (but not required) and replayed for fragment 
                    // navigations too. This will work for Mongoose, for example, but if an application
                    // needs to handle the two navigation types differently, it will have to use the
                    // FragmentNavigation callback, raise an internal flag, and possibly return null
                    // from its GetContentState() implementation or alter the JournalEntryName to
                    // reflect the original fragment location.
                    _pageA.BlockColor = Colors.Honeydew; 
                    NavWin.Navigate(new Uri("#bottom", UriKind.Relative));
                    //DRT.Assert(NavWin.Source.Fragment == "#bottom");
                    //--Causes NotSupportedException because the URI is relative [?!]
                    DRT.Assert(NavWin.Source.ToString().EndsWith("#bottom"));
                    Verify(Colors.Honeydew, "Honeydew SeaGreen", ""); 
                    _pageA.JournalAndChangeBlockColor(Colors.Fuchsia);
                    Verify(Colors.Fuchsia, "Honeydew Honeydew SeaGreen", ""); 
                    DRT.Assert(NavWin.Source.ToString().EndsWith("#bottom"));
                    NavWin.Navigate(new Uri("#top", UriKind.Relative));
                    DRT.Assert(NavWin.Source.ToString().EndsWith("#top"));
                    Verify(Colors.Fuchsia, "Fuchsia Honeydew Honeydew SeaGreen", ""); 
                    _pageA.BlockColor = Colors.Black; // to be able to fully verify the result of GoBack
                    NavWin.GoBack(); 
                    DRT.Assert(NavWin.Source.ToString().EndsWith("#bottom"));
                    Verify(Colors.Fuchsia, "Honeydew Honeydew SeaGreen", "Black"); 
                    NavWin.GoBack(); 
                    DRT.Assert(NavWin.Source.ToString().EndsWith("#bottom"));
                    Verify(Colors.Honeydew, "Honeydew SeaGreen", "Fuchsia Black"); 
                    _pageA.BlockColor = Colors.Yellow; // to be able to fully verify the result of GoBack
                    NavWin.GoBack(); 
                    DRT.Assert(NavWin.Source.ToString().IndexOf('#') == -1);
                    Verify(Colors.Honeydew, "SeaGreen", "Yellow Fuchsia Black"); 
                }
            };
        }

        CustomJournaling.PageA _pageA;

        void UpdatePageReference()
        {
            UpdatePageReference(true);
        }
        void UpdatePageReference(bool assertDifferent)
        {
            DRT.Assert(!assertDifferent || !object.ReferenceEquals(_pageA, NavWin.Content));
            _pageA = (CustomJournaling.PageA)NavWin.Content;
        }

        void Verify(Color blockColor, string backStack, string forwardStack)
        {
            DRT.Assert(_pageA.BlockColor == blockColor);
            DRT.Assert(GetBackStack() == backStack);
            DRT.Assert(backStack.Length == 0 ^ NavWin.CanGoBack);
            DRT.Assert(GetForwardStack() == forwardStack);
            DRT.Assert(forwardStack.Length == 0 ^ NavWin.CanGoForward);
        }

        private void GoBack()
        {
            NavWin.GoBack();
            DRT.Suspend();
        }
        private void GoForward()
        {
            NavWin.GoForward();
            DRT.Suspend();
        }
   };
}
