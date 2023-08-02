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
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Mouse.Captured property is set properly after various actions.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseCapturedApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Capture", "HwndSource", @"Compile and Verify Mouse.Captured property is set properly after various actions in HwndSource.")]
        [TestCase("1", @"CoreInput\Capture", "Browser", @"Compile and Verify Mouse.Captured property is set properly after various actions in Browser.")]
        [TestCase("2", @"CoreInput\Capture", "Window", @"Compile and Verify Mouse.Captured property is set properly after various actions in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MouseCapturedApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Capture", "HwndSource", @"Verify Mouse.Captured property is set properly after various actions in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Window", @"Verify Mouse.Captured property is set properly after various actions in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseCapturedApp(), "Run");

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
                panel.Name = "nonCapturebtn" + DateTime.Now.Ticks;
                panel.LostMouseCapture += new MouseEventHandler(OnCapture);
                panel.GotMouseCapture += new MouseEventHandler(OnCapture);
                Canvas.SetTop(panel, 0);
                Canvas.SetLeft(panel, 0);
                panel.Height = 40;
                panel.Width = 40;

                InstrFrameworkPanel panel2 = new InstrFrameworkPanel();
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
            }
            _rootElement = canvases[0];

            // Put the test element on the screen
            DisplayMe(_rootElement, 1, 1, 100, 100);

            MouseHelper.Move((UIElement)canvases[0].Children[1],MouseLocation.TopLeft);


            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            // Capture an element then kill it
            Canvas cvs = (Canvas)(_rootElement);

            Assert(cvs.Children.Count == 2, "Oh no! Incorrect number of elements in tree");

            // Capture on element 1
            cvs.Children[0].CaptureMouse();

            // Capture on element 2
            cvs.Children[1].CaptureMouse();
            Assert(Mouse.Captured == cvs.Children[1], "Capture failed - actual element not captured");

            // zap element 2
            _removedEl = (UIElement)(cvs.Children[1]);
            cvs.Children.Remove(_removedEl);
            DispatcherHelper.DoEventsPastInput();
            
            Assert(Mouse.Captured == null, "Capture failed - Mouse is still captured after captured element is removed from the tree (expected null capture)");

            // zap any capture settings
            Mouse.Capture(null);
            Assert(Mouse.Captured == null, "Capture failed - unexpected element captured");

            // Recapture mouse from deleted element to non-deleted element
            FrameworkElement e = Mouse.Captured as FrameworkElement;

            if (e == null)
            {
                Canvas par = _rootElement as Canvas;

                // With the element removed, we should have only 1 of 2 elements left.
                if (par.Children.Count == 1)
                {
                    // Recapture
                    par.Children[0].CaptureMouse();
                }
            }

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

            Canvas cvs = (Canvas)(_rootElement);
            Assert(cvs.Children.Count == 1, "Oh no!");

            // 3 Capture events, 2 lose Capture events
            CoreLogger.LogStatus("Event log: (expect 5) " + _eventLog.Count);
            Assert(_eventLog.Count == 5, "Wrong number of Capture events");

            // second element still has Capture
            CoreLogger.LogStatus("Mouse Captured: (expect first child) " + Mouse.Captured);
            Assert(Mouse.Captured == cvs.Children[0], "Capture failed");

            // second element still is targeted
            CoreLogger.LogStatus("Mouse target: (expect first child) " + Mouse.PrimaryDevice.Target);
            Assert(Mouse.PrimaryDevice.Target == cvs.Children[0], "Target failed");

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
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);

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

        private UIElement _removedEl = null;

        private List<MouseEventArgs> _eventLog = new List<MouseEventArgs>();
    }
}

