// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
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
    /// Verify Mouse cursor from custom cursor .ANI stream on a mouse move.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class UIElementMouseCursorAniStreamApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Cursor", "HwndSource", @"Compile and Verify Mouse cursor from custom cursor .ANI stream on a mouse move for ContentElement in HwndSource.")]
        [TestCase("2", @"CoreInput\Cursor", "Browser", @"Compile and Verify Mouse cursor from custom cursor .ANI stream on a mouse move for ContentElement in Browser.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", @"Compile and Verify Mouse cursor from custom cursor .ANI stream on a mouse move for ContentElement in window.")]
        [TestCaseSupportFile(@"anitest.ani")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);
            string[] contents = { "anitest.ani" };

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "UIElementMouseCursorAniStreamApp",
                "Run",
                hostType, null, contents);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Cursor", "HwndSource", @"Verify Mouse cursor from custom cursor .ANI stream on a mouse move for ContentElement in HwndSource.")]
        [TestCase("1", @"CoreInput\Cursor", "Window", @"Verify Mouse cursor from custom cursor .ANI stream on a mouse move for ContentElement in window.")]
        [TestCaseSupportFile(@"anitest.ani")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new UIElementMouseCursorAniStreamApp(), "Run");

        }
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");

            // Construct test element
            _rootElement = new InstrControlPanel();

            string cursorFile = "anitest.ani";

            if (!File.Exists(cursorFile))
            {
                CoreLogger.LogStatus("The file " + cursorFile + " was not found.");
            }

            ((FrameworkElement)_rootElement).Cursor = IOHelper.LoadCursorObjectFromFile(cursorFile);

            // Put the test element on the screen
            DisplayMe(_rootElement, 10, 10, 100, 100);

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

            // Note: for this test we are concerned about whether the proper cursor is set.

            // expect non-zero, non-arrow cursors
            IntPtr actualCursor = NativeMethods.GetCursor();
            bool actual = ((((FrameworkElement)_rootElement).Cursor != null) &&
                (actualCursor != IntPtr.Zero) &&
                (actualCursor != NativeMethods.LoadCursor(NativeMethods.NullHandleRef, NativeConstants.IDC_ARROW))
                );
            bool expected = true;
            CoreLogger.LogStatus("Found positive-value cursor? " + actual + ", expected: " + expected);

            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private IntPtr _customCursor = IntPtr.Zero;
    }
}
