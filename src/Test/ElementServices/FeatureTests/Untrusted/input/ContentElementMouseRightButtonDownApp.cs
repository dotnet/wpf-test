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
    /// Verify MouseRightButtonDown and MouseRightButtonUp events fire on a mouse click for ContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <area>CoreInput\Mouse</area>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementMouseRightButtonDownApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Compile and Verify MouseRightButtonDown and MouseRightButtonUp events fire on a mouse click for ContentElement in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify MouseRightButtonDown and MouseRightButtonUp events fire on a mouse click for ContentElement in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Compile and Verify MouseRightButtonDown and MouseRightButtonUp events fire on a mouse click for ContentElement in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "ContentElementMouseRightButtonDownApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Mouse","HwndSource",@"Verify MouseRightButtonDown and MouseRightButtonUp events fire on a mouse click for ContentElement in HwndSource.")]
        [TestCase("1",@"CoreInput\Mouse","Window",@"Verify MouseRightButtonDown and MouseRightButtonUp events fire on a mouse click for ContentElement in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementMouseRightButtonDownApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            CoreLogger.LogStatus("Constructing tree....");
            
            {
                // Construct test element
                InstrContentPanelHost host = new InstrContentPanelHost();

                // Construct child element
                _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);

                // Construct test element, add event handling
                _contentElement.PreviewMouseRightButtonDown += new MouseButtonEventHandler(OnPreviewMouseButton);
                _contentElement.MouseRightButtonDown += new MouseButtonEventHandler(OnMouseButton);
                _contentElement.PreviewMouseRightButtonUp += new MouseButtonEventHandler(OnPreviewMouseButton);
                _contentElement.MouseRightButtonUp += new MouseButtonEventHandler(OnMouseButton);
                host.AddChild(_contentElement);

                // Put the test element on the screen
                DisplayMe(host, 10, 10, 100, 100);
            }
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
            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                    MouseHelper.Click(MouseButton.Right, _rootElement);
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

            // Note: for this test we are concerned about whether both events fire once, plus their previews.
            // 1 down + 1 up + 2 previews = 4 events
            
            CoreLogger.LogStatus("Events found: "+_eventLog.Count);
            
            // expect non-negative event count
            int actual = (_eventLog.Count);
            int expected = 4;
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
        private void OnMouseButton(object sender, MouseButtonEventArgs args)
        {
            // Set test flag
            CoreLogger.LogStatus(" Adding event: "+args.ToString());
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X+","+pt.Y);
            CoreLogger.LogStatus("   Btn="+args.ChangedButton.ToString()+",State="+args.ButtonState.ToString()+",ClickCount="+args.ClickCount);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Standard preview mouse event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnPreviewMouseButton(object sender, MouseButtonEventArgs args)
        {
            // Set test flag
            CoreLogger.LogStatus(" Adding event: "+args.ToString());
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X+","+pt.Y);
            CoreLogger.LogStatus("   Btn="+args.ChangedButton.ToString()+",State="+args.ButtonState.ToString()+",ClickCount="+args.ClickCount);

            // Continue routing this event.
            args.Handled = false;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();

        private InstrContentPanel _contentElement;
    }
}
