// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
    /// Verify ForceCursor works properly when element moves under mouse.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class FrameworkElementForceCursorElementMoveApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify ForceCursor works properly when element moves under mouse in HwndSource.")]
        [TestCase("3", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify ForceCursor works properly when element moves under mouse in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "FrameworkElementForceCursorElementMoveApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify ForceCursor works properly when element moves under mouse in HwndSource.")]
        [TestCase("3", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Verify ForceCursor works properly when element moves under mouse in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new FrameworkElementForceCursorElementMoveApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Adding ArrowCD cursor to root canvas...");
            Canvas cvs = new Canvas();
            cvs.Cursor = Cursors.ArrowCD;

            // Build button for this canvas
            Button btn = new Button();
            Canvas.SetTop(btn, 0);
            Canvas.SetLeft(btn, 0);
            btn.Height = 100;
            btn.Width = 100;
            btn.Cursor = Cursors.Pen;

            // Build element for this button
            InstrFrameworkPanel buttonContent = new InstrFrameworkPanel();
            buttonContent.Height = 50;
            buttonContent.Width = 50;
            buttonContent.Cursor = Cursors.Cross;

            // Add element to button
            btn.Content = buttonContent;

            // Display canvas.
            cvs.Children.Add(btn);

            // Put the test element on the screen
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            Canvas cvs = (Canvas)(_rootElement);
            Button btn = (Button)(cvs.Children[0]);
            FrameworkElement buttonContent = (FrameworkElement)btn.Content;

            btn.ForceCursor = true;

            CoreLogger.LogStatus("Moving mouse to target...");
            MouseHelper.Move(buttonContent);

            IntPtr actualMoveToCursor = NativeMethods.GetCursor();
            IntPtr expectedMoveToCursor = NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_PEN);

            CoreLogger.LogStatus("Mouse Directly Over: " + Mouse.DirectlyOver);
            CoreLogger.LogStatus("Found cursor: " + actualMoveToCursor + ", expected IDC_PEN: " + expectedMoveToCursor);
            Assert(actualMoveToCursor == expectedMoveToCursor, "Expected IDC_PEN after moving on top of button content");

            CoreLogger.LogStatus("Shrinking contained element to a point...");
            buttonContent.Height = 1;
            buttonContent.Width = 1;

            IntPtr actualCursor = NativeMethods.GetCursor();
            IntPtr expectedCursor = NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_PEN);

            CoreLogger.LogStatus("Mouse Directly Over: " + Mouse.DirectlyOver);
            CoreLogger.LogStatus("Found cursor: " + actualCursor + ", expected IDC_PEN: " + expectedCursor);
            Assert(actualCursor == expectedCursor, "Expected IDC_PEN");

            CoreLogger.LogStatus("Disable button Force cursor after shrink!");
            btn.ForceCursor = false;

            IntPtr actualCursorNoForce = NativeMethods.GetCursor();
            IntPtr expectedCursorNoForce = NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_CROSS);

            CoreLogger.LogStatus("Mouse Directly Over: " + Mouse.DirectlyOver);
            CoreLogger.LogStatus("Found cursor: " + actualCursorNoForce + ", expected IDC_CROSS: " + expectedCursorNoForce);
            Assert(actualCursorNoForce == expectedCursorNoForce, "Expected IDC_CROSS after cursor shrink");

            CoreLogger.LogStatus("Force cursor on canvas!");
            cvs.ForceCursor = true;

            IntPtr actualCursorForce = NativeMethods.GetCursor();
            IntPtr expectedCursorForce = NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_ARROWCD);

            CoreLogger.LogStatus("Mouse Directly Over: " + Mouse.DirectlyOver);
            CoreLogger.LogStatus("Found cursor: " + actualCursorForce + ", expected IDC_ARROWCD: " + expectedCursorForce);
            Assert(actualCursorForce == expectedCursorForce, "Expected IDC_ARROWCD");

            CoreLogger.LogStatus("Moving container element...");
            Canvas.SetTop(btn, 100);
            Canvas.SetLeft(btn, 100);

            MouseHelper.Move(cvs);

            base.DoExecute(arg);
            return null;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            CoreLogger.LogStatus("Validating...");

            IntPtr actualCursorForce2 = NativeMethods.GetCursor();
            IntPtr expectedCursorForce2 = NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_ARROW);

            Canvas cvs = (Canvas)(_rootElement);
            CoreLogger.LogStatus("Mouse Directly Over: (expect null) " + Mouse.DirectlyOver);
            CoreLogger.LogStatus("Directly Over canvas? (expect true) : " + (Mouse.DirectlyOver == null));
            CoreLogger.LogStatus("Over canvas? : (expect false) " + (cvs.IsMouseOver));
            CoreLogger.LogStatus("Force cursor? : (expect true) " + cvs.ForceCursor);
            CoreLogger.LogStatus("Canvas cursor? : (expect default) " + cvs.Cursor);

            CoreLogger.LogStatus("Found cursor: " + actualCursorForce2 + ", expected IDC_ARROW: " + expectedCursorForce2);
            Assert(actualCursorForce2 == expectedCursorForce2, "Expected IDC_ARROW");

            bool eventFound = (true);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }
    }
}
