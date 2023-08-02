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
    /// Verify Mouse override cursor on a mouse move for UIElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementMouseCursorOverrideApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2",@"CoreInput\Cursor","HwndSource",@"Compile and Verify Mouse override cursor on a mouse move for UIElement in HwndSource.")]
        [TestCase("1",@"CoreInput\Cursor","Browser",@"Compile and Verify Mouse override cursor on a mouse move for UIElement in Browser.")]
        [TestCase("3",@"CoreInput\Cursor","Window",@"Compile and Verify Mouse override cursor on a mouse move for UIElement in window.")]        
        public static void LaunchTestCompile() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);


            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput", 
                "UIElementMouseCursorOverrideApp",
                "Run", 
                hostType,null,null );
            
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1",@"CoreInput\Cursor","HwndSource",@"Verify Mouse override cursor on a mouse move for UIElement in HwndSource.")]
        [TestCase("2",@"CoreInput\Cursor","Window",@"Verify Mouse override cursor on a mouse move for UIElement in window.")]               
        public static void LaunchTest() 
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType),DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementMouseCursorOverrideApp(),"Run");
            
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {

            CoreLogger.LogStatus("Constructing tree....");

            // Construct related Win32 window


            // Construct test element, add cursor
            _rootElement = new InstrControlPanel();
            ((FrameworkElement)_rootElement).Cursor = Cursors.ArrowCD;

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);

            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

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
            CoreLogger.LogStatus("Resetting override cursor....");
            Mouse.OverrideCursor = Cursors.Help;

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

            // Note: for this test we are concerned about whether the proper cursor is set.

            // expect matching stock cursors
            IntPtr actual = NativeMethods.GetCursor();
            IntPtr expected = NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_HELP);
            CoreLogger.LogStatus("Found cursor: " + actual + ", expected: " + expected);

            bool eventFound = (actual == expected) && (Mouse.OverrideCursor == Cursors.Help);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }
    }
}
