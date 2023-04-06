// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define TRACE

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // ForwardBackStacks

    /// <summary>
    /// BVT for testing ForwardStack and BackStack DPs
    /// We test N = 20 pages for Navigate(), GoBack and GoForward
    /// operations
    /// This BVT also verifies that the ForwardStack and BackStack properties
    /// hold and return beyond 9 items (even though only 9 are displayed on
    /// the drop down menus)
    /// Tests:
    /// 1) Blank test
    ///    Clear State (ie remove all back entries and forward entries)
    ///    Verify that CanGoBack and CanGoBack are false
    ///    Verify that InvalidOperationException is thrown when GoBack is called
    ///    with CanGoBack as false and GoForward is called with CanGoForward as false
    /// 2) N pages test 
    ///    N = 20
    ///    Navigate Pages 0..19
    ///    Verify expected and actual back / forward stacks are same
    ///    Go Back 19 times
    ///    Verify expected and actual back / forward stacks are same
    ///    Go Forward 19 times
    ///    Verify expected and actual back / forward stacks are same
    ///    
    /// </summary>

    public partial class NavigationTests : Application
    {
        NavigationWindow    _forwardBackStacks_navWin = null;
        Application         _forwardBackStacks_navApp = null;
        Frame               _forwardBackStacks_navFrame = null;
        bool                _forwardBackStacks_clickInitiated = true;
        int                 _forwardBackStacks_n = 20;

        enum ForwardBackStacks_CurrentState
        {
            ClearingState,
            UnInited,
            Inited,
            DoBlank,
            VerifyBlank,
            DoNPages,
            DoBackwardN,
            DoForwardN,
            Verify,
            DoNavigateAction
        };

        ForwardBackStacks_CurrentState _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.UnInited;
        ForwardBackStacks_CurrentState _forwardBackStacks_previousState = ForwardBackStacks_CurrentState.UnInited;

        List<Page> _forwardBackStacks_testPages = new List<Page>();
        Stack<VerificationJournalEntry> _forwardBackStacks_expectedBackStack = new Stack<VerificationJournalEntry>();
        Stack<VerificationJournalEntry> _forwardBackStacks_expectedForwardStack = new Stack<VerificationJournalEntry>();

        void ForwardBackStacks_Startup(object sender, StartupEventArgs e)
        {
            /*
             * TraceListener tr = new TextWriterTraceListener("xamlAD.txt");
             * Trace.Listeners.Add(tr);
             * Trace.AutoFlush = true;
             * */

            NavigationHelper.CreateLog("ForwardBackStacks");

            _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.Inited;

            _forwardBackStacks_navApp = Application.Current;
            _forwardBackStacks_navApp.StartupUri = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);

            //ForwardBackStacks_navApp.LoadCompleted += new LoadCompletedEventHandler(ForwardBackStacks_LoadCompleted);
            _forwardBackStacks_navApp.Navigated += new NavigatedEventHandler(ForwardBackStacks_Navigated);
            _forwardBackStacks_navApp.Exit += new ExitEventHandler(ForwardBackStacks_Exit);

            NavigationHelper.SetStage(TestStage.Run);
        }

        void ForwardBackStacks_LoadCompleted(object sender, NavigationEventArgs e)
        {
            ForwardBackStacks_RunTests();
        }

        void ForwardBackStacks_RunTests()
        {
            if (_forwardBackStacks_currentState != ForwardBackStacks_CurrentState.Inited &&
                !_forwardBackStacks_clickInitiated)
            {
                return;
            }

            NavigationHelper.Output("Current State: " + _forwardBackStacks_currentState
                                + "; Previous State: " + _forwardBackStacks_previousState);

            switch (_forwardBackStacks_currentState)
            {
                case ForwardBackStacks_CurrentState.ClearingState:
                    // do nothing
                    break;

                case ForwardBackStacks_CurrentState.Inited:
                    _forwardBackStacks_navWin = _forwardBackStacks_navApp.MainWindow as NavigationWindow;
                    ForwardBackStacks_ClearState();
                    _forwardBackStacks_previousState = _forwardBackStacks_currentState;
                    _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.DoBlank;
                    ForwardBackStacks_CreateWindowContent();
                    break;

                case ForwardBackStacks_CurrentState.DoBlank:
                    _forwardBackStacks_previousState = _forwardBackStacks_currentState;
                    NavigationHelper.Output("DoBlank start");
                    ForwardBackStacks_ClearState();
                    NavigationHelper.Output("DoBlank end");
                    _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.VerifyBlank;
                    ForwardBackStacks_RunTests();
                    break;

                case ForwardBackStacks_CurrentState.VerifyBlank:
                    ForwardBackStacks_ValidateState(false, false);
                    // expect backstack and forward stack properties to be empty
                    NavigationUtilities.VerifyJournalEntries(_forwardBackStacks_expectedBackStack.GetEnumerator(),
                                                _forwardBackStacks_expectedForwardStack.GetEnumerator(), _forwardBackStacks_navWin);
                    _forwardBackStacks_previousState = _forwardBackStacks_currentState;
                    _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.DoNPages;
                    ForwardBackStacks_RunTests();
                    break;

                case ForwardBackStacks_CurrentState.DoNPages:
                    _forwardBackStacks_previousState = _forwardBackStacks_currentState;
                    ForwardBackStacks_DoNPages(_forwardBackStacks_n);
                    break;

                case ForwardBackStacks_CurrentState.DoBackwardN:
                    _forwardBackStacks_previousState = _forwardBackStacks_currentState;
                    ForwardBackStacks_DoBackwardN(_forwardBackStacks_n);
                    break;

                case ForwardBackStacks_CurrentState.DoForwardN:
                    _forwardBackStacks_previousState = _forwardBackStacks_currentState;
                    ForwardBackStacks_DoForwardN(_forwardBackStacks_n);
                    break;

                case ForwardBackStacks_CurrentState.Verify:
                    bool match = NavigationUtilities.VerifyJournalEntries(_forwardBackStacks_expectedBackStack.GetEnumerator(),
                                                            _forwardBackStacks_expectedForwardStack.GetEnumerator(), _forwardBackStacks_navWin);

                    if (_forwardBackStacks_previousState == ForwardBackStacks_CurrentState.DoNPages)
                    {
                        _forwardBackStacks_previousState = _forwardBackStacks_currentState;
                        _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.DoBackwardN;
                        ForwardBackStacks_RunTests();
                    }
                    else if (_forwardBackStacks_previousState == ForwardBackStacks_CurrentState.DoBackwardN)
                    {
                        _forwardBackStacks_previousState = _forwardBackStacks_currentState;
                        _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.DoForwardN;
                        ForwardBackStacks_RunTests();
                    }
                    else if (_forwardBackStacks_previousState == ForwardBackStacks_CurrentState.DoForwardN)
                    {
                        if (match)
                        {
                            NavigationHelper.Pass("Expected and actual back/forward stacks matched");
                        }
                        else
                        {
                            NavigationHelper.Fail("Expected and actual back/forward stacks didn't match!!!");
                        } 
                    }
                    break;

                case ForwardBackStacks_CurrentState.DoNavigateAction:
                    _currentActionIndex++;
                    if ((_forwardBackStacks_previousState == ForwardBackStacks_CurrentState.DoNPages && 
                        _currentActionIndex == _forwardBackStacks_n - 1) ||
                        (_forwardBackStacks_previousState == ForwardBackStacks_CurrentState.DoBackwardN && 
                        _currentActionIndex == _forwardBackStacks_n - 2) ||
                        (_forwardBackStacks_previousState == ForwardBackStacks_CurrentState.DoForwardN && 
                        _currentActionIndex == _forwardBackStacks_n - 2))
                    {
                        _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.Verify;
                    }
                    ForwardBackStacks_DoCurrentAction();
                    break;
            }
        }

        void ForwardBackStacks_Navigated(object sender, NavigationEventArgs e)
        {
            if (_forwardBackStacks_currentState == ForwardBackStacks_CurrentState.ClearingState)
            {
                NavigationHelper.Output("Navigated fired via ClearState");
            }

            NavigationHelper.Output("ForwardBackStacks_Navigated: Sender = " + sender + "; Uri " + e.Uri
            + "; Content = " + e.Content);

            if (e.Content != null)
            {
                FrameworkElement fe = e.Content as FrameworkElement;
                if (fe != null)
                    NavigationHelper.Output("Content Name = " + fe.Name);
            }
        }

        void ForwardBackStacks_ClearState()
        {
            NavigationHelper.Output("ClearState start");
            ForwardBackStacks_CurrentState oldState = _forwardBackStacks_currentState;
            _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.ClearingState;

            if (_forwardBackStacks_navWin != null)
            {
                NavigationHelper.Output("navWin.Content = " + _forwardBackStacks_navWin.Content);
                NavigationHelper.Output("BackStackProperty = " + NavigationWindow.BackStackProperty);
                NavigationHelper.Output("ForwardStackProperty = " + NavigationWindow.ForwardStackProperty);

                // since we don't have a Add / Remove Forward Entry for obv reasons
                // to clear the forward stack we go all the way forward
                // then we clear the back stack

                int fwdCount = 0;
                NavigationHelper.Output("Initial canGoForward = " + _forwardBackStacks_navWin.CanGoForward);
                while (_forwardBackStacks_navWin.CanGoForward)
                {
                    _forwardBackStacks_navWin.GoForward();
                    ++fwdCount;
                    if (fwdCount < 0)
                    {
                        // overflow, ouch !!
                        break;
                    }
                }
                NavigationHelper.Output("Forward Count = " + fwdCount);

                int bkCount = 0;
                NavigationHelper.Output("Initial canGoBack = " + _forwardBackStacks_navWin.CanGoBack);
                while (_forwardBackStacks_navWin.CanGoBack)
                {
                    JournalEntry je = _forwardBackStacks_navWin.RemoveBackEntry();
                    NavigationHelper.Output("Removed entry name = " + je.Name
                                    + " source = " + je.Source
                                    + " je = " + je);
                    bkCount++;
                    if (bkCount < 0)
                    {
                        // overflow, hmm
                        break;
                    }
                }
                NavigationHelper.Output("Backward Count = " + bkCount);

                _forwardBackStacks_testPages.Clear();
                _forwardBackStacks_expectedBackStack.Clear();
                _forwardBackStacks_expectedForwardStack.Clear();
            }

            _forwardBackStacks_currentState = oldState;
            NavigationHelper.Output("ClearState end");
        }

        void ForwardBackStacks_CreateTestPages(int n)
        {
            for (int i = 0; i < n; i++)
            {
                Page p = new Page();
                p.Name = "Page" + i;
                p.SetValue(JournalEntry.NameProperty, p.Name);
                TextBlock t = new TextBlock();
                t.Text = "Page" + i;
                t.SetValue(DockPanel.DockProperty, Dock.Top);
                p.Content = t;
                _forwardBackStacks_testPages.Add(p);
            }
        }

        void ForwardBackStacks_CreateWindowContent()
        {
            _forwardBackStacks_navFrame = new Frame();
            _forwardBackStacks_navWin = Application.Current.MainWindow as NavigationWindow;

            if (_forwardBackStacks_navWin == null)
            {
                NavigationHelper.Fail("navWin is null");
            }

            DockPanel dp = new DockPanel();
            Button b = new Button();
            b.Content = "Proceed to next test";
            b.Click += new RoutedEventHandler(ForwardBackStacks_BtnClick);
            b.SetValue(DockPanel.DockProperty, Dock.Top);
            _forwardBackStacks_navFrame.SetValue(DockPanel.DockProperty, Dock.Bottom);
            dp.Children.Add(b);
            dp.Children.Add(_forwardBackStacks_navFrame);
            NavigationHelper.Output("navWin.Content = " + _forwardBackStacks_navWin.Content);
            _forwardBackStacks_navWin.Content = dp;
        }

        void ForwardBackStacks_BtnClick(object sender, RoutedEventArgs e)
        {
            NavigationHelper.Output("Button Clicked");
            _forwardBackStacks_clickInitiated = true;
            ForwardBackStacks_RunTests();
        }

        void ForwardBackStacks_DoBackwardN(int n)
        {
            StringBuilder backString = new StringBuilder();
            for (int i = 0; i < _forwardBackStacks_n - 1; i++)
            {
                if (i < _forwardBackStacks_n - 2)
                {
                    backString.Append("b,");
                }
                else
                {
                    backString.Append("b");
                }
            }
            // navigated N pages | should be able to go back N-1 times
            NavigationHelper.Output("Navigating back " + (_forwardBackStacks_n - 1) + " times; actionString = " + backString);
            _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.DoNavigateAction;
            ForwardBackStacks_InitiateNavigateActionSequence(backString.ToString());
        }

        void ForwardBackStacks_DoForwardN(int n)
        {
            StringBuilder forwardString = new StringBuilder();
            for (int i = 0; i < n - 1; i++)
            {
                if (i < n - 2)
                {
                    forwardString.Append("f,");
                }
                else
                {
                    forwardString.Append("f");
                }
            }
            // navigated back N-1 times | should be able to go forward N-1 times
            _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.DoNavigateAction;
            NavigationHelper.Output("Navigating forward " + (n - 1) + " times; actionString = " + forwardString);
            ForwardBackStacks_InitiateNavigateActionSequence(forwardString.ToString());
        }

        void ForwardBackStacks_DoNPages(int n)
        {
            NavigationHelper.Output("TestN start");
            ForwardBackStacks_ClearState();
            ForwardBackStacks_CreateTestPages(n);
            _forwardBackStacks_navWin.Title = "Test" + n;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < n; i++)
            {
                if (i < n - 1)
                    sb.Append(i + ",");
                else
                    sb.Append(i);
            }
            // navigated to N pages
            NavigationHelper.Output("Navigating from pages 0 to " + (n - 1) + " actionString = " + sb);

            _forwardBackStacks_currentState = ForwardBackStacks_CurrentState.DoNavigateAction;
            ForwardBackStacks_InitiateNavigateActionSequence(sb.ToString());
            NavigationHelper.Output("TestN end");
        }

        int _currentActionIndex = 0;
        String[] _actions;
        String _currentAction = String.Empty;

        void ForwardBackStacks_DoCurrentAction()
        {
            NavigationHelper.Output("currentActionIndex = " + _currentActionIndex);
            _currentAction = _actions[_currentActionIndex].Trim();
            if (String.IsNullOrEmpty(_currentAction))
            {
                return;
            }

            switch (_currentAction)
            {
                case "f":
                case "F":
                    NavigationHelper.Output("Start Action = forward");
                    VerificationJournalEntry backEntry = new VerificationJournalEntry();
                    backEntry.Name = (_forwardBackStacks_navFrame.Content as FrameworkElement).GetValue(JournalEntry.NameProperty) as String;
                    _forwardBackStacks_expectedBackStack.Push(backEntry);
                    _forwardBackStacks_expectedForwardStack.Pop();
                    _forwardBackStacks_navWin.GoForward();
                    NavigationHelper.Output("End Action = forward");
                    break;

                case "b":
                case "B":
                    NavigationHelper.Output("Start Action = back");
                    VerificationJournalEntry forwardEntry = new VerificationJournalEntry();
                    forwardEntry.Name = (_forwardBackStacks_navFrame.Content as FrameworkElement).GetValue(JournalEntry.NameProperty) as String;
                    _forwardBackStacks_expectedForwardStack.Push(forwardEntry);
                    _forwardBackStacks_expectedBackStack.Pop();
                    _forwardBackStacks_navWin.GoBack();
                    NavigationHelper.Output("End Action = back");
                    break;

                default:
                    NavigationHelper.Output("Start Action = nav To Page" + _currentAction);
                    VerificationJournalEntry navEntry = new VerificationJournalEntry();
                    Page previousPage = _forwardBackStacks_navFrame.Content as Page;
                    if (previousPage != null)
                    {
                        navEntry.Name = previousPage.GetValue(JournalEntry.NameProperty) as String;
                        _forwardBackStacks_expectedBackStack.Push(navEntry);
                    }
                    _forwardBackStacks_expectedForwardStack.Clear();
                    _forwardBackStacks_navFrame.Navigate(_forwardBackStacks_testPages[Int32.Parse(_currentAction)]);
                    NavigationHelper.Output("End Action = nav To Page" + _currentAction);
                    break;
            }
        }

        void ForwardBackStacks_InitiateNavigateActionSequence(String actionString)
        {
            if (String.IsNullOrEmpty(actionString))
            {
                return;
            }
            _actions = actionString.Split(',');
            _currentActionIndex = 0;
            _currentAction = _actions[_currentActionIndex].Trim();
            ForwardBackStacks_DoCurrentAction();
        }

        void ForwardBackStacks_ValidateState(bool backPossible, bool forwardPossible)
        {
            if (_forwardBackStacks_navWin == null)
            {
                return;
            }

            if (_forwardBackStacks_navWin.CanGoBack == backPossible &&
                _forwardBackStacks_navWin.CanGoForward == forwardPossible)
            {
                NavigationHelper.Output("Values match");
            }
            else
            {
                NavigationHelper.Fail("Values Match Failed. CanGoBack = " + _forwardBackStacks_navWin.CanGoBack
                                + " CanGoForward = " + _forwardBackStacks_navWin.CanGoForward);
            }

            // we shouldn't be able to go forward / back when both
            // cangoforward and cangoback are false

            try
            {
                if (!_forwardBackStacks_navWin.CanGoForward)
                {
                    _forwardBackStacks_navWin.GoForward();
                    NavigationHelper.Fail("Test failed. No IOE when trying to go forward CanGoForward = " + _forwardBackStacks_navWin.CanGoForward);
                }
            }
            catch (InvalidOperationException)
            {
                NavigationHelper.Output("Test pass. IOE when trying to go forward CanGoForward = " + _forwardBackStacks_navWin.CanGoForward);
            }

            try
            {
                if (!_forwardBackStacks_navWin.CanGoBack)
                {
                    _forwardBackStacks_navWin.GoBack();
                    NavigationHelper.Fail("Test failed. No IOE when trying to go back CanGoBack = " + _forwardBackStacks_navWin.CanGoBack);
                }
            }
            catch (InvalidOperationException)
            {
                NavigationHelper.Output("Test pass. IOE when trying to go back CanGoBack = " + _forwardBackStacks_navWin.CanGoBack);
            }
        }

        void ForwardBackStacks_Exit(object sender, ExitEventArgs e)
        {
            NavigationHelper.SetStage(TestStage.Cleanup);
            NavigationHelper.Output("OnExit");
        }
    }
}
