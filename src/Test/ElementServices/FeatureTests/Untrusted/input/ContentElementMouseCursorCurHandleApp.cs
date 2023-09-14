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
using System.Windows.Markup;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Serialization;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Mouse cursor from custom cursor .CUR handle on a mouse move for ContentElement.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class ContentElementMouseCursorCurHandleApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse cursor from custom cursor .CUR handle on a mouse move for ContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify Mouse cursor from custom cursor .CUR handle on a mouse move for ContentElement in window.")]
        [TestCaseSupportFile(@"star.cur")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);
            string[] contents = { "star.cur" };

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "ContentElementMouseCursorCurHandleApp",
                "Run",
                hostType, null, contents);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Cursor", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify Mouse cursor from custom cursor .CUR handle on a mouse move for ContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", TestCaseSecurityLevel.FullTrust, @"Verify Mouse cursor from custom cursor .CUR handle on a mouse move for ContentElement in window.")]
        [TestCaseSupportFile(@"star.cur")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new ContentElementMouseCursorCurHandleApp(), "Run");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {


            CoreLogger.LogStatus("Constructing window....");

            // Construct test element
            InstrContentPanelHost host = new InstrContentPanelHost();
            _frameworkContentElement = new InstrFrameworkContentPanel("rootLeaf", "Sample", host);
            host.AddChild(_frameworkContentElement);

            // Add cursor to test element
            string assemblyPath = GetDirectoryNameOfAssembly();
            string cursorFile = assemblyPath + "\\star.cur";

            if (!File.Exists(cursorFile))
            {
                CoreLogger.LogStatus("The file " + cursorFile + " was not found at " + assemblyPath);
            }

            _customCursor = new TestCursorSafeHandle(NativeMethods.LoadCursorFromFile(cursorFile));
            ((FrameworkContentElement)_frameworkContentElement).Cursor = CursorInteropHelper.Create(_customCursor);

            // Put the test element on the screen
            DisplayMe(host, 10, 10, 100, 100);

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
                    MouseHelper.Move((IntPtr)hwnd);
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

            // Note: for this test we are concerned about whether the proper cursor is set.

            // expect matching custom cursors
            IntPtr actual = NativeMethods.GetCursor();
            IntPtr expected = _customCursor.DangerousGetHandle();
            CoreLogger.LogStatus("Found cursor: " + actual + ", expected: " + expected);

            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private SafeHandle _customCursor;

        private FrameworkContentElement _frameworkContentElement;
    }
}
