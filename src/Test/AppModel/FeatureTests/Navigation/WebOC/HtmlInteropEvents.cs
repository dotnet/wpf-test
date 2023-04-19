// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class tests WebBrowser.Navigating, Navigated, and LoadCompleted events.
    /// This runs in partial trust and full trust.
    /// </summary>
    public class HtmlInteropEvents
    {

        #region Private Data
        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        bool _navigatingFired = false;
        bool _navigatedFired = false;
        bool _loadCompletedFired = false;
        bool _oneOrMoreTestsFailed = false;
        bool _alreadyLooping = false;
        DispatcherTimer _timer = null;

        NavigationStage _currentStage = NavigationStage.FirstNavigation;

        private enum NavigationStage
        {
            FirstNavigation,
            NavigateFromNavigating,
            CanceledNavigation
        }
        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop tests");
            }

            //Local machine lockdown disables scripting inside IE.  Re-enable it for testing purposes.
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 0, RegistryValueKind.DWord);

            NavigationHelper.SetStage(TestStage.Run);
            Application.Current.StartupUri = new Uri("HtmlInterop_Page1.xaml", UriKind.RelativeOrAbsolute);
        }

        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_browser != null)
            {
                return;
            }
            else
            {
                _browser = new WebBrowser();
                if (e.Navigator is Frame)
                {
                    Log.Current.CurrentVariation.LogMessage("Frame Navigation to " + e.Uri);
                    (e.Navigator as Frame).Content = _stackPanel;
                }
                else if (e.Navigator is NavigationWindow)
                {
                    Log.Current.CurrentVariation.LogMessage("NavWin Navigation to " + e.Uri);
                    (e.Navigator as NavigationWindow).Content = _stackPanel;
                }

                _stackPanel.Children.Add(_browser);

	        _browser.LoadCompleted += OnHtmlLoadCompleted;
                _browser.Navigating += OnHtmlNavigating;
                _browser.Navigated += OnHtmlNavigated;

                NavigationHelper.Output("Starting FirstNavigation test");
                _browser.Source = new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage1.htm", UriKind.RelativeOrAbsolute);
            }
        }

        public void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Fail("Unexpected exception caught. Test fails");
            e.Handled = true;
            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
        }
        #endregion

        #region Private Members
        private void OnHtmlNavigating(object sender, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            switch(_currentStage)
            {
                case NavigationStage.FirstNavigation:
                    _navigatingFired = true;
                break;

                case NavigationStage.NavigateFromNavigating:
                    //alreadyLooping is used to keep us from recursing forever.  We only want to nest 1 navigation.
                    if (!_alreadyLooping)
                    {
                        _alreadyLooping = true;
                        NavigationHelper.Output("Calling Navigate inside Navigating event");
                        _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage1.htm", UriKind.RelativeOrAbsolute));
                    }
                break;

                case NavigationStage.CanceledNavigation:
                    e.Cancel = true;
                break;
            }
        }

        private void OnHtmlNavigated(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("Navigated event fired");
            switch(_currentStage)
            {
                case NavigationStage.FirstNavigation:
                    _navigatedFired = true;
                break;

                case NavigationStage.NavigateFromNavigating:
                    //don't need to do anything here
                break;

                case NavigationStage.CanceledNavigation:
                    NavigationHelper.Output("This event should not have fired since we canceled navigation.");
                    _oneOrMoreTestsFailed = true;
                break;
            }
        }

        private void OnHtmlLoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            switch(_currentStage)
            {
                case NavigationStage.FirstNavigation:
                    _loadCompletedFired = true;
                    
                    if (_navigatingFired && _navigatedFired && _loadCompletedFired)
                    {
                        NavigationHelper.Output("Correctly received all navigation events.");
                    }
                    else
                    {
                        NavigationHelper.Output("One or more navigation events did not fire.");
                        _oneOrMoreTestsFailed = true;
                    }

                    NavigationHelper.Output("Starting NavigateFromNavigating test");
                    _currentStage = NavigationStage.NavigateFromNavigating;
                    _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage2.htm", UriKind.RelativeOrAbsolute));
                break;

                case NavigationStage.NavigateFromNavigating:
                    object returnValue = _browser.InvokeScript("SimpleTest");
                    if (returnValue.ToString() == "42")
                    {
                        NavigationHelper.Output("Navigate passed - Navigation went to page 1");
                    }
                    else
                    {
                        NavigationHelper.Output("Navigate failed - Navigation did not go to page 1");
                        _oneOrMoreTestsFailed = true;
                    }

                    NavigationHelper.Output("Starting CanceledNavigation test");
                    _currentStage = NavigationStage.CanceledNavigation;

                    //since we're canceling navigation, we want a timer to fire after a while so we can complete the test case.
                    _timer = new DispatcherTimer(DispatcherPriority.Normal);
                    _timer.Tick += new EventHandler(OnTick);
                    _timer.Interval = TimeSpan.FromMilliseconds(5000);
                    _timer.Start();

                    _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage2.htm", UriKind.RelativeOrAbsolute));
                break;

                case NavigationStage.CanceledNavigation:
                    NavigationHelper.Output("This event should not have fired since we canceled navigation.");
                    _oneOrMoreTestsFailed = true;
                break;
            }
        }
         
        private void OnTick(object sender, EventArgs e)
        {   
            _timer.Stop();
            NavigationHelper.Output("Second navigation was properly canceled.");

            //put local machine lockdown script setting back to disable
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

            //increase code coverage
            _browser.LoadCompleted -= OnHtmlLoadCompleted;
            _browser.Navigating -= OnHtmlNavigating;
            _browser.Navigated -= OnHtmlNavigated;
            NavigationHelper.Output("Removed LoadCompleted, Navigating, and Navigated event handlers");

            if (_oneOrMoreTestsFailed)
            {
                NavigationHelper.Fail("One or more HTML interop event tests failed");
            }
            else
            {
                NavigationHelper.Pass("HTML interop event tests passed");
            }
        }
        #endregion

    }
}
