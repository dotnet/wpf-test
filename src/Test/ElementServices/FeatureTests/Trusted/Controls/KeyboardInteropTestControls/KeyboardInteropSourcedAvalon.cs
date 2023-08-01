// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using System.Collections;
using System.Windows.Interop;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Microsoft.Test.Win32;
using Microsoft.Win32;
//using Microsoft.Test.Avalon.Input;
using Microsoft.Test.Threading;
using System.Windows.Forms.Integration;

namespace Avalon.Test.CoreUI.Trusted.Controls.KeyboardInteropModelControls
{
    /// <summary>
    /// 
    /// </summary>
    public class SourcedAvalon : HwndSource, IKeyboardInteropTestControl
    {
       
#region Trace to validate accelerator
        private AcceleratorTestState _accTestState;

        AcceleratorTestState IKeyboardInteropTestControl.RecordedAcceleratorTestState
        {
            get
            {
                if (_expectedAccTestState.TestType == AccelTestType.Global)
                {
                    _accTestState.BackColor = _stackPanel.Background;
                }
                return _accTestState;
            }
        }

        private ExpectedAccelTestState _expectedAccTestState;

        ExpectedAccelTestState IKeyboardInteropTestControl.ExpectedAcceleratorTestState
        {
            set
            {
                _expectedAccTestState = value;
            }
        }

#endregion

#region Tab Trace
        private TabTestState _tabTestState, _expectedTabTestState;

        private bool _bFirstTab = true;
        private bool _bShiftTab = false;

        TabTestState IKeyboardInteropTestControl.ExpectedTabTestState
        {
            get
            {
                _expectedTabTestState.RecordedTabStops = _buttons.Count;
                if (!_bShiftTab)
                {
                    CoreLogger.LogStatus("Expected First Tab: " + _buttons[_buttons.Count - 1].ToString());

                    _expectedTabTestState.RecordedFirstTabElement = _buttons[0];  // first tab stop
                    _expectedTabTestState.RecordedLastTabElement = _buttons[_buttons.Count - 1];   // last tab stop
                }
                else
                {
                    CoreLogger.LogStatus("Expected Shift First Tab: " + _buttons[_buttons.Count - 1].ToString());

                    _expectedTabTestState.RecordedFirstTabElement = _buttons[_buttons.Count - 1];  // first tab stop
                    _expectedTabTestState.RecordedLastTabElement = _buttons[0];   // last tab stop 
                }

                return _expectedTabTestState;
            }
            set
            {
                _expectedTabTestState.TestType = ((TabTestState)value).TestType;
            }
        }

        TabTestState IKeyboardInteropTestControl.RecordedTabTestState
        {
            get
            {
                return _tabTestState;
            }
        }

#endregion

        private int _gotMnemonics = 0;

#region controls
        /// <summary>
        /// 
        /// </summary>
        public StackPanel _stackPanel;
        /// <summary>
        /// 
        /// </summary>
        protected List<Button> _buttons = null;



#endregion

        private List<IKeyboardInputSink> _childSinks = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public SourcedAvalon(HwndSourceParameters p) : base(p)
        {
            // initial trace variables
            _accTestState = new AcceleratorTestState();
            _expectedTabTestState = new TabTestState();
            _tabTestState = new TabTestState();
            _expectedAccTestState = new ExpectedAccelTestState();

            _stackPanel = new StackPanel();

            _stackPanel.Background = Brushes.Yellow;

           
            this.RootVisual = (Visual)_stackPanel;

            Button b = new Button();
            _buttons = new List<Button>();
            _buttons.Add(b);

            b.Height = 30;
            b.Focusable = true;
            b.Content = "sourced Avalon Button #1";
            b.Click += new RoutedEventHandler(onClickButton);
            b.GotKeyboardFocus += this.OnGotFocus;
            //b.LostFocus += this.OnLostFocus;


            _stackPanel.Children.Add(b);

            InitAccelerators();
            

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnGotFocus(object sender, RoutedEventArgs e)
        {
            CoreLogger.LogStatus(e.Source + "get the focus");

            if (_bNeedRecord)
            {
                _tabTestState.RecordedTabStops++;

                if (_bFirstTab)
                {
                    UIElement element = e.Source as UIElement;
                    _tabTestState.RecordedFirstTabElement = element;
                    _bFirstTab = false;
                    _bShiftTab = (NativeMethods.GetKeyState(NativeConstants.VK_SHIFT) < 0);
                    CoreLogger.LogStatus("FirstTab: " + element.ToString() + "bshiftTab: " + _bShiftTab.ToString());
                }
            }
        }

        bool IKeyboardInteropTestControl.NeedRecord
        {
            set
            {
                _bNeedRecord = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnLostFocus(object sender, RoutedEventArgs e)
        {
            CoreLogger.LogStatus(e.Source + "lost the focus");
        }

        // Handle AccessKey (mnemonic) on button.
        void onClickButton(object o, RoutedEventArgs args)
        {
            CoreLogger.LogStatus(args.Source + " onClickButton has been raised");
            _gotMnemonics++;
        }

        private void InitAccelerators()
        {
            LoadCommonAccelerators();
            LoadUniqueAccelerators();
            LoadChildOverrideAccelerators();
            LoadParentOverrideAccelerators();
            LoadParentFirstAccelerators();
        }

        private void LoadCommonAccelerators()
        {
            // 
            // Hookup common command/accelerator 
            // 

            // Create command
            RoutedCommand commonCommand = new RoutedCommand("CommonCommand", this.GetType(), null);

            // Create binding
            CommandBinding commonCommandBinding = new CommandBinding(commonCommand);

            // set up key binding to command
            KeyBinding commonKeyBinding = new KeyBinding(commonCommand, Key.A, ModifierKeys.Control);

            // setup key binding to command
            commonCommandBinding.Executed += new ExecutedRoutedEventHandler(OnCommonCommand);

            // Add Command binding to the control
            _stackPanel.CommandBindings.Add(commonCommandBinding);
            _stackPanel.InputBindings.Add(commonKeyBinding);
        }

        private void OnCommonCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _accTestState.SumCommand++;

            // keep the trace to be validated
            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_A) < 0);
            _accTestState.RecordedCommand = (RoutedCommand)e.Command;

            e.Handled = true;
        }

        private void LoadUniqueAccelerators()
        {
            // Create command.
            RoutedCommand uniqueCommand = new RoutedCommand("UniqueCommand", this.GetType(), null);

            // Create binding.
            CommandBinding uniqueCommandBinding = new CommandBinding(uniqueCommand);

            // Set up key binding to command.
            KeyBinding uniqueKeyBinding = new KeyBinding(uniqueCommand, Key.X, ModifierKeys.Control);

            // Attach event to command binding.
            uniqueCommandBinding.Executed += new ExecutedRoutedEventHandler(OnUniqueCommand);

            // Add command binding to the control.
            _stackPanel.CommandBindings.Add(uniqueCommandBinding);
            _stackPanel.InputBindings.Add(uniqueKeyBinding);
        }

        private void OnUniqueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _accTestState.SumCommand++;

            // keep the trace to be validated
            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_X) < 0);
            _accTestState.RecordedCommand = (RoutedCommand)e.Command;

            e.Handled = true;
        }

        private void LoadChildOverrideAccelerators()
        {
            RoutedCommand diffCommand = new RoutedCommand("differentCommand", this.GetType(), null);

            CommandBinding diffCommandBinding = new CommandBinding(diffCommand);

            KeyBinding diffKeyBinding = new KeyBinding(diffCommand, Key.I, ModifierKeys.Control);

            diffCommandBinding.Executed += new ExecutedRoutedEventHandler(OnChildOverrideCommand);

            _buttons[0].CommandBindings.Add(diffCommandBinding);
            _buttons[0].InputBindings.Add(diffKeyBinding);
        }

        private void OnChildOverrideCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _accTestState.Owner = this.GetType().ToString();

            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_I) < 0);
            
        }

        private void LoadParentOverrideAccelerators()
        {
            RoutedCommand pCommand = new RoutedCommand("ParentCommand", this.GetType(), null);

            CommandBinding pCommandBinding = new CommandBinding(pCommand);

            KeyBinding pKeyBinding = new KeyBinding(pCommand, Key.J, ModifierKeys.Control);

            pCommandBinding.Executed += new ExecutedRoutedEventHandler(OnParentOverrideCommand);

            _buttons[0].CommandBindings.Add(pCommandBinding);
            _buttons[0].InputBindings.Add(pKeyBinding);
        }

        private void OnParentOverrideCommand(object sender, ExecutedRoutedEventArgs e)
        {
            int msg = NativeConstants.WM_COMMAND;

            int cmd = 0x0055;

            NativeMethods.SendMessage(new HandleRef(null, this.Handle), msg, (IntPtr)cmd, IntPtr.Zero);            
        }

        private void LoadParentFirstAccelerators()
        {
            RoutedCommand pCommand = new RoutedCommand("PfCommand", this.GetType(), null);

            CommandBinding pCommandBinding = new CommandBinding(pCommand);

            KeyBinding pKeyBinding = new KeyBinding(pCommand, Key.Q, ModifierKeys.Control);

            pCommandBinding.Executed += new ExecutedRoutedEventHandler(OnParentFirstCommand);

            _stackPanel.CommandBindings.Add(pCommandBinding);
            _stackPanel.InputBindings.Add(pKeyBinding);
        }

        private void OnParentFirstCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _accTestState.SumCommand++;
            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_Q) < 0);

        }

#region IKeyboardInteropTestControl

        void IKeyboardInteropTestControl.ResetTestState()
        {
            _accTestState.ResetTestState();
            _expectedAccTestState.ResetTestState();
            _expectedTabTestState.TestType = TabTestType.Default;

            _tabTestState.ResetTestState();
             _bFirstTab = true;
             _bShiftTab = false;
            _gotMnemonics = 0;
            _bNeedRecord = false;
        }

        private IntPtr HostedHwndMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;

            if (msg == NativeConstants.WM_COMMAND)
            {
                int command = NativeMethods.IntPtrToInt32(wParam) & 0x0000FFFF;
                switch(command)
                {
                    case 0x55:
                        CoreLogger.LogStatus("Sourced Control get the message for HwndHostedControl");
                        _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL)<0);
                        _accTestState.KeyState =( NativeMethods.GetKeyState(NativeConstants.VK_J) < 0);
                        NativeMethods.SendMessage(new HandleRef(null, this.Handle), msg, wParam, lParam);
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }

        private IKeyboardInteropTestControl InsertHostedHwnd()
        {
            HostedHwndControl childHwndHost = new HostedHwndControl();
            childHwndHost.Focusable = true;
            childHwndHost.Height = 30;

            childHwndHost.MessageHook += new HwndSourceHook(HostedHwndMsgFilter);

            _stackPanel.Children.Add(childHwndHost);

            if (_childSinks == null)
                _childSinks = new List<IKeyboardInputSink>();

            _childSinks.Add((IKeyboardInputSink)childHwndHost);

            return childHwndHost as IKeyboardInteropTestControl;

        }

        IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
        {
            IKeyboardInteropTestControl childControl = null;

            switch (childType)
            {
                case "HostedHwnd":
                    childControl = InsertHostedHwnd(); 
                    break;
                default:
                    throw new Exception("not supported");
                    
            }

            // Add another Avalon Button
            Button b = new Button();
            _buttons.Add(b);

            b.Height = 30;
            b.Focusable = true;
            b.Content = "sourced Avalon Button #" + _buttons.Count;
            b.GotKeyboardFocus += this.OnGotFocus;
            //b.LostFocus += this.OnLostFocus;

            _stackPanel.Children.Add(b);

            return childControl;
        }

        /// <summary>
        /// Add a mnemonic to the control.
        /// </summary>
        void IKeyboardInteropTestControl.AddMnemonic(string letter, ModifierKeys modifier)
        {
            // Register mnemonic with first control.
            AccessKeyManager.Register(letter, _buttons[0]);

            
        }

        int IKeyboardInteropTestControl.RecordedMnemonics
        {
            get
            {
                return _gotMnemonics;
            }
        }


        void IKeyboardInteropTestControl.SetFocusToFirstChild(bool first)
        {
            if (_buttons != null && _buttons.Count >= 1)
            {
                if (first)
                {
                    _buttons[0].Focus();
                }
                else
                {
                    _buttons[_buttons.Count - 1].Focus();
                }
            }
        }

        bool _bNeedRecord = false;
  
#endregion
    }
}
