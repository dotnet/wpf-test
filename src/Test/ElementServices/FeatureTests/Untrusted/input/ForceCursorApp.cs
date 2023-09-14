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
    /// Verify setting FrameworkElement ForceCursor overrides Cursor on child element.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ForceCursorApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Focus","HwndSource",@"Compile and Verify setting FrameworkElement ForceCursor overrides Cursor on child element in HwndSource.")]
        [TestCase("2",@"CoreInput\Focus","Browser",@"Compile and Verify setting FrameworkElement ForceCursor overrides Cursor on child element in Browser.")]
        [TestCase("2",@"CoreInput\Focus","Window",@"Compile and Verify setting FrameworkElement ForceCursor overrides Cursor on child element in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "ForceCursorApp",
                "Run", 
                hostType);
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Focus","HwndSource",@"Verify setting FrameworkElement ForceCursor overrides Cursor on child element in HwndSource.")]
        [TestCase("1",@"CoreInput\Focus","Window",@"Verify setting FrameworkElement ForceCursor overrides Cursor on child element in window.")]        
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ForceCursorApp(),"Run");
            
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

                // Construct canvas
                Canvas cvs = new Canvas();

                // Construct test element with dimensions
                _panel = new InstrFrameworkPanel();
                _panel.Focusable = true;
                _panel.Name = "nOnLostKeyboardFocusbtn" + DateTime.Now.Ticks;
                Canvas.SetTop(_panel, 0);
                Canvas.SetLeft(_panel, 0);
                _panel.Height = 90;
                _panel.Width = 90;

                // Set test element cursor
                _panel.Cursor = Cursors.SizeAll;

                // Set parent override (force) cursor
                cvs.Cursor = Cursors.SizeNWSE;
                _bOriginalForceCursor = cvs.ForceCursor;
                cvs.ForceCursor = true;

                // Put the test element on the canvas
                cvs.Children.Add(_panel);

                // Put the canvas on the screen
                DisplayMe(cvs, 10, 10, 100, 100);

            }
            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

            return null;
        }

        InstrFrameworkPanel _panel;  

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd) 
        {
            InputCallback[] ops = new InputCallback[] {

                delegate
                {
                    MouseHelper.Move(_panel);
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

            // For this test we need the parent cursor to take precedence.
            // We also need the default ForceCursor to be off.
            
            // expect matching stock cursors
            IntPtr actualCursor = NativeMethods.GetCursor();
            IntPtr expectedCursor = NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_SIZENWSE);
            CoreLogger.LogStatus("Found cursor: " + actualCursor + ", expected: " + expectedCursor);
            CoreLogger.LogStatus("Original force cursor? " + _bOriginalForceCursor);

            bool eventFound = (expectedCursor == actualCursor) && (!_bOriginalForceCursor);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Stores original ForceCursor value.
        /// </summary>
        private bool _bOriginalForceCursor;
    }
}
