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
    /// Verify element does not receive MouseButtonUp if another element is captured within MouseButtonDown.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseCaptureButtonDownApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Capture", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify element does not receive MouseButtonUp if another element is captured within MouseButtonDown in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify element does not receive MouseButtonUp if another element is captured within MouseButtonDown in Browser.")]
        [TestCase("3", @"CoreInput\Capture", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify element does not receive MouseButtonUp if another element is captured within MouseButtonDown in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MouseCaptureButtonDownApp",
                "Run",
                hostType);
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Capture", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify element does not receive MouseButtonUp if another element is captured within MouseButtonDown in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Window", TestCaseSecurityLevel.FullTrust, @"Verify element does not receive MouseButtonUp if another element is captured within MouseButtonDown in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseCaptureButtonDownApp(), "Run");

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
                InstrFrameworkPanel panel = new InstrFrameworkPanel();
                panel.Name = "nonCapturebtn";
                panel.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButtonDown);
                Canvas.SetTop(panel, 0);
                Canvas.SetLeft(panel, 0);
                panel.Height = 40;
                panel.Width = 40;

                InstrFrameworkPanel panel2 = new InstrFrameworkPanel();
                panel2.Name = "Capturebtn";
                panel2.MouseLeftButtonUp += new MouseButtonEventHandler(OnMouseButtonUp);
                Canvas.SetTop(panel2, 50);
                Canvas.SetLeft(panel2, 50);
                panel2.Height = 40;
                panel2.Width = 40;

                cvs.Children.Add(panel);
                cvs.Children.Add(panel2);
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

            Canvas cvs = (Canvas)(_rootElement);
            Assert(cvs.Children.Count == 2, "Oh no! Incorrect number of elements in tree");



            CoreLogger.LogStatus("Focusing....");
            cvs.Focusable = true;
            cvs.Focus();

            CoreLogger.LogStatus("Moving mouse to target...");

            CoreLogger.LogStatus("Clicking target...");
            MouseHelper.Click(cvs.Children[0]);

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

            Canvas cvs = (Canvas)(_rootElement);
            Assert(cvs.Children.Count == 2, "Oh no!");

            // 1 MouseDown event, 1 MouseUp event
            CoreLogger.LogStatus("Event log: (expect 2) " + _eventLog.Count);
            Assert(_eventLog.Count == 2, "Wrong number of Mouse events");

            // events shouldn't have identical sources
            RoutedEventArgs a1 = _eventLog[0];
            RoutedEventArgs a2 = _eventLog[1];
            Assert(a1.Source != null, "Event 1 has null source ... it shouldn't");
            Assert(a2.Source != null, "Event 2 has null source ... it shouldn't");
            Assert(a1.Source != a2.Source, "Events come from same source ... they shouldn't");

            // second element still has Capture
            CoreLogger.LogStatus("Mouse Captured: (expect valid element) " + Mouse.Captured);
            Assert(Mouse.Captured == cvs.Children[1], "Capture failed");

            // second element still is targeted
            CoreLogger.LogStatus("Mouse target: (expect valid target) " + Mouse.PrimaryDevice.Target);
            Assert(Mouse.PrimaryDevice.Target == cvs.Children[1], "Target failed");

            Mouse.Capture(null);

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Mouse button event handler, captures an element
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseButtonDown(object sender, MouseButtonEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            if (sender != null)
            {
                CoreLogger.LogStatus("   sender='" + ((FrameworkElement)sender).Name + "'");
            }

            // Capture!
            Canvas cvs = (Canvas)(_rootElement);
            Assert(cvs.Children.Count == 2, "Oh no!");

            CoreLogger.LogStatus("Capturing second element...");
            bool bCapture = cvs.Children[1].CaptureMouse();
            Assert(bCapture, "Oh no! API Capture didn't succeed");

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Standard mouse button event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseButtonUp(object sender, MouseButtonEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            if (sender != null)
            {
                CoreLogger.LogStatus("   sender='" + ((FrameworkElement)sender).Name + "'");
            }

            // Don't route this event any more.
            args.Handled = true;
        }

        private List<MouseButtonEventArgs> _eventLog = new List<MouseButtonEventArgs>();
    }
}

