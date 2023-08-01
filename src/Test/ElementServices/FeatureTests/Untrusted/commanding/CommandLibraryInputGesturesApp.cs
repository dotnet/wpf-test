// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Commanding
{
    /******************************************************************************
    * CLASS:          CommandLibraryInputGesturesApp
    ******************************************************************************/
    /// <summary>
    /// Verify gestures of commands in command library are set correctly on initialization.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [Test(0, "Commanding.Library", TestCaseSecurityLevel.FullTrust, "CommandLibraryInputGesturesApp", Keywords = "MicroSuite")]
    public class CommandLibraryInputGesturesApp : TestApp
    {
        #region Private Data
        private RoutedCommand[] _stdCommands;  // List of testable standard commands.
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          CommandLibraryInputGesturesApp Constructor
        ******************************************************************************/
        public CommandLibraryInputGesturesApp() :base()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            GlobalLog.LogStatus("Running app...");
            this.RunTestApp();
            GlobalLog.LogStatus("App run!");

            //Any test failure will be caught by an Assert during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /******************************************************************************
        * Function:          DoSetup
        ******************************************************************************/
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            GlobalLog.LogStatus("Constructing list of commands....");

            // Create list of testable commands.
            _stdCommands = TestCommandLibrary.AllCommands;

            return null;
        }

        /******************************************************************************
        * Function:          DoValidate
        ******************************************************************************/
        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.BeginVariation("CommandLibraryInputGesturesApp");
            GlobalLog.LogStatus("Validating...");

            // For this test we are just looking for RoutedCommand library default input gestures to be correct.

            StringBuilder sb = new StringBuilder(1024);
            sb.Append("Name: ");

            int i = 0;
            List<RoutedCommand> commandsWithIncorrectGestures = new List<RoutedCommand>();
            while (i < _stdCommands.Length)
            {
                if (!TestCommandLibrary.HasExpectedGestures(_stdCommands[i]))
                {
                    commandsWithIncorrectGestures.Add(_stdCommands[i]);
                }
                else
                {
                    // Success!
                    sb.Append(_stdCommands[i].Name + ",");
                }
                i++;
            }

            sb.Append("\nCount of incorrect commands: (expect 0) "+commandsWithIncorrectGestures.Count.ToString()+"\n");

            if (commandsWithIncorrectGestures.Count > 0)
            {
                foreach (RoutedCommand cmd in commandsWithIncorrectGestures) {
                    GlobalLog.LogStatus("Error verifying name '" + cmd.Name + "'\n");
                    GlobalLog.LogStatus("\tDeclaringType: " + cmd.OwnerType.ToString() + "\n");
                }
            }

            GlobalLog.LogStatus(sb.ToString());

            // expect to have seen every command OK.
            bool actual = (i == _stdCommands.Length) && (commandsWithIncorrectGestures.Count==0);
            bool expected = true;
            bool eventFound = (actual == expected);

            GlobalLog.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            GlobalLog.LogStatus("Validation complete!");
            CoreLogger.LogTestResult(this.TestPassed, "Passed if event found.  Failed if not.");
            CoreLogger.EndVariation();

            return null;
        }
        #endregion
    }
}
