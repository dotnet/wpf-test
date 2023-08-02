// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections.Generic;
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
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding.Security
{
    /// <summary>
    /// Verify SecureUICommand class command binding does not throw exception when executed directly in partial trust.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify SecureUICommand class command binding does not throw exception when executed directly in partial trust.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\Security")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.PartialTrust)]
    public class SecureCommandBindingExecutePartialTrustApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new SecureCommandBindingExecutePartialTrustApp();
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
            
            InstrPanel p = new InstrPanel();
            CommandBinding cb = new CommandBinding(_sampleCommand);
            cb.Executed += new ExecutedRoutedEventHandler(OnSample);
            CommandManager.RegisterClassCommandBinding(p.GetType(), cb);

            DisplayMe(p, 10, 10, 100, 100);

            return null;
        }


        /// <summary>
        /// New handler for thrown exception.
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        private void ExpectThisException(object o, EventArgs e)
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
            KeyboardHelper.EnsureFocus(_rootElement);

            CoreLogger.LogStatus("Executing command directly....");
            _sampleCommand.Execute(null, null);

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

            // We expect class command binding to be invoked successfully.

            CoreLogger.LogStatus("Command event count: (expect 1) " + _commandLog.Count);
            CoreLogger.LogStatus("Exception thrown: (expect no) " + s_exceptionThrown);

            bool actual = (!s_exceptionThrown) && (_commandLog.Count==1);
            bool expected = true;
            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void OnSample(object sender, ExecutedRoutedEventArgs e)
        {
            // Set test flag
            _commandLog.Add(e);

            // Log some debugging data
            CoreLogger.LogStatus(" ["+e.RoutedEvent.Name+"]");

            if (sender != null)
            {
                CoreLogger.LogStatus("  command sender Name: " + sender.ToString());
            }
            RoutedCommand cmd = e.Command as RoutedCommand;
            if (cmd != null)
            {
                CoreLogger.LogStatus("  command name:        " + cmd.Name);
            }
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private List<ExecutedRoutedEventArgs> _commandLog = new List<ExecutedRoutedEventArgs>();

        private RoutedUICommand _sampleCommand = null;

        private static bool s_exceptionThrown = false;
    }
}
