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
    /// Verify Mouse GetPosition works from MouseMove event handler on a captured element.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CapturePositionApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Capture","HwndSource",@"Compile and Verify Mouse GetPosition works from MouseMove event handler on a captured element in HwndSource.")]
        [TestCase("2",@"CoreInput\Capture","Browser",@"Compile and Verify Mouse GetPosition works from MouseMove event handler on a captured element in Browser.")]
        [TestCase("2",@"CoreInput\Capture","Window",@"Compile and Verify Mouse GetPosition works from MouseMove event handler on a captured element in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "CapturePositionApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Capture","HwndSource",@"Verify Mouse GetPosition works from MouseMove event handler on a captured element in HwndSource.")]
        [TestCase("2",@"CoreInput\Capture","Window",@"Verify Mouse GetPosition works from MouseMove event handler on a captured element in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CapturePositionApp(),"Run");
            
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


                // Construct element to capture
                Button btn = new Button();
                btn.Height = 15.00;
                btn.Width = 5.00;
                Canvas.SetLeft(btn, 65.00);
                Canvas.SetTop(btn, 5.00);

                // Add event handling to captured element
                _elementToCapture = btn;
                _elementToCapture.MouseMove += new MouseEventHandler(OnMouseMove);

                // Add everything to the visual tree
                Canvas cvs = new Canvas();
                cvs.Children.Add(btn);
                _rootElement = cvs;

                // Put the test element on the screen
                DisplayMe(cvs, 10, 10, 100, 100);


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
                    DoBeforeExecute();
                },
                delegate
                {
                    MouseHelper.Move(_rootElement);
                }                
            };
            return ops;
        }

        /// <summary>
        /// Execute stuff right before the test operations.
        /// </summary>
        private void DoBeforeExecute() 
        {
            // Capture!
            CoreLogger.LogStatus("Pre-capture: " +Mouse.Captured) ;
            CoreLogger.LogStatus("Capturing " + _elementToCapture.ToString()+ "...");
            _bCaptureAPI = _elementToCapture.CaptureMouse();
            CoreLogger.LogStatus("Captured? " +_bCaptureAPI);
            if (_bCaptureAPI) {
                CoreLogger.LogStatus("  Element: " +Mouse.Captured.ToString()) ;
            }

            // Now our TestOps will fire....
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg) 
        {
            CoreLogger.LogStatus("Validating...");

            // Note: for this test we are concerned about the input event firing on a captured element
            // and whether the saved point has negative cooordinates. (This means the mouse is to the left and above our element.)

            CoreLogger.LogStatus("Events found: "+_eventLog.Count);
            bool pointFound = false;
            if (_eventLog.Count == 1)
            {
                MouseEventArgs args = _eventLog[0] as MouseEventArgs;
                CoreLogger.LogStatus(" Event args: '"+args.ToString()+"'");

                pointFound = (_savedPoint.X < 0) && (_savedPoint.Y < 0);
                
                CoreLogger.LogStatus(" Point coords: '"+_savedPoint.ToString()+"'");
            }
            CoreLogger.LogStatus(" Point found? "+pointFound);
            
            CoreLogger.LogStatus("Captured? " +_bCaptureAPI);
            if (_bCaptureAPI) {
                CoreLogger.LogStatus("  Element: " +Mouse.Captured.ToString()) ;
            }
            bool actual = (_bCaptureAPI) && (Mouse.Captured == _elementToCapture) && pointFound;
            bool expected = true;
            bool eventFound = (actual == expected);
            
            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            Mouse.Capture(null);
            
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
            _savedPoint = Mouse.GetPosition((IInputElement)sender);
            Debug.WriteLine ("   Hello from: " + _savedPoint.X+","+_savedPoint.Y);

            // Don't route this event any more.
            ((IInputElement)(sender)).MouseMove -= new MouseEventHandler(OnMouseMove);
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our captured element.
        /// </summary>
        private IInputElement _elementToCapture;
        
        /// <summary>
        /// Store record of our capture API call.
        /// </summary>
        private bool _bCaptureAPI = false;

        /// <summary>
        /// Store record of our mouse move saved point.
        /// </summary>
        private Point _savedPoint;

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();
    }
}
