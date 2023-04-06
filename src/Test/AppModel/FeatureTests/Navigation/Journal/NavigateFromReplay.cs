// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Collections.Generic;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public partial class NavigationTests : Application
    {
        void NavigateFromCCSReplay_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("Navigate within CustomContentState Replay() Method");
            NavigationWindow navWin = new NavigationWindow();
            navWin.Content = new NavigateFromReplay1();
            Application.Current.Navigating += new NavigatingCancelEventHandler(NavigateFromReplay1.NavigatingHandler);
            Application.Current.Navigated += new NavigatedEventHandler(NavigateFromReplay1.NavigatedHandler);
            NavigationHelper.SetStage(Microsoft.Test.Logging.TestStage.Run);
            navWin.Show();
        }
    }

    public class NavigateFromReplay1 : Page, IProvideCustomContentState
    {
        // Keep track of the event arguments that are hit to make sure the expected 
        private static List<NavigatingCancelEventArgs> s_navigating = new List<NavigatingCancelEventArgs>();
        private static List<NavigationEventArgs> s_navigated = new List<NavigationEventArgs>();

        // Since using Loaded event handlers, use a bool to only handle navigation away once.
        private bool _haveNavigatedAway = false;

        public static void NavigatingHandler(object sender, NavigatingCancelEventArgs e)
        {
            s_navigating.Add(e);
            if (e.Uri != null)
            {
                NavigationHelper.Output("Navigating to URI: " + e.Uri.ToString());
            }
            else
            {
                NavigationHelper.Output("Navigating to Content: " + e.Content.GetType().ToString());
            }
        }

        public static void NavigatedHandler(object sender, NavigationEventArgs e)
        {
            s_navigated.Add(e);
            if (e.Uri != null)
            {
                NavigationHelper.Output("Navigated: " + e.Uri.ToString());

                // If we've navigated to PoniesPage, it's because the navigation from Replay() succeeded.
                if (e.Uri.ToString() == "Ponies_Page1.xaml")
                {
                    // Also make sure the penultimate navigation here was back to the CCS page
                    if (s_navigating[s_navigating.Count - 2].Content.GetType() == typeof(NavigateFromReplay1))
                    {
                        // This means we started navigating to the NavFromReplay1, but got redirected to ponies page.
                        NavigationHelper.PassTest("Navigation from Replay method of CCS control succeeded!");
                    }
                    else
                    {
                        NavigationHelper.Fail("Navigation redirected but did not see expected events fired for \"Navigate from Replay\" scenario");
                    }
                }
            } 
            else
            {
                NavigationHelper.Output("Navigated to Content: " + e.Content.GetType().ToString());
            }
        }

        public NavigateFromReplay1()
        {
            this.Loaded += new RoutedEventHandler(NavigateFromReplay1_Loaded);
        }

        void NavigateFromReplay1_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_haveNavigatedAway)
            {
                NavigationHelper.Output("In CCS page, navigating away to second page.");
                _haveNavigatedAway = true;
                this.NavigationService.Navigate(new NavigateFromReplay2());
            }
        }

        CustomContentState IProvideCustomContentState.GetContentState()
        {
            NavigationHelper.Output("GetContentState() called...");
            return new CustomPageContentState("some state info");
        }
    }
 
    // Page class that just navigates back, used to initiate CCS Replay().
    public class NavigateFromReplay2 : Page
    {
        public NavigateFromReplay2()
        {
            Grid grid = new Grid();
            grid.Background = Brushes.Yellow;
            grid.Loaded += new RoutedEventHandler(NavigateBackToReplayState);
            this.Content = grid;            
        }

        void NavigateBackToReplayState(object sender, RoutedEventArgs e)
        {
            NavigationHelper.Output("In second page, navigating back (will initiate replay)");
            this.NavigationService.GoBack();
        }
    }

    [Serializable]
    public class CustomPageContentState : CustomContentState
    {
        string _stateVariable = "";

        public CustomPageContentState(string state)
        {
            this._stateVariable = state;
        }

        public override void Replay(NavigationService navigationService, NavigationMode mode)
        {
            NavigationHelper.Output("Inside the CCS replay method, navigating to different page...");
            navigationService.Navigate(new Uri("Ponies_Page1.xaml", UriKind.Relative));
        }
    }

}
