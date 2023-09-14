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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;

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
    /// Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over ContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementMouseDownOutsideCapturedElementCanvasApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3",@"CoreInput\Mouse","HwndSource",@"Compile and Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over ContentElement in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over ContentElement in Browser.")]
        [TestCase("3",@"CoreInput\Mouse","Window",@"Compile and Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over ContentElement in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "ContentElementMouseDownOutsideCapturedElementCanvasApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Mouse","HwndSource",@"Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over ContentElement in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over ContentElement in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementMouseDownOutsideCapturedElementCanvasApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            // Build canvas for this window
            Canvas cvs = new Canvas();

            // Construct test element and child element
            InstrContentPanelHost panel = new InstrContentPanelHost();
            _frameworkContentElement = new InstrFrameworkContentPanel("rootLeaf", "Sample", panel);
            _frameworkContentElement.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnMouseButton));
            _frameworkContentElement.AddHandler(Mouse.PreviewMouseUpOutsideCapturedElementEvent, new MouseButtonEventHandler(OnMouseButton));
            _frameworkContentElement.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
            _frameworkContentElement.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));
            panel.AddChild(_frameworkContentElement);

            Canvas.SetTop(panel, 0);
            Canvas.SetLeft(panel, 0);
            panel.Height = 10;
            panel.Width = 10;

            // Add element to canvas
            cvs.Children.Add(panel);

            // Put the test element on the screen
            DisplayMe(cvs,10, 10, 100, 100);
            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            Mouse.Capture(_frameworkContentElement);

            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    MouseHelper.Click(MouseButton.Middle, (IntPtr)hwnd);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton1, (IntPtr)hwnd);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton2, (IntPtr)hwnd);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Right, (IntPtr)hwnd);
                } 
                
            };
            return ops;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about whether events fire for each button press.
            // 4 downoutsidecapturedelement + 4 down + 4 upoutsidecapturedelement + 4 up = 16 events

            CoreLogger.LogStatus("Events found: (expect 16) " + _buttonLog.Count);
            bool eventFound;
            if (_buttonLog.Count == 16)
            {
                eventFound = true;
            }
            else
            {
                eventFound = false;
            }

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard mouse event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs args)
        {
            // Set test flag
            CoreLogger.LogStatus(" Handling event: " + args.ToString());

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("Button=" + args.ChangedButton.ToString() + ",State=" + args.ButtonState.ToString() + ",ClickCount=" + args.ClickCount);
            CoreLogger.LogStatus("Left,Right,Middle,XButton1,XButton2: " +
                                args.LeftButton.ToString() + "," +
                                args.RightButton.ToString() + "," +
                                args.MiddleButton.ToString() + "," +
                                args.XButton1.ToString() + "," +
                                args.XButton2.ToString()
                                );

            // Store a record of our buttons pressed.
            _buttonLog.Add(args.ChangedButton);
            _stateLog.Add(args.ButtonState);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our fired buttons.
        /// </summary>
        private ArrayList _buttonLog = new ArrayList();

        /// <summary>
        /// Store record of our fired states.
        /// </summary>
        private ArrayList _stateLog = new ArrayList();

        private InstrFrameworkContentPanel _frameworkContentElement;

    }
}
