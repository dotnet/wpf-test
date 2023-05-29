// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
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
    /// Verify RoutedCommand Execute fails for invalid input element.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify RoutedCommand Execute fails for invalid input element.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class ExecuteInvalidInputElementApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new ExecuteInvalidInputElementApp();
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
            // Set up commands
            s_sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command=" + s_sampleCommand.ToString());

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Executing null element....");
            try
            {
                s_sampleCommand.Execute(null, null);
            }
            catch (ArgumentNullException e)
            {
                _exceptionLog.Add(e);
            }

            // Construct invalid input element.
            IInputElement dio = new TestDependencyInputObject();
            CoreLogger.LogStatus("Executing invalid input element....");
            try
            {
                s_sampleCommand.Execute(null, dio);
            }
            catch (InvalidOperationException e)
            {
                _exceptionLog.Add(e);
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

            // For this test we need no focus events to fire. (It IS an invalid input element.)
            // We also need an invalid operation exception to occur.

            CoreLogger.LogStatus("Exceptions found (expect 2): " + _exceptionLog.Count);
            foreach (object log in _exceptionLog)
            {
                CoreLogger.LogStatus("Logged exception:\n" + (Exception)log);
            }

            bool actual = (_exceptionLog.Count == 2) && 
                (_exceptionLog[0].GetType() == typeof(ArgumentNullException)) &&
                (_exceptionLog[1].GetType() == typeof(InvalidOperationException));
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// Store record of our fired exceptions.
        /// </summary>
        private ArrayList _exceptionLog = new ArrayList();

        /// <summary>
        /// Store command to execute.
        /// </summary>
        private static RoutedCommand s_sampleCommand;
    }
}
