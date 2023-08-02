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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Markup;
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
    /// Verify ContentElement Capture works for subtree on mouse input in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <





    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementCaptureToSubtreeMouseClickApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Capture", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify ContentElement Capture works for subtree on mouse input in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify ContentElement Capture works for subtree on mouse input in Browser.")]
        [TestCase("2", @"CoreInput\Capture", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify ContentElement Capture works for subtree on mouse input in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "ContentElementCaptureToSubtreeMouseClickApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Capture", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify ContentElement Capture works for subtree on mouse input in HwndSource.")]
        [TestCase("1", @"CoreInput\Capture", "Window", TestCaseSecurityLevel.FullTrust, @"Verify ContentElement Capture works for subtree on mouse input in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementCaptureToSubtreeMouseClickApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            CoreLogger.LogStatus("Constructing window....");

            // Construct test element and child element
            InstrContentPanelHost host = new InstrContentPanelHost();
            _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);

            // Add event handling
            _contentElement.GotMouseCapture += new MouseEventHandler(OnCapture);
            _contentElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);
            host.AddChild(_contentElement);

            // Put the test element on the screen
            DisplayMe(host, 10, 10, 100, 100);

            return null;
        }



        /// <summary>
        /// Execute stuff right before the test operations.
        /// </summary>
        private void DoBeforeExecute()
        {
            CoreLogger.LogStatus("Setting Capture to the element....");
            _bCaptureAPI = Mouse.Capture (_contentElement, CaptureMode.SubTree);
        }


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
                    MouseHelper.Click(_rootElement);
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
            // We also need a GotCapture event .

            CoreLogger.LogStatus("Capture set via API?           (expect true) " + (_bCaptureAPI));
            bool bCaptureIM = InputManagerHelper.Current.PrimaryMouseDevice.Captured != null;
            CoreLogger.LogStatus("Events found: (expect 1) " + _eventLog.Count);

            // expect non-negative event count
            bool actual = _bCaptureAPI && (_eventLog.Count == 1);
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

            CoreLogger.LogStatus("Setting Capture to the element....");
            _bCaptureAPI = Mouse.Capture(_contentElement, CaptureMode.SubTree);

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

        private InstrContentPanel _contentElement;
    }
}
