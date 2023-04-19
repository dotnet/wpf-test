// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Logging;
using System.Windows.Automation;
using Microsoft.Test.Input;
using Microsoft.Test.Loaders;
using System.Windows.Interop;
using Microsoft.Test.Deployment;
using Microsoft.Test.Globalization;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// HlinkNavigation

    /// <summary>
    /// Performs multiple hyperlink navigations in an avalon xbap, goes browser back and browser forward 
    /// and verifies IE 7 Travelog state at the end of each of these operations
    /// 
    /// </summary>
    public class HlinkNavigation
    {
        private enum CurrentTest
        {
            UnInit,
            Init,
            HlinkNav1,
            HlinkNav2,
            VerifyAfterNav,
            VerifyAfterGoBack,
            VerifyAfterGoForward,
            End
        }

        #region HlinkNavigation privates
        private CurrentTest _hlinkNavigationTestState = CurrentTest.UnInit;
        private NavigationWindow _navWin = null;
        private const String hyperlinkNavigationPage = @"HyperlinkLocalFileAccess_hyperlink.xaml";
        private String[] _stackAfterGoBack = null;
        private String[] _stackAfterGoForward = null;
        private String[] _stackAfterNav = null;
        private String[] _backstackAfterNav = null;
        private String[] _backstackAfterGoBack = null;
        private String[] _forwardstackAfterGoBack = null;
        private String[] _backstackAfterGoForward = null;

        // JournalHelper objects to hold journal info
        private JournalHelper _journalHelperAfterNav = null;
        private JournalHelper _journalHelperAfterGoBack = null;
        private JournalHelper _journalHelperAfterGoForward = null;
        #endregion

        public void Startup(object sender, StartupEventArgs e)
        {
            _hlinkNavigationTestState = CurrentTest.Init;
            NavigationHelper.CreateLog("Hyperlink navigation within xbap");
            if (ApplicationDeploymentHelper.GetIEVersion() >= 9)
            {
                NavigationHelper.ExitWithIgnore("Test broken on IE9+ due to changed journal behavior.  Ignore-ing result");
            }
            else
            {
                CreateExpectedStacks();
                NavigationHelper.SetStage(Microsoft.Test.Logging.TestStage.Run);
                Application.Current.StartupUri = new Uri(hyperlinkNavigationPage, UriKind.RelativeOrAbsolute);
            }
        }

        public void Navigated(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("In navigated event handler. State = " + _hlinkNavigationTestState);
            switch (_hlinkNavigationTestState)
            {
                case CurrentTest.Init:
                    if (_navWin == null)
                    {
                        _navWin = Application.Current.MainWindow as NavigationWindow;
                    }
                    break;
            }
        }

        // you get here after every navigation
        public void ContentRendered(object sender, EventArgs e)
        {
            NavigationHelper.Output("In ContentRendered. State : " + _hlinkNavigationTestState.ToString());

            switch (_hlinkNavigationTestState)
            {
                case CurrentTest.VerifyAfterGoBack:
                    _journalHelperAfterGoBack = new JournalHelper();
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoBack();
                            return null;
                        }, null);
                    break;

                case CurrentTest.VerifyAfterGoForward:
                    _journalHelperAfterGoForward = new JournalHelper();
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoForward();
                            return null;
                        }, null);
                    break;

                case CurrentTest.VerifyAfterNav:
                    _journalHelperAfterNav = new JournalHelper();
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterNav();
                            return null;
                        }, null);
                    break;

                default:
                    PerformNavigation();
                    break;
            }
        }

        // create expected states
        private void CreateExpectedStacks()
        {
            // retrieve the proper localized version for "Current Page"
            string currentPage = BrowserHelper.CurrentPageValue;

            _stackAfterGoForward
            = new string[]{
                    "CompiledPage 3 (Compiled3.xaml)", 
                    "CompiledPage 2 (HlinkNavigation_Compile", 
                    "HyperlinkLocalFileAccess_hyperlink.xaml"
            };

            _stackAfterGoBack
            = new string[]{
                    "CompiledPage 3 (Compiled3.xaml)", 
                    "CompiledPage 2 (HlinkNavigation_Compile", 
                    "HyperlinkLocalFileAccess_hyperlink.xaml"
            };

            _stackAfterNav =
                new string[] {
                    currentPage, 
                    "CompiledPage 2 (HlinkNavigation_Compile", 
                    "HyperlinkLocalFileAccess_hyperlink.xaml"
                };

            _backstackAfterNav =
                new string[] {
                    "CompiledPage 2 (HlinkNavigation_Compile", 
                    "HyperlinkLocalFileAccess_hyperlink.xaml"
                };

            _backstackAfterGoBack
                = new string[]{
                    "HyperlinkLocalFileAccess_hyperlink.xaml"
                };

            _forwardstackAfterGoBack
                = new string[]{
                    "CompiledPage 3 (Compiled3.xaml)"
                };

            _backstackAfterGoForward =
                new string[] {
                    "CompiledPage 2 (HlinkNavigation_Compile", 
                    "HyperlinkLocalFileAccess_hyperlink.xaml"
                };
        }

        // perform the navigation
        private void PerformNavigation()
        {
            Hyperlink hlinkTextBlock = null;
            switch (_hlinkNavigationTestState)
            {
                case CurrentTest.Init:
                    hlinkTextBlock = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, "hlinkTextBlock") as Hyperlink;
                    hlinkTextBlock.DoClick();
                    Thread.Sleep(1000);
                    _hlinkNavigationTestState = CurrentTest.HlinkNav1;
                    break;

                case CurrentTest.HlinkNav1:
                    hlinkTextBlock = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, "hlink") as Hyperlink;
                    hlinkTextBlock.DoClick();
                    _hlinkNavigationTestState = CurrentTest.VerifyAfterNav;
                    break;
            }
        }

        // verify after hyperlink navigations are done
        private void VerifyAfterNav()
        {
            NavigationHelper.Output("In VerifyAfterNav");

            Thread.Sleep(3000);

            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, false, _backstackAfterNav, 
                null, "CompiledPage 3", _journalHelperAfterNav))
            {
                NavigationHelper.Output("Journal State AfterNav verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State AfterNav verification failed");
            }

            // verify the browser journal properties against expected
            // stable enough only for Vista because we cannot identify the browser idle state
            if (Environment.OSVersion.Version.Major == 6) 
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, false, _stackAfterNav, "CompiledPage 3 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("Browser State AfterNav verified");
                }
                else
                {
                    NavigationHelper.Fail("Browser State AfterNav verification failed");
                }
            }

            Thread.Sleep(3000);
            // Move and click the mouse away from the "recent pages"
            // Otherwise invoke on browser back wouldn't succeed
            BrowserHelper.ClickCenterOfBrowser();

            // Invoke the browser back button
            BrowserHelper.GetMainBrowserWindow(Application.Current).WaitWhileBusy();
            NavigationHelper.Output("Invoking the Browser Back button");
            BrowserHelper.InvokeGoBackButton();
            BrowserHelper.GetMainBrowserWindow(Application.Current).WaitWhileBusy();
            NavigationHelper.Output("Successfully invoked the Browser Back button");

            _hlinkNavigationTestState = CurrentTest.VerifyAfterGoBack;
        }

        // verify after clicking on the GoBack button and click GoForward
        private void VerifyAfterGoBack()
        {
            NavigationHelper.Output("In VerifyAfterGoBack");
            Thread.Sleep(3000);

            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, true, _backstackAfterGoBack,
                _forwardstackAfterGoBack, "CompiledPage 2", _journalHelperAfterGoBack))
            {
                NavigationHelper.Output("Journal State AfterGoBack verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State AfterGoBack verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6) 
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, true, _stackAfterGoBack, "CompiledPage 2 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("Browser State AfterGoBack verified");
                }
                else
                {
                    NavigationHelper.Fail("Browser State AfterGoBack verification failed");
                }
            }

            Thread.Sleep(3000);
            // Move and click the mouse away from "recent pages" button
            // Otherwise click on Forward wouldn't succeed
            BrowserHelper.ClickCenterOfBrowser();

            BrowserHelper.GetMainBrowserWindow(Application.Current).WaitWhileBusy();
            NavigationHelper.Output("Invoking the Browser Forward button");
            BrowserHelper.InvokeGoForwardButton();
            NavigationHelper.Output("Successfully invoked the Browser Forward button");
            BrowserHelper.GetMainBrowserWindow(Application.Current).WaitWhileBusy();
            _hlinkNavigationTestState = CurrentTest.VerifyAfterGoForward;
        }

        // verify after clicking on the GoForward button
        private void VerifyAfterGoForward()
        {
            Thread.Sleep(3000);
            NavigationHelper.Output("In VerifyAfterGoForward");

            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, false, _backstackAfterGoForward,
                null, "CompiledPage 3", _journalHelperAfterGoBack))
            {
                NavigationHelper.Output("Journal State GoForward verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State GoForward verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6) 
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, false, _stackAfterGoForward, "CompiledPage 3 - Windows Internet Explorer"))
                {
                    NavigationHelper.Pass("Browser State AfterGoForward verified");
                }
                else
                {
                    NavigationHelper.Fail("Browser State AfterGoForward verification failed");
                }
            }
            else
            {
                NavigationHelper.Pass("All states verified");
            }

            NavigationHelper.SetStage(TestStage.Cleanup);
        }

        public void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Output("State = " + _hlinkNavigationTestState);

            e.Handled = true;
            NavigationHelper.Fail("Failed due to dispatcher exception");
        }
    }
}
