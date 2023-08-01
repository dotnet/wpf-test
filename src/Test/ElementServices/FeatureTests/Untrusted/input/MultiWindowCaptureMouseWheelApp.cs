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
    /// Verify capture is not released for element in window after mouse wheel in another window.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <

    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MultiWindowCaptureMouseWheelApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Capture", "HwndSource", @"Compile and Verify capture is not released for element in window after mouse wheel in another window in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Browser", @"Compile and Verify capture is not released for element in window after mouse wheel in another window in Browser.")]
        [TestCase("3", @"CoreInput\Capture", "Window", @"Compile and Verify capture is not released for element in window after mouse wheel in another window in window.")]
        [TestCaseTimeout(@"120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "CoreTestsUntrusted",
                "MultiWindowCaptureMouseWheelApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Capture", "HwndSource", @"Verify capture is not released for element in window after mouse wheel in another window in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Window", @"Verify capture is not released for element in window after mouse wheel in another window in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MultiWindowCaptureMouseWheelApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            Canvas[] canvases = new Canvas[] { new Canvas(), new Canvas() };
            foreach (Canvas cvs in canvases)
            {
                FrameworkElement panel = new InstrFrameworkPanel();
                panel.Focusable = true;
                panel.LostMouseCapture += new MouseEventHandler(OnMouseCapture);
                panel.GotMouseCapture += new MouseEventHandler(OnMouseCapture);
                panel.MouseWheel += new MouseWheelEventHandler(OnWheel);
                Canvas.SetTop(panel, 0);
                Canvas.SetLeft(panel, 0);
                panel.Height = 40;
                panel.Width = 40;

                FrameworkElement panel2 = new InstrFrameworkPanel();
                panel2.Focusable = true;
                panel2.LostMouseCapture += new MouseEventHandler(OnMouseCapture);
                panel2.GotMouseCapture += new MouseEventHandler(OnMouseCapture);
                Canvas.SetTop(panel2, 50);
                Canvas.SetLeft(panel2, 50);
                panel2.Height = 40;
                panel2.Width = 40;

                cvs.Children.Add(panel);
                cvs.Children.Add(panel2);

                _controlCollection.Add(panel);
            }
            ((FrameworkElement)(canvases[0].Children[0])).Name = "btnWindowA";

            ((FrameworkElement)(canvases[1].Children[0])).Name = "btnWindowB";

            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 100, 100);

            DisplayMe(canvases[1], 125, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            List<FrameworkElement> btns = _controlCollection;

            CoreLogger.LogStatus("Capture on button A...");
            btns[0].CaptureMouse();
            CoreLogger.LogStatus("Captured A!");

            CoreLogger.LogStatus("Capture on button B...");
            btns[1].CaptureMouse();
            CoreLogger.LogStatus("Captured B!");

            CoreLogger.LogStatus("Focus on button B again...");
            bool bFocus = btns[1].Focus();
            CoreLogger.LogStatus("Focused B? " + bFocus + " - " + InputHelper.GetFocusedElement());

            CoreLogger.LogStatus("Moving mouse to target location...");
            MouseHelper.Move(btns[0]);

            CoreLogger.LogStatus("Mouse over element A? (should be no) " + btns[0].IsMouseOver);
            CoreLogger.LogStatus("Mouse over element B? (should be yes) " + btns[1].IsMouseOver);
            CoreLogger.LogStatus("Capture in effect? (should be yes) " + (Mouse.Captured != null));

            Assert(!btns[0].IsMouseOver, "Mouse incorrectly over element A! (first mouse move)");
            Assert(btns[1].IsMouseOver, "Mouse not over captured element B! (first mouse move)");
            Assert(btns[1] == Mouse.Captured, "Mouse didn't capture element B! (first mouse move)");

            CoreLogger.LogStatus("Wheeling mouse...");
            MouseHelper.MoveWheel( MouseWheelDirection.Forward, 1);
            CoreLogger.LogStatus("Wheel complete...");
            
            CoreLogger.LogStatus("Getting ready to verify capture...");

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

            CoreLogger.LogStatus("Mouse over element A? (should be no) " + _controlCollection[0].IsMouseOver);
            CoreLogger.LogStatus("Mouse over element B? (should be yes) " + _controlCollection[1].IsMouseOver);
            CoreLogger.LogStatus("Capture in effect? (should be yes) " + (Mouse.Captured != null));

            Assert(!_controlCollection[0].IsMouseOver, "Mouse incorrectly over element A! (after wheel)");
            Assert(_controlCollection[1].IsMouseOver, "Mouse not over captured element B! (after wheel)");
            Assert(_controlCollection[1] == Mouse.Captured, "Mouse didn't capture element B! (after wheel)");

            TestContainer.CurrentSurface[1].Close();
                
            this.TestPassed = true;

            CoreLogger.LogStatus("Test pass status? " + TestPassed);

            return null;
        }

        /// <summary>
        /// Standard capture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseCapture(object sender, MouseEventArgs e)
        {
            // Log some debugging data
            string senderID = (sender is FrameworkElement) ? ((FrameworkElement)sender).Name : "";

            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] [" + senderID + "]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Standard mouse-wheel event handler.
        /// </summary>
        /// <param name="sender">Object sending the event.</param>
        /// <param name="e">Event-specific arguments</param>
        private void OnWheel(object sender, MouseWheelEventArgs e)
        {
            string senderID = (sender is FrameworkElement) ? ((FrameworkElement)sender).Name : "";

            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "] [" + senderID + "]");
            CoreLogger.LogStatus("   Delta=" + e.Delta);

            // Don't route this event any more.
            e.Handled = true;
        }

        private List<FrameworkElement> _controlCollection = new List<FrameworkElement>();
    }
}

