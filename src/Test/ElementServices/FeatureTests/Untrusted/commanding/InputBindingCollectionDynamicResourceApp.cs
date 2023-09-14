// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify that dynamic resources work with key bindings. In addition, verify
    /// that a CommandTarget can be set to an object through data binding.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>pdanino</author>
 
    [Test(1, "Commanding.CoreCommanding", TestCaseSecurityLevel.FullTrust, "InputBindingCollectionDynamicResourceApp",Versions="4.0+,4.0Client+")]
    public class InputBindingCollectionDynamicResourceApp : TestApp
    {
        #region Private Data

        // Store the results of the commands' parameters after they are called
        // Used for verification later
        private RoutedCommand _firstCommand;
        private RoutedCommand _secondCommand;
        private Button _sourceButton;
        private Button _targetButton;
        private StackPanel _panel;
        private string _commandDynamicResourceId = "KeyBindingRoutedCommand";
        private string _keyDynamicResourceId = "KeyDynamicResourceId";

        // Store record of our fired events.
        private List<CommandEventRecord> _firstCommandLog = new List<CommandEventRecord>();
        private List<CommandEventRecord> _secondCommandLog = new List<CommandEventRecord>();

        #endregion


        #region Constructor

        public InputBindingCollectionDynamicResourceApp()
        {
            GlobalLog.LogStatus("In InputBindingCollectionDynamicResourceApp constructor");
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps

        TestResult StartTest()
        {
            ExeStubContainerCore exe = new ExeStubContainerCore();
            TestApp app = new InputBindingCollectionDynamicResourceApp();
            exe.Run(app, "RunTestApp");

            //Any test failure will be caught by an Assert during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test elements
            // A stackpanel will contain both a text box and a button
            // The button will get the focus during the test, although the actions will be perfomed on the text box
            _sourceButton = new Button();
            _sourceButton.Width = 15;
            _sourceButton.Height = 15;
            _sourceButton.Name = "CommandSourceButton";

            _targetButton = new Button();
            _targetButton.Width = 15;
            _targetButton.Height = 15;
            _targetButton.Name = "CommandTargetButton";

            _panel = new StackPanel();
            // Name scope is used so that data binding can resolve the element name
            NameScope.SetNameScope(_panel, new NameScope());
            _panel.Children.Add(_targetButton);
            _panel.Children.Add(_sourceButton);
            _panel.RegisterName(_targetButton.Name, _targetButton);

            // Set up command with this Binding as a default
            _firstCommand = new RoutedCommand("FirstCommand", this.GetType());
            GlobalLog.LogStatus("Command constructed: Command=" + _firstCommand.ToString());
            _secondCommand = new RoutedCommand("SecondCommand", this.GetType());
            GlobalLog.LogStatus("Command constructed: Command=" + _secondCommand.ToString());

            // Create a normal command Binding using this command.
            CommandBinding firstCommandBinding = new CommandBinding(_firstCommand);
            CommandBinding secondCommandBinding = new CommandBinding(_secondCommand);

            // Attach events to bindings
            firstCommandBinding.Executed += delegate(object firstSender, ExecutedRoutedEventArgs firstArgs)
            {
                CommandExecuted(_firstCommandLog, new CommandEventRecord(firstSender, firstArgs));
            };
            secondCommandBinding.Executed += delegate(object secondSender, ExecutedRoutedEventArgs secondArgs)
            {
                CommandExecuted(_secondCommandLog, new CommandEventRecord(secondSender, secondArgs));
            };

            // Define the key binding
            KeyBinding keyBinding = new KeyBinding();
            DynamicResourceExtension dynamicResource = new DynamicResourceExtension(_keyDynamicResourceId);
            keyBinding.SetValue(KeyBinding.KeyProperty, dynamicResource.ProvideValue(null));
            GlobalLog.LogStatus("Created dynamic resource reference: " + _keyDynamicResourceId);
            keyBinding.Modifiers = ModifierKeys.Control;
            GlobalLog.LogStatus("Created key binding: " + keyBinding.Modifiers + "-" + keyBinding.Key);

            // Associate the key gesture to the a command
            dynamicResource = new DynamicResourceExtension(_commandDynamicResourceId);
            keyBinding.SetValue(InputBinding.CommandProperty, dynamicResource.ProvideValue(null));
            GlobalLog.LogStatus("Created dynamic resource reference: " + _commandDynamicResourceId);
            Binding commandTargetBinding = new Binding();
            PresentationTraceSources.SetTraceLevel(commandTargetBinding, PresentationTraceLevel.High);
            commandTargetBinding.ElementName = _targetButton.Name;
            BindingOperations.SetBinding(keyBinding, InputBinding.CommandTargetProperty, commandTargetBinding);
            GlobalLog.LogStatus("Created data binding to element: " + _targetButton.Name);

            // Put the test element on the screen
            _targetButton.CommandBindings.Add(firstCommandBinding);
            _targetButton.CommandBindings.Add(secondCommandBinding);
            GlobalLog.LogStatus("Added CommandBinding Button");
            _panel.Resources.Add(_commandDynamicResourceId, _firstCommand);
            _panel.Resources.Add(_keyDynamicResourceId, Key.M);
            _panel.InputBindings.Add(keyBinding);
            GlobalLog.LogStatus("Added Dynamic Resource and KeyBinding to Panel");

            _rootElement = new InstrPanel();
            ((InstrPanel)_rootElement).AppendChild(_panel);
            DisplayMe(_rootElement, 10, 10, 100, 100);

            return null;
        }

        /// <summary>
        /// Identify test operations to run.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <returns>Array of test operations.</returns>
        protected override InputCallback[] GetTestOps(HandleRef hwnd)
        {
            InputCallback[] ops = new InputCallback[]
            {
                // Try focusing on an element other than the textbox to later validate command target
                delegate
                {
                    GlobalLog.LogStatus("Setting focus to Source Button");
                    ChangeFocus(_sourceButton);
                },
                // Try to first
                delegate
                {
                    GlobalLog.LogStatus("Send key gesture to Target Button");
                    KeyboardHelper.TypeKey(Key.M, ModifierKeys.Control);
                },
                // Changing command to second
                delegate
                {
                    ChangeCommand(_secondCommand, _panel);
                },
                // Ensure focus is on button to test command target
                delegate
                {
                    GlobalLog.LogStatus("Setting focus back to Source Button");
                    ChangeFocus(_sourceButton);
                },
                // Try to second
                delegate
                {
                    GlobalLog.LogStatus("Send key gesture to Target Button");
                    KeyboardHelper.TypeKey(Key.M, ModifierKeys.Control);
                },
                // change the key dynamic resource
                delegate
                {
                    ChangeKey(Key.N, _panel);
                    ChangeFocus(_sourceButton);
                },
                // Try to execute second command with modified key
                delegate
                {
                    KeyboardHelper.TypeKey(Key.N, ModifierKeys.Control);
                },
            };
            return ops;
        }

        #endregion


        #region Public and Protected Members

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            CoreLogger.BeginVariation("InputBindingCollectionDynamicResourceApp");
            GlobalLog.LogStatus("Validating...");

            // Need to ensure that each of the commands was executed exactly once
            // This will ensure the event fired properly,

            GlobalLog.LogStatus(" Events found first: (expect 1) " + _firstCommandLog.Count);
            GlobalLog.LogStatus(" Events found second: (expect 2) " + _secondCommandLog.Count);

            bool commandLogsValid = false;
            if (_firstCommandLog.Count == 1 && _secondCommandLog.Count == 2 &&
                _firstCommandLog[0].Sender == _targetButton && _secondCommandLog[0].Sender == _targetButton && _secondCommandLog[1].Sender == _targetButton)
            {
                commandLogsValid = true;
            }

            GlobalLog.LogStatus("Setting log result to " + commandLogsValid);
            this.TestPassed = commandLogsValid;

            GlobalLog.LogStatus("Validation complete!");

            CoreLogger.LogTestResult(this.TestPassed, "Passed if event found.  Failed if not.");
            CoreLogger.EndVariation();

            return null;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Change the focus to the specified eleemnt
        /// </summary>
        private void ChangeFocus(UIElement element)
        {
            GlobalLog.LogStatus("Setting focus to the element....");
            bool isFocus = element.Focus();
            GlobalLog.LogStatus("Focus set via API?           " + (isFocus));
            GlobalLog.LogStatus("Focus set via InputManager?  " + (InputManager.Current.PrimaryKeyboardDevice.FocusedElement != null));
        }

        private void ChangeCommand(RoutedCommand routedCommand, FrameworkElement element)
        {
            RoutedCommand existingRoutedCommand = element.TryFindResource(_commandDynamicResourceId) as RoutedCommand;
            Assert(existingRoutedCommand != null, "Existing Command " + _commandDynamicResourceId + " could not be found on " + element.Name);
            string existingRoutedCommandName = existingRoutedCommand == null ? "Not Defined" : existingRoutedCommand.Name;
            GlobalLog.LogStatus("Changing DynamicResource to RoutedCommand from " + existingRoutedCommandName + " to " + routedCommand.Name);
            element.Resources[_commandDynamicResourceId] = routedCommand;
        }

        private void ChangeKey(Key key, FrameworkElement element)
        {
            object existingKey = element.TryFindResource(_keyDynamicResourceId);
            Assert(existingKey != null, "Existing Key " + _keyDynamicResourceId + " could not be found on " + element.Name);
            string existingKeyName = existingKey == null ? "Not Defined" : (((Key)(existingKey)).ToString());
            GlobalLog.LogStatus("Changing DynamicResource to Key from " + existingKeyName + " to " + key.ToString());
            element.Resources[_keyDynamicResourceId] = key;
        }

        /// <summary>
        /// Logs the fact the first command has been executed
        /// </summary>
        /// <param name="sender">Element that is the sender of the event.</param>
        /// <param name="e">Arguments pertaining to the command event.</param>
        private void CommandExecuted(List<CommandEventRecord> commandLog, CommandEventRecord commandEventRecord)
        {
            // We are executing a command
            commandLog.Add(commandEventRecord);

            GlobalLog.LogStatus("In CommandExecuted:");
            GlobalLog.LogStatus(" Command:            " + commandEventRecord.EventArguments.Command.ToString());
            if (commandEventRecord.Sender != null)
            {
                GlobalLog.LogStatus(" command sender Name: " + commandEventRecord.Sender.ToString());
            }
            if (commandEventRecord.EventArguments.Source != null)
            {
                GlobalLog.LogStatus(" command arg Source: " + commandEventRecord.EventArguments.Source.ToString());
            }
        }

        #endregion

        #region CommandRecord Class

        private class CommandEventRecord
        {
            public CommandEventRecord(object sender, ExecutedRoutedEventArgs e)
            {
                this.Sender = sender;
                this.EventArguments = e;
            }
            public ExecutedRoutedEventArgs EventArguments { get; private set; }
            public object Sender { get; private set; }
        }

        #endregion
    }
}
