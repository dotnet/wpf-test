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
    /// Verify IsMouseOver property drives opacity property trigger for FrameworkElement in tree.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkElementMouseOverVisibilityPropertyTriggerApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify IsMouseOver property drives opacity property trigger for FrameworkElement in tree in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify IsMouseOver property drives opacity property trigger for FrameworkElement in tree in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify IsMouseOver property drives opacity property trigger for FrameworkElement in tree in window.")]
        [TestCaseTimeout(@"120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FrameworkElementMouseOverVisibilityPropertyTriggerApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify IsMouseOver property drives opacity property trigger for FrameworkElement in tree in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", TestCaseSecurityLevel.FullTrust, @"Verify IsMouseOver property drives opacity property trigger for FrameworkElement in tree in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkElementMouseOverVisibilityPropertyTriggerApp(), "Run");

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
                FrameworkElement panel = new InstrFrameworkPanel();

                panel.Name = "nonfocusbtn";
                panel.MouseEnter += new MouseEventHandler(OnMouseEnter);
                panel.MouseLeave += new MouseEventHandler(OnMouseLeave);

                Canvas.SetTop(panel, 10);
                Canvas.SetLeft(panel, 10);
                panel.Height = 40;
                panel.Width = 40;
                cvs.Children.Add(panel);

                // One more panel for testing
                FrameworkElement panel2 = new InstrFrameworkPanel();

                panel2.Name = "nonfocusbtn2";
                Canvas.SetTop(panel2, 10);
                Canvas.SetLeft(panel2, 55);
                panel2.Height = 40;
                panel2.Width = 40;

                double opacity = (double)panel.GetValue(UIElement.OpacityProperty);
                CoreLogger.LogStatus("Element opacity: " + opacity);
                panel.Style = GetFrameworkElementStyle();

                cvs.Children.Add(panel2);
            }
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
            FrameworkElement panel = (FrameworkElement)(cvs.Children[0]);
            FrameworkElement panel2 = (FrameworkElement)(cvs.Children[1]);

            CoreLogger.LogStatus("Moving mouse to target...");
            MouseHelper.Move(panel);

            CoreLogger.LogStatus("Opacity before move back: (expect " + s_opacityValue + " value) " + panel.Opacity.ToString());
            Assert(panel.Opacity == s_opacityValue, "Wrong Opacity value before move back");

            CoreLogger.LogStatus("Moving mouse back to original location...");
            MouseHelper.Move(panel2);

            CoreLogger.LogStatus("Event log: " + _eventLog.Count);
            CoreLogger.LogStatus("Opacity after move back: (expect 1 value) " + panel.Opacity.ToString());
            Assert(panel.Opacity == 1.0D, "Wrong Opacity value after move back");

            CoreLogger.LogStatus("Moving mouse to target...");
            MouseHelper.Move(panel);

            CoreLogger.LogStatus("Opacity before move back: (expect " + s_opacityValue + " value) " + panel.Opacity.ToString());
            Assert(panel.Opacity == s_opacityValue, "Wrong Opacity value before move back");

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
            FrameworkElement panel = (FrameworkElement)(cvs.Children[0]);

            CoreLogger.LogStatus("Event log: (expect 3 enter/leave events) " + _eventLog.Count);
            Assert(_eventLog.Count == 3, "Wrong number of Mouse events");

            CoreLogger.LogStatus("Opacity after all mouse move: (expect " + s_opacityValue + " value) " + panel.Opacity.ToString());
            Assert(panel.Opacity == s_opacityValue, "Wrong opacity value");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Get default style for InstrFrameworkPanel.
        /// </summary>
        /// <returns>A style.</returns>
        /// <remarks>
        /// The goal of this particular style is to hold a property trigger.
        /// Whenever the mouse moves over the element, the element becomes half-opaque.
        /// Needless to say, this is a useless but fun effect.
        /// </remarks>
        public static Style GetFrameworkElementStyle()
        {
            //get default style
            Style style1 = new Style(typeof(InstrFrameworkPanel), (new InstrFrameworkPanel()).Style);

            //property triggers
            Trigger trigger = new Trigger();
            trigger.Property = FrameworkElement.IsMouseOverProperty;
            trigger.Value = true;
            trigger.Setters.Add( new Setter(UIElement.OpacityProperty, s_opacityValue));
            style1.Triggers.Add(trigger);

            //return style
            return style1;
        }


        /// <summary>
        /// Standard OnMouseEnter event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseEnter(object sender, MouseEventArgs args)
        {
            // Set test flag
            // Note: This will get called often thanks to property triggers!
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
            // Note: This will get called often thanks to property triggers!
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus("   Hello changing to MouseLeave");
        }

        private static double s_opacityValue = 0.43D;

        private List<MouseEventArgs> _eventLog = new List<MouseEventArgs>();
    }
}
