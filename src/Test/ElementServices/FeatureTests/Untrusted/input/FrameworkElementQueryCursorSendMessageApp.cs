// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
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
    /// Verify QueryCursor event is not raised on Win32 SendMessage.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkElementQueryCursorSendMessageApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify QueryCursor event is not raised on Win32 SendMessage in HwndSource.")]
        [TestCase("3", @"CoreInput\Cursor", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify QueryCursor event is not raised on Win32 SendMessage in Browser.")]
        [TestCase("3", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify QueryCursor event is not raised on Win32 SendMessage in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FrameworkElementQueryCursorSendMessageApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify QueryCursor event is not raised on Win32 SendMessage in HwndSource.")]
        [TestCase("3", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Verify QueryCursor event is not raised on Win32 SendMessage in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkElementQueryCursorSendMessageApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            // Build canvas for this window
            Canvas[] canvases = new Canvas[] { new Canvas() };

            foreach (Canvas cvs in canvases)
            {
                // Build element for this canvas
                FrameworkElement panel = new InstrFrameworkPanel();

                Canvas.SetTop(panel, 15);
                Canvas.SetLeft(panel, 15);
                panel.Height = 70;
                panel.Width = 70;

                // Add element to canvas
               cvs.Children.Add(panel);
            }

            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            Canvas cvs = (Canvas)_rootElement;
            InstrFrameworkPanel panel = (InstrFrameworkPanel)cvs.Children[0];

            CoreLogger.LogStatus("Moving to element....");
            MouseHelper.Move(panel);

            panel.QueryCursor += new QueryCursorEventHandler(OnQueryCursor);

            CoreLogger.LogStatus("Updating cursor...");

            // WM_SETCURSOR parameters
            int wParam = _hwnd.Handle.ToInt32();
            int lParam = (NativeConstants.HTCLIENT) + (NativeConstants.WM_MOUSEMOVE * 0x10000);

            NativeMethods.SendMessage(_hwnd, NativeConstants.WM_SETCURSOR, wParam, lParam);
            CoreLogger.LogStatus("Cursor updated. Validating...");

            panel.QueryCursor -= new QueryCursorEventHandler(OnQueryCursor);

            base.DoExecute(arg);
            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            // We want our element to have no QueryCursor events fired on it.
            // 0 on send-message = 0 events

            CoreLogger.LogStatus("Events found: (expect 0) " + _eventLog.Count);
            Assert(_eventLog.Count == 0, "Event count not correct");

            // Log final test results
            bool eventFound = true;

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }
        
        /// <summary>
        /// Event handler to run when a cursor is queried.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnQueryCursor(object sender, QueryCursorEventArgs e)
        {
            // Log some debugging data
            FrameworkElement fe = sender as FrameworkElement;
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] [" + ((fe != null) ? fe.ToString() : "") + "]");
            CoreLogger.LogStatus("   Cursor='" + ((e.Cursor != null) ? e.Cursor.ToString() : "") + "'");

            _eventLog.Add(e);
        }

        private List<QueryCursorEventArgs> _eventLog = new List<QueryCursorEventArgs>();

    }
}
