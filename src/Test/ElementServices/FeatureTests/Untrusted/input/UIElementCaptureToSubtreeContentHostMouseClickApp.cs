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
    /// Verify UIElement Capture works for subtree containing content host on mouse input.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <



    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementCaptureToSubtreeContentHostMouseClickApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Capture","HwndSource",@"Compile and Verify UIElement Capture works for subtree containing content host on mouse input in HwndSource.")]
        [TestCase("2",@"CoreInput\Capture","Browser",@"Compile and Verify UIElement Capture works for subtree containing content host on mouse input in Browser.")]
        [TestCase("2",@"CoreInput\Capture","Window",@"Compile and Verify UIElement Capture works for subtree containing content host on mouse input in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "UIElementCaptureToSubtreeContentHostMouseClickApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Capture","HwndSource",@"Verify UIElement Capture works for subtree containing content host on mouse input in HwndSource.")]
        [TestCase("2",@"CoreInput\Capture","Window",@"Verify UIElement Capture works for subtree containing content host on mouse input in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementCaptureToSubtreeContentHostMouseClickApp(),"Run");
            
        }
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {


            CoreLogger.LogStatus("Constructing window....");

            // Construct related Win32 window


            // Construct test element
            Canvas cvs = new Canvas();

            // Construct test element and child element (content host)
            InstrContentPanelHost host = new InstrContentPanelHost();
            ContentElement contentEl = new InstrContentPanel("rootLeaf", "Sample", host);
            host.AddChild(contentEl);

            // first element (source) - we set focus here
            Canvas.SetTop(host, 0);
            Canvas.SetLeft(host, 0);
            host.Height = 95;
            host.Width = 95;
            host.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);
            host.GotMouseCapture += new MouseEventHandler(OnCapture);
            host.LostMouseCapture += new MouseEventHandler(OnCapture);

            // Put the test element on the screen
            cvs.Children.Add(host);
            _rootElement = cvs;

            DisplayMe(_rootElement,10, 10, 100, 100);
            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Execute stuff right before the test operations.
        /// </summary>
        private void DoBeforeExecute()
        {
            CoreLogger.LogStatus("Setting Capture to the test element....");
            _bCaptureAPI = Mouse.Capture(_rootElement, CaptureMode.SubTree);
            CoreLogger.LogStatus("Capture set!");
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            InputCallback[] ops = new InputCallback[] {
                delegate
                {
                    DoBeforeExecute();
                },
                delegate
                {
                    MouseHelper.Click(_rootElement);
                }                
                
            };

            return ops;
        }

        /// <summary>
        /// Stores result of Capture API call.
        /// </summary>
        private bool _bCaptureAPI;

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need 2 Capture measurements to BOTH be true and to match each other.
            // We also need a Mouse event on child element because CaptureToSubtree is in effect.

            CoreLogger.LogStatus("Capture set via API?          (expect true) " + (_bCaptureAPI));
            bool bCaptureIM = InputManagerHelper.Current.PrimaryMouseDevice.Captured != null;
            CoreLogger.LogStatus("Capture set via InputManager? (expect false) " + (bCaptureIM));
            CoreLogger.LogStatus("Events found: (expect 1) " + _eventLog.Count);

            // expect non-negative event count
            bool actual = _bCaptureAPI && (!bCaptureIM) && (_eventLog.Count == 1);
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
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Set test flag
            CoreLogger.LogStatus(" Adding event: " + e.ToString());
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            Point pt = e.GetPosition(null);

            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);
            CoreLogger.LogStatus("   Btn=" + e.ChangedButton.ToString() + ",State=" + e.ButtonState.ToString() + ",ClickCount=" + e.ClickCount);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Standard Capture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnCapture(object sender, MouseEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Left,Right,Middle,XButton1,XButton2: " +
                                e.LeftButton.ToString() + "," +
                                e.RightButton.ToString() + "," +
                                e.MiddleButton.ToString() + "," +
                                e.XButton1.ToString() + "," +
                                e.XButton2.ToString()
                                );
            CoreLogger.LogStatus("     Mouse.Captured: '" + ((Mouse.Captured != null) ? Mouse.Captured.ToString() : "none") + "'");

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();
    }
}
