// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Mouse cursor from custom cursor .CUR handle on a mouse move.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementMouseCursorCurHandleApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse cursor from custom cursor .CUR handle on a mouse move in HwndSource.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse cursor from custom cursor .CUR handle on a mouse move in window.")]
        [TestCaseSupportFile(@"star.cur")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);
            string[] contents = { "star.cur" };

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "UIElementMouseCursorCurHandleApp",
                "Run",
                hostType, null, contents);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify Mouse cursor from custom cursor .CUR handle on a mouse move in HwndSource.")]
        [TestCase("1", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Verify Mouse cursor from custom cursor .CUR handle on a mouse move in window.")]
        [TestCaseSupportFile(@"star.cur")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementMouseCursorCurHandleApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test element
            _rootElement = new InstrControlPanel();

            // Add cursor to test element
            string assemblyPath = GetDirectoryNameOfAssembly();
            string cursorFile = assemblyPath + "\\star.cur";

            if (!File.Exists(cursorFile))
            {
                CoreLogger.LogStatus("The file " + cursorFile + " was not found at " + assemblyPath);
            }

            _cursorHandle = new TestCursorSafeHandle(NativeMethods.LoadCursorFromFile(cursorFile));
            _cursor = CursorInteropHelper.Create(_cursorHandle); ;
            ((FrameworkElement)_rootElement).Cursor = _cursor;

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute the test.
        /// </summary>
        protected override object DoExecute(object sender)
        {
            FrameworkElement fe = (FrameworkElement)_rootElement;

            MouseHelper.Move(_rootElement);

            // Verify custom cursor is the current cursor.
            if (!TestCursorLibrary.IsCurrentCursor(_cursorHandle))
            {
                throw new Microsoft.Test.TestValidationException("The current cursor is not the expected value.");
            }

            // Dispose custom cursor.
            _cursor.Dispose();

            DispatcherHelper.DoEvents();

            // Verify moving over element with custom cursor doesn't explode after
            // cursor was disposed.
            MouseHelper.MoveOutside(_rootElement, MouseLocation.CenterRight, true);
            MouseHelper.Move(_rootElement);

            // Set cursor to built-in cursor and dispose it.
            fe.Cursor = Cursors.Arrow;

            Cursor cursor = fe.Cursor;
            cursor.Dispose();

            DispatcherHelper.DoEvents();

            return base.DoExecute(sender);
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object arg)
        {
            this.TestPassed = true;

            return null;
        }

        private SafeHandle _cursorHandle = null;
        private Cursor _cursor = null;
    }
}
