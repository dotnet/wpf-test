// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Interop;
using System.Windows.Input;
using System.Windows.Automation;
using System.Windows.Automation.Provider;

using DRT;

namespace DrtJournal
{
    public class DrtJournalBase : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtJournalBase drt = new DrtJournalBase();
            return drt.Run(args);
        }

        private DrtJournalBase()
        {
            this.DrtName = "DrtJournal";
            this.TeamContact = "WPF";
            this.Contact = "Microsoft";
            this.Suites = new DrtTestSuite[] {
                new BasicJournalNavigationSuite(),
                new FragmentJournalingSuite(),
                new CustomJournalingSuite(),
                new IslandFrameSuite()
            };
        }

        public void UseNewNavigationWindow()
        {
            NavigationWindow navigationWindow = new NavigationWindow();

            navigationWindow.Visibility = Visibility.Visible;
            navigationWindow.Title = "NavigationWindow";

            PropertyInfo info = navigationWindow.GetType().GetProperty("HwndSourceWindow", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
            object obj = info.GetValue(navigationWindow, null);

            this.MainWindow = (HwndSource)obj;
            this.RootElement = navigationWindow;
        }
    }

    /// <summary>
    /// Base class for Journal suites
    /// </summary>
    public abstract class JournalTestSuite : DrtTestSuite
    {
        protected bool _cancelNextNavigation = false;
        protected bool _throwOnNextNavigation = false;

        protected JournalTestSuite(string name)
            : base(name)
        {
            this.TeamContact = "WPF";
            //this.Contact set by each suite.
        }

        protected NavigationWindow NavWin
        {
            get { return DRT.RootElement as NavigationWindow; }
        }

        protected void WireNavigationEvents()
        {
            NavWin.Navigating += this.OnNavigating;
            NavWin.NavigationStopped += this.OnNavigationStopped;
            NavWin.LoadCompleted += this.OnLoadCompleted;
        }

        protected void OnLoadCompleted(object source, NavigationEventArgs e)
        {
            string uri = e.Uri == null ? "null" : e.Uri.ToString();
            Console.WriteLine("OnLoadCompleted: " + uri);
            DRT.Resume();
        }

        protected void OnNavigationStopped(object source, NavigationEventArgs e)
        {
            Console.WriteLine("NavigationStopped: " + e.Uri.ToString());
            DRT.Resume();
        }

        protected void OnNavigating(object source, NavigatingCancelEventArgs e)
        {
            Console.WriteLine("Navigating to " + (e.Uri != null ? e.Uri : e.Content));
            if (_cancelNextNavigation)
            {
                e.Cancel = true;
                _cancelNextNavigation = false;
            }
            if (_throwOnNextNavigation)
            {
                _throwOnNextNavigation = false;
                throw new Exception("This represents user code throwing an exception in this event handler.  This is equivalent to canceling the navigation and the journal should not be broken.");
            }
        }

        protected Button GetBackButton()
        {
            return (Button)DRT.FindVisualByPropertyValue(Button.CommandProperty, NavigationCommands.BrowseBack);
        }
        protected Button GetForwardButton()
        {
            return (Button)DRT.FindVisualByPropertyValue(Button.CommandProperty, NavigationCommands.BrowseForward);
        }

        protected MenuItem GetJournalEntryMenu()
        {
            return GetJournalEntryMenu(this.NavWin);
        }

        protected MenuItem GetJournalEntryMenu(FrameworkElement element)
        {
            DRT.Assert(element is Frame || element is NavigationWindow);

            Menu menu = (Menu)DRT.FindVisualByType(typeof(Menu), element);
            DRT.Assert(menu != null, "Drop-down menu in navigation window not found.");
            DRT.Assert(menu.Items.Count == 1, 
                "Menu in navigation window/frame should have only one MenuItem, which contains the journal entry items.");
            return (MenuItem)menu.Items[0];
        }

        protected string GetBackStack()
        {
            return GetStack((IEnumerable)this.NavWin.GetValue(NavigationWindow.BackStackProperty));
        }
        protected string GetForwardStack()
        {
            return GetStack((IEnumerable)this.NavWin.GetValue(NavigationWindow.ForwardStackProperty));
        }
        protected virtual string GetStack(IEnumerable entries)
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

        protected string GetBackMenuStack(FrameworkElement element)
        {
            return GetMenuStack(GetJournalEntryMenu(element), true);
        }

        protected string GetForwardMenuStack(FrameworkElement element)
        {
            return GetMenuStack(GetJournalEntryMenu(element), false);
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
            for (int i = start ; i != end; i += back ? 1 : -1)
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
                    if (stack.Length > 0)
                    {
                        stack.Append(' ');
                    }
                    stack.Append((string)journalEntryMenuItem.Header);
                }
            }

            journalEntryMenu.IsSubmenuOpen = false;

            return stack.ToString();
        }

        protected void JumpBack(string entryName)
        {
            Console.WriteLine("Jumping back to '"+entryName+"'");
            Jump(this.GetJournalEntryMenu(), NavigationWindow.BackStackProperty, entryName);
        }
        protected void JumpForward(string entryName)
        {
            Console.WriteLine("Jumping forward to '" + entryName + "'");
            Jump(this.GetJournalEntryMenu(), NavigationWindow.ForwardStackProperty, entryName);
        }
        private void Jump(MenuItem journalEntryMenu, DependencyProperty dp, string entryName)
        {
            Jump(journalEntryMenu, (IEnumerable)this.NavWin.GetValue(dp), entryName);
        }
        protected void Jump(MenuItem journalEntryMenu, IEnumerable navStack, string entryName)
        {
            journalEntryMenu.IsSubmenuOpen = true;
            Thread.Sleep(100);

            JournalEntry entry = this.GetEntry(navStack, entryName);
            if (entry == null)
            {
                DRT.Fail("The " + navStack.GetType().Name + " does not contain entry '" + entryName + "'.");
            }
            MenuItem navItem = (MenuItem)journalEntryMenu.ItemContainerGenerator.ContainerFromItem(entry);
            DRT.Assert(navItem.IsEnabled);

#if OLD_AUTOMATION
            ((IInvokeProvider)navItem).Invoke();
#else
            MethodInfo info = typeof(MenuItem).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            info.Invoke(navItem, new object[] { });
#endif
            DRT.Suspend();
        }

        protected JournalEntry GetEntry(IEnumerable entries, string entryName)
        {
            IEnumerator entryEnumerator = entries.GetEnumerator();
            while (entryEnumerator.MoveNext())
            {
                JournalEntry entry = (JournalEntry)entryEnumerator.Current;
                if (entryName == entry.Name)
                    return entry;
            }

            return null;
        }
    };

}
