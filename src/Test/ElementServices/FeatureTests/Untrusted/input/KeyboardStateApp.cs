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
    /// Verify keyboard state properties via GetKeyState.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyboardStateApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify keyboard state properties via GetKeyState in HwndSource.")]
        [TestCase("1", @"CoreInput\Keyboard", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify keyboard state properties via GetKeyState in Browser.")]
        [TestCase("3", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify keyboard state properties via GetKeyState in window.")]
        [TestCase("3", @"CoreInput\Keyboard", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify keyboard state properties via GetKeyState in NavigationWindow.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.CoreInput",
                "KeyboardStateApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify keyboard state properties via GetKeyState in HwndSource.")]
        [TestCase("2", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Verify keyboard state properties via GetKeyState in window.")]
        [TestCase("2", @"CoreInput\Keyboard", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify keyboard state properties via GetKeyState in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyboardStateApp(), "Run");

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
            _rootElement.AddHandler(Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);

            InputMethod.SetPreferredImeState(_rootElement, InputMethodState.Off);

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
                    DoBeforeExecute();
                },               
                delegate
                {
                    KeyboardHelper.TypeKey(Key.Z);
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
            _oldKeyStates = InputManagerHelper.Current.PrimaryKeyboardDevice.GetKeyStates(Key.Z);

            CoreLogger.LogStatus("KeyStates (old)='" + _oldKeyStates.ToString() + "'");

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

            // Note: for this test we are concerned about the proper key state being reported.

            CoreLogger.LogStatus("Events found: " + _eventLog.Count);

            bool actual = (KeyStates.Down & _newKeyStates) != 0;
            bool expected = true;

            bool eventFound = (expected == actual);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            // Set test flags
            _eventLog.Add(args);

            _newKeyStates = InputManagerHelper.Current.PrimaryKeyboardDevice.GetKeyStates(Key.Z);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: " + args.Key.ToString() + ", " + args.KeyStates.ToString());
            CoreLogger.LogStatus("   KeyStates from GetKeyStates: '" + _newKeyStates.ToString() + "'");

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();

        /// <summary>
        /// Store record of our key's previous states.
        /// </summary>
        private KeyStates _oldKeyStates;

        /// <summary>
        /// Store record of our key's current states.
        /// </summary>
        private KeyStates _newKeyStates;
    }
}
