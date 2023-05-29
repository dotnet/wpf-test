// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify MouseWheel events fire on a mouse wheel.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events. Invoked by test extender BasicInputTests.txr
    /// </description>
    /// <author>Microsoft</author>
 
    public class MouseWheelApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MouseWheelApp",
                "Run",
                hostType);
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseWheelApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            // Construct test element, add event handling
            _rootElement = new InstrPanel();
            _rootElement.Focusable = true;
            _rootElement.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(OnPreviewMouseLeftButtonDown);
            _rootElement.MouseWheel += new MouseWheelEventHandler(OnMouseWheel);

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            // MouseWheel needs focus on some element
            _rootElement.Focus();

            InputCallback[] ops = new InputCallback[]
            {
                delegate
                {
                    MouseHelper.Click(_rootElement);
                },
                delegate
                {
                    MouseHelper.MoveWheel(MouseWheelDirection.Forward, 1);
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

            // Note: for this test we are concerned about whether wheel events fire
            // and the wheel delta is non-zero

            // expect events
            CoreLogger.LogStatus("Events found: (expect 1) " + _eventLog.Count);

            // expect non-zero delta
            int delta = 0;
            if (_eventLog.Count > 0)
            {
                delta = _eventLog[0].Delta;
            }
            CoreLogger.LogStatus("Delta found: (expect non-zero) " + delta);

            // expect non-negative event count
            bool actual = (_eventLog.Count == 1) && (delta != 0);
            bool expected = true;

            bool bResult = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + bResult);
            this.TestPassed = bResult;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard mouse wheel event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("   Delta=" + e.Delta);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Focus the InstrPanel on click. This is not done by default.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UIElement uie = (UIElement)sender;
            
            if (!uie.Focusable)
            {
                throw new TestValidationException("Clicked test element is not focusable as expected.");
            }

            uie.Focus();
        }

        private List<MouseWheelEventArgs> _eventLog = new List<MouseWheelEventArgs>();
    }
}
