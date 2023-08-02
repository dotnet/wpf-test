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
    /// Verify KeyEventArgs key state properties.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyStateApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify KeyEventArgs key state properties in HwndSource.")]
        [TestCase("2", @"CoreInput\Keyboard", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify KeyEventArgs key state properties in Browser.")]
        [TestCase("2", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify KeyEventArgs key state properties in window.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "KeyStateApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify KeyEventArgs key state properties in HwndSource.")]
        [TestCase("1", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Verify KeyEventArgs key state properties in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyStateApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test element, add event handling
            InstrPanel panel = new InstrPanel();
            panel.AddHandler(Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);

            // Put the test element on the screen
            DisplayMe(panel, 10, 10, 100, 100);

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
                    KeyboardHelper.TypeKey(_targetKey);
                }                
            };
            return ops;
        }

        /// <summary>
        /// Execute stuff right before the test operations.
        /// </summary>
        private void DoBeforeExecute()
        {
            CoreLogger.LogStatus("Saving keyboard state....");
            _oldKeyStates = Keyboard.GetKeyStates(_targetKey);
            _oldIsDown = Keyboard.IsKeyDown(_targetKey);
            _oldIsUp = Keyboard.IsKeyUp(_targetKey);

            CoreLogger.LogStatus("KeyStates,IsDown,IsUp=" + _oldKeyStates.ToString() + "'," + _oldIsDown + "," + _oldIsUp);

            KeyboardHelper.EnsureFocus(_rootElement);

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

            // Note: for this test we are concerned about whether events fire for a key press.
            // We are also need to inspect the proper key state properties corresponding to each event.
            // For keydown events, keys must be down and not up. 
            // For keyup events, keys must be up and not down.
            // For all key down events, the toggle state must change.

            int expected = 1;
            int actual = _buttonLog.Count;

            CoreLogger.LogStatus("Events found: " + actual + " (expected " + expected + ")");

            bool bKeyDownStateChange = (!_oldIsDown) && (_newIsDown);
            CoreLogger.LogStatus("Keys down? (expect yes) " + bKeyDownStateChange);

            bool bKeyUpStateChange = (_oldIsUp) && (!_newIsUp);
            CoreLogger.LogStatus("Keys up? (expect yes) " + bKeyUpStateChange);

            CoreLogger.LogStatus("Keys repeat? (expect no) " + _newIsRepeat);

            bool eventFound = (expected == actual) && bKeyDownStateChange && bKeyUpStateChange && !_newIsRepeat;

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            Input.ResetKeyboardState();

            return null;
        }

        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="e">Event-specific arguments.</param>
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            // Set test flags
            _buttonLog.Add(e);
            _newKeyStates = e.KeyStates;
            _newIsDown = e.IsDown;
            _newIsUp = e.IsUp;
            _newIsRepeat = e.IsRepeat;

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: " + e.Key.ToString() + ", " + _newKeyStates.ToString());
            CoreLogger.LogStatus("KeyStates,IsDown,IsUp,IsRepeat='" + _newKeyStates.ToString() + "'," + _newIsDown + "," + _newIsUp + "," + _newIsRepeat);

            // Don't route this event any more.
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired buttons.
        /// </summary>
        private ArrayList _buttonLog = new ArrayList();

        /// <summary>
        /// Store record of our key's previous state.
        /// </summary>
        private KeyStates _oldKeyStates;

        /// <summary>
        /// Store record of our key's current state.
        /// </summary>
        private KeyStates _newKeyStates;

        /// <summary>
        /// Store record of our key's previous is-down-ness.
        /// </summary>
        private bool _oldIsDown;

        /// <summary>
        /// Store record of our key's current is-down-ness.
        /// </summary>
        private bool _newIsDown;

        /// <summary>
        /// Store record of our key's previous is-up-ness.
        /// </summary>
        private bool _oldIsUp;

        /// <summary>
        /// Store record of our key's current is-up-ness.
        /// </summary>
        private bool _newIsUp;

        /// <summary>
        /// Store record of our key's current is-repeat-ness.
        /// </summary>
        private bool _newIsRepeat;

        /// <summary>
        /// Which key to use as our target?
        /// </summary>
        Key _targetKey = Key.Scroll;

    }
}
