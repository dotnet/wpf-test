// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Threading;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Keyboard.FocusedElement property is set properly after changing frame content.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <




    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyboardFocusedFrameApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Focus", "HwndSource", @"Compile and Verify Keyboard.FocusedElement property is set properly after changing frame content in HwndSource.")]
        [TestCase("2", @"CoreInput\Focus", "Browser", @"Compile and Verify Keyboard.FocusedElement property is set properly after changing frame content in Browser.")]
        [TestCase("2", @"CoreInput\Focus", "Window", @"Compile and Verify Keyboard.FocusedElement property is set properly after changing frame content in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "KeyboardFocusedFrameApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Focus", "HwndSource", @"Verify Keyboard.FocusedElement property is set properly after changing frame content in HwndSource.")]
        [TestCase("1", @"CoreInput\Focus", "Window", @"Verify Keyboard.FocusedElement property is set properly after changing frame content in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyboardFocusedFrameApp(), "Run");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            Canvas cvs = new Canvas();
            cvs.Focusable = true;
            cvs.Name = "rootCanvas";

            CoreLogger.LogStatus("Adding element to our main canvas....");
            FrameworkElement panel = new InstrFrameworkPanel();
            panel.Name = "nonfocusbtn" + DateTime.Now.Ticks;
            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);
            panel.Height = 40;
            panel.Width = 40;
            panel.Focusable = true;
            cvs.Children.Add(panel);

            CoreLogger.LogStatus("Adding frame to our main canvas....");
            _frame = new Frame();

            _frame.Name = "focusfrm" + DateTime.Now.Ticks;
            Canvas.SetTop(_frame, 50);
            Canvas.SetLeft(_frame, 50);
            _frame.Height = 40;
            _frame.Width = 40;
            _frame.Focusable = true;
            CoreLogger.LogStatus("Created frame. frame content=" + _frame.Content);

            CoreLogger.LogStatus("  Adding secondary canvas to our frame....");
            Canvas frameCanvas = new Canvas();
            _framePanel = new InstrFrameworkPanel();
            _framePanel.Name = "focusbtnframe" + DateTime.Now.Ticks;
            Canvas.SetTop(_framePanel, 0);
            Canvas.SetLeft(_framePanel, 0);
            _framePanel.Height = 20;
            _framePanel.Width = 20;
            _framePanel.Focusable = true;

            frameCanvas.Children.Add(_framePanel);
            _frame.Content = frameCanvas;

            cvs.Children.Add(_frame);

            _rootElement = cvs;

            // Put the test element on the screen
            DisplayMe(_rootElement, 1, 1, 100, 100);

            CoreLogger.LogStatus("Frame added. frame content=" + _frame.Content);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            Canvas cvs = (Canvas)(_rootElement);
            CoreLogger.LogStatus("Children in canvas: (expect 2) " + cvs.Children.Count);
            Assert(cvs.Children.Count == 2, "Oh no! Incorrect number of elements in tree");

            // STEP 0
            CoreLogger.LogStatus("Focusing on the parent....");
            bool bFocus = _rootElement.Focus();
            CoreLogger.LogStatus("Focus set? (expect true) " + bFocus);
            CoreLogger.LogStatus("Startup focused element: " + Keyboard.FocusedElement);

            CoreLogger.LogStatus("We just added frame content. frame content=" + _frame.Content);
            Assert(_frame.Content != null, "Oh no! Frame content is supposed to be non-null!");

            // STEP 1
            CoreLogger.LogStatus("Focusing on element outside frame...");
            UIElement e = cvs.Children[0];
            bFocus = e.Focus();
            CoreLogger.LogStatus("Focus element succeeded? " + bFocus);

            // STEP 2
            CoreLogger.LogStatus("Focusing on element inside frame...");
            bFocus = _framePanel.Focus();
            CoreLogger.LogStatus("Focus on framed element succeeded? " + bFocus);
            CoreLogger.LogStatus("Element in focus pre-content-change: (expect non-null el) '" + Keyboard.FocusedElement + "'");

            Assert(Keyboard.FocusedElement == _framePanel, "Item not in focus! (expected framed element) ");

            // STEP 3
            CoreLogger.LogStatus("Changing content to different tree...");
            //_frame.Content = null;
            _frame.Content = ConstructTreeWithFocusableElement(true);
            CoreLogger.LogStatus("Content changed.  frame content=" + _frame.Content);

            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Construct simple content control with one child element that may or may not be focusable.
        /// </summary>
        /// <param name="isFocusable">Is this child focusable?</param>
        /// <returns>New control.</returns>
        private object ConstructTreeWithFocusableElement(bool isFocusable)
        {
            StackPanel sp = new StackPanel();

            FrameworkElement panel = new InstrFrameworkPanel();
            panel.Name = "nonfocusbtn" + DateTime.Now.Ticks;
            panel.Height = 40;
            panel.Width = 40;
            panel.Focusable = isFocusable;
            sp.Children.Add(panel);
            sp.Orientation = Orientation.Vertical;

            return sp;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            CoreLogger.LogStatus("In validation stage. frame content=" + _frame.Content);

            Canvas cvs = (Canvas)(_rootElement);
            CoreLogger.LogStatus("Children in canvas: (expect 2) " + cvs.Children.Count);
            Assert(cvs.Children.Count == 2, "Unexpected element present!");

            CoreLogger.LogStatus("Element in focus post-removal: (expect non-null element) '" + Keyboard.FocusedElement + "'");
            Assert(Keyboard.FocusedElement != null, "Unexpected element in focus! (expected non-null)");
            Assert(Keyboard.FocusedElement != _removedEl, "Removed element still has focus!");
            //Assert(Keyboard.FocusedElement is Canvas, "Unexpected element in focus! (expected Canvas)");
            //Assert(((FrameworkElement)Keyboard.FocusedElement).ID == "rootCanvas", "Unexpected element in focus! (expected root canvas)");

            // Log final test results
            this.TestPassed = true;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private UIElement _removedEl = null;
        private Frame _frame = null;
        private FrameworkElement _framePanel = null;
    }
}

