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
using System.Windows.Controls;
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
    /// Verify Mouse Capture remains on captured element after dismissing modeless window.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CaptureAfterNonModalWindowApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All","All","2",@"CoreInput\Capture",TestCaseSecurityLevel.FullTrust,"Verify Mouse Capture remains on captured element after dismissing modeless window.")]
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

            // Construct test element, add event handling
            Canvas cvs = new Canvas();

            // Captureable panel 1
            InstrFrameworkPanel panel = new InstrFrameworkPanel();
            panel.Name = "nOnLostMouseCapturebtn" + DateTime.Now.Ticks;
            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);
            panel.Height = 40;
            panel.Width = 40;

            // Captureable panel 2 - with events attached
            InstrFrameworkPanel panel2 = new InstrFrameworkPanel();
            panel2.Name = "Capturebtn" + DateTime.Now.Ticks;
            Canvas.SetTop(panel2, 50);
            Canvas.SetLeft(panel2, 50);
            panel2.Height = 40;
            panel2.Width = 40;
            panel2.LostMouseCapture += new MouseEventHandler(OnLostMouseCapture);
            panel2.GotMouseCapture += new MouseEventHandler(OnGotMouseCapture);

            cvs.Children.Add(panel);
            cvs.Children.Add(panel2);

            // Put the test element on the screen
            _rootElement = cvs;
            DisplayMe(_rootElement, 10, 10, 100, 100);

            // With test element on the screen, set Capture and then show our window
            Debug.Assert(cvs.Children.Count == 2, "Canvas children not added correctly");

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
            CoreLogger.LogStatus("Setting Capture to element #1...");
            bool bCaptured = cvs.Children[0].CaptureMouse();
            CoreLogger.LogStatus("Success? " + bCaptured);

            CoreLogger.LogStatus("Setting Capture to element #2...");
            bCaptured = cvs.Children[1].CaptureMouse();
            CoreLogger.LogStatus("Success? " + bCaptured);

            CoreLogger.LogStatus("Popping up NonModalWindow...");
            // Create unrelated window (child of the desktop).
            _nonModalWindow = new SimpleNonModalTestWindow(250, 50, 200, 200);
            _nonModalWindow.Show();

            CoreLogger.LogStatus("Destroying old window...");
            _nonModalWindow.Destroy();

            CoreLogger.LogStatus("Switching to our window (simulate Alt+Tab)...");
            NativeMethods.SetForegroundWindow(_hwnd);

            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need 1 GotCapture event to fire on our target element, on setup.
            // We don't want to lose capture on showing window so we verify that Mouse.Captured is valid

            CoreLogger.LogStatus("Events found: (expect 1) " + _eventLog.Count);
            CoreLogger.LogStatus(" Mouse element captured? (expect true) " + (Mouse.Captured != null));

            bool actual = (_eventLog.Count == 1) && (Mouse.Captured != null);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard Capture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            string senderID = (sender is FrameworkElement) ? ((FrameworkElement)sender).Name : "";
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] [" + senderID + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Standard Capture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnGotMouseCapture(object sender, MouseEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            string senderID = (sender is FrameworkElement) ? ((FrameworkElement)sender).Name : "";
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] [" + senderID + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);

            // Don't route this event any more.
            e.Handled = true;
        }


        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();

        /// <summary>
        /// Store reference to our test window.
        /// </summary>
        private SimpleNonModalTestWindow _nonModalWindow = null;
    }
}
