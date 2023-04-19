// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class tests WebBrowser.Refresh() and Refresh(bool noCache).
    /// This runs in partial trust and full trust.
    /// </summary>
    public class HtmlInteropRefresh
    {

        #region Private Data
        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        bool _oneOrMoreTestsFailed = false;
        HtmlInteropTestClass _testObject = new HtmlInteropTestClass();
        DispatcherTimer _timer = new DispatcherTimer();

        RefreshStage _currentStage = RefreshStage.BeforeRefresh;

        private enum RefreshStage
        {
            BeforeRefresh,
            SimpleRefresh,
            NoCacheRefresh,
            CacheRefresh
        }
        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop refresh tests");
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

                _browser.LoadCompleted += OnHtmlLoadCompleted;

                //try Refresh before there's an HTMLDocument
                bool caught = false;
                try
                {
                    _browser.Refresh();
                }
                catch (System.Runtime.InteropServices.COMException exception)
                {
                    NavigationHelper.Output("browser.Refresh with no HTMLDocument got expected exception: " + exception.ToString());
                    caught = true;
                }
                if (!caught)
                {
                    NavigationHelper.Output("browser.Refresh didn't throw COMException as expected");
                    _oneOrMoreTestsFailed = true;
                }    

                _browser.ObjectForScripting = _testObject;
                _timer.Tick += OnTick;
                _timer.Interval = TimeSpan.FromMilliseconds(2000);

                NavigationHelper.Output("Navigating to html test page");
                _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage1.htm", UriKind.RelativeOrAbsolute));
            }
        }

        public void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Fail("Unexpected exception caught. Test fails");
            e.Handled = true;
            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
        }
        #endregion

        #region Private Members

        private bool _loadCompletedHandlerExecutedAlready = false;
        private void OnHtmlLoadCompleted(object sender, NavigationEventArgs e)
        {
            if (!_loadCompletedHandlerExecutedAlready)
            {
                _loadCompletedHandlerExecutedAlready = true;
                NavigationHelper.Output("LoadCompleted event fired");
                NavigationHelper.Output("About to call Refresh()");
                _currentStage = RefreshStage.SimpleRefresh;
                _browser.InvokeScript("MarkBeforeRefresh");  //this sets the html page's background to blue, so we can measure that Refresh happened.
                _browser.Refresh();
                _timer.Start();
            }
        }

        private void OnTick(object sender, EventArgs e)
        {   
            object returnValue = null;

            _timer.Stop();
            NavigationHelper.Output("OnTick fired");

            switch(_currentStage)
            {
                case RefreshStage.SimpleRefresh:                    
                    returnValue = _browser.InvokeScript("RefreshTest");
                    if (returnValue.ToString() == "passed")
                    {
                        NavigationHelper.Output("Refresh() passed");
                    }
                    else
                    {
                        NavigationHelper.Output("Refresh() failed");
                        _oneOrMoreTestsFailed = true;
                    }

                    NavigationHelper.Output("Starting NoCacheRefresh test");
                    _currentStage = RefreshStage.NoCacheRefresh;
                    _browser.InvokeScript("MarkBeforeRefresh");
                    _browser.Refresh(true);
                    _timer.Start();
                break;

                case RefreshStage.NoCacheRefresh:
                    returnValue = _browser.InvokeScript("RefreshTest");
                    if (returnValue.ToString() == "passed")
                    {
                        NavigationHelper.Output("Refresh(true) passed");
                    }
                    else
                    {
                        NavigationHelper.Output("Refresh(true) failed");
                        _oneOrMoreTestsFailed = true;
                    }

                    NavigationHelper.Output("Starting CacheRefresh test");
                    _currentStage = RefreshStage.CacheRefresh;
                    _browser.InvokeScript("MarkBeforeRefresh");
                    _browser.Refresh(false);
                    _timer.Start();
                break;

                case RefreshStage.CacheRefresh:
                    returnValue = _browser.InvokeScript("RefreshTest");
                    if (returnValue.ToString() == "passed")
                    {
                        NavigationHelper.Output("Refresh(false) passed");
                    }
                    else
                    {
                        NavigationHelper.Output("Refresh(false) failed");
                        _oneOrMoreTestsFailed = true;
                    }

                    //put local machine lockdown script setting back to disable
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

                    if (_oneOrMoreTestsFailed)
                    {
                        NavigationHelper.Fail("One or more HTML interop refresh tests failed");
                    }
                    else
                    {
                        NavigationHelper.Pass("HTML interop refresh tests passed");
                    }
                break;
            }
        }
        #endregion

    }
}
