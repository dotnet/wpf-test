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

namespace Avalon.Test.CoreUI.Trusted.Controls.KeyboardInteropModelControls
{
    /// <summary>
    /// Top level win32 window.
    /// Implemented using Microsoft.Test.Win32.HwndWrapper.
    /// </summary>
    public class Win32Window : IKeyboardInteropTestControl
    {
#region controls

        // Top level Hwnd and WndProc.
        private IntPtr _hwnd = IntPtr.Zero;
        private HwndWrapper _hwndWrapper = null;
        private HwndWrapperHook _hook;

        // Hwnd buttons used for tab stops.
        private List<IntPtr> _buttons = null;
        // Button subclass (WndProc)
        private HwndSubclass _buttonSubclass = null;
        private HwndWrapperHook _buttonHwndSubclassHook;

#endregion 

        private List<IKeyboardInputSink> _childSinks = null;

        private AccelaratorTable _acceleratorTable = null;

        private List<int> _mnemonicList = null;

#region Trace to validate accelerators
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

#region Tab Trace
        private TabTestState _tabTestState,_expectedTabTestState;

        TabTestState IKeyboardInteropTestControl.ExpectedTabTestState
        {
            get
            {
                _expectedTabTestState.RecordedTabStops = _buttons.Count;
                if (!_bShiftTab)
                {
                    CoreLogger.LogStatus("Expected First tab: " + _buttons[0].ToString());
                    _expectedTabTestState.RecordedFirstTab = _buttons[0];  // first tab stop
                    _expectedTabTestState.RecordedLastTab = _buttons[_buttons.Count - 1];   // last tab stop
                }
                else
                {
                    CoreLogger.LogStatus("Expected Shift First tab: " + _buttons[_buttons.Count - 1].ToString());
                    _expectedTabTestState.RecordedFirstTab = _buttons[_buttons.Count - 1];  // first tab stop
                    _expectedTabTestState.RecordedLastTab = _buttons[0];   // last tab stop 
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
#endregion

#region mnemonic Trace
        private int _gotMnemonics = 0;
#endregion

        /// <summary>
        /// Constructor, if TabStops is true a button will be put in the window.
        /// </summary>
        public Win32Window()
        {
            // initial trace variables
            _accTestState = new AcceleratorTestState();
            _expectedTabTestState = new TabTestState();
            _tabTestState = new TabTestState();

            _hwndWrapper = new HwndWrapper(
                0,                              // classStyle 
                NativeConstants.WS_OVERLAPPEDWINDOW, // style
                0,                              // exStyle
                0,                              // x
                0,                              // y
                400,                            // width 
                500,                            // height
                "Top Level Hwnd Window",        // title
                IntPtr.Zero,                    // parent 
                null                            // hooks
                );
            _hwnd = _hwndWrapper.Handle;

            // Add my WndProc to the hwnd.
            _hook = new HwndWrapperHook(this.TopLevelHwndHook);
            _hwndWrapper.AddHook(_hook);

            // Connect to ComponentDispatcher.
            ComponentDispatcher.ThreadPreprocessMessage += new ThreadMessageEventHandler(this.OnPreProcessMessage);

            CoreLogger.LogStatus("Created top level hwnd " + _hwnd);

            // Show the hwnd and the buttons.
            NativeMethods.ShowWindow(new HandleRef(this, _hwndWrapper.Handle), NativeConstants.SW_SHOW);

            NativeMethods.SetForegroundWindow(new HandleRef(this, _hwndWrapper.Handle));

 
            _buttons = new List<IntPtr>();

            // Add hwnd button to the hwnd.
            IntPtr button = NativeMethods.CreateWindowEx(
                0,                              //dwExStyle
                "Button",                       // lpszClassName
                "top level Win32 button #1",    // lpszWindowName
                NativeConstants.WS_CHILD,       // style
                0,                              // x
                0,                              // y
                400,                            // width
                30,                             // height
                _hwndWrapper.Handle,             // hWndParent
                IntPtr.Zero,                    // hMenu
                IntPtr.Zero,                    // hInst
                null                            // pvParam
                );

            _buttons.Add(button);
            _buttonHwndSubclassHook = new HwndWrapperHook(buttonHelper);
            _buttonSubclass = new HwndSubclass(_buttonHwndSubclassHook);
            _buttonSubclass.Attach(button);

            NativeMethods.ShowWindow(new HandleRef(this, button), NativeConstants.SW_SHOW);

            CoreLogger.LogStatus("Created top hwnd button");


            InitAccelerators();

        }

        IntPtr buttonHelper(IntPtr hwnd, int msg, IntPtr wParam,
             IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeConstants.WM_SETFOCUS:
                    CoreLogger.LogStatus("Win32Window.buttonHelper: SETFOCUS message");
                    if (_bNeedRecord)
                    {
                        if (!_bRecordToken)
                        {
                            _bRecordToken = true;
                            if (_bFirstTab)
                            {
                                _tabTestState.RecordedFirstTab = hwnd;
                                _bFirstTab = false;
                                CoreLogger.LogStatus("Recorded First tab: " + hwnd.ToString()+"Shift: "+_bShiftTab.ToString());
                            }
                            CoreLogger.LogStatus("tab++............");
                            _tabTestState.RecordedTabStops++;
                        }
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        private bool _bRecordToken = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        public void SetTabTrace(IntPtr hwnd)
        {
            CoreLogger.LogStatus("SetTabTrace............");
   
            if (!_bRecordToken)
            {
                if (_bFirstTab)
                {
                    
                    _tabTestState.RecordedFirstTab = hwnd;
                    _bFirstTab = false;
                    CoreLogger.LogStatus("Recorded First tab: " + hwnd.ToString() + " shift: "+ _bShiftTab.ToString());
                }

                CoreLogger.LogStatus("tab++............");
                _tabTestState.RecordedTabStops++;
                _bRecordToken = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetRecordToken()
        {
            _bRecordToken = false;
        }

        private void InitAccelerators()
        {

            // Create accelerator table. IKIS.TranslateAccelerator will call NativeMethods.TranslateAccelerator which will
            // make use of this table.
            NativeStructs.ACCEL[] accelArray = new NativeStructs.ACCEL[6];
            NativeStructs.ACCEL accel;

            //
            // Unique accelerator for HostedHwndControl Ctrl-3
            //
            accel = new NativeStructs.ACCEL();
            accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
            accel.key = (short)(NativeMethods.VkKeyScan('W') & 0x00FF);
            accel.cmd = 0x00FF;

            accelArray[0] = accel;

            // Common accelerator Ctrl-O
            accel = new NativeStructs.ACCEL();
            accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
            accel.key = (short)(NativeMethods.VkKeyScan('A') & 0x00FF);
            accel.cmd = 0x0042;

            accelArray[1] = accel;

            // global accelerator Ctrl-m
            accel = new NativeStructs.ACCEL();
            accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
            accel.key = (short)(NativeMethods.VkKeyScan('M') & 0x00FF);
            accel.cmd = 0x0041;

            accelArray[2] = accel;

            // child overriden accelerator ctrl-i
            accel = new NativeStructs.ACCEL();
            accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
            accel.key = (short)(NativeMethods.VkKeyScan('I') & 0x00FF);
            accel.cmd = 0x0040;

            accelArray[3] = accel;

            // parent overriden accelerator ctrl-j
            accel = new NativeStructs.ACCEL();
            accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
            accel.key = (short)(NativeMethods.VkKeyScan('J') & 0x00FF);
            accel.cmd = 0x0055;

            accelArray[4] = accel;

            accel = new NativeStructs.ACCEL();
            accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
            accel.key = (short)(NativeMethods.VkKeyScan('Q') & 0x00FF);
            accel.cmd = 0x0056;

            accelArray[5] = accel;

            _acceleratorTable = new AccelaratorTable(accelArray);
            _acceleratorTable.Create();
        }

        private void InitMnemonics()
        {
            // Initialize mnemonics.
            // _mnemonicTable = new MnemonicsTable();
            _mnemonicList = new List<int>();
            //_mnemonicTable.Add((int)'n', ModifierKeys.Alt, myMnemonicEventHandler);
        }

        private bool OnUniqueCommand()
        {
            _accTestState.SumCommand++;

            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_W) < 0);

            return true;
        }

        private bool OnCommonCommand()
        {
            _accTestState.SumCommand++;

            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_A) < 0);

            return true;
        }

        private bool OnChildOverrideCommand()
        {
            _accTestState.Owner = this.GetType().ToString();

            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_I) < 0);

            return true;
        }

        private bool OnParentOverrideCommand()
        {
            _accTestState.SumCommand++;

            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_J) < 0);

            return true;
        }


        /// <summary>
        /// Top level hwnd WndProc
        /// </summary>
        private IntPtr TopLevelHwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            bool bHandled = false; 

            switch (msg)
            {
                case NativeConstants.WM_COMMAND:

                    // Mask notification code (high order word) of Accelerator or Menu message source.
                    int command = NativeMethods.IntPtrToInt32(wParam) & 0x0000FFFF;

                    // CoreLogger.LogStatus("Win32Window.TopLevelHwndHook WM_COMMAND " + command);
                    switch (command)
                    {
                        case 0x00FF:
                            //CoreLogger.LogStatus("Win32Window.TopLevelHwndHook got unique accelerator");
                            bHandled = OnUniqueCommand();
                            break;
                        case 0x0042:
                            //CoreLogger.LogStatus("Win32Window.TopLevelHwndHook got common accelerator");
                            bHandled = OnCommonCommand();
                            break;
                        case 0x0041:
                            //bHandled = OnGlobalCommand();
                            break;
                        case 0x0040:
                            bHandled = OnChildOverrideCommand();
                            break;
                        case 0x0055:
                            bHandled = OnParentOverrideCommand();
                            break;
                        case 0x0056:
                            // the accelerator first is processed by parent. 
                            _accTestState.SumCommand++;
                            _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
                            _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_Q) < 0);
                            bHandled = false;
                            break;
                        default:
                            break;
                    }

                    break;
                default:
                    // CoreLogger.LogStatus("TopLevelHwndHook " + msg);
                    break;
            }
 
            return IntPtr.Zero;
        }

        private bool TabOut(ref MSG msg)
        {
            IntPtr focusedHandle = NativeMethods.GetFocus();
            int index = _buttons.IndexOf(focusedHandle);
            bool bHandled = false;

            if (index >= 0)
            {
                FocusNavigationDirection tabDirection = FocusNavigationDirection.Next;
                if (NativeMethods.GetKeyState(NativeConstants.VK_SHIFT) < 0)
                {
                    tabDirection = FocusNavigationDirection.Previous;
                }

                // Foward tab, go to sink with same index unless this is the last button.
                if (tabDirection == FocusNavigationDirection.Next)
                {
                    // Loop around to the first button.
                    if (index == (_buttons.Count - 1))
                    {
                        NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));
                    }
                    else  // Tab into next sink.
                    {
                        ((IKeyboardInputSink)_childSinks[index]).TabInto(new TraversalRequest(FocusNavigationDirection.First));
                    }
                }
                else // shift-tab, go to sink with index - 1 unless this is the first button
                {
                    // Loop around to the last button.
                    if (index == 0)
                    {
                        NativeMethods.SetFocus(new HandleRef(null, _buttons[_buttons.Count - 1]));

                    }
                    else // Tab into previous sink.
                    {
                        ((IKeyboardInputSink)_childSinks[index - 1]).TabInto(new TraversalRequest(FocusNavigationDirection.Last));
                    }
                }

                // note that if focus isn't currently in any of the win32 _buttons[] we pass this message to the sink
                // with focus (if any) below.
                bHandled = true;
            }
            else if (_childSinks != null)   // pass accelarator to a child sink with focus.
            {
                foreach (IKeyboardInputSink childSink in _childSinks)
                {
                    if (childSink.HasFocusWithin() == true)
                    {
                        // todo: get modifier keys
                        ModifierKeys m = new ModifierKeys();
                        bHandled = childSink.TranslateAccelerator(ref msg, m);
                        break;
                    }
                }
            }

            return bHandled;
        }

        private bool OnMnemonics(ref MSG msg)
        {
            bool bHandled = false;

            if (_mnemonicList != null)
            {
                foreach (int mnemonicLetter in _mnemonicList)
                {
                    if ((NativeMethods.IntPtrToInt32(msg.wParam) == mnemonicLetter))
                    {
                        _gotMnemonics++;
                        bHandled = true;
                    }
                }
            }

            // Call OnMnemonic on children until one handles it.
            if (!bHandled && (_childSinks != null))
            {
                foreach (IKeyboardInputSink childSink in _childSinks)
                {
                    if (childSink.OnMnemonic(ref msg, ModifierKeys.Alt) == true)
                    {
                        bHandled = true;
                        break;
                    }
                }
            }

            return bHandled;
        }

        private bool CFirstProcessAccelerator(ref MSG msg)
        {
            bool bHandled = false;

            if (_childSinks != null)   // pass accelarator to a child sink with focus.
            {
                foreach (IKeyboardInputSink childSink in _childSinks)
                {
                    if (childSink.HasFocusWithin() == true)
                    {
                        // todo: get modifier keys
                        ModifierKeys m = new ModifierKeys();
                        bHandled = childSink.TranslateAccelerator(ref msg, m);
                        break;
                    }
                }
            }

            // Focus is in _buttons, give the message to the unmanaged TranslateAccelerator.
            if (!bHandled)
            {
                if (_acceleratorTable != null)
                {
                    bHandled = (0 != NativeMethods.TranslateAccelerator(new HandleRef(null, _hwnd), new HandleRef(null, _acceleratorTable.ACCELTable), ref msg));
                }
            }

            return bHandled;
 
        }

        private bool PFirstProcessAccelerator(ref MSG msg)
        {
            bool bHandled = false;

            if (msg.message == NativeConstants.WM_KEYDOWN && msg.wParam == new IntPtr(NativeConstants.VK_Q))
            {
                _accTestState.SumCommand++;
                _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
                _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_Q) < 0);

                if (_childSinks != null)   // pass accelarator to a child sink with focus.
                {
                    foreach (IKeyboardInputSink childSink in _childSinks)
                    {
                        if (childSink.HasFocusWithin() == true)
                        {
                            bHandled = childSink.TranslateAccelerator(ref msg, ModifierKeys.Control);
                            break;
                        }
                    }
                }

            }

            return bHandled;
        }

        /// <summary>
        /// Handler for messages from the ComponentDispatcher. 
        /// Distributes accelerators to child controls or child sinks depending on focus.
        /// </summary>
        private void OnPreProcessMessage(ref MSG msg, ref bool handled)
        {
            bool bHandled = false;

            switch (msg.message)
            {
                case NativeConstants.WM_KEYDOWN:
                    if (msg.wParam == new IntPtr(NativeConstants.VK_TAB))
                    {
                        if (_expectedTabTestState.TestType == TabTestType.Loop)
                        {
                            if (_childSinks != null)
                            {
                                foreach (IKeyboardInputSink child in _childSinks)
                                {
                                    ((Win32KeyboardInputSite)child.KeyboardInputSite).TabType = TabTestType.Loop;
                                }
                            }
                        }

                        bHandled = TabOut(ref msg);
                    }
                    else
                    {
                        if (_expectedAccTestState.TestType == AccelTestType.PFirst)
                        {
                            CoreLogger.LogStatus("Win32: Parent first");
                            bHandled = PFirstProcessAccelerator(ref msg);
                        }
                        else
                            bHandled = CFirstProcessAccelerator(ref msg);
                    }
                    break;
                case NativeConstants.WM_CHAR:
                case NativeConstants.WM_DEADCHAR:
                    // CoreLogger.LogStatus("Win32Window.OnPreprocessMessage: Char message");
                    break;
                case NativeConstants.WM_COMMAND:
                case NativeConstants.WM_SYSCOMMAND:
                    // CoreLogger.LogStatus("Win32Window.OnPreprocessMessage: Command message");
                    break;
                case NativeConstants.WM_SYSCHAR:
                case NativeConstants.WM_SYSDEADCHAR:
                    bHandled = OnMnemonics(ref msg);
                    break;

                default:
                    break;
            }

            handled = bHandled;
        }

#region IKeyboardInteropTestControl
        /// <summary>
        /// Tab testing helper. Set tab to first child in a control. 
        /// </summary>
        void IKeyboardInteropTestControl.SetFocusToFirstChild(bool first)
        {
            if (_buttons != null)
            {
                if (first)
                {
                    NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));
                    SetTabTrace(_buttons[0]);
                    ResetRecordToken();
                }
                else
                {
                    NativeMethods.SetFocus(new HandleRef(null, _buttons[_buttons.Count - 1]));
                    _bShiftTab = true;
                    SetTabTrace(_buttons[_buttons.Count - 1]);
                    ResetRecordToken();

                }
            }
        }

        bool _bNeedRecord = false;

        bool IKeyboardInteropTestControl.NeedRecord
        {
            set
            {
                _bNeedRecord = value;
            }
        }



        /// <summary>
        /// Reset recorded accelerators, mnemonics and tabs. 
        /// </summary>
        void IKeyboardInteropTestControl.ResetTestState()
        {
            _accTestState.ResetTestState();
            _expectedAccTestState.ResetTestState();
            if (_expectedTabTestState.TestType == TabTestType.Loop)
            {
                if (_childSinks != null)
                {
                    foreach (IKeyboardInputSink child in _childSinks)
                    {
                        ((Win32KeyboardInputSite)child.KeyboardInputSite).TabType = TabTestType.Default;
                    }
                }
            }
            _expectedTabTestState.TestType = TabTestType.Default;
            _tabTestState.ResetTestState();

            _bFirstTab = true;
            _bShiftTab = false;
            _bRecordToken = false;
            _gotMnemonics = 0;
            _bNeedRecord = false;
        }

        private IKeyboardInteropTestControl InsertHwndSource(ref int sourcePosition)
        {
            sourcePosition = 210;
            if (_childSinks == null)
            {
                sourcePosition = 30;
                _childSinks = new List<IKeyboardInputSink>();
            }


            HwndSourceParameters hwndParams = new HwndSourceParameters("Hwnd Sourced Avalon", 400, 150);

            hwndParams.ParentWindow = _hwnd;
            hwndParams.SetPosition(0, sourcePosition);
            hwndParams.WindowStyle = NativeConstants.WS_VISIBLE | NativeConstants.WS_CHILD;

            SourcedAvalon childHwndSource = new SourcedAvalon(hwndParams);

            childHwndSource.AddHook(new HwndSourceHook(ChildHwndSourceMsgFilter));

            _childSinks.Add((IKeyboardInputSink)childHwndSource);

            return (IKeyboardInteropTestControl)childHwndSource;

        }

        private IntPtr ChildHwndSourceMsgFilter(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = false;

            //CoreLogger.LogStatus("Win32 get the message for HwndSource");

            if (msg == NativeConstants.WM_COMMAND)
            {
                int command = NativeMethods.IntPtrToInt32(wParam) & 0x0000FFFF;
                switch (command)
                {
                    case 0x55:
                        CoreLogger.LogStatus("Win32 get the message for HwndSource");
                        OnParentOverrideCommand();
                        handled = true;
                        break;
                }
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// 
        /// </summary>
        IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
        {
            IKeyboardInteropTestControl child = null;

            if (childType == "HwndHost")
                return null;

            int sourcePosition = 0;

            switch (childType)
            {
                case "SourcedAvalon":
                    child = InsertHwndSource(ref sourcePosition);
                    break;
            }

            IntPtr nextHwnd = IntPtr.Zero;
            IntPtr prevHwnd = IntPtr.Zero;
            if (_buttons != null)
            {
                prevHwnd = _buttons[_buttons.Count - 1];
            }

            //
            // Create button for t/m/a testing.
            //

            IntPtr button = NativeMethods.CreateWindowEx(
                0,                          // dwExStyle
                "Button",                   // lpszClassName
                "top level Win32 button #" + (_buttons.Count + 1),  // lpszWindowName
                NativeConstants.WS_CHILD,   // style
                0,                          // x
                sourcePosition + 150,        // y
                400,                        // width
                30,                         // height
                _hwnd,                      // hWndParent
                IntPtr.Zero,                // hMenu
                IntPtr.Zero,                // hInst
                null                        // pvParam
                );


            _buttons.Add(button);

            NativeMethods.ShowWindow(new HandleRef(this, button), NativeConstants.SW_SHOW);

            _buttonHwndSubclassHook = new HwndWrapperHook(buttonHelper);
            _buttonSubclass = new HwndSubclass(_buttonHwndSubclassHook);
            _buttonSubclass.Attach(button);


            nextHwnd = button;

            // Create a keyboard input site for the HwndSource sink and connect it.
            // When there are no more tab stops in the child focus will move to the button hwnd.

            ((IKeyboardInputSink)child).KeyboardInputSite = new Win32KeyboardInputSite(prevHwnd, (IKeyboardInputSink)child, nextHwnd, this);


            return child;

        }

        /// <summary>
        /// Add a mnemonic to the control.
        /// </summary>
        void IKeyboardInteropTestControl.AddMnemonic(string letter, ModifierKeys modifier)
        {
            // Initialize mnemonics.

            if (_mnemonicList == null)
            {
                _mnemonicList = new List<int>();
            }

            _mnemonicList.Add((int)letter[0]);
        }

        int IKeyboardInteropTestControl.RecordedMnemonics
        {
            get
            {
                return _gotMnemonics;
            }
        }

#endregion

    }

}
