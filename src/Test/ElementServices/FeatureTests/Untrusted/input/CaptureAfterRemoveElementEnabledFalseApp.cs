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
    /// Verify Capture is not lost after capturing element, disabling it, then removing it.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CaptureAfterRemoveElementEnabledFalseApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All","All","2",@"CoreInput\Capture",TestCaseSecurityLevel.FullTrust,"Verify Capture is not lost after capturing element, disabling it, then removing it.")]
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

            Canvas cvs = new Canvas();
            FrameworkElement panel = new InstrFrameworkPanel();
            panel.Name = "nonCapturebtn" + DateTime.Now.Ticks;
            panel.LostMouseCapture += new MouseEventHandler(OnCapture);
            panel.GotMouseCapture += new MouseEventHandler(OnCapture);
            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);
            panel.Height = 40;
            panel.Width = 40;

            FrameworkElement panel2 = new InstrFrameworkPanel();
            panel2.Name = "Capturebtn" + DateTime.Now.Ticks;
            panel2.LostMouseCapture += new MouseEventHandler(OnCapture);
            panel2.GotMouseCapture += new MouseEventHandler(OnCapture);
            Canvas.SetTop(panel2, 50);
            Canvas.SetLeft(panel2, 50);
            panel2.Height = 40;
            panel2.Width = 40;
            panel2.LostMouseCapture += new MouseEventHandler(OnCaptureStateChange);

            cvs.Children.Add(panel);
            cvs.Children.Add(panel2);

            _rootElement = cvs;

            // Put the test element on the screen
            DisplayMe(_rootElement, 1, 1, 100, 100);

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

            // Capture on element 1
            cvs.Children[0].CaptureMouse();
            // Capture on element 2
            cvs.Children[1].CaptureMouse();

            Debug.Assert(Mouse.Captured == cvs.Children[1], "Capture failed");

            // Disable enabledness on element 2
            CoreLogger.LogStatus("Disabling second elements...");
            _removedEl = (FrameworkElement)(cvs.Children[1]);
            _removedEl.IsEnabled = false;

            CoreLogger.LogStatus("Removing second elements...");
            cvs.Children.Remove(_removedEl);

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

            Canvas cvs = (Canvas)(_rootElement);
            Debug.Assert(cvs.Children.Count == 1, "Oh no!");

            // 2 Capture events, 1 lose Capture events
            CoreLogger.LogStatus("Event log: " + _eventLog.Count);
            CheckIf(_eventLog.Count == 4, "Wrong number of Capture events (pre-release) (expected 4)");

            // second element still has capture 
            CoreLogger.LogStatus("Mouse Captured: " + (Mouse.Captured == null? "Null" : Mouse.Captured.ToString()));
            CheckIf(Mouse.Captured == null, "Capture is still null (pre-release) (expected null)");

            // second element exists but is not targeted
            CoreLogger.LogStatus("Mouse target: " + Mouse.PrimaryDevice.Target);
            CheckIf(_removedEl != null, "Removed element is null! (pre-release) (expected non-null)");
            CheckIf(Mouse.PrimaryDevice.Target == null, "Target failed (pre-release) (expected null)");

            CoreLogger.LogStatus("Releasing capture....");
            Mouse.Capture(null);


            // 2 Capture events, 2 lose Capture events
            CoreLogger.LogStatus("Event log: " + _eventLog.Count);
            CheckIf(_eventLog.Count == 4, "Wrong number of Capture events (post-release) (expected 4)");

            // second element no longer has capture
            CoreLogger.LogStatus("Mouse Captured: " + Mouse.Captured);
            CheckIf(Mouse.Captured == null, "Capture not null (post-release) (expected null)");

            // second element exists but is no longer targeted
            CoreLogger.LogStatus("Mouse target: " + Mouse.PrimaryDevice.Target);
            CheckIf(Mouse.PrimaryDevice.Target == null, "Target failed (post-release) (expected null)");
            CheckIf(Mouse.PrimaryDevice.Target != _removedEl, "Target comparison to removed element failed (post-release) (expected false)");


            // Log final test results
            this.TestPassed = true;
            CoreLogger.LogStatus("Test passed");

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

        /// <summary>
        /// Standard Capture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnCaptureStateChange(object sender, MouseEventArgs args)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" HEY BABY - [" + args.RoutedEvent.Name + "]");

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

        private FrameworkElement _removedEl = null;
    }
}

