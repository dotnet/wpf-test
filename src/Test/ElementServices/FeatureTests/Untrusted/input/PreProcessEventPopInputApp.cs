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
using Microsoft.Test.Input;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify InputManager PreProcessInputEventArgs.PopInput works as expected.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input events.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class PreProcessEventPopInputApp : TestApp
    {


        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCaseContainerAttribute("All", "All", "1", @"CoreInput\InputManager", TestCaseSecurityLevel.FullTrust, "Verify InputManager PreProcessInputEventArgs.PopInput works as expected.")]
        [TestCaseContainerAttribute("TestApplicationStub", "Browser", "1", @"CoreInput\InputManager", "Verify InputManager PreProcessInputEventArgs.PopInput works as expected.")]
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
            CoreLogger.LogStatus("Constructing window....");


            // SPECIAL NOTE: According to breaking change ,
            // in order for activation input events to be heard,
            // activation input event handlers must be attached BEFORE creating the source.

            // Attaching InputManagerEvents
            InputManagerHelper.CurrentHelper.PreProcessInput += new PreProcessInputEventHandler(Current_PreProcessInput);

            // Construct test element, add event handling
            _rootElement = new InstrPanel();

            // Put the test element on the screen
            DisplayMe(_rootElement, 50, 50, 50, 30);

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
                    KeyboardHelper.TypeKey("A");
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

            // For this test we expect 1 input report event, not 2.
            // We also expect the return value from PopInput to be the input event args we pushed.

            CoreLogger.LogStatus("Events found: " + _eventLog.Count);

            // expect non-negative event count
            int actual = (_eventLog.Count);
            int expected = 1;
            bool eventFound = (actual >= expected) &&
                 (_popInputItem != null) &&
                 (_popInputItem.Input.GetType() == typeof(MouseWheelEventArgs));

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private void Current_PreProcessInput(object sender, PreProcessInputEventArgs e)
        {
            CoreLogger.LogStatus("Preprocess!    " + e.ToString() + " [sender=" + sender.GetType().ToString() + "]");
            CoreLogger.LogStatus(" Canceled?     " + e.Canceled);

            // We check for two staging items. One on activation, another on a pushed event.
            // If PopInput is working correctly, we should never see the pushed event.
            InputEventArgs inputArgs = e.StagingItem.Input;

            CoreLogger.LogStatus(" Staging item: " + inputArgs.RoutedEvent.Name + " [" + inputArgs.Timestamp + "]");
            if (inputArgs.RoutedEvent == Mouse.MouseWheelEvent)
            {
                // Process MouseWheel action
                CoreLogger.LogStatus("Adding InputEventArgs (MouseWheel)...");
                _eventLog.Add(inputArgs);
            }
            else if (InputHelper.IsInputReport(inputArgs.RoutedEvent))
            {
                InputReportEventArgsWrapper inputReportArgs = new InputReportEventArgsWrapper(inputArgs);
                InputReportWrapper ir = inputReportArgs.Report;

                CoreLogger.LogStatus(" Report:       " + ir.Name);

                if (ir.Name == "RawKeyboardInputReport")
                {
                    RawKeyboardInputReportWrapper rkim = new RawKeyboardInputReportWrapper(ir);

                    CoreLogger.LogStatus("   Actions:       " + rkim.Actions.ToString());
                    if ((rkim.Actions & Microsoft.Test.Input.RawKeyboardActions.Activate) != 0)
                    {
                        // Pop Test

                        // Process Activate action
                        CoreLogger.LogStatus("Adding InputEventArgs (Activate)...");
                        _eventLog.Add(inputArgs);

                        // Create valid argument package to be pushed in.
                        InputManager im = sender as InputManager;
                        MouseWheelEventArgs wargs = new MouseWheelEventArgs(im.PrimaryMouseDevice, Environment.TickCount, 0);
                        wargs.RoutedEvent = Mouse.MouseWheelEvent;

                        ProcessInputEventArgs argsWrapper = e;

                        // Push it 
                        CoreLogger.LogStatus("Pushing InputEventArgs...");
                        argsWrapper.PushInput(wargs, null);

                        // Pop input to undo what we just did, stash any reported value.
                        if (_popInputItem == null)
                        {
                            CoreLogger.LogStatus("Popping InputEventArgs...");
                            _popInputItem = argsWrapper.PopInput();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Store record of our popped item.
        /// </summary>
        private StagingAreaInputItem _popInputItem = null;

        /// <summary>
        /// Store record of our raised events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();
    }
}
