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
using System.Windows.Controls;
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
    /// Verify MouseButtonEventArgs ClickCount on a mouse triple-click.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseTripleClickApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource", @"Compile and Verify MouseButtonEventArgs ClickCount on a mouse triple-click in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser", @"Compile and Verify MouseButtonEventArgs ClickCount on a mouse triple-click in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window", @"Compile and Verify MouseButtonEventArgs ClickCount on a mouse triple-click in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "MouseTripleClickApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Mouse","HwndSource",  @"Verify MouseButtonEventArgs ClickCount on a mouse triple-click in HwndSource.")]
        [TestCase("1",@"CoreInput\Mouse","Window",  @"Verify MouseButtonEventArgs ClickCount on a mouse triple-click in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseTripleClickApp(),"Run");
            
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
            _rootElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouse);

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
                    MouseHelper.Click(_rootElement,3);
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

            // Note: for this test we are concerned about whether events fire for each click.
            // 3 down = 3 events
            // We are also need to inspect that the third click reports the proper click count
            
            CoreLogger.LogStatus("Events found: "+_buttonLog.Count);
            bool eventFound;
            if (_buttonLog.Count == 3)
            {
                // Grab the data from the third event.
                MouseButton btn = (MouseButton)_buttonLog[2];
                MouseButtonState state = (MouseButtonState)_stateLog[2];
                int count = (int)_countLog[2];

                bool actual0 = (btn == MouseButton.Left) && (state == MouseButtonState.Pressed) && (count == 3);
                CoreLogger.LogStatus("LeftButton state? "+ actual0+", ClickCount="+count);

                eventFound = (actual0);
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
        /// Standard mouse button event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouse(object sender, MouseButtonEventArgs args)
        {
            // Log some debugging data
            CoreLogger.LogStatus("Button="+args.ChangedButton.ToString()+",State="+args.ButtonState.ToString()+",Click="+args.ClickCount);
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
            _countLog.Add(args.ClickCount);

            // Continue routing this event any more.
            args.Handled = false;
        }

        /// <summary>
        /// Store record of our fired buttons.
        /// </summary>
        private ArrayList _buttonLog = new ArrayList();
        
        /// <summary>
        /// Store record of our fired states.
        /// </summary>
        private ArrayList _stateLog = new ArrayList();

        /// <summary>
        /// Store record of our fired click counts.
        /// </summary>
        private ArrayList _countLog = new ArrayList();

    }
}
