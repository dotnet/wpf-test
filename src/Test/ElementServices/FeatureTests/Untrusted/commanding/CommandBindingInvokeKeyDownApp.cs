// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
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

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify CommandBinding Invoke event works with KeyDown event.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <





    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandBinding Invoke event works with KeyDown event.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("1")]
    public class CommandBindingInvokeKeyDownApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingInvokeKeyDownApp();
            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus("App run!");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");

            // Construct related Win32 window
            HwndSource source = CreateStandardSource(10, 10, 100, 100);

            // Construct test element
            _rootElement = new InstrPanel();
            _rootElement.KeyDown += new KeyEventHandler(OnKeyDown);

            // Set up command
            RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command=" + sampleCommand.ToString());

            // Set up command links referring to command
            CommandBinding[] sampleCommandBinding = new CommandBinding[] { 
                new CommandBinding(sampleCommand),
            };
            CoreLogger.LogStatus("Command links constructed: " + sampleCommandBinding.Length);

            // Set up non-conflicting key bindings for each command
            KeyGesture sampleKeyGesture = new KeyGesture(Key.F16, ModifierKeys.None);
            CoreLogger.LogStatus("Command link KeyBinding constructed: KeyBinding=" + sampleKeyGesture.ToString());

            // Attach events to links, then links to the element
            foreach (CommandBinding binding in sampleCommandBinding)
            {
                binding.Executed += new ExecutedRoutedEventHandler(OnSample);
                binding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);
                _rootElement.CommandBindings.Add(binding);
            }
            KeyBinding sampleKeyBinding = new KeyBinding(sampleCommand, sampleKeyGesture);
            _rootElement.InputBindings.Add(sampleKeyBinding);

            // Put the test element on the screen
            Visual v = _rootElement;
            source.RootVisual = v;

            // Save Win32 window handle for later
            _hwnd = new HandleRef(source, source.Handle);

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
            // Two key sequences to invoke 2 commands.
            
            InputCallback[] ops = new InputCallback[]
            {
                delegate
                {
                    DoBeforeExecute();
                },
                delegate
                {
                    KeyboardHelper.TypeKey(Key.F16);
                }

            };
            return ops;
        }    

        /// <summary>
        /// Execute stuff right before the test operations.
        /// </summary>
        private void DoBeforeExecute()
        {
            CoreLogger.LogStatus("Setting focus to the element....");
            bool bFocus = _rootElement.Focus();
            CoreLogger.LogStatus("Focus set via API?           " + (bFocus));
            CoreLogger.LogStatus("Focus set via InputManager?  " + (InputHelper.GetFocusedElement()!=null));

            // Now our TestOps will fire....
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we are looking for a command invocation (Invoke event)
            // for each key sequence.
            // We also expect the corresponding KeyDown for the key sequence.

            int expectedCmd = 1;
            int actualCmd = _commandLog.Count;
            int expectedEvent = 1;
            int actualEvent = _eventLog.Count;

            CoreLogger.LogStatus("Command Events found: " + actualCmd + ", expected=" + expectedCmd);
            CoreLogger.LogStatus("Key Events found:     " + actualEvent + ", expected=" + expectedEvent);

            // expect non-negative event count
            bool actual = (actualCmd == expectedCmd) && (actualEvent == expectedEvent);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Store record of our fired command events.
        /// </summary>
        private ArrayList _commandLog = new ArrayList();

        /// <summary>
        /// Store record of our fired key events.
        /// </summary>
        private ArrayList _eventLog = new ArrayList();

        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="args">Arguments pertaining to the command event.</param>
        private void OnSample(object target, ExecutedRoutedEventArgs args)
        {
            // We are executing a command!
            _commandLog.Add(args);

            CoreLogger.LogStatus(" Command:            " + args.Command.ToString());
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }
            CoreLogger.LogStatus(" Events found: " + _commandLog.Count);


        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="args">Arguments pertaining to the command event.</param>
        private void OnQuery(object target, CanExecuteRoutedEventArgs args)
        {
            // if we are in this handler, the case passes!
            CoreLogger.LogStatus(" Command:            " + args.Command.ToString());
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }

            // Show we are handled and we wish to accept commands!

        }

        /// <summary>
        /// Standard key event handler.
        /// </summary>
        /// <param name="sender">Source sending the event.</param>
        /// <param name="args">Event-specific arguments.</param>
        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            CoreLogger.LogStatus(" Adding event: " + args.ToString());

            // Set test flag
            _eventLog.Add(args);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + args.RoutedEvent.Name + "]");
            CoreLogger.LogStatus("   Hello from: " + args.Key.ToString() + ", " + args.KeyStates.ToString());

            // Don't route this event any more.
            args.Handled = true;
        }
    }
}
