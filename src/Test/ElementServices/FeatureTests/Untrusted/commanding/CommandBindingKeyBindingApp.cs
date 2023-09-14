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
    /// Verify UIElement CommandBinding KeyBinding works for element.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CommandBindingKeyBindingApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\InputBindings\KeyBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify UIElement CommandBinding KeyBinding works for element in HwndSource.")]
        [TestCase("3", @"Commanding\InputBindings\KeyBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify UIElement CommandBinding KeyBinding works for element in window.")]
        [TestCase("1", @"Commanding\InputBindings\KeyBindings", "Browser", TestCaseSecurityLevel.PartialTrust, @"Compile and Verify UIElement CommandBinding KeyBinding works for element in Browser.")]
        [TestCase("3", @"Commanding\InputBindings\KeyBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Compile and Verify UIElement CommandBinding KeyBinding works for element in NavigationWindow.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "CommandBindingKeyBindingApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\InputBindings\KeyBindings", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify UIElement CommandBinding KeyBinding works for element in HwndSource.")]
        [TestCase("2", @"Commanding\InputBindings\KeyBindings", "Window", TestCaseSecurityLevel.FullTrust, @"Verify UIElement CommandBinding KeyBinding works for element in window.")]
        [TestCase("2", @"Commanding\InputBindings\KeyBindings", "NavigationWindow", TestCaseSecurityLevel.FullTrust, @"Verify UIElement CommandBinding KeyBinding works for element in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CommandBindingKeyBindingApp(), "Run");

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
            InstrPanel panel = new InstrPanel();

            // Set up commands, links
            RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command=" + sampleCommand.ToString());
            CommandBinding sampleCommandBinding = new CommandBinding(sampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + sampleCommandBinding.ToString());

            // Set up key bindings
            KeyBinding sampleKeyBinding = new KeyBinding(sampleCommand, Key.F2, ModifierKeys.Shift);
            CoreLogger.LogStatus("Command Binding KeyBinding constructed: KeyBinding=" + sampleKeyBinding.ToString());

            // Set up key bindings
            KeyBinding sampleKeyBinding2 = new KeyBinding(sampleCommand, new KeyGesture(Key.F17));
            CoreLogger.LogStatus("Command Binding KeyBinding constructed: KeyBinding=" + sampleKeyBinding2.ToString());

            // Attach events to links
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            // Add links to test element
            panel.CommandBindings.Add(sampleCommandBinding);
            panel.InputBindings.Add(sampleKeyBinding);
            panel.InputBindings.Add(sampleKeyBinding2);

            // Put the test element on the screen
            DisplayMe(panel, 10, 10, 100, 100);

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
                    KeyboardHelper.TypeKey(Key.F2, ModifierKeys.Shift);
                },
                delegate
                {
                    KeyboardHelper.TypeKey(Key.F17);
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

            // For this test we are just looking for a command invocation (Invoke event).

            CoreLogger.LogStatus("Events found: (expect 2) " + _commandLog.Count);

            // expect non-negative event count
            bool actual = (_commandLog.Count == 2);
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

    }
}
