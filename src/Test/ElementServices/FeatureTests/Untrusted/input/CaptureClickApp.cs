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
    /// Verify capture works from MouseRightButtonDown event handler.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <area>CoreInput\Mouse</area>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CaptureClickApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Capture","HwndSource",@"Compile and Verify capture works from MouseRightButtonDown event handler in HwndSource.")]
        [TestCase("2",@"CoreInput\Capture","Browser",@"Compile and Verify capture works from MouseRightButtonDown event handler in Browser.")]
        [TestCase("2",@"CoreInput\Capture","Window",@"Compile and Verify capture works from MouseRightButtonDown event handler in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "CaptureClickApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Capture","HwndSource",@"Verify capture works from MouseRightButtonDown event handler in HwndSource.")]
        [TestCase("2",@"CoreInput\Capture","Window",@"Verify capture works from MouseRightButtonDown event handler in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CaptureClickApp(),"Run");
            
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

                // Construct test element, add event handling
                InstrPanel _myRootElement = new InstrPanel();
                _myRootElement.MouseRightButtonDown += new MouseButtonEventHandler(OnMouseButton);

                // Construct element to capture and put it in test element's visual tree
                _elementToCapture = new InstrPanel();
                _myRootElement.AppendChild((UIElement)_elementToCapture);

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
                    MouseHelper.Click(MouseButton.Right,_rootElement);
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

            // Note: for this test we are concerned about capture properties

            bool actual = (_bCaptureAPI) && (Mouse.Captured == _elementToCapture);
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
        private void OnMouseButton(object sender, MouseButtonEventArgs args)
        {
            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            Debug.WriteLine ("   Hello from: " + pt.X+","+pt.Y);
            Debug.WriteLine ("   Btn="+args.ChangedButton.ToString()+",State="+args.ButtonState.ToString()+",ClickCount="+args.ClickCount);

            // Capture!
            _bCaptureAPI = _elementToCapture.CaptureMouse();

            // Don't route this event any more.
            ((IInputElement)(sender)).MouseRightButtonDown -= new MouseButtonEventHandler(OnMouseButton);
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
