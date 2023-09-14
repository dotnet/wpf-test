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
using System.Windows.Interop;
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
    /// Verify CanExecuteRoutedEventArgs(null) works for element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("2")]
    [TestCaseTitle("Verify CanExecuteRoutedEventArgs(null) works for element in window.")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CanExecuteRoutedEventArgsNullApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CanExecuteRoutedEventArgsNullApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus("App object: "+app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus("App run!");
        }

        CommandBinding _sampleCommandBinding = null;

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            CoreLogger.LogStatus("Constructing window....");
            
            {
                // Construct related Win32 window
                HwndSource source = CreateStandardSource(10, 10, 100, 100);

                // Construct test element, add event handling
                _rootElement = new InstrPanel();

                // Set up commands, Bindings
                RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
                CoreLogger.LogStatus("Command constructed: Command="+sampleCommand.ToString());
                _sampleCommandBinding = new CommandBinding(sampleCommand);
                CoreLogger.LogStatus("Command Binding constructed: CommandBinding="+_sampleCommandBinding.ToString());

                // Add Bindings to test element
                _rootElement.CommandBindings.Add(_sampleCommandBinding); 

                // Save Win32 window handle for later
                _hwnd = new HandleRef(source, source.Handle);

            }
            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg) 
        {
            CoreLogger.LogStatus("Creating args....");
/*
            RoutedCommand sCommand = null;

            try
            {
                // Note - This no longer has a public constructor
                CanExecuteRoutedEventArgs eCommand = new CanExecuteRoutedEventArgs(sCommand);
            }
            catch (ArgumentNullException ex)
            {
                _exceptionLog.Add(ex);
            }
*/
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
            CoreLogger.LogStatus("Validating... NOTE - This test is now testing something that doesn't exist");

            // For this test we need exception to have been thrown using a null constructor.
            
            CoreLogger.LogStatus("Events found: "+_exceptionLog.Count);
            
            // expect non-negative event count
            bool actual = (_exceptionLog.Count==1);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _exceptionLog = new ArrayList();
    }
}
