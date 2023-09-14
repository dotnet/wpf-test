// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows.Controls;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using System.Windows.Markup;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Click mouse buttons and verify that Event handlers MouseLeftButtonDown, 
    /// MouseRightButtonDown, MouseLeftButtonUp, and MouseRightButtonUp on content elements contained by frame 
    /// are bubbled up to elements outside the frame.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseButtonOnFrameWithContentElementApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse\BubbleCe", "HwndSource", @"Compile and Verify MouseButton bubble events from frame on ContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse\BubbleCe", "Browser", @"Compile and Verify MouseButton bubble events from frame on ContentElement in Browser.")]
        [TestCase("2", @"CoreInput\Mouse\BubbleCe", "Window", @"Compile and Verify MouseButton bubble events from frame on ContentElement in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MouseButtonOnFrameWithContentElementApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Mouse\BubbleCe", "HwndSource", @"Verify MouseButton bubble events from frame on ContentElement in HwndSource.")]
        [TestCase("1", @"CoreInput\Mouse\BubbleCe", "Window", @"Verify MouseButton bubble events from frame on ContentElement in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseButtonOnFrameWithContentElementApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            // Construct tree
            Canvas canvas = new Canvas();
            _button = new InstrContentPanelHost();
            InstrContentPanel contentElement = new InstrContentPanel("rootLeaf", "Sample", _button);
            _button.AddChild(contentElement);
            canvas.Children.Add(_button);

            // Construct frame for tree
            Frame f = new Frame();
            f.Background = Brushes.Pink;
            f.Content = canvas;
            Canvas.SetTop(f, 0.0);
            Canvas.SetLeft(f, 0.0);
            f.Width = 200;
            f.Height = 100;

            Canvas frameCanvas = new Canvas();
            frameCanvas.Children.Add(f);

            // Position button
            Canvas.SetTop(_button, 0.0);
            Canvas.SetLeft(_button, 0.0);
            _button.Width = 100;
            _button.Height = 50;

            f.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnMouseButton);
            f.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);
            f.PreviewMouseRightButtonUp += new MouseButtonEventHandler(OnMouseButton);
            f.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnMouseButton);

            f.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseButton);
            f.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);
            f.MouseRightButtonUp += new MouseButtonEventHandler(OnMouseButton);
            f.MouseRightButtonDown += new MouseButtonEventHandler(OnMouseButton);

            frameCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseButton);
            frameCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);
            frameCanvas.MouseRightButtonUp += new MouseButtonEventHandler(OnMouseButton);
            frameCanvas.MouseRightButtonDown += new MouseButtonEventHandler(OnMouseButton);

            contentElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);
            contentElement.MouseRightButtonDown += new MouseButtonEventHandler(OnMouseButton);

            // Put the test element on the screen
            DisplayMe(frameCanvas, 10, 10, 200, 100);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    MouseHelper.Click(MouseButton.Left, _button);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Middle, _button);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton1,_button);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton2, _button);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Right, _button);
                }                     
            };
            return ops;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about whether events fire for each button press.

            CoreLogger.LogStatus("Events found (expect 14): " + _buttonLog.Count);
            Assert(_buttonLog.Count == 14, "Button Count not correct");
            Assert(_stateLog.Count == 14, "State Count not correct");

            this.TestPassed = true;
            CoreLogger.LogStatus("Validation complete!");

            return null;
        }
        /// <summary>
        /// Standard mouse event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] sender=" + sender);
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("Button=" + e.ChangedButton.ToString() + ",State=" + e.ButtonState.ToString() + ",ClickCount=" + e.ClickCount);
            CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: " +
                                e.LeftButton.ToString() + "," +
                                e.RightButton.ToString() + "," +
                                e.MiddleButton.ToString() + "," +
                                e.XButton1.ToString() + "," +
                                e.XButton2.ToString()
                                );

            // Store a record of our buttons pressed.
            _buttonLog.Add(e.ChangedButton);
            _stateLog.Add(e.ButtonState);

            // Route this event.
            e.Handled = false;
        }

        /// <summary>
        /// Store record of our fired buttons.
        /// </summary>
        private List<MouseButton> _buttonLog = new List<MouseButton>();

        /// <summary>
        /// Store record of our fired states.
        /// </summary>
        private List<MouseButtonState> _stateLog = new List<MouseButtonState>();

        private InstrContentPanelHost _button;
    }
}
