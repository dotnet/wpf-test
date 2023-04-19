// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Refresh NavigationWindow test case
//
//  Step 1 - Navigate to initial page RefreshNavWin_Page1.xaml
//  Step 2 - Create new content and verify it is found when content is rendered
//  Step 3 - Call Refresh() on the NavigationWindow
//  Step 4 - Verify NavigationMode is NavigationMode.Refresh in Navigating event handler
//  Step 5 - Verify that programmatically added content is not preserved after Refresh() is called
//  Step 6 - Verify that the content of the initial page can be found
//  Step 7 - Verify the expected journal entries
//
//

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;                   // TestLog, TestStage
using Microsoft.Windows.Test.Client.AppSec.Navigation;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class RefreshNavWin
    {
        private enum State
        {
            InitialNav, 
            Navigated
        }

        State _curState = State.InitialNav;

        public void Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("RefreshNavWin");
            NavigationHelper.Output("Initializing TestLog...");

            NavigationHelper.SetStage(TestStage.Run);
            Application.Current.StartupUri = new Uri("RefreshNavWin_Page1.xaml", UriKind.RelativeOrAbsolute);

            // expected test counters
            NavigationHelper.NumExpectedNavigatingEvents = 2;
            NavigationHelper.NumExpectedLoadCompletedEvents = 2;
        }

        public void Navigating(object source, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired. State - " + _curState);
            NavigationHelper.NumActualNavigatingEvents++;

            if (_curState == State.Navigated)
            {
                if (e.NavigationMode != NavigationMode.Refresh)
                {
                    NavigationHelper.Fail("Did not find Refreshing NavigationMode");
                }
                else
                {
                    NavigationHelper.Output("Navigation mode is Refreshing");
                }
            }
        }

        public void LoadCompleted(object source, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired. State - " + _curState);
            NavigationHelper.NumActualLoadCompletedEvents++;

            NavigationHelper.Output("uri is: " + e.Uri.ToString());
            NavigationHelper.Output("uri is: " + NavigationHelper.getFileName(e.Uri));

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                // Add a button and create a custom journal entry only for 
                // the first time page is loaded
                if (_curState == State.InitialNav)
                {
                    AddButton(); // Add a new button programmatically

                    // AddBackEntry
                    nw.AddBackEntry(new CustomButtonState("NewButton created"));
                }
            }
            else 
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }

        public void ContentRendered(object sender, EventArgs e)
        {
            NavigationHelper.Output("ContentRendered event fired. State - " + _curState);

            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                TreeUtilities treeUtilities = new TreeUtilities();
                DockPanel docPanel = nw.Content as DockPanel;

                switch (_curState)
                {
                    case State.InitialNav:
                        // verify the NewButton exists and call refresh
                        _curState = State.Navigated;
                        if (treeUtilities.FindVisualTreeElementByID("NewButton", (DependencyObject)docPanel) == null)
                        {
                            NavigationHelper.Fail("Couldn't find the NewButton in visual tree");
                        }
                        else
                        {
                            NavigationHelper.Output("Found NewButton");

                            // call Refresh
                            NavigationHelper.Output("Calling Refresh()");
                            nw.Refresh();
                        }
                        break;

                    case State.Navigated:
                        // we are here after Refresh is called
                        // programmatically added content should not be preserved
                        // we should not see the NewButton that is added during previous load
                        if (treeUtilities.FindVisualTreeElementByID("NewButton", (DependencyObject)docPanel) == null)
                        {
                            NavigationHelper.Output("Did not find the NewButton in visual tree as expected");

                            // make sure the content of the initial page can be found
                            if (treeUtilities.FindVisualTreeElementByID("Text1", (DependencyObject)docPanel) == null)
                            {
                                NavigationHelper.Fail("Couldn't find the Text1 in visual tree");
                            }
                            else
                            {
                                NavigationHelper.Output("Found Text1 in visual tree");

                                // verify the back stack
                                JournalHelper journalHelper = new JournalHelper();
                                String[] backStack = journalHelper.GetBackMenuItems();

                                // Uncomment the following lines if 194346 gets fixed
                                /*
                                if (backStack != null)
                                {
                                    for (int i = 0; i < backStack.Length; i++)
                                    {
                                        NavigationHelper.Output("Back Stack item found = " + backStack[i]);
                                    }
                                    NavigationHelper.Fail("Unexpected journal items found");
                                }*/

                                
                                if (backStack != null)
                                {
                                    for (int i = 0; i < backStack.Length; i++)
                                    {
                                        NavigationHelper.Output("Back Stack item found = " + backStack[i]);
                                    }

                                    if (backStack.Length > 1)
                                    {
                                        NavigationHelper.Fail("More than one back stack items found");
                                    }
                                }

                                // we found the expected content. finish the test
                                NavigationHelper.FinishTest(true);
                            }
                        }
                        else
                        {
                            NavigationHelper.Fail("NewButton is found after refresh");
                        }
                        break;
                }
            }
            else
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }

        private void AddButton()
        {
            NavigationWindow nw = Application.Current.MainWindow as NavigationWindow;
            if (nw != null)
            {
                DockPanel docPanel = nw.Content as DockPanel;
                Button newButton = new Button();
                newButton.Width = 100;
                newButton.Height = 50;
                newButton.Name = "NewButton";
                newButton.Content = "New Button";
                docPanel.Children.Add(newButton);
            }
            else
            {
                NavigationHelper.Fail("Could not get NavigationWindow");
            }
        }
    }

    // This is a dummy CustomContentState
    [Serializable]
    public class CustomButtonState : CustomContentState
    {
        private String _name;

        public CustomButtonState(String myName)
        {
            _name = myName;
        }

        public override string JournalEntryName
        {
            get
            {
                return _name;
            }
        }

        // helps with visual verification
        public override void Replay(NavigationService navigationService, NavigationMode mode)
        {
            DockPanel docPanel = navigationService.Content as DockPanel;
            TreeUtilities treeUtilities = new TreeUtilities();
            Button button = (Button) treeUtilities.FindVisualTreeElementByID("NewButton", (DependencyObject)docPanel);
            button.Background = Brushes.DeepSkyBlue;
        }
    }
}

