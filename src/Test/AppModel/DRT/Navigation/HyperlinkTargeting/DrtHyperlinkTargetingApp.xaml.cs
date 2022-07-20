// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Reflection;

using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Markup;

namespace DrtHyperlinkTargeting
{
    public partial class DrtHyperlinkTargetingApp : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Log(@"DrtHyperlinkTargeting [AppModel\Microsoft]");

            LoadCompleted       += new LoadCompletedEventHandler(this.OnLoadCompleted);

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_success)
                Log("Test PASSED");
            else
                Log("Test FAILED");

            base.OnExit(e);
        }

        private void OnLoadCompleted(object source, NavigationEventArgs e)
        {
            if (e.IsNavigationInitiator == false)
                return;

            switch (_state)
            {
                case State.StartupUriLoaded:
                    {
                        //Load up both windows first since both have the same targets and the hyperlink targetting
                        //code should target the first window found
                        _startupWindow = e.Navigator as NavigationWindow;
                        VerifyState(_startupWindow != null, "Could not get Startup Window, LoadCompleted event not firing correctly?");
                        _newWindow = new NavigationWindow();
                        _newWindow.Name = "NewWindow";
                        _newWindow.Show();
                        _newWindow.Navigate(new Uri("FramesTwo.xaml", UriKind.RelativeOrAbsolute));
                        _state = State.NewWindowLoaded;
                        break;
                    }
                case State.NewWindowLoaded:
                    {
                        ClickHyperlink("HLinkOne");
                        _state = State.TargetInSameWindowNavigated;
                        break;
                    }
                case State.TargetInSameWindowNavigated:
                    {
                        Frame frame = e.Navigator as Frame;
                        VerifyState(frame != null, "Navigation should have occured in a frame");

                        NavigationWindow targetWindow = Window.GetWindow(frame) as NavigationWindow;
                        VerifyState(targetWindow == _startupWindow, "Wrong window used for Hyperlink Targeting");

                        ClickHyperlink("HLinkTwo");
                        _state = State.TargetInNewWindowFrameNavigated;
                        break;
                    }
                case State.TargetInNewWindowFrameNavigated:
                    {
                        Frame frame = e.Navigator as Frame;
                        NavigationWindow targetWindow = Window.GetWindow(frame) as NavigationWindow;
                        VerifyState(targetWindow == _newWindow, "Wrong window used for Hyperlink Targeting");

                        ClickHyperlink("HLinkThree");
                        _state = State.TargetInNewWindowNavigated;
                        break;
                    }
                case State.TargetInNewWindowNavigated:
                    {
                        NavigationWindow targetWindow = e.Navigator as NavigationWindow;
                        VerifyState(targetWindow == _newWindow, "New window should have been used for Hyperlink Targeting");

                        ClickHyperlink("HLinkFour");
                        _state = State.EmptyTargetNavigated;
                        break;
                    }
                case State.EmptyTargetNavigated:
                    {
                        NavigationWindow targetWindow = e.Navigator as NavigationWindow;
                        VerifyState(targetWindow == _startupWindow, "Same (startup)window should have been used for Hyperlink Targeting");
                        Shutdown(0);
                        break;
                    }
            }
        }

        private void ClickHyperlink(string linkID)
        {
            Hyperlink hlink = LogicalTreeHelper.FindLogicalNode((DependencyObject)_startupWindow.Content, linkID) as Hyperlink;
            VerifyState(hlink != null, "Could not find hyperlink element to invoke click");

            MethodInfo info = typeof(Hyperlink).GetMethod("OnClick", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            VerifyState(info != null, "Could not find Hyperlink's OnClick method");

            info.Invoke(hlink, new object[] {});
        }

        private void VerifyState(bool checkPassed, string errorMessage)
        {
            if (!checkPassed)
            {
                Log(errorMessage);
                _success = false;
                Shutdown(-1);
                throw new ApplicationException("ERROR: " + errorMessage);
            }
        }

        private void Log(string message, params object[] args)
        {
            _logger.Log(message, args);
        }

        private void VerboseLog(string message, params object[] args)
        {
            if (_verbose)
                Log(message, args);
        }

        private State   _state      = 0;
        private bool    _verbose    = false;
        private bool    _success    = true;

        private NavigationWindow _startupWindow  = null;
        private NavigationWindow _newWindow      = null;

        private DRT.Logger _logger  = new DRT.Logger("DrtHyperlinkTargeting", "Microsoft", "Testing Hyperlink Targeting in multiple windows");

        private enum State
        {
            StartupUriLoaded,
            NewWindowLoaded,
            TargetInSameWindowNavigated,
            TargetInNewWindowFrameNavigated,
            TargetInNewWindowNavigated,
            EmptyTargetNavigated,
        }
    }
}
