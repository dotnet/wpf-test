// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

using DRT;

namespace DrtFrameJournaling
{
    public class DrtFrameJournalBase : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtFrameJournalBase drt = new DrtFrameJournalBase();

            return drt.Run(args);
        }

        private DrtFrameJournalBase()
        {
            this.TeamContact = "AppModel";
            this.Contact = "Microsoft";
            this.Suites = new DrtTestSuite[] {
                new BasicJournalNavigationSuite(),
            };
        }
    }

    /// <summary>
    /// Base class for Journal suites
    /// </summary>
    public abstract class JournalTestSuite : DrtTestSuite
    {
        protected JournalTestSuite(string name)
            : base(name)
        {
        }

        protected NavigationWindow NavWin
        {
//            get { return DRT.RootElement as NavigationWindow; }
            get { return DRT.Application.MainWindow as NavigationWindow; }
        }

        protected void OnLoadCompleted(object source, NavigationEventArgs e)
        {
            DrtBase._Logger.Log("OnLoadCompleted: " + e.Uri.ToString());
            DRT.Resume();
//            DrtBase._Logger.Log("*** Source:");
//            this.DumpVisualTree((Visual)source, 0);
//            DrtBase._Logger.Log("*** NavWin:");
//            this.DumpVisualTree((Visual)this.NavWin, 0);
//            this.DumpLogicalTree(this.NavWin, 0);
        }

//        private void DumpVisualTree(Visual element, int indentLevel)
//        {
//            if (indentLevel == 0)
//                DrtBase._Logger.Log("-- Logical tree");
//
//            for (int indent = 0; indent < indentLevel; ++indent)
//                Console.Write("  ");
//
//            UIElement ui = element as UIElement;
//
//            if (ui != null)
//            {
//                System.Diagnostics.Debug.Assert(ui.Visibility == Visibility.Visible);
//            }
//
//            Pane pane = element as Pane;
//
//            if ((pane != null) && (pane is _Page6))
//            {
//                Frame frame = pane.Parent as Frame;
//
//                if (frame != null)
//                {
//                    frame.Width = new Length(300);
//                    frame.Height = new Length(300);
//                }
//            }
//
//            DrtBase._Logger.Log(element.GetType().ToString());
//            foreach (Visual child in ((Visual)element)).Children((Visual)element))
//            {
//                DumpVisualTree(child, indentLevel + 1);
//            }
//        }
//
//        private void DumpLogicalTree(object element, int indentLevel)
//        {
//            if (indentLevel == 0)
//                DrtBase._Logger.Log("-- Logical tree");
//
//            for (int indent = 0; indent < indentLevel; ++indent)
//                Console.Write("  ");
//
//            DrtBase._Logger.Log(element.GetType().ToString());
//
//            DependencyObject node = element as DependencyObject;
//
//            if (node != null)
//            {
//                IEnumerator children = LogicalTreeHelper.GetChildren(node).GetEnumerator();
//
//                if (children != null)
//                {
//                    while (children.MoveNext())
//                    {
//                        DumpLogicalTree(children.Current, indentLevel + 1);
//                    }
//                }
//
//                Frame frame = node as Frame;
//
//                if ((frame != null) && (frame.Content != null))
//                {
//                    DumpLogicalTree(frame.Content, indentLevel + 1);
//                }
//            }
//        }
    }


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
            : base("Frame Navigation")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]
            {
                (DrtTest) delegate()
                {
                    // Perform the initial navigation to PageA.xaml
                    DRT.Application.LoadCompleted += new LoadCompletedEventHandler(this.OnLoadCompleted);
                    this.NavigateTo('A');
                    DRT.Suspend(3); // One for page
                },
                (DrtTest) delegate() { this.Verify("A(C(G())E())", "", "", "After initial navigation to A"); },
                (DrtTest) delegate() { this.ClickHyperLink('H'); },
                (DrtTest) delegate() { this.Verify("A(C(H())E())", "G", "", "After navigation to H"); },
                (DrtTest) delegate() { this.ClickHyperLink('D'); },
                (DrtTest) delegate() { this.Verify("A(D()E())", "C", "", "After navigation to D"); },
                (DrtTest) delegate() { this.GoBack(); DRT.Suspend(1);},
                (DrtTest) delegate() { this.Verify("A(C(H())E())", "G", "D", "After going back to H"); },
                (DrtTest) delegate() { this.ClickHyperLink('B'); },
                (DrtTest) delegate() { this.Verify("B()", "A", "", "After navigating to B");},
                (DrtTest) delegate() { this.GoBack(); DRT.Suspend(3);},
                (DrtTest) delegate() { this.Verify("A(C(H())E())", "G", "B", "After going back to A"); },
                (DrtTest) delegate() { this.ClickHyperLink('G'); },
                (DrtTest) delegate() { this.Verify("A(C(G())E())", "HG", "", "After navigation to G"); },
                (DrtTest) delegate() { this.ClickHyperLink('H'); },
                (DrtTest) delegate() { this.Verify("A(C(H())E())", "GHG", "", "After navigation to H"); },
                (DrtTest) delegate() { this.ClickHyperLink('D'); },
                (DrtTest) delegate() { this.Verify("A(D()E())", "C", "", "After navigation to D (non-navigable entries should be gone)"); },
                (DrtTest) delegate() { this.GoBack(); DRT.Suspend(1);},
                (DrtTest) delegate() { this.Verify("A(C(H())E())", "GHG", "D", "After going back to C, entries now navigable should be back"); },
                (DrtTest) delegate() { this.GoForward();},
                (DrtTest) delegate() { this.Verify("A(D()E())", "C", "", "After going forward to D non-navigable entries should be gone"); },
                (DrtTest) delegate() { this.GoBack(); DRT.Suspend(1);},
                (DrtTest) delegate() { this.Verify("A(C(H())E())", "GHG", "D", "After going back to C, entries now navigable should be back (2)"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify("A(C(G())E())", "HG", "HD", "After going back to G"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify("A(C(H())E())", "G", "GHD", "After going back to H"); },
                (DrtTest) delegate() { this.GoBack(); },
                (DrtTest) delegate() { this.Verify("A(C(G())E())", "", "HGHD", "After going back to G again"); },
            };
        }

        private void NavigateTo(char page)
        {
            string partialPath = "Page" + page + ".xaml";
            DrtBase._Logger.Log("Navigating to " + partialPath);
            this.NavWin.Navigate(new Uri(partialPath, UriKind.RelativeOrAbsolute));
            DRT.Suspend();
        }

        private void GoBack()
        {
            // Go back by invoking the NavigationWindow's back button
            DrtBase._Logger.Log("Clicking the Back button");
            DoClick(this.GetBackButton());
            DRT.Suspend();
        }

        private void GoForward()
        {
            // Go forward by invoking the NavigationWindow's forward button
            DrtBase._Logger.Log("Clicking the Forward button");
            DoClick(this.GetForwardButton());
            DRT.Suspend();
        }

        private Button GetBackButton()
        {
            return (Button)DRT.FindVisualByPropertyValue(Button.CommandProperty, NavigationCommands.BrowseBack);
        }

        private Button GetForwardButton()
        {
            return (Button)DRT.FindVisualByPropertyValue(Button.CommandProperty, NavigationCommands.BrowseForward);
        }

        private void JumpBack(char page)
        {
            DrtBase._Logger.Log("Jumping back to Page" + page + ".xaml");
            Jump("BackButtonMenu", NavigationWindow.BackStackProperty, page);
        }

        private void JumpForward(char page)
        {
            DrtBase._Logger.Log("Jumping forward to Page" + page + ".xaml");
            Jump("FwdButtonMenu", NavigationWindow.ForwardStackProperty, page);
        }

        private void Jump(string menuId, DependencyProperty dp, char page)
        {
            Menu menu = (Menu)DRT.FindVisualByID(menuId);
            MenuItem menuItem = (MenuItem)menu.Items[0];

            menuItem.IsSubmenuOpen = true;
            Thread.Sleep(100);

            JournalEntry entry = this.GetEntry((IEnumerable)this.NavWin.GetValue(dp), page);
            MenuItem navItem = (MenuItem)menuItem.ItemContainerGenerator.ContainerFromItem(entry);

#if OLD_AUTOMATION
            ((IInvokeProvider)navItem).Invoke();
#else
            MethodInfo info = typeof(MenuItem).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            info.Invoke(navItem, new object[] { });
#endif
            DRT.Suspend();
        }

        private void Verify(string tree, string backStack, string forwardStack, string text)
        {
            // Pump the dispatcher to allow CommandManager to update the enabled state on buttons
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, (DispatcherOperationCallback)delegate(object arg) { return null; }, null);

            DrtBase._Logger.Log("Verifying.");

            // Test that the correct page is being displayed
            DRT.AssertEqual(tree, this.GetNavTreeState(), text + " (Content)");

            // Test the contents of the back stack
            DRT.AssertEqual(backStack, this.GetBackStack(), text + " (BackStack)");

            // Test the contents of the back button menu
            DRT.AssertEqual(backStack, this.GetBackMenu(), text + " (Back Menu)");

            // Test the contents of the forward stack
            DRT.AssertEqual(forwardStack, this.GetForwardStack(), text + " (ForwardStack)");

            // Test the contents of the forward menu
            DRT.AssertEqual(forwardStack, this.GetForwardMenu(), text + " (Forward Menu)");

            // Test the state of NavigationWindow.CanGoBack
            DRT.AssertEqual(backStack.Length != 0, this.NavWin.CanGoBack, text + " (NavigationWindow.CanGoBack)");

            // Test the IsEnabled property of the back button
            DRT.AssertEqual(backStack.Length != 0, this.GetBackButton().IsEnabled, text + " (Back button IsEnabled)");

            // Test the state of NavigationWindow.CanGoForward
            DRT.AssertEqual(forwardStack.Length != 0, this.NavWin.CanGoForward, text + " (NavigationWindow.CanGoForward)");

            // Test the IsEnabled property of the forward button
            DRT.AssertEqual(forwardStack.Length != 0, this.GetForwardButton().IsEnabled, text + " (Forward button IsEnabled)");
        }

        private string GetBackStack()
        {
            return GetStack((IEnumerable)this.NavWin.GetValue(NavigationWindow.BackStackProperty));
        }

        private string GetForwardStack()
        {
            return GetStack((IEnumerable)this.NavWin.GetValue(NavigationWindow.ForwardStackProperty));
        }

        private string GetStack(IEnumerable entries)
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
            Menu menu = (Menu)DRT.FindVisualByType(typeof(Menu), NavWin);
            return this.GetMenuStack((MenuItem)menu.Items[0], true);
        }

        private string GetForwardMenu()
        {
            Menu menu = (Menu)DRT.FindVisualByType(typeof(Menu), NavWin);
            return this.GetMenuStack((MenuItem)menu.Items[0], false);
        }

        protected string GetMenuStack(MenuItem journalEntryMenu, bool back)
        {
            // The submenu has to be opened in order for the ItemContainerGenerator to start working.
            journalEntryMenu.IsSubmenuOpen = true;

            ItemContainerGenerator icg = journalEntryMenu.ItemContainerGenerator;
            StringBuilder stack = new StringBuilder();

            bool passedCurrent = false;
            int start = back ? 0 : journalEntryMenu.Items.Count - 1;
            int end = back ? journalEntryMenu.Items.Count : -1;
            for (int i = start; i != end; i += back ? 1 : -1)
            {
                MenuItem journalEntryMenuItem = (MenuItem)icg.ContainerFromIndex(i);
                if (journalEntryMenuItem == null)
                    break; // no more

                if ((string)journalEntryMenuItem.Header == "Current Page")
                {
                    passedCurrent = true;
                }
                else if (passedCurrent)
                {
                    stack.Append(GetName((string)journalEntryMenuItem.Header));
                }
            }

            journalEntryMenu.IsSubmenuOpen = false;

            return stack.ToString();
        }

        private char GetName(string entryName)
        {

/* This code checks against the title of journal entry - there's a consistency
   problem in that the title may not accurately correspond to the URI of the journal entry.
            int index = entryName.IndexOf("Page");

            if (index == -1)
                return '?';

            if (entryName[index + 4] != ' ')
                return entryName[index + 4];
            else
                return entryName[index + 5];
*/

            // As a workaround in this specific DRT (whose title always matches
            //  the markup file: "Page A" == PageA.xaml) look at the name of the XAML.
            int index = entryName.IndexOf(".xaml");

            if( index == -1 )
                return '?';

            return entryName[ index - 1 ];

        }

        private JournalEntry GetEntry(IEnumerable entries, char name)
        {
            IEnumerator entryEnumerator = entries.GetEnumerator();
            while (entryEnumerator.MoveNext())
            {
                JournalEntry entry = (JournalEntry)entryEnumerator.Current;
                if (name == this.GetName(entry.Name))
                    return entry;
            }

            return null;
        }

        private string GetNavTreeState()
        {
            string state = this.GetName(this.NavWin.Source.ToString()).ToString();
            state += "(";
            state += (this.GetNavTreeState((Visual)this.NavWin));
            return (state += ")");
        }

        private string GetNavTreeState(DependencyObject element)
        {
            string state = string.Empty;

            FrameworkElement fe = element as FrameworkElement;
            if (fe != null)
            {
                fe.ApplyTemplate();
            }
            Frame navigator = element as Frame;

            if (navigator != null)
            {
                // We don't account for this frame since it exists for just one specific test.
                if (navigator.Name == "EmptyFrame")
                    navigator = null;
                else
                {
                    state += this.GetName(navigator.Source.ToString()).ToString();
                    state += "(";
                }
            }

            int count = VisualTreeHelper.GetChildrenCount(element);
            for(int i = 0; i < count; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element,i);
                state += GetNavTreeState(child);
            }

            if (navigator != null)
            {
                state += ")";
            }

            return state;
        }

        void ClickHyperLink(char page)
        {
            DrtBase._Logger.Log("Clicking Hyperlink for Page" + page + " .xaml");
            Hyperlink hyperlink = (Hyperlink)DRT.FindElementByPropertyValue(Hyperlink.NavigateUriProperty, new Uri("Page" + page + ".xaml", UriKind.RelativeOrAbsolute));
            MethodInfo info = typeof(Hyperlink).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            info.Invoke(hyperlink, new object[] {});
            DRT.Suspend();
        }

        /// <summary>
        /// Helper. Invokes Click on a given uielement. 
        /// </summary>
        private void DoClick(UIElement uielement)
        {
            if (uielement != null)
            {
                AutomationPeer ap = UIElementAutomationPeer.CreatePeerForElement(uielement);
                if (ap != null)
                {
                    IInvokeProvider iip = (IInvokeProvider)ap.GetPattern(PatternInterface.Invoke);
                    if (iip != null)
                    {
                        iip.Invoke();
                    }
                }
            }
        }
    }
}

