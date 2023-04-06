// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  NavigateFrameToObject tests several things using a Hyperlink
//  contained in a Frame, including:
//  [1] Hyperlink navigation to PageFunction
//  [2] Hyperlink navigation to a non-Page/PageFunction object (i.e. Image?)
//  [3] Hyperlink navigation to a Page containing a Frame, and then navigating
//      the nested Frames 
//

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // NavigateFrameToObject
    public class NavigateFrameToObject
    {
        private enum CurrentTest
        {
            UnInit,
            hlink_PageFunction,
            hlink_PageWithFrame,
            hlink_NonPageObject,
            #region BUGBUG 1309270
            hlink_JPEGFile,
            #endregion
            End
        }

        #region NavigateFrameToObject globals
        CurrentTest _navFrameToObjTest = CurrentTest.UnInit;
        private int _navFrameToObjSubTest = 0;
        private Frame _homeFrame = null;

        private JournalHelper _navFrameToObjJHelper = null;
        private NavigationStateCollection _navFrameToObjActualStates = new NavigationStateCollection();
        private NavigationStateCollection _navFrameToObjExpectedStates = new NavigationStateCollection();

        private bool _navFrameToObjInVerifyMode = true;
        private const string navFrameToObjResults = @"NavigateFrameToObjectResults_Loose.xml";
        FrameTestClass _frameTest = null;

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
            _frameTest = new FrameTestClass("NavigateFrameToObject");
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = false;

            /*
             * This code no longer works because of the error "cscomp.dll is missing" 
             * You may use it when this issue is fixed
            if (navFrameToObjInVerifyMode)
                navFrameToObjExpectedStates = NavigationStateCollection.GetResults(
                    Application.GetRemoteStream(new Uri(navFrameToObjResults, UriKind.RelativeOrAbsolute)).Stream);
             */
            if (_navFrameToObjInVerifyMode)
            {
                CreateExpectedNavigationStates();

                // populate the expected navigation states
                for (int index = 0; index < _stateDescription.Length; index++)
                {
                    _navFrameToObjExpectedStates.states.Add(new NavigationState(_stateDescription[index],
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
            _frameTest.Output("NavigateFrameToObject.LoadCompleted -- e.Navigator is " + e.Navigator.ToString());

            if (_navFrameToObjJHelper != null)
            {
                _navFrameToObjActualStates.RecordNewResult(_navFrameToObjJHelper, "App LoadCompleted Navigator = " + e.Navigator + " uri = " + e.Uri);
            }

            if (_frameTest.IsFirstRun &&
                _navFrameToObjTest == CurrentTest.UnInit)
            {
                _frameTest.SetupTest();
                
                _navFrameToObjJHelper = new JournalHelper(_frameTest.NavigationWindow);

                _homeFrame = LogicalTreeHelper.FindLogicalNode(((NavigationWindow)Application.Current.MainWindow).Content as DependencyObject, "testFrame") as Frame;
                _homeFrame.ContentRendered += new EventHandler(ContentRendered);

                if (_navFrameToObjJHelper == null)
                {
                    _frameTest.Fail("JournalHelper could not be initialized and is still null. Exiting test.");
                }

                if (_frameTest.StdFrame == null)
                {
                    _frameTest.Fail("Could not locate Frame to run NavigateFrameToObject test case on");
                }

                // frameTest.StdFrame.Source = new Uri(CONTROLSPAGE, UriKind.RelativeOrAbsolute);
                _frameTest.StdFrame.Source = new Uri("NavigateFrameToObject_FrameContents.xaml", UriKind.RelativeOrAbsolute);


                _frameTest.IsFirstRun = false;
                _navFrameToObjTest = CurrentTest.hlink_PageFunction;
            }
        }

        public void ContentRendered(object sender, EventArgs e)
        {
            if (sender is Frame)
            {
                _frameTest.Output("Frame.Source = " + _frameTest.StdFrame.Source);
                _navFrameToObjActualStates.RecordNewResult(_navFrameToObjJHelper, "NavigateFrameToObject Source = " + _frameTest.StdFrame.Source);

                switch (_navFrameToObjTest)
                {
                    case CurrentTest.hlink_PageFunction:
                        RoutePageFunction();
                        break;

                    case CurrentTest.hlink_PageWithFrame:
                        RoutePageWithFrame();
                        break;

                    case CurrentTest.hlink_NonPageObject:
                        RouteNonPageObject();
                        break;

                    case CurrentTest.hlink_JPEGFile:
                        RouteJPEGFile();
                        break;

                    case CurrentTest.End:
                        // Pass or fail?
                        if (_navFrameToObjInVerifyMode)
                        {
                            if (NavigationStateCollection.Compare(_navFrameToObjActualStates, _navFrameToObjExpectedStates))
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
                            _navFrameToObjActualStates.WriteResults(navFrameToObjResults);
                        }
                        break;

                    default:
                        _frameTest.Fail("Subtest was not one of the predefined tests. Exiting test case.");
                        break;
                }
            }
        }

        #region BUGBUG 1309270
        private void RouteJPEGFile()
        {
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            switch (_navFrameToObjSubTest)
            {
                case 6:
                    _frameTest.Output("Navigate to a JPEG file [included content in project]");
                    // Click on the Hyperlink that will navigate this Frame to the JPEG file
                    // specified in the NavigateUri
                    BrowserHelper.NavigateHyperlinkViaEvent(_navFrameToObjTest.ToString(), _frameTest.StdFrame.Content as DependencyObject);
                    break;

                case 7:
                    _frameTest.Output("Frame currently displaying a JPEG file. Returning to previous contents");
                    // GoBack to starting page
                    _frameTest.GoBack(navWin);
                    _navFrameToObjTest = CurrentTest.End;
                    break;
            }
            _navFrameToObjSubTest++;
            return;
        }
        #endregion

        private void RouteNonPageObject()
        {
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            switch (_navFrameToObjSubTest)
            {
                case 4:
                    _frameTest.Output("Navigation to a non-Page object");
                    // Click on the Hyperlink that will navigate this Frame to 
                    // a non Page object
                    BrowserHelper.NavigateHyperlinkViaEvent(_navFrameToObjTest.ToString(), _frameTest.StdFrame.Content as DependencyObject);
                    break;

                case 5:
                    _frameTest.Output("Frame contains a non-Page object. In this case a JPG. Returning to previous content");
                    // GoBack to starting page
                    _frameTest.GoBack(navWin);
                    #region BUGBUG 1309270
                    // uncomment the line below and remove the next line after it
                
                    //navFrameToObjTest = CurrentTest.hlink_JPEGFile;
                    #endregion
                    _navFrameToObjTest = CurrentTest.End;
                    break;
            }
            _navFrameToObjSubTest++;
            return;
        }

        private void RoutePageWithFrame()
        {
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
            switch (_navFrameToObjSubTest)
            {
                case 2:
                    _frameTest.Output("Navigation to a page containing another frame");
                    // Click on the Hyperlink that will navigate this Frame to 
                    // a Page containing another Frame
                    BrowserHelper.NavigateHyperlinkViaEvent(_navFrameToObjTest.ToString(), _frameTest.StdFrame.Content as DependencyObject);
                    break;

                case 3:
                    _frameTest.Output("Frame contains another page with a frame. Returning to previous content");
                    // GoBack to starting page
                    _frameTest.GoBack(navWin);
                    _navFrameToObjTest = CurrentTest.hlink_NonPageObject;
                    break;
            }
            _navFrameToObjSubTest++;
            return;
        }

        private void RoutePageFunction()
        {
            switch (_navFrameToObjSubTest)
            {
                case 0:
                    _frameTest.Output("Navigation to a PageFunction");
                    // Click on the Hyperlink that will navigate this Frame to 
                    // a PageFunction
                    BrowserHelper.InvokeHyperlink(_navFrameToObjTest.ToString(), _frameTest.StdFrame.Content as DependencyObject);
                    break;

                case 1:
                    _frameTest.Output("Currently on the PageFunction. Returning to parent Page");
                    // Click on the button that will navigate this PageFunction back to the parent page
                    BrowserHelper.InvokeButton("LNKDone", _frameTest.StdFrame.Content as DependencyObject);
                    _navFrameToObjTest = CurrentTest.hlink_PageWithFrame;
                    break;
            }
            _navFrameToObjSubTest++;
            return;
        }

        /// <summary>
        /// Create expected navigation states 
        /// </summary>
        void CreateExpectedNavigationStates()
        {
            _stateDescription
                = new String[]{
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml uri = NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml",
                        "NavigateFrameToObject Source = NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame uri = ",
                        "NavigateFrameToObject Source = ",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml uri = NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml",
                        "NavigateFrameToObject Source = NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/NavigateFrameToObject_PageWithFrame_Content.xaml uri = NavigationTests_Standalone;component/NavigateFrameToObject_PageWithFrame_Content.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame uri = ",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/NavigateFrameToObject_PageWithFrame.xaml uri = NavigationTests_Standalone;component/NavigateFrameToObject_PageWithFrame.xaml",
                        "NavigateFrameToObject Source = NavigationTests_Standalone;component/NavigateFrameToObject_PageWithFrame.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml uri = NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml",
                        "NavigateFrameToObject Source = NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/NavigateFrameToObject_VomitWithLove.xaml uri = NavigationTests_Standalone;component/NavigateFrameToObject_VomitWithLove.xaml",
                        "NavigateFrameToObject Source = NavigationTests_Standalone;component/NavigateFrameToObject_VomitWithLove.xaml",
                        "App LoadCompleted Navigator = System.Windows.Controls.Frame: NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml uri = NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml",
                        "NavigateFrameToObject Source = NavigationTests_Standalone;component/NavigateFrameToObject_FrameContents.xaml"
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
                        true,
                        true,
                        false,
                        false,
                        true,
                        true
                };

            _backStack = new String[16][];
            _backStack[0] = new String[0];
            _backStack[1] = new String[0];
            _backStack[2] = new String[] { "NavigateFrameToObject_FrameContents.xaml" };
            _backStack[3] = new String[] { "NavigateFrameToObject_FrameContents.xaml" };
            _backStack[4] = new String[0];
            _backStack[5] = new String[0];
            _backStack[6] = new String[] { "NavigateFrameToObject_FrameContents.xaml" };
            _backStack[7] = new String[] { "NavigateFrameToObject_FrameContents.xaml" };
            _backStack[8] = new String[] { "NavigateFrameToObject_FrameContents.xaml" };
            _backStack[9] = new String[] { "NavigateFrameToObject_FrameContents.xaml" };
            _backStack[10] = new String[0];
            _backStack[11] = new String[0];
            _backStack[12] = new String[] { "NavigateFrameToObject_FrameContents.xaml" };
            _backStack[13] = new String[] { "NavigateFrameToObject_FrameContents.xaml" };
            _backStack[14] = new String[0];
            _backStack[15] = new String[0];

            _forwardStack = new String[16][];
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
            _forwardStack[10] = new String[] { "NavigateFrameToObject_PageWithFrame.xaml" };
            _forwardStack[11] = new String[] { "NavigateFrameToObject_PageWithFrame.xaml" };
            _forwardStack[12] = new String[0];
            _forwardStack[13] = new String[0];
            _forwardStack[14] = new String[] { "NavigateFrameToObject_VomitWithLove.xaml" };
            _forwardStack[15] = new String[] { "NavigateFrameToObject_VomitWithLove.xaml" };
        }
    }
}
