// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
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

namespace Avalon.Test.CoreUI.CoreInput
{
    /// <summary>
    /// Verify Cursor can be created with a relative path to a custom cursor string.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for input.
    /// </description>
    /// <remarks>
    /// Revisit when product 




    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify Cursor can be created with a relative path to a custom cursor string.")]
    [TestCasePriority("0")]
    [TestCaseArea(@"CoreInput\Cursor")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("1")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    [TestCaseSupportFile(@"star.cur")]
    [TestCaseSupportFile(@"anitest.ani")]
    public class CursorRelativePathStringApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CursorRelativePathStringApp();
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
            CoreLogger.LogStatus("Constructing cursor....");
            _cursorCur = new Cursor(@"star.cur");
            _cursorAni = new Cursor(@"anitest.ani");
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

            CoreLogger.LogStatus("Cur cursor created? (expect non-null) " + _cursorCur);
            CoreLogger.LogStatus("Ani cursor created? (expect non-null) " + _cursorAni);

            this.Assert(_cursorCur != null && _cursorAni != null, "Custom cursors not created");

            this.TestPassed = true;

            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private Cursor _cursorCur;
        private Cursor _cursorAni;
    }
}
