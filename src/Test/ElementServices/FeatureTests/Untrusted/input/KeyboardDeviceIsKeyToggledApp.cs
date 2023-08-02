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
    /// Verify keyboard toggled key properties. (IsToggled, IsKeyToggled)
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyboardDeviceIsKeyToggledApp : TestApp
    {

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All","All","2",@"CoreInput\Keyboard",TestCaseSecurityLevel.FullTrust,"Compile and Verify keyboard toggled key properties. (IsToggled, IsKeyToggled).")]
        [TestCaseContainerAttribute("TestApplicationStub","Browser","2",@"CoreInput\Keyboard","Compile and Verify keyboard toggled key properties. (IsToggled, IsKeyToggled).")]
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


            // Construct test element, add event handling
            _rootElement = new InstrPanel();
            _rootElement.AddHandler(Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);

            // Put the test element on the screen
            DisplayMe(_rootElement,10, 10, 100, 100);

            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

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
            _oldIsToggled = Keyboard.IsKeyToggled(_targetKey);

            CoreLogger.LogStatus("IsToggled='" + _oldIsToggled);

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

            bool bToggleWorked = (_oldIsToggled != _newIsToggled);
            CoreLogger.LogStatus("Toggle worked? " + bToggleWorked);

            bool eventFound = bToggleWorked;

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
        /// <param name="args">Event-specific arguments.</param>
        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            // Set test flags
            _newIsToggled = args.IsToggled;

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: " + args.Key.ToString());
            CoreLogger.LogStatus("IsToggled='" + _newIsToggled);

            // Don't route this event any more.
            args.Handled = true;
        }

        /// <summary>
        /// Store record of our key's previous toggled-ness.
        /// </summary>
        private bool _oldIsToggled;

        /// <summary>
        /// Store record of our key's current toggled-ness.
        /// </summary>
        private bool _newIsToggled;

        /// <summary>
        /// Which key to use as our target?
        /// </summary>
        Key _targetKey = Key.CapsLock;

    }
}
