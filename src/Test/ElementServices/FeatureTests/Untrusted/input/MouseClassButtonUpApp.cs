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
    /// Verify Mouse class button properties on a button-up on a button-up.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseClassButtonUpApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Compile and Verify Mouse class button properties on a button-up in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify Mouse class button properties on a button-up in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Compile and Verify Mouse class button properties on a button-up in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "MouseClassButtonUpApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Verify Mouse class button properties on a button-up in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify Mouse class button properties on a button-up in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseClassButtonUpApp(),"Run");
            
        }
        
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {

            // Construct test element, add event handling
            _rootElement = new InstrPanel();
            _rootElement.AddHandler (Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);
        
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
            // All buttons should be RELEASED.
            
            CoreLogger.LogStatus("Events found: "+_stateLog.Count);
            bool eventFound;
            if (_stateLog.Count == 5)
            {
                MouseStatus ms = (MouseStatus)_stateLog[0];
                CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: "+
                                    ms.LeftButton.ToString()+","+
                                    ms.RightButton.ToString()+","+
                                    ms.MiddleButton.ToString()+","+
                                    ms.XButton1.ToString()+","+
                                    ms.XButton2.ToString()
                                    );

                bool actual0 = (ms.LeftButton == MouseButtonState.Released);
                CoreLogger.LogStatus("LeftButton state? "+ actual0);

                ms = (MouseStatus)_stateLog[1];
                CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: "+
                                    ms.LeftButton.ToString()+","+
                                    ms.RightButton.ToString()+","+
                                    ms.MiddleButton.ToString()+","+
                                    ms.XButton1.ToString()+","+
                                    ms.XButton2.ToString()
                                    );
                bool actual1 = (ms.MiddleButton == MouseButtonState.Released);
                CoreLogger.LogStatus("MiddleButton state? "+ actual1);

                ms = (MouseStatus)_stateLog[2];
                CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: "+
                                    ms.LeftButton.ToString()+","+
                                    ms.RightButton.ToString()+","+
                                    ms.MiddleButton.ToString()+","+
                                    ms.XButton1.ToString()+","+
                                    ms.XButton2.ToString()
                                    );
                bool actual2 = (ms.XButton1 == MouseButtonState.Released);
                CoreLogger.LogStatus("XButton1Button state? "+ actual2);

                ms = (MouseStatus)_stateLog[3];
                CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: "+
                                    ms.LeftButton.ToString()+","+
                                    ms.RightButton.ToString()+","+
                                    ms.MiddleButton.ToString()+","+
                                    ms.XButton1.ToString()+","+
                                    ms.XButton2.ToString()
                                    );
                bool actual3 = (ms.XButton2 == MouseButtonState.Released);
                CoreLogger.LogStatus("XButton2Button state? "+ actual3);

                ms = (MouseStatus)_stateLog[4];
                CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: "+
                                    ms.LeftButton.ToString()+","+
                                    ms.RightButton.ToString()+","+
                                    ms.MiddleButton.ToString()+","+
                                    ms.XButton1.ToString()+","+
                                    ms.XButton2.ToString()
                                    );
                bool actual4 = (ms.RightButton == MouseButtonState.Released);
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
        /// Data structure holding various mouse states.
        /// </summary>
        struct MouseStatus {
            /// <summary>LeftButton</summary>
            public MouseButtonState LeftButton;
            /// <summary>RightButton</summary>
            public MouseButtonState RightButton;
            /// <summary>MiddleButton</summary>
            public MouseButtonState MiddleButton;
            /// <summary>XButton1</summary>
            public MouseButtonState XButton1;
            /// <summary>XButton2</summary>
            public MouseButtonState XButton2;
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
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Point pt = Mouse.GetPosition(null);
            Debug.WriteLine ("   Hello from: " + pt.X+","+pt.Y);
            CoreLogger.LogStatus("Button="+args.ChangedButton.ToString()+",State="+args.ButtonState.ToString()+",ClickCount="+args.ClickCount);

            // Preserve mouse status
            MouseStatus ms = new MouseStatus();
            ms.LeftButton = Mouse.LeftButton;
            ms.RightButton = Mouse.RightButton;
            ms.MiddleButton = Mouse.MiddleButton;
            ms.XButton1 = Mouse.XButton1;
            ms.XButton2 = Mouse.XButton2;

            CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: "+
                                ms.LeftButton.ToString()+","+
                                ms.RightButton.ToString()+","+
                                ms.MiddleButton.ToString()+","+
                                ms.XButton1.ToString()+","+
                                ms.XButton2.ToString()
                                );

            // Store a record of our buttons pressed.
            _stateLog.Add(ms);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our fired states.
        /// </summary>
        private ArrayList _stateLog = new ArrayList();
    }
}
