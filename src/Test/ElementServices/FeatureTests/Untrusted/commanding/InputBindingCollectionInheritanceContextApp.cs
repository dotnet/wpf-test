// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Verify that an input binding collection passes through the data context by
    /// using data binding on a KeyBinding to the CommandParameter
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>pdanino</author>
 
    [Test(1, "Commanding.CoreCommanding", TestCaseSecurityLevel.FullTrust, "InputBindingCollectionInheritanceContextApp", Versions = "4.0+,4.0Client+")]
    public class InputBindingCollectionInheritanceContextApp : TestApp
    {
        #region Private Data

        private string _rootKeyCommandParameter = "RootKey";
        private string _rootMouseCommandParameter = "RootMouse";
        private string _localCommandParameter = "Local";
        private Button _buttonWithoutKeyBinding;
        private Button _buttonWithKeyBinding;
        private Button _buttonWithDataContextKeyBinding;
        private Label _label;
        private StackPanel _stackPanel;
        private DataContext<string> _rootDataContext;
        private DataContext<string> _localDataContext;
        KeyBinding _rootKeyBinding;
        MouseBinding _rootMouseBinding;

        // Store the results of the commands' parameters after they are called
        // Used for verification later
        private List<string> _rootCommandParameters = new List<string>();
        private List<string> _localCommandParameters = new List<string>();

        #endregion


        #region Constructor

        public InputBindingCollectionInheritanceContextApp()
        {
            GlobalLog.LogStatus("In InputBindingCollectionInheritanceContextApp constructor");
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps

        TestResult StartTest()
        {
            ExeStubContainerCore exe = new ExeStubContainerCore();
            TestApp app = new InputBindingCollectionInheritanceContextApp();
            exe.Run(app, "RunTestApp");

            //Any test failure will be caught by an Assert during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public and Protected Members

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender)
        {
            // Construct test element
            _buttonWithoutKeyBinding = new Button();
            _buttonWithoutKeyBinding.Name = "ButtonNoKeyBinding";
            _buttonWithoutKeyBinding.Content = _buttonWithoutKeyBinding.Name;

            _buttonWithKeyBinding = new Button();
            _buttonWithKeyBinding.Name = "ButtonWithKeyBinding";
            _buttonWithKeyBinding.Content = _buttonWithKeyBinding.Name;

            _buttonWithDataContextKeyBinding = new Button();
            _buttonWithDataContextKeyBinding.Name = "ButtonWithDataContextKeyBinding";
            _buttonWithDataContextKeyBinding.Content = _buttonWithDataContextKeyBinding.Name;

            _label = new Label();
            _label.Name = "LabelForMouseTesting";
            _label.Content = _label.Name;

            _stackPanel = new StackPanel();
            _stackPanel.Name = "RootPanel";
            _stackPanel.Height = 400;
            _stackPanel.Width = 90;
            _stackPanel.Children.Add(_buttonWithoutKeyBinding);
            _stackPanel.Children.Add(_buttonWithKeyBinding);
            _stackPanel.Children.Add(_buttonWithDataContextKeyBinding);
            _stackPanel.Children.Add(_label);

            //Set properties on the controls
            _rootDataContext = new DataContext<string>(RootDataContextCommandExecuted, _rootKeyCommandParameter, _rootMouseCommandParameter);
            SetElementProperties(_stackPanel, true, true, _rootDataContext, ref _rootKeyBinding, ref _rootMouseBinding);
            KeyBinding tempKeyBinding = null;
            MouseBinding tempMouseBinding = null;
            SetElementProperties(_buttonWithoutKeyBinding, false, false, null, ref tempKeyBinding, ref tempMouseBinding);
            SetElementProperties(_buttonWithKeyBinding, true, false, null, ref tempKeyBinding, ref tempMouseBinding);
            _localDataContext = new DataContext<string>(LocalDataContextCommandExecuted, _localCommandParameter, _localCommandParameter);
            SetElementProperties(_buttonWithDataContextKeyBinding, true, false, _localDataContext, ref tempKeyBinding, ref tempMouseBinding);

            // Put the test element on the screen
            _rootElement = _stackPanel;
            DisplayMe(_rootElement,10, 10, 100, 500);

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
                // Ensure focus is on button
                delegate
                {
                    ChangeFocus(_buttonWithoutKeyBinding);
                },
                // Try to invoke root key binding's command
                delegate
                {
                    KeyboardHelper.TypeKey(Key.A, ModifierKeys.Control);
                },
                // change focus to the button with key binding but no data context
                delegate
                {
                    ChangeFocus(_buttonWithKeyBinding);
                },
                // Try to invoke button key binding's command
                delegate
                {
                    KeyboardHelper.TypeKey(Key.A, ModifierKeys.Control);
                },
                // change focus to the button with key binding and data context
                delegate
                {
                    ChangeFocus(_buttonWithDataContextKeyBinding);
                },
                // Try to invoke button key binding's command
                delegate
                {
                    KeyboardHelper.TypeKey(Key.A, ModifierKeys.Control);
                },
                //change key and modifiers on root datacontext
                delegate
                {
                    ChangeKeyAndModifiers(false, false);
                    ChangeFocus(_buttonWithoutKeyBinding);
                },
                // Try to invoke root key binding with modified key and modifiers
                delegate
                {
                    KeyboardHelper.TypeKey(Key.B, ModifierKeys.Alt);
                },
                // change gesture on root keybinding
                delegate
                {
                    ChangeKeyGesture(_rootKeyBinding);
                    ChangeFocus(_buttonWithoutKeyBinding);
                },
                // Try to invoke root key binding with modified gesture
                delegate
                {
                    KeyboardHelper.TypeKey(Key.C, ModifierKeys.Alt);
                },
                // Try to invoke rook mouse binding with mouse
                delegate
                {
                    MouseHelper.Click(MouseButton.Left, _label, 2);
                },
                // change mouse action on root datacontext
                delegate
                {
                    ChangedMouseAction(false, false);
                },
                // try to invoke root mouse binding with  modified mouse action
                delegate
                {
                    MouseHelper.Click(MouseButton.Right, _label, 2);
                },
                // change gesture on root mouse binding
                delegate
                {
                    ChangedMouseGesture(_rootMouseBinding);
                },
                // try to invoke root mouse binding with modified gesture
                delegate
                {
                    MouseHelper.Click(MouseButton.Middle, _label, 1);
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
            CoreLogger.BeginVariation("InputBindingCollectionInheritanceContextApp");
            GlobalLog.LogStatus("Validating...");

            // We need to ensure that both the local and root contexts were resolved correctly.
            // The root context should be used by the button with no local key bindings set

            GlobalLog.LogStatus("Root commands executed: (expect 7): " + _rootCommandParameters.Count);
            GlobalLog.LogStatus("Local commands executed: (expect 1): " + _localCommandParameters.Count);

            bool testPassed = true;

            if (_rootCommandParameters.Count != 7 || _localCommandParameters.Count != 1)
            {
                testPassed &= false;
                GlobalLog.LogStatus("Wrong number of commands executed");
            }
            else
            {
                testPassed &= ValidateCommandParameter(_rootCommandParameters, 0, _rootKeyCommandParameter);
                testPassed &= ValidateCommandParameter(_rootCommandParameters, 1, _rootKeyCommandParameter);
                testPassed &= ValidateCommandParameter(_rootCommandParameters, 2, _rootKeyCommandParameter);
                testPassed &= ValidateCommandParameter(_rootCommandParameters, 3, _rootKeyCommandParameter);
                testPassed &= ValidateCommandParameter(_rootCommandParameters, 4, _rootMouseCommandParameter);
                testPassed &= ValidateCommandParameter(_rootCommandParameters, 5, _rootMouseCommandParameter);
                testPassed &= ValidateCommandParameter(_rootCommandParameters, 6, _rootMouseCommandParameter);

                testPassed &= ValidateCommandParameter(_localCommandParameters, 0, _localCommandParameter);
            }

            testPassed &= ValidateKeyBinding();
            testPassed &= ValidateMouseBinding();

            GlobalLog.LogStatus("Setting log result to " + testPassed);
            this.TestPassed = testPassed;

            GlobalLog.LogStatus("Validation complete!");
            CoreLogger.LogTestResult(this.TestPassed, "Passed if event found.  Failed if not.");
            CoreLogger.EndVariation();

            return null;
        }

        #endregion


        #region Private Members

        // Sets up key binding and data context on a framework element
        private void SetElementProperties(FrameworkElement element, bool createKeyBinding, bool createMouseBinding, object dataContext, ref KeyBinding keyBinding, ref MouseBinding mouseBinding)
        {
            GlobalLog.LogStatus("Setting KeyBinding properties: " + element.Name);

            if (createKeyBinding)
            {
                // Create key bindings
                keyBinding = new KeyBinding();
                Binding binding = new Binding();
                binding.Path = new PropertyPath("CommandKey");
                binding.Mode = BindingMode.TwoWay;
                PresentationTraceSources.SetTraceLevel(binding, PresentationTraceLevel.High);
                BindingOperations.SetBinding(keyBinding, KeyBinding.KeyProperty, binding);
                GlobalLog.LogStatus(" Set binding for Key property");

                binding = new Binding();
                binding.Path = new PropertyPath("CommandModifiers");
                binding.Mode = BindingMode.TwoWay;
                PresentationTraceSources.SetTraceLevel(binding, PresentationTraceLevel.High);
                BindingOperations.SetBinding(keyBinding, KeyBinding.ModifiersProperty, binding);
                GlobalLog.LogStatus(" Set binding for Modifiers property");

                binding = new Binding();
                binding.Path = new PropertyPath("Command");
                PresentationTraceSources.SetTraceLevel(binding, PresentationTraceLevel.High);
                BindingOperations.SetBinding(keyBinding, InputBinding.CommandProperty, binding);
                GlobalLog.LogStatus("  Set binding for Command property");
                binding = new Binding();
                binding.Path = new PropertyPath("KeyCommandParameter");
                PresentationTraceSources.SetTraceLevel(binding, PresentationTraceLevel.High);
                BindingOperations.SetBinding(keyBinding, InputBinding.CommandParameterProperty, binding);
                GlobalLog.LogStatus("  Set binding for CommandParameter property");

                element.InputBindings.Add(keyBinding);
                GlobalLog.LogStatus("  Key binding added to InputBindings collection");
            }
            else
            {
                GlobalLog.LogStatus("  No key bindings added");
            }
            if (createMouseBinding)
            {
                mouseBinding = new MouseBinding();
                Binding binding = new Binding();
                binding.Path = new PropertyPath("CommandMouseAction");
                binding.Mode = BindingMode.TwoWay;
                PresentationTraceSources.SetTraceLevel(binding, PresentationTraceLevel.High);
                BindingOperations.SetBinding(mouseBinding, MouseBinding.MouseActionProperty, binding);
                GlobalLog.LogStatus(" Set binding for MouseAction property");

                binding = new Binding();
                binding.Path = new PropertyPath("Command");
                PresentationTraceSources.SetTraceLevel(binding, PresentationTraceLevel.High);
                BindingOperations.SetBinding(mouseBinding, InputBinding.CommandProperty, binding);
                GlobalLog.LogStatus("  Set binding for Command property");
                binding = new Binding();
                binding.Path = new PropertyPath("MouseCommandParameter");
                PresentationTraceSources.SetTraceLevel(binding, PresentationTraceLevel.High);
                BindingOperations.SetBinding(mouseBinding, InputBinding.CommandParameterProperty, binding);
                GlobalLog.LogStatus("  Set binding for CommandParameter property");

                element.InputBindings.Add(mouseBinding);
                GlobalLog.LogStatus("  Mouse binding added to InputBindings collection");
            }
            if (dataContext != null)
            {
                element.DataContext = dataContext;
                GlobalLog.LogStatus("Set DataContext");
            }
            else
            {
                GlobalLog.LogStatus("  No data context set");
            }
        }

        // Change the focus to the element passed as argument
        private void ChangeFocus(UIElement element)
        {
            GlobalLog.LogStatus("Setting focus to the element....");
            bool isFocus = element.Focus();
            GlobalLog.LogStatus("Focus set via API?           " + (isFocus));
            GlobalLog.LogStatus("Focus set via InputManager?  " + (InputManager.Current.PrimaryKeyboardDevice.FocusedElement != null));
        }

        // Changes key and modifiers on the datacontext
        private void ChangeKeyAndModifiers(bool isLocal, bool reset)
        {
            DataContext<string> dataContext = null;
            if (isLocal)
            {
                dataContext = _localDataContext;
            }
            else
            {
                dataContext = _rootDataContext;
            }
            if (dataContext != null)
            {
                if (reset)
                {
                    dataContext.CommandKey = Key.A;
                    dataContext.CommandModifiers = ModifierKeys.Control;
                }
                else
                {
                    dataContext.CommandKey = Key.B;
                    dataContext.CommandModifiers = ModifierKeys.Alt;
                }
                GlobalLog.LogStatus("Changed Key and Modifiers on datacontext");
            }
        }

        // changes gesture on root key binding
        private void ChangeKeyGesture(KeyBinding keyBinding)
        {
            keyBinding.Gesture = new KeyGesture(Key.C, ModifierKeys.Alt);
            GlobalLog.LogStatus("Changed gesture on root key binding");
        }

        // changes mouse action on datacontext
        private void ChangedMouseAction(bool isLocal, bool reset)
        {
            DataContext<string> dataContext = null;
            if (isLocal)
            {
                dataContext = _localDataContext;
            }
            else
            {
                dataContext = _rootDataContext;
            }
            if(dataContext != null)
            {
                if (reset)
                {
                    dataContext.CommandMouseAction = MouseAction.LeftDoubleClick;
                }
                else
                {
                    dataContext.CommandMouseAction = MouseAction.RightDoubleClick;
                }
                GlobalLog.LogStatus("Changed MouseAction on datacontext");
            }
        }

        // changes gesture on root mouse binding
        private void ChangedMouseGesture(MouseBinding mouseBinding)
        {
            mouseBinding.Gesture = new MouseGesture(MouseAction.MiddleClick);
            GlobalLog.LogStatus("Changed gesture on root mouse binding");
        }

        // Callback executed when element's command is invoked
        private void RootDataContextCommandExecuted(string parameter)
        {
            GlobalLog.LogStatus("Root key binding command executed");
            this.Assert(!String.IsNullOrEmpty(parameter), "Root key binding command parameter should not be null or empty");

            GlobalLog.LogStatus("Root key binding command parameter:" + parameter);
            _rootCommandParameters.Add(parameter);
        }

        // Callback executed when element's command is invoked
        private void LocalDataContextCommandExecuted(string parameter)
        {
            GlobalLog.LogStatus("Local key binding command executed");
            this.Assert(!String.IsNullOrEmpty(parameter), "Local key binding command parameter should not be null or empty");

            GlobalLog.LogStatus("Local key binding command parameter:" + parameter);
            _localCommandParameters.Add(parameter);
        }

        // Validates if the parameter at given index is valid
        private bool ValidateCommandParameter(List<string> parameters, int index, string parameter)
        {
            if (!string.Equals(parameters[index], parameter))
            {
                GlobalLog.LogStatus(string.Format("Command executed with wrong parameter: Index - {0}, ExpectedParameter - {1}, ResultParameter - {2}",
                    index,
                    parameter,
                    parameters[index]));
                return false;
            }
            return true;
        }

        // Validates the key binding
        private bool ValidateKeyBinding()
        {
            if (_rootKeyBinding.Key != ((KeyGesture)(_rootKeyBinding.Gesture)).Key ||
                _rootKeyBinding.Modifiers != ((KeyGesture)(_rootKeyBinding.Gesture)).Modifiers)
            {
                GlobalLog.LogStatus("Root Key binding's Key and Modifiers are not in sync with its Gesture");
                return false;
            }

            if (_rootKeyBinding.Key != _rootDataContext.CommandKey ||
                _rootKeyBinding.Modifiers != _rootDataContext.CommandModifiers)
            {
                GlobalLog.LogStatus("Root Key binding's Key and Modifiers are not in sycn with the DataContext");
                return false;
            }
            return true;
        }

        // Validates the mouse binding
        private bool ValidateMouseBinding()
        {
            if (_rootMouseBinding.MouseAction != ((MouseGesture)(_rootMouseBinding.Gesture)).MouseAction)
            {
                GlobalLog.LogStatus("Root Mouse Binding's MouseAction is not in sync with its Gesture");
                return false;
            }

            if (_rootMouseBinding.MouseAction != _rootDataContext.CommandMouseAction)
            {
                GlobalLog.LogStatus("Root Mouse Binding's MouseAction is not in sycn with the DataContext");
                return false;
            }

            return true;
        }

        #endregion

        #region DataContext Class

        // This class is assigned to an element's DataContext property
        // Data binding is then used to invoke the class' Command
        // Class would be private if it were not for the WPF data binding engine's need to have access
        internal class DataContext<U> : INotifyPropertyChanged
        {
            private string _keyCommandParameter;
            private string _mouseCommandParameter;
            private ICommand _command;
            private Key _key = Key.A;
            private ModifierKeys _modifiers = ModifierKeys.Control;
            private MouseAction _mouseAction = MouseAction.LeftDoubleClick;

            public DataContext(Action<U> executeMethod, string keyCommandParameter, string mouseCommandParameter)
            {
                this._command = new DelegateCommand<U>(executeMethod);
                this._keyCommandParameter = keyCommandParameter;
                this._mouseCommandParameter = mouseCommandParameter;
            }

            public ICommand Command
            {
                get { return _command; }
            }

            public string KeyCommandParameter
            {
                get { return _keyCommandParameter; }
            }

            public string MouseCommandParameter
            {
                get { return _mouseCommandParameter; }
            }

            public Key CommandKey
            {
                get
                {
                    return _key;
                }
                set
                {
                    if (_key != value)
                    {
                        _key = value;
                        OnPropertyChanged("CommandKey");
                    }
                }
            }

            public ModifierKeys CommandModifiers
            {
                get
                {
                    return _modifiers;
                }
                set
                {
                    if (_modifiers != value)
                    {
                        _modifiers = value;
                        OnPropertyChanged("CommandModifiers");
                    }
                }
            }

            public MouseAction CommandMouseAction
            {
                get
                {
                    return _mouseAction;
                }
                set
                {
                    if (value != _mouseAction)
                    {
                        _mouseAction = value;
                        OnPropertyChanged("CommandMouseAction");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }

            #region DelegateCommand Class
            /// <summary>
            ///     This class allows delegating the commanding logic to methods passed as parameters,
            ///     and enables a View to bind commands to objects that are not part of the element tree.
            /// </summary>
            /// <typeparam name="T">Type of the parameter passed to the delegates</typeparam>
            private class DelegateCommand<T> : ICommand
            {
                #region Constructors

                /// <summary>
                ///     Constructor
                /// </summary>
                public DelegateCommand(Action<T> executeMethod)
                    : this(executeMethod, null)
                {
                }

                /// <summary>
                ///     Constructor
                /// </summary>
                public DelegateCommand(System.Action<T> executeMethod, System.Func<T, bool> canExecuteMethod)
                {
                    if (executeMethod == null)
                    {
                        throw new ArgumentNullException("executeMethod");
                    }

                    _executeMethod = executeMethod;
                    _canExecuteMethod = canExecuteMethod;
                }

                #endregion

                #region Public Methods

                /// <summary>
                ///     Method to determine if the command can be executed
                /// </summary>
                public bool CanExecute(T parameter)
                {
                    if (_canExecuteMethod != null)
                    {
                        return _canExecuteMethod(parameter);
                    }
                    return true;
                }

                /// <summary>
                ///     Execution of the command
                /// </summary>
                public void Execute(T parameter)
                {
                    if (_executeMethod != null)
                    {
                        _executeMethod(parameter);
                    }
                }

                #endregion

                #region ICommand Members

                event EventHandler ICommand.CanExecuteChanged
                {
                    add { CommandManager.RequerySuggested += value; }
                    remove { CommandManager.RequerySuggested -= value; }
                }

                bool ICommand.CanExecute(object parameter)
                {
                    // if T is of value type and the parameter is not
                    // set yet, then return false if CanExecute delegate
                    // exists, else return true
                    if (parameter == null && typeof(T).IsValueType)
                    {
                        return (_canExecuteMethod == null);
                    }
                    return CanExecute((T)parameter);
                }

                void ICommand.Execute(object parameter)
                {
                    Execute((T)parameter);
                }

                #endregion

                #region Data

                private readonly System.Action<T> _executeMethod = null;
                private readonly System.Func<T, bool> _canExecuteMethod = null;

                #endregion
            }
            #endregion
        }

        #endregion
    }
}
