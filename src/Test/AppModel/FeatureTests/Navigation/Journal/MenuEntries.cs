// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Test.Logging;
using Microsoft.Test.Globalization;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // MenuEntries

    /// <summary>
    /// BVT that creates custom journal entries (each journal entry
    /// corresponds to a change to a button's background)
    /// Matches the counts of the actual Back / Forward Menus to the 
    /// expected counts (checks to see that only 9 are exposed)
    /// Also matches the counts of the actual Back / Forward stack journal 
    /// entries
    /// </summary>
    public partial class NavigationTests : Application
    {
        enum MenuEntries_CurrentTest
        {
            UnInit,
            Init,
            Nav,
            VerifyGoBack
        };

        #region MenuEntries globals
        NavigationWindow _menuEntries_navWin = null;
        MenuEntries_CurrentTest _menuEntries_currentState = MenuEntries_CurrentTest.UnInit;
        MenuEntriesButtonPage _menuEntries_buttonPage = null;
        Color _menuEntries_c = new Color();
        int _menuEntries_navigatedEvents = 0;
        #endregion

        void MenuEntries_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("MenuEntries");
            if (!BrowserInteropHelper.IsBrowserHosted)
                NavigationHelper.Output("MenuEntries:: Standalone test");

            _menuEntries_currentState = MenuEntries_CurrentTest.Init;
            Application.Current.StartupUri = new Uri("MenuEntriesButtonPage.xaml", UriKind.RelativeOrAbsolute);
            Application.Current.Navigated += new NavigatedEventHandler(MenuEntries_Navigated);
            //Application.Current.LoadCompleted += new LoadCompletedEventHandler(MenuEntries_LoadCompleted);

            NavigationHelper.SetStage(TestStage.Run);
        }


        void MenuEntries_Navigated(object sender, NavigationEventArgs e)
        {
            switch (_menuEntries_currentState)
            {
                case MenuEntries_CurrentTest.Init:
                    _menuEntries_navWin = Application.Current.MainWindow as NavigationWindow;
                    break;

                case MenuEntries_CurrentTest.VerifyGoBack:
                    _menuEntries_navigatedEvents++;
                    break;
            }
        }

        void MenuEntries_ContentRendered(object sender, EventArgs e)
        {
            switch (_menuEntries_currentState)
            {
                case MenuEntries_CurrentTest.Init:
                    _menuEntries_currentState = MenuEntries_CurrentTest.Nav;
                    _menuEntries_c.R = 0;
                    _menuEntries_c.G = 255;
                    _menuEntries_c.B = 0;
                    _menuEntries_c.A = 150;
                    _menuEntries_buttonPage = _menuEntries_navWin.Content as MenuEntriesButtonPage;

                    for (int i = 0; i < 15; i++)
                    {
                        if (_menuEntries_buttonPage.ChangeColor(_menuEntries_c))
                        {
                            _menuEntries_c.B += 15;
                            _menuEntries_c.R += 15;
                            _menuEntries_c.G -= 15;
                        }
                        else
                        {
                            NavigationHelper.Fail("Could not change the button colour");
                        }
                    }

                    NavigationHelper.Output("Changed button color 15 times");
                    MenuEntries_VerifyNumberOfEntries(9, 0, 15, 0);

                    _menuEntries_currentState = MenuEntries_CurrentTest.VerifyGoBack;
                    for (int i = 0; i < 15; i++)
                    {
                        _menuEntries_navWin.GoBack();
                    }

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object ob)
                        {
                            if (_menuEntries_navigatedEvents == 15)
                                NavigationHelper.Output("Went back 15 times");

                            MenuEntries_VerifyNumberOfEntries(0, 9, 0, 15);
                            return null;
                        }, null);
                    break;
            }
        }

        private void MenuEntries_VerifyNumberOfEntries(int expectedBackMenu, int expectedForwardMenu,
                            int expectedBackStack, int expectedForwardStack)
        {
            Menu backMenu = NavigationUtilities.GetBackMenu(_menuEntries_navWin);
            Menu forwardMenu = NavigationUtilities.GetForwardMenu(_menuEntries_navWin);
            // Count the items in the shared journal drop down.
            int actualBackMenu = 0;
            int actualForwardMenu = 0;

            // Standalone (IE7-like) mode: both menus are the same - arbitrarily pick backMenu
            if (!BrowserInteropHelper.IsBrowserHosted)
            {
                MenuItem journal = backMenu.Items[0] as MenuItem;
                if (journal == null)
                    NavigationHelper.Fail("Could not get a hold of the journal drop-down menu");

                // Open the submenu
                journal.IsSubmenuOpen = true;
                ItemContainerGenerator journalICG = journal.ItemContainerGenerator;
                int journalEntryCount = journal.Items.Count;
                bool foundCurrentPage = false;
                string currentPage = "Current Page";
                Assembly presentationFramework = null;
            
                try
                {
#if TESTBUILD_CLR40
                    presentationFramework = Assembly.Load("PresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
#endif
#if TESTBUILD_CLR20
                    presentationFramework = Assembly.Load("PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35");
#endif
                    currentPage = Extract.GetExceptionString("NavWindowMenuCurrentPage", presentationFramework);
                }
                catch (Exception e)
                {
                    NavigationHelper.Output("Couldn't get resource string for NavWindowMenuCurrentPage: " + e.ToString());
                }

                for (int i = 0; i < journalEntryCount && !foundCurrentPage; i++)
                {
                    MenuItem currentEntry = (MenuItem)journalICG.ContainerFromIndex(i);
                    if (currentEntry == null)
                        break;                          // kill loop

                    if ((String)currentEntry.Header == currentPage)
                        foundCurrentPage = true;
                    else
                        actualForwardMenu++;
                }
                actualBackMenu = journalEntryCount - actualForwardMenu - 1;
            }
            else
            {
                actualBackMenu = (backMenu.Items[0] as MenuItem).Items.Count;
                actualForwardMenu = (forwardMenu.Items[0] as MenuItem).Items.Count;
            }

            NavigationHelper.Output("Back menu item count: ACTUAL = " + actualBackMenu + "; EXPECTED = " + expectedBackMenu);
            NavigationHelper.Output("Forward menu item count: ACTUAL = " + actualForwardMenu + "; EXPECTED = " + expectedForwardMenu);

            int actualBackStack = Count(NavigationUtilities.GetBackStack(_menuEntries_navWin));
            int actualForwardStack = Count(NavigationUtilities.GetForwardStack(_menuEntries_navWin));
            NavigationHelper.Output("Back stack item count: ACTUAL = " + actualBackStack + "; EXPECTED = " + expectedBackStack);
            NavigationHelper.Output("Forward stack item count: ACTUAL = " + actualForwardStack + "; EXPECTED = " + expectedForwardStack);

            bool allMatched =
                (actualBackMenu == expectedBackMenu)
                && (actualForwardMenu == expectedForwardMenu)
                && (actualBackStack == expectedBackStack)
                && (actualBackMenu == expectedBackMenu);

            if (allMatched)
            {
                NavigationHelper.Output("Test Passed. All counts match");
                if (_menuEntries_currentState == MenuEntries_CurrentTest.VerifyGoBack)
                    NavigationHelper.Pass("SUCCESS!!!  All tests passed.");
            }
            else
            {
                NavigationHelper.Output("Test failed. All counts don't match");
                if (_menuEntries_currentState == MenuEntries_CurrentTest.VerifyGoBack)
                    NavigationHelper.Fail("Menu entries did not match expected values");
            }
        }

        int Count(IEnumerator enumerator)
        {
            int count = 0;
            while (enumerator.MoveNext())
            {
                ++count;
            }
            return count;
        }
    }
}
