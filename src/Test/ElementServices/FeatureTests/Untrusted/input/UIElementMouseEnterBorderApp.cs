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
    /// Verify MouseEnter and MouseLeave events fire on a mouse over / mouse out over UIElement with border.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events. Invoked by test extender BasicInputTests.txr.
    /// </description>
    /// <author>Microsoft</author>
 
    public class UIElementMouseEnterBorderApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "UIElementMouseEnterBorderApp",
                "Run",
                hostType, null, null);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementMouseEnterBorderApp(), "Run");

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
            panel.Name = "Panel";
            panel.MouseEnter += new MouseEventHandler(OnMouse);
            panel.MouseLeave += new MouseEventHandler(OnMouse);
            panel.Height = 25;
            panel.Width = 25;

            // Construct border around element, add event handling
            Border border = new Border();
            Canvas.SetTop(border, 5);
            Canvas.SetLeft(border, 5);
            border.BorderBrush = new SolidColorBrush(Colors.Blue);
            border.Height = 70;
            border.Width = 70;
            border.BorderThickness = new Thickness(10);
            border.Name = "Border";
            border.MouseEnter += new MouseEventHandler(OnMouse);
            border.MouseLeave += new MouseEventHandler(OnMouse);

            // Add border to element
            border.Child = panel;

            // Construct canvas holder, add border to it
            Canvas cvs = new Canvas();
            cvs.Height = 100;
            cvs.Width = 100;
            cvs.Children.Add(border);
            _rootElement = cvs;

            // Put the canvas on the screen
            DisplayMe(_rootElement, 10, 10, 120, 120);

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
            Canvas cvs = (Canvas)_rootElement;

            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    MouseHelper.Move(_rootElement, MouseLocation.CenterLeft);
                },      
                delegate
                {
                    MouseHelper.Move(cvs.Children[0]);
                },      
                delegate
                {
                    MouseHelper.Move(_rootElement, MouseLocation.Bottom);
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
            // 5 border enter + 1 element enter + 1 element leave + 5 border leave = 12 events

            CoreLogger.LogStatus("Events found (expect 12): " + _eventLog.Count);
            Assert(_eventLog.Count == 12, "incorrect event count");

            RoutedEvent[] expectedIDs = new RoutedEvent[] {
                Mouse.MouseEnterEvent, 
                Mouse.MouseLeaveEvent, 
                Mouse.MouseEnterEvent, 
                Mouse.MouseLeaveEvent,
                Mouse.MouseEnterEvent, 
                Mouse.MouseLeaveEvent,                  
                Mouse.MouseEnterEvent, 
                Mouse.MouseEnterEvent, 
                Mouse.MouseLeaveEvent, 
                Mouse.MouseLeaveEvent,
                Mouse.MouseEnterEvent, 
                Mouse.MouseLeaveEvent, 
            };
            InputEventArgs[] actualEvents = (InputEventArgs[])_eventLog.ToArray();
            for (int i = 0; i < actualEvents.Length; i++)
            {
                CoreLogger.LogStatus("i: " + i + " EventID:" + actualEvents[i].RoutedEvent.Name + " (expect " + expectedIDs[i].Name + ")");
            }

            string[] expectedNames = new string[] {
                "Border", 
                "Border", 
                "Border", 
                "Border", 
                "Border", 
                "Border", 
                "Panel", 
                "Border", 
                "Panel", 
                "Border",
                "Border", 
                "Border", 
            };
            FrameworkElement[] actualElements = (FrameworkElement[])_senderLog.ToArray();
            bool validationSuccess = true;
            for (int i = 0; i < actualElements.Length; i++)
            {
                CoreLogger.LogStatus(i + " SenderID: (expect:Actual):: (" + expectedNames[i] + " : " + actualElements[i].Name + ")");
                validationSuccess = validationSuccess && ((expectedIDs[i] == actualEvents[i].RoutedEvent) && (expectedNames[i] == actualElements[i].Name));
                // 
            }
            Assert(validationSuccess, "RoutedEvents / Names do not match");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

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

            FrameworkElement fe = sender as FrameworkElement;
            _senderLog.Add((fe != null) ? fe : new FrameworkElement());

            // Log some debugging data
            string senderID = (fe != null) ? fe.Name : "";
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] [" + senderID + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<InputEventArgs> _eventLog = new List<InputEventArgs>();

        /// <summary>
        /// Store record of our senders that sent our fired events.
        /// </summary>
        private List<FrameworkElement> _senderLog = new List<FrameworkElement>();

    }
}
