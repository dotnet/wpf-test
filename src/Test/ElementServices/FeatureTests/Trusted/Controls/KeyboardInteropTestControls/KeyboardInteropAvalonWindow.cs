// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
using System.Windows.Forms.Integration;

using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;

using Microsoft.Test.Win32;
using Microsoft.Win32;
//using Microsoft.Test.Avalon.Input;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.Trusted.Controls.KeyboardInteropModelControls
{

  /// <summary/>
  public class AvalonWindow : IKeyboardInteropTestControl
  {
#region controls 
       private StackPanel _stackPanel;
       private List<Button> _buttons = null;
       private Window _window = null;
#endregion

#region Trace to validate Accelerator
       private AcceleratorTestState _accTestState;
       
       AcceleratorTestState IKeyboardInteropTestControl.RecordedAcceleratorTestState
        {
            get
            {
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

#region Trace to validate TAB
      private TabTestState _tabTestState;
      private TabTestState _expectedTabTestState;

      TabTestState IKeyboardInteropTestControl.ExpectedTabTestState
      {
          get
          {
              _expectedTabTestState.RecordedTabStops = _buttons.Count;
              if (!_bShiftTab)
              {
                  _expectedTabTestState.RecordedFirstTabElement = _buttons[0];  // first tab stop
                  _expectedTabTestState.RecordedLastTabElement = _buttons[_buttons.Count - 1];   // last tab stop
              }
              else
              {
                  _expectedTabTestState.RecordedFirstTabElement = _buttons[_buttons.Count-1];  // first tab stop
                  _expectedTabTestState.RecordedLastTabElement = _buttons[0];   // last tab stop 
              }

              CoreLogger.LogStatus("Expected Firsttab: " + _expectedTabTestState.RecordedFirstTabElement.ToString()
                        + "shift: " + _bShiftTab.ToString());

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

       private bool _bFirstTab = true;

       private bool _bShiftTab = false;
#endregion

#region Trace to validate mnemonics
       private int _gotMnemonics = 0;
#endregion

      private List<IKeyboardInputSink> _childSinks = null;

      /// <summary/>
      public AvalonWindow()
        {
             _accTestState = new AcceleratorTestState();
            _expectedTabTestState = new TabTestState();
            _tabTestState = new TabTestState();
            _expectedAccTestState = new ExpectedAccelTestState();

            _window = new Window();
            _window.Title = "Top Level Avalon Window";
            _window.Width = 400;
            _window.Height = 500;

            _stackPanel = new StackPanel();

            _window.Content = _stackPanel;

            Button b = new Button();
            _buttons = new List<Button>();
            _buttons.Add(b);

            b.Height = 30;
            b.Focusable = true;
            b.Content = "Top Avalon Button 1";
            b.Click += new RoutedEventHandler(onClickButton);
            b.GotKeyboardFocus += this.OnGotFocus;
            //b.LostFocus += this.OnLostFocus;

            _stackPanel.Children.Add(b);

            InitAccelerators();
  
            _window.Show();
            _window.Activate();

        }

        /// <summary/>
        public void OnGotFocus(object sender, RoutedEventArgs e)
        {
            // keep Tab trace
            if (_bNeedRecord)
            {
                CoreLogger.LogStatus(e.Source + "get the focus");

                _tabTestState.RecordedTabStops++;

                if (_bFirstTab)
                {
                    _tabTestState.RecordedFirstTabElement = e.Source as UIElement;
                    _bFirstTab = false;
                    //bShiftTab = (NativeMethods.GetKeyState(NativeConstants.VK_SHIFT) < 0);
                }
            }
        }

      private bool _bNeedRecord = false;

      bool IKeyboardInteropTestControl.NeedRecord
      {
          set
          {
              _bNeedRecord = value;
          }
      }

      private void OnLostFocus(object sender, RoutedEventArgs e)
      {
          CoreLogger.LogStatus(e.Source + "lost the focus");
      }

        /// <summary> Handle AccessKey (mnemonic) on button.
        /// </summary>
        public void onClickButton(object o, RoutedEventArgs args)
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
          LoadPFirstAccelerators();
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
            _window.CommandBindings.Add(commonCommandBinding);
            _window.InputBindings.Add(commonKeyBinding);

        }

        private void OnCommonCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _accTestState.SumCommand++;

            // keep the trace to be validated
            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_A) < 0);
            _accTestState.RecordedCommand = (RoutedCommand) e.Command;

            e.Handled = true;
        }

        /// <summary/>
        public void LoadUniqueAccelerators()
        {
            // Create command
            RoutedCommand uniqueCommand = new RoutedCommand("UniqueCommand", this.GetType(), null);

            // Create binding
            CommandBinding uniqueCommandBinding = new CommandBinding(uniqueCommand);

            // set up key binding to command
            KeyBinding uniqueKeyBinding = new KeyBinding(uniqueCommand, Key.Z, ModifierKeys.Control);

            // setup key binding to command
            uniqueCommandBinding.Executed += new ExecutedRoutedEventHandler(OnUniqueCommand);

            _window.CommandBindings.Add(uniqueCommandBinding);
            _window.InputBindings.Add(uniqueKeyBinding);
        }

        private void OnUniqueCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _accTestState.SumCommand++;

            // keep the trace to be validated
            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_Z) < 0);
            _accTestState.RecordedCommand = (RoutedCommand)e.Command;

            e.Handled = true;
        }

        private void LoadChildOverrideAccelerators()
        {
            RoutedCommand diffCommand = new RoutedCommand("ChildOverCommand", this.GetType(), null);

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
          RoutedCommand pCommand = new RoutedCommand("ParentOverrideCommand", this.GetType(), null);

          CommandBinding pCommandBinding = new CommandBinding(pCommand);

          KeyBinding pKeyBinding = new KeyBinding(pCommand, Key.J, ModifierKeys.Control);

          pCommandBinding.Executed += new ExecutedRoutedEventHandler(OnParentOverrideCommand);

          _buttons[0].CommandBindings.Add(pCommandBinding);
          _buttons[0].InputBindings.Add(pKeyBinding);
      }

      private void OnParentOverrideCommand(object sender, ExecutedRoutedEventArgs e)
      {
          //accTestState.Owner = this.GetType().ToString();
          _accTestState.SumCommand++;
          _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
          _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_J) < 0);
      }

      private void LoadPFirstAccelerators()
      {
          RoutedCommand pCommand = new RoutedCommand("PfCommand", this.GetType(), null);

          CommandBinding pCommandBinding = new CommandBinding(pCommand);

          KeyBinding pKeyBinding = new KeyBinding(pCommand, Key.Q, ModifierKeys.Control);

          pCommandBinding.Executed += new ExecutedRoutedEventHandler(OnPFirstCommand);

          _buttons[0].CommandBindings.Add(pCommandBinding);
          _buttons[0].InputBindings.Add(pKeyBinding);
      }

      private void OnPFirstCommand(object sender, ExecutedRoutedEventArgs e)
      {
          //accTestState.Owner = this.GetType().ToString();
          _accTestState.SumCommand++;
          _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
          _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_Q) < 0);

          e.Handled = false;
      }

        private IKeyboardInteropTestControl InsertHostedHwnd()
        {
            HostedHwndControl childHwndHost = new HostedHwndControl();
            childHwndHost.Focusable = true;
            childHwndHost.Height = 90;

            // parent override child hwndhost's accelerator
            childHwndHost.MessageHook += new HwndSourceHook(HostedHwndMsgFilter);

            _stackPanel.Children.Add(childHwndHost);

            if (_childSinks == null)
                _childSinks = new List<IKeyboardInputSink>();

            _childSinks.Add((IKeyboardInputSink)childHwndHost);

            return childHwndHost as IKeyboardInteropTestControl;

        }

        private IKeyboardInteropTestControl InsertWinForm()
        {
            WindowsFormsHost wfHost = new WindowsFormsHost();
      
            wfHost.Focusable = true;

            HostedWinFormsControl child = null;

            wfHost.Width = 400;
            wfHost.Height = 90;
            child = new HostedWinFormsControl((int)wfHost.Width, (int)wfHost.Height);

            wfHost.Child = child;
            _stackPanel.Children.Add(wfHost);

            if (_childSinks == null)
                _childSinks = new List<IKeyboardInputSink>();

            _childSinks.Add((IKeyboardInputSink)wfHost);

            return  child as IKeyboardInteropTestControl;
        }

#region IKeyboardInteropTestControl

        void IKeyboardInteropTestControl.ResetTestState()
        {
            _accTestState.ResetTestState();
            _expectedAccTestState.ResetTestState();

            _tabTestState.ResetTestState();

            _gotMnemonics = 0;
            _bFirstTab = true;
            _bShiftTab = false;
            _bNeedRecord = false;
        }

      private IntPtr HostedHwndMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
      {
          handled = false;

          if (msg == NativeConstants.WM_COMMAND)
          {
              int command = NativeMethods.IntPtrToInt32(wParam) & 0x0000FFFF;
              switch (command)
              {
                  case 0x55:
                      CoreLogger.LogStatus("Avalon window get the message for HwndHostedControl");
                      OnParentOverrideCommand(null, null);
                      break;
              }
          }

          return IntPtr.Zero;
      }

        IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
        {
            IKeyboardInteropTestControl childControl = null;

            switch (childType)
            {
                case "HostedHwnd":
                    childControl = InsertHostedHwnd();
                    break;
                case "WinForm":
                    childControl = InsertWinForm();
                    break;
                default:
                    break;
            }

            // Add another Avalon Button
            Button b = new Button();
            _buttons.Add(b);

            b.Height = 30;
            b.Focusable = true;
            b.Content = "Top Av Button " + _buttons.Count;

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
                if (first == true)
                {
                    if (_buttons[0].Focus() == false)
                    {
                        CoreLogger.LogStatus("unable to set focus to first child in Avalon window");
                    }
                }
                else
                {
                    if (_buttons[_buttons.Count - 1].Focus() == false)
                    {
                        CoreLogger.LogStatus("unable to set focus to last child in Avalon window");
                    }
                    _bShiftTab = true;
                }
            }
        }

#endregion
    }

}
