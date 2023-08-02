// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
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
    /// Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over UIElement with border.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <remarks>
    /// Disabled until immediate mouse movement is re-enabled in MouseHelper.
    /// </remarks>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementMouseEnterBorderImmediateMoveApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", @"1", TestCaseSecurityLevel.FullTrust, @"Compile and Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over UIElement with border in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", @"1", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over UIElement with border in Browser.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", @"1", TestCaseSecurityLevel.FullTrust, @"Compile and Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over UIElement with border in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "UIElementMouseEnterBorderImmediateMoveApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", @"1", TestCaseSecurityLevel.FullTrust, @"Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over UIElement with border in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", @"1", TestCaseSecurityLevel.FullTrust, @"Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over UIElement with border in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementMouseEnterBorderImmediateMoveApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test element, add event handling
            InstrFrameworkPanel panel = new InstrFrameworkPanel();
            panel.MouseEnter += new MouseEventHandler(OnMouse);
            panel.MouseLeave += new MouseEventHandler(OnMouse);

            // Construct border around element, add event handling
            Border border = new Border();
            Canvas.SetTop(border, 5);
            Canvas.SetLeft(border, 5);
            border.BorderBrush = new SolidColorBrush(Colors.Blue);
            border.Height = 70;
            border.Width = 70;
            border.BorderThickness = new Thickness(10);
            border.MouseEnter += new MouseEventHandler(OnMouse);
            border.MouseLeave += new MouseEventHandler(OnMouse);

            // Add border to element
            border.Child = panel;

            // Construct canvas holder, add border to it
            Canvas cvs = new Canvas();
            cvs.Height = 100;
            cvs.Width = 100;
            cvs.Children.Add(border);

            // Put the canvas on the screen
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }


        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            Canvas cvs = (Canvas)_rootElement;
            // Move over element, move out of element.

            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    MouseHelper.Move(cvs.Children[0]);
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

            // Note: for this test we are concerned about whether both events fire once per element.
            // 1 border enter + 1 element enter + 1 element leave + 1 border leave = 4 events

            CoreLogger.LogStatus("Events found (expect 4): " + _eventLog.Count);
            this.Assert(_eventLog.Count == 4, "Event count not correct");

            RoutedEvent[] expectedIDs = new RoutedEvent[] {
                Mouse.MouseEnterEvent, 
                Mouse.MouseEnterEvent, 
                Mouse.MouseLeaveEvent, 
                Mouse.MouseLeaveEvent
            };
            InputEventArgs[] actualEvents = (InputEventArgs[])_eventLog.ToArray();

            // expect non-negative event count
            bool actual = (_eventLog.Count == 8) &&
                (expectedIDs[0] == actualEvents[0].RoutedEvent) &&
                (expectedIDs[1] == actualEvents[1].RoutedEvent) &&
                (expectedIDs[2] == actualEvents[2].RoutedEvent) &&
                (expectedIDs[3] == actualEvents[3].RoutedEvent);

            bool expected = true;
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
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouse(object sender, MouseEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y + " sender="+sender);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<InputEventArgs> _eventLog = new List<InputEventArgs>();
    }
}
