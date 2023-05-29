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
    /// Verify InputGesturesCollection Count works in a command's collection.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCasePriority("1")]
    [TestCaseTitle(@"Verify InputGesturesCollection Count works in a command's collection.")]
    [TestCaseArea(@"Commanding\Collections")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class InputGesturesCollectionCountFromDefaultsApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new InputGesturesCollectionCountFromDefaultsApp();
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
            CoreLogger.LogStatus("Constructing window....");
            
            {
                // Set up key gestures
                KeyGesture gesture = new KeyGesture(Key.F2, ModifierKeys.Shift);
                InputGestureCollection igc = new InputGestureCollection();
                igc.Add(gesture);

                // Set up commands
                _sampleCommand = new RoutedCommand("Sample", this.GetType(), igc);
                CoreLogger.LogStatus("Command constructed: Command=" + _sampleCommand.ToString());
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

            // For this test we are just looking for a proper count.
            
            // expect multiple command links (not coalesced even though command is the same.)
            int actual = (_sampleCommand.InputGestures.Count);
            int expected = 1;

            CoreLogger.LogStatus("Input gestures found: " + actual + " (expected=" + expected + ")");

            bool eventFound = (actual == expected);

            CoreLogger.LogStatus("Setting log result to " + eventFound);
            this.TestPassed = eventFound;
            
            CoreLogger.LogStatus("Validation complete!");
            
            return null;
        }

        /// <summary>
        /// Store record of our fired events.
        /// </summary>
        private ArrayList _commandLog = new ArrayList();
        
        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="args">Arguments pertaining to the command event.</param>
        private void OnSample(object target, ExecutedRoutedEventArgs args) 
        {
            // We are executing a command!
            _commandLog.Add(args);

            CoreLogger.LogStatus("In command event:");
            CoreLogger.LogStatus(" Command:            " + args.Command.ToString());
            if (target!=null) 
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }
            CoreLogger.LogStatus(" Events found: "+_commandLog.Count);

            
        }
        
        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="args">Arguments pertaining to the command event.</param>
        private void OnQuery(object target, CanExecuteRoutedEventArgs args) 
        {
            // if we are in this handler, the case passes!
            CoreLogger.LogStatus("In query event:");
            CoreLogger.LogStatus(" Command:            " + args.Command.ToString());
            if (target!=null) 
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }
         
            // Show we are handled and we wish to accept commands!
            
        }

        /// <summary>
        /// Our sample command.
        /// </summary>
        private RoutedCommand _sampleCommand;
       
    }
}
