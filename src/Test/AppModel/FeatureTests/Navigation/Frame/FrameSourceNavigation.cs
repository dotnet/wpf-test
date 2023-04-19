// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description:  FrameSourceNavigation tests that the Frame Source public
//  property is updated when the Navigating event is fired.  If the navigation
//  is cancelled then the Source property reverts to the value of CurrentSource.
//
//  [1] Navigate by setting Frame.Source (Source should be new value in the 
//      Navigating event)
//  [2] Navigate by setting NavigationWindow.Source
//  [3] Navigate by invoking Frame's NavigationService.Navigate 
//  [4] Navigate by invoking Frame.Navigate  
//  [5] Navigate by invoking NavigationWindow.Navigate
//  [6] Navigate by invoking NavigationWindow's NavigationService.Navigate
//  [7] Navigate by setting Frame's NavigationService.Source
//  [8] Navigate by setting NavigationWindow's NavigationService.Source
//  [9] Cancel navigation started by setting Frame.Source
//  [10] Cancel navigation started by setting NavigationWindow.Source
//  [11] Fragment navigation in Frame by setting Source property
//  [12] Fragment navigation in Frame by invoking Navigate method


using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Windows.Interop;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    public class FrameSourceNavigation
    {
        private enum CurrentTest
        {
            UnInit,
            FrameSource,
            FrameNavigate,
            FrameNavigationServiceSource,
            FrameNavigationServiceNavigate,
            FrameCancelNavigation,
            NavigationWindowSource,
            NavigationWindowNavigate,
            NavigationWindowNavigationServiceSource,
            NavigationWindowNavigationServiceNavigate,
            NavigationWindowCancelNavigation,
            FrameFragmentNavigationSource,
            FrameFragmentNavigationNavigate,
            End
        }

        #region FrameSourceNavigation privates

        private CurrentTest _frameSourceNavigationTest = CurrentTest.UnInit;
        private FrameTestClass _frameTest = null;
        private const String flowDocPage = @"FlowDocument_Loose.xaml";
        private const String anchoredPage = @"AnchoredPage_Loose.xaml";
        private const String controlsPage = @"ContentControls_Page.xaml";
        private const String bottomFragment = @"#bottom";
        private const String topFragment = @"#top";
        private const String frameSourceStandalone = @"NavigationTests_Standalone;component/";
        private const String frameSourceBrowser = @"NavigationTests_Browser;component/";
        private String _frameSource = ""; // this string get appended to the Frame.Source

        // use the following List to verify the sequence of expected states
        private List<CurrentTest> _navigatedStates = new List<CurrentTest>();
        #endregion

        public void Startup(object sender, StartupEventArgs e)
        {
            _frameTest = new FrameTestClass("FrameSourceNavigation");
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = false;

            // determine if standalone or browser hosted
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                _frameSource = frameSourceBrowser;
            }
            else
            {
                _frameSource = frameSourceStandalone;
            }

            // Set the expected navigation counts
            NavigationHelper.NumExpectedNavigatingEvents = 14;
            NavigationHelper.NumExpectedNavigatedEvents = 11;
            NavigationHelper.NumExpectedLoadCompletedEvents = 11;
        }

        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            _frameTest.Output("------In LoadCompleted. Current State = " + _frameSourceNavigationTest.ToString());
            NavigationHelper.NumActualLoadCompletedEvents++;

            _frameTest.Output("sender = " + sender.ToString());
            _frameTest.Output("e.Navigator = " + e.Navigator.ToString());

            if (_frameTest.IsFirstRun && _frameSourceNavigationTest == CurrentTest.UnInit)
            {
                _frameTest.SetupTest();

                if (_frameTest.StdFrame == null)
                {
                    _frameTest.Fail("Could not locate Frame to run FrameSourceNavigation test case on");
                }

                _frameTest.StdFrame.Source = new Uri(flowDocPage, UriKind.RelativeOrAbsolute);
                _frameSourceNavigationTest = CurrentTest.FrameSource;
                _frameTest.IsFirstRun = false;
                RouteFrameSourceNavigationSubtests();
            }
            else
            {
                switch (_frameSourceNavigationTest)
                {
                    case CurrentTest.FrameFragmentNavigationSource:
                        _frameSourceNavigationTest = CurrentTest.FrameFragmentNavigationNavigate;
                        RouteFrameSourceNavigationSubtests();
                        break;

                    case CurrentTest.FrameFragmentNavigationNavigate:
                        _frameSourceNavigationTest = CurrentTest.End;
                        RouteFrameSourceNavigationSubtests();
                        break;

                    case CurrentTest.FrameNavigate:
                        _frameSourceNavigationTest = CurrentTest.FrameNavigationServiceSource;
                        RouteFrameSourceNavigationSubtests();
                        break;

                    case CurrentTest.FrameNavigationServiceSource:
                        _frameSourceNavigationTest = CurrentTest.FrameNavigationServiceNavigate;
                        RouteFrameSourceNavigationSubtests();
                        break;

                    case CurrentTest.FrameNavigationServiceNavigate:
                        _frameSourceNavigationTest = CurrentTest.FrameCancelNavigation;
                        RouteFrameSourceNavigationSubtests();
                        break;
                }
            }
        }

        #region event handlers
        /// <summary>
        /// Checks to see that:
        /// [1] the Uri in the event args is not an absolute Uri
        /// [2] CurrentSource now reflects the value of Source, which is the Uri used in the navigation
        /// and in Frame case only: [3] SourceProperty DP matches e.Uri, CurrentSource and Source
        /// </summary>
        public void Navigated(object sender, NavigationEventArgs e)
        {
            _frameTest.Output("------In Navigated. Current State = " + _frameSourceNavigationTest.ToString());
            _navigatedStates.Add(_frameSourceNavigationTest);
            NavigationHelper.NumActualNavigatedEvents++;

            // Check to see if the Uri in the event args is absolute
            if (e.Uri.IsAbsoluteUri)
            {
                _frameTest.Fail("EXPECTED: Uri in event args should be relative; ACTUAL: Uri in event args is absolute");
            }

            // CurrentSource should be updated to equal the value of Source (the Uri we navigated to)
            if (e.Navigator is NavigationWindow)
            {
                // NavigationWindow case
                NavigationWindow eNavWin = e.Navigator as NavigationWindow;

                if (_frameTest.VerifyCurrentSource(eNavWin, eNavWin.Source.ToString()) &&
                    _frameTest.VerifyCurrentSource(eNavWin, e.Uri.ToString()))
                {
                    _frameTest.Output("CurrentSource = Source = e.Uri");
                }
                else
                {
                    _frameTest.Fail("EXPECTED: CurrentSource = Source = e.Uri; ACTUAL: CurrentSource != Source or CurrentSource != e.Uri");
                }
            }
            else if (e.Navigator is Frame)
            {
                // Frame case
                Frame eFrame = e.Navigator as Frame;

                if (_frameTest.VerifyCurrentSource(_frameTest.StdFrame, _frameTest.StdFrame.Source.ToString()) &&
                    _frameTest.VerifyCurrentSource(_frameTest.StdFrame, e.Uri.ToString()))
                {
                    _frameTest.Output("CurrentSource = Source = e.Uri");
                    if ((Uri)_frameTest.StdFrame.GetValue(Frame.SourceProperty) != e.Uri)
                    {
                        _frameTest.Fail("EXPECTED: SourceProperty DP == e.Uri; ACTUAL: SourceProperty DP = "
                            + (Uri)_frameTest.StdFrame.GetValue(Frame.SourceProperty) + ", e.Uri = " + e.Uri.ToString());
                    }
                    else
                    {
                        _frameTest.Output("SourceProperty DP == e.Uri");
                    }
                }
                else
                {
                    _frameTest.Fail("EXPECTED: CurrentSource = Source = e.Uri; ACTUAL: CurrentSource != Source or CurrentSource != e.Uri");
                }
            }
        }

        /// <summary>
        /// Checks to see if :
        /// [1] the Source (CLR) property was updated to reflect the Uri used in navigation.
        /// [2] the Uri in the event args is not an absolute Uri
        /// And [3] cancels the Navigating event, if we are in a CancelNavigation test
        /// </summary>
        public void Navigating(object sender, NavigatingCancelEventArgs e)
        {
            _frameTest.Output("------In Navigating. Current State = " + _frameSourceNavigationTest.ToString());
            NavigationHelper.NumActualNavigatingEvents++;
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;

            // Check to see if the Uri in the event args is absolute
            if (e.Uri.IsAbsoluteUri)
            {
                _frameTest.Fail("EXPECTED: Uri in event args should be relative; ACTUAL: Uri in event args is absolute");
            }

            // In a CancelNavigation test, cancel that navigation
            if (_frameSourceNavigationTest.ToString().Contains("CancelNavigation"))
            {
                e.Cancel = true;
                return;
            }

            // Check that the Source property has changed to be the Uri used for the navigation
            if (_frameSourceNavigationTest != CurrentTest.UnInit)
            {
                String sourceStr = String.Empty;
                if (e.Navigator is NavigationWindow)
                {
                    sourceStr = navWin.Source.ToString();
                }
                else if (e.Navigator is Frame)
                {
                    if (_frameSourceNavigationTest == CurrentTest.FrameSource || _frameSourceNavigationTest == CurrentTest.FrameFragmentNavigationSource) 
                    {
                        // In the states FrameSource or FrameFragmentNavigationSource we expect the source to be appended with frameSource
                        sourceStr = _frameSource + _frameTest.StdFrame.Source.ToString();
                    }
                    else
                    {
                        sourceStr = _frameTest.StdFrame.Source.ToString();
                    }
                }

                if (e.Navigator is NavigationWindow && _frameTest.VerifySource(navWin, e.Uri.ToString()))
                {
                    // NavigationWindow case
                    _frameTest.Output("Source has been set to the Uri used in navigation");
                }
                else if (e.Navigator is Frame && String.Compare(sourceStr, e.Uri.ToString()) == 0)
                {
                    // Frame case
                    _frameTest.Output("Source has been set to the Uri used in navigation");
                }
                else
                {
                    _frameTest.Fail("EXPECTED: Source (e.Uri) is " + e.Uri.ToString() + "; ACTUAL: Source is " + sourceStr);
                }
            }

            return;
        }


        /// <summary>
        /// Changes the CurrentTest to the next one on the list, and runs the subtest associated with it
        /// </summary>
        public void ContentRendered(object sender, EventArgs e)
        {
            _frameTest.Output("------In ContentRendered. Current State = " + _frameSourceNavigationTest.ToString());

            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;

            if (_frameSourceNavigationTest.ToString().Contains("NavigationWindow"))
            {
                _frameTest.Output("NavigationWindow.Source = " + navWin.Source.ToString());
            }
            else if (_frameSourceNavigationTest.ToString().Contains("Frame"))
            {
                _frameTest.Output("Frame.Source = " + _frameTest.StdFrame.Source.ToString());
            }

            switch (_frameSourceNavigationTest)
            {
                case CurrentTest.FrameSource:
                    _frameSourceNavigationTest = CurrentTest.FrameNavigate;
                    RouteFrameSourceNavigationSubtests();
                    break;

                case CurrentTest.FrameNavigate:
                    _frameSourceNavigationTest = CurrentTest.FrameNavigationServiceSource;
                    RouteFrameSourceNavigationSubtests();
                    break;

                case CurrentTest.FrameNavigationServiceSource:
                    _frameSourceNavigationTest = CurrentTest.FrameNavigationServiceNavigate;
                    RouteFrameSourceNavigationSubtests();
                    break;

                case CurrentTest.FrameNavigationServiceNavigate:
                    _frameSourceNavigationTest = CurrentTest.FrameFragmentNavigationSource;
                    RouteFrameSourceNavigationSubtests();
                    break;

                case CurrentTest.FrameFragmentNavigationSource:
                case CurrentTest.FrameFragmentNavigationNavigate:
                case CurrentTest.FrameCancelNavigation:
                    break;

                case CurrentTest.NavigationWindowSource:
                    _frameSourceNavigationTest = CurrentTest.NavigationWindowNavigate;
                    RouteFrameSourceNavigationSubtests();
                    break;

                case CurrentTest.NavigationWindowNavigate:
                    _frameSourceNavigationTest = CurrentTest.NavigationWindowNavigationServiceSource;
                    RouteFrameSourceNavigationSubtests();
                    break;

                case CurrentTest.NavigationWindowNavigationServiceSource:
                    _frameSourceNavigationTest = CurrentTest.NavigationWindowNavigationServiceNavigate;
                    RouteFrameSourceNavigationSubtests();
                    break;

                case CurrentTest.NavigationWindowNavigationServiceNavigate:
                    _frameSourceNavigationTest = CurrentTest.NavigationWindowCancelNavigation;
                    RouteFrameSourceNavigationSubtests();
                    break;

                // This shouldn't get hit, since we cancel the navigation
                // and don't re-render the contents of the NavigationWindow
                case CurrentTest.NavigationWindowCancelNavigation:
                    _frameTest.Fail("Content Rendered fired in state NavigationWindowCancelNavigation");
                    break;

                default:
                    _frameTest.Fail(_frameSourceNavigationTest.ToString() + " was not one of the predefined tests. Exiting test case.");
                    break;
            }
        }

        public void FragmentNavigation(object sender, FragmentNavigationEventArgs e)
        {
            _frameTest.Output("sender: " + sender.ToString());
            _frameTest.Output("FragmentNavigation args: e.Fragment = " + e.Fragment + "; e.Handled = " + e.Handled + "; e.Navigator = " + e.Navigator);
        }
        #endregion

        /// <summary>
        /// This runs the appropriate subtest, depending on which CurrentTest 
        /// value we currently hold
        /// </summary>
        private void RouteFrameSourceNavigationSubtests()
        {
            bool testPassed = true;
            _frameTest.Output("Routing subtests...");
            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;

            switch (_frameSourceNavigationTest)
            {
                #region Frame subtests
                case CurrentTest.FrameSource:
                    _frameTest.Output("[1] Navigate using Frame.Source");
                    testPassed = FrameSourceNavigationTestHelper(_frameTest.StdFrame, controlsPage);
                    break;

                case CurrentTest.FrameNavigate:
                    _frameTest.Output("[2] Navigate using Frame.Navigate");
                    testPassed = FrameSourceNavigationTestHelper(_frameTest.StdFrame, anchoredPage);
                    break;

                case CurrentTest.FrameNavigationServiceSource:
                    _frameTest.Output("[3] Navigate using Frame's NavigationService.Source");
                    testPassed = FrameSourceNavigationTestHelper(_frameTest.StdFrame, controlsPage);
                    break;

                case CurrentTest.FrameNavigationServiceNavigate:
                    _frameTest.Output("[4] Navigate using Frame's NavigationService.Navigate");
                    testPassed = FrameSourceNavigationTestHelper(_frameTest.StdFrame, anchoredPage);
                    break;

                case CurrentTest.FrameFragmentNavigationSource:
                    _frameTest.Output("[11] Fragment navigation in Frame using Source property");
                    testPassed = FrameSourceNavigationTestHelper(_frameTest.StdFrame, anchoredPage + topFragment);
                    break;

                case CurrentTest.FrameFragmentNavigationNavigate:
                    _frameTest.Output("[12] Fragment navigation in Frame using Navigate method");
                    testPassed = FrameSourceNavigationTestHelper(_frameTest.StdFrame, anchoredPage + bottomFragment);
                    break;

                case CurrentTest.FrameCancelNavigation:
                    _frameTest.Output("[7] Cancel Frame navigation");
                    testPassed = FrameSourceNavigationTestHelper(_frameTest.StdFrame, anchoredPage);
                    break;
                #endregion

                #region NavigationWindow subtests
                case CurrentTest.NavigationWindowSource:
                    _frameTest.Output("[8] Navigate using NavigationWindow.Source");
                    testPassed = FrameSourceNavigationTestHelper(navWin, anchoredPage);
                    break;

                case CurrentTest.NavigationWindowNavigate:
                    _frameTest.Output("[9] Navigate using NavigationWindow.Navigate");
                    testPassed = FrameSourceNavigationTestHelper(navWin, controlsPage);
                    break;

                case CurrentTest.NavigationWindowNavigationServiceSource:
                    _frameTest.Output("[10] Navigate using NavigationWindow's NavigationService.Source");
                    testPassed = FrameSourceNavigationTestHelper(navWin, flowDocPage);
                    break;

                case CurrentTest.NavigationWindowNavigationServiceNavigate:
                    _frameTest.Output("[5] Navigate using NavigationWindow's NavigationService.Navigate");
                    testPassed = FrameSourceNavigationTestHelper(navWin, anchoredPage);
                    break;

                case CurrentTest.NavigationWindowCancelNavigation:
                    _frameTest.Output("[6] Cancel NavigationWindow navigation");
                    testPassed = FrameSourceNavigationTestHelper(navWin, controlsPage);
                    break;
                #endregion

                case CurrentTest.End:
                    // verify the sequence of navigated states
                    VerifyNavigatedStates();

                    if (_frameTest.VerifyEventCount())
                    {
                        _frameTest.Pass("Event counts matched.");
                    }
                    else
                    {
                        _frameTest.Fail("Event counts did not match.");
                    }

                    break;
            }

            if (testPassed == false)
            {
                _frameTest.Fail("Failing Test");
            }

            return;
        }


        /// <summary>
        /// This initiates a navigation several ways: by setting the Source property on Frame and
        /// NavigationWindow, by invoking the Navigate method, by setting the NavigationService's 
        /// Source property and by invoking the NavigationService's Navigate method
        /// </summary>
        /// <param name="c">Frame/NavigationWindow</param>
        /// <param name="targetUri">String to be used in Uri</param>
        /// <returns>true if Source was set with no exceptions thrown, false otherwise</returns>
        private bool FrameSourceNavigationTestHelper(ContentControl c, String targetUri)
        {
            if (c == null)
            {
                _frameTest.Output("EXPECTED: ContentControl is a Frame/NavigationWindow; ACTUAL: ContentControl is null");
                return false;
            }

            if (!(c is NavigationWindow) && !(c is Frame))
            {
                _frameTest.Output("EXPECTED: ContentControl is a Frame/NavigationWindow; ACTUAL: ContentControl is " + c.ToString());
                return false;
            }

            Uri destinationUri = new Uri(targetUri, UriKind.RelativeOrAbsolute);

            if (c is Frame)
            {
                Frame cFrame = c as Frame;
                try
                {
                    if (_frameSourceNavigationTest.ToString().Contains("FrameSource") ||
                        _frameSourceNavigationTest.ToString().Contains("FrameFragmentNavigationSource"))
                    {
                        _frameTest.Output("Setting Frame's Source property to " + targetUri);
                        cFrame.Source = destinationUri;
                        return true;
                    }
                    else if (_frameSourceNavigationTest.ToString().Contains("FrameNavigate") ||
                        _frameSourceNavigationTest.ToString().Contains("FrameFragmentNavigationNavigate"))
                    {
                        _frameTest.Output("Invoking Frame's Navigate method to " + targetUri);
                        cFrame.Navigate(destinationUri);
                        return true;
                    }
                    else if (_frameSourceNavigationTest.ToString().Contains("FrameNavigationServiceSource"))
                    {
                        _frameTest.Output("Setting Frame's NavigationService.Source property to " + targetUri);
                        NavigationService.GetNavigationService((DependencyObject)cFrame.Content).Source = destinationUri;
                        return true;
                    }
                    else if (_frameSourceNavigationTest.ToString().Contains("FrameNavigationServiceNavigate"))
                    {
                        _frameTest.Output("Invoking Frame's NavigationService.Navigate method to " + targetUri);
                        NavigationService.GetNavigationService((DependencyObject)cFrame.Content).Navigate(destinationUri);
                        return true;
                    }
                    else if (_frameSourceNavigationTest.ToString().Contains("FrameCancelNavigation"))
                    {
                        _frameTest.Output("Navigating Frame to " + targetUri + " and cancelling navigation");
                        cFrame.Source = destinationUri;

                        // Check to see that the cancel actually happened
                        if (!_frameTest.VerifySource(cFrame, cFrame.CurrentSource.ToString()))
                        {
                            _frameTest.Fail("EXPECTED: After cancel, Source == CurrentSource; ACTUAL: Source = " + cFrame.Source.ToString());
                        }
                        else
                        {
                            _frameTest.Output("Source == CurrentSource");
                            // ContentRendered, Navigated events won't get fired, so start off next test case here
                            _frameSourceNavigationTest = CurrentTest.NavigationWindowSource;
                            RouteFrameSourceNavigationSubtests();
                            return true;
                        }
                    }
                    else
                    {
                        _frameTest.Fail("Unknown state reached.");
                    }
                }
                catch (Exception exp)
                {
                    _frameTest.Fail("Unexpected exception caught: " + exp.ToString());
                }
            } 

            else if (c is NavigationWindow)
            {
                NavigationWindow cNavWin = c as NavigationWindow;
                try
                {
                    if (_frameSourceNavigationTest.ToString().Contains("NavigationWindowSource"))
                    {
                        _frameTest.Output("Setting NavigationWindow's Source property to " + targetUri);
                        cNavWin.Source = destinationUri;
                        return true;
                    }
                    else if (_frameSourceNavigationTest.ToString().Contains("NavigationWindowNavigate"))
                    {
                        _frameTest.Output("Invoking NavigationWindow's Navigate method to " + targetUri);
                        cNavWin.Navigate(destinationUri);
                        return true;
                    }
                    else if (_frameSourceNavigationTest.ToString().Contains("NavigationWindowNavigationServiceSource"))
                    {
                        _frameTest.Output("Setting NavigationWindow's NavigationService.Source property to " + targetUri);
                        NavigationService.GetNavigationService((DependencyObject)cNavWin.Content).Source = destinationUri;
                        return true;
                    }
                    else if (_frameSourceNavigationTest.ToString().Contains("NavigationWindowNavigationServiceNavigate"))
                    {
                        _frameTest.Output("Invoking NavigationWindow's NavigationService.Navigate method to " + targetUri);
                        NavigationService.GetNavigationService((DependencyObject)cNavWin.Content).Navigate(destinationUri);
                        return true;
                    }
                    else if (_frameSourceNavigationTest.ToString().Contains("NavigationWindowCancelNavigation"))
                    {
                        _frameTest.Output("Navigating NavigationWindow to " + targetUri + " and cancelling navigation");
                        cNavWin.Source = destinationUri;

                        // Check to see that cancel really happened
                        if (!_frameTest.VerifySource(cNavWin, cNavWin.CurrentSource.ToString()))
                        {
                            _frameTest.Fail("EXPECTED: After cancel, Source == CurrentSource; ACTUAL: Source = " + cNavWin.Source.ToString());
                        }
                        else
                        {
                            _frameTest.Output("Source == CurrentSource");
                            // ContentRendered, Navigated events won't get fired, so start off next test case here
                            _frameSourceNavigationTest = CurrentTest.FrameFragmentNavigationSource;
                            RouteFrameSourceNavigationSubtests();
                            return true;
                        }
                    }
                    else
                    {
                        _frameTest.Fail("Unknown state reached.");
                    }
                }
                catch (Exception exp)
                {
                    _frameTest.Fail("Unexpected exception caught: " + exp.ToString());
                }
            }

            // Shouldn't be able to get to this point
            return false;
        }

        // verify the sequence of navigated events fired
        // this will fail if you change the test to switch some states
        private void VerifyNavigatedStates()
        {
            for (int i = 0; i < _navigatedStates.Count; i++)
            {
                if (i > 0 && (_navigatedStates[i] <= _navigatedStates[i - 1]))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append("VerifyStates failed. navigatedStates[");
                    sb.Append(i.ToString());
                    sb.Append("]");
                    sb.Append(" = ");
                    sb.Append(_navigatedStates[i].ToString());
                    sb.Append("; navigatedStates[");
                    sb.Append((i+1).ToString());
                    sb.Append("]");
                    sb.Append(" = ");
                    sb.Append(_navigatedStates[i+1].ToString());

                    _frameTest.Fail(sb.ToString());
                }
            }
        }
    }
}
