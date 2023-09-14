// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
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
    /// Verify capture is regained after showing WinForms message box.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CaptureAfterWinFormsMessageBoxApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainer("All","All","1",@"CoreInput\Capture",TestCaseSecurityLevel.FullTrust,"Verify capture is regained after showing WinForms message box.")]
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

            CoreLogger.LogStatus("Constructing tree....");
            InstrFrameworkPanel[] canvases = new InstrFrameworkPanel[] { new InstrFrameworkPanel() };

            CoreLogger.LogStatus("Adding event handlers....");
            canvases[0].GotMouseCapture += new MouseEventHandler(OnCapture);
            canvases[0].LostMouseCapture += new MouseEventHandler(OnCapture);

            _rootElement = canvases[0];

            // Put the test element on the screen
            DisplayMe(_rootElement, 1, 1, 100, 100);

            MouseHelper.Move(_rootElement);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Capture beginning on the canvas....");
            _rootElement.CaptureMouse();
            CoreLogger.LogStatus("Canvas captured!");

            AutoCloseWinFormsMessageBox mb = new AutoCloseWinFormsMessageBox(5000, "Wait 5 seconds for box to close", "Wait");
            mb.Show();

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
            this.TestPassed = false;

            CoreLogger.LogStatus("Mouse.Captured: " + Mouse.Captured);
            CheckIf(Mouse.Captured == null, "Oh no - expected nothing to have the capture!");

            CoreLogger.LogStatus("Capture events found: " + _eventLog.Count);
            CheckIf(_eventLog.Count == 2, "Oh no - incorrect number of events (expected 2)");

            // Log final test results
            this.TestPassed = true;

            CoreLogger.LogStatus("Validation complete!");
            Mouse.Capture(null);

            return null;
        }

        /// <summary>
        /// Standard Capture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnCapture(object sender, MouseEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: " +
                                args.LeftButton.ToString() + "," +
                                args.RightButton.ToString() + "," +
                                args.MiddleButton.ToString() + "," +
                                args.XButton1.ToString() + "," +
                                args.XButton2.ToString()
                                );

            // Don't route this event any more.
            args.Handled = true;
        }

        private void CheckIf(bool condition, string exceptionMsg)
        {
            // Log intermediate result
            if (!condition)
            {
                // Intermediate result = FAIL
                string exceptionString = exceptionMsg;
                TestPassed = false;
                throw new Microsoft.Test.TestValidationException(exceptionMsg);
            }
        }

        private List<MouseEventArgs> _eventLog = new List<MouseEventArgs>();
    }
}

