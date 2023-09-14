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
using System.Drawing;
using System.ComponentModel;
using Microsoft.Test.Win32;
using Microsoft.Win32;
//using Microsoft.Test.Avalon.Input;
using Microsoft.Test.Threading;

using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWM = System.Windows.Media;

namespace Avalon.Test.CoreUI.Trusted.Controls.KeyboardInteropModelControls
{
    /// <summary>
    /// 
    /// </summary>
    public class WinForm : SWF.Form, IKeyboardInteropTestControl
      {
          //private ElementHost ctrlHost;
        private List<SWF.Button> _buttons = null;
        private Hashtable _accelHash;

        /// <summary>
        /// 
        /// </summary>
        public WinForm()
        {
            InitializeComponent();
        }

        System.Windows.Forms.Panel _panel1 = null;

        private void InitializeComponent()
        {
          if (_buttons == null)
              _buttons = new List<SWF.Button>();

          System.Windows.Forms.Button button1 = new System.Windows.Forms.Button();
          System.Windows.Forms.Button button2 = new System.Windows.Forms.Button();

          _buttons.Add(button1);
          _buttons.Add(button2);

          _panel1 = new System.Windows.Forms.Panel();

          this.SuspendLayout();

          // 
          // button1
          // 
          button1.Location = new System.Drawing.Point(0, 0);
          button1.Name = "button1";
          button1.Size = new System.Drawing.Size(400, 30);
          button1.TabIndex = 11;
          button1.Text = "Top WinForm button #1";
          button1.UseVisualStyleBackColor = true;
          button1.Click += new EventHandler(OnClick);
          button1.GotFocus += new EventHandler(OnGotFocus);
          this.Controls.Add(button1);

          // 
          // panel1
          // 
          _panel1.Location = new System.Drawing.Point(0, 33);
          _panel1.Name = "panel2";
          _panel1.Size = new System.Drawing.Size(400, 300);
          _panel1.BackColor = System.Drawing.Color.Red;
          _panel1.TabIndex = 15;
          this.Controls.Add(_panel1);


          // 
          // the other button1
          // 
          button2.Location = new System.Drawing.Point(0, 403);
          button2.Name = "button2";
          button2.Size = new System.Drawing.Size(400, 30);
          button2.TabIndex = 11;
          button2.Text = "Top WinForm button #1";
          button2.UseVisualStyleBackColor = true;
          button2.Click += new EventHandler(OnClick);
          button2.GotFocus += new EventHandler(OnGotFocus);
          this.Controls.Add(button2);

          this.Load += new System.EventHandler(AddChild);
          this.Width = 400;
          this.Height = 500;
          this.ResumeLayout(false);
          this.PerformLayout();

          InitializeAccelerators();
        }

        private ElementHost _ctrlHost;
        private AvPanel _av;

        /// <summary>
        /// 
        /// </summary>
        public IKeyboardInteropTestControl Child
        {
        get
        {
            return (IKeyboardInteropTestControl)_av;
        }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddChild(object sender, EventArgs e)
        {
          _ctrlHost = new ElementHost();
          _ctrlHost.Width = _panel1.Width;
          _ctrlHost.Height = _panel1.Height;
          _ctrlHost.BackColor = System.Drawing.Color.Red;
          _ctrlHost.Dock = SWF.DockStyle.Fill;

          _av = new AvPanel(_panel1.Width, _panel1.Height);
          _ctrlHost.Child = _av;

          _panel1.Controls.Add(_ctrlHost);
        }

        private void OnClick(object sender, EventArgs e)
        {
          _gotMnemonics++;

        }

        private void OnGotFocus(object sender, EventArgs e)
        {
          if (_bNeedRecord)
          {
              CoreLogger.LogStatus(sender + "get the focus");

              _tabTestState.RecordedTabStops++;

              if (_bFirstTab)
              {
                  SWF.Control c = sender as SWF.Control;
                  if (c != null)
                  {
                      _tabTestState.RecordedFirstTab = c.Handle;
                  }
                  _bFirstTab = false;
                  
              }
          }
        }

        /// <summary>
        /// 
        /// </summary>
        public class AcceleratorKey
        {
            private SWF.Keys _key = SWF.Keys.None;
            /// <summary>
            /// 
            /// </summary>
            public AcceleratorKey()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="key"></param>
            public AcceleratorKey(SWF.Keys key)
            {
              _key = key;
            }

            /// <summary>
            /// 
            /// </summary>
            public SWF.Keys Key
            {
              get { return _key; }
              set { _key = value; }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override Int32 GetHashCode()
            {
              return (Int32)_key;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public override bool Equals(Object obj)
            {
              if (obj.GetHashCode() == (Int32)_key) return true;

              return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected void InitializeAccelerators()
        {
            _accelHash = new Hashtable();

            // load accelerators 
            _accelHash.Add(NativeConstants.VK_A,
              AccelTestType.Common);
            _accelHash.Add(NativeConstants.VK_L,
              AccelTestType.Unique);
            _accelHash.Add(NativeConstants.VK_Q,
              AccelTestType.PFirst);
            _accelHash.Add(NativeConstants.VK_M,
              AccelTestType.Global);
            _accelHash.Add(NativeConstants.VK_I,
              AccelTestType.COverride);
            _accelHash.Add(NativeConstants.VK_J,
              AccelTestType.POverride);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref SWF.Message msg, SWF.Keys keyData)
        {
            bool bHandled = false;

            if (msg.Msg == NativeConstants.WM_KEYDOWN && NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0)
            {
                AccelTestType at = AccelTestType.Default;
                int key = msg.WParam.ToInt32();
                if (_accelHash.ContainsKey(key))
                {
                    at = (AccelTestType)_accelHash[key];
                    switch (at)
                    {
                        case AccelTestType.Common:
                            _accTestState.SumCommand++;
                            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
                            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_A) < 0);

                            bHandled = true;
                            break;
                        case AccelTestType.Unique:
                            _accTestState.SumCommand++;
                            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
                            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_L) < 0);
                            bHandled = true;
                            break;
                        case AccelTestType.Default:
                        default:
                            break;
                    }
                } // end if 
            }

            if (!bHandled)
                bHandled = base.ProcessCmdKey(ref msg, keyData);

            return bHandled;
        }

        #region IKeyboardInteropTestControl

        IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
        {
            throw new Exception("SourcedAvalon has been loaded");
        }

        AcceleratorTestState _accTestState;


        delegate AcceleratorTestState GetAcceleratorTestStateHandler();

        AcceleratorTestState GetAccTestState()
        {
            if (_expectedAccTestState.TestType == AccelTestType.Global)
            {
                System.Windows.Media.Color color = System.Windows.Media.Color.FromRgb(this.BackColor.R,
                    this.BackColor.B, this.BackColor.G);
                SolidColorBrush scb = new SolidColorBrush(color);
                _accTestState.BackColor = scb;
            }
            return _accTestState;
        }

        AcceleratorTestState IKeyboardInteropTestControl.RecordedAcceleratorTestState
        {
            get
            {
                if (InvokeRequired)
                {
                    return (AcceleratorTestState)Invoke(new GetAcceleratorTestStateHandler(
                        GetAccTestState));
                }
                return GetAccTestState();
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

        private delegate void AddMnemonicHandler(string letter);

        void FrmAddMnemonic(string letter)
        {
            string mneString;

            mneString = " mne: &" + letter;
            _buttons[0].Text += mneString;
        }

        void IKeyboardInteropTestControl.AddMnemonic(string letter, ModifierKeys modifier)
        {
            if (InvokeRequired)
            {
                Invoke(new AddMnemonicHandler(FrmAddMnemonic), letter);
            }
            else
            {
                FrmAddMnemonic(letter);                
            }
        }

        int IKeyboardInteropTestControl.RecordedMnemonics
        {
            get
            {
                return _gotMnemonics;
            }
        }

        delegate void SetFocusToFirstChildHandler(bool bFirst);

        void SetFocusToFirstChildInForm(bool bFirst)
        {
            if (_buttons != null && _buttons.Count >= 1)
            {
                if (bFirst == true)
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

        void IKeyboardInteropTestControl.SetFocusToFirstChild(bool bFirst)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new SetFocusToFirstChildHandler(SetFocusToFirstChildInForm),
                    bFirst);
            }
            else
            {
                SetFocusToFirstChildInForm(bFirst);
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

        #endregion 

        #region Trace to validate TAB

        private TabTestState _tabTestState;
        private TabTestState _expectedTabTestState;

        private delegate TabTestState GetTabTestStateHandler();

        private TabTestState GetTabTestState()
        {
            _expectedTabTestState.RecordedTabStops = _buttons.Count;
            if (!_bShiftTab)
            {
                _expectedTabTestState.RecordedFirstTab = _buttons[0].Handle;  // first tab stop
                _expectedTabTestState.RecordedLastTab = _buttons[_buttons.Count - 1].Handle;   // last tab stop
            }
            else
            {
                _expectedTabTestState.RecordedFirstTab = _buttons[_buttons.Count - 1].Handle;  // first tab stop
                _expectedTabTestState.RecordedLastTab = _buttons[0].Handle; // last tab stop 
            }

            return _expectedTabTestState;
        }

        TabTestState IKeyboardInteropTestControl.ExpectedTabTestState
        {
            get
            {
                if (InvokeRequired)
                {
                    return (TabTestState)Invoke(new GetTabTestStateHandler(GetTabTestState));
                }
                else
                    return GetTabTestState();
                
            }
            set
            {
                //_expectedTabTestState.TestType = ((TabTestState)value).TestType;
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

        void IKeyboardInteropTestControl.ResetTestState()
        {
            if (_accTestState.BackColor == System.Windows.Media.Brushes.Red)
            {
                this.BackColor = System.Drawing.SystemColors.GrayText;
            }

            _accTestState.ResetTestState();
            _expectedAccTestState.ResetTestState();

            _tabTestState.ResetTestState();

            _gotMnemonics = 0;
            _bFirstTab = true;
            _bShiftTab = false;
            _bNeedRecord = false;
        }


        #endregion

        #region Trace to validate mnemonics
        private int _gotMnemonics = 0;
        #endregion


      }


    /// <summary>
    /// to be used to integrate into WinForm, has less functions than sourcedAvalon
    /// </summary>
    public class AvPanel : StackPanel, IKeyboardInteropTestControl
    {

        #region Trace to validate accelerator

        private AcceleratorTestState _accTestState;

        AcceleratorTestState IKeyboardInteropTestControl.RecordedAcceleratorTestState
        {
          get
          {
              if (_expectedAccTestState.TestType == AccelTestType.Global)
              {
                  _accTestState.BackColor = this.Background;
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

        TabTestState IKeyboardInteropTestControl.ExpectedTabTestState
        {
          get
          {
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
        protected List<Button> _buttons = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public AvPanel(int width, int height)
        {
          this.Width = (double)width;
          this.Height = (double)height;

          // initial trace variables
          _accTestState = new AcceleratorTestState();
          _expectedTabTestState = new TabTestState();
          _tabTestState = new TabTestState();
          _expectedAccTestState = new ExpectedAccelTestState();

          this.Background = System.Windows.Media.Brushes.Yellow;

          Button b = new Button();
          _buttons = new List<Button>();
          _buttons.Add(b);

          b.Height = 30;
          b.Focusable = true;
          b.Content = "sourced Avalon Button #1";
          b.Click += new RoutedEventHandler(onClickButton);
          b.GotKeyboardFocus += this.OnGotFocus;
          //b.LostFocus += this.OnLostFocus;

          _expectedTabTestState.RecordedTabStops = 1;
          _expectedTabTestState.RecordedFirstTabElement = b;
          _expectedTabTestState.RecordedLastTabElement = b;

          this.Children.Add(b);

          InitAccelerators();

        }

        /// <summary>
        /// 
        /// </summary>
          public AvPanel()
          {
              // initial trace variables
              _accTestState = new AcceleratorTestState();
              _expectedTabTestState = new TabTestState();
              _tabTestState = new TabTestState();
              _expectedAccTestState = new ExpectedAccelTestState();

              this.Background = System.Windows.Media.Brushes.Yellow;

              Button b = new Button();
              _buttons = new List<Button>();
              _buttons.Add(b);

              b.Height = 30;
              b.Focusable = true;
              b.Content = "sourced Avalon Button #1";
              b.Click += new RoutedEventHandler(onClickButton);
              b.GotKeyboardFocus += this.OnGotFocus;
              //b.LostFocus += this.OnLostFocus;

              _expectedTabTestState.RecordedTabStops++;
              _expectedTabTestState.RecordedFirstTabElement = b;
              _expectedTabTestState.RecordedLastTabElement = b;

              this.Children.Add(b);

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
              this.CommandBindings.Add(commonCommandBinding);
              this.InputBindings.Add(commonKeyBinding);
          }

          private void OnCommonCommand(object sender, ExecutedRoutedEventArgs e)
          {
              _accTestState.SumCommand++;

              // keep the trace to be validated
              _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
              _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_A) < 0);
              _accTestState.RecordedCommand = (RoutedCommand)e.Command;

              CoreLogger.LogStatus("SourcedAvalon: OnCommonCommand");

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
              this.CommandBindings.Add(uniqueCommandBinding);
              this.InputBindings.Add(uniqueKeyBinding);
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

      

          #region IKeyboardInteropTestControl

          void IKeyboardInteropTestControl.ResetTestState()
          {
              _accTestState.ResetTestState();
              _expectedAccTestState.ResetTestState();
              if (_expectedAccTestState.TestType == AccelTestType.Global)
              {
                  this.Background = System.Windows.Media.Brushes.Yellow;
              }

              _tabTestState.ResetTestState();
              _bFirstTab = true;
              _gotMnemonics = 0;
              _bNeedRecord = false;
          }

          IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
          {
              return null;
          }

          /// <summary>
          /// Add a mnemonic to the control.
          /// </summary>
          void IKeyboardInteropTestControl.AddMnemonic(string letter, ModifierKeys modifier)
          {
              // Register mnemonic with first control.
              this.Dispatcher.BeginInvoke(DispatcherPriority.Send,
                  (DispatcherOperationCallback)delegate(object param)
                  {
                      string mne = (string)param;

                      AccessKeyManager.Register(mne, _buttons[0]);

                      return null;
                  }, letter);
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

              Dispatcher dispatcher = this.Dispatcher;

              dispatcher.BeginInvoke(DispatcherPriority.Send,
                  (DispatcherOperationCallback)delegate(object param)
                  {
                      bool bFirst = (bool)param;

                      if (_buttons != null && _buttons.Count >= 1)
                      {
                          if (first)
                              _buttons[0].Focus();
                          else
                              _buttons[_buttons.Count - 1].Focus();
                      }

                      return null;
                  }, first);
          }

          bool _bNeedRecord = false;
          #endregion
      }

}
