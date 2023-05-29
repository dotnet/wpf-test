// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding.Security
{
    /// <summary>
    /// Verify SecureUICommand does not throw exception when created in partial trust.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify SecureUICommand does not throw exception when created in partial trust.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\Security")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.PartialTrust)]
    public class SecureCommandPartialTrustApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new SecureCommandPartialTrustApp();
            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.Run();
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
            try
            {
                _sampleCommand = ApplicationCommands.Paste;
            }
            catch (Exception e)
            {
                CoreLogger.LogStatus("Unexpected exception on construction:\n" + e.ToString());
                _exceptionHandler = true;
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

            // For this test we are not looking for a command exception thrown.

            CoreLogger.LogStatus("Exception thrown? (expect no) " + _exceptionHandler);
            if (_sampleCommand != null)
            {
                CoreLogger.LogStatus("Command enabled? (expect no) " + _sampleCommand.CanExecute(null, null));
            }

            bool actual = (!_exceptionHandler) && (_sampleCommand != null) && (!_sampleCommand.CanExecute(null, null));
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private RoutedUICommand _sampleCommand = null;

        private bool _exceptionHandler = false;
    }
}
