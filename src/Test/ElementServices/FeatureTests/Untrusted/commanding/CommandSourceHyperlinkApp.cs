// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify ICommandSource properties work for Hyperlink.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CommandSourceHyperlinkApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\InputBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify ICommandSource properties work for Hyperlink in HwndSource.")]
        [TestCase("2", @"Commanding\InputBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify ICommandSource properties work for Hyperlink in window.")]
        [TestCase("1", @"Commanding\InputBindings", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify ICommandSource properties work for Hyperlink in Browser.")]
        [TestCase("2", @"Commanding\InputBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify ICommandSource properties work for Hyperlink in NavigationWindow.")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "CommandSourceHyperlinkApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\InputBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify ICommandSource properties work for Hyperlink in HwndSource.")]
        [TestCase("1", @"Commanding\InputBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Verify ICommandSource properties work for Hyperlink in window.")]
        [TestCase("1", @"Commanding\InputBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify ICommandSource properties work for Hyperlink in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CommandSourceHyperlinkApp(), "Run");

        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");

            // Construct test element
            _link = new Hyperlink();

            TextBlock tb = new TextBlock(_link);

            Canvas cvs = new Canvas();
            cvs.Children.Add(tb);

            // Set up commands, links
            RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command=" + sampleCommand.ToString());
            CommandBinding sampleCommandBinding = new CommandBinding(sampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + sampleCommandBinding.ToString());

            // Attach events to links
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            // Add links to test element
            _link.CommandBindings.Add(sampleCommandBinding);
            _link.Command = sampleCommand;
            _link.CommandParameter = _testObject;
            _link.Inlines.Clear();
            _link.Inlines.Add(new Run("Link"));

            // Put the test element on the screen
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Executing...");

            _link.DoClick();

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

            // For this test we are just looking for a command invocation (Invoke event) and a correct parameter.
            // Correct target is assumed by not focusing element.

            CoreLogger.LogStatus("Events found: (expect 1) " + _commandLog.Count);
            if (_commandLog.Count > 0)
            {
                CoreLogger.LogStatus("  parameter='" + _commandLog[0].Parameter + "' (expect some object) ");
            }

            bool actual = (_commandLog.Count == 1) && (_commandLog[0].Parameter == _testObject) ;
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
        private void OnSample(object sender, ExecutedRoutedEventArgs e)
        {
            // We are executing a command!
            _commandLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");

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
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            // Log some debugging data
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");
            if (sender != null)
            {
                CoreLogger.LogStatus("  command sender Name: " + sender.ToString());
            }

            // Show we are handled and we wish to accept commands!
            e.CanExecute = true;
            e.Handled = true;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<ExecutedRoutedEventArgs> _commandLog = new List<ExecutedRoutedEventArgs>();

        private object _testObject = 0.02f;

        private Hyperlink _link;

    }
}
