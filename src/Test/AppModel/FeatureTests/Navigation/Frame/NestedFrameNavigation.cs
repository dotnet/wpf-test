// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  NestedFrameNavigation tests several things using a Hyperlink
//  contained in a Frame, including:
//  [1] With a Hyperlink in the outer frame: navigate the outer frame
//  [2] With a Hyperlink in the outer frame: navigate the inner frame
//  [3] With a Hyperlink in the inner frame: navigate the outer frame
//  [4] With a Hyperlink in the inner frame: navigate the inner frame 
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class NestedFrameNavigation
    {
       private enum CurrentTest
        {
            UnInit,
            hlinkOuterNavigatingOuter,
            GoBack1,  // Go back to previous page
            hlinkOuterNavigatingInner,
            hlinkInnerNavigatingInner,
            GoBack2, // Go back to previous page
            hlinkInnerNavigatingOuter,
            End
        }

        #region NestedFrameNavigation globals
        private CurrentTest _nestedFrameNavigationTest = CurrentTest.UnInit;

        private Frame _nestedFrameNavigationInnerFrame = null;
        private Frame _nestedFrameNavigationOuterFrame = null;

        private JournalHelper _nestedFrameNavigationJHelper = null;
        private NavigationStateCollection _nestedFrameNavigationActualStates = new NavigationStateCollection();
        private NavigationStateCollection _nestedFrameNavigationExpectedStates = new NavigationStateCollection();

        private const String innerFrameName = @"InnerFrame";
        private const String nestedFramePage = @"NestedFrame_Loose.xaml";
        private FrameTestClass _frameTest = null;

        // expected values for NavigationState
        private String[] _stateDescription = null;
        private String[] _windowTitle = null;
        private bool[] _backButtonEnabled = null;
        private bool[] _forwardButtonEnabled = null;
        private String[][] _backStack = null;
        private String[][] _forwardStack = null;
        #endregion

        // startup event handler
        public void Startup(object sender, StartupEventArgs e)
        {
            _frameTest = new FrameTestClass("NestedFrameNavigation");
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = false;
        }

        // load completed event handler
        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            _frameTest.Output("e.Navigator is " + e.Navigator.ToString());

            if (_nestedFrameNavigationJHelper != null)
            {
                _nestedFrameNavigationActualStates.RecordNewResult(_nestedFrameNavigationJHelper, "App LoadCompleted Navigator = " + e.Navigator + " uri = " + e.Uri);
            }

            if (_frameTest.IsFirstRun && _nestedFrameNavigationTest == CurrentTest.UnInit)
            {
                _frameTest.SetupTest();

                if (_frameTest.StdFrame == null)
                {
                    _frameTest.Fail("Could not locate Frame to run NestedFrameNavigation test case on");
                }

                _frameTest.StdFrame.Source = new Uri(nestedFramePage, UriKind.RelativeOrAbsolute);

                _nestedFrameNavigationOuterFrame = _frameTest.StdFrame;
                _nestedFrameNavigationOuterFrame.ContentRendered += new EventHandler(ContentRenderedOuter);

                _nestedFrameNavigationTest = CurrentTest.hlinkOuterNavigatingOuter;
                _frameTest.IsFirstRun = false;

                NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
                _nestedFrameNavigationJHelper = new JournalHelper(navWin);

                CreateExpectedNavigationStates();

                // populate the expected navigation states
                for (int index = 0; index < _stateDescription.Length; index++)
                {
                    _nestedFrameNavigationExpectedStates.states.Add(new NavigationState(_stateDescription[index],
                        _windowTitle[index],
                        _backButtonEnabled[index],
                        _forwardButtonEnabled[index],
                        _backStack[index],
                        _forwardStack[index]));
                }
            }
        }

        // ContentRendered handler for outer frame
        public void ContentRenderedOuter(object sender, EventArgs e)
        {
            _frameTest.Output("In ContentRenderedOuter. outerFrame.Source = " + _nestedFrameNavigationOuterFrame.Source);
            _frameTest.Output("State - " + _nestedFrameNavigationTest.ToString());
            _nestedFrameNavigationActualStates.RecordNewResult(_nestedFrameNavigationJHelper, "OnContentRendered_OuterFrame Source = " + _nestedFrameNavigationOuterFrame.Source);

            switch (_nestedFrameNavigationTest)
            {
                case CurrentTest.hlinkOuterNavigatingOuter:
                    RouteNavigateFrameSubTest();
                    break;

                case CurrentTest.hlinkOuterNavigatingInner:
                    _nestedFrameNavigationInnerFrame = LogicalTreeHelper.FindLogicalNode(_nestedFrameNavigationOuterFrame.Content as DependencyObject, innerFrameName) as Frame;
                    if (_nestedFrameNavigationInnerFrame == null)
                    {
                        _frameTest.Fail("InnerFrame is null");
                    }
                    else
                    {
                        _frameTest.Output("InnerFrame is found");
                    }
                    _nestedFrameNavigationOuterFrame.ContentRendered -= ContentRenderedOuter;
                    _nestedFrameNavigationInnerFrame.ContentRendered += new EventHandler(ContentRenderedInner);
                    RouteNavigateFrameSubTest();
                    break;

                case CurrentTest.hlinkInnerNavigatingOuter:
                    RouteNavigateFrameSubTest();
                    break;

                case CurrentTest.GoBack1:
                    RouteNavigateFrameSubTest();
                    break;

                case CurrentTest.End:
                    // Pass or fail?
                    if (NavigationStateCollection.Compare(_nestedFrameNavigationActualStates, _nestedFrameNavigationExpectedStates))
                    {
                        _frameTest.Pass("All states matched");
                    }
                    else
                    {
                        _frameTest.Fail("All states did not match");
                    }

                    break;

                default:
                    _frameTest.Fail("Subtest " + _nestedFrameNavigationTest.ToString() + " was not one of the predefined tests. Exiting test case.");
                    break;
            }
        }

        // ContentRendered handler for the inner frame
        private void ContentRenderedInner(object sender, EventArgs e)
        {
            _frameTest.Output("In ContentRenderedInner. State - " + _nestedFrameNavigationTest.ToString());
            _nestedFrameNavigationInnerFrame = LogicalTreeHelper.FindLogicalNode(_nestedFrameNavigationOuterFrame.Content as DependencyObject, innerFrameName) as Frame;
            if (_nestedFrameNavigationInnerFrame == null)
            {
                _frameTest.Fail("InnerFrame is null");
            }
            else
            {
                _frameTest.Output("InnerFrame is found");
            }

            _frameTest.Output("In ContentRenderedInner. innerFrame.Source = " + _nestedFrameNavigationInnerFrame.Source);
            _nestedFrameNavigationActualStates.RecordNewResult(_nestedFrameNavigationJHelper, "OnContentRendered_InnerFrame Source = " + _nestedFrameNavigationInnerFrame.Source);

            switch (_nestedFrameNavigationTest)
            {
                case CurrentTest.hlinkOuterNavigatingInner:
                    RouteNavigateFrameSubTest();
                    break;

                case CurrentTest.hlinkInnerNavigatingInner:
                    RouteNavigateFrameSubTest();
                    break;

                case CurrentTest.GoBack2:
                    RouteNavigateFrameSubTest();
                    break;

                case CurrentTest.hlinkInnerNavigatingOuter:
                    _nestedFrameNavigationOuterFrame.ContentRendered += new EventHandler(ContentRenderedOuter);
                    _nestedFrameNavigationInnerFrame.ContentRendered -= ContentRenderedInner;
                    RouteNavigateFrameSubTest();
                    break;

                default:
                    _frameTest.Fail("Subtest " + _nestedFrameNavigationTest.ToString() + " was not one of the predefined tests. Exiting test case.");
                    break;
            }
        }

        private void RouteNavigateFrameSubTest()
        {
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            switch (_nestedFrameNavigationTest)
            {
                case CurrentTest.hlinkOuterNavigatingOuter:
                    _frameTest.Output(_nestedFrameNavigationTest.ToString() + ": Navigation to a page in outer frame");

                    // Click on the Hyperlink that will navigate this outer frame to a Page
                    BrowserHelper.NavigateHyperlinkViaEvent(_nestedFrameNavigationTest.ToString(), _nestedFrameNavigationOuterFrame.Content as DependencyObject);
                    _nestedFrameNavigationTest = CurrentTest.GoBack1;
                    break;

                case CurrentTest.GoBack1:
                    _frameTest.Output("Returning to previous content");
                    // GoBack to starting page
                    _frameTest.GoBack(navWin);
                    _nestedFrameNavigationTest = CurrentTest.hlinkOuterNavigatingInner;
                    break;

                case CurrentTest.hlinkOuterNavigatingInner:
                    _frameTest.Output(_nestedFrameNavigationTest.ToString() + ": Navigation to a page in the inner frame by clicking the hlink in outer frame");

                    // Click on the Hyperlink in outer frame that will navigate to a page in the inner frame
                    BrowserHelper.NavigateHyperlinkViaEvent(_nestedFrameNavigationTest.ToString(), _nestedFrameNavigationOuterFrame.Content as DependencyObject);
                    _nestedFrameNavigationTest = CurrentTest.hlinkInnerNavigatingInner;
                    break;

                case CurrentTest.hlinkInnerNavigatingInner:
                    _frameTest.Output(_nestedFrameNavigationTest.ToString() + ": Navigation to a page in the inner frame by clicking the hlink in inner frame");
                    // Click on the Hyperlink in the inner frame that will navigate to a page in the inner frame
                    BrowserHelper.NavigateHyperlinkViaEvent(_nestedFrameNavigationTest.ToString(), _nestedFrameNavigationInnerFrame.Content as DependencyObject);
                    _nestedFrameNavigationTest = CurrentTest.GoBack2;
                    break;

                case CurrentTest.GoBack2:
                    _frameTest.Output("Returning to previous content");
                    // GoBack to previous page
                    _frameTest.GoBack(navWin);
                    _nestedFrameNavigationTest = CurrentTest.hlinkInnerNavigatingOuter;
                    break;

                case CurrentTest.hlinkInnerNavigatingOuter:
                    _frameTest.Output(_nestedFrameNavigationTest.ToString() + ": Navigation to a page in outer frame by clicking the hlink in inner frame");
                    // Click on the Hyperlink in the inner frame that will navigate to a page in the outer frame
                    BrowserHelper.NavigateHyperlinkViaEvent(_nestedFrameNavigationTest.ToString(), _nestedFrameNavigationInnerFrame.Content as DependencyObject);
                    _nestedFrameNavigationTest = CurrentTest.End;
                    break;
            }
            return;
        }

        /// <summary>
        /// Create expected navigation states 
        /// </summary>
        void CreateExpectedNavigationStates()
        {
            _stateDescription
                = new String[]{
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/NestedFrame_Loose.xaml uri = NavigationTests_Standalone;component/NestedFrame_Loose.xaml",
                        "OnContentRendered_OuterFrame Source = NavigationTests_Standalone;component/NestedFrame_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/Page1_Loose.xaml uri = NavigationTests_Standalone;component/Page1_Loose.xaml",
                        "OnContentRendered_OuterFrame Source = NavigationTests_Standalone;component/Page1_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/NestedFrame_Loose.xaml uri = NavigationTests_Standalone;component/NestedFrame_Loose.xaml",
                        "OnContentRendered_OuterFrame Source = NavigationTests_Standalone;component/NestedFrame_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/InnerFramePage_Loose.xaml uri = NavigationTests_Standalone;component/InnerFramePage_Loose.xaml",
                        "OnContentRendered_InnerFrame Source = NavigationTests_Standalone;component/InnerFramePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/FlowDocument_Loose.xaml uri = NavigationTests_Standalone;component/FlowDocument_Loose.xaml",
                        "OnContentRendered_InnerFrame Source = NavigationTests_Standalone;component/FlowDocument_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/InnerFramePage_Loose.xaml uri = NavigationTests_Standalone;component/InnerFramePage_Loose.xaml",
                        "OnContentRendered_InnerFrame Source = NavigationTests_Standalone;component/InnerFramePage_Loose.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/Page1_Loose.xaml uri = NavigationTests_Standalone;component/Page1_Loose.xaml",
                        "OnContentRendered_OuterFrame Source = NavigationTests_Standalone;component/Page1_Loose.xaml"
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
                        "RedirectMitigation"
                };

            _backButtonEnabled
                = new bool[]{
                        false,
                        false,
                        true,
                        true,
                        false,
                        false,
                        false,
                        false,
                        true,
                        true,
                        false,
                        false,
                        true,
                        true
                };

            _forwardButtonEnabled
                = new bool[]{
                        false,
                        false,
                        false,
                        false,
                        true,
                        true,
                        true,
                        true,
                        false,
                        false,
                        true,
                        true,
                        false,
                        false
                };

            _backStack = new String[14][];
            _backStack[0] = new String[0];
            _backStack[1] = new String[0];
            _backStack[2] = new String[] { "Nested Frame Loose" };
            _backStack[3] = new String[] { "Nested Frame Loose" };
            _backStack[4] = new String[0];
            _backStack[5] = new String[0];
            _backStack[6] = new String[0];
            _backStack[7] = new String[0];
            _backStack[8] = new String[] { "Inner FramePage Loose" };
            _backStack[9] = new String[] { "Inner FramePage Loose" };
            _backStack[10] = new String[0];
            _backStack[11] = new String[0];
            _backStack[12] = new String[] { "Nested Frame Loose" };
            _backStack[13] = new String[] { "Nested Frame Loose" };

            _forwardStack = new String[14][];
            _forwardStack[0] = new String[0];
            _forwardStack[1] = new String[0];
            _forwardStack[2] = new String[0];
            _forwardStack[3] = new String[0];
            _forwardStack[4] = new String[] { "Page1_Loose.xaml" };
            _forwardStack[5] = new String[] { "Page1_Loose.xaml" };
            _forwardStack[6] = new String[] { "Page1_Loose.xaml" };
            _forwardStack[7] = new String[] { "Page1_Loose.xaml" };
            _forwardStack[8] = new String[0];
            _forwardStack[9] = new String[0];
            _forwardStack[10] = new String[] { "Lipsum, For The Masses" };
            _forwardStack[11] = new String[] { "Lipsum, For The Masses" };
            _forwardStack[12] = new String[0];
            _forwardStack[13] = new String[0];
        }
    }
}
