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
using System.Windows.Markup;
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
    /// Verify MouseEventArgs and MouseButtonEventArgs button properties on ContentElement on immediate mouse-move.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <remarks>
    /// This case should not intermittently fail in automated tests.
    /// Investigate more closely if it does.
    /// </remarks>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementMouseButtonImmediateMoveApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Compile and Verify MouseEventArgs and MouseButtonEventArgs button properties on ContentElement on immediate mouse-move in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify MouseEventArgs and MouseButtonEventArgs button properties on ContentElement on immediate mouse-move in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Compile and Verify MouseEventArgs and MouseButtonEventArgs button properties on ContentElement on immediate mouse-move in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "ContentElementMouseButtonImmediateMoveApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Verify MouseEventArgs and MouseButtonEventArgs button properties on ContentElement on immediate mouse-move in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify MouseEventArgs and MouseButtonEventArgs button properties on ContentElement on immediate mouse-move in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementMouseButtonImmediateMoveApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {

            // Construct test element and child element
            InstrContentPanelHost host = new InstrContentPanelHost();
            InstrContentPanel contentElement = new InstrContentPanel("rootLeaf", "Sample", host);

            // Add event handling
            contentElement.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
            host.AddChild(contentElement);

            // Put the test element on the screen
            DisplayMe(host, 10, 10, 100, 100);

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
                    MouseHelper.Move(_rootElement);
                },
                delegate
                {
                    MouseHelper.Click();
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Middle);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton1);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton2);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Right);
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
            
            CoreLogger.LogStatus("Events found: "+_buttonLog.Count + " (expected 5)");
            bool eventFound;
            if (_buttonLog.Count == 5)
            {
                MouseButton btn = (MouseButton)_buttonLog[0];
                MouseButtonState state = (MouseButtonState)_stateLog[0];
                bool actual0 = (btn == MouseButton.Left) && (state == MouseButtonState.Pressed);
                CoreLogger.LogStatus("LeftButton state? "+ actual0);

                btn = (MouseButton)_buttonLog[1];
                state = (MouseButtonState)_stateLog[1];
                bool actual1 = (btn == MouseButton.Middle) && (state == MouseButtonState.Pressed);
                CoreLogger.LogStatus("MiddleButton state? "+ actual1);

                btn = (MouseButton)_buttonLog[2];
                state = (MouseButtonState)_stateLog[2];
                bool actual2 = (btn == MouseButton.XButton1) && (state == MouseButtonState.Pressed);
                CoreLogger.LogStatus("XButton1Button state? "+ actual2);

                btn = (MouseButton)_buttonLog[3];
                state = (MouseButtonState)_stateLog[3];
                bool actual3 = (btn == MouseButton.XButton2) && (state == MouseButtonState.Pressed);
                CoreLogger.LogStatus("XButton2Button state? "+ actual3);

                btn = (MouseButton)_buttonLog[4];
                state = (MouseButtonState)_stateLog[4];
                bool actual4 = (btn == MouseButton.Right) && (state == MouseButtonState.Pressed);
                CoreLogger.LogStatus("RightButton state? "+ actual4);

                eventFound = (actual0) && (actual1) && (actual2) && (actual3) && (actual4);
            }
            else
            {
                eventFound = false;
            }

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

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
            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X+","+pt.Y);
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
