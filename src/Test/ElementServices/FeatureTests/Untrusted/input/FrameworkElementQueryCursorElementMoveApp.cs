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
    /// Verify QueryCursor event is raised when element moves under mouse.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkElementQueryCursorElementMoveApp: TestApp
    {


        /// <summary>
        /// Verify QueryCursor event is raised when element moves under mouse.
        /// </summary>
        [TestCase("2", @"CoreInput\Cursor", TestCaseSecurityLevel.FullTrust, @" QueryCursorEventArgs Ctor")]
        public void SimpleTest()
        {
            // Microsoft agrees on this. 
            QueryCursorEventArgs q = new QueryCursorEventArgs(InputManager.Current.PrimaryMouseDevice,0, null);            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify QueryCursor event is raised when element moves under mouse in HwndSource.")]
        [TestCase("3", @"CoreInput\Cursor", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify QueryCursor event is raised when element moves under mouse in Browser.")]
        [TestCase("3", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify QueryCursor event is raised when element moves under mouse in window.")]
        [TestCaseTimeout(@"120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FrameworkElementQueryCursorElementMoveApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify QueryCursor event is raised when element moves under mouse in HwndSource.")]
        [TestCase("3", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Verify QueryCursor event is raised when element moves under mouse in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkElementQueryCursorElementMoveApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
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
            Canvas cvs = (Canvas)(_rootElement);
            FrameworkElement panel = (FrameworkElement)(cvs.Children[0]);

            CoreLogger.LogStatus("Moving mouse to target...");
            MouseHelper.Move(panel);
            CoreLogger.LogStatus("Move-To complete");

            panel.QueryCursor += new QueryCursorEventHandler(OnQueryCursor);

            CoreLogger.LogStatus("Moving element...");
            double oldLeft = Canvas.GetLeft(panel);
            double newLeft = oldLeft + 15.0;
            Canvas.SetLeft(panel, newLeft);

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

            // We want our element to have some QueryCursor events fired on it.
            // 0 on mouse-activation, 1 on mouse-move = 1 events

            CoreLogger.LogStatus("Events found: " + _eventLog.Count);
            Assert(_eventLog.Count >= 1 && _eventLog.Count <= 1, "Event count not correct, expected 1");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

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

            if (fe != null)
            {
                Point pt = e.GetPosition(fe);
                CoreLogger.LogStatus("   Position (sender-relative): " + pt.X + "," + pt.Y);
            }

            _eventLog.Add(e);
        }

        private List<QueryCursorEventArgs> _eventLog = new List<QueryCursorEventArgs>();
    }
}
