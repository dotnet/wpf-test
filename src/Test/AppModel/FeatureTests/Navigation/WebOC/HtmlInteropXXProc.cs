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
    /// This class tests the Vista-only Cross-cross process security mitigation.
    /// This only runs in partial trust.
    /// </summary>
    public class HtmlInteropXXProc
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

            // Find the WebBrowser control's process ID
            BrowserHelper helper = BrowserHelper.GetMainBrowserWindow(Application.Current);
            IntPtr webBrowserProcID = helper.GetWebBrowserProcessId(_browser);
            // And our main Window's process ID
            IntPtr windowProcID = helper.GetWindowProcessId(Application.Current.MainWindow);
            NavigationHelper.Output("WebBrowser proc ID: " + webBrowserProcID + " App proc ID: " + windowProcID);

            if ((Environment.OSVersion.Version.Major < 6) && (Environment.Version.Major < 4))  // pre-Vista with .NET < 4
            { 
                if (webBrowserProcID == windowProcID)
                {
                    NavigationHelper.Pass("On pre-Vista, WebBrowser is owned by the app, as expected.");
                }
                else
                {
                    NavigationHelper.Fail("On pre-Vista, WebBrowser proc ID unexpectedly didn't match the app's proc ID");
                }
            }
            else // (Vista or greater), or (Pre-Vista and .NET 4)
            {
                if (webBrowserProcID != windowProcID)
                {
                    NavigationHelper.Pass("On Vista+ or XP on .NET4+, WebBrowser is owned by IE, as expected.");
                }
                else
                {
                    NavigationHelper.Fail("On Vista+ or XP on .NET4+, WebBrowser proc ID unexpectedly matched the app's proc ID");
                }
            }
        }
        #endregion
    }
}
