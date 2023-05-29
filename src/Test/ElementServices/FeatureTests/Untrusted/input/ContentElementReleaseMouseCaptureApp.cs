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
    /// Verify ContentElement ReleaseMouseCapture works for element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementReleaseMouseCaptureApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Capture","HwndSource",@"Compile and Verify ContentElement ReleaseMouseCapture works for element in HwndSource.")]
        [TestCase("2",@"CoreInput\Capture","Browser",@"Compile and Verify ContentElement ReleaseMouseCapture works for element in Browser.")]
        [TestCase("2",@"CoreInput\Capture","Window",@"Compile and Verify ContentElement ReleaseMouseCapture works for element in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "ContentElementReleaseMouseCaptureApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Capture","HwndSource",@"Verify ContentElement ReleaseMouseCapture works for element in HwndSource.")]
        [TestCase("1",@"CoreInput\Capture","Window",@"Verify ContentElement ReleaseMouseCapture works for element in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementReleaseMouseCaptureApp(),"Run");
            
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

                // Construct child element and add event handlers.
                _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);
                _contentElement.LostMouseCapture += new MouseEventHandler(OnLostCapture);
                _contentElement.GotMouseCapture += new MouseEventHandler(OnCapture);
                host.AddChild(_contentElement);

                // Put the test element on the screen
                DisplayMe(host,10, 10, 100, 100);
            }
            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg) 
        {
            CoreLogger.LogStatus("Setting Capture to the element....");
            _bCaptureAPI = _contentElement.CaptureMouse();
            
            // Release capture for this event.
            _contentElement.ReleaseMouseCapture();

            base.DoExecute(arg);

            return null;
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

            // For this test we need the saved capture measurement to be true,
            // We also need current capture via InputManager to be false,
            // We also need saved lost-capture measurement to be false,
            // We also (obviously) need a LostCapture event.
            
            CoreLogger.LogStatus("Capture set via API?           " + (_bCaptureAPI));
            bool bCaptureIM = InputManagerHelper.Current.PrimaryMouseDevice.Captured!=null;
            CoreLogger.LogStatus("Capture set via InputManager?  " + (bCaptureIM));

            bool bIsMouseCaptured = _rootElement.IsMouseCaptured;
            CoreLogger.LogStatus("Capture lost via API?      " + (!bIsMouseCaptured));

            CoreLogger.LogStatus("Events found: "+_eventLog.Count);
            
            // expect non-negative event count
            bool actual = _bCaptureAPI && !bCaptureIM && !bIsMouseCaptured && (_eventLog.Count==1);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Standard Capture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnCapture(object sender, MouseEventArgs args)
        {
            // We are not setting a test flag here. 

            // Log some debugging data
            Debug.WriteLine (" ["+args.RoutedEvent.Name+"]");
            Point pt = args.GetPosition(null);
            Debug.WriteLine ("   Hello from: " + pt.X+","+pt.Y);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Standard LostCapture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnLostCapture(object sender, MouseEventArgs args)
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

        private InstrContentPanel _contentElement;
    }
}
