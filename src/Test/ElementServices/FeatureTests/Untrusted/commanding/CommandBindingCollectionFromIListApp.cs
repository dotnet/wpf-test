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
    /// Verify constructing CommandBindingCollection from an IList works as expected.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify constructing CommandBindingCollection from an IList works as expected.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingCollectionFromIListApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingCollectionFromIListApp();
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
            CoreLogger.LogStatus("Constructing from null list...");
            try
            {
                _collection = new CommandBindingCollection(null);
            }
            catch (ArgumentNullException)
            {
                _isArgNullExceptionThrown = true;
            }


            CoreLogger.LogStatus("Item list null exception thrown? (expect no) " + _isArgNullExceptionThrown);
            Assert(!_isArgNullExceptionThrown, "Whoops, we had an incorrect null exception!");
            Assert(_collection != null, "Whoops, collection not created at all!");
            Assert(_collection.Count == 0, "Whoops, collection has non-zero number of elements!");

            CoreLogger.LogStatus("Constructing from invalid IList...");
            try
            {
                IList stringList = (IList)(new string[] { "A", "B" });
                _collection = new CommandBindingCollection(stringList);
            }
            catch (NotSupportedException)
            {
                _isNotSupportedExceptionThrown = true;
            }

            CoreLogger.LogStatus("Item list invalid exception thrown? (expect yes) " + _isNotSupportedExceptionThrown);
            Assert(_isNotSupportedExceptionThrown, "Whoops, we had an incorrect invalid exception!");

            CoreLogger.LogStatus("Constructing from valid IList...");
            IList validList = (IList)(new CommandBinding[] { 
                        new CommandBinding(SampleCommand),
                    });
            _collection = new CommandBindingCollection(validList);

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

            CoreLogger.LogStatus("Valid collection created? (expect yes) " + (_collection != null));
            Assert(_collection != null, "Whoops, collection not created at all!");

            CoreLogger.LogStatus("Valid collection count: (expect 1) " + (_collection.Count));
            Assert(_collection.Count == 1, "Whoops, collection has unexpected number of elements!");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);

            CoreLogger.LogStatus("Validation complete!");

            return null;
        }

        
        
        
        

        /// <summary>
        /// Sample command belonging to this class.
        /// </summary>
        public static RoutedCommand SampleCommand
        {
            get
            {
                if (s_sampleCommand == null)
                {
                    s_sampleCommand = new RoutedCommand("Sample", typeof(CommandBindingCollectionFromIListApp), null);
                }
                return s_sampleCommand;
            }
        }
        private static RoutedCommand s_sampleCommand = null;

        private CommandBindingCollection _collection;

        private bool _isArgNullExceptionThrown = false;
        private bool _isNotSupportedExceptionThrown = false;
    }
}
