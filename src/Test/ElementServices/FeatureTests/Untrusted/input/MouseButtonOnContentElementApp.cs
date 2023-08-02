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
using System.Threading;
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
    /// Click mouse buttons and verify that Event handlers oneMouseLeftButtonDown, 
    /// MouseRightButtonDown, MouseLeftButtonUp, and MouseRightButtonUp have been 
    /// called event ButtonDown/UP change Handled to true.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseButtonOnContentElementApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse\BubbleCe","HwndSource", @"Compile and Verify MouseButton bubble events on ContentElement in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse\BubbleCe","Browser", @"Compile and Verify MouseButton bubble events on ContentElement in Browser.")]
        [TestCase("2",@"CoreInput\Mouse\BubbleCe","Window", @"Compile and Verify MouseButton bubble events on ContentElement in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "MouseButtonOnContentElementApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Mouse\BubbleCe","HwndSource",  @"Verify MouseButton bubble events on ContentElement in HwndSource.")]
        [TestCase("1",@"CoreInput\Mouse\BubbleCe","Window",  @"Verify MouseButton bubble events on ContentElement in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseButtonOnContentElementApp(),"Run");
            
        }
        
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            CoreLogger.LogStatus("Constructing tree....");
            
            {


                // Construct tree

                // Construct test element and child element
                InstrContentPanelHost host = new InstrContentPanelHost();
                InstrContentPanel contentElement = new InstrContentPanel("rootLeaf", "Sample", host);

                host.AddChild(contentElement);


                //Add event handler to MouseDown and MouseUp to change handled to be true.
                host.AddHandler (Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
                host.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));
                contentElement.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
                contentElement.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));

                //Add handlers to clr events
                host.MouseLeftButtonUp += new MouseButtonEventHandler(OnParentMouseLeftButtonUp);
                host.MouseLeftButtonDown += new MouseButtonEventHandler(OnParentMouseLeftButtonDown);
                host.MouseRightButtonUp += new MouseButtonEventHandler(OnParentMouseRightButtonUp);
                host.MouseRightButtonDown += new MouseButtonEventHandler(OnParentMouseRightButtonDown);

                contentElement.MouseLeftButtonUp += new MouseButtonEventHandler(OnChildMouseLeftButtonUp);
                contentElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnChildMouseLeftButtonDown);
                contentElement.MouseRightButtonUp += new MouseButtonEventHandler(OnChildMouseRightButtonUp);
                contentElement.MouseRightButtonDown += new MouseButtonEventHandler(OnChildMouseRightButtonDown);

                // Put the test element on the screen
                 DisplayMe(host, 10, 10, 100, 100);

            }
            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

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
                    MouseHelper.Click(_rootElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Middle, _rootElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton1,_rootElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton2, _rootElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Right, _rootElement);
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
            
            CoreLogger.LogStatus("Events found: "+_buttonLog.Count);

            //set Passed to be true
            this.TestPassed = true;
            CoreLogger.LogStatus("Before testing called, case passed: " + this.TestPassed.ToString());
            if (_parentMouseLeftButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on MouseLeftButtonUp event on the Parent has been called.");
                this.TestPassed = false;
            }
            if (_parentMouseLeftButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on MouseLeftButtonDown event on the Parent has been called.");
                this.TestPassed = false;
            }
            if (_parentMouseRightButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on MouseRightButtonDown event on the Parent has been called.");
                this.TestPassed = false;
            }
            if (_parentMouseRightButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on MouseRightButtonUp event on the Parent has been called.");
                this.TestPassed = false;
            }
            if (!_childMouseLeftButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on MouseLeftButtonUp event on the Child has not been called.");
                this.TestPassed = false;
            }
            if (!_childMouseLeftButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on MouseLeftButtonDown event on the Child has not been called.");
                this.TestPassed = false;
            }
            if (!_childMouseRightButtonDownCalled)
            {
                CoreLogger.LogStatus("Handler on MouseRightButtonDown event on the Child has not been called.");
                this.TestPassed = false;
            }
            if (!_childMouseRightButtonUpCalled)
            {
                CoreLogger.LogStatus("Handler on MouseRightButtonUp event on the Child has not been called.");
                this.TestPassed = false;
            }


            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        private void OnParentMouseLeftButtonUp(object sender, MouseButtonEventArgs  args)
        {
            _parentMouseLeftButtonUpCalled = true;
        }
        bool _parentMouseLeftButtonUpCalled = false;

        private void OnParentMouseLeftButtonDown(object sender, MouseButtonEventArgs  args)
        {
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
            _childMouseLeftButtonUpCalled = true;
        }
        bool _childMouseLeftButtonUpCalled = false;

        private void OnChildMouseLeftButtonDown(object sender, MouseButtonEventArgs  args)
        {
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
            // Set test flag
            CoreLogger.LogStatus(" Handling event: "+args.ToString());

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            Debug.WriteLine ("   Hello from: " + pt.X+","+pt.Y);
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
        private ArrayList _buttonLog = new ArrayList();
        
        /// <summary>
        /// Store record of our fired states.
        /// </summary>
        private ArrayList _stateLog = new ArrayList();
    }
}
