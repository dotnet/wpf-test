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
    /// Verify setting MouseGesture properties works as expected.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <

    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify setting MouseGesture properties works as expected.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\Interfaces")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class MouseGesturePropertiesApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new MouseGesturePropertiesApp();
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
                MouseAction ma = (MouseAction)(0);
                _gesture = new MouseGesture(ma);
            }
            catch (ArgumentNullException)
            {
                _isArgNullExceptionThrown = true;
            }
            CoreLogger.LogStatus("Null exception thrown? (expect no) " + _isArgNullExceptionThrown);
            this.Assert(!_isArgNullExceptionThrown, "Whoops, we had an incorrect null exception!");
            this.Assert(_gesture != null, "Whoops, gesture not created at all!");


            CoreLogger.LogStatus("Constructing from invalid MouseAction value...");
            _gesture = null;
            _isNotSupportedExceptionThrown = false;
            try
            {
                MouseAction ma = MouseAction.None;
                unchecked { ma = (MouseAction)(-1); }
                _gesture = new MouseGesture(ma);
            }
            catch (InvalidEnumArgumentException)
            {
                _isNotSupportedExceptionThrown = true;
            }
            CoreLogger.LogStatus("Item list invalid exception thrown? (expect yes) " + _isNotSupportedExceptionThrown);
            this.Assert(_isNotSupportedExceptionThrown, "Whoops, we had an incorrect invalid exception from bad value!");


            CoreLogger.LogStatus("Constructing from valid MouseAction and zero modifier value...");
            _gesture = null;
            MouseAction maValid = MouseAction.RightClick;
            ModifierKeys mkValid = (ModifierKeys)(0);
            _gesture = new MouseGesture(maValid, mkValid);
            CoreLogger.LogStatus("Valid gesture created? (expect yes) " + (_gesture != null));
            this.Assert(_gesture != null, "Whoops, gesture not created at all!");


            CoreLogger.LogStatus("Constructing from invalid modifier value...");
            _gesture = null;
            _isNotSupportedExceptionThrown = false;
            try
            {
                MouseAction maInvalid = MouseAction.MiddleClick;
                ModifierKeys mkInvalid = (ModifierKeys)(-1);
                _gesture = new MouseGesture(maInvalid, mkInvalid);

            }
            catch (InvalidEnumArgumentException)
            {
                _isNotSupportedExceptionThrown = true;
            }
            CoreLogger.LogStatus("Item modifiers invalid exception thrown? (expect yes) " + _isNotSupportedExceptionThrown);
            this.Assert(_isNotSupportedExceptionThrown, "Whoops, we didn't have an incorrect invalid exception from bad modifiers!");
            CoreLogger.LogStatus("Checking out MouseAction and modifiers in invalid gesture...");
            this.Assert(_gesture == null, "Whoops, unexpected gesture!");

            CoreLogger.LogStatus("Setting MouseAction to another value...");
            _gesture = new MouseGesture(MouseAction.RightDoubleClick);
            _gesture.MouseAction = MouseAction.LeftDoubleClick;
            CoreLogger.LogStatus("Gesture after set? (expect LeftDoubleClick) " + _gesture.MouseAction);
            CoreLogger.LogStatus("Gesture modifiers match? (expect yes) " + (_gesture.Modifiers == ModifierKeys.None));
            this.Assert((_gesture.MouseAction == MouseAction.LeftDoubleClick) && (_gesture.Modifiers == ModifierKeys.None), "whoops, gesture not correct after MouseAction set! (equal comparison 1)");
            // 

            CoreLogger.LogStatus("Setting modifier to another value...");
            _gesture = new MouseGesture(MouseAction.MiddleClick);
            _gesture.Modifiers = _gesture.Modifiers | ModifierKeys.Alt;


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

            CoreLogger.LogStatus("Gesture exists? (expect yes) " + _gesture);
            CoreLogger.LogStatus("Gesture after set? (expect MiddleClick) " + _gesture.MouseAction);
            CoreLogger.LogStatus("Gesture modifiers match? (expect yes) " + (_gesture.Modifiers == ModifierKeys.Alt));
            
            this.Assert((_gesture.MouseAction == MouseAction.MiddleClick) && (_gesture.Modifiers == ModifierKeys.Alt), "whoops, gesture not correct after Modifier set! (equal comparison 1)");
            // 

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        private MouseGesture _gesture;

        private bool _isArgNullExceptionThrown = false;
        private bool _isNotSupportedExceptionThrown = false;
    }
}
