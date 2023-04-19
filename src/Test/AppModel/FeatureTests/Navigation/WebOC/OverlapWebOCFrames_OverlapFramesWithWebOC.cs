// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: OverlapFramesWithWebOC tests that we are able to:
//  [1] host WebOCs in Frames where one is drawn over the other
//      and verify how these are rendered onscreen.
//  [2] navigate the Frame to and from the WebOCs to other XAML pages
//  [3] navigate Frame to PF and launch WebOC from PF
//  [4] Go back to WebOC in Frame after navigating away from it
//
//

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using Microsoft.Windows.Test.Client.AppSec.Navigation;


namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Setup:
    /// 1.  Have a NavigationWindow hosting two Frames that overlap
    /// 2.  The Frames start off with some standard XAML content
    /// 3.  Navigate both Frames to HTML content so that both now host WebOCs
    /// 4.  Navigate Frame1 to a PageFunction
    /// 5.  Navigate Frame2 to some other XAML page (the starting contents of F1?)
    /// then GoBack to WebOC/HTML
    /// </summary>
    public class OverlapWebOCFrames
    {
        internal enum CurrentTest
        {
            UnInit,
            hlink_OverlapFrames,
            hlink_NavigateFrame1ToWebOC,
            hlink_NavigateFrame2ToWebOC,
            hlink_NavigateFrame2ToPage,
            hlink_LaunchPFFromFrame1,
            hlink_GoBack,
            End
        }

        #region globals
        private NavigationWindow _currNavWin = null;
        private Frame _frame1 = null;
        private Frame _frame2 = null;
        private CurrentTest _test = CurrentTest.UnInit;
        private String _currentHyperlinkId = String.Empty;
        #endregion

        #region NavigationTestLibrary
        private JournalHelper _journalHelper = null;
        private NavigationStateCollection _actualStates = new NavigationStateCollection();
        private NavigationStateCollection _expectedStates = new NavigationStateCollection();
        private const String homePage = "OverlapWebOCFrames_HomePage.xaml";
        private const String framesPage = "OverlapWebOCFrames_OverlappedFrames.xaml";
        private const String frame1Content = "OverlapWebOCFrames_Frame1Contents.xaml";
        private const String frame2Content = "MarkupStrPF1.xaml";
        private const String msnbc = @"http://www.msnbc.com/";
        private const String microsoft = @"http://www.microsoft.com/";

        // expected values for NavigationState
        private String[] _stateDescription = null;
        private String[] _windowTitle = null;
        private bool[] _backButtonEnabled = null;
        private bool[] _forwardButtonEnabled = null;
        private String[][] _backStack = null;
        private String[][] _forwardStack = null;
        #endregion


        public void Startup(object sender, StartupEventArgs e)
        {
            // Initialize TestLog
            NavigationHelper.CreateLog("OverlapFramesWithWebOC");
            NavigationHelper.SetStage(TestStage.Run);
            Application.Current.StartupUri = new Uri(homePage, UriKind.RelativeOrAbsolute);

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


        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.Output("----In OnLoadCompletedAPP. State - " + _test.ToString());
            NavigationHelper.Output("e.Navigator is " + e.Navigator.ToString());

            if (_journalHelper != null)
            {
                _actualStates.RecordNewResult(_journalHelper, "App LoadCompleted Navigator = " + e.Navigator + " uri = " + e.Uri);
            }

            if (_test == CurrentTest.UnInit)
            {
                NavigationHelper.Output("Initializing test.");
                if (e.Navigator is NavigationWindow)
                {
                    _currNavWin = Application.Current.MainWindow as NavigationWindow;
                    _currNavWin.ContentRendered += new EventHandler(ContentRenderedNavWin);

                    _journalHelper = new JournalHelper(_currNavWin);
                    if (_journalHelper == null)
                    {
                        NavigationHelper.Fail("JournalHelper could not be initialized (JournalHelper is null). Exiting test case.");
                    }

                    _test = CurrentTest.hlink_OverlapFrames;
                }
            }
        }


        private void ContentRenderedNavWin(object sender, EventArgs e)
        {
            NavigationHelper.Output("----In ContentRenderedNavWin. State - " + _test.ToString());
            NavigationHelper.Output("currNavWin.Source = " + _currNavWin.Source);
            _actualStates.RecordNewResult(_journalHelper, "OnContentRenderedNavWin Source=" + _currNavWin.Source);

            switch (_test)
            {
                case CurrentTest.hlink_OverlapFrames:
                    NavigationHelper.Output("Navigating to " + framesPage);
                    NavigateToUri(new Uri(framesPage, UriKind.RelativeOrAbsolute));
                    _test = CurrentTest.hlink_NavigateFrame1ToWebOC;
                    break;

                // Navigate each of the Frames to HTML (instantiating WebOC)
                case CurrentTest.hlink_NavigateFrame1ToWebOC:
                    if (_currNavWin.Source != null && _currNavWin.Source.ToString().Equals(framesPage))
                    {
                        NavigationHelper.Output("Currently on " + framesPage);
                        _frame1 = LogicalTreeHelper.FindLogicalNode(_currNavWin.Content as DependencyObject, "frame1") as Frame;
                        _frame1.ContentRendered += new EventHandler(ContentRenderedFrame1);
                        _frame2 = LogicalTreeHelper.FindLogicalNode(_currNavWin.Content as DependencyObject, "frame2") as Frame;
                        _frame2.ContentRendered += new EventHandler(ContentRenderedFrame2);
                    }
                    if (_frame1 != null && _frame1.Source.ToString().Equals(frame1Content))
                    {
                        NavigationHelper.Output("Navigating frame 1 to HTML");
                        NavigateHyperlink();
                    }
                    break;

                case CurrentTest.hlink_NavigateFrame2ToWebOC:
                case CurrentTest.hlink_NavigateFrame2ToPage:
                case CurrentTest.hlink_LaunchPFFromFrame1:
                case CurrentTest.hlink_GoBack:
                case CurrentTest.End:
                    break;

                default:
                    NavigationHelper.Fail(_test.ToString() + " is not one of the predefined subtests.  Exiting test case.");
                    break;
            }
        }

        // ContentRendered handler for frame1
        private void ContentRenderedFrame1(object sender, EventArgs e)
        {
            NavigationHelper.Output("----In ContentRenderedFrame1. State - " + _test.ToString());

            if (_test == CurrentTest.hlink_NavigateFrame1ToWebOC && _frame1.Source.ToString().Equals(msnbc))
            {
                NavigationHelper.Output("frame 1 is currently on HTML content");
                _test = CurrentTest.hlink_NavigateFrame2ToWebOC;
                NavigationHelper.Output("Navigating frame 2 to HTML");
                NavigateHyperlink();
            }
            else if (_test == CurrentTest.hlink_LaunchPFFromFrame1 && _frame1.Source.ToString().Equals(frame2Content))
            {
                _test = CurrentTest.hlink_GoBack;
                GoBack();
            }
            else if (_test == CurrentTest.hlink_GoBack)
            {
                _test = CurrentTest.End;

                // Compare also the actual and expected stateDescription
                NavigationStateCollection.CompareDescription = true;
                // Pass or fail?
                if (NavigationStateCollection.Compare(_actualStates, _expectedStates))
                {
                    NavigationHelper.Pass("All states matched - test passed");
                }
                else
                {
                    NavigationHelper.Fail("All states did not match - test failed");
                }
            }
        }

        // ContentRendered handler for frame2
        private void ContentRenderedFrame2(object sender, EventArgs e)
        {
            NavigationHelper.Output("----In ContentRenderedFrame2. State - " + _test.ToString());

            if (_test == CurrentTest.hlink_NavigateFrame2ToWebOC && _frame2.Source.ToString().Equals(microsoft))
            {
                // Navigate Frame2 away from HTML content
                NavigationHelper.Output("frame 2 is currently on HTML content");
                NavigationHelper.Output("Navigating frame 2 to " + frame1Content);
                _test = CurrentTest.hlink_NavigateFrame2ToPage;
                NavigateHyperlink();
            }
            else if (_test == CurrentTest.hlink_NavigateFrame2ToPage)
            {
                NavigationHelper.Output("Navigating frame 1 to markup PageFunction");
                _test = CurrentTest.hlink_LaunchPFFromFrame1;
                NavigateHyperlink();
            }
        }

        public void DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            NavigationHelper.Output("Dispatcher exception = " + e.Exception);
            NavigationHelper.Output("currentState = " + _test);

            // System.Net.WebException can be caught on some machines due to DNS issues
            if (e.Exception is System.Net.WebException)
            {
                NavigationHelper.ExitWithIgnore("Invalid DNS configuration hit. Could not resolve msnbc.com or microsoft.com.  Please verify scenario manually.");
            }
            else
            {
                NavigationHelper.Fail("Unexpected exception caught. Test fails");
            }

            e.Handled = true;
            Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
        }

        #region asynchronous navigation
        void NavigateHyperlink()
        {
            RequestNavigation();
        }


        void RaiseRequestNavigate(String hyperlinkId)
        {
            NavigationHelper.Output("Navigating to " + hyperlinkId);
            Hyperlink hlink = LogicalTreeHelper.FindLogicalNode(_currNavWin.Content as DependencyObject, hyperlinkId) as Hyperlink;
            RequestNavigateEventArgs requestNavigateEventArgs = new RequestNavigateEventArgs(hlink.NavigateUri, hlink.TargetName);
            requestNavigateEventArgs.Source = hlink;
            hlink.RaiseEvent(requestNavigateEventArgs);
        }


        void RequestNavigation()
        {
            _currentHyperlinkId = _test.ToString();

            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                    (DispatcherOperationCallback)delegate(object ob)
                                    {
                                        RaiseRequestNavigate(_currentHyperlinkId);
                                        return null;
                                    }, null);
        }


        void NavigateToUri(object target)
        {
            NavigationHelper.Output("Navigating to " + target.ToString());
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                    (DispatcherOperationCallback)delegate(object ob)
                                    {
                                        _currNavWin.Navigate((Uri)target);
                                        return null;
                                    }, null);
        }


        void GoBackOnce()
        {
            NavigationHelper.Output("Going back to previous page");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background,
                                (DispatcherOperationCallback)delegate(object ob)
                                {
                                    _currNavWin.GoBack();
                                    return null;
                                }, null);
        }

        void GoBack()
        {
            GoBackOnce();
        }
        #endregion

        /// <summary>
        /// Create expected navigation states 
        /// </summary>
        void CreateExpectedNavigationStates()
        {
            _stateDescription
                = new String[]{
                        "OnContentRenderedNavWin Source=OverlapWebOCFrames_HomePage.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: OverlapWebOCFrames_Frame1Contents.xaml uri = OverlapWebOCFrames_Frame1Contents.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: OverlapWebOCFrames_Frame2Contents.xaml uri = OverlapWebOCFrames_Frame2Contents.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow uri = OverlapWebOCFrames_OverlappedFrames.xaml",
                        "OnContentRenderedNavWin Source=OverlapWebOCFrames_OverlappedFrames.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: http://www.msnbc.com/ uri = http://www.msnbc.com/",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: http://www.microsoft.com/ uri = http://www.microsoft.com/",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: OverlapWebOCFrames_Frame1Contents.xaml uri = OverlapWebOCFrames_Frame1Contents.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: MarkupStrPF1.xaml uri = MarkupStrPF1.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: http://www.msnbc.com/ uri = http://www.msnbc.com/"
                };

            _windowTitle
                = new String[]{
                        "My Little Ponies",
                        "My Little Ponies",
                        "My Little Ponies",
                        "My Little Ponies",
                        "My Little Ponies",
                        "My Little Ponies",
                        "My Little Ponies",
                        "My Little Ponies",
                        "My Little Ponies",
                        "My Little Ponies"
                };

            _backButtonEnabled
                = new bool[]{
                        false,
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
                        false,
                        false,
                        true
                };

            _backStack = new String[10][];
            _backStack[0] = new String[0];
            _backStack[1] = new String[] { "My Little Ponies (OverlapWebOCFrames_HomePage.xaml)" };
            _backStack[2] = new String[] { "My Little Ponies (OverlapWebOCFrames_HomePage.xaml)" };
            _backStack[3] = new String[] { "My Little Ponies (OverlapWebOCFrames_HomePage.xaml)" };
            _backStack[4] = new String[] { "My Little Ponies (OverlapWebOCFrames_HomePage.xaml)" };
            _backStack[5] = new String[] { "OverlapWebOCFrames_Frame1Contents.xaml", "My Little Ponies (OverlapWebOCFrames_HomePage.xaml)" };
            _backStack[6] = new String[] { "OverlapWebOCFrames_Frame2Contents.xaml", "OverlapWebOCFrames_Frame1Contents.xaml", "My Little Ponies (OverlapWebOCFrames_HomePage.xaml)" };
            _backStack[7] = new String[] { "http://www.microsoft.com/", "OverlapWebOCFrames_Frame2Contents.xaml", "OverlapWebOCFrames_Frame1Contents.xaml", "My Little Ponies (OverlapWebOCFrames_HomePage.xaml)" };
            _backStack[8] = new String[] { "http://www.msnbc.com/", "http://www.microsoft.com/", "OverlapWebOCFrames_Frame2Contents.xaml", "OverlapWebOCFrames_Frame1Contents.xaml", "My Little Ponies (OverlapWebOCFrames_HomePage.xaml)" };
            _backStack[9] = new String[] { "http://www.microsoft.com/", "OverlapWebOCFrames_Frame2Contents.xaml", "OverlapWebOCFrames_Frame1Contents.xaml", "My Little Ponies (OverlapWebOCFrames_HomePage.xaml)" };

            _forwardStack = new String[10][];
            _forwardStack[0] = new String[0];
            _forwardStack[1] = new String[0];
            _forwardStack[2] = new String[0];
            _forwardStack[3] = new String[0];
            _forwardStack[4] = new String[0];
            _forwardStack[5] = new String[0];
            _forwardStack[6] = new String[0];
            _forwardStack[7] = new String[0];
            _forwardStack[8] = new String[0];
            _forwardStack[9] = new String[] { "MarkupStrPF1.xaml" };
        }
    }
}
