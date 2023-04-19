// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
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
    /// This class tests WebBrowser.Navigate.
    /// This runs in partial trust and full trust.
    /// Note the one special-cased area dealing with navigate to about:blank since only that part isn't allowed in partial trust.
    /// </summary>
    public class HtmlInteropNavigate
    {

        #region Private Data
        private enum NavigationStage
        {
            InitialNavigation,
            NavigateWithArguments,
            NavigateToAboutBlank,
            NavigateToNull,
            NegativeTests
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
                new TestLog("HTML interop Navigate/Load tests");
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

                //browser.Source test
                if (_browser.Source == null)
                {
                    NavigationHelper.Output("Getting null browser.Source passed");
                }
                else
                {
                    NavigationHelper.Output("browser.Source was not null as expected.  Result: " + _browser.Source.ToString());
                    _oneOrMoreTestsFailed = true;
                }

                _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage1.htm", UriKind.RelativeOrAbsolute));
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
            bool caught = false;
            object returnValue;
            switch (_currentStage)
            {
                case NavigationStage.InitialNavigation:
                    NavigationHelper.Output("In InitialNavigation");

                    returnValue = _browser.InvokeScript("SimpleTest");
                    if (returnValue.ToString() == "42")
                    {
                        NavigationHelper.Output("Navigate passed - Initial navigation went to page 1");
                    }
                    else
                    {
                        NavigationHelper.Output("Navigate failed - Initial navigation did not go to page 1");
                        _oneOrMoreTestsFailed = true;
                    }

                    //verify browser.Source can be retrieved when not null
                    if (_browser.Source != null)
                    {
                        NavigationHelper.Output("Getting browser.Source passed: " + _browser.Source.ToString());
                    }
                    else
                    {
                        NavigationHelper.Output("browser.Source was null when it should have been valid");
                        _oneOrMoreTestsFailed = true;
                    }

                    _currentStage = NavigationStage.NavigateWithArguments;
                    try
                    {
                        _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage2.htm", UriKind.RelativeOrAbsolute),
                            (BrowserInteropHelper.IsBrowserHosted) ? null : "_self", 
                            new ASCIIEncoding().GetBytes("Arg1=9"), "Content-Type: application/x-www-form-urlencoded\r\n");
                    }
                    catch(Exception exception)
                    {
                        NavigationHelper.Output("got unexpected exception: " + exception.ToString());
                        _oneOrMoreTestsFailed = true;
                    }
                break;

                case NavigationStage.NavigateWithArguments:
                    NavigationHelper.Output("In NavigateWithArguments");

                    //apparently there's no easy way to verify the post data and optional header args went anywhere unless we post to a server
                    //so just verify we're on the right page
                    returnValue = _browser.InvokeScript("SimpleTest");
                    if (returnValue.ToString() == "1337")
                    {
                        NavigationHelper.Output("Second navigation went to page 2");
                    }
                    else
                    {
                        NavigationHelper.Output("Second navigation did not go to page 2.  Return value was: " + returnValue.ToString() + " expected: 1337");
                        _oneOrMoreTestsFailed = true;
                    }

                    _currentStage = NavigationStage.NavigateToAboutBlank;
                    caught = false;
                    try
                    {
                        //navigate to about:blank isn't valid in partial trust - just navigate to null to move past this
                        if (BrowserInteropHelper.IsBrowserHosted)
                        {
                            _currentStage = NavigationStage.NavigateToNull;
                            _browser.Navigate((Uri)null);
                        }
                        else
                        {
                            _browser.Navigate(new Uri("about:blank"));
                        }
                    }
                    catch (Exception exception)
                    {
                        NavigationHelper.Output("Navigate('about:blank') got unexpected exception: " + exception.ToString());
                        _oneOrMoreTestsFailed = true;
                    }
                break;

                case NavigationStage.NavigateToAboutBlank:
                    NavigationHelper.Output("In NavigateToAboutBlank");

                    //verify we went to about:blank
                    if ((_browser.Source.ToString() == "about:blank") && (e.Uri.ToString() == "about:blank"))
                    {
                        NavigationHelper.Output("Navigate to about:blank was successful");
                    }
                    else
                    {
                        NavigationHelper.Output("Navigate to about:blank failed.  Return value of Source was: " + _browser.Source.ToString());
                        NavigationHelper.Output("Navigate to about:blank failed.  Return value of e.Uri was: " + e.Uri.ToString());
                        _oneOrMoreTestsFailed = true;
                    }

                    _currentStage = NavigationStage.NavigateToNull;
                    try
                    {
                        _browser.Navigate((Uri)null);
                    }
                    catch (Exception exception)
                    {
                        NavigationHelper.Output("Navigate(null) got unexpected exception: " + exception.ToString());
                        _oneOrMoreTestsFailed = true;
                    }
                break;

                case NavigationStage.NavigateToNull:
                    NavigationHelper.Output("In NavigateToNull");

                    //verify we went to null
                    if (_browser.Source == null)
                    {
                        NavigationHelper.Output("Navigate to null was successful");
                    }
                    else
                    {
                        NavigationHelper.Output("Navigate to null failed.  Return value of Source was: " + _browser.Source.ToString());
                        _oneOrMoreTestsFailed = true;
                    }

                    _currentStage = NavigationStage.NegativeTests;
                    //negative test - nav to relative uri
                    caught = false;
                    try
                    {
                        _browser.Navigate(new Uri("..\foo.html", UriKind.Relative));
                    }
                    catch (ArgumentException exception)
                    {
                        NavigationHelper.Output("Navigate(relative Uri) got expected exception: " + exception.ToString());
                        caught = true;
                    }
                    if (!caught)
                    {
                        NavigationHelper.Output("Navigate(relative Uri) didn't throw as expected");
                        _oneOrMoreTestsFailed = true;
                    }

                    //negative test - navigate disposed browser
                    caught = false;
                    _browser.Dispose();     
                    try
                    {
                        _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage1.htm", UriKind.RelativeOrAbsolute));
                    }
                    catch (ObjectDisposedException exception)
                    {
                        NavigationHelper.Output("Navigate after dispose got expected exception: " + exception.ToString());
                        caught = true;
                    }
                    if (!caught)
                    {
                        NavigationHelper.Output("Navigate after dispose didn't throw as expected");
                        _oneOrMoreTestsFailed = true;
                    }

                    //put local machine lockdown script setting back to disable
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

                    if (_oneOrMoreTestsFailed)
                    {
                        NavigationHelper.Fail("One or more HTML interop navigate tests failed");
                    }
                    else
                    {
                       NavigationHelper.Pass("HTML interop navigate tests passed");
                    }
                break;
            }
        }
        #endregion
    }
}
