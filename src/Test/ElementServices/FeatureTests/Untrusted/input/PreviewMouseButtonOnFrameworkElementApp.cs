// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
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
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Click mouse buttons and verify that Event handlers onePreviewMouseLeftButtonDown, 
    /// PreviewMouseRightButtonDown, PreviewMouseLeftButtonUp, and PreviewMouseRightButtonUp have been 
    /// called event ButtonDown/UP change Handled to true.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class PreviewMouseButtonOnFrameworkElementApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse\TunnelFe","HwndSource", @"Compile and Verify MouseButton tunnel events on FrameworkElement in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse\TunnelFe","Browser", @"Compile and Verify MouseButton tunnel events on FrameworkElement in Browser.")]
        [TestCase("2",@"CoreInput\Mouse\TunnelFe","Window", @"Compile and Verify MouseButton tunnel events on FrameworkElement in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "PreviewMouseButtonOnFrameworkElementApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Mouse\TunnelFe","HwndSource",  @"Verify MouseButton tunnel events on FrameworkElement in HwndSource.")]
        [TestCase("1",@"CoreInput\Mouse\TunnelFe","Window",  @"Verify MouseButton tunnel events on FrameworkElement in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new PreviewMouseButtonOnFrameworkElementApp(),"Run");
            
        }


        FrameworkElement _button;

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
            _button = new InstrFrameworkPanel();
            canvas.Children.Add(_button);
            _rootElement = canvas;

            // Position button
            Canvas.SetTop(_button, 0.0);
            Canvas.SetLeft(_button, 0.0);
            _button.Width = 100;
            _button.Height = 100;


            //Add event handler to MouseDown and MouseUp to change handled to be true.
            canvas.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
            canvas.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));
            _button.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
            _button.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));

            //Add handlers to clr events
            canvas.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnParentPreviewMouseLeftButtonUp);
            canvas.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnParentPreviewMouseLeftButtonDown);
            canvas.PreviewMouseRightButtonUp += new MouseButtonEventHandler(OnParentPreviewMouseRightButtonUp);
            canvas.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnParentPreviewMouseRightButtonDown);

            _button.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonUp);
            _button.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnChildPreviewMouseLeftButtonDown);
            _button.PreviewMouseRightButtonUp += new MouseButtonEventHandler(OnChildPreviewMouseRightButtonUp);
            _button.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnChildPreviewMouseRightButtonDown);

            // Put the test element on the screen
            DisplayMe(canvas, 10, 10, 100, 100);

            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

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
                    MouseHelper.Click(_button);
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
            // 5 down = 5 events
            // We are also need to inspect the proper button state properties corresponding to each event.

            CoreLogger.LogStatus("Events found: " + _buttonLog.Count);

            //set Passed to be true
            this.TestPassed = true;
            if (!_parentPreviewMouseLeftButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseLeftButtonUp event on the Parent has not been called.");
                this.TestPassed = false;
            }
            if (!_parentPreviewMouseLeftButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseLeftButtonDown event on the Parent has not been called.");
                this.TestPassed = false;
            }
            if (!_parentPreviewMouseRightButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseRightButtonDown event on the Parent has not been called.");
                this.TestPassed = false;
            }
            if (!_parentPreviewMouseRightButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseRightButtonUp event on the Parent has not been called.");
                this.TestPassed = false;
            }

            if (!_childPreviewMouseLeftButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseLeftButtonUp event on the Child has been called.");
                this.TestPassed = false;
            }
            if (!_childPreviewMouseLeftButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseLeftButtonDown event on the Child has been called.");
                this.TestPassed = false;
            }
            if (!_childPreviewMouseRightButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseRightButtonDown event on the Child ha been called.");
                this.TestPassed = false;
            }
            if (!_childPreviewMouseRightButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on PreviewMouseRightButtonUp event on the Child has been called.");
                this.TestPassed = false;
            }


            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private void OnParentPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            LogEventData(sender, args);
            _parentPreviewMouseLeftButtonUpCalled = true;
        }
        bool _parentPreviewMouseLeftButtonUpCalled = false;

        private void OnParentPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            LogEventData(sender, args);
            _parentPreviewMouseLeftButtonDownCalled = true;
        }
        bool _parentPreviewMouseLeftButtonDownCalled = false;

        private void OnParentPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs args)
        {
            LogEventData(sender, args);
            _parentPreviewMouseRightButtonUpCalled = true;
        }
        bool _parentPreviewMouseRightButtonUpCalled = false;

        private void OnParentPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs args)
        {
            LogEventData(sender, args);
            _parentPreviewMouseRightButtonDownCalled = true;
        }
        bool _parentPreviewMouseRightButtonDownCalled = false;


        private void OnChildPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            LogEventData(sender, args);
            _childPreviewMouseLeftButtonUpCalled = true;
        }
        bool _childPreviewMouseLeftButtonUpCalled = false;

        private void OnChildPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            LogEventData(sender, args);
            _childPreviewMouseLeftButtonDownCalled = true;
        }
        bool _childPreviewMouseLeftButtonDownCalled = false;

        private void OnChildPreviewMouseRightButtonUp(object sender, MouseButtonEventArgs args)
        {
            LogEventData(sender, args);
            _childPreviewMouseRightButtonUpCalled = true;
        }
        bool _childPreviewMouseRightButtonUpCalled = false;

        private void OnChildPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs args)
        {
            LogEventData(sender, args);
            _childPreviewMouseRightButtonDownCalled = true;
        }
        bool _childPreviewMouseRightButtonDownCalled = false;


        /// <summary>
        /// Standard mouse event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs args)
        {
            // Log some debugging data
            LogEventData(sender, args);
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("Button=" + args.ChangedButton.ToString() + ",State=" + args.ButtonState.ToString() + ",ClickCount=" + args.ClickCount);
            CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: " +
                                args.LeftButton.ToString() + "," +
                                args.RightButton.ToString() + "," +
                                args.MiddleButton.ToString() + "," +
                                args.XButton1.ToString() + "," +
                                args.XButton2.ToString()
                                );

            // Store a record of our buttons pressed.
            _buttonLog.Add(args.ChangedButton);
            _stateLog.Add(args.ButtonState);

            // Don't route this event any more.
            args.Handled = true;
        }

        private void LogEventData(object sender, RoutedEventArgs e)
        {
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] (sender=" + sender.ToString() + ")");
        }

        /// <summary>
        /// Store record of our fired buttons.
        /// </summary>
        private ArrayList _buttonLog = new ArrayList();

        /// <summary>
        /// Store record of our fired states.
        /// </summary>
        private ArrayList _stateLog = new ArrayList();
    }
}
