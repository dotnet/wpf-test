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
using System.Windows.Interop;
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
    /// Verify Mouse Capture returns to Captured element after dismissing modal window.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify Mouse Capture returns to Captured element after dismissing modal window.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"CoreInput\Capture")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CaptureAfterModalWindowApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CaptureAfterModalWindowApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus("App object: "+app.ToString());

            app.VerboseTrace = false;

            CoreLogger.LogStatus("Running app...");
            app.Run();
            CoreLogger.LogStatus("App run!");
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
                HwndSource source = CreateStandardSource(10, 10, 100, 100);

                // Construct test element, add event handling
                Canvas[] canvases = new Canvas[] { new Canvas() };
                foreach (Canvas cvs in canvases)
                {
                    // Captureable panel 1
                    InstrFrameworkPanel panel = new InstrFrameworkPanel();
                    panel.Name = "nOnLostCapturebtn" + DateTime.Now.Ticks;
                    Canvas.SetTop(panel, 0);
                    Canvas.SetLeft(panel, 0);
                    panel.Height = 40;
                    panel.Width = 40;

                    // Captureable panel 2 - with events attached
                    InstrFrameworkPanel panel2 = new InstrFrameworkPanel();
                    panel2.Name = "Capturebtn" + DateTime.Now.Ticks;
                    Canvas.SetTop(panel2, 50);
                    Canvas.SetLeft(panel2, 50);
                    panel2.Height = 40;
                    panel2.Width = 40;
                    panel2.LostMouseCapture += new MouseEventHandler(OnLostMouseCapture);
                    panel2.GotMouseCapture += new MouseEventHandler(OnGotMouseCapture);

                    cvs.Children.Add(panel);
                    cvs.Children.Add(panel2);
                }

                // Put the test element on the screen
                _rootElement = canvases[0];
                Visual v = _rootElement;
                source.RootVisual = v;

                // Save Win32 window handle for later
                _hwnd = new HandleRef(source, source.Handle);

                // Create child window for this window.
                _modalWindow = new SimpleModalTestWindow(source.Handle);

            }
            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

            // With test element on the screen, set Capture and then show our dialog
            
            {
                Canvas cvs = _rootElement as Canvas;
                Debug.Assert(cvs.Children.Count == 2, "Canvas children not added correctly");
                CoreLogger.LogStatus("Setting Capture to all our elements (leaving Capture on our target element)...");
                cvs.Children[0].CaptureMouse();
                cvs.Children[1].CaptureMouse();

                CoreLogger.LogStatus("Popping up modalWindow...");
                _modalWindow.Show();
            }

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd) 
        {
            HandleRef hwndDialog = new HandleRef(null, _modalWindow.Source.Handle);

            // Execute Alt+F4 to close dialog.
            CoreLogger.LogStatus("Pressing Alt+F4 to close dialog...");

            InputCallback[] ops = new InputCallback[] 
            {
                delegate
                {
                   KeyboardHelper.TypeKey(Key.F4, ModifierKeys.Alt);
                }                
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

            // For this test we need 1 GotCapture event to fire on our target element, on setup.
            // We also need 1 LostCapture, on modal dialog popup.
            // We t want to lose capture on showing window, so we verify that Mouse.Captured is valid

            CoreLogger.LogStatus("Events found (expect 2): " + _eventLog.Count);
            CoreLogger.LogStatus(" Mouse captured? (expect no): " + (Mouse.Captured != null));

            bool actual = (_eventLog.Count == 2) && (Mouse.Captured == null);
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
        private void OnLostMouseCapture(object sender, MouseEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            string senderID = (sender is FrameworkElement) ? ((FrameworkElement)sender).Name : "";
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "] [" + senderID + "]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Standard Capture event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnGotMouseCapture(object sender, MouseEventArgs args)
        {
            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            string senderID = (sender is FrameworkElement) ? ((FrameworkElement)sender).Name : "";
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "] [" + senderID + "]");
            Point pt = args.GetPosition(null);
            CoreLogger.LogStatus("   Hello from: " + pt.X + "," + pt.Y);

            // Don't route this event any more.
            args.Handled = true;
        }



        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();

        /// <summary>
        /// Store reference to our test window.
        /// </summary>
        private SimpleModalTestWindow _modalWindow = null;
    }
}
