// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Mouse cursor on a mouse move.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class MouseCursorApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Cursor", "HwndSource", @"Compile and Verify Mouse cursor on a mouse move in HwndSource.")]
        [TestCase("1", @"CoreInput\Cursor", "Browser", @"Compile and Verify Mouse cursor on a mouse move in Browser.")]
        [TestCase("2", @"CoreInput\Cursor", "Window", @"Compile and Verify Mouse cursor on a mouse move in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "MouseCursorApp",
                "Run",
                hostType, null, null);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Cursor", "HwndSource", @"Verify Mouse cursor on a mouse move in HwndSource.")]
        [TestCase("0", @"CoreInput\Cursor", "Window", @"Verify Mouse cursor on a mouse move in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new MouseCursorApp(), "Run");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            CoreLogger.LogStatus("Constructing window....");
            
            // Construct test element, add cursor
            InstrControlPanel panel = new InstrControlPanel();
            panel.Cursor = Cursors.ArrowCD;

            // Put the test element on the screen
            DisplayMe(panel, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object sender)
        {
            InstrControlPanel cvs = (InstrControlPanel)_rootElement;

            // Check all cursors on
            _allCursorsFound = true;
            foreach (Cursor c in TestCursorLibrary.AllCursors)
            {
                cvs.Cursor = c;

                CoreLogger.LogStatus("Moving mouse to target...");
                MouseHelper.MoveOutside(_rootElement, MouseLocation.CenterRight, true);
                MouseHelper.Move(_rootElement, MouseLocation.CenterRight);
                DispatcherHelper.DoEvents(100);

                bool isCurrentCursor = TestCursorLibrary.IsCurrentCursor(c);
                CoreLogger.LogStatus("Found cursor " + TestCursorLibrary.GetName(c) + "? (expect yes) " + isCurrentCursor);

                if (!isCurrentCursor)
                {
                    _allCursorsFound = false;
                    break;
                }
            }

            base.DoExecute(sender);
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

            // Note: for this test we are concerned about whether the proper cursor is set.
            
            // expect matching stock cursors
            bool actual = _allCursorsFound;
            bool expected = true;
            CoreLogger.LogStatus("Found all cursors? (expect yes) "+actual);

            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        private bool _allCursorsFound;
    }
}
