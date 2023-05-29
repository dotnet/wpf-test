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
    /// Verify MouseDoubleClick event fires on a double-click.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseDoubleClickApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource", @"Compile and Verify MouseDoubleClick event fires on a double-click in HwndSource.")]
        [TestCase("1",@"CoreInput\Mouse","Browser", @"Compile and Verify MouseDoubleClick event fires on a double-click in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window", @"Compile and Verify MouseDoubleClick event fires on a double-click in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "MouseDoubleClickApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Mouse","HwndSource",  @"Verify MouseDoubleClick event fires on a double-click in HwndSource.")]
        [TestCase("0",@"CoreInput\Mouse","Window",  @"Verify MouseDoubleClick event fires on a double-click in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseDoubleClickApp(),"Run");
            
        }
        

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {

            // Construct test element, add event handling
            _rootElement = new InstrControlPanel();
            ((Control)(_rootElement)).PreviewMouseDoubleClick += new MouseButtonEventHandler(OnMouseClick);
            ((Control)(_rootElement)).MouseDoubleClick += new MouseButtonEventHandler(OnMouseClick);

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
                   MouseHelper.Click(_rootElement,2);
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

            // Note: for this test we are concerned about whether events fire for each double-click.
            // 1 preview + 1 click = 2 events
            // We are also need to inspect the proper button state properties corresponding to each event.
            
            CoreLogger.LogStatus("Events found: "+_buttonLog.Count);
            bool eventFound;
            if (_buttonLog.Count == 2)
            {
                MouseButton btn = (MouseButton)_buttonLog[0];
                MouseButtonState state = (MouseButtonState)_stateLog[0];
                bool actual0 = (btn == MouseButton.Left) && (state == MouseButtonState.Pressed);
                CoreLogger.LogStatus("LeftButton state? "+ actual0);

                btn = (MouseButton)_buttonLog[1];
                state = (MouseButtonState)_stateLog[1];
                bool actual1 = (btn == MouseButton.Left) && (state == MouseButtonState.Pressed);
                CoreLogger.LogStatus("MiddleButton state? "+ actual1);

                eventFound = (actual0) && (actual1);
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
        /// Standard mouse double-click event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseClick(object sender, MouseButtonEventArgs args)
        {
            // Set test flag
            CoreLogger.LogStatus(" Handling event: "+args.ToString());

            // Log some debugging data
            //Debug.WriteLine (" ["+args.RoutedEvent.Name+"]").
            Point pt = args.GetPosition(null);
            Debug.WriteLine ("   Hello from: " + pt.X+","+pt.Y);
            CoreLogger.LogStatus("Button="+args.ChangedButton.ToString()+",State="+args.ButtonState.ToString());
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
