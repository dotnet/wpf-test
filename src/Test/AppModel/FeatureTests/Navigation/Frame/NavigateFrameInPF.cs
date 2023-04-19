// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: NavigateFrameInPF tests that we are able to:
//  [1] navigate via a Hyperlink to a PageFunction containing a Frame, 
//  [2] navigate the Frame within the PageFunction using a Hyperlink
//  [3] navigate away from the PF, without returning to its parent
//  [4] GoBack and check PF and Frame contents, as well as journal state
//
//

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // NavigateFrameInPF
    public class NavigateFrameInPF
    {
        private enum CurrentTest
        {
            UnInit,
            NavigateToPageFunction,
            hlink_NavigatePageFunctionFrame,
            NavigateAwayFromPF,
            ReturnPageFunction
        }

        #region NavigateFrameInPF globals
        CurrentTest _navFrameInPFTest = CurrentTest.UnInit;
        private int _navFrameInPFSubTestCase = 0;
        private int _currentStateIndex = 0;

        private JournalHelper _navFrameInPFJHelper = null;
        private NavigationStateCollection _navFrameInPFActualStates = new NavigationStateCollection();
        private NavigationStateCollection _navFrameInPFExpectedStates = new NavigationStateCollection();
        private bool _navFrameInPFInVerifyMode = true;
        private Frame _navFrameInPFTestFrame = null;

        private const String navFrameInPFResults = @"NavFrameInPFResults_Loose.xml";
        private const String pfFrame = @"pfFrame";
        private const String pfReturnButton = "pfButton";
        FrameTestClass _frameTest = null;
        private Frame _homeFrame = null;

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
            _frameTest = new FrameTestClass("NavigateFrameInPF");
            _frameTest.Output("[Startup] navFrameInPFInVerifyMode=" + _navFrameInPFInVerifyMode.ToString());
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = false;

            /*
             * This code no longer works because of the error "cscomp.dll is missing" 
             * You may use it when this issue is fixed
            if (navFrameInPFInVerifyMode)
                navFrameInPFExpectedStates = NavigationStateCollection.GetResults(
                    Application.GetRemoteStream(new Uri(navFrameInPFResults, UriKind.RelativeOrAbsolute)).Stream);
             */

            if (_navFrameInPFInVerifyMode)
            {
                CreateExpectedNavigationStates();

                // populate the expected navigation states
                for (int index = 0; index < _stateDescription.Length; index++)
                {
                    _navFrameInPFExpectedStates.states.Add(new NavigationState(_stateDescription[index],
                        _windowTitle[index],
                        _backButtonEnabled[index],
                        _forwardButtonEnabled[index],
                        _backStack[index],
                        _forwardStack[index]));
                }
            }
        }

        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            _frameTest.Output("[LoadCompleted] sender=" + sender.ToString() + " - e.Navigator=" + e.Navigator.ToString());

            if (_frameTest.IsFirstRun &&
                _navFrameInPFTest == CurrentTest.UnInit)
            {
                _frameTest.SetupTest();

                _frameTest.NavigationWindow.Source = new Uri("NavigateFrameInPF_PageFunction.xaml", UriKind.RelativeOrAbsolute);

                if (_frameTest.StdFrame == null)
                {
                    _frameTest.Fail("Could not locate Frame to run NavigateFrameInPF test case on");
                }

                _navFrameInPFJHelper = new JournalHelper(_frameTest.NavigationWindow);
                _homeFrame = LogicalTreeHelper.FindLogicalNode(((NavigationWindow)Application.Current.MainWindow).Content as DependencyObject, "testFrame") as Frame;
                _homeFrame.ContentRendered += new EventHandler(ContentRendered);

                if (_navFrameInPFJHelper == null)
                {
                    _frameTest.Fail("JournalHelper could not be initialized and is still null. Exiting test.");
                }

                _frameTest.IsFirstRun = false;

                _navFrameInPFTest = CurrentTest.NavigateToPageFunction;
            }

            if (_navFrameInPFJHelper != null) 
            {
                RecordNewResult("App LoadCompleted Navigator = " + e.Navigator + " uri = " + e.Uri);
            }
        }

        public void ContentRendered(object sender, EventArgs e)
        {
            _frameTest.Output("[ContentRendered] navFrameInPFTest=" + _navFrameInPFTest.ToString());
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            if (sender is NavigationWindow)
            {
                RecordNewResult("OnContentRendered_NavWin Source = " + navWin.Source);

                switch(_navFrameInPFTest)
                {
                    case CurrentTest.NavigateToPageFunction:
                        RouteNavigateToPageFunctionSubTest();
                        break;

                    case CurrentTest.hlink_NavigatePageFunctionFrame:
                        _frameTest.Output("ERROR: Not supposed to be in " + _navFrameInPFTest.ToString());
                        break;

                    case CurrentTest.NavigateAwayFromPF:
                        RouteNavigateAwayFromPageFunctionSubTest();
                        break;

                    case CurrentTest.ReturnPageFunction:
                        RouteReturnPageFunctionSubTest();
                        break;

                    default:
                        _frameTest.Output(_navFrameInPFTest.ToString() + " is not one of the predefined subtests. Exiting test case");
                        break;
                }
            }
            else if (sender is Frame)
            {
                RecordNewResult("OnContentRendered_Frame Source = " + _navFrameInPFTestFrame.Source);

                switch (_navFrameInPFTest)
                {
                    case CurrentTest.hlink_NavigatePageFunctionFrame:
                        RouteNavigatePageFunctionFrameSubTest();
                        break;

                    default:
                        _frameTest.Output(_navFrameInPFTest.ToString() + " is not one of the predefined subtests. Exiting test case");
                        break;
                }
            }
        }

        private void RouteNavigateToPageFunctionSubTest()
        {
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            switch (_navFrameInPFSubTestCase)
            {
                case 0:
                    // Navigate to a PageFunction
                    _frameTest.Output("Instantiating and navigating to a PageFunction");
                    NavigateFrameInPF_PageFunction pf = new NavigateFrameInPF_PageFunction();
                    pf.InitializeComponent();
                    _frameTest.NavigateToObject(navWin, pf);
                    _navFrameInPFSubTestCase++;
                    _frameTest.Output("Instantiated and navigated to a PageFunction");
                    break;

                case 1:
                    // Grab the Frame within the PageFunction, add eventhandler
                    _frameTest.Output("Currently on PageFunction");
                    _navFrameInPFTest = CurrentTest.hlink_NavigatePageFunctionFrame;
                    _navFrameInPFTestFrame = LogicalTreeHelper.FindLogicalNode(navWin.Content as DependencyObject, pfFrame) as Frame;
                    _navFrameInPFTestFrame.ContentRendered += new EventHandler(ContentRendered);
                    _navFrameInPFSubTestCase--;
                    _frameTest.Output("Currently out of PageFunction");
                    break;
            }
            return;
        }

        private void RouteNavigatePageFunctionFrameSubTest()
        {
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            switch (_navFrameInPFSubTestCase)
            {
                case 0:
                    // Navigate the Frame to another XAML file
                    _frameTest.Output("Navigating frame elsewhere");
                    BrowserHelper.NavigateHyperlinkViaEvent(_navFrameInPFTest.ToString(), navWin.Content as DependencyObject);
                    _navFrameInPFSubTestCase++;
                    _frameTest.Output("Navigated frame elsewhere");
                    break;

                case 1:
                    _frameTest.Output("Frame is now at a different location: " + _navFrameInPFTestFrame.Source);
                    _navFrameInPFTest = CurrentTest.NavigateAwayFromPF;
                    _navFrameInPFSubTestCase--;
                    RouteNavigateAwayFromPageFunctionSubTest();
                    _frameTest.Output("Frame is back to original location.");
                    break;
            }
            return;
        }

        private void RouteNavigateAwayFromPageFunctionSubTest()
        {
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            switch (_navFrameInPFSubTestCase)
            {
                case 0:
                    _frameTest.Output("Navigating away from PageFunction to Giant Button of Doom.");
                    Button mySpecialButton = new Button();
                    mySpecialButton.Content = "The Magic Button of Doom!";
                    _frameTest.NavigateToObject(navWin, mySpecialButton);
                    _navFrameInPFSubTestCase++;
                    _frameTest.Output("Navigated away from PageFunction to Giant Button of Doom.");
                    break;

                case 1:
                    _frameTest.Output("Currently at Giant Button of Doom, going back to PageFunction");
                    _frameTest.GoBack(navWin);
                    _navFrameInPFTest = CurrentTest.ReturnPageFunction;
                    _navFrameInPFSubTestCase--;
                    _frameTest.Output("Back to PageFunction");
                    break;
            }
            return;
        }

        private void RouteReturnPageFunctionSubTest()
        {
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            switch (_navFrameInPFSubTestCase)
            {
                case 0:
                    _frameTest.Output("Currently on PageFunction. Getting ready to return");
                    // Find the Return button on the PageFunction
                    Button returnButton = LogicalTreeHelper.FindLogicalNode(navWin.Content as DependencyObject, pfReturnButton) as Button;
                    // Click on the button
                    _frameTest.Output("Attempting to finish the PageFunction by clicking the " + pfReturnButton + " button");
                    BrowserHelper.InvokeButton(pfReturnButton, navWin.Content as DependencyObject);
                    _navFrameInPFSubTestCase++;
                    _frameTest.Output("Finished the PageFunction by clicking.");
                    break;

                case 1:
                    _frameTest.Output("On the PageFunction's parent page. Verifying NavigationState.");
                    _navFrameInPFSubTestCase--;
                    // Pass or fail?
                    if (_navFrameInPFInVerifyMode)
                    {
                        if (NavigationStateCollection.Compare(_navFrameInPFActualStates, _navFrameInPFExpectedStates))
                        {
                            _frameTest.Pass("All states matched");
                        }
                        else
                        {
                            _frameTest.Fail("All states did not match");
                        }
                    }
                    else
                    {
                        _navFrameInPFActualStates.WriteResults(navFrameInPFResults);
                    }
                    _frameTest.Output("Case is complete.");
                    break;
            }
            return;
        }

        private void RecordNewResult(string logString)
        {
            _navFrameInPFActualStates.RecordNewResult(_navFrameInPFJHelper, logString);
            _frameTest.Output(String.Format("[{0} - {1}] {2}",
                _navFrameInPFTest.ToString(),
                (_currentStateIndex++).ToString(CultureInfo.InvariantCulture),
                logString));
        }

        /// <summary>
        /// Create expected navigation states 
        /// </summary>
        void CreateExpectedNavigationStates()
        {
            _stateDescription
                = new String[]{
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow uri = FrameTestPage.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: FlowDocument_Loose.xaml uri = FlowDocument_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow uri = NavigateFrameInPF_PageFunction.xaml",
                        "OnContentRendered_NavWin Source = NavigateFrameInPF_PageFunction.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/FlowDocument_Loose.xaml uri = NavigationTests_Standalone;component/FlowDocument_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow uri = ",
                        "OnContentRendered_NavWin Source = ",
                        "OnContentRendered_Frame Source = NavigationTests_Standalone;component/FlowDocument_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/ImagePage_Loose.xaml uri = NavigationTests_Standalone;component/ImagePage_Loose.xaml",
                        "OnContentRendered_Frame Source = NavigationTests_Standalone;component/ImagePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow: The Magic Button of Doom! uri = ",
                        "OnContentRendered_NavWin Source = ",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/ImagePage_Loose.xaml uri = NavigationTests_Standalone;component/ImagePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow uri = ",
                        "OnContentRendered_NavWin Source = ",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: FlowDocument_Loose.xaml uri = FlowDocument_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Navigation.NavigationWindow uri = NavigateFrameInPF_PageFunction.xaml",
                        "OnContentRendered_NavWin Source = NavigateFrameInPF_PageFunction.xaml"
                };

            _windowTitle
                = new String[]{
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation",
                        "RedirectMitigation"
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
                        false,
                        false,
                        false,
                        true,
                        true,
                        true,
                        false,
                        false,
                        false
                };

            _backStack = new String[18][];
            _backStack[0] = new String[0];
            _backStack[1] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[2] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[3] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[4] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[5] = new String[] { "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[6] = new String[] { "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[7] = new String[] { "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[8] = new String[] { "Lipsum, For The Masses", "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[9] = new String[] { "Lipsum, For The Masses", "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[10] = new String[] { "RedirectMitigation", "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[11] = new String[] { "RedirectMitigation", "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[12] = new String[] { "RedirectMitigation", "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[13] = new String[] { "Lipsum, For The Masses", "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[14] = new String[] { "Lipsum, For The Masses", "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[15] = new String[] { "Lipsum, For The Masses", "RedirectMitigation (NavigateFrameInPF_PageFunction.xaml)", "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[16] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };
            _backStack[17] = new String[] { "RedirectMitigation (FrameTestPage.xaml)" };

            _forwardStack = new String[18][];
            _forwardStack[0] = new String[0];
            _forwardStack[1] = new String[0];
            _forwardStack[2] = new String[0];
            _forwardStack[3] = new String[0];
            _forwardStack[4] = new String[0];
            _forwardStack[5] = new String[0];
            _forwardStack[6] = new String[0];
            _forwardStack[7] = new String[0];
            _forwardStack[8] = new String[0];
            _forwardStack[9] = new String[0];
            _forwardStack[10] = new String[0];
            _forwardStack[11] = new String[0];
            _forwardStack[12] = new String[0];
            _forwardStack[13] = new String[] { "RedirectMitigation" };
            _forwardStack[14] = new String[] { "RedirectMitigation" };
            _forwardStack[15] = new String[0];
            _forwardStack[16] = new String[0];
            _forwardStack[17] = new String[0];
        }
    }
}
