// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Data;
using System.Xml;
using System.Configuration;
using System.Windows.Controls;
using Microsoft.Test;
using System.Windows.Documents;
using Microsoft.Test.Logging;
using MTI = Microsoft.Test.Input;
using System.Windows.Threading;
using System.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// BVT for WebOC navigation 
    /// Also override Application Navigation protected virtuals
    /// Navigate NavWin to Google
    /// Navigate NavWin to HTML at site of origin    
    /// Navigate Frame to Google
    /// Navigate Frame to HTML at site of origin    
    /// Cancel WebOC navigation in a frame
    /// Refresh WebOC frame

    public class WebOCGeneral
    {
        private NavigationWindow _navWin = null;
        private Frame _frame = null;

        private int _expectedNavigatedCount = 9;
        private int _expectedNavigatingCount = 9;
        private int _expectedContentRenderedCount = 9;

        private int _actualNavigatedCount = 0;
        private int _actualContentRenderedCount = 0;
        private int _actualNavigatingCount = 0;
        private int _actualLoadCompletedCount = 0;

        private enum State
        {
            Init,
            GoogleNavWin,
            GoBack,
            SiteOfOriginHTMLNavWin,
            GoogleFrame,
            SiteOfOriginHTMLFrame,
            CancelWebOCNavigation,
            RefreshFrameWebOC,
            XamlFrame,
            End
        }

        private State _currentState = State.Init;
        private State _previousState = State.Init;

        private String _google = "http://www.google.com/";
        private String _htmlSiteOfOrigin = "pack://siteoforigin:,,,/WebPage1_Loose.html";
        private String _xaml = "pack://siteoforigin:,,,/Page1_Loose.xaml";

        private Hyperlink _hlink = null;
        private NavigationService _ns = null;


        public void Startup(object sender, StartupEventArgs e)
        {
            if (Log.Current == null)
            {
                new TestLog("WebOC Tests");
            }

            NavigationHelper.SetStage(TestStage.Run);
            Application.Current.StartupUri = new Uri("WebOCGeneral_Page1.xaml", UriKind.RelativeOrAbsolute);
        }

        private void InvokeHyperlink(String id)
        {
            _hlink = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, id) as Hyperlink;
            _hlink.DoClick();
        }

        private void VerifySource(String actualSrc)
        {
            String expectedSrc = String.Empty;
            if (_currentState == State.GoogleNavWin || _currentState == State.GoogleFrame
                || _currentState == State.RefreshFrameWebOC)
            {
                expectedSrc = _google;
            }
            else if (_currentState == State.SiteOfOriginHTMLFrame || _currentState == State.SiteOfOriginHTMLNavWin)
            {
                expectedSrc = _htmlSiteOfOrigin;
            }
            else if (_currentState == State.XamlFrame)
            {
                expectedSrc = _xaml;
            }

            Log.Current.CurrentVariation.LogMessage("Actual src = " + actualSrc + " Expected src = " + expectedSrc);
            if (actualSrc.Equals(expectedSrc))
            {
                NavigationHelper.CacheTestResult(Result.Pass);
            }
            else
            {
                Log.Current.CurrentVariation.LogMessage("Actual & expected sources didn't match");
                NavigationHelper.CacheTestResult(Result.Fail);
            }
        }


        public void Navigated(object sender, NavigationEventArgs e)
        {
            ++_actualNavigatedCount;
            switch (_currentState)
            {
                case State.Init:
                    _navWin = Application.Current.MainWindow as NavigationWindow;
                    _navWin.ContentRendered += new EventHandler(NavWinContentRendered);
                    _navWin.Navigating += new NavigatingCancelEventHandler(NavWinNavigating);
                    break;
            }
        }

        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("LoadCompleted State = " + _currentState);
            ++_actualLoadCompletedCount;
            DependencyObject content = null;

            if (e.Navigator is Frame)
            {
                Log.Current.CurrentVariation.LogMessage("Frame Navigation to " + e.Uri);
                content = (e.Navigator as Frame).Content as DependencyObject;
            }
            else if (e.Navigator is NavigationWindow)
            {
                Log.Current.CurrentVariation.LogMessage("NavWin Navigation to " + e.Uri);
                content = (e.Navigator as NavigationWindow).Content as DependencyObject;
            }
            _ns = NavigationService.GetNavigationService(content);

            switch (_currentState)
            {
                case State.CancelWebOCNavigation:
                    // shouldn't get here - since we're cancelling weboc navigation
                    // in navigating
                    Log.Current.CurrentVariation.LogMessage("OnLoadCompleted even though frame navigation was cancelled - test fail");
                    NavigationHelper.CacheTestResult(Result.Fail);
                    break;
            }
        }

        private void RefreshWebOC()
        {
            Thread.Sleep(5000);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                {
                    MTI.Input.SendUnicodeString("Entering some text");
                    return null;
                }, null);
            Thread.Sleep(5000);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                {
                    _ns.Refresh();
                    return null;
                }, null);

        }

        private void FrameContentRendered(object sender, EventArgs e)
        {
            ++_actualContentRenderedCount;

            switch (_currentState)
            {
                case State.SiteOfOriginHTMLFrame:
                    VerifySource(_ns.Source.ToString());
                    _currentState = State.GoogleFrame;
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                        {       
                            InvokeHyperlink("GoogleFrame");
                            return null;
                        }, null);
                    break;

                case State.GoogleFrame:
                    VerifySource(_ns.Source.ToString());
                    _currentState = State.RefreshFrameWebOC;
                    Thread t = new Thread(new ThreadStart(RefreshWebOC));
                    t.Start();
                    break;

                case State.RefreshFrameWebOC:
                    VerifySource(_ns.Source.ToString());
                    _currentState = State.CancelWebOCNavigation;
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                        {      
                            InvokeHyperlink("HTMLFrame");
                            return null;
                        }, null);
                    break;

                case State.XamlFrame:
                    VerifySource(_ns.Source.ToString());
                    _currentState = State.End;
                    Log.Current.CurrentVariation.LogMessage("navigated = " + _actualNavigatedCount
                        + " navigating = " + _actualNavigatingCount
                        + " contentrendered = " + _actualContentRenderedCount
                        + " loadCompleted = " + +_actualLoadCompletedCount);
                    if (_actualContentRenderedCount == _expectedContentRenderedCount
                        && _actualNavigatedCount == _expectedNavigatedCount
                        && _actualNavigatingCount == _expectedNavigatingCount)
                    {
                        NavigationHelper.Pass("Event counts match. Test pass");
                    }
                    else
                    {
                        NavigationHelper.Fail("Event did not match. Test fail");
                    }
                    Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
                    break;
            }
        }

        public void Navigating(object sender, NavigatingCancelEventArgs e)
        {
            Log.Current.CurrentVariation.LogMessage("OnNavigating currentState = " + _currentState);
            switch (_currentState)
            {
                case State.CancelWebOCNavigation:
                    Log.Current.CurrentVariation.LogMessage("Cancelling WebOC navigation");
                    e.Cancel = true;
                    _currentState = State.XamlFrame;
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                    {
                        InvokeHyperlink("XamlFrame");
                        return null;
                    }, null);
                    break;

            }
        }

        public void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Output("currentState = " + _currentState);

            // System.Net.WebException can be caught on some machines due to DNS issues
            if (e.Exception is System.Net.WebException)
            {
                NavigationHelper.ExitWithIgnore("Invalid DNS configuration hit. Could not resolve google.com.  Please verify scenario manually.");
            }
            else
            {
                NavigationHelper.Fail("Unexpected exception caught. Test fails");
            }

            e.Handled = true;
            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
        }

        private void FrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
            ++_actualNavigatingCount;
        }

        private void NavWinNavigating(object sender, NavigatingCancelEventArgs e)
        {
            ++_actualNavigatingCount;
        }

        private void NavWinContentRendered(object sender, EventArgs e)
        {
            ++_actualContentRenderedCount;

            switch (_currentState)
            {
                case State.Init:
                    _currentState = State.GoogleNavWin;
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                        {       
                            InvokeHyperlink("GoogleNavWin");
                            return null;
                        }, null);
                    break;

                case State.GoogleNavWin:
                    VerifySource(_ns.Source.ToString());
                    _previousState = _currentState;
                    _currentState = State.GoBack;
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                        {       
                            _navWin.GoBack();
                            return null;
                        }, null);

                    break;

                case State.GoBack:
                    if (_previousState == State.GoogleNavWin)
                    {
                        _currentState = State.SiteOfOriginHTMLNavWin;
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                            {    
                                InvokeHyperlink("HTMLNavWin");
                                return null;
                            }, null);
                    }
                    else if (_previousState == State.SiteOfOriginHTMLNavWin)
                    {
                        _currentState = State.SiteOfOriginHTMLFrame;
                        _frame = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, "frame") as Frame;
                        _frame.Navigating += new NavigatingCancelEventHandler(FrameNavigating);
                        _frame.ContentRendered += new EventHandler(FrameContentRendered);
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                            {     
                                InvokeHyperlink("HTMLFrame");
                                return null;
                            }, null);
                    }
                    break;

                case State.SiteOfOriginHTMLNavWin:
                    VerifySource(_ns.Source.ToString());
                    _previousState = _currentState;
                    _currentState = State.GoBack;
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object o)
                        {     
                            _navWin.GoBack();
                            return null;
                        }, null);

                    break;
            }
        }
    }
}
