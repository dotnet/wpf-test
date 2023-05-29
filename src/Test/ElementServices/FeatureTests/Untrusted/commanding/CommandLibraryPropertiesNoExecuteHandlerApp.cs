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
using System.Windows.Documents;

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
    /// Verify properties of commands in command library are set correctly with no execute handler attached to bindings.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class CommandLibraryPropertiesNoExecuteHandlerApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\Library", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Compile and Verify properties of commands in command library are set correctly with no execute handler attached to bindings in HwndSource.")]
        [TestCase("2", @"Commanding\Library", "Window", TestCaseSecurityLevel.FullTrust, @"Compile and Verify properties of commands in command library are set correctly with no execute handler attached to bindings in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "CommandLibraryPropertiesNoExecuteHandlerApp",
                "Run",
                hostType);

        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\Library", "HwndSource", TestCaseSecurityLevel.FullTrust, @"Verify properties of commands in command library are set correctly with no execute handler attached to bindings in HwndSource.")]
        [TestCase("1", @"Commanding\Library", "Window", TestCaseSecurityLevel.FullTrust, @"Verify properties of commands in command library are set correctly with no execute handler attached to bindings in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new CommandLibraryPropertiesNoExecuteHandlerApp(), "Run");
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

            // Create a test element for targeting
            InstrPanel p = new InstrPanel();
            cvs.Children.Add(p);

            // Get prepared to mark all these commands as disabled upon a query
            // (Execute handler must be attached to bindings to enable the commands)

            foreach (RoutedCommand cmd in this.CommandsToTest)
            {
                CommandBinding binding = new CommandBinding(cmd);
                binding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

                //binding.Execute += new ExecutedRoutedEventHandler(OnSample).

                p.CommandBindings.Add(binding);
            }

            // Put the test element on the screen
            CoreLogger.LogStatus("Showing window...");
            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }


        /// <summary>
        /// Commands to be tested for various properties.
        /// </summary>
        /// <remarks>
        /// Override this property to test subset of commands.
        /// </remarks>
        protected virtual RoutedCommand[] CommandsToTest
        {
            get
            {
                // Just test everything.
                return TestCommandLibrary.AllCommands;
            }
        }

        /// <summary>
        /// Is this command a valid library command?
        /// </summary>
        /// <param name="cmd">RoutedCommand</param>
        /// <returns>true if it's valid, false otherwise.</returns>
        protected virtual bool IsValidLibraryCommand(RoutedCommand cmd)
        {
            return (cmd.Name != null) && (cmd.Name != "") &&
                    (
                        (cmd.OwnerType == typeof(ApplicationCommands)) ||
                        (cmd.OwnerType == typeof(ComponentCommands)) ||
                        (cmd.OwnerType == typeof(EditingCommands)) ||
                        (cmd.OwnerType == typeof(MediaCommands)) ||
                        (cmd.OwnerType == typeof(NavigationCommands))
                    );

        }

        /// <summary>
        /// Is this command enabled for our test?
        /// </summary>
        /// <param name="cmd">RoutedCommand</param>
        /// <returns>true if it's enabled, false otherwise.</returns>
        protected bool IsLibraryCommandEnabled(RoutedCommand cmd)
        {
            return cmd.CanExecute(null, null);
        }

        /// <summary>
        /// Are the command properties correct for when current element has focus?
        /// </summary>
        /// <param name="cmd">RoutedCommand</param>
        /// <returns>true if correct, false otherwise.</returns>
        protected virtual bool IsValidPropertiesFocused(RoutedCommand cmd)
        {
            return (IsValidLibraryCommand(cmd) && !IsLibraryCommandEnabled(cmd));
        }

        /// <summary>
        /// Are the command properties correct for when current element does not have focus?
        /// </summary>
        /// <param name="cmd">RoutedCommand</param>
        /// <returns>true if correct, false otherwise.</returns>
        protected virtual bool IsValidPropertiesNotFocused(RoutedCommand cmd)
        {
            return (IsValidLibraryCommand(cmd) && !IsLibraryCommandEnabled(cmd));
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we are just looking for RoutedCommand library properties to be correct:
            // IsEnabled, DeclaringType, Name
            // IsEnabled is false if the commandable element does not have an Execute binding.

            StringBuilder sb = new StringBuilder(1024);
            sb.Append("Name: ");

            Canvas cvs = (Canvas)_rootElement;
            foreach (RoutedCommand cmd in this.CommandsToTest)
            {
                IInputElement iie = Keyboard.Focus(cvs.Children[0]);
                CoreLogger.LogStatus("Currently focused element? (expect child) " + iie);

                if (!IsValidPropertiesFocused(cmd))
                {
                    // We have a problem
                    sb.Append("\nError verifying name '" + cmd.Name + "'\n");
                    sb.Append("\tCanExecute? '" + IsValidPropertiesFocused(cmd) + "' (should be true)\n");
                    sb.Append("\tDeclaringType: " + cmd.OwnerType.ToString() + "\n");
                    _problemStdCommands.Add(cmd);
                    continue;
                }
                else
                {
                    // Disable us
                    iie = Keyboard.Focus(cvs);
                    CoreLogger.LogStatus("Currently focused element? (expect parent) " + iie);

                    if (!IsValidPropertiesNotFocused(cmd))
                    {
                        // We still have a problem
                        sb.Append("\nError verifying name '" + cmd.Name + "'\n");
                        sb.Append("\tCanExecute? '" + IsValidPropertiesNotFocused(cmd) + "' (should be true\n");
                        _problemStdCommands.Add(cmd);
                        continue;
                    }
                    else
                    {
                        // Success!
                        sb.Append(cmd.Name + ",");
                    }
                }
            }

            CoreLogger.LogStatus(sb.ToString());

            // expect to have seen every command OK.
            CoreLogger.LogStatus("Commands in error: (expect 0) " + _problemStdCommands.Count);
            bool actual = (_problemStdCommands.Count == 0);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
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
            // Log some debugging data
            CoreLogger.LogStatus(" [" + e.RoutedEvent.Name + "]  [" + sender + "]");

            RoutedCommand cmd = e.Command as RoutedCommand;
            if (cmd != null)
            {
                CoreLogger.LogStatus("  command name:        " + cmd.Name);
            }

            // Show we are handled and we do not wish to accept commands!
            e.CanExecute = false;
        }

        /// <summary>
        /// List of problem standard commands.
        /// </summary>
        private List<RoutedCommand> _problemStdCommands = new List<RoutedCommand>();

    }
}
