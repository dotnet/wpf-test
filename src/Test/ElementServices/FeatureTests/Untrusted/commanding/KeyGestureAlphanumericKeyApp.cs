// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
    /// Verify KeyGesture constructed with plain alphanumeric key fails.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class KeyGestureAlphanumericKeyApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("2", @"Commanding\CoreCommanding", "HwndSource", TestCaseSecurityLevel.FullTrust,@"Compile and Verify KeyGesture constructed with plain alphanumeric key fails in HwndSource.")]
        [TestCase("1", @"Commanding\CoreCommanding", "Browser",TestCaseSecurityLevel.PartialTrust, @"Compile and Verify KeyGesture constructed with plain alphanumeric key fails in Browser.")]
        [TestCase("2", @"Commanding\CoreCommanding", "Window", TestCaseSecurityLevel.FullTrust,@"Compile and Verify KeyGesture constructed with plain alphanumeric key fails in window.")]
        [TestCaseTimeout("120")]
        public static void LaunchTestCompile()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            GenericCompileHostedCase.RunCase(
                "Avalon.Test.CoreUI.Commanding",
                "CoreTestsUntrusted",
                "KeyGestureAlphanumericKeyApp",
                "Run",
                hostType);
        }

        /// <summary>
        /// Launch our test.
        /// </summary>
        [TestCase("1", @"Commanding\CoreCommanding", "HwndSource",TestCaseSecurityLevel.FullTrust, @"Verify KeyGesture constructed with plain alphanumeric key fails in HwndSource.")]
        [TestCase("1", @"Commanding\CoreCommanding", "Window", TestCaseSecurityLevel.FullTrust,@"Verify KeyGesture constructed with plain alphanumeric key fails in window.")]
        public static void LaunchTest()
        {
            HostType hostType = (HostType)Enum.Parse(typeof(HostType), DriverState.DriverParameters["TestParameters"]);

            ExeStubContainerFramework exe = new ExeStubContainerFramework(hostType);
            exe.Run(new KeyGestureAlphanumericKeyApp(), "Run");

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

            DisplayMe(cvs, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Execute stuff
        /// </summary>
        protected override object DoExecute(object sender)
        {
            CoreLogger.LogStatus("Setting up gestures....");
            try
            {
                KeyGesture commonGesture = new KeyGesture(Key.F);
            }
            catch (NotSupportedException ex)
            {
                CoreLogger.LogStatus("Expected exception thrown:\n"+ex);
                _exceptionThrown = true;
            }

            base.DoExecute(sender);
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

            CoreLogger.LogStatus("Exception thrown? (expect yes) " + _exceptionThrown);

            Assert(_exceptionThrown, "Didn't see expected exception");

            TestPassed = true;
            CoreLogger.LogStatus("Test passed");

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private bool _exceptionThrown = false;

    }
}
