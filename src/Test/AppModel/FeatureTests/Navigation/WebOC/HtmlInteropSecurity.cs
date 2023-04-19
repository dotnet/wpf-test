// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Permissions;
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
    /// This class tests security-related aspects of Html Interop.
    /// This only runs in partial trust.
    /// </summary>
    public class HtmlInteropSecurity
    {

        #region Private Data
        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        bool _oneOrMoreTestsFailed = false;
        HtmlInteropSecurityTestClass _testObject = new HtmlInteropSecurityTestClass();

        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop Security tests");
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
            NavigationHelper.Output("In LoadCompleted");

            //test calling script that calls back into our class, on a method demanding UnmanagedCode permission
            object returnValue = _browser.InvokeScript("SecurityTestDirect");
            NavigationHelper.Output("InvokeScript(security test direct) result is: " + returnValue);
            if (returnValue.ToString() != "SecurityException")
            {
                NavigationHelper.Output("InvokeScript security test failed");
                _oneOrMoreTestsFailed = true;
            }

            //test calling script that calls back into our class, on a method calling a method demanding UnmanagedCode permission
            returnValue = _browser.InvokeScript("SecurityTestIndirect");
            NavigationHelper.Output("InvokeScript(security test indirect) result is: " + returnValue);
            if (returnValue.ToString() != "SecurityException")
            {
                NavigationHelper.Output("InvokeScript security test failed");
                _oneOrMoreTestsFailed = true;
            }

            //try to navigate to outside siteoforigin
            bool caught = false;

            try
            {
                //note: The file referenced below doesn't exist on the test server, and doesn't need to.
                //We just need to have WPF detect it's not site-of-origin
                //(which happens before we ever try to get the page), and throw a SecurityException.
                //Site of origin for this test case is the local machine, thus http://wpfapps is cross-site.
                _browser.Navigate(new Uri("http://wpfapps/testscratch/andren/bogus.htm"));
            }
            catch (SecurityException exception)
            {
                NavigationHelper.Output("Nav to outside the site of origin got expected exception: " + exception.ToString());
                caught = true;
            } 
            if (!caught)
            {
                NavigationHelper.Output("Nav to outside site of origin did not get exception when it should have.");
                _oneOrMoreTestsFailed = true;
            }

            //try to navigate to about:blank (not allowed in partial trust)
            caught = false;
            try
            {
                _browser.Navigate(new Uri("about:blank"));
            }
            catch (SecurityException exception)
            {
                NavigationHelper.Output("Navigate('about:blank') got expected exception: " + exception.ToString());
                caught = true;
            }
            if (!caught)
            {
                NavigationHelper.Output("Nav to about:blank did not get exception when it should have.");
                _oneOrMoreTestsFailed = true;
            }

            //try to access browser.Document (not allowed in partial trust)
            caught = false;
            try
            {
                object unused = _browser.Document;
            }
            catch (SecurityException exception)
            {
                NavigationHelper.Output("Accessing Document got expected exception: " + exception.ToString());
                caught = true;
            }
            if (!caught)
            {
                NavigationHelper.Output("Accessing Document did not get exception when it should have.");
                _oneOrMoreTestsFailed = true;
            }

            //put local machine lockdown script setting back to disable
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

            if (_oneOrMoreTestsFailed)
            {
                NavigationHelper.Fail("One or more HTML interop security tests failed");
            }
            else
            {
                NavigationHelper.Pass("HTML interop security tests passed");
            }
        }
        #endregion

        /// <summary>
        /// An instance of this class is passed to an html page by setting ObjectForScripting.
        /// This lets the html script call back into the test app.
        /// </summary>
        [System.Runtime.InteropServices.ComVisibleAttribute(true)]
        public class HtmlInteropSecurityTestClass
        {
            public void SetPageTitle(string title)
            {
                SetPageTitleCore(title);
            }

            [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
            public void SetPageTitleCore(string title)
            {
                PageTitle = title;
            }

            public string PageTitle;
        }
    }
}
