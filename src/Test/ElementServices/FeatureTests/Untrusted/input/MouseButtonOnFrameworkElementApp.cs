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
using System.Threading;
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
    /// Click mouse buttons and verify that Event handlers oneMouseLeftButtonDown, 
    /// MouseRightButtonDown, MouseLeftButtonUp, and MouseRightButtonUp have been 
    /// called event ButtonDown/UP change Handled to true.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseButtonOnFrameworkElementApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse\BubbleFe","HwndSource", @"Compile and Verify MouseButton bubble events on FrameworkElement in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse\BubbleFe","Browser", @"Compile and Verify MouseButton bubble events on FrameworkElement in Browser.")]
        [TestCase("2",@"CoreInput\Mouse\BubbleFe","Window", @"Compile and Verify MouseButton bubble events on FrameworkElement in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "MouseButtonOnFrameworkElementApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Mouse\BubbleFe","HwndSource",  @"Verify MouseButton bubble events on FrameworkElement in HwndSource.")]
        [TestCase("1",@"CoreInput\Mouse\BubbleFe","Window",  @"Verify MouseButton bubble events on FrameworkElement in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseButtonOnFrameworkElementApp(),"Run");
            
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
            _button = new InstrFrameworkPanel();
            canvas.Children.Add(_button);

            // Position button
            Canvas.SetTop(_button, 0.0);
            Canvas.SetLeft(_button, 0.0);
            _button.Width = 100;
            _button.Height = 100;

            //Add event handler to MouseDown and MouseUp to change handled to be true.
            canvas.AddHandler (Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
            canvas.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));
            _button.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
            _button.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));

            //Add handlers to clr events
            canvas.MouseLeftButtonUp += new MouseButtonEventHandler(OnParentMouseLeftButtonUp);
            canvas.MouseLeftButtonDown += new MouseButtonEventHandler(OnParentMouseLeftButtonDown);
            canvas.MouseRightButtonUp += new MouseButtonEventHandler(OnParentMouseRightButtonUp);
            canvas.MouseRightButtonDown += new MouseButtonEventHandler(OnParentMouseRightButtonDown);

            _button.MouseLeftButtonUp += new MouseButtonEventHandler(OnChildMouseLeftButtonUp);
            _button.MouseLeftButtonDown += new MouseButtonEventHandler(OnChildMouseLeftButtonDown);
            _button.MouseRightButtonUp += new MouseButtonEventHandler(OnChildMouseRightButtonUp);
            _button.MouseRightButtonDown += new MouseButtonEventHandler(OnChildMouseRightButtonDown);

            // Put the test element on the screen
            DisplayMe(canvas, 10, 10, 100, 100);

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
            // 5 down = 5 events
            // We are also need to inspect the proper button state properties corresponding to each event.
            
            CoreLogger.LogStatus("Events found: (expect 10) "+_buttonLog.Count);
            Assert (_buttonLog.Count == 10, "Incorrect number of events");

            Assert (!_parentMouseLeftButtonUpCalled, "Handler on MouseLeftButtonUp event on the Parent has been called.");
            Assert (!_parentMouseLeftButtonDownCalled, "Handler on MouseLeftButtonDown event on the Parent has been called.");
            Assert (!_parentMouseRightButtonDownCalled, "Handler on MouseRightButtonDown event on the Parent has been called.");
            Assert (!_parentMouseRightButtonUpCalled, "Handler on MouseRightButtonUp event on the Parent has been called.");
            Assert (_childMouseLeftButtonUpCalled, "Handler on MouseLeftButtonUp event on the Child has not been called.");
            Assert (_childMouseLeftButtonDownCalled, "Handler on MouseLeftButtonDown event on the Child has not been called.");
            Assert (_childMouseRightButtonDownCalled, "Handler on MouseRightButtonDown event on the Child has not been called.");
            Assert (_childMouseRightButtonUpCalled, "Handler on MouseRightButtonUp event on the Child has not been called.");

            this.TestPassed = true;
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        private void OnParentMouseLeftButtonUp(object sender, MouseButtonEventArgs  args)
        {
            CoreLogger.LogStatus("-- OnParentMouseLeftButtonUp");
            _parentMouseLeftButtonUpCalled = true;
        }
        bool _parentMouseLeftButtonUpCalled = false;

        private void OnParentMouseLeftButtonDown(object sender, MouseButtonEventArgs  args)
        {
            CoreLogger.LogStatus("-- OnParentMouseLeftButtonDown");
            _parentMouseLeftButtonDownCalled = true;
        }
        bool _parentMouseLeftButtonDownCalled = false;

        private void OnParentMouseRightButtonUp(object sender, MouseButtonEventArgs  args)
        {
            _parentMouseRightButtonUpCalled = true;
        }
        bool _parentMouseRightButtonUpCalled = false;

        private void OnParentMouseRightButtonDown(object sender, MouseButtonEventArgs  args)
        {
            _parentMouseRightButtonDownCalled = true;
        }
        bool _parentMouseRightButtonDownCalled = false;

        private void OnChildMouseLeftButtonUp(object sender, MouseButtonEventArgs  args)
        {
            CoreLogger.LogStatus("-- OnChildMouseLeftButtonUp");
            _childMouseLeftButtonUpCalled = true;
        }
        bool _childMouseLeftButtonUpCalled = false;

        private void OnChildMouseLeftButtonDown(object sender, MouseButtonEventArgs  args)
        {
            CoreLogger.LogStatus("-- OnChildMouseLeftButtonDown");
            _childMouseLeftButtonDownCalled = true;
        }
        bool _childMouseLeftButtonDownCalled = false;

        private void OnChildMouseRightButtonUp(object sender, MouseButtonEventArgs  args)
        {
            _childMouseRightButtonUpCalled = true;
        }
        bool _childMouseRightButtonUpCalled = false;

        private void OnChildMouseRightButtonDown(object sender, MouseButtonEventArgs  args)
        {
            _childMouseRightButtonDownCalled = true;
        }
        bool _childMouseRightButtonDownCalled = false;


        /// <summary>
        /// Standard mouse event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs args)
        {
            // Log some debugging data
            CoreLogger.LogStatus (" ["+args.RoutedEvent.Name+"] sender="+sender);
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus ("   Hello from: " + pt.X+","+pt.Y);
            CoreLogger.LogStatus("Button="+args.ChangedButton.ToString()+",State="+args.ButtonState.ToString()+",ClickCount="+args.ClickCount);
            CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: "+
                                args.LeftButton.ToString()+","+
                                args.RightButton.ToString()+","+
                                args.MiddleButton.ToString()+","+
                                args.XButton1.ToString()+","+
                                args.XButton2.ToString()
                                );

            // Store a record of our buttons pressed.
            _buttonLog.Add(args.ChangedButton);
            _stateLog.Add(args.ButtonState);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our fired buttons.
        /// </summary>
        private List<MouseButton> _buttonLog = new List<MouseButton>();
        
        /// <summary>
        /// Store record of our fired states.
        /// </summary>
        private List<MouseButtonState> _stateLog = new List<MouseButtonState>();

        private FrameworkElement _button;
    }
}
