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
    /// Verify CommandBindingCollection Remove method does nothing if a non-commandBinding is removed.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandBindingCollection Remove method does nothing if a non-commandBinding is removed.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingCollectionRemoveNonBindingApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingCollectionRemoveNonBindingApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus("App object: "+app.ToString());

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
            
            {
                // Set up commands, Bindings
                _sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
                CoreLogger.LogStatus("Command constructed: Command="+_sampleCommand.ToString());
                CommandBinding sampleCommandBinding = new CommandBinding(_sampleCommand);
                CoreLogger.LogStatus("Command Binding constructed: CommandBinding="+sampleCommandBinding.ToString());
            
                // Set up read-write Binding collection
                CommandBinding[] defaultBindings = new CommandBinding[] { sampleCommandBinding };
                _readwriteCommandBindings = new CommandBindingCollection();
                _readwriteCommandBindings.Add(sampleCommandBinding);
                CoreLogger.LogStatus("Read-write RoutedCommand Binding collection constructed: " + _readwriteCommandBindings.ToString());
            }

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg) 
        {
            // Remove the command
            CoreLogger.LogStatus("Removing command:");
            IList readwriteCommandBindingsList = _readwriteCommandBindings as IList;
            readwriteCommandBindingsList.Remove(_sampleCommand);
        
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

            // For this test we want to know if a RoutedCommand Binding collection was sealed correctly.

            CoreLogger.LogStatus("Commmand Bindings count: " + _readwriteCommandBindings.Count);

            this.TestPassed = (_readwriteCommandBindings.Count == 1);

            CoreLogger.LogStatus("Validation complete!");
            return null;
        }

        /// <summary>
        /// Stores a read-write collection of Bindings.
        /// </summary>
        private CommandBindingCollection _readwriteCommandBindings;
        
        /// <summary>
        /// Stores a sample command.
        /// </summary>
        private RoutedCommand _sampleCommand;
        
    }
}
