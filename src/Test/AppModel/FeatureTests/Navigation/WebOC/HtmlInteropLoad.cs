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
    /// This class tests WebBrowser.NavigateToStream/String (formerly called LoadFromStream/string).
    /// This runs in partial trust and full trust.
    /// </summary>
    public class HtmlInteropLoad
    {

        #region Private Data
        private enum NavigationStage
        {
            NavigateToStream,
            NavigateToString,
        }

        WebBrowser _browser = null;
        StackPanel _stackPanel = new StackPanel();
        NavigationStage _currentStage = NavigationStage.NavigateToStream;
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

                _currentStage = NavigationStage.NavigateToStream;

                Stream source = Application.GetResourceStream(new Uri("pack://application:,,,/HtmlInterop_ResourcePage.html")).Stream;
                _browser.NavigateToStream(source);
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
                case NavigationStage.NavigateToStream:
                    NavigationHelper.Output("In NavigateToStream");

                    _browser.ObjectForScripting = _testObject;
                    returnValue = _browser.InvokeScript("SimpleTest");
                    if (returnValue.ToString() == "182")
                    {
                        NavigationHelper.Output("NavigateToStream passed");
                    }
                    else
                    {
                        NavigationHelper.Output("NavigateToStream failed, result was: " + returnValue.ToString() + " expected: 182");
                        _oneOrMoreTestsFailed = true;
                    }

                    //negative test - null stream
                    caught = false;
                    try
                    {
                        _browser.NavigateToStream(null);
                    }
                    catch (ArgumentNullException exception)
                    {
                        NavigationHelper.Output("NavigateToStream(null) got expected exception: " + exception.ToString());
                        caught = true;
                    }
                    if (!caught)
                    {
                        NavigationHelper.Output("NavigateToStream(null) didn't throw ArgumentNullException as expected");
                        _oneOrMoreTestsFailed = true;
                    }

                    _currentStage = NavigationStage.NavigateToString;
                    string htmlString = "<html><head><title>Page From String</title>\r\n" +
                       "<script type=\"text/javascript\">function SimpleTest(){ return 314159; }</script>\r\n" +
                       "</head><body><p>Page from string</p></body></html> ";
                    _browser.NavigateToString(htmlString);

               break;

               case NavigationStage.NavigateToString:
                    //put local machine lockdown script setting back to disable
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings\Lockdown_Zones\0", "1400", 1, RegistryValueKind.DWord);

                    NavigationHelper.Output("In NavigateToString");

                    _browser.ObjectForScripting = _testObject;
                    returnValue = _browser.InvokeScript("SimpleTest");
                    if (returnValue.ToString() == "314159")
                    {
                        NavigationHelper.Output("NavigateToString passed");
                    }
                    else
                    {
                        NavigationHelper.Output("NavigateToString failed, result was: " + returnValue.ToString() + " expected: 314159");
                        _oneOrMoreTestsFailed = true;
                    }

                    //negative test - null string
                    caught = false;
                    try
                    {
                        _browser.NavigateToString(null);
                    }
                    catch (ArgumentNullException exception)
                    {
                        NavigationHelper.Output("NavigateToString(null) got expected exception: " + exception.ToString());
                        caught = true;
                    }
                    if (!caught)
                    {
                        NavigationHelper.Output("NavigateToString(null) didn't throw ArgumentNullException as expected");
                        _oneOrMoreTestsFailed = true;
                    }

                    //test garbage string, should just show as text in the browser
                    caught = false;
                    try
                    {
                        _browser.NavigateToString("<invalid <");
                    }
                    catch (Exception exception)
                    {
                        NavigationHelper.Output("NavigateToString(garbage text) got unexpected exception: " + exception.ToString());
                        caught = true;
                        _oneOrMoreTestsFailed = true;
                    }
                    if (!caught)
                    {
                        NavigationHelper.Output("NavigateToString(garbage text) completed without exception - pass");
                    }


                    if (_oneOrMoreTestsFailed)
                    {
                        NavigationHelper.Fail("One or more HTML interop load tests failed");
                    }
                    else
                    {
                       NavigationHelper.Pass("HTML interop load tests passed");
                    }

                break;
            }
        }
        #endregion

    }
}
