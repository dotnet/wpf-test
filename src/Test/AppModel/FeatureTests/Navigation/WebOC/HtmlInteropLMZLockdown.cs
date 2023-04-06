// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Verify that FEATURE_LOCALMACHINE_LOCKDOWN is on for the WebOC.  Perform URLACTION_SCRIPT_RUN by loading an html page
    /// with script on it, and use UIAutomation to find the gold bar that IE shows.
    /// </summary>
    public class HtmlInteropLMZLockdown
    {
        #region Private Data

        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();

        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop LMZ lockdown test");
            }

            NavigationHelper.SetStage(TestStage.Run);

            if (Environment.OSVersion.Version.Major < 6)
            {
                NavigationHelper.Pass("The BrowserHelper code we'll use later has a bug on XP, so skip this test on pre-Vista.");
            }
            else
            {
                Application.Current.StartupUri = new Uri("HtmlInterop_Page1.xaml", UriKind.RelativeOrAbsolute);
            }
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

                _browser.Source = new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage1.htm", UriKind.RelativeOrAbsolute);
            }
        }

        public void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Fail("Unexpected exception caught. Test fails");
            e.Handled = true;
            ApplicationMonitor.NotifyStopMonitoring();
        }
        #endregion

        #region Private Members
        private void OnHtmlLoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("In WebBrowser.LoadCompleted");
            try
            {
                BrowserHelper helper = BrowserHelper.GetMainBrowserWindow(Application.Current);

                Thread thread = new Thread(GetGoldBarPresent);
                thread.Start(helper);
            }
            catch(Exception exception)
            {
                NavigationHelper.Output("exception: " + exception.ToString());
            }
        }

        /// Find out if IE's gold bar is present.  We call UIAutomation on a second thread to avoid deadlock.
        private void GetGoldBarPresent(object helper)
        {
            NavigationHelper.Output("In GetGoldBarPresent");

            bool isGoldBarShown = ((BrowserHelper)helper).IsGoldBarPresent;
            NavigationHelper.Output("IsGoldBarShown: " + isGoldBarShown.ToString());

            if(isGoldBarShown)
            {
                NavigationHelper.Pass("Gold bar was shown due to LMZ lockdown, as expected.");
            }
            else
            {
                NavigationHelper.Fail("Gold bar was not shown when it should have been.");
            }
        }

        #endregion
    }
}
