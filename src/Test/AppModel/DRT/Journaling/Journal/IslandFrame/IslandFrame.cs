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
     * This suite exercises the different journaling modes of Frame, determined by the JournalOwnership
     * property. By default, Frame uses its parent's journal, if available. Informally, an "island frame"
     * is a frame that has its own journal (JournalOwnership=OwnsJournal). When in this mode, Frame 
     * behaves similarly to NavigationWindow: it has BackStack & ForwardStack, can show a navigation
     * chrome, and can handle the main NavigationCommands. Frame's navigation chrome is also tested here.
    */
    public class IslandFrameSuite : JournalTestSuite
    {
        public IslandFrameSuite()
            : base("Island Frame")
        {
            this.Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[] {
                // As in the other journaling & navigation suites, this test sequence is broken up
                // into multiple DrtTest delegates to allow asynchronous navigations to complete before
                // continuing.
                (DrtTest) delegate()
                {
                    (DRT as DrtJournalBase).UseNewNavigationWindow();
                    WireNavigationEvents();

                    this.NavWin.Source = new Uri(@"Basic/PageA.xaml", UriKind.Relative);
                },
                (DrtTest) delegate() 
                {
                    this.NavWin.Source = new Uri(@"/IslandFrame/PageWithFrame.xaml", UriKind.Relative);
                    // The frame's Source is set to "Basic/PageB.xaml".
                },
                (DrtTest) delegate() 
                {
                    DRT.Assert(NavWin.Title == "Page with Frame");

                    Frame f = GetFrameFromPageWithFrame();
                    WireNavigationEvents(f);

                    DRT.Assert(f.JournalOwnership == JournalOwnership.UsesParentJournal,
                        "Frame should have started using NavigationWindow's journal.");
                    DRT.Assert(f.NavigationUIVisibility == NavigationUIVisibility.Automatic); // default

                    Verify(f, "PageB.xaml", null/*backStack*/, null/*fwdStack*/);

                    // This frame navigation will create an entry in NavWin's journal.
                    f.Navigate(new Uri(@"Basic/PageC.xaml", UriKind.Relative));
                },
                (DrtTest) delegate() 
                {
                    Frame f = GetFrameFromPageWithFrame();
                    Verify(f, "PageC.xaml", null, null);

                    VerifyGoBackIsInvalid(f, "Frame.GoBack should be available only when the frame has its own journal.");

                    bool failed = false;
                    try { f.RemoveBackEntry(); }
                    catch(InvalidOperationException) { failed = true; }
                    DRT.Assert(failed, "Frame.RemoveBackEntry() must not touch its parent's journal.");

                    NavigationService ns = f.NavigationService;
                    DRT.Assert(ns.Content == f.Content);
                    DRT.Assert(ns.CanGoBack, "Frame's NavigationService should use the parent journal when Frame doesn't have its own.");

                    f.Navigate(new Uri(@"Basic/PageD.xaml", UriKind.Relative));
                },
                (DrtTest) delegate() 
                {
                    Frame f = GetFrameFromPageWithFrame();
                    Verify(f, "PageD.xaml", null, null);
                },
                (DrtTest) delegate() 
                {
                    NavWin.GoBack(); // Navigate the frame back to Page C.
                },
                (DrtTest) delegate() 
                {
                    Frame f = GetFrameFromPageWithFrame();
                    Verify(f, "PageC.xaml", null/*backStack*/, null/*fwdStack*/);
                    DRT.Assert(NavWin.CanGoForward, 
                        "Back navigation in the child frame should create an entry in NavWin's Forward stack.");

                    //--------------------------------------------------------------------
                    //** From here on, the frame will use its own journal.

                    f.JournalOwnership = JournalOwnership.OwnsJournal;
                    _islandFrame = f;
                    //[Interrupting the regular programming for a Dispatcher break...]
                },
                (DrtTest) delegate() 
                {
                    Verify(_islandFrame, "PageC.xaml", ""/*empty stack*/, "");

                    DRT.Assert(!NavWin.CanGoForward, 
                        "Switching Frame from UsesParentJournal to OwnsJournal should remove the frame's "+
                        "journal entries from the parent journal.");
                    // One entry must have been deleted from NavWin's back stack and fwd stack. Now it 
                    // should have only one entry in the back stack - for its own navigation from Page A
                    // to PageWithFrame. This will be verified later on.

                    DRT.Assert(_islandFrame.RemoveBackEntry() == null); // should not throw now

                    DRT.Assert(NavWin.Title == "Page with Frame",
                        "Navigating an island frame to a Page that sets WindowTitle should not change "+
                        "the window's title.");

                    _islandFrame.Navigate(new Uri(@"Basic/PageD.xaml", UriKind.Relative));
                },
                (DrtTest) delegate() 
                {
                    Verify(_islandFrame, "PageD.xaml", "Basic/PageC.xaml", "");
                    VerifyNavigationMenus(_islandFrame);

                    //--------------------------------------------------------------------

                    // Page D has a child frame ('nestedFrame' below). It will be navigated several 
                    // times, expecting these navigations to be recorded in _islandFrame's journal. 
                    // Further, when _islandFrame's parent (the NavigationWindow) is navigated away, 
                    // _islandFrame's entire journal should be preserved and then restored when returning 
                    // to it, including the last page shown in the nested frame.

                    Frame nestedFrame = GetChildFrame(_islandFrame);
                    WireNavigationEvents(nestedFrame);
                    nestedFrame.Source = new Uri(@"PageE.xaml", UriKind.Relative);
                        // This will actually navigate to "pack://applicaton,,,/Basic/PageE.xaml" because
                        // the frame already has its BaseUri from PageD.
                },
                (DrtTest) delegate() 
                {
                    Verify(_islandFrame, "PageD.xaml", "Basic/PageC.xaml", "");
                    Frame nestedFrame = GetChildFrame(_islandFrame);
                    Verify(nestedFrame, "PageE.xaml", null, null);

                    // Navigate nestedFrame again. It should start using _islandFrame's journal.
                    nestedFrame.Source = new Uri(@"PageF.xaml", UriKind.Relative);
                },
                (DrtTest) delegate() 
                {
                    Verify(_islandFrame, "PageD.xaml", "Basic/PageE.xaml Basic/PageC.xaml", "");
                    VerifyNavigationMenus(_islandFrame);

                    Frame nestedFrame = GetChildFrame(_islandFrame);
                    DRT.Assert(nestedFrame.JournalOwnership == JournalOwnership.UsesParentJournal,
                        "The nested frame in Page D should have started using the parent frame's journal.");
                    DRT.Assert(nestedFrame.NavigationUIVisibility == NavigationUIVisibility.Automatic);
                    Verify(nestedFrame, "PageF.xaml", null/*no backStack--it doesn't own a journal*/, null);
    
                    VerifyGoBackIsInvalid(nestedFrame, 
                        "The nested frame should not be able to GoBack, because it uses its parent's journal.");

                    // This should cause nestedFrame to go back
                    _islandFrame.GoBack();
                },
                (DrtTest) delegate() 
                {
                    Verify(_islandFrame, "PageD.xaml", 
                        "Basic/PageC.xaml", "Basic/PageF.xaml"/*fwdStack--entry from nestedFrame*/);
                    Frame nestedFrame = GetChildFrame(_islandFrame);
                    Verify(nestedFrame, "PageE.xaml", null, null/*fwdStack--it doesn't own a journal*/);

                    DRT.Assert(!GetForwardButton().IsEnabled, "NavWin's Forward button should remain disabled.");
                    NavWin.GoBack();
                },
                (DrtTest) delegate() 
                {
                    DRT.Assert(NavWin.Title == "Page A", 
                        "If NavWin doesn't return to Page A, journal entries from the child frame that "+
                        "switched to its own journal might not have been removed from NavWin's journal.");
                    DRT.Assert(!GetBackButton(/*NavWin*/).IsEnabled && GetForwardButton().IsEnabled);
                    NavWin.GoForward();
                },
                (DrtTest) delegate() 
                {
                    // Now frame's own journal should be restored. It includes journal entries for the
                    // frame nested in that frame (from PageD).
                    Frame f = GetFrameFromPageWithFrame();
                    DRT.Assert(f != _islandFrame, 
                        "_islandFrame's parent page should have been journaled by URI, not kept alive.");
                    _islandFrame = f;
                    Verify(_islandFrame, "PageD.xaml", "Basic/PageC.xaml", "Basic/PageF.xaml");
                    WireNavigationEvents(_islandFrame);

                    Frame nestedFrame = GetChildFrame(_islandFrame);
                    Verify(nestedFrame, "PageE.xaml", null, null);
                    WireNavigationEvents(nestedFrame);

                    //--------------------------------------------------------------------

                    // Now repeat the journal-journaling test but first navigate the nested frame back
                    // to PageC so that PageD (which has the nested frame, for which the island frame's
                    // journal has an entry (PageF)) is not the current one.

                    // Instead of Frame.GoBack(), try the Back button.
                    ClickButton(GetBackButton(_islandFrame));
                },
                (DrtTest) delegate() 
                {
                    // Note that PageF is not in the forward stack here, because it's for a frame that
                    // is not in the current page.
                    Verify(_islandFrame, "PageC.xaml", "", "Basic/PageD.xaml");
                    NavWin.GoBack();
                },
                (DrtTest) delegate() 
                {
                    DRT.Assert(NavWin.Title == "Page A");
                    NavWin.GoForward();
                },
                (DrtTest) delegate() 
                {
                    DRT.Assert(NavWin.Title == "Page with Frame");
                    Frame f = GetFrameFromPageWithFrame();
                    WireNavigationEvents(f);
                    _islandFrame = f;
                    Verify(_islandFrame, "PageC.xaml", "", "Basic/PageD.xaml");
                    ClickButton(GetForwardButton(_islandFrame));
                },
                (DrtTest) delegate() 
                {
                    // PageF should reappear in the fwd stack, because we're at PageD now, which has the
                    // frame for which this journal entry was created.
                    Verify(_islandFrame, "PageD.xaml", "Basic/PageC.xaml", "Basic/PageF.xaml");
                    Frame nestedFrame = GetChildFrame(_islandFrame);
                    Verify(nestedFrame, "PageE.xaml", null, null);

                    _islandFrame.GoForward(); // Navigate nestedFrame to PageF
                },
                (DrtTest) delegate() 
                {
                    Verify(_islandFrame, "PageD.xaml", "Basic/PageE.xaml Basic/PageC.xaml", "");
                    VerifyNavigationMenus(_islandFrame);

                    Frame nestedFrame = GetChildFrame(_islandFrame);
                    Verify(nestedFrame, "PageF.xaml", null, null);

                    //--------------------------------------------------------------------

                    // Try jumping back & forward using the drop-down navigation menus..
                    JumpBack(_islandFrame, "Basic/PageC.xaml");
                },
                (DrtTest) delegate() 
                {
                    Verify(_islandFrame, "PageC.xaml", "", "Basic/PageD.xaml");
                    JumpForward(_islandFrame, "Basic/PageD.xaml");
                },
                (DrtTest) delegate() 
                {
                    Verify(_islandFrame, "PageD.xaml", "Basic/PageE.xaml Basic/PageC.xaml", "");
                    //--------------------------------------------------------------------
                },
                (DrtTest) delegate() 
                {
                    // Frame's handling of the main NavigationCommands is tested by driving the nav. chrome.
                    // Try BrowseStop here.
                    _islandFrame.GoBack();
                    NavigationCommands.BrowseStop.Execute(null, (IInputElement)_islandFrame.Content);
                },
                (DrtTest) delegate() 
                {
                    Verify(_islandFrame, "PageD.xaml", "Basic/PageE.xaml Basic/PageC.xaml", "");

                    //--------------------------------------------------------------------

                    // Detach the frame from its parent page and change journal ownership while a
                    // navigation is underway

                    Frame nestedFrame = GetChildFrame(_islandFrame);
                    Verify(nestedFrame, "PageF.xaml", null, null); // from way back^^
                    nestedFrame.Navigate(new Uri(@"Basic/PageG.xaml", UriKind.Relative));
                    ((Panel)nestedFrame.Parent).Children.Remove(nestedFrame);
                    DRT.Assert(nestedFrame.JournalOwnership == JournalOwnership.UsesParentJournal);
                    nestedFrame.JournalOwnership = JournalOwnership.Automatic;
                        // Expected to switch to UsesOwnJournal when the navigation completes.
                    _detachedFrame = nestedFrame;
                },
                (DrtTest) delegate() 
                {
                    // Now the detached frame should have completed the navigation and created its own
                    // journal to record the previous page.
                    Verify(_detachedFrame, "PageG.xaml", "Basic/PageF.xaml", "");

                    // PageE should disappear from _islandFrame's back stack because the child frame for
                    // which this journal entry was created is gone.
                    Verify(_islandFrame, "PageD.xaml", "Basic/PageC.xaml", "");

                    //--------------------------------------------------------------------

                    // Finally, try navigation without journaling

                    _detachedFrame.JournalOwnership = JournalOwnership.UsesParentJournal;
                    VerifyGoBackIsInvalid(_detachedFrame, "Frame should abandom its own journal upon switching to UsesParentJournal.");
                    Verify(_detachedFrame, "PageG.xaml", null, null);

                    _detachedFrame.Source = new Uri(@"PageH.xaml", UriKind.Relative);
                },
                (DrtTest) delegate() 
                {
                    Verify(_detachedFrame, "PageH.xaml", null, null);
                    DRT.Assert(_detachedFrame.JournalOwnership == JournalOwnership.UsesParentJournal);
                },
                // "THAT'S ALL, FOLKS."
            };
        }

        Frame _islandFrame, _detachedFrame;

        Frame GetFrameFromPageWithFrame()
        {
            IFPageWithFrame page = NavWin.Content as IFPageWithFrame;
            DRT.Assert(page != null, "NavigationWindow.Content was expected to be PageWithFrame.");
            return page.MyFrame;
        }

        Frame GetChildFrame(Frame outerFrame)
        {
            DRT.Assert(outerFrame != null, "Encountered null Frame reference.");
            DRT.Assert(outerFrame.Content is Page, "The given frame's Content is not a Page.");
            Frame childFrame = (Frame)((Page)outerFrame.Content).FindName("ChildFrame");
            DRT.Assert(childFrame != null, "The given frame does not contain 'ChildFrame'.");
            return childFrame;
        }

        void WireNavigationEvents(Frame f)
        {
            f.Navigating += base.OnNavigating;
            f.LoadCompleted += base.OnLoadCompleted;
        }

        void Verify(Frame f, string currentPageName, string backStack, string forwardStack)
        {
            DRT.Assert(f.Content == null && currentPageName == null
                || f.Source.ToString().EndsWith(currentPageName, StringComparison.OrdinalIgnoreCase));

            DRT.Assert(GetBackStack(f) == backStack,
                "The frame's back stack was expected to be " + backStack + "; got: " + GetBackStack(f));
            DRT.Assert(string.IsNullOrEmpty(backStack) ^ f.CanGoBack);
            DRT.Assert(GetForwardStack(f) == forwardStack,
                "The frame's forward stack was expected to be " + forwardStack + "; got: " + GetForwardStack(f));
            DRT.Assert(string.IsNullOrEmpty(forwardStack) ^ f.CanGoForward);

            DRT.Assert(f.JournalOwnership == JournalOwnership.OwnsJournal ^ f.BackStack == null);

            Button backButton = GetBackButton(f), fwdButton = GetForwardButton(f);
            if (backButton != null && !backButton.IsVisible)
            {
                backButton = null; // pretend it doesn't exist
            }
            if (fwdButton != null && !fwdButton.IsVisible)
            {
                fwdButton = null;
            }
            bool shouldShowNavChrome = f != _detachedFrame /*nav.chrome not visible then*/ &&
                (f.NavigationUIVisibility == NavigationUIVisibility.Visible
                  || f.NavigationUIVisibility == NavigationUIVisibility.Automatic && f.JournalOwnership == JournalOwnership.OwnsJournal);
            DRT.Assert((shouldShowNavChrome ^ backButton == null) && (shouldShowNavChrome ^ fwdButton == null),
                "Frame's navigation buttons were expected to be "+(shouldShowNavChrome ? "visible." : "hidden."));
            if (shouldShowNavChrome)
            {
                DRT.Assert(backButton.IsEnabled == f.CanGoBack, 
                    "Frame's Back navigation button was expected to be "+(f.CanGoBack ? "enabled" : "disabled"));
                DRT.Assert(fwdButton.IsEnabled == f.CanGoForward,
                    "Frame's Forward navigation button was expected to be "+(f.CanGoForward ? "enabled" : "disabled"));
            }            
        }

        void VerifyGoBackIsInvalid(Frame f, string failMsg)
        {
            bool failed = false;
            try { f.GoBack(); }
            catch (InvalidOperationException) { failed = true; }
            DRT.Assert(failed && !f.CanGoBack, failMsg);
        }

        // This test requires actually opening the drop-down menus, which is slowish. That's why it's
        // separate from Verify() and called only occasionally.
        protected void VerifyNavigationMenus(Frame f)
        {
            string menuItems = GetBackMenuStack(f);
            DRT.Assert(menuItems == GetBackStack(f), "The Back navigation menu does not match the back stack. " +
                "Expected: " + GetBackStack(f) + "; got: " + menuItems);

            menuItems = GetForwardMenuStack(f);
            DRT.Assert(menuItems == GetForwardStack(f), "The Forward navigation menu does not match the forward stack. " +
                "Expected: " + GetForwardStack(f) + "; got: " + menuItems);
        }

        Button GetBackButton(Frame f)
        {
            return (Button)DRT.FindVisualByPropertyValue(
                Button.CommandProperty, NavigationCommands.BrowseBack, f, false);
        }
        Button GetForwardButton(Frame f)
        {
            return (Button)DRT.FindVisualByPropertyValue(
                Button.CommandProperty, NavigationCommands.BrowseForward, f, false);
        }

        string GetBackStack(Frame f)
        {
            // Reading the DP value before the CLR property and then asserting equality makes sure 
            // that we don't rely on lazy creation of Journal triggered by the property getter.
            IEnumerable backStack = (IEnumerable)f.GetValue(Frame.BackStackProperty);
            DRT.Assert(backStack == f.BackStack);
            if (backStack == null)
                return null;
            return GetStack(backStack);
        }
        string GetForwardStack(Frame f)
        {
            IEnumerable fwdStack = (IEnumerable)f.GetValue(Frame.ForwardStackProperty);
            DRT.Assert(fwdStack == f.ForwardStack);
            if (fwdStack == null)
                return null;
            return GetStack(fwdStack);
        }

        void JumpBack(Frame f, string entryName)
        {
            Console.WriteLine("Jumping Frame back to '" + entryName + "'");
            Jump(GetJournalEntryMenu(f), f.BackStack, entryName);
        }
        void JumpForward(Frame f, string entryName)
        {
            Console.WriteLine("Jumping Frame forward to '" + entryName + "'");
            Jump(GetJournalEntryMenu(f), f.ForwardStack, entryName);
        }

        void ClickButton(Button b)
        {
            typeof(Button).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(b, null);
        }
    };
}
