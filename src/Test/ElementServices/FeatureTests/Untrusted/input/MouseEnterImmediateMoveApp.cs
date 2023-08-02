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
    /// Verify MouseEnter, MouseLeave, and Preview events fire on an immediate mouse over / mouse out.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseEnterImmediateMoveApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Compile and Verify MouseEnter, MouseLeave, and Preview events fire on an immediate mouse over / mouse out in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify MouseEnter, MouseLeave, and Preview events fire on an immediate mouse over / mouse out in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Compile and Verify MouseEnter, MouseLeave, and Preview events fire on an immediate mouse over / mouse out in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "MouseEnterImmediateMoveApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Verify MouseEnter, MouseLeave, and Preview events fire on an immediate mouse over / mouse out in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify MouseEnter, MouseLeave, and Preview events fire on an immediate mouse over / mouse out in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseEnterImmediateMoveApp(),"Run");
            
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
            _rootElement.MouseEnter += new MouseEventHandler(OnMouse);
            _rootElement.MouseLeave += new MouseEventHandler(OnMouse);

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);

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
            // Move over element, move out of element.
            
            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    MouseHelper.Move(_rootElement);
                },

                delegate
                {
                    MouseHelper.Move(_rootElement,MouseLocation.Bottom);
                },
                delegate
                {
                    MouseHelper.MoveOnVirtualScreenMonitor();
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

            // Note: for this test we are concerned about whether both events fire once
            // and whether both preview events fire once.
            // 2 regular = 2 events
            
            CoreLogger.LogStatus("Events found: (expect 2) "+_eventLog.Count);
            
            // expect non-negative event count
            int actual = (_eventLog.Count);
            int expected = 2;
            bool eventFound = (actual == expected);

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
        private void OnMouse(object sender, MouseEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X+","+pt.Y);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();
    }
}
