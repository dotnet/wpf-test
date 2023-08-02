// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections.Generic;
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
    /// Verify UIElement RoutedCommand Enabled works for element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify UIElement RoutedCommand Enabled works for element in window.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\CommandBindings")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]    
    public class CommandBindingEnabledApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingEnabledApp();
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
            CoreLogger.LogStatus("Constructing tree....");

            // Construct test element
            InstrPanel panel = new InstrPanel();

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
            panel.CommandBindings.Add(sampleCommandBinding);
            _removableInputBinding = new MouseBinding(sampleCommand, new MouseGesture(MouseAction.MiddleClick));
            panel.InputBindings.Add(_removableInputBinding);

            // Put the test element on the screen
            DisplayMe(panel, 10, 10, 200, 200);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            KeyboardHelper.EnsureFocus(_rootElement);

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

            CoreLogger.LogStatus("Execute Events found: (expect 0) " + _commandLog.Count);
            int queryLogCountBefore = _queryLog.Count;
            CoreLogger.LogStatus("Query-Enabled Events found before: (expect 1) " + queryLogCountBefore);

            bool bIsCommandEnabled = false;
            if (_rootElement.CommandBindings.Count > 0)
            {
                CommandBinding sampleCommandBinding = _rootElement.CommandBindings[0];
                bIsCommandEnabled = sampleCommandBinding.Command.CanExecute(null);
                CoreLogger.LogStatus("Command enabled? " + bIsCommandEnabled + " (should be false)");
            }

            int queryLogCountAfter = _queryLog.Count;
            CoreLogger.LogStatus("Query-Enabled Events found after: (expect 2) " + queryLogCountAfter);

            // expect non-negative event count
            bool actual = (_commandLog.Count == 0) && (queryLogCountBefore == 1) && (queryLogCountAfter == 2) && (!_bWasCommandEnabled) && (!bIsCommandEnabled);
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
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSample(object target, ExecutedRoutedEventArgs e)
        {
            // We are executing a command!
            _commandLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
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
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnQuery(object target, CanExecuteRoutedEventArgs e)
        {
            // We are querying a command!
            _queryLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }

            // Show we are not handled and we do not wish to accept commands!
            e.CanExecute = false;
        }

        /// <summary>
        /// Store enabled-ness of command.
        /// </summary>
        private bool _bWasCommandEnabled = false;

        /// <summary>
        /// Store record of our fired command execute events.
        /// </summary>
        private List<ExecutedRoutedEventArgs> _commandLog = new List<ExecutedRoutedEventArgs>();

        /// <summary>
        /// Store record of our fired command query-enabled events.
        /// </summary>
        private List<CanExecuteRoutedEventArgs> _queryLog = new List<CanExecuteRoutedEventArgs>();

        /// <summary>
        /// Store record of input binding to be removed.
        /// </summary>
        private InputBinding _removableInputBinding;
    }
}
