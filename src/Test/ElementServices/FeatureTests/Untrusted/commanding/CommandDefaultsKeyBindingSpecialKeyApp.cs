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
    /// Verify multiple command Bindings with RoutedCommand setter, non-conflicting special key binding (Defaults)
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <


    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify multiple command Bindings with RoutedCommand setter, non-conflicting special key binding (Defaults)")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandDefaultsKeyBindingSpecialKeyApp: TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandDefaultsKeyBindingSpecialKeyApp();
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

                // Set up key bindings for command Bindings
                KeyGesture sampleKeyGesture = new KeyGesture(Key.F2, ModifierKeys.Control);
                CoreLogger.LogStatus("Command Binding KeyGesture constructed: KeyGesture=" + sampleKeyGesture.ToString());
                KeyGesture sampleKeyGesture2 = new KeyGesture(Key.F9, ModifierKeys.None);
                CoreLogger.LogStatus("Command Binding KeyGesture constructed: KeyGesture=" + sampleKeyGesture2.ToString());

                // Set up commands with Defaults
                InputGestureCollection gestures = new InputGestureCollection();
                gestures.Add(sampleKeyGesture);
                gestures.Add(sampleKeyGesture2);

                RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), gestures);
                CoreLogger.LogStatus("Command constructed: Command=" + sampleCommand.ToString());

                //CoreLogger.LogStatus("Command default Bindings: Count="+sampleCommand.Defaults.Count).
                
                // Set up empty Bindings
                CommandBinding[] sampleCommandBinding = new CommandBinding[] { 
                    new CommandBinding(),
                    new CommandBinding(),
                };
                CoreLogger.LogStatus("Command Bindings constructed: "+sampleCommandBinding.Length);
                
                // Attach events to Bindings, then Bindings to the element
                foreach (CommandBinding bind in sampleCommandBinding)
                {
                    bind.Command = sampleCommand;
                    bind.Executed += new ExecutedRoutedEventHandler(OnSample);
                    bind.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);
                    _rootElement.CommandBindings.Add(bind);
                }

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
                    KeyboardHelper.TypeKey(Key.F2, ModifierKeys.Control);
                },
                delegate
                {
                    KeyboardHelper.TypeKey(Key.F9);
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

            // For this test we are just looking for a command invocation (Invoke event)
            // for each key sequence.
            
            CoreLogger.LogStatus("Events found: "+_commandLog.Count);
            
            // expect non-negative event count
            int actual = (_commandLog.Count);
            int expected = 2;
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
