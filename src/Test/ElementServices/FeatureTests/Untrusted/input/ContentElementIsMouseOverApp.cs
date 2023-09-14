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
    /// Verify ContentElement IsMouseOver works for element in window
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementIsMouseOverApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3",@"CoreInput\Mouse","HwndSource",@"Compile and Verify ContentElement IsMouseOver works for element in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify ContentElement IsMouseOver works for element in Browser.")]
        [TestCase("3",@"CoreInput\Mouse","Window",@"Compile and Verify ContentElement IsMouseOver works for element in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "ContentElementIsMouseOverApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Verify ContentElement IsMouseOver works for element in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify ContentElement IsMouseOver works for element in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementIsMouseOverApp(),"Run");
            
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


            // Construct visual element
            InstrContentPanelHost[] btns = new InstrContentPanelHost[] {new InstrContentPanelHost()};
            foreach (InstrContentPanelHost btn in btns) {
                btn.Height = 80.00;
                btn.Width = 65.00;
                Canvas.SetTop(btn, 5.00);
            }
            Canvas.SetLeft(btns[0], 20.00);

            // Store content element 
            _contentElement = new InstrContentPanel("rootLeaf", "SampleRed", btns[0]);
            (btns[0]).AddChild(_contentElement);

            // Add event handlers
            _contentElement.MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);

            // Add everything to the visual tree
            Canvas cvs = new Canvas();
            cvs.Children.Add(btns[0]);

            // Put the test element on the screen
            DisplayMe(cvs, 10, 10, 100, 100);

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
                    MouseHelper.Click(_rootElement);
                },                

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

            // For this test we need to verify that the element is now directly under the mouse.
            Canvas cvs = _rootElement as Canvas;
            Debug.Assert(cvs.Children.Count == 1, "Incorrect children count (post-removal)");
            
            bool bIsMouseOverAfterClick = _contentElement.IsMouseOver;
            CoreLogger.LogStatus("Mouse over red during click? (expect yes) "+_bIsMouseOverDuringClick);
            CoreLogger.LogStatus("Mouse over red after click? (expect yes) "+bIsMouseOverAfterClick);
            
            bool actual = (_bIsMouseOverDuringClick && bIsMouseOverAfterClick);
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
            CoreLogger.LogStatus(" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X+","+pt.Y);
            CoreLogger.LogStatus("   Btn="+args.ChangedButton.ToString()+",State="+args.ButtonState.ToString()+",ClickCount="+args.ClickCount);

            // Verify this element exists
            CoreLogger.LogStatus("Inspecting canvas elements...");
            Canvas cvs = _rootElement as Canvas;
            Debug.Assert(cvs.Children.Count == 1, "Incorrect children count (pre-removal)");
            
            // Check that the element is now directly under the mouse.
            _bIsMouseOverDuringClick = _contentElement.IsMouseOver;
            CoreLogger.LogStatus("Mouse over red?   " + _bIsMouseOverDuringClick);

            // Don't route this event any more.
            args.Handled = true;
        }

        private bool _bIsMouseOverDuringClick = false;

        private ContentElement _contentElement = null;

    }
}
