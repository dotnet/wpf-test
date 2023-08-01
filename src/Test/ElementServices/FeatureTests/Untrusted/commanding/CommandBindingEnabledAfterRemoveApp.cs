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
    /// Verify UIElement RoutedCommand Enabled works for element in window after removing input binding.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify UIElement RoutedCommand Enabled works for element in window after removing input binding.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingEnabledAfterRemoveApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingEnabledAfterRemoveApp();
            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

            app.VerboseTrace = false;

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

            // Set up commands, Bindings
            RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: " + sampleCommand.ToString());
            _bWasCommandEnabled = sampleCommand.CanExecute(null, null);
            CoreLogger.LogStatus("Command enabled? " + _bWasCommandEnabled + " (should be false)");

            // Attach events to Bindings
            CommandBinding sampleCommandBinding = new CommandBinding(sampleCommand);
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            // Add Bindings to test element
            _rootElement.CommandBindings.Add(sampleCommandBinding);
            _removableInputBinding = new MouseBinding(sampleCommand, new MouseGesture(MouseAction.MiddleClick));
            _rootElement.InputBindings.Add(_removableInputBinding);

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
            bool bFocus = _rootElement.Focus();
            CoreLogger.LogStatus("Focus set via API?           " + (bFocus));

            CoreLogger.LogStatus("Turning off input Binding...");
            _rootElement.InputBindings.Remove(_removableInputBinding);

            InputCallback[] ops = new InputCallback[]
            {
                delegate
                {
                    MouseHelper.Click(MouseButton.Middle, _rootElement);
                }                
            };
            return ops;
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

            // For this test we are looking for 0 command invocations (no Execute events),
            // We are also looking for 1 additional command query-enabled event right after reading Command.IsEnabled property,
            // We are also looking for the command to stay disabled after query.

            CoreLogger.LogStatus("Execute Events found: " + _commandLog.Count);
            int queryLogCountBefore = _queryLog.Count;
            CoreLogger.LogStatus("Query-Enabled Events found before: " + queryLogCountBefore);

            IList list = (IList)(_rootElement.CommandBindings);
            CommandBinding sampleCommandBinding = list[0] as CommandBinding;
            bool bIsCommandEnabled = sampleCommandBinding.Command.CanExecute(null);
            CoreLogger.LogStatus("Command enabled? " + bIsCommandEnabled + " (should be false)");

            int queryLogCountAfter = _queryLog.Count;
            CoreLogger.LogStatus("Query-Enabled Events found after: " + queryLogCountAfter);

            // expect non-negative event count
            bool actual = (_commandLog.Count == 0) && (queryLogCountBefore == 0) && (queryLogCountAfter == 1) && (!_bWasCommandEnabled) && (!bIsCommandEnabled);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="args">Arguments pertaining to the command event.</param>
        private void OnSample(object target, ExecutedRoutedEventArgs args)
        {
            // We are executing a command!
            _commandLog.Add(args);

            CoreLogger.LogStatus("In command event:");
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
            // We are querying a command!
            _queryLog.Add(args);

            // if we are in this handler, the case passes!
            CoreLogger.LogStatus("In query event:");
            CoreLogger.LogStatus(" Command:            " + args.Command.ToString());
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }

            // Show we are not handled and we do not wish to accept commands!
            args.CanExecute = false;
        }

        /// <summary>
        /// Store enabled-ness of command.
        /// </summary>
        private bool _bWasCommandEnabled = false;

        /// <summary>
        /// Store record of our fired command execute events.
        /// </summary>
        private ArrayList _commandLog = new ArrayList();

        /// <summary>
        /// Store record of our fired command query-enabled events.
        /// </summary>
        private ArrayList _queryLog = new ArrayList();

        /// <summary>
        /// Store record of input binding to be removed.
        /// </summary>
        private InputBinding _removableInputBinding;
    }
}
