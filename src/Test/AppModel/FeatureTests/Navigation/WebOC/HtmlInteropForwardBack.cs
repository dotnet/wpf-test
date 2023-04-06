// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    /// This class tests WebBrowser.GoForward, GoBack, CanGoForward, and CanGoBack.
    /// This runs in partial trust and full trust.
    /// </summary>
    public class HtmlInteropForwardBack
    {

        #region Private Data
        private enum NavigationStage
        {
            InitialNavigation,
            SecondNavigation,
            NavigateBack,
            NavigateForward
        }

        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        NavigationStage _currentStage = NavigationStage.InitialNavigation;
        bool _oneOrMoreTestsFailed = false;

        HtmlInteropTestClass _testObject = new HtmlInteropTestClass();
        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop forward/back tests");
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
            object returnValue;
            bool caught = false;
            switch (_currentStage)
            {
                case NavigationStage.InitialNavigation:
                    //test calling w/no args
                    returnValue = _browser.InvokeScript("SimpleTest");
                    NavigationHelper.Output("In InitialNavigation");
                    if (returnValue.ToString() == "42")
                    {
                        NavigationHelper.Output("Initial navigation went to page 1");
                    }
                    else
                    {
                        NavigationHelper.Output("Initial navigation did not go to page 1");
                        _oneOrMoreTestsFailed = true;
                    }

                    _currentStage = NavigationStage.SecondNavigation;
                    _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage2.htm", UriKind.RelativeOrAbsolute));
                break;

                case NavigationStage.SecondNavigation:
                    //test calling w/no args
                    returnValue = _browser.InvokeScript("SimpleTest");
                    NavigationHelper.Output("In SecondNavigation");
                    if (returnValue.ToString() == "1337")
                    {
                        NavigationHelper.Output("Second navigation went to page 2");
                    }
                    else
                    {
                        NavigationHelper.Output("Second navigation did not go to page 2");
                        _oneOrMoreTestsFailed = true;
                    }

                    _currentStage = NavigationStage.NavigateBack;
                    _browser.GoBack();
                break;

                case NavigationStage.NavigateBack:
                    NavigationHelper.Output("In NavigateBack");

                    if (_browser.CanGoForward && !_browser.CanGoBack)
                    {
                        NavigationHelper.Output("GoBack correctly went back");
                    }
                    else
                    {
                        NavigationHelper.Output("Incorrect navigation state.  CanGoForward = " + _browser.CanGoForward + ", CanGoBack = " + _browser.CanGoBack);
                        _oneOrMoreTestsFailed = true;
                    }

                    //Negative test - try going back when there is no back
                    caught = false;
                    try
                    {
                        _browser.GoBack();
                    }
                    catch (COMException exception)
                    {
                        NavigationHelper.Output("GoBack when there is no back, got expected exception: " + exception.ToString());
                        caught = true;
                    }
                    if (!caught)
                    {
                        NavigationHelper.Output("Did not see expected exception when attempting to GoBack when invalid");
                        _oneOrMoreTestsFailed = true;
                    }

                    _currentStage = NavigationStage.NavigateForward;
                    _browser.GoForward();
                break;

                case NavigationStage.NavigateForward:
                    //put local machine lockdown script setting back to disable
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

                    NavigationHelper.Output("In NavigateForward");

                    if (!_browser.CanGoForward && _browser.CanGoBack)
                    {
                        NavigationHelper.Output("GoForward correctly went forward");
                    }
                    else
                    {
                        NavigationHelper.Output("Incorrect navigation state.  CanGoForward = " + _browser.CanGoForward + ", CanGoBack = " + _browser.CanGoBack);
                        _oneOrMoreTestsFailed = true;
                    }

                    //Negative test - try going forward when there is no forward
                    caught = false;
                    try
                    {
                        _browser.GoForward();
                    }
                    catch (COMException exception)
                    {
                        NavigationHelper.Output("GoForward when there is no forward, got expected exception: " + exception.ToString());
                        caught = true;
                    }
                    if (!caught)
                    {
                        NavigationHelper.Output("Did not see expected exception when attempting to GoForward when invalid");
                        _oneOrMoreTestsFailed = true;
                    }

                    if (_oneOrMoreTestsFailed)
                    {
                        NavigationHelper.Fail("One or more HTML interop forward/back tests failed");
                    }
                    else
                    {
                        NavigationHelper.Pass("HTML interop forward/back tests passed");
                    }
                break;
            }
        }
        #endregion

    }
}
