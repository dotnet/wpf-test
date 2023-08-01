// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
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
    /// Verify KeyDown event fires on a key down.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyDownApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust,@"Compile and Verify KeyDown event fires on a key down in HwndSource.")]
        [TestCase("0", @"CoreInput\Keyboard", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify KeyDown event fires on a key down in Browser.")]
        [TestCase("3", @"CoreInput\Keyboard", "Window",TestCaseSecurityLevel.FullTrust, @"Compile and Verify KeyDown event fires on a key down in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "KeyDownApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("0", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust,@"Verify KeyDown event fires on a key down in HwndSource.")]
        [TestCase("0", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust,@"Verify KeyDown event fires on a key down in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyDownApp(), "Run");

        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test element, add event handling
            _rootElement = new InstrPanel();
            _rootElement.KeyDown += new KeyEventHandler(OnKeyDown);

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
                    KeyboardHelper.EnsureFocus(_rootElement);
                },               
                delegate
                {
                    KeyboardHelper.TypeKey(Key.F2);
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

            // Note: for this test we are only concerned about whether the event fires at all.
            // Hence, we are only concerned if 0 events are found.
            // Any positive number of event counts is fine.

            CoreLogger.LogStatus("Events found: (expect more than 0) " + _eventLog.Count);

            // expect non-negative event count
            bool actual = (_eventLog.Count > 0);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Set test flag
            _eventLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: " + e.Key.ToString() + ", " + e.KeyStates.ToString());

            if (e.ImeProcessedKey != Key.None)
            {
                throw new Microsoft.Test.TestValidationException("KeyEventArgs.ImeProcessedKey != Key.None");
            }

            if (PresentationHelper.FromKeyEventArgs(e) != PresentationHelper.FromElement(_rootElement))
            {
                throw new Microsoft.Test.TestValidationException("KeyEventArgs.InputSource does not equal the root element's PresentationSource.");
            }

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<KeyEventArgs> _eventLog = new List<KeyEventArgs>();
    }
}
