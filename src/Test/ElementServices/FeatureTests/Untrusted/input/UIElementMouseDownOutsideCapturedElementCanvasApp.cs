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
    /// Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over element.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementMouseDownOutsideCapturedElementCanvasApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Compile and Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over element in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over element in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Compile and Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over element in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "UIElementMouseDownOutsideCapturedElementCanvasApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Mouse","HwndSource",@"Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over element in HwndSource.")]
        [TestCase("1",@"CoreInput\Mouse","Window",@"Verify MouseDownOutsideCapturedElement and MouseDownOutsideCapturedElement events work as expected when not over element in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementMouseDownOutsideCapturedElementCanvasApp(),"Run");
            
        }
        
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {


            // Build window


            // Build canvas for this window
            Canvas[] canvases = new Canvas[] { new Canvas() };
            foreach (Canvas cvs in canvases)
            {
                // Build element for this canvas
                FrameworkElement panel = new InstrFrameworkPanel();
                Canvas.SetTop(panel, 0);
                Canvas.SetLeft(panel, 0);
                panel.Height = 10;
                panel.Width = 10;
                panel.AddHandler(Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnMouseButton));
                panel.AddHandler(Mouse.PreviewMouseUpOutsideCapturedElementEvent, new MouseButtonEventHandler(OnMouseButton));
                panel.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseButton));
                panel.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseButton));

                // Add element to canvas
                cvs.Children.Add(panel);

            }
            
            _iInputElement = (IInputElement)canvases[0].Children[0];
            // Display canvas.
            DisplayMe(canvases[0], 10, 10, 100, 100);


            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

            return null;
        }

        IInputElement _iInputElement = null;

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            IInputElement elementToCapture = _iInputElement;
            if (!Mouse.Capture(elementToCapture))
            {
                this.TestPassed = false;
                CoreLogger.LogStatus("Captured return false!!!! ERROR");
            }

            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    MouseHelper.Click((UIElement)_iInputElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Middle, (UIElement)_iInputElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton1,(UIElement)_iInputElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.XButton2, (UIElement)_iInputElement);
                },
                delegate
                {
                    MouseHelper.Click(MouseButton.Right, (UIElement)_iInputElement);
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
            // 5 downoutsidecapturedelement + 5 down + 5 upoutsidecapturedelement + 5 up = 20 events

            CoreLogger.LogStatus("Events found: (expect 20) " + _buttonLog.Count);
            bool eventFound;
            if (_buttonLog.Count == 20)
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


    }
}
