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
    /// Verify UIElement CommandBinding bindings can be retrieved from defaults collection.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify UIElement CommandBinding bindings can be retrieved from defaults collection.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\CommandBindings")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class CommandBindingBindingsFromDefaultsApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingBindingsFromDefaultsApp();
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
            // Construct test element
            _rootElement = new InstrPanel();

            // Set up command with some gestures as a default
            InputGestureCollection defaultInputGestures = new InputGestureCollection(new InputGesture[] {
                new KeyGesture(Key.A, ModifierKeys.Windows), 
                new MouseGesture(MouseAction.WheelClick) 
            });
            _sampleCommand = new RoutedCommand("Sample", this.GetType(), defaultInputGestures);
            CoreLogger.LogStatus("Command constructed: Command=" + _sampleCommand.ToString());

            // Create a normal command Binding using this command.
            CommandBinding sampleCommandBinding = new CommandBinding(_sampleCommand, OnSample);

            // Add same binding to test element as class binding
            CommandManager.RegisterClassCommandBinding(_rootElement.GetType(), sampleCommandBinding);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Executing command for handlers to pick up...");
            _sampleCommand.Execute(null, _rootElement);

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

            // For this test we are looking if our command is set up to use defaults.
            // If so, we want to make sure that if our command Binding has no local bindings,
            // we use the non-local (Command.Defaults) binding

            CoreLogger.LogStatus("Local bindings found: (expect 0) " + _rootElement.CommandBindings.Count);
            CoreLogger.LogStatus("Bindings found: (expect 1) " + _commandLog.Count);

            bool result = ((_rootElement.CommandBindings.Count == 0) && (_commandLog.Count == 1));

            CoreLogger.LogStatus("Setting log result to " + result);
            this.TestPassed = result;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSample(object sender, ExecutedRoutedEventArgs e)
        {
            // We are executing a command!
            _commandLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            if (sender != null)
            {
                CoreLogger.LogStatus("  command sender Name: " + sender.ToString());
            }
            RoutedCommand cmd = e.Command as RoutedCommand;
            if (cmd != null)
            {
                CoreLogger.LogStatus("  command name:        " + cmd.Name);
            }

        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<ExecutedRoutedEventArgs> _commandLog = new List<ExecutedRoutedEventArgs>();

        /// <summary>
        /// Stores sample command object.
        /// </summary>
        private RoutedCommand _sampleCommand;
    }
}
