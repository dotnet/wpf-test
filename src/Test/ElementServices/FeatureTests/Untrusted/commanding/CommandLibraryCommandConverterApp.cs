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
using Avalon.Test.CoreUI.Trusted;

using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify commands in command library are converted to string correctly.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("2")]
    [TestCaseTitle(@"Verify commands in command library are converted to string correctly.")]
    [TestCaseArea(@"Commanding\Library")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class CommandLibraryCommandConverterConvertToApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandLibraryCommandConverterConvertToApp();
            app.CreateLog = false;
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

            // For this test we are just looking for RoutedCommand library default conversions to be within tolerance.

            StringBuilder sb = new StringBuilder(1024);
            sb.Append("Name: ");

            bool foundFailure = false;
            int i;
            List<RoutedCommand> commandsWithIncorrectStrings = new List<RoutedCommand>();
            for (i = 0; i < _stdCommands.Length; i++)
            {                
                //This is a workaround to match the number of variations between old and new infra
                //Each validation is created as a seperate variation and result logged. 
                CoreLogger.BeginVariation(_stdCommands[i].Name);            
                CoreLogger.LogStatus("Variation: " + i.ToString());

                if (!TestCommandLibrary.HasExpectedString(_stdCommands[i]))
                {             
                    foundFailure = true;
                    commandsWithIncorrectStrings.Add(_stdCommands[i]);
                }
                else
                {
                    // Success!                 
                    sb.Append(_stdCommands[i].Name + ",");
                }
                CoreLogger.LogTestResult(!foundFailure, ("Result of variation"+_stdCommands[i].Name));
                CoreLogger.EndVariation();
            }            
            
            sb.Append("\nCount of incorrect commands: (expect 0) " + commandsWithIncorrectStrings.Count.ToString() + "\n");

            if (commandsWithIncorrectStrings.Count > 0)
            {
                foreach (RoutedCommand cmd in commandsWithIncorrectStrings)
                {
                    CoreLogger.LogStatus("Error verifying name '" + cmd.Name + "' (expected '" + TestCommandLibrary.GetExpectedStringValue(cmd) + ")'\n");
                    CoreLogger.LogStatus("\tDeclaringType: " + cmd.OwnerType.ToString() + "\n");
                }
            }

            CoreLogger.LogStatus(sb.ToString());

            float failureTolerance = 0.1f;
            float failureRate = (float)(commandsWithIncorrectStrings.Count) / _stdCommands.Length;
            CoreLogger.LogStatus("Failure rate: (expect under "+failureTolerance+"): "+failureRate);

            // expect to have seen every command OK.
            bool actual = (i == _stdCommands.Length) && (failureRate <= failureTolerance);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");


            if (foundFailure)
            {
                throw new TestValidationException("Command validation failed.  See incorrect commands above.");
            }

            return null;
        }

        /// <summary>
        /// List of testable standard commands.
        /// </summary>
        private RoutedCommand[] _stdCommands;
    }
}
