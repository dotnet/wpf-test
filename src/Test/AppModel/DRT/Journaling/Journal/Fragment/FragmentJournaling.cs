// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows;
using System.Text;
using System.Diagnostics;
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
    /*
    This suite exercises fragment navigation & journaling. All fragment navigation is done within
    FragPage.xaml. It has "top" and "bottom" named target elements, and a few fragment navigations
    are done between them.
     
    To make things more interesting, FragPage is loaded in a Frame (in PageWithFrame.xaml),
    and both the Frame and its parent (the entire NavigationWindow) are navigated to different content
    and then, after going Back, the fragment navigation journal entries are verified.
     
    Fragment navigation with a URI-less element tree is also tested.
     
    Special features tested are:
     -- Journaling of controls state (DPs with FrameworkMetadataOptions.Journal).
     -- Journal entry grouping [introduced as resolution]:
        1) All JEs created for fragment navigation within a given page share the same controls state
           (the same JournalEntryGroupState internally), and fragment navigation within a page never
            changes controls state.
        2) After navigating to a different page, only the "exit" entry for the previous page should
            be visible in the backstack. (This is not necessarily the last entry.) After going back,
            all other entries become visible and available for journal navigation.
*/
    public class FragmentJournalingSuite : JournalTestSuite
    {
        public FragmentJournalingSuite()
            : base("Fragment Journaling")
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

                    this.NavWin.Source = new Uri(@"Fragment\PageWithFrame.xaml", UriKind.Relative);
                    // The frame's Source is set to "FragPage.xaml".
                    _textBoxText = "";

                    // Interrupting the current DrtTest to allow navigation to complete (it's async).
                },
                (DrtTest) delegate() 
                {
                    Frame f = GetFrame();
                    WireNavigationEvents(f); // just for the -verbose output
                    DRT.Assert(f.Content is FragPage);
                    FragPage page = (FragPage)f.Content;

                    f.Navigate(new Uri("#bottom", UriKind.Relative));
                    // Fragment navigation is synchronous, so no need to wait.
                    Verify("#bottom", "FragPage", "");

                    SetTextBox("Stay put");
                    f.Navigate(new Uri("#top", UriKind.Relative));
                    Verify("#top", "FragPage#bottom FragPage", ""); // also checks the TextBox's text

                    SetTextBox("Stay put 2");
                    GoBack();
                    Verify("#bottom", "FragPage", "FragPage#top");

                    GoForward();
                    Verify("#top", "FragPage#bottom FragPage", "");

                    // Replace the frame's content with another page.
                    _fragPage = page;
                    Page page2 = new Page();
                    page2.Title = "Page2";
                    f.Navigate(page2);
                },
                (DrtTest) delegate() 
                {
                    DRT.Assert(GetBackStack() == "FragPage#top", 
                        "When at another page, only the exit entry of all fragment navigation entries should "+
                        "be visible in the back stack.");

                    GoBack();
                },
                (DrtTest) delegate() 
                {
                    FragPage page = GetFragPage();
                    DRT.Assert(page != _fragPage, "FragPage should be journaled by URI, not as keep-alive.");
                    _fragPage = null;

                    Verify("#top", "FragPage#bottom FragPage", "Page2");
                    // Verify() also makes sure the TextBox text is equal to _textBoxText.
                    GoBack();
                    Verify("#bottom", "FragPage", "FragPage#top Page2");

                    SetTextBox("Stay put 3");
                    JumpForward("Page2");
                    DRT.Resume(); //JumpForward() calls Suspend(), but it it doesn't actually seem needed anymore.
                },
                (DrtTest) delegate() 
                {
                    Frame f = GetFrame();
                    DRT.Assert(((Page)f.Content).Title == "Page2");
                    DRT.Assert(GetBackStack() == "FragPage#bottom", 
                        "When at another page, only the exit entry of all fragment navigation entries should "+
                        "be visible in the back stack.");

                    GoBack();
                },
                (DrtTest) delegate() 
                {
                    Verify("#bottom", "FragPage", "FragPage#top Page2");

                    GoForward();
                    Verify("#top", "FragPage#bottom FragPage", "Page2");

                    GoBack();

                    // Navigate to entire NavigationWindow to another page.
                    // This will clear the Fwd stack.
                    NavWin.Navigate(new Uri(@"Basic\PageA.xaml", UriKind.Relative));
                    _pageWithFrame = (PageWithFrame)NavWin.Content;
                    _fragPage = GetFragPage();
                },
                (DrtTest) delegate() 
                {
                    DRT.Assert(GetBackStack() == "Page with Frame", "The entire NavigationWindow was "+
                        "navigated; so, no journal entries associated with the previous page's Frame "+
                        "should be visible in the backstack.");
                    DRT.Assert(!NavWin.CanGoForward);

                    Page page = (Page)NavWin.Content;
                    page.Title = "PageA"; 
                        // otherwise it's "Page A (Basic/PageA.xaml)" -- too unwieldy for fwd stack verification
                    GoBack();
                },
                (DrtTest) delegate() 
                {
                    DRT.Assert(NavWin.Content is PageWithFrame);
                    DRT.Assert(NavWin.Content != _pageWithFrame, 
                        "PageWithFrame should be journaled by URI, not as keep-alive.");
                    _pageWithFrame = null;

                    FragPage page = GetFragPage();
                    DRT.Assert(page != _fragPage, "FragPage should be journaled by URI, not as keep-alive.");
                    _fragPage = null;

                    Verify("#bottom", "FragPage", "PageA");

                    GoBack();
                    Verify("", "", "FragPage#bottom PageA");

                    Frame f = GetFrame();
                    // This should be the same as "#bottom".
                    f.Navigate(new Uri(@"Fragment\FragPage.xaml#bottom", UriKind.Relative));
                    Verify("#bottom", "FragPage", "");

                    // Finally, test Refresh(). It should do full reloading and discard any changed
                    // controls state.
                    f.Refresh();
                    Debug.Assert(_textBoxText.Length > 0); 
                    _textBoxText = "";
                },
                (DrtTest) delegate() 
                {
                    Verify("#bottom", "FragPage", "");
                    GoBack();
                    Verify("", "", "FragPage#bottom");

                    SetTextBox("New text value");
                    GoForward();
                    Verify("#bottom", "FragPage", "");

                    // This should still be handled as fragment navigation within the current page.
                    Frame f = GetFrame();
                    f.Navigate(new Uri(@"Fragment\FragPage.xaml#top", UriKind.Relative));
                    Verify("#top", "FragPage#bottom FragPage", "");

                    GoBack();
                    SetTextBox("");

                    //
                    // Test a bit of fragment navigation with a URI-less element tree.
                    //
                    // First, navigate the frame to FragPage as an object.
                    FragPage fp2 = new FragPage();
                    fp2.InitializeComponent();
                    fp2.Title = "FragPage2";
                    f.Navigate(fp2);
                },
                (DrtTest) delegate() 
                {
                    Verify("", "FragPage#bottom"/*journal entry group collapsed*/, "");

                    Frame f = GetFrame();
                    f.Navigate(new Uri("#bottom", UriKind.Relative));
                    Verify("#bottom", "FragPage2 FragPage#bottom", "");

                    f.Source = null;
                },
                (DrtTest) delegate() 
                {
                    Frame f = GetFrame();
                    DRT.Assert(f.Content == null);

                    // Regression test for Returning from some URI to <keep-alive object>#fragment.
                    f.Navigate(new Uri(@"Basic\PageB.xaml", UriKind.Relative));
                },
                (DrtTest) delegate() 
                {
                    Verify("", "FragPage2#bottom FragPage#bottom", "", false);
                    Page p = (Page)GetFrame().Content;
                    p.Title = "PageB"; // Override the default journal entry name, which is the page's URI.
                    GoBack();
                },
                (DrtTest) delegate() 
                {
                    Verify("#bottom", "FragPage2 FragPage#bottom", "PageB");
                    //[End regression test.]

                    GoBack();
                    Verify("", "FragPage#bottom", "FragPage2#bottom PageB");

                    GoBack();
                },
                (DrtTest) delegate() 
                {
                    Verify("#bottom", "FragPage", "FragPage2 PageB");
                },
                (DrtTest) delegate() 
                {
                    GoForward();
                },
                (DrtTest) delegate() 
                {
                    Verify("", "FragPage#bottom", "FragPage2#bottom PageB");
                    GoForward();
                    Verify("#bottom", "FragPage2 FragPage#bottom", "PageB");
                },
                (DrtTest) delegate() 
                {
                    // [New test]
                    // Test non-consecutive navigations to the same object and jumping b/w journal 
                    // entries corresponding to the different navigations. This should be handled
                    // as fragment navigation, but journal entry grouping should still work correctly.
                    // (NavigationService._contentId needs to be updated.)
                    _fragPage = GetFragPage();
                    DRT.Assert(_fragPage.Title == "FragPage2");
                    GoForward(); // on to PageB
                },
                (DrtTest) delegate() 
                {
                    Verify("", "FragPage2#bottom FragPage#bottom", "", false);
                    Frame frame = GetFrame();
                    frame.Navigate(_fragPage);
                },
                (DrtTest) delegate() 
                {
                    Verify("", "Basic/PageB.xaml FragPage2#bottom FragPage#bottom", "");
                    DRT.Assert(GetFragPage() == _fragPage);

                    GetFrame().Navigate(new Uri("#bottom", UriKind.Relative));
                    Verify("#bottom", "FragPage2 Basic/PageB.xaml FragPage2#bottom FragPage#bottom", "");
                    GoBack();
                    Verify("", "Basic/PageB.xaml FragPage2#bottom FragPage#bottom", "FragPage2#bottom");
                },
                (DrtTest) delegate() 
                {
                    // Going back twice should be the same as jumping to the 'FragPage2#bottom' entry.
                    // Because the content object is the same, this should be handled as fragment
                    // navigation, which is synchronous. That's why the verification is immediate.
                    GoBack();
                    GoBack();
                    Verify("#bottom", "FragPage2 FragPage#bottom", "Basic/PageB.xaml FragPage2");
                    //(Notice the 'FragPage#bottom' reappears in the backstack, while 'FragPage2#bottom'
                    // is filtered out of the forward stack.)

                    //[End of test for non-consecutive navigations to the same object]
                },
            };
        }

        FragPage _fragPage;
        PageWithFrame _pageWithFrame;
        string _textBoxText;

        void WireNavigationEvents(Frame f)
        {
            f.Navigating += base.OnNavigating;
            f.LoadCompleted += base.OnLoadCompleted;
        }

        Frame GetFrame()
        {
            PageWithFrame page = NavWin.Content as PageWithFrame;
            DRT.Assert(page != null, "NavigationWindow.Content was expected to be PageWithFrame.");
            return page.MyFrame;
        }

        FragPage GetFragPage()
        {
            Frame frame = GetFrame();
            FragPage fp = frame.Content as FragPage;
            DRT.Assert(fp != null, "FragPage was not found in the Frame.");
            return fp;
        }

        void SetTextBox(string text)
        {
            FragPage page = GetFragPage();
            page.TextBox1.Text = text;
            _textBoxText = text;
        }

        static string FragmentName(Uri uri)
        {
            if (uri == null)
                return null;
            // uri.Fragment causes NotSupportedException because the URI is relative [?!]
            string s = uri.ToString();
            int i = s.IndexOf('#');
            if (i == -1)
                return null;
            return s.Substring(i + 1);
        }

        void Verify(string expectedFragment, string backStack, string forwardStack)
        {
            Verify(expectedFragment, backStack, forwardStack, true);
        }
        void Verify(string expectedFragment, string backStack, string forwardStack, bool verifyFragPageWithTextBox)
        {
            Frame f = GetFrame();

            if (expectedFragment != null)
            {
                if (expectedFragment.Length == 0)
                {
                    expectedFragment = null;
                }
                else if (expectedFragment[0] == '#')
                {
                    expectedFragment = expectedFragment.Substring(1);
                }
            }
            string currentFragment = FragmentName(f.CurrentSource);
            if(expectedFragment != currentFragment)
            {
                DRT.Fail("The current URI fragment is '{0}'; '{1}' was expected.", 
                        currentFragment, expectedFragment);
            }

            DRT.Assert(GetBackStack() == backStack, "The back stack was expected to be "+backStack);
            DRT.Assert(backStack.Length == 0 ^ NavWin.CanGoBack);
            DRT.Assert(GetForwardStack() == forwardStack, "The forward stack was expected to be "+forwardStack);
            DRT.Assert(forwardStack.Length == 0 ^ NavWin.CanGoForward);

            if (verifyFragPageWithTextBox)
            {
                FragPage page = f.Content as FragPage;
                DRT.Assert(page != null, "FragPage was not found in the Frame.");
                DRT.Assert(page.TextBox1.Text == _textBoxText, "The TextBox text was not journaled properly.");
            }
        }

        private void GoBack()
        {
            Console.WriteLine("Going Back");
            NavWin.GoBack();
        }
        private void GoForward()
        {
            Console.WriteLine("Going Forward");
            NavWin.GoForward();
        }
   };
}
