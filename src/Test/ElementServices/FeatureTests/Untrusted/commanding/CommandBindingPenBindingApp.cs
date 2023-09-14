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
    /// Verify UIElement CommandBinding SamplePenBinding works for element in window.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\InputBindings")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingSamplePenBindingApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");

            TestApp app = new CommandBindingSamplePenBindingApp();

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
            
            {
                // Construct test element
                _rootElement = new InstrPanel();

                // Set up commands, links
                RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);

                CoreLogger.LogStatus("Command constructed: Command=" + sampleCommand.ToString());

                CommandBinding sampleCommandBinding = new CommandBinding(sampleCommand);

                CoreLogger.LogStatus("Command link constructed: CommandBinding=" + sampleCommandBinding.ToString());

                // Set up pen bindings
                _penBinding = new SamplePenBinding(null);
                CoreLogger.LogStatus("Command link SamplePenBinding constructed: SamplePenBinding=" + _penBinding.ToString());

                // Add links to test element
                _rootElement.CommandBindings.Add(sampleCommandBinding);
                _rootElement.InputBindings.Add(_penBinding);
            }

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

            // For this test we are just looking if we can use a pen-binding indexer to retrieve a command link.
            
            CoreLogger.LogStatus("Events found: " + _rootElement.CommandBindings.Count);

            bool eventFound;

            if (_rootElement.CommandBindings.Count > 0)
            {
                // expect non-negative event count
                bool actual = _rootElement.InputBindings.Contains(_penBinding);

                CoreLogger.LogStatus("Command link found? " + (actual ? "YES: " + _penBinding.ToString() : "NO"));

                eventFound = (actual);
            }
            else
            {
                eventFound = false;
            }

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Store record of our pen binding.
        /// </summary>
        private SamplePenBinding _penBinding = null;
    }

    class SamplePenBinding : InputBinding
    {
        public SamplePenBinding(InputGesture gesture)
        {
            // do nothing
        }
    }
}
