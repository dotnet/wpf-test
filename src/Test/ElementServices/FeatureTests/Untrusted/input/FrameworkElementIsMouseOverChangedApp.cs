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
using System.Windows.Markup;
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
    /// Verify MouseEnter/Leave event is raised for FrameworkElement in tree.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkElementIsMouseOverChangedApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify MouseEnter/Leave event is raised for FrameworkElement in tree in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify MouseEnter/Leave event is raised for FrameworkElement in tree in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify MouseEnter/Leave event is raised for FrameworkElement in tree in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FrameworkElementIsMouseOverChangedApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify MouseEnter/Leave event is raised for FrameworkElement in tree in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Verify MouseEnter/Leave event is raised for FrameworkElement in tree in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkElementIsMouseOverChangedApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Creating canvas for our window....");
            Canvas[] canvases = new Canvas[] { new Canvas() };
            foreach (Canvas cvs in canvases)
            {
                FrameworkElement panel = new InstrFrameworkPanel();

                panel.Name = "nonfocusbtn" + DateTime.Now.Ticks;
                panel.MouseEnter += new MouseEventHandler(OnMouseEnter);
                panel.MouseLeave += new MouseEventHandler(OnMouseLeave);

                Canvas.SetTop(panel, 10);
                Canvas.SetLeft(panel, 10);
                panel.Height = 40;
                panel.Width = 40;

                cvs.Children.Add(panel);

                // One more panel for testing
                FrameworkElement panel2 = new InstrFrameworkPanel();

                panel2.Name = "nonfocusbtn2" + DateTime.Now.Ticks;
                Canvas.SetTop(panel2, 10);
                Canvas.SetLeft(panel2, 55);
                panel2.Height = 40;
                panel2.Width = 40;

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
            Panel cvs = (Panel)(_rootElement);
            FrameworkElement panel = (FrameworkElement)(cvs.Children[0]);
            FrameworkElement panel2 = (FrameworkElement)(cvs.Children[1]);

            CoreLogger.LogStatus("Moving mouse to target...");
            MouseHelper.Move(panel);

            Assert(panel.IsMouseOver, "Mouse not over element! (first mouse move)");

            CoreLogger.LogStatus("Moving mouse to target...");
            MouseHelper.Move(panel2);

            CoreLogger.LogStatus("Event log: " + _eventLog.Count);
            Assert(_eventLog.Count == 2, "Wrong number of focus events");

            // Remove any event handlers from our element under test
            panel.MouseEnter -= new MouseEventHandler(OnMouseEnter);
            panel.MouseLeave -= new MouseEventHandler(OnMouseLeave);

            Assert(!panel.IsMouseOver, "Mouse over element!");
            Assert(panel2.IsMouseOver, "Mouse not over element! (second element)");

            CoreLogger.LogStatus("Moving mouse to target a second time...");
            MouseHelper.Move(panel);

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

            Panel cvs = (Panel)(_rootElement);
            FrameworkElement panel = (FrameworkElement)(cvs.Children[0]);
            FrameworkElement panel2 = (FrameworkElement)(cvs.Children[1]);

            Assert(panel.IsMouseOver, "Mouse not over element! (second mouse move)");
            Assert(_eventLog.Count == 2, "Wrong number of focus events");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }



        /// <summary>
        /// Standard OnMouseEnter event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseEnter(object sender, MouseEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus("   Hello changing to MouseEnter");
        }

        /// <summary>
        /// Standard OnMouseLeave event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseLeave(object sender, MouseEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus("   Hello changing to MouseLeave");
        }

        private List<MouseEventArgs> _eventLog = new List<MouseEventArgs>();
    }
}
