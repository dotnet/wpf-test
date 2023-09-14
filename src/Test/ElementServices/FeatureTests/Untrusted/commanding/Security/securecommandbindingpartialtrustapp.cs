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
using System.Security.Policy;
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
    /// Verify SecureUICommand does not throw exception when bound in partial trust.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify SecureUICommand does not throw exception when bound in partial trust.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\Security")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.PartialTrust)]
    public class SecureCommandBindingPartialTrustApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new SecureCommandBindingPartialTrustApp();
            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

            app.FailAfterException = false;

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
            this.TestContainer.ShouldLogUnhandledException = false;
            this.TestContainer.ExceptionThrown += new EventHandler(ExpectThisException);

            CoreLogger.LogStatus("Constructing command....");
            _sampleCommand = ApplicationCommands.Paste;

            return null;
        }


        /// <summary>
        /// New handler for thrown exception.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="args"></param>
        private void ExpectThisException(object o, EventArgs args)
        {
            CoreLogger.LogStatus("=== Hey I expected this exception ===");
            s_exceptionThrown = true;

            CoreLogger.LogStatus("=== Closing.... ===");
            this.DoValidate(null);
            this.DoTearDown(null);
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            CoreLogger.LogStatus("Executing command binding....");
            _commandBinding = new CommandBinding(_sampleCommand);

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

            // For this test we should already have thrown an exception.
            CoreLogger.LogStatus("Exception thrown: (expect no) " + s_exceptionThrown);
            CoreLogger.LogStatus("Stored binding: (expect non-null) " + _commandBinding);

            bool actual = (!s_exceptionThrown) && (_commandBinding != null);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private RoutedUICommand _sampleCommand = null;

        private CommandBinding _commandBinding = null;

        private static bool s_exceptionThrown = false;
    }
}
