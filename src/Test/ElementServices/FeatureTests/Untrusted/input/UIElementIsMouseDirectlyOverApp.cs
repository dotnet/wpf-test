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
    /// Verify UIElement IsMouseDirectlyOver works for element in window after element on top is removed.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementIsMouseDirectlyOverApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Mouse", "HwndSource", @"Compile and Verify UIElement IsMouseDirectlyOver works for element in window after element on top is removed in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Browser", @"Compile and Verify UIElement IsMouseDirectlyOver works for element in window after element on top is removed in Browser.")]
        [TestCase("3", @"CoreInput\Mouse", "Window", @"Compile and Verify UIElement IsMouseDirectlyOver works for element in window after element on top is removed in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "UIElementIsMouseDirectlyOverApp",
                "Run",
                hostType, null, null);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Mouse", "HwndSource", @"Verify UIElement IsMouseDirectlyOver works for element in window after element on top is removed in HwndSource.")]
        [TestCase("2", @"CoreInput\Mouse", "Window", @"Verify UIElement IsMouseDirectlyOver works for element in window after element on top is removed in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementIsMouseDirectlyOverApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {


            CoreLogger.LogStatus("Constructing window....");

            // Construct two elements, one overlapping the other
            InstrFrameworkPanel[] btns = new InstrFrameworkPanel[] { new InstrFrameworkPanel(), new InstrFrameworkPanel() };
            foreach (InstrFrameworkPanel btn in btns)
            {

                btn.Height = 80.00;
                btn.Width = 30.00;
                Canvas.SetTop(btn, 5.00);
            }
            // These will overlap
            Canvas.SetLeft(btns[0], 35.00);
            Canvas.SetLeft(btns[1], 36.00);
            btns[0].Color = Colors.Red;
            btns[1].Color = Colors.Green;
            btns[1].MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);

            // Add everything to the visual tree
            Canvas cvs = new Canvas();
            cvs.Children.Add(btns[0]);
            cvs.Children.Add(btns[1]);

            // Put the test element on the screen
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            Canvas cvs = (Canvas)_rootElement;
            InstrFrameworkPanel grnElement = cvs.Children[1] as InstrFrameworkPanel;

            InputCallback[] ops = new InputCallback[]
            {
                delegate
                {
                    MouseHelper.Click(cvs);
                },
                delegate
                {
                    MouseHelper.Move(cvs);
                },
                delegate
                {
                    MouseHelper.Move(cvs.Children[0]);
                }                
            };
            return ops;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need to verify that the bottom element is now directly under the mouse.
            Canvas cvs = _rootElement as Canvas;
            Debug.Assert(cvs.Children.Count == 1, "Incorrect children count (post-removal)");

            InstrFrameworkPanel bottomElement = cvs.Children[0] as InstrFrameworkPanel;
            CoreLogger.LogStatus("Colors: " + bottomElement.Color);
            bool bIsMouseOverAfterRemove = bottomElement.IsMouseDirectlyOver;
            CoreLogger.LogStatus("Mouse over red? (expect true) " + bIsMouseOverAfterRemove);

            bool actual = (_bIsMouseOverBeforeRemove && bIsMouseOverAfterRemove);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard mouse button event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs args)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("   Btn=" + args.ChangedButton.ToString() + ",State=" + args.ButtonState.ToString() + ",ClickCount=" + args.ClickCount);

            // Get ready to delete this element
            CoreLogger.LogStatus("Inspecting canvas elements...");
            Canvas cvs = _rootElement as Canvas;
            Debug.Assert(cvs.Children.Count == 2, "Incorrect children count (pre-removal)");

            InstrFrameworkPanel redElement = cvs.Children[0] as InstrFrameworkPanel;
            InstrFrameworkPanel grnElement = cvs.Children[1] as InstrFrameworkPanel;
            CoreLogger.LogStatus("Colors: " + redElement.Color + "," + grnElement.Color);

            bool bIsMouseOverPre = redElement.IsMouseDirectlyOver;
            bool bIsMouseOverPre2 = grnElement.IsMouseDirectlyOver;
            CoreLogger.LogStatus("Mouse over red?  (expect false) " + bIsMouseOverPre);
            CoreLogger.LogStatus("Mouse over green?  (expect true)" + bIsMouseOverPre2);
            _bIsMouseOverBeforeRemove = (!bIsMouseOverPre) && bIsMouseOverPre2;

            // Delete it!
            CoreLogger.LogStatus("Removing element on top...");
            cvs.Children.Remove(grnElement);

            // Don't route this event any more.
            args.Handled = true;
        }

        private bool _bIsMouseOverBeforeRemove = false;

    }
}
