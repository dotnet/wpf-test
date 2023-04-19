// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Interop;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class tests WebBrowser.Navigate(string) using an "incorrectly" escaped Uri.  Expect Uri to be passed to the html page.
    /// This runs in partial trust and full trust.
    /// </summary>
    public class Htmlinterop5
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
                new TestLog("HTML interop Navigate(string) test");
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

                string sourceLocation = BrowserInteropHelper.Source.ToString();
                sourceLocation = sourceLocation.Substring(0, sourceLocation.LastIndexOf('/'));

#if TESTBUILD_CLR40
                browser.Navigate(sourceLocation + "/HtmlInterop_HtmlPage1.htm?crypt=%7E%9D%CD%CB%A0%AE%A6%AF%87vu%94qm%82%90");
#endif
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
            object returnValue;

            NavigationHelper.Output("In LoadCompleted");

            returnValue = _browser.InvokeScript("VerifyUrlParameter");

            NavigationHelper.Output("Url parameter from script: " + returnValue.ToString());

            if (returnValue.ToString() == "crypt=%7E%9D%CD%CB%A0%AE%A6%AF%87vu%94qm%82%90")
            {
                NavigationHelper.Pass("Navigate(string) passed - Initial navigation went to page 1 with proper Url parameter");
            }
            else
            {
                NavigationHelper.Fail("Navigate(string) failed - Initial navigation did not have proper Url parameter");
            }
        }
        #endregion
    }
}
