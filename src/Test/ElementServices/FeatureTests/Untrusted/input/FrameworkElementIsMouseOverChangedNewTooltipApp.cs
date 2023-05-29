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
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using SysInfo = System.Windows.Forms.SystemInformation;
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
    /// Verify MouseEnter/Leave events are raised for FrameworkElement upon tooltip appearing on top.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 

    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkElementIsMouseOverChangedNewTooltipApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify MouseEnter/Leave events are raised for FrameworkElement upon tooltip appearing on top in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify MouseEnter/Leave events are raised for FrameworkElement upon tooltip appearing on top in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify MouseEnter/Leave events are raised for FrameworkElement upon tooltip appearing on top in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FrameworkElementIsMouseOverChangedNewTooltipApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify MouseEnter/Leave events are raised for FrameworkElement upon tooltip appearing on top in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Verify MouseEnter/Leave events are raised for FrameworkElement upon tooltip appearing on top in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkElementIsMouseOverChangedNewTooltipApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            CoreLogger.LogStatus("Creating canvas for our window....");
            Canvas cvs = new Canvas();
            cvs.Background = new SolidColorBrush(Colors.Green);
            Control panel = new InstrControlPanel();

            CoreLogger.LogStatus("Creating element for our canvas....");
            panel.Name = "nonfocusbtn" + DateTime.Now.Ticks;
            panel.MouseEnter += new MouseEventHandler(OnMouseLeaveOrEnter);
            panel.MouseLeave += new MouseEventHandler(OnMouseLeaveOrEnter);

            Canvas.SetTop(panel, 10);
            Canvas.SetLeft(panel, 10);
            panel.Height = 85;
            panel.Width = 80;
            panel.Background = new SolidColorBrush(Colors.Brown);

            CoreLogger.LogStatus("Setting up tooltip...");
            ToolTip tt = new ToolTip();
            tt.Opened += new RoutedEventHandler(OnOpenedOrClosed);
            tt.Closed += new RoutedEventHandler(OnOpenedOrClosed);
            tt.Content = "FrameworkElementIsMouseOverChangedNewTooltipApp" + " - ToolTip\n" +
                "FrameworkElementIsMouseOverChangedNewTooltipApp" + " - ToolTip\n" +
                "FrameworkElementIsMouseOverChangedNewTooltipApp" + " - ToolTip\n" +
                "FrameworkElementIsMouseOverChangedNewTooltipApp" + " - ToolTip";
            tt.Placement = PlacementMode.Relative;
            panel.ToolTip = tt;

            CoreLogger.LogStatus("Adding element to canvas...");
            cvs.Children.Add(panel);

            // Put the test element on the screen
            DisplayMe(cvs, 10, 10, 100, 100);

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
            Control panel = (Control)(cvs.Children[0]);

            CoreLogger.LogStatus("Moving mouse to target... (wait 10 s for tooltip to come and go)");
            MouseHelper.Move(panel);
            DispatcherHelper.DoEvents(10000);

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
            Canvas cvs = (Canvas)(_rootElement);
            FrameworkElement panel = (FrameworkElement)(cvs.Children[0]);

            CoreLogger.LogStatus("Validating...");

            IInputElement mouseOverEl = Mouse.DirectlyOver;
            CoreLogger.LogStatus("Mouse over anything? (expect something) " + mouseOverEl);
            Assert(mouseOverEl != null, "Mouse not over element! (should be right now)");

            CoreLogger.LogStatus("Mouse over panel? (expect yes) " + panel.IsMouseOver);
            Assert(panel.IsMouseOver, "Mouse not over our panel! (should be right now)");

            CoreLogger.LogStatus("Event log: (expect 3) " + _eventLog.Count);
            Assert(_eventLog.Count == 3, "Wrong number of focus events (expected 3)");

            Assert(_eventLog[0].RoutedEvent == Mouse.MouseEnterEvent, "Oh no - pre-tooltip appear IsMouseOver should have been MouseEnter");
            Assert(_eventLog[1].RoutedEvent == Mouse.MouseLeaveEvent, "Oh no - post-tooltip appear IsMouseOver should have been MouseLeave");
            Assert(_eventLog[2].RoutedEvent == Mouse.MouseEnterEvent, "Oh no - post-tooltip disappear IsMouseOver should have been MouseEnter");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard MouseLeave event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseLeaveOrEnter(object sender, MouseEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            if (sender is IInputElement)
            {
                CoreLogger.LogStatus("   Mouse over sender? " + ((IInputElement)sender).IsMouseOver);
            }
        }

        /// <summary>
        /// Standard opened event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnOpenedOrClosed(object sender, RoutedEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
        }

        private List<MouseEventArgs> _eventLog = new List<MouseEventArgs>();

    }
}
