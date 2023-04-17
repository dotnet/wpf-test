// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class is a regression test for Dev10 573618 - Verify javascript: and vbscript: protocols can be navigated.
    /// This runs in partial trust and full trust.
    /// </summary>
    public class Htmlinterop4
    {
        #region Private Data

        private enum CurrentStage
        {
            Starting,
            TestJavascript,
            TestVBScript,
            NavigateToPage2,
            TestJSNavBack,
        }

        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        bool _oneOrMoreTestsFailed = false;
        DispatcherTimer _timer = new DispatcherTimer();

        CurrentStage _stage = CurrentStage.Starting;

        HtmlInteropTestClass _testObject = new HtmlInteropTestClass();
        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop 573618 regression tests");
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

                _timer.Interval = new TimeSpan(0,0,2);
                _timer.Tick += Timer_Tick;

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
        private void Timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();
            NavigationHelper.Output("Timer tick fired");

            switch(_stage)
            {
                case CurrentStage.TestJavascript:
                    NavigationHelper.Output("In TestJavascript");
                    object retval = _browser.InvokeScript("getMarker");
                    if(retval.ToString() == "42")
                    {
                        NavigationHelper.Output("Javascript navigation worked - got back 42");
                    }
                     else
                    {
                        NavigationHelper.Output("Javascript navigation failed: " + retval.ToString());
                        _oneOrMoreTestsFailed = true;
                    }

                    _stage = CurrentStage.TestVBScript;
                    NavigationHelper.Output("about to invoke script for doSetMarkerVB");
                    _browser.InvokeScript("doSetMarkerVB", 201);

                    // Navigating to a javascript: or vbscript: uri doesn't do a complete navigation, yet we still need to
                    // wait for the browser html to update.  So we start our timer for that.
                    _timer.Start();

                    break;

                case CurrentStage.TestVBScript:
                    NavigationHelper.Output("In TestVBScript");

                    retval = _browser.InvokeScript("getMarker");
                    if(retval.ToString() == "201")
                    {
                        NavigationHelper.Output("Javascript navigation worked - got back 201");
                    }
                    else
                    {
                        NavigationHelper.Output("Javascript navigation failed: " + retval.ToString());
                        _oneOrMoreTestsFailed = true;
                    }

                    //now we want to test a jscript nav back, but we need to nav forward first!
                    _stage = CurrentStage.NavigateToPage2;
                    NavigationHelper.Output("about to navigate to page 2");
                    _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage2.htm", UriKind.RelativeOrAbsolute));

                    break;

                default:
                    NavigationHelper.Output("Shouldn't have hit this");
                    _oneOrMoreTestsFailed = true;

                    break;
            }
        }

        private void OnHtmlLoadCompleted(object sender, NavigationEventArgs e)
        {

            NavigationHelper.Output("LoadCompleted event fired");
            NavigationHelper.Output("source: " + _browser.Source);

            switch(_stage)
            {
                case CurrentStage.Starting:
                    _stage = CurrentStage.TestJavascript;
                    NavigationHelper.Output("about to invoke script for doSetMarkerJS");
                    _browser.InvokeScript("doSetMarkerJS", 42);

                    // Navigating to a javascript: or vbscript: uri doesn't do a complete navigation, yet we still need to
                    // wait for the browser html to update.  So we start our timer for that.
                    _timer.Start();

                    break;

                case CurrentStage.NavigateToPage2:
                    _stage = CurrentStage.TestJSNavBack;
                    NavigationHelper.Output("about to invoke script for JSNavBack.  Full navigation should be triggered.");
                    _browser.InvokeScript("JSNavBack");

                    break;

                case CurrentStage.TestJSNavBack:
                    NavigationHelper.Output("in TestJSNavBack");
             
                    if (_browser.Source.ToString().Contains("Page1"))
                    {
                        NavigationHelper.Output("JavaScript back navigation worked - we're on page 1 as expected.");
                    }
                    else
                    {
                        NavigationHelper.Output("JavaScript back navigation failed - we're not on page 1");
                        _oneOrMoreTestsFailed = true;
                    }

                    //put local machine lockdown script setting back to disable
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

                    if (_oneOrMoreTestsFailed)
                    {
                        NavigationHelper.Fail("One or more HTML interop 573618 regression tests failed");
                    }
                    else
                    {
                       NavigationHelper.Pass("HTML interop 573618 regression tests passed");
                    }

                    break;
            }
        }
        #endregion

    }
}
