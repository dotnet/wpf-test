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
    /// Verify capture works for elements in multiple windows.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MultiWindowCaptureApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Capture", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify capture works for elements in multiple windows in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify capture works for elements in multiple windows in Browser.")]
        [TestCase("3", @"CoreInput\Capture", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify capture works for elements in multiple windows in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MultiWindowCaptureApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Capture", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify capture works for elements in multiple windows in HwndSource.")]
        [TestCase("2", @"CoreInput\Capture", "Window", TestCaseSecurityLevel.FullTrust, @"Verify capture works for elements in multiple windows in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MultiWindowCaptureApp(), "Run");

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
                panel.LostMouseCapture += new MouseEventHandler(OnMouseCapture);
                panel.GotMouseCapture += new MouseEventHandler(OnMouseCapture);
                Canvas.SetTop(panel, 0);
                Canvas.SetLeft(panel, 0);
                panel.Height = 40;
                panel.Width = 40;

                FrameworkElement panel2 = new InstrFrameworkPanel();
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
            FrameworkElement[] btns = new FrameworkElement[] { _controlCollection[0] as FrameworkElement, _controlCollection[1] as FrameworkElement };

            CoreLogger.LogStatus("Capture on button A...");
            btns[0].CaptureMouse();
            CoreLogger.LogStatus("Captured A!");

            CoreLogger.LogStatus("Capture on button B...");
            btns[1].CaptureMouse();
            CoreLogger.LogStatus("Captured B!");

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

            this.TestPassed = false;
            try
            {
                // We want our element to have fired a particular event.
                // Only the mouse leave event triggered by the animation
                // should be in the log when it has completed.
                CoreLogger.LogStatus("Event log: " + _eventLog.Count);
                Assert(_eventLog.Count == 3, "Event count not correct (expected 3)");


                TestContainer.CurrentSurface[1].Close();

                // Log final test results.
                TestPassed = (true);
                CoreLogger.LogStatus("Test passed");
            }
            catch (Microsoft.Test.TestValidationException e)
            {
                // Log final test results.
                TestPassed = false;
                CoreLogger.LogStatus("Assert failed: " + e.Message);
            }

            CoreLogger.LogStatus("Test pass status? " + TestPassed);

            return null;
        }

        /// <summary>
        /// Standard focus event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseCapture(object sender, MouseEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            string senderID = (sender is FrameworkElement) ? ((FrameworkElement)sender).Name : "";

            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "] [" + senderID + "]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);

            // Don't route this event any more.
            args.Handled = true;
        }

        private List<MouseEventArgs> _eventLog = new List<MouseEventArgs>();

        private List<FrameworkElement> _controlCollection = new List<FrameworkElement>();
    }
}

