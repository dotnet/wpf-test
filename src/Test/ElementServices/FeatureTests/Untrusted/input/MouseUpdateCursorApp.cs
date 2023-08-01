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
using Microsoft.Test.Input;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Mouse cursor update on a mouse move and click on FrameworkElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseUpdateCursorApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All", "All", "1", @"CoreInput\Cursor", TestCaseSecurityLevel.FullTrust, "Verify Mouse cursor update on a mouse move and click on FrameworkElement.")]
        public void LaunchTest()
        {
            Run();
        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");

            // Construct test element, add initial cursor
            InstrControlPanel panel = new InstrControlPanel();
            panel.Cursor = Cursors.SizeNS;

            // Add event handlers
            panel.QueryCursor += new QueryCursorEventHandler(OnQuery);

            // Put the test element on the screen
            DisplayMe(panel, 10, 10, 100, 100);

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
                    DoAfterExecute();
                }                

            };
            return ops;
        }

        /// <summary>
        /// Execute stuff right after the test operations.
        /// </summary>
        private void DoAfterExecute()
        {
            CoreLogger.LogStatus("Adding input helper...");
            InputManagerHelper.CurrentHelper.PostProcessInput += new ProcessInputEventHandler(OnProcess);
            // NOTE: Listening for PostNotifyInput works equally well for this test.
            // However, the design specifies that mouse updates should result in PostProcessInput events being raised.

            CoreLogger.LogStatus("Changing to a different cursor...");

            // The following will result in calling the mouse device's UpdateCursor public API.
            ((FrameworkElement)(_rootElement)).Cursor = Cursors.SizeNESW;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about whether the cursor was updated correctly.
            // We also check if the InputManager events with QueryCursor actions were fired for each mouse input.

            // we expect matching stock cursors and event counts
            IntPtr actualCursor = NativeMethods.GetCursor();
            IntPtr expectedCursor = NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SIZENESW);
            CoreLogger.LogStatus("Found cursor: " + actualCursor + ", expected: " + expectedCursor);

            // We expect 1 standard QueryCursor event
            int actualEvents = _eventLog.Count;
            int expectedEvents = 1;

            bool actual = (actualEvents >= expectedEvents) && (actualCursor == expectedCursor);
            bool expected = true;

            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard process event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnProcess(object sender, ProcessInputEventArgs e)
        {
            // Use InputManager and StagingItem to find input event
            InputEventArgs inputArgs = e.StagingItem.Input;
            RoutedEvent reid = inputArgs.RoutedEvent;
            CoreLogger.LogStatus("/  InputName      =  " + reid.Name + " [" + inputArgs.Timestamp.ToString() + "]");

            // Input Report stuff
            if (reid == Mouse.QueryCursorEvent)
            {
                // Found it! Set the flag
                CoreLogger.LogStatus("/ Adding: " + reid.Name + "...");
                _eventLog.Add(inputArgs);
            }
        }

        /// <summary>
        /// Standard query cursor event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnQuery(object sender, QueryCursorEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from cursor: '" + e.Cursor.ToString() + "'");

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();
    }
}
