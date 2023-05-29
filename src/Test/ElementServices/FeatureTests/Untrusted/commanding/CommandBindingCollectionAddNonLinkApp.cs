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
    /// Verify CommandBindingCollection Add method fails if a non-commandBinding is added.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <


    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandBindingCollection Add method fails if a non-commandBinding is added.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class CommandBindingCollectionAddNonBindingApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            
            CoreLogger.LogStatus ("Creating app object...");
            TestApp app = new CommandBindingCollectionAddNonBindingApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus ("App object: "+app.ToString());

            CoreLogger.LogStatus ("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus ("App run!");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            
            {
                // Set up read-write Binding collection
                _readwriteCommandBindings = new CommandBindingCollection(null);
                CoreLogger.LogStatus ("Read-write RoutedCommand Binding collection constructed: "+_readwriteCommandBindings.ToString());
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
            CoreLogger.LogStatus ("Command constructed: Command="+sampleCommand.ToString());

            // Add the Binding
            IList readwriteCommandBindingsList = _readwriteCommandBindings as IList;
            CoreLogger.LogStatus ("Adding command:");
            try
            {
                _readwriteCommandBindings.Add(null);
                readwriteCommandBindingsList.Add(sampleCommand);
            }
            catch (NotSupportedException e)
            {
                CoreLogger.LogStatus ("Expected exception: "+e.Message);
                _exceptionThrown = true;                    
            }
        
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
            CoreLogger.LogStatus ("Validating...");

            // For this test we want to know if a RoutedCommand Binding collection was sealed correctly.

            CoreLogger.LogStatus ("Exception thrown? "+_exceptionThrown);

            this.TestPassed = _exceptionThrown;

            CoreLogger.LogStatus ("Validation complete!");
            return null;
        }

        /// <summary>
        /// Was the expected exception thrown?
        /// </summary>
        private bool _exceptionThrown = false;

        /// <summary>
        /// Stores a read-write collection of Bindings.
        /// </summary>
        private CommandBindingCollection _readwriteCommandBindings;
        
    }
}
