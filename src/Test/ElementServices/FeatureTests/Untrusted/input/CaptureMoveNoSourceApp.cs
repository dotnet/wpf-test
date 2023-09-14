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
    /// Verify capture fails from MouseMove event handler if element is not in a source.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <remarks>
    /// Disabled due to 




    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CaptureMoveNoSourceApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Capture","HwndSource",@"Compile and Verify capture fails from MouseMove event handler if element is not in a source in HwndSource.")]
        [TestCase("2",@"CoreInput\Capture","Browser",@"Compile and Verify capture fails from MouseMove event handler if element is not in a source in Browser.")]
        [TestCase("2",@"CoreInput\Capture","Window",@"Compile and Verify capture fails from MouseMove event handler if element is not in a source in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "CaptureMoveNoSourceApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Capture","HwndSource",@"Verify capture fails from MouseMove event handler if element is not in a source in HwndSource.")]
        [TestCase("2",@"CoreInput\Capture","Window",@"Verify capture fails from MouseMove event handler if element is not in a source in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CaptureMoveNoSourceApp(),"Run");
            
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
                _rootElement = new InstrPanel();
                _rootElement.MouseMove += new MouseEventHandler(OnMouseMove);

                // Construct element to capture and put it in test element's visual tree
                _elementToCapture = new InstrPanel();

                // NOTE: If the following line is commented out, the element To Capture is not in a tree or a valid source.
                // For this case, it should be commented out....

                // VisualTreeHelper.GetChildren(_rootElement).Add(_elementToCapture).

                // Put the test element on the screen
                DisplayMe(_rootElement, 10, 10, 100, 100);
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

            // Note: for this test we are concerned about capture properties.
            // No capture should be in effect!

            CoreLogger.LogStatus("Captured? " +_bCaptureAPI);
            if (_bCaptureAPI) {
                CoreLogger.LogStatus("  Element: " +Mouse.Captured.ToString()) ;
            }

            bool actual = (!_bCaptureAPI) && (Mouse.Captured == null);
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
            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            Debug.WriteLine ("   Hello from: " + pt.X+","+pt.Y);

            // Capture!
            CoreLogger.LogStatus("Pre-capture: " +Mouse.Captured) ;
            CoreLogger.LogStatus("Capturing " + _elementToCapture.ToString()+ "...");
            _bCaptureAPI = _elementToCapture.CaptureMouse();
            CoreLogger.LogStatus("Captured? " +_bCaptureAPI);
            if (_bCaptureAPI) {
                CoreLogger.LogStatus("  Element: " +Mouse.Captured.ToString()) ;
            }

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
    }
}

