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
    public class HostedWinFormsControl : SWF.UserControl, IKeyboardInteropTestControl
    {
        private List<SWF.Button> _buttons = null;

        private Hashtable _accelHash;
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public HostedWinFormsControl(int width, int height)
        {
            if (_buttons == null)
                _buttons = new List<SWF.Button>();

            SWF.Button button = new SWF.Button();

            _buttons.Add(button);

            this.SuspendLayout();

            button.Location = new System.Drawing.Point(0, 0);
            button.Size = new System.Drawing.Size(width, 30);
            button.Text = "Hosted WinForm button #" + _buttons.Count;
            button.BackColor = System.Drawing.SystemColors.Control;
            button.Click += new EventHandler(OnClick);
            button.GotFocus += new EventHandler(OnGotFocus);
            this.Controls.Add(button);

            this.BackColor = System.Drawing.SystemColors.GrayText;
            this.TabIndex = 1;
            this.Size = new System.Drawing.Size(width, height);
            this.ResumeLayout(false);
            this.PerformLayout();

            InitializeAccelerators();
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
                    _bShiftTab = (NativeMethods.GetKeyState(NativeConstants.VK_SHIFT) < 0);
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
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
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
            throw new Exception("crossbow not supports nested level");
        }

        AcceleratorTestState _accTestState;

        AcceleratorTestState IKeyboardInteropTestControl.RecordedAcceleratorTestState
        {
            get
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
        }

        private ExpectedAccelTestState _expectedAccTestState;

        ExpectedAccelTestState IKeyboardInteropTestControl.ExpectedAcceleratorTestState
        {
            set
            {
                _expectedAccTestState = value;
            }
        }

        void IKeyboardInteropTestControl.AddMnemonic(string letter, ModifierKeys modifier)
        {
            string mneString;

            mneString = " mne: &" + letter;
            _buttons[0].Text += mneString;

        }

        int IKeyboardInteropTestControl.RecordedMnemonics
        {
            get
            {
                return _gotMnemonics;
            }
        }

        void IKeyboardInteropTestControl.SetFocusToFirstChild(bool bFirst)
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

        #endregion 

        #region Trace to validate TAB

        private TabTestState _tabTestState;
        private TabTestState _expectedTabTestState;

        TabTestState IKeyboardInteropTestControl.ExpectedTabTestState
        {
            get
            {
                _expectedTabTestState.RecordedTabStops = _buttons.Count;
                if (_bShiftTab)
                {
                    _expectedTabTestState.RecordedFirstTab = _buttons[0].Handle;  // first tab stop
                    _expectedTabTestState.RecordedLastTab = _buttons[_buttons.Count - 1].Handle;   // last tab stop
                }
                else
                {
                    _expectedTabTestState.RecordedFirstTab = _buttons[_buttons.Count - 1].Handle; // first tab stop
                    _expectedTabTestState.RecordedLastTab = _buttons[0].Handle;  // last tab stop
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

}

