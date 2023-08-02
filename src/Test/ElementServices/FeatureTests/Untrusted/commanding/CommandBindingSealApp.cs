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
    /// Verify RoutedCommand InputGestureCollection is not sealed on creating a command binding.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify RoutedCommand InputGestureCollection is not sealed on creating a command binding.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\CommandBindings")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingSealApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingSealApp();
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
            // Construct test element
            _rootElement = new InstrPanel();

            // Construct input gestures
            _igc = new InputGestureCollection();
            _igc.Add(new KeyGesture(Key.F7));

            // Set up commands, links
            _sampleCommand = new RoutedCommand("Sample", this.GetType(), _igc);
            CoreLogger.LogStatus("Command constructed: Command="+_sampleCommand.ToString());

            CoreLogger.LogStatus("Gestures found: " + _sampleCommand.InputGestures.Count);
            Assert(_sampleCommand.InputGestures.Count==1, "Incorrect number of gestures found after cmd construction (expect 1)");

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg) 
        {
            
            // Add another input gesture API
            CoreLogger.LogStatus("Trying to add a new input gesture post-seal...");
            _igc.Add(new KeyGesture(Key.F8));

            CoreLogger.LogStatus("Gestures found: " + _sampleCommand.InputGestures.Count);
            Assert(_sampleCommand.InputGestures.Count==2, "Incorrect number of gestures found after gesture addition (expect 2)");

            _wasSealed = _igc.IsReadOnly;
            CoreLogger.LogStatus("Previous seal state = " + _wasSealed + ". Sealing the Gesture....");

            // Seal by binding to a command
            _sampleCommandBinding = new CommandBinding(_sampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + _sampleCommandBinding.ToString());
            CoreLogger.LogStatus("Gestures found: " + _sampleCommand.InputGestures.Count);

            // Add command bindings to test element
            CoreLogger.LogStatus("Trying to add a new command binding pre-seal...");
            _rootElement.CommandBindings.Add(_sampleCommandBinding);

            // Add one input gesture
            CoreLogger.LogStatus("Trying to add a new input gesture pre-seal...");
            _sampleCommand.InputGestures.Add(new KeyGesture(Key.F10));

            // Seal it up
            CoreLogger.LogStatus("Sealing input gesture collection...");
            _sampleCommand.InputGestures.Seal();

            // Add another input gesture
            CoreLogger.LogStatus("Trying to add a new input gesture post-seal...");
            try
            {
                _sampleCommand.InputGestures.Add(new KeyGesture(Key.F9));
            }
            catch (NotSupportedException)
            {
                _expectedExceptionThrown = true;
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
            CoreLogger.LogStatus("Validating...");

            // For this test we are looking if a non-sealed Gesture was sealed correctly.
            
            CoreLogger.LogStatus("Gestures found: (expect 3) " + _sampleCommand.InputGestures.Count);

            bool eventFound;
            if (_sampleCommand.InputGestures.Count == 3)
            {
                CoreLogger.LogStatus("Collection readonly? (expect yes) " + _igc.IsReadOnly);
                CoreLogger.LogStatus("Collection was sealed? (expect no) " + _wasSealed);
                CoreLogger.LogStatus("Expected exception thrown? (expect yes) " + _expectedExceptionThrown);

                bool actual = ((_igc.IsReadOnly) && (!_wasSealed) && _expectedExceptionThrown);

                CoreLogger.LogStatus("Command Gesture found? " + (actual ? "YES: Sealed=" + _igc.IsReadOnly : "NO"));

                eventFound = (actual);
            }
            else
            {
                CoreLogger.LogStatus("Incorrect number of RoutedCommand Gestures found!");
                eventFound = false;
            }

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Store record of our previous sealing state.
        /// </summary>
        private bool _wasSealed;

        private InputGestureCollection _igc;

        private CommandBinding _sampleCommandBinding = null;

        private RoutedCommand _sampleCommand = null;

        private bool _expectedExceptionThrown = false;
    }
}
