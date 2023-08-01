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
    /// Verify FrameworkContentElement IsMouseOver works for element in window
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkContentElementIsMouseOverApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3",@"CoreInput\Mouse","HwndSource",@"Compile and Verify FrameworkContentElement IsMouseOver works for element in window in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify FrameworkContentElement IsMouseOver works for element in window in Browser.")]
        [TestCase("3",@"CoreInput\Mouse","Window",@"Compile and Verify FrameworkContentElement IsMouseOver works for element in window in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "FrameworkContentElementIsMouseOverApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Verify FrameworkContentElement IsMouseOver works for element in window in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify FrameworkContentElement IsMouseOver works for element in window in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkContentElementIsMouseOverApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            CoreLogger.LogStatus("Constructing window....");
            
            // Construct visual element
            InstrContentPanelHost[] btns = new InstrContentPanelHost[] {new InstrContentPanelHost()};
            foreach (InstrContentPanelHost btn in btns) {

                btn.Height = 80;
                btn.Width = 65;
                Canvas.SetTop(btn, 5);
            }
            Canvas.SetLeft(btns[0], 20);

            // Add event handlers
            btns[0].MouseLeftButtonDown += new MouseButtonEventHandler(OnMouseButton);

            // Store content element 
            _contentElement = new InstrFrameworkContentPanel("rootLeaf", "SampleRed", btns[0]);
            (btns[0]).AddChild(_contentElement);

            // Add everything to the visual tree
            Canvas cvs = new Canvas();
            cvs.Children.Add(btns[0]);

            // Put the test element on the screen
            DisplayMe(cvs, 10, 10, 100, 150);

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
            InputCallback[] ops = new InputCallback[]
            {
                delegate
                {
                    MouseHelper.Click(cvs.Children[0]);
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
            CoreLogger.LogStatus("Mouse over red? (expect yes) "+bIsMouseOverAfterClick);
            
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
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseButton(object sender, MouseButtonEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X+","+pt.Y);
            CoreLogger.LogStatus("   Btn="+e.ChangedButton.ToString()+",State="+e.ButtonState.ToString()+",ClickCount="+e.ClickCount);

            // Verify this element exists
            CoreLogger.LogStatus("Inspecting canvas elements...");
            Canvas cvs = _rootElement as Canvas;
            Debug.Assert(cvs.Children.Count == 1, "Incorrect children count (pre-removal)");
            
            // Check that the element is now directly under the mouse.
            _bIsMouseOverDuringClick = _contentElement.IsMouseOver;
            CoreLogger.LogStatus("Mouse over red? (expect yes)  " + _bIsMouseOverDuringClick);

            // Don't route this event any more.
            e.Handled = true;
        }

        private bool _bIsMouseOverDuringClick = false;

        private FrameworkContentElement _contentElement = null;

    }
}
