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
    /// Verify Mouse DirectlyOver property on a static drawing visual.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseDirectlyOverVisualApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Compile and Verify Mouse DirectlyOver property on a static drawing visual in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Browser",@"Compile and Verify Mouse DirectlyOver property on a static drawing visual in Browser.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Compile and Verify Mouse DirectlyOver property on a static drawing visual in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "MouseDirectlyOverVisualApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Mouse","HwndSource",@"Verify Mouse DirectlyOver property on a static drawing visual in HwndSource.")]
        [TestCase("2",@"CoreInput\Mouse","Window",@"Verify Mouse DirectlyOver property on a static drawing visual in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseDirectlyOverVisualApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {

            CoreLogger.LogStatus("Constructing window....");
            
            {
                // Construct related Win32 window


                // Construct test element, add event handling
                InstrPanel _myRootElement = new InstrPanel();
                _myRootElement.MouseMove += new MouseEventHandler(OnMouseMove);

                CoreLogger.LogStatus("Adding static drawing visual....");
                DrawingVisual myDrawingVisual = new DrawingVisual();
                DrawingContext myDrawingContext = myDrawingVisual.RenderOpen();
                SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Blue);
                Pen myPen = new Pen(Brushes.Black, 10);
                EllipseGeometry aGeometry = new EllipseGeometry(new Point(50, 50), 500, 500);

                myDrawingContext.DrawGeometry(mySolidColorBrush, myPen, aGeometry);
                myDrawingContext.Close();
                _myRootElement.AppendChild(myDrawingVisual);

                // Put the test element on the screen
                DisplayMe(_myRootElement, 10, 10, 100, 100);
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
            CoreLogger.LogStatus("Events found: " + _eventLog.Count);

            IInputElement elementMouseDirectlyOver = Mouse.DirectlyOver;
            CoreLogger.LogStatus("DirectlyOver? " + ((elementMouseDirectlyOver!=null) ? elementMouseDirectlyOver.ToString() : "NULL"));
            
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
        /// <param name="args">Event-specific arguments.</param>
        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            Debug.WriteLine ("   Hello from: " + pt.X+","+pt.Y);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();
    }
}
