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
    /// Verify CommandManager RegisterClassInputBinding for binding on parent class of element.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandManager RegisterClassInputBinding for binding on parent class of element.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandManagerRegisterClassInputBindingTypeParentApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandManagerRegisterClassInputBindingTypeParentApp();
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
            CoreLogger.LogStatus("Constructing window....");

            // Construct related Win32 window
            HwndSource source = CreateStandardSource(10, 10, 100, 100);

            // Construct test element
            _rootElement = new MyInstrPanel();

            // Set up commands, Bindings
            _sampleCommand = new RoutedCommand("Sample", typeof(InstrPanel), null);
            CoreLogger.LogStatus("Command constructed: Command=" + _sampleCommand.ToString());
            CommandBinding sampleCommandBinding = new CommandBinding(_sampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + sampleCommandBinding.ToString());
            CommandBinding sampleCommandBinding2 = new CommandBinding(_sampleCommand);
            CoreLogger.LogStatus("Command Binding constructed: CommandBinding=" + sampleCommandBinding2.ToString());

            // Set up mouse bindings
            MouseGesture gesture = new MouseGesture(MouseAction.RightClick);
            MouseBinding sampleMouseBinding = new MouseBinding(_sampleCommand, gesture);
            CoreLogger.LogStatus("Command Binding MouseBinding constructed: MouseBinding=" + sampleMouseBinding.ToString());

            MouseGesture gesture2 = new MouseGesture(MouseAction.LeftClick);
            MouseBinding sampleMouseBinding2 = new MouseBinding(_sampleCommand, gesture2);
            CoreLogger.LogStatus("Command Binding MouseBinding constructed: MouseBinding=" + sampleMouseBinding2.ToString());

            // Attach events to Bindings
            sampleCommandBinding.Executed += new ExecutedRoutedEventHandler(OnSample);
            sampleCommandBinding.CanExecute += new CanExecuteRoutedEventHandler(OnQuery);

            sampleCommandBinding2.Executed += new ExecutedRoutedEventHandler(OnSample2);
            sampleCommandBinding2.CanExecute += new CanExecuteRoutedEventHandler(OnQuery2);

            // Add Bindings to test element
            CommandManager.RegisterClassCommandBinding(typeof(InstrPanel), sampleCommandBinding);
            //_rootElement.CommandBindings.Add(sampleCommandBinding).
            //CommandManager.RegisterClassCommandBinding(typeof(InstrPanel), sampleCommandBinding2).
            //_rootElement.CommandBindings.Add(sampleCommandBinding2).

            CommandManager.RegisterClassInputBinding(typeof(InstrPanel), sampleMouseBinding);
            //_rootElement.InputBindings.Add(sampleMouseBinding).
            //CommandManager.RegisterClassInputBinding(typeof(InstrPanel), sampleMouseBinding2).
            //_rootElement.InputBindings.Add(sampleMouseBinding2).

            // Put the test element on the screen
            Visual v = _rootElement;
            source.RootVisual = v;

            // Save Win32 window handle for later
            _hwnd = new HandleRef(source, source.Handle);


            CoreLogger.LogStatus("Window constructed: hwnd=" + _hwnd.Handle);

            return null;
        }




        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd) 
        {
            _rootElement.Focus();            
            InputCallback[] ops = new InputCallback[]
            {

                delegate
                {
                    MouseHelper.Click(MouseButton.Right, _rootElement);
                }                
            };
            return ops;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.LogStatus("Validating...");

            // For this test we are just looking for a command invocation (Invoke event).

            CoreLogger.LogStatus("Events found: (expect 1) " + _commandLog.Count);

            // expect non-negative event count - 1 command executed
            bool actual = (_commandLog.Count == 1);
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
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }
            CoreLogger.LogStatus(" Events found: " + _commandLog.Count);
        }

        /// <summary>
        /// If we are in this CommandEvent Handler, the case passes.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="args">Arguments pertaining to the command event.</param>
        private void OnSample2(object target, ExecutedRoutedEventArgs args)
        {
            // We are executing a command!
            _commandLog.Add(args);

            CoreLogger.LogStatus("In command2 event:");
            CoreLogger.LogStatus(" Command:            " + args.Command.ToString());
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }
            CoreLogger.LogStatus(" Events found: " + _commandLog.Count);
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
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }

            // Show we are handled and we wish to accept commands!
            args.CanExecute = true;
        }

        /// <summary>
        /// If we are in this event handler, we are being queried.
        /// </summary>
        /// <param name="target">Element that is the target of the event.</param>
        /// <param name="args">Arguments pertaining to the command event.</param>
        private void OnQuery2(object target, CanExecuteRoutedEventArgs args)
        {
            // if we are in this handler, the case passes!
            CoreLogger.LogStatus("In query2 event:");
            CoreLogger.LogStatus(" Command:            " + args.Command.ToString());
            if (target != null)
            {
                CoreLogger.LogStatus(" command target Name: " + target.ToString());
            }

            // Show we are handled and we wish to accept commands!
            args.CanExecute = true;
        }

        private RoutedCommand _sampleCommand = null;

        /// <summary>
        /// Element class that is added to the tree.
        /// </summary>
        public class MyInstrPanel : InstrPanel
        {
        }
    }

}
