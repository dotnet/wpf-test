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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Interop;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class tests WebBrowser.ObjectForScripting.
    /// This runs in partial trust and full trust.
    /// </summary>
    public class HtmlInteropObjectForScripting
    {
        #region Private Data
        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        bool _oneOrMoreTestsFailed = false;

        HtmlInteropTestClass _testObject = new HtmlInteropTestClass();
        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop ObjectForScripting tests");
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

                //test ObjectForScripting get and set
                _browser.ObjectForScripting = _testObject;

                NavigationHelper.Output("ObjectForScripting result is: " + _testObject.ToString());
                if (_browser.ObjectForScripting != _testObject)
                {
                    NavigationHelper.Output("ObjectForScripting get/set test failed");
                    _oneOrMoreTestsFailed = true;
                }

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

            //put local machine lockdown script setting back to disable
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

            //test that ObjectForScripting was set properly
            object returnValue = _browser.InvokeScript("CallbackTest", new string[]{"ObjectForScripting test"});

            NavigationHelper.Output("ObjectForScripting usage result is: '" + _testObject.PageTitle + "' expected: 'ObjectForScripting test'");
            if (_testObject.PageTitle != "ObjectForScripting test")
            {
                NavigationHelper.Output("ObjectForScripting usage test failed");
                _oneOrMoreTestsFailed = true;
            }

            //test that using an internal object throws an ArgumentException
            bool caught = false;
            try
            {
                _browser.ObjectForScripting = new HtmlInteropInternalTestClass();
            }
            catch (ArgumentException exception)
            {
                NavigationHelper.Output("Set ObjectForScripting to internal class, got expected exception: " + exception.ToString());
                caught = true;
            }
            if (!caught)
            {
                NavigationHelper.Output("Did not see expected exception when setting ObjectForScripting to internal class.");
                _oneOrMoreTestsFailed = true;
            }

            //test that using an object not marked ComVisible throws an ArgumentException
            caught = false;
            try
            {
                _browser.ObjectForScripting = new HtmlInteropNonComVisibleTestClass();
            }
            catch (ArgumentException exception)
            {
                NavigationHelper.Output("Set ObjectForScripting to non-ComVisible class, got expected exception: " + exception.ToString());
                caught = true;
            }
            if (!caught)
            {
                NavigationHelper.Output("Did not see expected exception when setting ObjectForScripting to non-ComVisible class.");
                _oneOrMoreTestsFailed = true;
            }

            //test that setting ObjectForScripting to null is OK
            caught = false;
            try
            {
                _browser.ObjectForScripting = null;
            }
            catch (Exception exception)
            {
                NavigationHelper.Output("Setting ObjectForScripting to null got unexpected exception: " + exception.ToString());
                caught = true;
                _oneOrMoreTestsFailed = true;
            }
            if (!caught)
            {
                NavigationHelper.Output("Setting ObjectForScripting to null was successful.");
            }

            if (_oneOrMoreTestsFailed)
            {
                NavigationHelper.Fail("One or more HTML interop ObjectForScripting tests failed");
            }
            else
            {
                NavigationHelper.Pass("HTML interop ObjectForScripting tests passed");
            }
        }
        #endregion


        [System.Runtime.InteropServices.ComVisibleAttribute(true)]
        internal class HtmlInteropInternalTestClass
        {
            public void SetPageTitle(string title)
            {
                PageTitle = title;
            }

            public string PageTitle;
        }

        public class HtmlInteropNonComVisibleTestClass
        {
            public void SetPageTitle(string title)
            {
                PageTitle = title;
            }

            public string PageTitle;
        }
    }
}
