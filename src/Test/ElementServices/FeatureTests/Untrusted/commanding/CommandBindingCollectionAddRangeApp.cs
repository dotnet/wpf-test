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
    /// Verify CommandBindingCollection AddRange method succeeds on ArrayList of bindings.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandBindingCollection AddRange method succeeds on ArrayList of bindings.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingCollectionAddRangeApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingCollectionAddRangeApp();
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
                // Set up readonly Binding collection
                _readwriteCommandBindings = new CommandBindingCollection(null);
                CoreLogger.LogStatus("Readonly RoutedCommand Binding collection constructed: "+_readwriteCommandBindings.ToString());
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
            // Set up commands, Bindings
            RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
            CoreLogger.LogStatus("Command constructed: Command="+sampleCommand.ToString());
            
            CommandBinding sampleCommandBinding = new CommandBinding(sampleCommand);
            CommandBinding sampleCommandBinding2 = new CommandBinding(sampleCommand);
            ArrayList bindinglist = new ArrayList();
            bindinglist.Add(sampleCommandBinding);
            bindinglist.Add(sampleCommandBinding2);

            _bIsReadOnly = ((IList)_readwriteCommandBindings).IsReadOnly;

            // Add the Binding
            CoreLogger.LogStatus("Adding command Bindings (range=2):");
            _readwriteCommandBindings.AddRange(bindinglist);
        
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

            // For this test we want to know if a RoutedCommand Link collection was sealed correctly.

            CoreLogger.LogStatus("Collection read-only? " + _bIsReadOnly);
            CoreLogger.LogStatus("Collection Binding count: " + _readwriteCommandBindings.Count);

            this.TestPassed = (!_bIsReadOnly) && (_readwriteCommandBindings.Count == 2);

            CoreLogger.LogStatus("Validation complete!");
            return null;
        }

        /// <summary>
        /// Is the collection read-only?
        /// </summary>
        private bool _bIsReadOnly = true;

        /// <summary>
        /// Stores a readonly collection of Bindings.
        /// </summary>
        private CommandBindingCollection _readwriteCommandBindings;
        
    }
}
