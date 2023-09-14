// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
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
    /// Verify key status is reported correctly after showing WinForms message box.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyAfterWinFormsMessageBoxApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify key status is reported correctly after showing WinForms message box in HwndSource.")]
        [TestCase("3", @"CoreInput\Keyboard", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify key status is reported correctly after showing WinForms message box in Browser.")]
        [TestCase("3", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify key status is reported correctly after showing WinForms message box in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            try
            {
                GenericCompileHostedCase.RunCase(
                    "Avalon.Test.CoreUI.CoreInput",
                    "KeyAfterWinFormsMessageBoxApp",
                    "Run",
                    hostType);
            }
            finally
            {
                Input.SendKeyboardInput((byte)KeyInterop.VirtualKeyFromKey(Key.LeftShift), false);
            }
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("3", @"CoreInput\Keyboard", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify key status is reported correctly after showing WinForms message box in HwndSource.")]
        [TestCase("3", @"CoreInput\Keyboard", "Window", TestCaseSecurityLevel.FullTrust, @"Verify key status is reported correctly after showing WinForms message box in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            try
            {
                exe.Run(new KeyAfterWinFormsMessageBoxApp(), "Run");
            }
            finally
            {
                Input.SendKeyboardInput((byte)KeyInterop.VirtualKeyFromKey(Key.LeftShift), false);
            } 
        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");
            InstrFrameworkPanel[] canvases = new InstrFrameworkPanel[] { new InstrFrameworkPanel() };

            CoreLogger.LogStatus("Adding event handlers....");
            canvases[0].KeyDown += new KeyEventHandler(OnKeyDown);

            // Put the test element on the screen
            DisplayMe(canvases[0], 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {

            FrameworkElement cvs = (FrameworkElement)(_rootElement);

            CoreLogger.LogStatus("Focusing....");
            cvs.Focusable = true;
            cvs.Focus();

            CoreLogger.LogStatus("Keying down on target...");
            KeyboardHelper.PressKey(Key.LeftShift);

            // NOTE - we expect a key to still be pressed at this point.
            // We sanity-check for it here.
            KeyStates keydownKeyStates = Keyboard.GetKeyStates(Key.LeftShift);
            CoreLogger.LogStatus("KeyStates while key down (expect Down): " + keydownKeyStates.ToString());
            Assert((keydownKeyStates & KeyStates.Down) != 0, "Key not pressed before message box!");

            CoreLogger.LogStatus("Showing message box....");
            AutoCloseMessageBox mb = new AutoCloseWinFormsSendInputMessageBox(5000, "Wait 5 seconds for box to close", "Wait", VKeys.VkLShift, false);
            mb.Show();

            // NOTE - some input has been sent to the system while the message box was shown.
            // Again, we sanity-check for it here.
            keydownKeyStates = Keyboard.GetKeyStates(Key.LeftShift);
            CoreLogger.LogStatus("KeyStates while key down after message box shown (expect not Down): " + keydownKeyStates.ToString());
            Assert((keydownKeyStates & KeyStates.Down) == 0, "Key still pressed after message box!");

            // And let's get on with final validation.
            CoreLogger.LogStatus("Shown message box. Getting ready to verify....");

            base.DoExecute(arg);

            return null;
        }


        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            KeyStates messageboxKeyStates = Keyboard.GetKeyStates(Key.LeftShift);
            CoreLogger.LogStatus("KeyStates (expect not down): " + messageboxKeyStates.ToString());
            Assert((messageboxKeyStates & KeyStates.Down) == 0, "Key still down after message box!");
            CoreLogger.LogStatus("Key events found (expect 1): " + _eventLog.Count);
            Assert(_eventLog.Count == 1, "Oh no - incorrect number of events");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

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

            // Stop routing this event.
            e.Handled = true;
        }

        private List<KeyEventArgs> _eventLog = new List<KeyEventArgs>();
    }
}

