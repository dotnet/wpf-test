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
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify RoutedCommand cannot be created with a null declaring type.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\Interfaces")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class CommandNullDeclaringTypeApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandNullDeclaringTypeApp();
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
            CoreLogger.LogStatus("Constructing command....");
            Type declaringType = null;
            try
            {
                _sampleCommand = new RoutedCommand("Sample", declaringType, null);
            }
            catch (ArgumentNullException)
            {
                _exceptionThrownThreeParameters = true;
            }

            Type declaringType2 = null;
            try
            {
                RoutedCommand sampleCommand2 = new RoutedCommand("Sample2", declaringType2);
            }
            catch (ArgumentNullException)
            {
                _exceptionThrownTwoParameters = true;
            }

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

            // For this test we are just looking for a command exception thrown.

            CoreLogger.LogStatus("Exception thrown (three parameter ctor)? (expect yes) " + _exceptionThrownThreeParameters);
            CoreLogger.LogStatus("Exception thrown (two parameter ctor)? (expect yes) " + _exceptionThrownTwoParameters);

            bool actual = (_exceptionThrownThreeParameters) && (_exceptionThrownTwoParameters);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private RoutedCommand _sampleCommand;

        private bool _exceptionThrownThreeParameters = false;
        private bool _exceptionThrownTwoParameters = false;
    }
}
