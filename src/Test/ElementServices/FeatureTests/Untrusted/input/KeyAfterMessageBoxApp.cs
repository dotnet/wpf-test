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
    /// Verify key status is reported correctly after showing Win32 message box.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyAfterMessageBoxApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All","All","3",@"CoreInput\Keyboard",TestCaseSecurityLevel.FullTrust,"Compile and Verify key status is reported correctly after showing Win32 message box.")]
        public void LaunchTest()
        {
            Run();
        }        


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            Input.ResetKeyboardState();

            CoreLogger.LogStatus("Constructing element....");
            _rootElement = new InstrFrameworkPanel();

            CoreLogger.LogStatus("Adding event handlers....");
            _rootElement.KeyDown += new KeyEventHandler(OnKeyDown);

            // Put the test element on the screen
            CoreLogger.LogStatus("Showing window....");
            DisplayMe(_rootElement, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff
        /// </summary>
        protected override object DoExecute(object sender)
        {
            KeyboardHelper.EnsureFocus(_rootElement);

            CoreLogger.LogStatus("Keying down on target...");
            KeyboardHelper.PressKey(Key.RightCtrl);
            KeyStates keydownKeyStates = Keyboard.GetKeyStates(Key.RightCtrl);
            CoreLogger.LogStatus("KeyStates (pre-show message box):  " + keydownKeyStates.ToString());
            Assert((keydownKeyStates & KeyStates.Down) != 0, "Key not down before message box! (it should be)");

            CoreLogger.LogStatus("Showing message box....");
            AutoCloseMessageBox mb = new AutoCloseWin32SendInputMessageBox(1000, "Wait 1 second for box to close", "Wait", VKeys.VkRControl, false);

            // NOTE - we expect a key to still be pressed at this point.
            // We sanity-check for it here.
            keydownKeyStates = Keyboard.GetKeyStates(Key.RightCtrl);
            CoreLogger.LogStatus("KeyStates (post-show message box):  " + keydownKeyStates.ToString());
            Assert((keydownKeyStates & KeyStates.Down) != 0, "Key not pressed down after message box! (it should be)");

            mb.Show();

            // And let's get on with final validation.
            CoreLogger.LogStatus("Shown message box. Getting ready to verify....");

            base.DoExecute(sender);
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

            KeyStates messageboxKeyStates = Keyboard.GetKeyStates(Key.RightCtrl);
            CoreLogger.LogStatus("KeyStates (post-show message box): " + messageboxKeyStates.ToString());
            Assert((messageboxKeyStates & KeyStates.Down) == 0, "Key still down after message box! (it shouldn't be)");

            CoreLogger.LogStatus("Key events found: " + _eventLog.Count);
            Assert(_eventLog.Count == 1, "Oh no - incorrect number of events (expected 1)");

            TestPassed = true;
            CoreLogger.LogStatus("Test passed");

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

