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
    /// Verify CommandManager RegisterClassCommandBindings for ContentElement in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandManager RegisterClassCommandBindings for ContentElement in window.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\CommandBindings")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandManagerRegisterClassCommandBindingsContentElementApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");

            TestApp app = new CommandManagerRegisterClassCommandBindingsContentElementApp();

            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

            app.VerboseTrace = true;

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

            InstrContentPanelHost host = new InstrContentPanelHost();
            _contentElement = new InstrContentPanel("rootLeaf", "Sample", host);
            host.AddChild(_contentElement);

            // Set up commands, Bindings
            s_sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command=" + s_sampleCommand.ToString());

            CommandBinding sampleCommandBinding = new CommandBinding(s_sampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + sampleCommandBinding.ToString());

            // Attach events to Bindings
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            // Set up class binding
            CommandManager.RegisterClassCommandBinding(typeof(InstrContentPanel), sampleCommandBinding);
            CoreLogger.LogStatus("Command Binding registered with element class!");

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
            KeyboardHelper.EnsureFocus(_contentElement);

            CoreLogger.LogStatus("Executing command for handlers to pick up...");
            s_sampleCommand.Execute(null, null);

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

            // For this test we are just looking for a command invocation (Execute event).
            // We also want to make sure the event sender was the content element.

            CoreLogger.LogStatus("Events found: (expect 1) " + s_commandLog.Count);
            if (s_commandLog.Count > 0)
            {
                CoreLogger.LogStatus(" command sender: " + ((ExecutedRoutedEventArgs)s_commandLog[0]).Source.GetType().ToString());
            }

            bool actual = (s_commandLog.Count == 1) && (((ExecutedRoutedEventArgs)s_commandLog[0]).Source.GetType() == typeof(InstrContentPanel));
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
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private static void OnSample(object sender, ExecutedRoutedEventArgs e)
        {
            // We are executing a command! Save who sent it.
            s_commandLog.Add(e);

            CoreLogger.LogStatus("In command event:");
            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            if (sender != null)
            {
                CoreLogger.LogStatus(" command sender Name: " + sender.ToString());
            }

            CoreLogger.LogStatus(" Events found: " + s_commandLog.Count);


        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private static void OnQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            CoreLogger.LogStatus("In query event:");
            CoreLogger.LogStatus(" Command:            " + e.Command.ToString());
            if (sender != null)
            {
                CoreLogger.LogStatus(" command sender Name: " + sender.ToString());
            }

            // Show we are handled and we wish to accept commands!
            e.CanExecute = true;

        }

        private static RoutedCommand s_sampleCommand;

        private ContentElement _contentElement;

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private static List<ExecutedRoutedEventArgs> s_commandLog = new List<ExecutedRoutedEventArgs>();
    }
}
