// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    /// Verify QueryCursor event is raised when mouse moves over a Cursor null element.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkElementQueryCursorNullCursorApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify QueryCursor event is raised when mouse moves over a Cursor null element in HwndSource.")]
        [TestCase("3", @"CoreInput\Cursor", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify QueryCursor event is raised when mouse moves over a Cursor null element in Browser.")]
        [TestCase("3", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify QueryCursor event is raised when mouse moves over a Cursor null element in window.")]
        [TestCaseTimeout(@"120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "CoreTestsUntrusted",
                "FrameworkElementQueryCursorNullCursorApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify QueryCursor event is raised when mouse moves over a Cursor null element in HwndSource.")]
        [TestCase("3", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Verify QueryCursor event is raised when mouse moves over a Cursor null element in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkElementQueryCursorNullCursorApp(), "Run");

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

                panel.Cursor = Cursors.Hand;

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
            FrameworkElement panel = (FrameworkElement)(((Panel)_rootElement).Children[0]);

            CoreLogger.LogStatus("Moving mouse to initial position...");
            MouseHelper.Move(panel, MouseLocation.TopLeft, isImmediate:true);

            CoreLogger.LogStatus("Cursor-setting...");
            Assert(panel.Cursor == Cursors.Hand, "Panel is null (pre-set)");
            panel.Cursor = null;

            CoreLogger.LogStatus("Attaching event handler...");
            panel.QueryCursor += new QueryCursorEventHandler(OnQueryCursor);

            CoreLogger.LogStatus("Moving mouse to target...");
            MouseHelper.Move(panel);

            CoreLogger.LogStatus("Move complete. Getting ready to verify query cursor...");

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

            CoreLogger.LogStatus("Events found: (expect non-zero) " + _eventLog.Count);
            Assert(_eventLog.Count >= 1, "Event count not correct, expected 1-2");

            this.TestPassed = true;

            CoreLogger.LogStatus("Test pass status? " + TestPassed);

            return null;
        }

        /// <summary>
        /// Event handler to run when a cursor is queried.
        /// </summary>
        /// <param name="sender">object sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnQueryCursor(object sender, QueryCursorEventArgs e)
        {
            _eventLog.Add(e);

            // Log some debugging data
            FrameworkElement fe = sender as FrameworkElement;
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] [" + ((fe != null) ? fe.ToString() : "") + "]");
            CoreLogger.LogStatus("   Cursor='" + ((e.Cursor != null) ? e.Cursor.ToString() : "") + "'");
        }

        private List<QueryCursorEventArgs> _eventLog = new List<QueryCursorEventArgs>();
    }
}
