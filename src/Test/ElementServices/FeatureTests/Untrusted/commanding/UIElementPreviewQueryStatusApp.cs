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
    /// Verify UIElement PreviewCanExecute event is raised to cancel CanExecute event for element.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("1")]
    [TestCaseTitle("Verify UIElement PreviewCanExecute event is raised to cancel CanExecute event for element.")]
    [TestCaseArea(@"Commanding\CommandBindings")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class UIElementPreviewCanExecuteApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new UIElementPreviewCanExecuteApp();
            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

            app.VerboseTrace = false;

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus("App run!");
        }

        CommandBinding _sampleCommandBinding = null;

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing tree....");


            // Construct test element, add event handling
            InstrPanel panel = new InstrPanel();

            // Set up commands, Bindings
            RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command=" + sampleCommand.ToString());

            _sampleCommandBinding = new CommandBinding(sampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + _sampleCommandBinding.ToString());

            // Attach events to Bindings
            _sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            _sampleCommandBinding.PreviewCanExecute += new CanExecuteRoutedEventHandler(OnPreviewQuery);
            _sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            // Add Bindings to test element
            panel.CommandBindings.Add(_sampleCommandBinding);

            DisplayMe(panel, 10, 10, 200, 200);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Executing query command....");

            RoutedCommand cmd = (RoutedCommand)_sampleCommandBinding.Command;
            _bQueryStatusRaised = cmd.CanExecute(null, _rootElement);

            base.DoExecute(arg);

            return null;
        }

        /// <summary>
        /// Stores result of API call.
        /// </summary>
        private bool _bQueryStatusRaised = false;

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we need return value for API call to be true.
            // We also need the Invoke event.

            CoreLogger.LogStatus("Command raised via API?   (expect false)        " + (_bQueryStatusRaised));
            CoreLogger.LogStatus("Events found: (expect 1) " + _queryLog.Count);

            // expect non-negative event count
            bool actual = !_bQueryStatusRaised && (_queryLog.Count == 1);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<CanExecuteRoutedEventArgs> _queryLog = new List<CanExecuteRoutedEventArgs>();

        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSample(object target, ExecutedRoutedEventArgs e)
        {
            CoreLogger.LogStatus("In command event:");
            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }
        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnPreviewQuery(object target, CanExecuteRoutedEventArgs e)
        {
            // We are executing a command!
            _queryLog.Add(e);

            // if we are in this handler, the case passes!
            CoreLogger.LogStatus("In preview query event:");
            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }

            e.Handled = true;
        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnQuery(object target, CanExecuteRoutedEventArgs e)
        {
            // We are executing a command!
            _queryLog.Add(e);

            // if we are in this handler, the case passes!
            CoreLogger.LogStatus("In query event:");
            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }

        }
    }
}
