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
    /// Verify Mouse DirectlyOver property on a drawing visual placed over input element.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseDirectlyOverDrawingVisualApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Compile and Verify Mouse DirectlyOver property on a drawing visual placed over input element in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify Mouse DirectlyOver property on a drawing visual placed over input element in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Compile and Verify Mouse DirectlyOver property on a drawing visual placed over input element in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "MouseDirectlyOverDrawingVisualApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Verify Mouse DirectlyOver property on a drawing visual placed over input element in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify Mouse DirectlyOver property on a drawing visual placed over input element in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseDirectlyOverDrawingVisualApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            // Construct test element, add event handling
            InstrFrameworkPanel panel = new InstrFrameworkPanel();
            panel.MouseMove += new MouseEventHandler(OnMouseMove);

            // Put the test element on the screen
            DisplayMe(panel, 10, 10, 200, 200);
        
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
                    MouseHelper.Move(_rootElement);
                },
                delegate
                {
                    DoAfterExecute();
                }                
            };

            return ops;
        }

        /// <summary>
        /// Execute stuff right after the test operations.
        /// </summary>
        private void DoAfterExecute()
        {
            CoreLogger.LogStatus("Replacing with drawing visual....");

            DrawingVisual myDrawingVisual = new DrawingVisual();
            DrawingContext myDrawingContext = myDrawingVisual.RenderOpen();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Blue);
            Pen myPen = new Pen(Brushes.Black, 10);
            EllipseGeometry aGeometry = new EllipseGeometry(new Point(100, 100), 500, 500);
            myDrawingContext.DrawGeometry(mySolidColorBrush, myPen, aGeometry);
            myDrawingContext.Close();

            InstrFrameworkPanel panel = (InstrFrameworkPanel)(_rootElement);
            panel.Children.Add(myDrawingVisual);
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg) 
        {
            // Note: for this test we are concerned about whether the mouse is directly over the element.
            // The mouse is actually over a visual.
            // Internally the input manager is supposed to forward request to containing input element.
            
            CoreLogger.LogStatus("Validating...");
            CoreLogger.LogStatus("Events found: (expect more than 0) " + _eventLog.Count);

            IInputElement elementMouseDirectlyOver = Mouse.DirectlyOver;
            CoreLogger.LogStatus("DirectlyOver? (expect panel) " + ((elementMouseDirectlyOver!=null) ? elementMouseDirectlyOver.ToString() : "NULL"));
            
            // expect non-negative event count, mouse over our root element.
            bool actual = (_eventLog.Count > 0) && (elementMouseDirectlyOver == _rootElement);
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
        /// <param name="e">Event-specific arguments.</param>
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus (" ["+e.RoutedEvent.Name+"]");
            Point pt = e.GetPosition(null);
            CoreLogger.LogStatus ("   Hello from: " + pt.X+","+pt.Y);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();
    }
}
