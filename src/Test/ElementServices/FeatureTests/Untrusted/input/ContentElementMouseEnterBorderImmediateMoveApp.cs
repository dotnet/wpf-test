// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    /// Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over ContentElement with border.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementMouseEnterBorderImmediateMoveApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Compile and Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over ContentElement with border in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over ContentElement with border in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Compile and Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over ContentElement with border in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "ContentElementMouseEnterBorderImmediateMoveApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over ContentElement with border in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify MouseEnter and MouseLeave events fire on immediate mouse over / mouse out over ContentElement with border in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementMouseEnterBorderImmediateMoveApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {


            // Construct test element and content element, add event handling
            InstrContentPanelHost host = new InstrContentPanelHost();
            InstrContentPanel panel = new InstrContentPanel("rootLeaf", "Sample", host);
            panel.MouseEnter += new MouseEventHandler(OnMouse);
            panel.MouseLeave += new MouseEventHandler(OnMouse);

            // Add it to test element.
            host.AddChild(panel);

            // Construct border around element, add event handling
            Border border = new Border();
            Canvas.SetTop(border, 5);
            Canvas.SetLeft(border, 5);
            border.BorderBrush = new SolidColorBrush(Colors.Blue);
            border.Height = 70;
            border.Width = 70;
            border.BorderThickness = new Thickness(10);
            border.MouseEnter += new MouseEventHandler(OnMouse);
            border.MouseLeave += new MouseEventHandler(OnMouse);

            // Add border to element
            border.Child = host;

            // Construct canvas holder, add border to it
            Canvas cvs = new Canvas();
            cvs.Height = 100;
            cvs.Width = 100;
            cvs.Children.Add(border);
            _rootElement = cvs;

            // Put the canvas on the screen
            DisplayMe(cvs,10, 10, 100, 100);

            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

            return null;
        }



        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd) 
        {
            // Move over element, move out of element.
            
            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    MouseHelper.Move(_rootElement);
                },

                delegate
                {
                    MouseHelper.Move(_rootElement,MouseLocation.Bottom);
                },
                delegate
                {
                    MouseHelper.MoveOnVirtualScreenMonitor();
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

            // Note: for this test we are concerned about whether both events fire once per element.
            // 1 border enter + 1 element enter + 1 element leave + 1 border leave = 4 events
            
            CoreLogger.LogStatus("Events found (expect 4): "+_eventLog.Count);

            RoutedEvent[] expectedIDs = new RoutedEvent[] {
                Mouse.MouseEnterEvent, 
                Mouse.MouseEnterEvent, 
                Mouse.MouseLeaveEvent, 
                Mouse.MouseLeaveEvent
            };
            InputEventArgs[] actualEvents = (InputEventArgs[])_eventLog.ToArray();

            // expect non-negative event count
            bool actual = (_eventLog.Count == 4) &&
                (expectedIDs[0] == actualEvents[0].RoutedEvent) &&
                (expectedIDs[1] == actualEvents[1].RoutedEvent) &&
                (expectedIDs[2] == actualEvents[2].RoutedEvent) &&
                (expectedIDs[3] == actualEvents[3].RoutedEvent);

            bool expected = true;
            bool eventFound = (actual == expected);

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
        private void OnMouse(object sender, MouseEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X+","+pt.Y);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<InputEventArgs> _eventLog = new List<InputEventArgs>();
    }
}
