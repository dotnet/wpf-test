// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Description: FrameFocus tests that a control that has focus within
//  a frame will not retain focus when you navigate away and return to
//  the original contents.  This test also verifies that this holds true
//  for IslandFrame
//
//

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Test.Loaders;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Setup:
    /// 1.  Set focus to a control within the current Frame
    /// 2.  Navigate the Frame away from the current contents
    /// 3.  Return to the original contents of the Frame, and check 
    ///     if that control retained focus
    /// 4.  Do the above three steps for IslandFrame
    /// </summary>
    public class FrameFocus
    {
        internal enum CurrentTest
        {
            UnInit,
            SetFocusOnControl,
            FrameGoBack,
            End
        }

        #region Private Members
        private CurrentTest _frameFocusTest = CurrentTest.UnInit;
        private const String flowDocPage = @"FlowDocument_Loose.xaml";
        private const String controlsPage = @"ContentControls_Page.xaml";
        private const String controlsPageSourceStandalone = @"NavigationTests_Standalone;component/ContentControls_Page.xaml";
        private const String controlsPageSourceBrowser = @"NavigationTests_Browser;component/ContentControls_Page.xaml";
        private String _controlsPageSource = "";
        private FrameTestClass _frameTest = null;
        #endregion

        // empty constructor
        public FrameFocus()
        {
        }

        #region Test Delegates

        public void Startup(object sender, StartupEventArgs e)
        {
            // Instantiate the FrameTestClass
            _frameTest = new FrameTestClass("FrameFocus");
            _frameTest.FrameTestType = FrameTestClass.FrameType.Frame;
            _frameTest.RegisterNavEventHandlers = false;

            // determine if standalone or browser hosted
            if (AppDomain.CurrentDomain.FriendlyName.ToString().Contains(ApplicationDeploymentHelper.BROWSER_APPLICATION_EXTENSION))
            {
                _controlsPageSource = controlsPageSourceBrowser;
            }
            else
            {
                _controlsPageSource = controlsPageSourceStandalone;
            }

            // Actual event counts
            NavigationHelper.NumActualLoadCompletedEvents = 0;
            NavigationHelper.NumActualNavigatedEvents = 0;

            // Expected event counts
            NavigationHelper.NumExpectedLoadCompletedEvents = 7;
            NavigationHelper.NumExpectedNavigatedEvents = 7;
        }

        public void LoadCompleted(object sender, NavigationEventArgs e)
        {
            NavigationHelper.NumActualLoadCompletedEvents++;

            if (_frameTest.IsFirstRun &&
                _frameFocusTest == CurrentTest.UnInit)
            {
                _frameTest.SetupTest();

                if (_frameTest.StdFrame == null || _frameTest.IslandFrame == null)
                {
                    _frameTest.Fail("Could not locate Frame/IslandFrame to run FrameFocus test case on");
                }

                _frameTest.StdFrame.Source = new Uri(controlsPage, UriKind.RelativeOrAbsolute);

                _frameFocusTest = CurrentTest.SetFocusOnControl;
                _frameTest.IsFirstRun = false;

                // register with frame content rendered event handlers
                _frameTest.StdFrame.ContentRendered += new EventHandler(OnStdFrameContentRendered);
                _frameTest.IslandFrame.ContentRendered += new EventHandler(OnIslandFrameContentRendered);
            }
        }

        public void ContentRendered(object sender, EventArgs e)
        {
            _frameTest.Output("Current State - " + _frameFocusTest.ToString());

            Frame f = GetFrame(); // retrieve the frame

            if (_frameFocusTest == CurrentTest.SetFocusOnControl)
            {
                if (_frameTest.VerifySource(f, _controlsPageSource))
                {
                    // Set focus on TextBox in the Frame/IslandFrame's content area
                    SetFocusOnTextBox((DependencyObject)f.Content);
                    // Navigate Frame/IslandFrame away to a second page
                    _frameTest.Output("Navigating " + _frameTest.FrameTestType + " to " + flowDocPage);
                    f.Navigate(new Uri(flowDocPage, UriKind.RelativeOrAbsolute));
                    _frameFocusTest = CurrentTest.FrameGoBack;
                }
            }
            else
            {
                _frameTest.Output(_frameFocusTest.ToString() + " is not one of the predefined subtests.  Exiting test case.");
            }
        }

        // Frame ContentRendered event handler
        private void OnStdFrameContentRendered(object sender, EventArgs e)
        {
            _frameTest.Output("Current State - " + _frameFocusTest.ToString());
            _frameTest.Output("In OnStdFrameContentRendered");

            FrameGoBack();
        }

        // Frame ContentRendered event handler
        private void OnIslandFrameContentRendered(object sender, EventArgs e)
        {
            _frameTest.Output("Current State - " + _frameFocusTest.ToString());
            _frameTest.Output("In OnIslandFrameContentRendered");

            Frame f = GetFrame();

            if (_frameFocusTest == CurrentTest.SetFocusOnControl)
            {
                if (_frameTest.VerifySource(f, _controlsPageSource))
                {
                    // Set focus on TextBox in the Frame/IslandFrame's content area
                    SetFocusOnTextBox((DependencyObject)f.Content);
                    // Navigate Frame/IslandFrame away to a second page
                    _frameTest.Output("Navigating " + _frameTest.FrameTestType + " to " + flowDocPage);
                    f.Navigate(new Uri(flowDocPage, UriKind.RelativeOrAbsolute));
                    _frameFocusTest = CurrentTest.FrameGoBack;
                }
                return;
            }

            FrameGoBack();
        }

        // retrieves the correct frame
        private Frame GetFrame()
        {
            Frame f = null;
            switch (_frameTest.FrameTestType)
            {
                case FrameTestClass.FrameType.Frame:
                    _frameTest.Output("frame.Source = " + _frameTest.StdFrame.Source);
                    f = _frameTest.StdFrame;
                    break;

                case FrameTestClass.FrameType.IslandFrame:
                    _frameTest.Output("islandFrame.Source = " + _frameTest.IslandFrame.Source);
                    f = _frameTest.IslandFrame;
                    break;
            }

            return f;
        }

        // Handle GoBack on the frame
        private void FrameGoBack()
        {
            Frame f = GetFrame();

            switch (_frameFocusTest)
            {
                case CurrentTest.FrameGoBack:
                    if (_frameTest.VerifySource(f, flowDocPage))
                    {
                        // Go back in NavigationWindow (if testing Frame) or Frame (if testing IslandFrame)
                        _frameTest.Output("Calling GoBack() on " +
                            (_frameTest.FrameTestType == FrameTestClass.FrameType.IslandFrame ? "Frame" : "NavigationWindow"));
                        if (_frameTest.FrameTestType == FrameTestClass.FrameType.IslandFrame)
                        {
                            f.GoBack();
                        }
                        else
                        {
                            NavigationWindow navWin = Application.Current.MainWindow as NavigationWindow;
                            navWin.GoBack();
                        }
                        _frameFocusTest = CurrentTest.End;
                    }
                    break;

                case CurrentTest.End:
                    if (_frameTest.VerifySource(f, _controlsPageSource))
                    {
                        _frameTest.Output("Check: TextBox didn't retain focus after returning to original Frame contents");
                        if (!CheckFocusOnTextBox((DependencyObject)f.Content))
                        {
                            _frameTest.Output("Correct!  TextBox did not retain focus");

                            if (_frameTest.FrameTestType == FrameTestClass.FrameType.Frame)
                            {
                                _frameTest.Output("Frame subtest passed.  Switching to IslandFrame subtest.");
                                // Move onto testing IslandFrame
                                _frameTest.FrameTestType = FrameTestClass.FrameType.IslandFrame;
                                _frameFocusTest = CurrentTest.SetFocusOnControl;
                                _frameTest.IslandFrame.Source = new Uri(controlsPage, UriKind.RelativeOrAbsolute);
                            }
                            else if (_frameTest.FrameTestType == FrameTestClass.FrameType.IslandFrame)
                            {
                                _frameTest.Output("IslandFrame subtest passed");
                                if (_frameTest.VerifyEventCount() == true)
                                {
                                    _frameTest.Pass("TextBox did not retain focus after navigating back to Frame's original contents");
                                }
                                else
                                {
                                    _frameTest.Fail("EventCount verification failed.");
                                }
                            }
                        }
                        else
                        {
                            _frameTest.Fail("TextBox should not have focus after navigating back to " + controlsPage,
                                "TextBox still has focus");
                        }
                    }
                    else
                    {
                        _frameTest.Fail("Frame should be back on " + controlsPage,
                            "Frame is on " + f.Source);
                    }
                    break;

                default:
                    _frameTest.Output(_frameFocusTest.ToString() + " is not one of the predefined subtests.  Exiting test case.");
                    break;
            }
        }

        // Navigated event handler
        public void Navigated(object source, NavigationEventArgs e)
        {
            NavigationHelper.NumActualNavigatedEvents++;
        }

        #endregion


        private void SetFocusOnTextBox(DependencyObject tbContainer)
        {
            TextBox tb1 = LogicalTreeHelper.FindLogicalNode(tbContainer, "statusText") as TextBox;
            _frameTest.Output("Setting focus on TextBox");

            if (tb1 == null)
            {
                _frameTest.Fail("Could not find TextBox to set focus on it.");
            }
            else
            {
                tb1.Focus();
            }

            return;
        }

        private bool CheckFocusOnTextBox(DependencyObject tbContainer)
        {
            TextBox tb1 = LogicalTreeHelper.FindLogicalNode(tbContainer, "statusText") as TextBox;
            _frameTest.Output("Checking TextBox focus");

            if (tb1 == null)
            {
                _frameTest.Fail("Could not find TextBox to check its focus");
            }
            else
            {
                return tb1.IsFocused;
            }

            return false;
        }
    }
}
