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
    /// Verify CommandBindingCollection Contains works for element with multiple CommandBindings to same RoutedCommand
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandBindingCollection Contains works for element with multiple CommandBindings to same Command")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\Collections")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingCollectionContainsApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingCollectionContainsApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus("App object: "+app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus("App run!");
        }

        /// <summary>
        /// How many command links on our root element?
        /// </summary>
        private const int NUMCOMMANDLINKS = 2;
       
        CommandBinding[] _sampleCommandBindings = null;

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

                // Set up commands
                RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
                CoreLogger.LogStatus("Command constructed: Command="+sampleCommand.ToString());
                
                // Create multiple command links to the samme command.
                _sampleCommandBindings = new CommandBinding[NUMCOMMANDLINKS];
                for (int i=0; i<_sampleCommandBindings.Length; i++) 
                {
                    // Create the link
                    _sampleCommandBindings[i] = new CommandBinding(sampleCommand);
                    CoreLogger.LogStatus("Command link constructed: CommandBinding="+_sampleCommandBindings[i].ToString());

                    // Attach events to the link
                    _sampleCommandBindings[i].Executed += new ExecutedRoutedEventHandler(OnSample);
                    _sampleCommandBindings[i].CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

                    // Add link to test element
                    _rootElement.CommandBindings.Add(_sampleCommandBindings[i]); 
                }
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

            // For this test we are just evaluating various Contains() command-link scenarios.
            
            // grab existing command link
            CommandBinding sampleCommandBinding = _sampleCommandBindings[0];
            // create new command link associated with same command as previous links
            CommandBinding newCommandBinding = new CommandBinding(sampleCommandBinding.Command);
            
            bool containsExisting = _rootElement.CommandBindings.Contains(sampleCommandBinding);
            bool containsNew = _rootElement.CommandBindings.Contains(newCommandBinding);
            CoreLogger.LogStatus("Collection contains existing command link?  " + containsExisting);
            CoreLogger.LogStatus("Collection contains new command link?  " + containsNew);
            
            bool actual = (containsExisting) && (!containsNew);
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
    }
}
