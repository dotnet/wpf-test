// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// This class tests the Cross-cross process security mitigation doesn't break html Alerts.
    /// This only runs in partial trust.
    /// </summary>
    public class HtmlInteropXXProcAlert
    {
        #region Private Data

        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        Button _passButton = new Button();

        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (TestLog.Current == null)
            {
                new TestLog("HTML interop XXProc test");
            }

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
                    TestLog.Current.LogStatus("Frame Navigation to " + e.Uri);
                    (e.Navigator as Frame).Content = _stackPanel;
                }
                else if (e.Navigator is NavigationWindow)
                {
                    TestLog.Current.LogStatus("NavWin Navigation to " + e.Uri);
                    (e.Navigator as NavigationWindow).Content = _stackPanel;
                }

                _passButton.Content = "Click to Pass";
                _passButton.Click += PassTest;
                _stackPanel.Children.Add(_passButton);

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
            NavigationHelper.Output("In WebBrowser.LoadCompleted, showing Alert now.");

            Application.Current.MainWindow.Title = "Loaded";

            _browser.InvokeScript("ShowAlert");

            //if alert isn't shown, test will fail by timing out.  If alert is shown, Scripter will click the alert's OK button, then our Pass button. 
        }

        private void PassTest(object sender, EventArgs e)
        {
            NavigationHelper.Pass("Script Alert was shown from an XXProc WebOC as expected.");
        }
        #endregion
    }
}
