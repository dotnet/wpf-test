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
using System.Windows.Markup;
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
    /// Verify ContentElement RaiseCommand works for element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("0")]
    [TestCaseArea(@"Commanding\CommandBindings")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]    
    public class ContentElementRaiseCommandApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new ContentElementRaiseCommandApp();
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
            CoreLogger.LogStatus("Constructing tree....");

            // Construct test element
            InstrContentPanelHost host = new InstrContentPanelHost();
            _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);
            host.AddChild(_contentElement);
            _rootElement = host;

            // Set up commands, Bindings
            RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command=" + sampleCommand.ToString());
            _sampleCommandBinding = new CommandBinding(sampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + _sampleCommandBinding.ToString() + " (" + _sampleCommandBinding.GetHashCode() + ")");

            // Attach events to Bindings
            _sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            _sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            // Add Bindings to test element
            _contentElement.CommandBindings.Add(_sampleCommandBinding);

            // Put the test element on the screen
            DisplayMe(host, 10, 10, 200, 200);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Executing command....");
            ((RoutedCommand)(_sampleCommandBinding.Command)).Execute(null, _contentElement);
            _bCommandRaised = true;

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

            // For this test we need return value for API call to be true.
            // We also need the Invoke event.

            CoreLogger.LogStatus("Command raised via API? (expect yes) " + (_bCommandRaised));
            CoreLogger.LogStatus("Command Events found: (expect 1) " + _commandLog.Count);
            CoreLogger.LogStatus("Query status events found: (expect 1) " + _commandLog.Count);
            bool bStatusRaised = (_status == _sampleCommandBinding.Command);
            CoreLogger.LogStatus("Status raised? (expect yes) " + bStatusRaised);

            // expect non-negative event count
            bool actual = _bCommandRaised && (_commandLog.Count == 1) && (_queryLog.Count == 1) && bStatusRaised;
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

            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]");

            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            CoreLogger.LogStatus("  Status:            " + e.CanExecute.ToString());
            _status = e.Command;

            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }

            // Show we are handled and we wish to accept commands!
            e.CanExecute = true;
        }

        /// <summary>
        /// Stores result of query status.
        /// </summary>
        private ICommand _status = null;

        /// <summary>
        /// Stores result of API call.
        /// </summary>
        private bool _bCommandRaised = false;

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<ExecutedRoutedEventArgs> _commandLog = new List<ExecutedRoutedEventArgs>();

        /// <summary>
        /// Store record of our fired queries.
        /// </summary>
        private List<CanExecuteRoutedEventArgs> _queryLog = new List<CanExecuteRoutedEventArgs>();

        private ContentElement _contentElement;

        private CommandBinding _sampleCommandBinding = null;
    }
}
