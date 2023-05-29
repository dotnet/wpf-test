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
using System.Windows.Media;
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
    /// Verify UIElement Capture works for subtree in window on mouse input.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementCaptureToSubtreeMouseClickApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Capture", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify UIElement Capture works for subtree in window on mouse input in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify UIElement Capture works for subtree in window on mouse input in Browser.")]
        [TestCase("2", @"CoreInput\Capture", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify UIElement Capture works for subtree in window on mouse input in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "UIElementCaptureToSubtreeMouseClickApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Capture", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify UIElement Capture works for subtree in window on mouse input in HwndSource.")]
        [TestCase("1", @"CoreInput\Capture", "Window", TestCaseSecurityLevel.FullTrust, @"Verify UIElement Capture works for subtree in window on mouse input in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementCaptureToSubtreeMouseClickApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test element
            Canvas cvs = new Canvas();

            FrameworkElement[] panels = new FrameworkElement[] { new InstrFrameworkPanel() };
            foreach (FrameworkElement p in panels)
            {
                // It's necessary to enable each framework element to receive focus.
                CoreLogger.LogStatus("Panel focusable? " + p.Focusable + ". Turning it on...");
                p.Focusable = true;
            }

            // first element (source) - we set focus here
            Canvas.SetTop(panels[0], 0);
            Canvas.SetLeft(panels[0], 0);
            panels[0].Height = 95;
            panels[0].Width = 95;
            panels[0].MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);
            panels[0].GotMouseCapture += new MouseEventHandler(OnCapture);

            _fe = panels[0];


            // Put the test element on the screen
            cvs.Children.Add(panels[0]);
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff right before the test operations.
        /// </summary>
        private void DoBeforeExecute()
        {
            CoreLogger.LogStatus("Setting Capture to the test element....");
            _bCaptureAPI = Mouse.Capture(_rootElement, CaptureMode.SubTree);
        }

        FrameworkElement _fe = null;

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            InputCallback[] ops = new InputCallback[] {
                delegate
                {
                    DoBeforeExecute();
                },
                delegate
                {
                    MouseHelper.Click(_fe);
                }                
            };

            return ops;
        }

        /// <summary>
        /// Stores result of Capture API call.
        /// </summary>
        private bool _bCaptureAPI;

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need 2 Capture measurements to BOTH be true and to match each other.
            // We also need a Mouse event on child element because CaptureToSubtree is in effect.

            CoreLogger.LogStatus("Capture set via API?           (expect true) " + (_bCaptureAPI));
            bool bCaptureIM = InputManagerHelper.Current.PrimaryMouseDevice.Captured != null;
            CoreLogger.LogStatus("Capture set via InputManager?  (expect true) " + (bCaptureIM));
            CoreLogger.LogStatus("Events found: (expect 1) " + _eventLog.Count);

            // expect non-negative event count
            bool actual = _bCaptureAPI && bCaptureIM && (_eventLog.Count == 1);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Cleaning up by releasing capture...");
            InputManagerHelper.Current.PrimaryMouseDevice.Capture(null);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard mouse button event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("   Btn=" + e.ChangedButton.ToString() + ",State=" + e.ButtonState.ToString() + ",ClickCount=" + e.ClickCount);

            CoreLogger.LogStatus("Setting Capture to the test element....");
            Canvas cvs = (Canvas)_rootElement;
            _bCaptureAPI = Mouse.Capture(cvs.Children[0], CaptureMode.SubTree);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Standard Capture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnCapture(object sender, MouseEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<MouseEventArgs> _eventLog = new List<MouseEventArgs>();
    }
}
