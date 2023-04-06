// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{

    /// <summary>
    /// This class is a regression test for Dev10 520954 - WebBrowser.NavigateToStream fails silently if done from Navigating event.
    /// This runs in partial trust and full trust.
    /// </summary>
    public class HtmlInterop520954
    {

        #region Private Data
        private enum NavigationStage
        {
            NavigateToStreamFirst,
            NavigateToStreamSecond,
        }

        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        NavigationStage _currentStage = NavigationStage.NavigateToStreamFirst;
        bool _oneOrMoreTestsFailed = false;
        bool _alreadyLooping = false;

        HtmlInteropTestClass _testObject = new HtmlInteropTestClass();
        #endregion

        #region Public Members
        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("HTML interop 520954 regression test");
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

                _currentStage = NavigationStage.NavigateToStreamFirst;

                Stream source = Application.GetResourceStream(new Uri("pack://application:,,,/HtmlInterop_ResourcePage.html")).Stream;
                _browser.NavigateToStream(source);
                //we could do this in Navigating but it's easier to do it here
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
        private void OnHtmlNavigating(object sender, NavigatingCancelEventArgs e)
        {
            NavigationHelper.Output("Navigating event fired");
            switch(_currentStage)
            {
                case NavigationStage.NavigateToStreamFirst:
                    //do nothing here
                break;

                case NavigationStage.NavigateToStreamSecond:
                    //alreadyLooping is used to keep us from recursing forever.  We only want to nest 1 navigation.
                    if (!_alreadyLooping)
                    {
                        _alreadyLooping = true;
                        NavigationHelper.Output("Calling NavigateToStream inside Navigating event");
                        Stream source = Application.GetResourceStream(new Uri("pack://application:,,,/HtmlInterop_ResourcePage.html")).Stream;
                        _browser.NavigateToStream(source);
                    }
                break;
            }
        }

        private void OnHtmlLoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("LoadCompleted event fired");
            switch (_currentStage)
            {
                case NavigationStage.NavigateToStreamFirst:
                    NavigationHelper.Output("In NavigateToStreamFirst");

                    if (false == BrowserInteropHelper.IsBrowserHosted) //standalone app case - webOC is not cross-cross process
                    {
                        if(_browser.Source == null)
                        {
                            NavigationHelper.Output("NavigateToStream then Navigate before LoadCompleted failed to go to correct page.  Source is null.");
                            _oneOrMoreTestsFailed = true;
                        }
                        else if (!_browser.Source.ToString().Contains("HtmlPage1.htm"))
                        {
                            NavigationHelper.Output("NavigateToStream then Navigate before LoadCompleted failed to go to correct page.  Went to: " + _browser.Source + " but expected htmlpage1.htm");
                            _oneOrMoreTestsFailed = true;
                        }
                        else
                        {
                            NavigationHelper.Output("NavigateToStream then Navigate before LoadCompleted succeeded - navigated to HtmlPage1.htm");
                        }
                    }
                    else //in browser, we're in cross-cross process mode.  Navigation events behave differently by design in this case.
                    {
                        if(_browser.Source == null)
                        {
                            NavigationHelper.Output("NavigateToStream then Navigate before LoadCompleted failed to go to correct page.  Source is null.");
                            _oneOrMoreTestsFailed = true;
                        }
                        else if (!_browser.Source.ToString().Contains("about:blank"))
                        {
                            NavigationHelper.Output("NavigateToStream then Navigate before LoadCompleted failed to go to correct page.  Went to: " + _browser.Source + " but expected about:blank");
                            _oneOrMoreTestsFailed = true;
                        }
                        else
                        {
                            NavigationHelper.Output("NavigateToStream then Navigate before LoadCompleted succeeded - navigated to about:blank");
                        }
                    }

                    _currentStage = NavigationStage.NavigateToStreamSecond;

                    _browser.Navigating += OnHtmlNavigating;
                    _browser.Navigate(new Uri("pack://siteoforigin:,,,/HtmlInterop_HtmlPage2.htm", UriKind.RelativeOrAbsolute));
               break;

               case NavigationStage.NavigateToStreamSecond:
                    //put local machine lockdown script setting back to disable
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

                    NavigationHelper.Output("In NavigateToStreamSecond");

                    if(_browser.Source == null)
                    {
                        NavigationHelper.Output("Navigate then NavigateToStream from Navigating succeeded - Navigated to stream.");
                    }
                    else
                    {                        
                        NavigationHelper.Output("Navigate then NavigateToStream from Navigating failed to navigate to stream.");
                        NavigationHelper.Output("Instead, got: " + _browser.Source.ToString());
                        _oneOrMoreTestsFailed = true;
                    }

                    if (_oneOrMoreTestsFailed)
                    {
                        NavigationHelper.Fail("One or more HTML interop 520954 regression tests failed");
                    }
                    else
                    {
                       NavigationHelper.Pass("HTML interop 520954 regression tests passed");
                    }
                break;
            }
        }
        #endregion

    }
}
