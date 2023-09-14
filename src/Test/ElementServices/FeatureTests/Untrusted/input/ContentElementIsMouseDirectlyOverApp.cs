// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
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
    /// Verify ContentElement IsMouseDirectlyOver works for element in window after element on top is removed.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <





    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementIsMouseDirectlyOverApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", @"Compile and Verify ContentElement IsMouseDirectlyOver works for element in window after element on top is removed in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", @"Compile and Verify ContentElement IsMouseDirectlyOver works for element in window after element on top is removed in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", @"Compile and Verify ContentElement IsMouseDirectlyOver works for element in window after element on top is removed in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "ContentElementIsMouseDirectlyOverApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", @"Verify ContentElement IsMouseDirectlyOver works for element in window after element on top is removed in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", @"Verify ContentElement IsMouseDirectlyOver works for element in window after element on top is removed in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementIsMouseDirectlyOverApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct two elements, one overlapping the other
            InstrContentPanelHost[] btns = new InstrContentPanelHost[] { new InstrContentPanelHost(), new InstrContentPanelHost() };

            foreach (InstrContentPanelHost btn in btns)
            {
                btn.Height = 80.00;
                btn.Width = 65.00;
                Canvas.SetTop(btn, 5.00);
            }

            // These will overlap
            Canvas.SetLeft(btns[0], 20.00);
            Canvas.SetLeft(btns[1], 21.00);

            // Store content element for underneath button.
            _contentElementRed = new InstrContentPanel("rootLeaf", "SampleRed", btns[0]);
            btns[0].AddChild(_contentElementRed);

            // Add arbitrary content element for top button.
            _contentElementGreen = new InstrContentPanel("rootLeaf", "SampleGreen", btns[1]);
            btns[1].AddChild(_contentElementGreen);

            // Add everything to the visual tree
            Canvas cvs = new Canvas();

            cvs.Children.Add(btns[0]);
            cvs.Children.Add(btns[1]);
            _rootElement = cvs;

            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object sender)
        {
            Canvas cvs = (Canvas)_rootElement;
            UIElement panel = cvs.Children[1];

            CoreLogger.LogStatus("Moving mouse to target...");
            CoreLogger.LogStatus("Clicking...");
            MouseHelper.Click(panel);


            CoreLogger.LogStatus("Inspecting canvas elements...");
            Assert(cvs.Children.Count == 2, "Incorrect children count (pre-validation)");

            bool bIsMouseOverPre = _contentElementRed.IsMouseDirectlyOver;
            bool bIsMouseOverPre2 = _contentElementGreen.IsMouseDirectlyOver;

            CoreLogger.LogStatus("Mouse over red?   " + bIsMouseOverPre);
            CoreLogger.LogStatus("Mouse over green? " + bIsMouseOverPre2);

            // We should be over green, not red.
            bool bIsMouseOverBeforeRemove = (!bIsMouseOverPre) && bIsMouseOverPre2;

            Assert(bIsMouseOverBeforeRemove, "Content-overness not correct! (pre-removal)");

            CoreLogger.LogStatus("Mouse directly over (pre-remove): " + Mouse.DirectlyOver);
            
            CoreLogger.LogStatus("Removing element on top...");
            FrameworkElement grnElement = cvs.Children[1] as FrameworkElement;
            cvs.Children.Remove(grnElement);
            CoreLogger.LogStatus("Element removed....");

            base.DoExecute(sender);
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

            CoreLogger.LogStatus("Inspecting canvas elements (post-removal)...");
            Canvas cvs = (Canvas)_rootElement;
            Assert(cvs.Children.Count == 1, "Incorrect children count (post-removal)");

            // We should be over red, not green.

            bool bIsMouseOverAfterRemove = _contentElementRed.IsMouseDirectlyOver;
            CoreLogger.LogStatus("Mouse over after remove? " + bIsMouseOverAfterRemove);
            CoreLogger.LogStatus("Mouse directly over: " + Mouse.DirectlyOver);

            Assert(bIsMouseOverAfterRemove, "Content-over-ness not correct (post-removal)");

            // Log final test results
            TestPassed = (true);
            CoreLogger.LogStatus("Test passed");

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private ContentElement _contentElementRed = null;

        private ContentElement _contentElementGreen = null;
    }
}
