// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Documents;
using System.Windows.Markup;
using Avalon.Test.CoreUI.Trusted;

using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify CommandValueSerializer ConvertFrom works correctly for library commands.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <

    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("2")]
    [TestCaseTitle(@"Verify CommandValueSerializer ConvertFrom works correctly for library commands.")]
    [TestCaseArea(@"Commanding\Library")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandLibraryCommandValueSerializerConvertFromApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandLibraryCommandValueSerializerConvertFromApp();
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
            CoreLogger.LogStatus("Constructing list of commands....");

            // Create list of testable commands.
            _stdCommands = TestCommandLibrary.AllCommands;

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

            // For this test we are just looking for RoutedCommand library default conversions to be correct.

            StringBuilder sb = new StringBuilder(1024);
            sb.Append("Name: ");

            int i;
            ValueSerializer vs = ValueSerializer.GetSerializerFor(typeof(RoutedCommand));
            List<RoutedCommand> commandsWithIncorrectCommands = new List<RoutedCommand>();
            for (i = 0; i < _stdCommands.Length; i++)
            {
                string s = TestCommandLibrary.GetExpectedStringValue(_stdCommands[i]);
                // NOTE: Delete and Stop commands are special-cased, pending 
                if (s == "Delete" || s == "Stop")
                {
                    continue;
                }

                RoutedCommand expectedCommand = _stdCommands[i];
                RoutedCommand actualCommand;
                try
                {
                    actualCommand = vs.ConvertFromString(s, null) as RoutedCommand;
                }
                catch (NotSupportedException)
                {
                    actualCommand = null;
                }

                if ((expectedCommand != actualCommand))
                {
                    commandsWithIncorrectCommands.Add(_stdCommands[i]);
                }
                else
                {
                    // Success!
                    sb.Append(_stdCommands[i].Name + ",");
                }
            }

            int nIncorrectCommands = commandsWithIncorrectCommands.Count;
            sb.Append("\nCount of incorrect commands: (expect 0) " + nIncorrectCommands + "\n");

            if (nIncorrectCommands > 0)
            {
                foreach (RoutedCommand cmd in commandsWithIncorrectCommands)
                {
                    CoreLogger.LogStatus("\tDeclaringType: " + cmd.OwnerType.ToString());
                    CoreLogger.LogStatus("Error verifying command '" + cmd.Name + "' (expected '" + TestCommandLibrary.GetExpectedCommandValue(cmd.Name) + "')");
                }
            }

            CoreLogger.LogStatus(sb.ToString());

            float failureTolerance = 0.1f;
            float failureRate = (float)(commandsWithIncorrectCommands.Count) / _stdCommands.Length;
            CoreLogger.LogStatus("Failure rate: (expect under " + failureTolerance + "): " + failureRate);

            // expect to have seen every command OK.
            bool actual = (i == _stdCommands.Length) && (failureRate <= failureTolerance);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// List of testable standard commands.
        /// </summary>
        private RoutedCommand[] _stdCommands;
    }
}
