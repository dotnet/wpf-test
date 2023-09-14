// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Verify setting KeyGesture properties works as expected.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify setting KeyGesture properties works as expected.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\Interfaces")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class KeyGesturePropertiesApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new KeyGesturePropertiesApp();
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
            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object sender)
        {
            CoreLogger.LogStatus("Constructing from zero...");
            _gesture = null;
            try
            {
                Key k = (Key)(0);
                _gesture = new KeyGesture(k);
            }
            catch (ArgumentNullException)
            {
                _isArgNullExceptionThrown = true;
            }

            CoreLogger.LogStatus("Null exception thrown? (expect no) " + _isArgNullExceptionThrown);
            this.Assert(!_isArgNullExceptionThrown, "Whoops, we had an incorrect null exception!");
            this.Assert(_gesture != null, "Whoops, gesture not created at all!");

            CoreLogger.LogStatus("Constructing from invalid key value...");
            _gesture = null;
            _isNotSupportedExceptionThrown = false;
            try
            {
                Key k = (Key)(-1);
                _gesture = new KeyGesture(k);
            }
            catch (InvalidEnumArgumentException)
            {
                _isNotSupportedExceptionThrown = true;
            }

            CoreLogger.LogStatus("Item list invalid exception thrown? (expect yes) " + _isNotSupportedExceptionThrown);
            this.Assert(_isNotSupportedExceptionThrown, "Whoops, we had an incorrect invalid exception from bad value!");

            CoreLogger.LogStatus("Constructing from valid key and zero modifier value...");
            _gesture = null;
            Key kValid = Key.Prior;
            ModifierKeys mkValid = (ModifierKeys)(0);
            _gesture = new KeyGesture(kValid, mkValid);

            CoreLogger.LogStatus("Valid gesture created? (expect yes) " + (_gesture != null));
            this.Assert(_gesture != null, "Whoops, gesture not created at all!");

            CoreLogger.LogStatus("Constructing from invalid modifier value...");
            _gesture = null;
            _isNotSupportedExceptionThrown = false;
            try
            {
                Key kInvalid = Key.Next;
                ModifierKeys mkInvalid = (ModifierKeys)(-1);
                _gesture = new KeyGesture(kInvalid, mkInvalid);

            }
            catch (InvalidEnumArgumentException)
            {
                _isNotSupportedExceptionThrown = true;
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

            CoreLogger.LogStatus("Item modifiers invalid exception thrown? (expect yes) " + _isNotSupportedExceptionThrown);
            this.Assert(_isNotSupportedExceptionThrown, "Whoops, we didn't have an incorrect invalid exception from bad modifiers!");
            CoreLogger.LogStatus("Checking out key and modifiers in invalid gesture...");
            this.Assert(_gesture == null, "Whoops, unexpected gesture!");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private KeyGesture _gesture;

        private bool _isArgNullExceptionThrown = false;
        private bool _isNotSupportedExceptionThrown = false;
    }
}
