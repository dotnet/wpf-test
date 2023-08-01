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
    /// Verify CommandBinding KeyBinding to different links using same key, different modifiers.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandBinding KeyBinding to different links using same key, different modifiers.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingSameKeyBindingApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingSameKeyBindingApp();
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
                // Construct related Win32 window
                HwndSource source = CreateStandardSource(10, 10, 100, 100);

                // Construct test element
                _rootElement = new InstrPanel();

                // Set up commands
                RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);
                CoreLogger.LogStatus("Command constructed: Command="+sampleCommand.ToString());
                RoutedCommand sample2Command = new RoutedCommand("Sample2", this.GetType(), null);
                CoreLogger.LogStatus("Command constructed: Command="+sample2Command.ToString());
                
                // Set up links
                CommandBinding sampleCommandBinding = new CommandBinding(sampleCommand);
                CoreLogger.LogStatus("Command link constructed: CommandBinding="+sampleCommandBinding.ToString());
                CommandBinding sample2CommandBinding = new CommandBinding(sample2Command);
                CoreLogger.LogStatus("Command link constructed: CommandBinding="+sample2CommandBinding.ToString());

                // Set up key bindings
                KeyBinding sampleKeyBinding = new KeyBinding(sampleCommand, Key.F1, ModifierKeys.Shift);
                CoreLogger.LogStatus("Command link KeyBinding constructed: KeyBinding="+sampleKeyBinding.ToString());
                KeyBinding sample2KeyBinding = new KeyBinding(sample2Command, Key.F1, ModifierKeys.Control);
                CoreLogger.LogStatus("Command link KeyBinding constructed: KeyBinding="+sample2KeyBinding.ToString());

                // Attach events to links
                sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
                sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);
                sample2CommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
                sample2CommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

                // Add links to test element
                _rootElement.CommandBindings.Add(sampleCommandBinding); 
                _rootElement.CommandBindings.Add(sample2CommandBinding);
                _rootElement.InputBindings.Add(sampleKeyBinding);
                _rootElement.InputBindings.Add(sample2KeyBinding);

                // Put the test element on the screen
                Visual v = _rootElement;
                source.RootVisual = v;

                // Save Win32 window handle for later
                _hwnd = new HandleRef(source, source.Handle);

            }
            CoreLogger.LogStatus("Window constructed: hwnd="+_hwnd.Handle);

            return null;
        }


        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd) 
        {
            // Two key sequences to invoke 2 commands.
            
            InputCallback[] ops = new InputCallback[]
            {
                delegate
                {
                    DoBeforeExecute();
                },
                delegate
                {
                    KeyboardHelper.TypeKey(Key.F1, ModifierKeys.Shift);
                },
                delegate
                {
                    KeyboardHelper.TypeKey(Key.F1, ModifierKeys.Control);
                },               

            };
            return ops;
        }    

        /// <summary>
        /// Execute stuff right before the test operations.
        /// </summary>
        private void DoBeforeExecute() 
        {
            CoreLogger.LogStatus("Setting focus to the element....");
            bool bFocus = _rootElement.Focus();
            CoreLogger.LogStatus("Focus set via API?           " + (bFocus));
            CoreLogger.LogStatus("Focus set via InputManager?  " + (InputHelper.GetFocusedElement()!=null));

            // Now our TestOps will fire....
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender) 
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we are just looking for the right number of commands.
            // We also want to make sure that both commands were invoked, not just one.
            // Their command names should not be identical.
            
            CoreLogger.LogStatus("Events found: "+_commandLog.Count);
            bool bCommandsMatch = true;
            if (_commandLog.Count >= 2)
            {
                ExecutedRoutedEventArgs cieArgs1 = _commandLog[0] as ExecutedRoutedEventArgs;
                ExecutedRoutedEventArgs cieArgs2 = _commandLog[1] as ExecutedRoutedEventArgs;
                string name1 = ((RoutedCommand)(cieArgs1.Command)).Name;
                string name2 = ((RoutedCommand)(cieArgs2.Command)).Name;

                bCommandsMatch = (name1 == name2);
            }
            
            // expect non-negative event count and non-matching commands
            bool actual = (_commandLog.Count==2) && (!bCommandsMatch);
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
            args.CanExecute = true;
        }
    }
}
