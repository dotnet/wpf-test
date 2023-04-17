// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class is a regression test 1.  Nav to an html page, then to a string.  Navigate back should not assert.
    /// This runs in partial trust and full trust.
    /// </summary>
    public class htmlinterop1
    {

        #region Private Data
        private enum NavigationStage
        {
            InitialNavigation,
            SecondNavigation,
            NavigateBack
        }

        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        NavigationStage _currentStage = NavigationStage.InitialNavigation;

        HtmlInteropTestClass _testObject = new HtmlInteropTestClass();
        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop bug 193491 regression test");
            }

            //Local machine lockdown disables scripting inside IE.  Re-enable it for testing purposes.
            //Since we run in partial trust, we can't use the TestRuntime MachineStateManager stuff.  Do this instead.
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

                _browser.ObjectForScripting = _testObject;
	        _browser.LoadCompleted += OnHtmlLoadCompleted;

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
        private void OnHtmlLoadCompleted(object sender, NavigationEventArgs e)
        {
            switch (_currentStage)
            {
                case NavigationStage.InitialNavigation:
                    NavigationHelper.Output("In InitialNavigation");

                    _currentStage = NavigationStage.SecondNavigation;
                    string htmlString = "<html><head><title>Page From String</title>\r\n" +
                       "<script type=\"text/javascript\">function SimpleTest(){ return 314159; }</script>\r\n" +
                       "</head><body><p>Page from string</p></body></html> ";
                    _browser.NavigateToString(htmlString);
                break;

                case NavigationStage.SecondNavigation:
                    NavigationHelper.Output("In SecondNavigation");

                    _currentStage = NavigationStage.NavigateBack;
                    _browser.GoBack();
                break;

                case NavigationStage.NavigateBack:
                    //put local machine lockdown script setting back to disable
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

                    NavigationHelper.Output("In NavigateBack");

                    NavigationHelper.Output("CanGoForward: " + _browser.CanGoForward.ToString() + " CanGoBack: " + _browser.CanGoBack.ToString());
                    // We just went back from a page loaded from a string.  That doesn't show up in the journal.
                    // So both CanGoForward and CanGoBack should be false.
                    if (!_browser.CanGoForward && !_browser.CanGoBack)
                    {
                        NavigationHelper.Pass("HTML interop 193491 regression test passed");
                    }
                    else
                    {
                        NavigationHelper.Fail("HTML interop 193491 regression test failed");
                    }
                break;
            }
        }
        #endregion

    }
}
