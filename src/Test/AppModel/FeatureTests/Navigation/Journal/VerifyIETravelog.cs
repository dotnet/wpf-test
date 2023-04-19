// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.IO;
using System.Windows;
using System.Reflection;
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
    /// VerifyIETravelog - Test IEJournal navigation scenarios other than hyperlink
    /// <summary>
    /// 
    /// Step1 - Navigate to StartupUri - VerifyIETravelog_Start.xaml
    /// Step2 - Navigate to compiled XAML in directory - Pages/VerifyIETravelog_Page1.xaml
    /// Step3 - Navigate to compiled XAML at root - VerifyIETravelog_Page2.xaml
    /// Step4 - Navigate to a loose XAML - LoosePages/VerifyIETravelog_Page3.xaml
    /// Step5 - Navigate to compiled XAML at root - VerifyIETravelog_Page4.xaml
    /// Step6 - Verify the browser states (back, forward, window titles) and journal items
    /// Step7 - Go browser back four times and verify the browser states and journal items
    /// Step8 - Go browser forward four times and verify the browser states and journal items
    /// 
    /// </summary>
    public class VerifyIETravelog
    {
        private enum CurrentTest
        {
            UnInit,
            Init,
            NavToCompiledXamlInDirectory,
            NavToCompiledXaml,
            NavToXamlAtSiteOfOriginInDir,
            VerifyAfterNav,
            VerifyAfterGoBack1,
            VerifyAfterGoBack2,
            VerifyAfterGoBack3,
            VerifyAfterGoBack4,
            VerifyAfterGoForward1,
            VerifyAfterGoForward2,
            VerifyAfterGoForward3,
            VerifyAfterGoForward4,
            End
        }

        #region VerifyIETravelog privates
        private NavigationWindow _navWin = null;
        private Uri _activationUri = null;
        private CurrentTest _testState = CurrentTest.UnInit;
        private String[] _afterNavStack = null;
        private String[] _afterGoBackStack = null;
        private String[] _afterGoForwardStack = null;
        private String[] _backstackAfterNav = null;
        private String[] _forwardstackAfterGoBack = null;
        private String[] _backstackAfterGoForward = null;

        // JournalHelper objects to hold journal info
        private JournalHelper _journalHelperAfterNav = null;
        private JournalHelper _journalHelperAfterGoBack = null;
        private JournalHelper _journalHelperAfterGoForward = null;
        #endregion

        public void Startup(object sender, StartupEventArgs e)
        {
            if (ApplicationDeploymentHelper.GetIEVersion() >= 9)
            {
                NavigationHelper.ExitWithIgnore("Test broken on IE9+ due to changed journal behavior.  Ignore-ing result");
            }
            else
            {
                _testState = CurrentTest.Init;
                NavigationHelper.CreateLog("VerifyIETravelog within xbap");
                NavigationHelper.SetStage(Microsoft.Test.Logging.TestStage.Run);

                // Build proper URI activation path
                if (BrowserInteropHelper.IsBrowserHosted)
                {
                    _activationUri = System.Windows.Interop.BrowserInteropHelper.Source;
                }
                else
                {
                    _activationUri = new Uri(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), UriKind.RelativeOrAbsolute);
                }
                NavigationHelper.Output("Detected activation URI = " + _activationUri);

                Application.Current.StartupUri = new Uri("VerifyIETravelog_Start.xaml", UriKind.RelativeOrAbsolute);

                CreateExpectedStacks();
            }
        }

        public void Navigated(object sender, NavigationEventArgs e)
        {
            switch (_testState)
            {
                case CurrentTest.Init:
                    if (_navWin == null)
                    {
                        _navWin = Application.Current.MainWindow as NavigationWindow;
                        _navWin.ContentRendered += new EventHandler(ContentRendered);
                    }
                    break;
            }
        }

        // you get here after every navigation
        private void ContentRendered(object sender, EventArgs e)
        {
            NavigationHelper.Output("In ContentRendered. State : " + _testState.ToString());

            switch (_testState)
            {

                case CurrentTest.VerifyAfterGoBack1:
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoBack1();
                            return null;
                        }, null);
                    break;

                case CurrentTest.VerifyAfterGoBack2:
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoBack2();
                            return null;
                        }, null);
                    break;

                case CurrentTest.VerifyAfterGoBack3:
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoBack3();
                            return null;
                        }, null);
                    break;

                case CurrentTest.VerifyAfterGoBack4:
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoBack4();
                            return null;
                        }, null);
                    break;

                case CurrentTest.VerifyAfterGoForward1:
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoForward1();
                            return null;
                        }, null);
                    break;

                case CurrentTest.VerifyAfterGoForward2:
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoForward2();
                            return null;
                        }, null);
                    break;

                case CurrentTest.VerifyAfterGoForward3:
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoForward3();
                            return null;
                        }, null);
                    break;

                case CurrentTest.VerifyAfterGoForward4:
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterGoForward4();
                            return null;
                        }, null);
                    break;

                default:
                    NextState(); // move to next state when not verifying
                    break;
            }
        }

        private void CreateExpectedStacks()
        {
            // retrieve the proper localized version for "Current Page"
            string currentPage = BrowserHelper.CurrentPageValue;
            NavigationHelper.Output("Using \"Current Page\" string = " + currentPage);

            string siteofOriginPage = "Page3 ("; // append the rest from activation URI            

            // append to siteofOriginPage first 20 characters of activationUri
            siteofOriginPage += _activationUri.ToString().Substring(0, 20);

            _afterNavStack
                = new String[]{
                        currentPage, 
                        siteofOriginPage, 
                        "Page2 (VerifyIETravelog_Page2.xaml)",
                        "Page1 (Pages/VerifyIETravelog_Page1.xaml)",
                        "Start Page (VerifyIETravelog_Start.xaml)"
                };

            _afterGoBackStack
                 = new String[]{
                        currentPage,
                        siteofOriginPage, 
                        "Page2 (VerifyIETravelog_Page2.xaml)",
                        "Page1 (Pages/VerifyIETravelog_Page1.xaml)",
                        "Start Page (VerifyIETravelog_Start.xaml)"
                };

            _afterGoForwardStack
                = new String[]{
                        currentPage,
                        siteofOriginPage, 
                        "Page2 (VerifyIETravelog_Page2.xaml)",
                        "Page1 (Pages/VerifyIETravelog_Page1.xaml)",
                        "Start Page (VerifyIETravelog_Start.xaml)"
                };

            _backstackAfterNav
                = new String[]{
                        siteofOriginPage, 
                        "Page2 (VerifyIETravelog_Page2.xaml)",
                        "Page1 (Pages/VerifyIETravelog_Page1.xaml)",
                        "Start Page (VerifyIETravelog_Start.xaml)"
                };

            _forwardstackAfterGoBack // has to be in reverse order of afterGoBackStack
                = new String[]{
                        "Page1 (Pages/VerifyIETravelog_Page1.xaml)",
                        "Page2 (VerifyIETravelog_Page2.xaml)",
                        siteofOriginPage, 
                        "Page4 (VerifyIETravelog_Page4.xaml)" 
                };

            _backstackAfterGoForward
                = new String[]{
                        siteofOriginPage, 
                        "Page2 (VerifyIETravelog_Page2.xaml)",
                        "Page1 (Pages/VerifyIETravelog_Page1.xaml)",
                        "Start Page (VerifyIETravelog_Start.xaml)"
                };
        }

        // perform the navigation
        private void NextState()
        {
            switch (_testState)
            {
                // navigate to a compiled XAML in a directory
                case CurrentTest.Init:
                    Thread.Sleep(1000);
                    _testState = CurrentTest.NavToCompiledXamlInDirectory;
                    _navWin.Navigate(new Uri(@"pack://application:,,,/Pages/VerifyIETravelog_Page1.xaml", UriKind.RelativeOrAbsolute));
                    break;

                // navigate to a compiled XAML at root
                case CurrentTest.NavToCompiledXamlInDirectory:
                    Thread.Sleep(1000);
                    _testState = CurrentTest.NavToCompiledXaml;
                    _navWin.Navigate(new Uri(@"..\VerifyIETravelog_Page2.xaml", UriKind.RelativeOrAbsolute));
                    break;

                // navigate to a loose xaml page
                case CurrentTest.NavToCompiledXaml:
                    Thread.Sleep(1000);
                    _testState = CurrentTest.NavToXamlAtSiteOfOriginInDir;
                    _navWin.Navigate(new Uri(@"pack://siteoforigin:,,,/LoosePages/VerifyIETravelog_Page3.xaml", UriKind.RelativeOrAbsolute));
                    break;

                // navigate to a compiled xaml at root
                case CurrentTest.NavToXamlAtSiteOfOriginInDir:
                    Thread.Sleep(1000);
                    _testState = CurrentTest.VerifyAfterNav;
                    _navWin.Navigate(new Uri(@"..\VerifyIETravelog_Page4.xaml", UriKind.RelativeOrAbsolute));
                    break;

                // verify after navigations
                case CurrentTest.VerifyAfterNav:
                    Thread.Sleep(1000);
                    _journalHelperAfterNav = new JournalHelper();
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                        (DispatcherOperationCallback)delegate(object obj)
                        {
                            VerifyAfterNav();
                            return null;
                        }, null);
                    break;

                // done. cleanup and exit
                case CurrentTest.End:
                    NavigationHelper.Pass("VerifyIETravelog test passes");
                    NavigationHelper.SetStage(TestStage.Cleanup);
                    break;

            }
        }

        // verify after page navigations are complete
        private void VerifyAfterNav()
        {
            NavigationHelper.Output("In VerifyAfterNav");

            // verify travelog, forward, back button states, window title
            Thread.Sleep(4000);

            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, false, _backstackAfterNav,
                null, "Page4", _journalHelperAfterNav))
            {
                NavigationHelper.Output("Journal State AfterNav verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State AfterNav verification failed");
            }

            // stable enough only for Vista because we cannot identify the browser idle state 
            if (Environment.OSVersion.Version.Major == 6)
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, false, _afterNavStack, "Page4 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("IE7BrowserState verified");
                }
                else
                {
                    NavigationHelper.Fail("IE7BrowserState verification failed");
                }
            }

            Thread.Sleep(3000);
            GoBack();

            _testState = CurrentTest.VerifyAfterGoBack1;
        }

        // click go back button
        private void GoBack()
        {
            // Move and click the mouse away from "recent pages" button
            // Otherwise click on Forward wouldn't succeed
            BrowserHelper.ClickCenterOfBrowser();

            // Invoke the browser back button
            BrowserHelper.GetMainBrowserWindow(Application.Current).WaitWhileBusy();
            NavigationHelper.Output("Invoking the Browser Back button");
            BrowserHelper.InvokeGoBackButton();
            BrowserHelper.GetMainBrowserWindow(Application.Current).WaitWhileBusy();
            NavigationHelper.Output("Successfully invoked the Browser Back button");
        }

        // click go forward button
        private void GoForward()
        {
            // Move and click the mouse away from "recent pages" button
            // Otherwise click on Forward wouldn't succeed
            BrowserHelper.ClickCenterOfBrowser();

            BrowserHelper.GetMainBrowserWindow(Application.Current).WaitWhileBusy();
            NavigationHelper.Output("Invoking the Browser Forward button");
            BrowserHelper.InvokeGoForwardButton();
            NavigationHelper.Output("Successfully invoked the Browser Forward button");
            BrowserHelper.GetMainBrowserWindow(Application.Current).WaitWhileBusy();
        }

        // verify after invoking the GoBack button 
        private void VerifyAfterGoBack1()
        {
            NavigationHelper.Output("In VerifyAfterGoBack1");
            Thread.Sleep(1000);

            _journalHelperAfterGoBack = new JournalHelper();
            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, true, null,
                null, "Page3", _journalHelperAfterGoBack))
            {
                NavigationHelper.Output("Journal State verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6)
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, true, null, "Page3 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("IE7BrowserState verified");
                }
                else
                {
                    NavigationHelper.Fail("IE7BrowserState verification failed");
                }
            }

            // invoke the back button
            GoBack();

            _testState = CurrentTest.VerifyAfterGoBack2;
        }

        // verify after invoking the GoBack button for the second time 
        private void VerifyAfterGoBack2()
        {
            NavigationHelper.Output("In VerifyAfterGoBack2");
            Thread.Sleep(1000);

            _journalHelperAfterGoBack = new JournalHelper();
            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, true, null,
                null, "Page2", _journalHelperAfterGoBack))
            {
                NavigationHelper.Output("Journal State verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6)
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, true, null, "Page2 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("IE7BrowserState verified");
                }
                else
                {
                    NavigationHelper.Fail("IE7BrowserState verification failed");
                }
            }

            // invoke the back button
            GoBack();

            _testState = CurrentTest.VerifyAfterGoBack3;
        }

        // verify after invoking the GoBack button for the third time 
        private void VerifyAfterGoBack3()
        {
            NavigationHelper.Output("In VerifyAfterGoBack3");
            // verify travelog, forward, back button states, window title
            Thread.Sleep(1000);

            _journalHelperAfterGoBack = new JournalHelper();
            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, true, null,
                null, "Page1", _journalHelperAfterGoBack))
            {
                NavigationHelper.Output("Journal State verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6)
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, true, null, "Page1 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("IE7BrowserState verified");
                }
                else
                {
                    NavigationHelper.Fail("IE7BrowserState verification failed");
                }
            }

            // invoke the back button
            GoBack();

            _testState = CurrentTest.VerifyAfterGoBack4;
        }

        // verify after invoking the GoBack button for the fourth time 
        private void VerifyAfterGoBack4()
        {
            NavigationHelper.Output("In VerifyAfterGoBack4");
            // verify travelog, forward, back button states, window title
            Thread.Sleep(4000);

            _journalHelperAfterGoBack = new JournalHelper();
            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(false, true, null,
                _forwardstackAfterGoBack, "Start Page", _journalHelperAfterGoBack))
            {
                NavigationHelper.Output("Journal State verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6)
            {
                if (NavigationUtilities.VerifyIE7BrowserState(false, true, _afterGoBackStack, "Start Page - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("IE7BrowserState verified (Checking \"afterGoBackStack\") ");
                }
                else
                {
                    NavigationHelper.Output("First Check of \"afterGoBackStack\") failed, trying second possible version");
                    // Workaround: IE8 has nondeterministic journal behavior and this isn't changing...  
                    // So give it a second chance with the page's actual name.
                    _afterGoBackStack[0] = "Page4 (VerifyIETravelog_Page4.xaml)";
                    if (NavigationUtilities.VerifyIE7BrowserState(false, true, _afterGoBackStack, "Start Page - Windows Internet Explorer"))
                    {
                        NavigationHelper.Output("IE7BrowserState verified (Checking \"afterGoBackStack\" version 2) ");
                    }
                    else
                    {
                        NavigationHelper.Fail("IE7BrowserState verification failed (Checking \"afterGoBackStack\") ");
                    }
                }
            }

            Thread.Sleep(3000);
            // invoke the forward button
            GoForward();

            _testState = CurrentTest.VerifyAfterGoForward1;
        }

        // verify after invoking the GoForward button for the first time
        private void VerifyAfterGoForward1()
        {
            NavigationHelper.Output("In VerifyAfterGoForward1");
            Thread.Sleep(1000);

            _journalHelperAfterGoForward = new JournalHelper();
            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, true, null,
                null, "Page1", _journalHelperAfterGoForward))
            {
                NavigationHelper.Output("Journal State verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6)
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, true, null, "Page1 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("IE7BrowserState verified");
                }
                else
                {
                    NavigationHelper.Fail("IE7BrowserState verification failed");
                }
            }

            // invoke the forward button
            GoForward();

            _testState = CurrentTest.VerifyAfterGoForward2;
        }

        // verify after invoking the GoForward button for the second time
        private void VerifyAfterGoForward2()
        {
            NavigationHelper.Output("In VerifyAfterGoForward2");
            Thread.Sleep(1000);

            _journalHelperAfterGoForward = new JournalHelper();
            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, true, null,
                null, "Page2", _journalHelperAfterGoForward))
            {
                NavigationHelper.Output("Journal State verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6)
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, true, null, "Page2 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("IE7BrowserState verified");
                }
                else
                {
                    NavigationHelper.Fail("IE7BrowserState verification failed");
                }
            }

            // invoke the forward button
            GoForward();

            _testState = CurrentTest.VerifyAfterGoForward3;
        }

        // verify after invoking the GoForward button for the third time
        private void VerifyAfterGoForward3()
        {
            NavigationHelper.Output("In VerifyAfterGoForward3");
            Thread.Sleep(1000);

            _journalHelperAfterGoForward = new JournalHelper();
            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, true, null,
                null, "Page3", _journalHelperAfterGoForward))
            {
                NavigationHelper.Output("Journal State verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6)
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, true, null, "Page3 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("IE7BrowserState verified");
                }
                else
                {
                    NavigationHelper.Fail("IE7BrowserState verification failed");
                }
            }

            // invoke the forward button
            GoForward();

            _testState = CurrentTest.VerifyAfterGoForward4;
        }

        // verify after invoking the GoForward button for the fourth time
        private void VerifyAfterGoForward4()
        {
            NavigationHelper.Output("In VerifyAfterGoForward4");
            Thread.Sleep(4000);

            _journalHelperAfterGoForward = new JournalHelper();
            // verify the expected journal properties against the properties retrieved from NavigationWindow
            if (NavigationUtilities.VerifyJournalEntries(true, false, _backstackAfterGoForward,
                null, "Page4", _journalHelperAfterGoForward))
            {
                NavigationHelper.Output("Journal State verified");
            }
            else
            {
                NavigationHelper.Fail("Journal State verification failed");
            }

            if (Environment.OSVersion.Version.Major == 6)
            {
                if (NavigationUtilities.VerifyIE7BrowserState(true, false, _afterGoForwardStack, "Page4 - Windows Internet Explorer"))
                {
                    NavigationHelper.Output("IE7BrowserState verified");
                }
                else
                {
                    NavigationHelper.Output("First Check of \"afterGoForwardStack\") failed, trying second possible version");
                    // Workaround: IE8 has nondeterministic journal behavior and this isn't changing...  
                    // So give it a second chance with the page's actual name.
                    _afterGoForwardStack[0] = "Page4 (VerifyIETravelog_Page4.xaml)";
                    if (NavigationUtilities.VerifyIE7BrowserState(true, false, _afterGoForwardStack, "Page4 - Windows Internet Explorer"))
                    {
                        NavigationHelper.Output("IE7BrowserState verified (Checking \"afterGoForwardStack\" version 2) ");
                    }
                    else
                    {
                        NavigationHelper.Fail("IE7BrowserState verification failed (Checking \"afterGoForwardStack\") ");
                    }
                }
            }

            // done
            _testState = CurrentTest.End;
            NextState();
        }

        public void DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Output("State = " + _testState);

            e.Handled = true;
            NavigationHelper.Fail("Failed due to dispatcher exception");
        }
    }
}
