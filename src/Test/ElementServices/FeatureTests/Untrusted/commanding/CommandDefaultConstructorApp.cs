// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
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
    * CLASS:          CommandDefaultConstructorApp
    ******************************************************************************/
    /// <summary>
    /// Verify RoutedCommand can be created with default constructors.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [Test(0, "Commanding.Interfaces", TestCaseSecurityLevel.FullTrust, "CommandDefaultConstructorApp")]
    public class CommandDefaultConstructorApp : TestApp
    {
        #region Private Data
        private RoutedCommand _sampleCommand = null;
        private bool _exceptionThrown = false;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          CommandDefaultConstructorApp Constructor
        ******************************************************************************/
        public CommandDefaultConstructorApp() :base()
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
        public override object DoSetup(object sender)
        {
            GlobalLog.LogStatus("Constructing command....");
            try
            {
                _sampleCommand = new RoutedCommand();
            }
            catch (Exception e)
            {
                GlobalLog.LogStatus("Unexpected exception on construction:\n" + e.ToString());
                _exceptionThrown = true;
            }

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
            CoreLogger.BeginVariation("CommandDefaultConstructorApp");

            GlobalLog.LogStatus("Validating...");

            // For this test we are just looking for a command exception thrown.

            GlobalLog.LogStatus("Exception thrown? (expect no) " + _exceptionThrown);
            if (_sampleCommand != null)
            {
                GlobalLog.LogStatus("Command enabled? (expect no) " + _sampleCommand.CanExecute(null, null));
            }

            bool actual = (!_exceptionThrown) && (_sampleCommand != null) && (!_sampleCommand.CanExecute(null, null));
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
