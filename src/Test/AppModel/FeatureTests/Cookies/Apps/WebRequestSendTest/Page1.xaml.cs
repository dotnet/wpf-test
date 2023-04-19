// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;

namespace WebRequestCookie
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class Page1 : Page
    {

        static public Uri webrequestUri;

        public Page1()
        {
            InitializeComponent();


            if (TestLog.Current != null)
            {
                TestLog.Current.LogStatus("Getting cookie headers that are sent to the server with webrequest");
            }

            try
            {
                // Invoking helper to set application title to the value of cookie headers being sent to the server on navigation.
                TestWebRequestCookieHeader();
            }
            catch (Exception ex)
            {
                if (TestLog.Current != null)
                {
                    TestLog.Current.LogEvidence("Error getting header from webrequest." + ex.Message);
                    TestLog.Current.Result = TestResult.Fail;
                }
                ApplicationMonitor.NotifyStopMonitoring();
            }
        }

        public static void TestWebRequestCookieHeader()
        {
            Application.Current.MainWindow.Title = "TESTING";
            webrequestUri = new Uri(BrowserInteropHelper.Source.ToString() + "foo.html", UriKind.Absolute);
            Application.Current.Navigating += new NavigatingCancelEventHandler(Application_Navigating);
            ((NavigationWindow)Application.Current.MainWindow).Navigate(webrequestUri);
        }

        private static void Application_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.WebRequest.GetType() == typeof(HttpWebRequest))
            {
                Application.Current.MainWindow.Title = ((HttpWebRequest)e.WebRequest).CookieContainer.GetCookieHeader(webrequestUri);
                e.Cancel = true;
            }
        }
    }
}
