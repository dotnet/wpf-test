// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: Tests fragment navigation to objects created programmatically 
//
// 
//  The following is the sequence of events for this test.
//  1) Navigate to to fragment in Hyperlink hlinkframe1
//  2) Set TextBox content to "This is some text"
//  3) Navigate to to fragment in Hyperlink hlinkFrame2
//  4) Check the checkbox
//  5) Go back all the way
//  6) Go forward all the way
//  7) Navigate to to fragment in Hyperlink hlinkFrame3 
//  8) Go back 
//  9) Look at the results.xml file for recorded navigation state at different steps
//     This will be used for verification when actual test is run
//
//

using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Logging;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class FragmentNavigationToObject
    {
        enum State
        {
            Init,
            fragment1,
            textbox,
            hlinkFrame2,
            checkbox,
            GoBackAll,
            GoForwardAll,
            hlinkFrame3,
            GoBack,
            HandleFragmentNavigation,
            End
        }

        private NavigationWindow _navWin = null;
        private TextBox _textBox = null;
        private CheckBox _checkBox = null;
        private Frame _frame1 = null;
        private Frame _frame2 = null;
        private ScrollViewer _frameScrollViewer = null;

        private State _currentState = State.Init;
        private bool _inVerifyMode = true;

        private JournalHelper _journalHelper = null;
        private NavigationStateCollection _actualStates = new NavigationStateCollection();
        private NavigationStateCollection _expectedStates = new NavigationStateCollection();
        private int _currentStateIndex = 0;

        // expected values for NavigationState
        private String[] _stateDescription = null;
        private String[] _windowTitle = null;
        private bool[] _backButtonEnabled = null;
        private bool[] _forwardButtonEnabled = null;
        private String[][] _backStack = null;
        private String[][] _forwardStack = null;

        /// <summary>
        /// FragmentNavigationToObject test entry point
        /// </summary>
        /// <param name="sender">Object that raised startup event (not used)</param>
        /// <param name="e">Event arguments (not used)</param>
        public void Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.SetStage(TestStage.Initialize);

            /*
             * This code no longer works because of the error "cscomp.dll is missing"
             * You may use it when this issue is fixed
            if (inVerifyMode)
            {
                // retrieve the XML files with expected states 
                NavigationHelper.Output("Reading in expected results from XML file [FragmentNavigationToObjectResults_Loose.xml]");
                Stream s = Application.GetRemoteStream(new Uri("pack://siteoforigin:,,,/FragmentNavigationToObjectResults_Loose.xml", UriKind.RelativeOrAbsolute)).Stream;
                expectedStates = NavigationStateCollection.GetResults(s);
            }
             */

            if (_inVerifyMode)
            {
                CreateExpectedNavigationStates();

                // populate the expected navigation states
                for (int index = 0; index < _stateDescription.Length; index++)
                {
                    _expectedStates.states.Add(new NavigationState(_stateDescription[index],
                        _windowTitle[index],
                        _backButtonEnabled[index],
                        _forwardButtonEnabled[index],
                        _backStack[index],
                        _forwardStack[index]));
                }
            }

            // Begin the test
            NavigationHelper.SetStage(TestStage.Run);

            // Create a navigation window and objects
            _navWin = new NavigationWindow();
            _navWin.Title = "FragmentNavigationToObject";

            // Grid contains threee stack panels each one of them contain objects
            Grid grid = new Grid();

            StackPanel stackPanel1 = new StackPanel();
            TextBlock textBlock1 = new TextBlock();
            textBlock1.Name = "hlink_frame1";
            Hyperlink hyperlink1 = new Hyperlink();
            hyperlink1.Name = "hlinkFrame1";
            hyperlink1.NavigateUri = new Uri("#frame1", UriKind.Relative);
            textBlock1.Inlines.Add(hyperlink1);

            TextBlock textBlock2 = new TextBlock();
            textBlock2.Name = "hlink_frame2";
            Hyperlink hyperlink2 = new Hyperlink();
            hyperlink2.Name = "hlinkFrame2";
            hyperlink2.NavigateUri = new Uri("#frame2", UriKind.Relative);
            textBlock2.Inlines.Add(hyperlink2);

            TextBlock textBlock3 = new TextBlock();
            textBlock3.Name = "hlink_frame3";
            Hyperlink hyperlink3 = new Hyperlink();
            hyperlink3.Name = "hlinkFrame3";
            hyperlink3.NavigateUri = new Uri("#frame3", UriKind.Relative);
            textBlock3.Inlines.Add(hyperlink3);

            stackPanel1.Children.Add(textBlock1);
            stackPanel1.Children.Add(textBlock2);
            stackPanel1.Children.Add(textBlock3);

            grid.Children.Add(stackPanel1);

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Name = "FragmentNavigation_frameScrollViewer";

            grid.Children.Add(scrollViewer); 

            StackPanel stackPanel2 = new StackPanel();
            Frame frame1 = new Frame();
            frame1.Name = "frame1";
            frame1.Height = 100;
            frame1.Background = new SolidColorBrush(Colors.Aqua);
            frame1.BorderBrush = new SolidColorBrush(Colors.Blue);
            Frame frame2 = new Frame();
            frame2.Name = "frame2";
            frame2.Height = 100;
            frame2.Background = new SolidColorBrush(Colors.Beige);
            frame2.BorderBrush = new SolidColorBrush(Colors.DarkGray);
            Frame frame3 = new Frame();
            frame3.Name = "frame3";
            frame3.Height = 100;
            frame3.Background = new SolidColorBrush(Colors.DeepPink);
            frame3.BorderBrush = new SolidColorBrush(Colors.Green);
            stackPanel2.Children.Add(frame1);
            stackPanel2.Children.Add(frame2);
            stackPanel2.Children.Add(frame3);

            grid.Children.Add(stackPanel2);

            StackPanel stackPanel3 = new StackPanel();
            stackPanel3.Width = 200;
            stackPanel3.Orientation = Orientation.Vertical;
            TextBox textBox = new TextBox();
            textBox.Name = "textBox";
            CheckBox checkBox = new CheckBox();
            checkBox.Name = "checkBox";
            stackPanel3.Children.Add(checkBox);
            stackPanel3.Children.Add(textBox);
            grid.Children.Add(stackPanel3);

            _navWin.Content = grid;
            _navWin.Show();

        }

        /// <summary>
        /// LoadCompleted Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            _navWin.ContentRendered += new EventHandler(navWin_ContentRendered);
            _journalHelper = new JournalHelper(_navWin);

            if (_currentState != State.GoBack)
            {
                RecordNewResult("LoadCompleted");
                NextTest();
            }
        }

        /// <summary>
        /// ContentRendered event handler 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void navWin_ContentRendered(object sender, EventArgs e)
        {
            if (_currentState == State.Init)
            {
                NavigationHelper.Output("Grabbing all the controls we'll play with later");
                _frameScrollViewer = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject,
                                        "FragmentNavigation_frameScrollViewer") as ScrollViewer;
                if (_frameScrollViewer == null)
                {
                    NavigationHelper.Fail("Couldn't find the object frameScrollViewer");
                }
                _textBox = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, "textBox") as TextBox;
                if (_textBox == null)
                {
                    NavigationHelper.Fail("Couldn't find the object textBox");
                }
                _checkBox = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, "checkBox") as CheckBox;
                if (_checkBox == null)
                {
                    NavigationHelper.Fail("Couldn't find the object checkBox");
                }
                _frame1 = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, "frame1") as Frame;
                if (_frame1 == null)
                {
                    NavigationHelper.Fail("Couldn't find the object frame1");
                }
                _frame2 = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, "frame2") as Frame;
                if (_frame2 == null)
                {
                    NavigationHelper.Fail("Couldn't find the object frame2");
                }
            }

            if (_currentState == State.Init)
            {
                _currentState = State.fragment1;
                NavigateHyperlink("hlinkFrame1");
            }
            else
            {
                NavigationHelper.Fail("FragmentNavigation test fails - Unexpected State");
            }
        }

        public void Navigated(object sender, NavigationEventArgs e)
        {
            if (_journalHelper != null)
            {
                NavigationHelper.Output(_currentState + ": Recording navigation of " + e.Navigator + " to " + e.Uri);
                RecordNewResult("Navigated - Uri = " + e.Uri + " IsNavigationInitiator " + e.IsNavigationInitiator);
            }
        }

        public void FragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            NavigationHelper.Output(_currentState + ": Recording navigation of " + e.Navigator + " to " + e.Fragment);
            RecordNewResult("FragmentNavigationToObject " + e.Fragment);

            if (_currentState == State.HandleFragmentNavigation)
            {
                // FragmentNavigation_frameScrollViewer.ScrollChangedEvent shouldn't get fired
                e.Handled = true;
            }
        }

        void frameScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (_currentState == State.HandleFragmentNavigation)
            {
                // shouldn't get called 
                NavigationHelper.Output("FAIL!!  ScrollChanged was called in HandleFragmentNavigation state");
                NavigationHelper.Fail("FragmentNavigation test fails");
            }
        }

        #region asynchronous navigation helpers
        private void RaiseRequestNavigate(String hyperlinkId)
        {
            Hyperlink hlink = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, hyperlinkId) as Hyperlink;

            NavigationHelper.Output(_currentState + ": Navigating " + hlink.TargetName + " to " + hlink.NavigateUri);
            RequestNavigateEventArgs requestNavigateEventArgs = new RequestNavigateEventArgs(hlink.NavigateUri, hlink.TargetName);
            requestNavigateEventArgs.Source = hlink;
            hlink.RaiseEvent(requestNavigateEventArgs);
        }

        private void RequestNavigation(string linkId)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object ob)
            {
                RaiseRequestNavigate(linkId);
                return null;
            }, null);
        }

        private void NavigateHyperlink(String linkId)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object ob)
            {
                RequestNavigation(linkId);
                return null;
            }, null);
        }

        private void GoBackOnce()
        {
            NavigationHelper.Output(_currentState + ": Calling GoBack on the NavigationWindow");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object ob)
            {
                _navWin.GoBack();
                return null;
            }, null);
        }

        private void GoBack()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object ob)
            {
                GoBackOnce();
                return null;
            }, null);
        }

        private void GoForwardOnce()
        {
            NavigationHelper.Output(_currentState + ": Calling GoForward on the NavigationWindow");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object ob)
            {
                _navWin.GoForward();
                return null;
            }, null);
        }

        private void GoForward()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object ob)
            {
                GoForwardOnce();
                return null;
            }, null);
        }

        private void HandleFragmentNav()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new DispatcherOperationCallback(NavToHlinkFrame1),
                null);
        }

        private Object NavToHlinkFrame1(object arg)
        {
            _currentState = State.HandleFragmentNavigation;
            _frameScrollViewer.ScrollChanged += new ScrollChangedEventHandler(frameScrollViewer_ScrollChanged);
            NavigateHyperlink("hlinkFrame1");
            return null;
        }

        #endregion

        /// <summary>
        /// Function to move between states
        /// </summary>
        private void NextTest()
        {
            switch (_currentState)
            {
                case State.fragment1:
                    _currentState = State.textbox;
                    NavigationHelper.Output(_currentState + ": Changing textBox Text to be 'This is a test'");
                    _textBox.Text = "This is a test";
                    NextTest();
                    break;

                case State.textbox:
                    _currentState = State.hlinkFrame2;
                    NavigateHyperlink("hlinkFrame2");
                    break;

                case State.hlinkFrame2:
                    _currentState = State.checkbox;
                    NavigationHelper.Output(_currentState + ": Changing checkBox to be checked");
                    _checkBox = LogicalTreeHelper.FindLogicalNode(_navWin.Content as DependencyObject, "checkBox") as CheckBox;
                    _checkBox.IsChecked = true;
                    NextTest();
                    break;

                case State.checkbox:
                    _currentState = State.GoBackAll;
                    NextTest();
                    break;

                case State.GoBackAll:
                    NavigationHelper.Output("GoBack to the Startup page");
                    if (_navWin.CanGoBack)
                    {
                        GoBack();
                    }
                    else
                    {
                        _currentState = State.GoForwardAll;
                        NextTest();
                    }
                    break;

                case State.GoForwardAll:
                    NavigationHelper.Output("GoForward to the very last page visited");
                    if (_navWin.CanGoForward)
                    {
                        GoForward();
                    }
                    else
                    {
                        _currentState = State.hlinkFrame3;
                        NavigateHyperlink("hlinkFrame3");
                    }
                    break;

                case State.hlinkFrame3:
                    _currentState = State.GoBack;
                    GoBack();
                    NextTest();
                    break;

                case State.GoBack:
                    NavigationHelper.Output("Checking state of controls.");
                    NavigationHelper.Output("TextBox: " + _textBox.Text);
                    NavigationHelper.Output("CheckBox checked? " + _checkBox.IsChecked);
                    String controlStateInfo = "TextBox text = " + _textBox.Text + " checkBox checked = " + _checkBox.IsChecked;
                    RecordNewResult("State = " + _currentState + " ContentRendered "
                        + " Control State = " + controlStateInfo);

                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, (DispatcherOperationCallback)delegate(object ob)
                    {
                        HandleFragmentNav();
                        return null;
                    }, null);
                    break;

                case State.HandleFragmentNavigation:
                    _currentState = State.End;
                    if (_inVerifyMode)
                    {
                        if (NavigationStateCollection.Compare(_actualStates, _expectedStates))
                        {
                            NavigationHelper.Pass("FragmentNavigationToObject test passes");
                            NavigationHelper.SetStage(TestStage.Cleanup);
                        }
                        else
                        {
                            NavigationHelper.Output("All states did not match - test failed");

                            NavigationHelper.Fail("FragmentNavigationToObject test fails");

                            NavigationHelper.Output("Writing output to isolated storage...");

                            try
                            {
                                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                                {
                                    using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("actualResults.xml", FileMode.CreateNew, storage))
                                    {
                                        _actualStates.WriteResults(stream);
                                    }
                                }

                                NavigationHelper.Output("Actual results written to actualResults.xml in isolated storage.");

                            }
                            catch (IOException)
                            {
                                NavigationHelper.Output("WARNING: Unable to write actual results to isolated storage.");
                            }
                        }
                    }
                    else
                    {
                        _actualStates.WriteResults("results.xml");
                    }
                    break;
            }
        }

        private void RecordNewResult(string logString)
        {
            _actualStates.RecordNewResult(_journalHelper, logString);
            NavigationHelper.Output(String.Format("[{0} - {1}] {2}",
                _currentState,
                (_currentStateIndex++).ToString(CultureInfo.InvariantCulture),
                logString));
        }

        /// <summary>
        /// Create expected navigation states 
        /// </summary>
        private void CreateExpectedNavigationStates()
        {
            _stateDescription
                = new String[]{
                        "LoadCompleted",
                        "FragmentNavigationToObject frame1",
                        "Navigated - Uri = #frame1 IsNavigationInitiator True",
                        "LoadCompleted - #frame1",
                        "FragmentNavigationToObject frame2",
                        "Navigated - Uri = #frame2 IsNavigationInitiator True",
                        "LoadCompleted - #frame2",
                        "FragmentNavigationToObject frame1",
                        "Navigated - Uri = #frame1 IsNavigationInitiator True",
                        "LoadCompleted - #frame1",
                        "Navigated - Uri =  IsNavigationInitiator True",
                        "LoadCompleted",
                        "LoadCompleted",
                        "FragmentNavigationToObject frame1",
                        "Navigated - Uri = #frame1 IsNavigationInitiator True",
                        "FragmentNavigationToObject frame2",
                        "Navigated - Uri = #frame2 IsNavigationInitiator True",
                        "LoadCompleted - #frame2",
                        "FragmentNavigationToObject frame3",
                        "Navigated - Uri = #frame3 IsNavigationInitiator True",
                        "LoadCompleted - #frame3",
                        "State = GoBack ContentRendered  ControState = TextBox text = This is a test checkBox checked = True",
                        "FragmentNavigationToObject frame2",
                        "Navigated - Uri = #frame2 IsNavigationInitiator True",
                        "FragmentNavigationToObject frame1, BackButtonEnabled=True",
                        "Navigated - Uri = #frame1 IsNavigationInitiator True",
                        "LoadCompleted - #frame1"
                };

            _windowTitle
                = new String[]{
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject",
                        "FragmentNavigationToObject"
                };

            _backButtonEnabled
                = new bool[]{
                        false,
                        false,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        false,
                        false,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true
                };

            _forwardButtonEnabled
                = new bool[]{
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        true,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        false,
                        true,
                        true,
                        true,
                        false,
                        false
                };

            _backStack = new String[27][];
            _backStack[0] = new String[0];
            _backStack[1] = new String[0];
            _backStack[2] = new String[] { "FragmentNavigationToObject" };
            _backStack[3] = new String[] { "FragmentNavigationToObject" };
            _backStack[4] = new String[] { "FragmentNavigationToObject" };
            _backStack[5] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[6] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[7] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[8] = new String[] { "FragmentNavigationToObject" };
            _backStack[9] = new String[] { "FragmentNavigationToObject" };
            _backStack[10] = new String[0];
            _backStack[11] = new String[0];
            _backStack[12] = new String[0];
            _backStack[13] = new String[] { "FragmentNavigationToObject" };
            _backStack[14] = new String[] { "FragmentNavigationToObject" };
            _backStack[15] = new String[] { "FragmentNavigationToObject" };
            _backStack[16] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[17] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[18] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[19] = new String[] { "FragmentNavigationToObject (#frame2)", "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[20] = new String[] { "FragmentNavigationToObject (#frame2)", "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[21] = new String[] { "FragmentNavigationToObject (#frame2)", "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[22] = new String[] { "FragmentNavigationToObject (#frame2)", "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[23] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[24] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[25] = new String[] { "FragmentNavigationToObject (#frame2)", "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };
            _backStack[26] = new String[] { "FragmentNavigationToObject (#frame2)", "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject" };

            _forwardStack = new String[27][];
            _forwardStack[0] = new String[0];
            _forwardStack[1] = new String[0];
            _forwardStack[2] = new String[0];
            _forwardStack[3] = new String[0];
            _forwardStack[4] = new String[0];
            _forwardStack[5] = new String[0];
            _forwardStack[6] = new String[0];
            _forwardStack[7] = new String[0];
            _forwardStack[8] = new String[] { "FragmentNavigationToObject (#frame2)" };
            _forwardStack[9] = new String[] { "FragmentNavigationToObject (#frame2)" };
            _forwardStack[10] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject (#frame2)" };
            _forwardStack[11] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject (#frame2)" };
            _forwardStack[12] = new String[] { "FragmentNavigationToObject (#frame1)", "FragmentNavigationToObject (#frame2)" };
            _forwardStack[13] = new String[] { "FragmentNavigationToObject (#frame2)" };
            _forwardStack[14] = new String[] { "FragmentNavigationToObject (#frame2)" };
            _forwardStack[15] = new String[] { "FragmentNavigationToObject (#frame2)" };
            _forwardStack[16] = new String[0];
            _forwardStack[17] = new String[0];
            _forwardStack[18] = new String[0];
            _forwardStack[19] = new String[0];
            _forwardStack[20] = new String[0];
            _forwardStack[21] = new String[0];
            _forwardStack[22] = new String[0];
            _forwardStack[23] = new String[] { "FragmentNavigationToObject (#frame3)" };
            _forwardStack[24] = new String[] { "FragmentNavigationToObject (#frame3)" };
            _forwardStack[25] = new String[0];
            _forwardStack[26] = new String[0];
        }
    }
}
