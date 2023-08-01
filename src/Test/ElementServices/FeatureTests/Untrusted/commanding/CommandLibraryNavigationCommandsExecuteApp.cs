// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Documents;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify navigation commands in command library can be executed from default input gestures.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CommandLibraryNavigationCommandsExecuteApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\Library", "HwndSource", "1", TestCaseSecurityLevel.FullTrust, @"Compile and Verify navigation commands in command library can be executed from default input gestures in HwndSource.")]
        [TestCase("2", @"Commanding\Library", "Window", "1", TestCaseSecurityLevel.FullTrust, @"Compile and Verify navigation commands in command library can be executed from default input gestures in window.")]
        [TestCase("2", @"Commanding\Library", "NavigationWindow", "1", TestCaseSecurityLevel.FullTrust, @"Compile and Verify navigation commands in command library can be executed from default input gestures in NavigationWindow.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "CommandLibraryNavigationCommandsExecuteApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\Library", "HwndSource", "1", TestCaseSecurityLevel.FullTrust, @"Verify navigation commands in command library can be executed from default input gestures in HwndSource.")]
        [TestCase("1", @"Commanding\Library", "Window", "1", TestCaseSecurityLevel.FullTrust, @"Verify navigation commands in command library can be executed from default input gestures in window.")]
        [TestCase("1", @"Commanding\Library", "NavigationWindow", "1", TestCaseSecurityLevel.FullTrust, @"Verify navigation commands in command library can be executed from default input gestures in NavigationWindow.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CommandLibraryNavigationCommandsExecuteApp(), "Run");
        }


        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            CoreLogger.LogStatus("Constructing window....");
            Canvas cvs = new Canvas();
            cvs.Focusable = true;

            // Prepare list of testable commands.
            _stdCommands = TestCommandLibrary.AllCommands;

            foreach (RoutedCommand cmd in _stdCommands)
            {
                // Prepare table of executed commands (including filtering desired commands)
                if (cmd.OwnerType == typeof(NavigationCommands))
                {
                    s_commandExecutionDictionary.Add(cmd, 0);
                }
            }

            CoreLogger.LogStatus("Showing window...");
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
            Canvas cvs = (Canvas)_rootElement;

            foreach (RoutedCommand cmd in _stdCommands)
            {
                // Filter desired commands
                if (cmd.OwnerType != typeof(NavigationCommands))
                {
                    continue;
                }

                // Create a test element for targeting
                InstrPanel p = new InstrPanel();
                cvs.Children.Add(p);

                // Attach event handlers
                CommandBinding binding = new CommandBinding(cmd);
                binding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);
                binding.Executed += new ExecutedRoutedEventHandler(OnSample);
                p.CommandBindings.Add(binding);

                // Invoke all default commands bound to the element
                InputGestureCollection defaultGestures = TestCommandLibrary.GetExpectedGestures(cmd);
                if (defaultGestures != null)
                {
                    foreach (InputGesture g in defaultGestures)
                    {
                        KeyGesture expectedKeyGesture = g as KeyGesture;
                        if ((expectedKeyGesture == null))
                        {
                            // Not a key gesture, try the next gesture in the command.
                            continue;
                        }

                        if ((expectedKeyGesture.Key != Key.None) && !(expectedKeyGesture.Key==Key.Escape && expectedKeyGesture.Modifiers==ModifierKeys.Alt))
                        {
                            // Valid key gesture, let's send it to the element
                            CoreLogger.LogStatus(" Focusing element...");
                            p.Focus();
                            DispatcherHelper.DoEventsPastInput();
                            CoreLogger.LogStatus("  Item in focus: " + InputHelper.GetFocusedElement());

                            CoreLogger.LogStatus(" Pressing key..." + expectedKeyGesture.Modifiers + "+" + expectedKeyGesture.Key +
                                " to execute '" + cmd.Name + "'");
                            KeyboardHelper.TypeKey(expectedKeyGesture.Key, expectedKeyGesture.Modifiers);
                            DispatcherHelper.DoEventsPastInput();
                            CoreLogger.LogStatus(" ...Key pressed!");
                        }
                        else
                        {
                            // Null key gesture, nothing to do but pretend we sent it.
                            s_commandExecutionDictionary[cmd]++;
                        }
                    }
                }

                // Remove test element for targeting
                cvs.Children.Remove(p);
            }

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

            //// For this test we just want to see, for each command, 
            //// the count of executed commands equaling the count of default input gestures.

            bool bErrorsFound = false;
            foreach (KeyValuePair<RoutedCommand, int> kvp in s_commandExecutionDictionary)
            {
                int actualGestureCount = kvp.Value;
                InputGestureCollection expectedGestures = TestCommandLibrary.GetExpectedGestures(kvp.Key);
                int expectedGestureCount = (expectedGestures == null ? 0 : expectedGestures.Count);
                bool countsMatch = (actualGestureCount == expectedGestureCount);
                if (!countsMatch)
                {
                    CoreLogger.LogStatus("Error verifying name '" + kvp.Key.Name + "'");
                    CoreLogger.LogStatus("\tExpected = " + expectedGestureCount + ", Actual = " + actualGestureCount);
                    bErrorsFound = true;
                }

            }

            this.TestPassed = (!bErrorsFound);

            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, a command has been invoked.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSample(object sender, ExecutedRoutedEventArgs e)
        {
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");

            RoutedCommand cmd = e.Command as RoutedCommand;
            if (cmd != null)
            {
                // Log some execution data (increase our execution count for this command)
                s_commandExecutionDictionary[cmd]++;

                CoreLogger.LogStatus("  command name:        " + cmd.Name +
                    "   (" + s_commandExecutionDictionary[cmd] + ")");

            }

        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <remarks>
        /// Strategy: For our desired command, enable the appropriate Binding on the queried element.
        /// </remarks>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnQuery(object sender, CanExecuteRoutedEventArgs e)
        {
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");

            // Show we are handled and we wish to accept commands!
            e.CanExecute = true;
        }

        /// <summary>
        /// Log of command execution.
        /// </summary>
        /// <remarks>
        /// With this dictionary, we track how often each command has been executed.
        /// Key = the command, Value = count of how many times this command was executed.
        /// </remarks>
        private static Dictionary<RoutedCommand, int> s_commandExecutionDictionary =
            new Dictionary<RoutedCommand, int>();

        /// <summary>
        /// List of testable standard commands.
        /// </summary>
        private RoutedCommand[] _stdCommands;

    }
}
