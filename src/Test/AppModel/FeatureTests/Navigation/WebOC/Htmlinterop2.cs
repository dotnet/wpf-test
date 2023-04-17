// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// This class is a regression test.  Nav to a string on the UI thread while trying to nav to a page on another thread.  Expect an InvalidOperationException.
    /// This runs in partial trust and full trust.
    /// </summary>
    public class Htmlinterop2
    {
        #region Private Data

        bool _caught = false;
        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();

        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop bug 196538 regression test");
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

                ThreadStart start = new ThreadStart(MyCode);
                Thread thread = new Thread(start);
                thread.Start();

                string htmlString = "<html><head><title>Page From String</title>\r\n" +
                   "<script type=\"text/javascript\">function SimpleTest(){ return 314159; }</script>\r\n" +
                   "</head><body><p>Page from string</p></body></html> ";
                _browser.NavigateToString(htmlString);
            }
        }

        public void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Fail("Unexpected exception caught. Test fails");
            e.Handled = true;
            ApplicationMonitor.NotifyStopMonitoring();
        }

        public void MyCode()
        {
            try
            {
                _browser.Navigate(new Uri("http://www.microsoft.com"));
            }
            catch (InvalidOperationException ex)
            {
                NavigationHelper.Output("Got expected exception: " + ex.ToString());
                _caught = true;
            }
            catch (Exception ex)
            {
                NavigationHelper.Fail("Got unexpected exception: " + ex.ToString());
            }
        }
        #endregion

        #region Private Members
        private void OnHtmlLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (_caught)
            {
                NavigationHelper.Pass("Caught expected exception trying to use WebBrowser from another thread.  HTML interop 196538 regression test passed");
            }
            else
            {
                NavigationHelper.Fail("Did not catch expected exception. HTML interop 196538 regression test failed");
            }
        }
        #endregion
    }
}
