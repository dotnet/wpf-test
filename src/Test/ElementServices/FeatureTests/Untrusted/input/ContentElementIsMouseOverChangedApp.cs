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
    /// Verify MouseEnter event is raised for ContentElement in tree.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementIsMouseOverChangedApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", @"Compile and Verify MouseEnter event is raised for ContentElement in tree in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", @"Compile and Verify MouseEnter event is raised for ContentElement in tree in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", @"Compile and Verify MouseEnter event is raised for ContentElement in tree in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "ContentElementIsMouseOverChangedApp",
                "Run",
                hostType, null, null);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", @"Verify MouseEnter event is raised for ContentElement in tree in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", @"Verify MouseEnter event is raised for ContentElement in tree in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementIsMouseOverChangedApp(), "Run");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            CoreLogger.LogStatus("Constructing window....");
            Canvas[] canvases = new Canvas[] { new Canvas() };
            foreach (Canvas cvs in canvases)
            {
                // Construct test element
                InstrContentPanelHost panel = new InstrContentPanelHost();

                // Construct child element
                _contentElement = new InstrContentPanel("rootLeaf", "Sample", panel);
                _contentElement.MouseEnter += new MouseEventHandler(OnMouseEnter);
                _contentElement.MouseLeave += new MouseEventHandler(OnMouseLeave);

                panel.AddChild(_contentElement);

                panel.Name = "nonfocusbtn" + DateTime.Now.Ticks;
                Canvas.SetTop(panel, 10);
                Canvas.SetLeft(panel, 10);
                panel.Height = 40;
                panel.Width = 40;

                cvs.Children.Add(panel);

                // One more panel for testing
                // Construct test element
                InstrContentPanelHost panel2 = new InstrContentPanelHost();

                // Construct child element
                _contentElement2 = new InstrContentPanel("rootLeaf", "More", panel2);
                panel2.AddChild(_contentElement2);

                panel2.Name = "nonfocusbtn2" + DateTime.Now.Ticks;
                Canvas.SetTop(panel2, 10);
                Canvas.SetLeft(panel2, 55);
                panel2.Height = 40;
                panel2.Width = 40;

                cvs.Children.Add(panel2);
            }
            _rootElement = canvases[0];

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);

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

            CoreLogger.LogStatus("Moving to element 1...");
            MouseHelper.Move((UIElement)cvs.Children[0]);

            Assert(_contentElement.IsMouseOver, "Mouse not over element! (first mouse move)");

            CoreLogger.LogStatus("Moving to element 2...");
            MouseHelper.Move((UIElement)cvs.Children[1]);
            CoreLogger.LogStatus("Event log (expect 2): " + _eventLog.Count);
            Assert(_eventLog.Count == 2, "Wrong number of focus events");

            Assert(!_contentElement.IsMouseOver, "Mouse over element!");
            Assert(_contentElement2.IsMouseOver, "Mouse not over element! (second element)");

            CoreLogger.LogStatus("Removing any event handlers from our element under test...");
            _contentElement.MouseEnter -= new MouseEventHandler(OnMouseEnter);
            _contentElement.MouseLeave -= new MouseEventHandler(OnMouseLeave);

            CoreLogger.LogStatus("Moving back to element 1...");
            MouseHelper.Move((UIElement)cvs.Children[0]);

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

            // 2 changed events, since we removed any event handlers
            CoreLogger.LogStatus("Event log (expect 2): " + _eventLog.Count);
            Assert(_eventLog.Count == 2, "Wrong number of focus events");
            Assert(_contentElement.IsMouseOver, "Mouse not over element! (second mouse move)");

            bool eventFound = true;

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard focus event handler.
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
        /// Standard focus event handler.
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


        private ContentElement _contentElement = null;
        private ContentElement _contentElement2 = null;

        private List<MouseEventArgs> _eventLog = new List<MouseEventArgs>();

    }
}
