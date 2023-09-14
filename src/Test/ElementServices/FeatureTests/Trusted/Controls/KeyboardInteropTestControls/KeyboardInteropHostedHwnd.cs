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
    /// 
    /// </summary>
    public class HostedHwndControl : HwndHost, IKeyboardInputSink, IKeyboardInteropTestControl
    {
        bool _bNeedRecord = false;

        #region controls

        private IntPtr _hwnd = IntPtr.Zero;
        // Button subclass (WndProc)
        private HwndSubclass _buttonSubclass = null;
        private HwndWrapperHook _buttonHwndSubclassHook;

        #endregion

  
        #region Accelerator trace

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

        private bool _bFirstTab = true;
        private bool _bShiftTab = false;

        TabTestState IKeyboardInteropTestControl.ExpectedTabTestState
        {
           get
           {
               _expectedTabTestState.RecordedTabStops = _buttons.Count;
               if (!_bShiftTab)
               {
                   _expectedTabTestState.RecordedFirstTab = _buttons[0];        // first tab stop
                   _expectedTabTestState.RecordedLastTab = _buttons[_buttons.Count-1];         // last tab stop
               }
               else
               {
                   _expectedTabTestState.RecordedFirstTab = _buttons[_buttons.Count-1];        // first tab stop
                   _expectedTabTestState.RecordedLastTab = _buttons[0];         // last tab stop
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

        #region Mnemonic Trace

        private int _gotMnemonics = 0;

        #endregion

        private List<IKeyboardInputSink> _childSinks = null;
        private List<int> _mnemonicList = null;
        private SourcedAvalon _childHwndSource;

        /// <summary>
        /// 
        /// </summary>
        public HostedHwndControl()
        {
            // initial trace variables
            _accTestState = new AcceleratorTestState();
            _expectedTabTestState = new TabTestState();
            _tabTestState = new TabTestState();


            _expectedAccTestState = new ExpectedAccelTestState();

            HwndWrapper hwndWrapper = new HwndWrapper(
                   0,
                   NativeConstants.WS_CHILD,
                   0,
                   0,
                   0,
                   400,
                   150,
                   "HostedHwnd",
                   NativeConstants.HWND_MESSAGE,
                   null
                   );

            _hwnd = hwndWrapper.Handle;

            InitAccelerators();
            InitMnemonics();
            
        }

        private AccelaratorTable _acceleratorTable = null;

        private void InitAccelerators()
        {
           // Create accelerator table. IKIS.TranslateAccelerator will call NativeMethods.TranslateAccelerator which will
           // make use of this table.
           NativeStructs.ACCEL[] accelArray = new NativeStructs.ACCEL[2];
           NativeStructs.ACCEL accel;

           //
           //  accelerator for HostedHwndControl Ctrl-I: parent and child control deal with it in different ways
           //
           accel = new NativeStructs.ACCEL();
           accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
           accel.key = (short)(NativeMethods.VkKeyScan('I') & 0x00FF);
           accel.cmd = 0x0030;

           accelArray[0] = accel;


           // Ctrl-J
           accel = new NativeStructs.ACCEL();
           accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
           accel.key = (short)(NativeMethods.VkKeyScan('J') & 0x00FF);
           accel.cmd = 0x0055;

           accelArray[1] = accel;

           _acceleratorTable = new AccelaratorTable(accelArray);
           _acceleratorTable.Create();

        }

        private bool OnChildOverrideCommand()
        {
           _accTestState.Owner = this.GetType().ToString();

           _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
           _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_I) < 0);

           return true;
        }

        private void InitMnemonics()
        {
           _mnemonicList = new List<int>();
        }

        #region HwndHost

        private HwndTarget _target = null;

        private const int HWNDHOST_ID = 0x00000002;

        /// <summary>
        /// 
        /// </summary>
        protected override HandleRef BuildWindowCore(HandleRef parent)
        {
            NativeMethods.SetParent(new HandleRef(this, _hwnd), parent);

            NativeMethods.ShowWindow(new HandleRef(this, _hwnd), NativeConstants.SW_SHOW);


            this.MessageHook += new HwndSourceHook(HelperHwndSourceHook);

            //_hwnd = NativeMethods.CreateWindowEx(0, "static", "",
            //                        NativeConstants.WS_CHILD | NativeConstants.WS_VISIBLE,
            //                        0, 0,
            //                        400, 150,
            //                        parent.Handle,
            //                        (IntPtr)HWNDHOST_ID,
            //                        IntPtr.Zero,
            //                        0);

            _target = new HwndTarget(_hwnd);
            
            _buttons = new List<IntPtr>();
            
            IntPtr button = NativeMethods.CreateWindowEx(
                0,
                "Button",
                "HwndHost Hwnd button #" + (_buttons.Count + 1),
                NativeConstants.WS_CHILD,
                0,
                0,
                400,
                30,
                _hwnd,
                IntPtr.Zero,
                IntPtr.Zero,
                null);

            NativeMethods.ShowWindow(new HandleRef(this, button), NativeConstants.SW_SHOW);

            _buttons.Add(button);
            _buttonHwndSubclassHook = new HwndWrapperHook(buttonHelper);
            _buttonSubclass = new HwndSubclass(_buttonHwndSubclassHook);
            _buttonSubclass.Attach(button);



            return new HandleRef(null, _hwnd);
        }

        IntPtr HelperHwndSourceHook(IntPtr window, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            bool bHandled = false;

            switch (msg)
            {
                case NativeConstants.WM_COMMAND:
                    int command = NativeMethods.IntPtrToInt32(wParam) & 0x0000FFFF;

                    switch (command)
                    {
                        case 0x0030:
                            bHandled = OnChildOverrideCommand();
                            break;
                        default:
                            break;
                    }
                    break;
                case NativeConstants.WM_SETFOCUS:
                    CoreLogger.LogStatus(window.ToString() + " get the focus");
                    break;
  
            }

            handled = bHandled;
            return IntPtr.Zero;
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            NativeMethods.DestroyWindow(hwnd);
        }

        #endregion

     

        bool IKeyboardInteropTestControl.NeedRecord
        {
           set
           {
               _bNeedRecord = value;
           }
        }


        #region IKeyboardInputSink

        /// <SecurityNote>
        /// Critical - This code causes unmanaged code elevation.
        /// </SecurityNote>
        ///[SuppressUnmanagedCodeSecurity, SecurityCritical]
        [DllImport("User32.dll", SetLastError = true, ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern bool IsChild(IntPtr hWndParent, IntPtr hwnd);


        bool IKeyboardInputSink.HasFocusWithin()
        {
            IntPtr hwndFocus = NativeMethods.GetFocus();

            if (_hwnd != IntPtr.Zero)
            {
                if (_hwnd == hwndFocus || IsChild(_hwnd, hwndFocus))
                {
                    return true;
                }
            }

            return false;
        }

        private IKeyboardInputSink ChildSinkWithFocus
        {
            get
            {
                IKeyboardInputSink ikis = null;

                if (null == _childSinks)
                    return null;

                foreach (IKeyboardInputSink childSink in _childSinks)
                {
                    if (childSink.HasFocusWithin())
                    {
                        ikis = childSink;
                        break;
                    }
                }

                return ikis;
            }
        }

       IntPtr buttonHelper(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
       {
           switch (msg)
           {
               case NativeConstants.WM_SETFOCUS:
                   CoreLogger.LogStatus("HostedHwndControl.buttonHelper: SETFOCUS message");
                   if (_bNeedRecord && !_bFirstTab)
                   {
                       _tabTestState.RecordedTabStops++;
                   }
                   break;
               default:
                   break;
           }
           return IntPtr.Zero;
       }

       bool IKeyboardInputSink.TabInto(TraversalRequest request)
       {
           CoreLogger.LogStatus("HostedHwndControl.TabInto");

           IntPtr ptr = IntPtr.Zero;
           if (_buttons != null)
           {
               if (request.FocusNavigationDirection == FocusNavigationDirection.First || (request.FocusNavigationDirection == FocusNavigationDirection.Right))
               {
                   // we have to work around the issue that logic focus does not cross boundaries
                   ptr = NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));

                   _tabTestState.RecordedTabStops++;
                   _tabTestState.RecordedFirstTab = _buttons[0];
                   _bFirstTab = false;
                   _bShiftTab = (NativeMethods.GetKeyState(NativeConstants.VK_SHIFT) < 0);

                }
               else if (request.FocusNavigationDirection == FocusNavigationDirection.Last || (request.FocusNavigationDirection == FocusNavigationDirection.Left))
               {
                   //due to the issue of logic focus, we have to directly call wndproc to work around it
                   ptr = NativeMethods.SetFocus(new HandleRef(null, _buttons[_buttons.Count - 1]));
                   _tabTestState.RecordedTabStops++;
                   _tabTestState.RecordedFirstTab = _buttons[_buttons.Count - 1];
                   _bFirstTab = false;
                   _bShiftTab = (NativeMethods.GetKeyState(NativeConstants.VK_SHIFT) < 0);
               }
               else
               {
                   throw new Exception("The tab request was not expected with the current values.  Mode: " + request.FocusNavigationDirection.ToString());
               }

               //if (bHandled)
               {
                   // tabbedInto = true;
                   CoreLogger.LogStatus("  Tabbed into");
                   return true;
               }
           }
  
           return false;

       }

       private bool TabOut(ref MSG msg, ModifierKeys modifiers)
       {
           IntPtr focusedHandle = NativeMethods.GetFocus();
           int index = _buttons.IndexOf(focusedHandle);
           bool bHandled = false;

           // tab out
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
                   // No more tab stops after last button.
                   if (index == (_buttons.Count - 1))
                   {
                       //NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));
                       CoreLogger.LogStatus("  No more tab stops in HwndHost, calling site");
                       if (((IKeyboardInputSink)this).KeyboardInputSite.OnNoMoreTabStops(new TraversalRequest(tabDirection)))
                       {
                           CoreLogger.LogStatus("  site accepted");
                       }
                       else
                       {
                           CoreLogger.LogStatus("   PARENT DID NOT ACCEPT TAB LOOPING!!!");
                           NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));
                       }
                   }
                   else  // Tab into next sink.
                   {
                       ((IKeyboardInputSink)_childSinks[index]).TabInto(new TraversalRequest(FocusNavigationDirection.First));
                   }
               }
               else // shift tab, go to sink with index - 1 unless this is the first button
               {
                   // No more tab stops before _buttons[0].
                   if (index == 0)
                   {
                       CoreLogger.LogStatus("  No more tab stops in HwndHost, calling site");
                       if (((IKeyboardInputSink)this).KeyboardInputSite.OnNoMoreTabStops(new TraversalRequest(tabDirection)))
                       {
                           CoreLogger.LogStatus("    *site accepted");
                       }

                   }
                   else // Tab into previous sink.
                   {
                       (_childSinks[index - 1]).TabInto(new TraversalRequest(FocusNavigationDirection.Last));
                   }
               }

               // note that if focus isn't currently in one of the win32 _buttons[] we pass this message to the sink
               // with focus (if any) below.
               return true;
           }
           else if (_childSinks != null) // Pass tab to child sinks
           {
               //foreach (IKeyboardInputSink childSink in _childSinks)
               //{
               //    if (childSink.HasFocusWithin() == true)
               //    {
               //        bHandled = childSink.TranslateAccelerator(ref msg, modifiers);
               //        break;
               //    }
               //}
               bHandled = ChildSinkWithFocus.TranslateAccelerator(ref msg, modifiers);
           }

           return bHandled;
       }

        private bool ProcessAccelerators(ref MSG msg, ModifierKeys modifiers)
        {
            bool bHandled = false;

            if (modifiers == ModifierKeys.Control)
            {
                //_accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
                switch(NativeMethods.IntPtrToInt32(msg.wParam))
                {
                    case NativeConstants.VK_A:  // common
                        _accTestState.SumCommand++;
                        _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_A) < 0);
                        _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
                        bHandled = true;
                        break;
                    case NativeConstants.VK_Y:  // unique
                        _accTestState.SumCommand++;
                        _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_Y) < 0);
                        _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
                        bHandled = true;
                        break;
                    case NativeConstants.VK_Q:  // parent first
                        _accTestState.SumCommand++;
                        _accTestState.KeyState = (NativeMethods.GetKeyState(NativeConstants.VK_Q) < 0);
                        _accTestState.ControlState = (NativeMethods.GetKeyState(NativeConstants.VK_CONTROL) < 0);
                        return false;                        
                }
            }

            if (!bHandled)
            {
                if (_acceleratorTable != null)
                {
                    bHandled = (0 != NativeMethods.TranslateAccelerator(new HandleRef(null, _hwnd), new HandleRef(null, _acceleratorTable.ACCELTable), ref msg));
                }
            }

            return bHandled;
        }

        bool IKeyboardInputSink.TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {
            bool bHandled = false;
            MSG ownMsg = new MSG();

            switch (msg.message)
            {
                case NativeConstants.WM_KEYDOWN:
                    if (msg.wParam == new IntPtr(NativeConstants.VK_TAB))
                    {
                        if (_expectedTabTestState.TestType == TabTestType.Normal)
                        {
                            // work around the issue that VK_TAB is unable to handled by TranslateChar                           
                            NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));
                            if (NativeMethods.TranslateMessage(ref msg))
                            {
                                NativeMethods.TranslateMessage(ref msg);
                                NativeMethods.DispatchMessage(ref msg);
                                while (NativeMethods.GetMessage(ref ownMsg, IntPtr.Zero, 0, 0) != 0)
                                {
                                    if (ownMsg.message == NativeConstants.WM_CHAR)
                                    {
                                        _tabTestState.RecordedTabStops++;
                                        break;
                                    }
                                }
                                CoreLogger.LogStatus("Hostedhwnd: message is translated");
                            }
                            else
                                CoreLogger.LogStatus("HostedHwnd: failed to tranlate message");
                            return false;
                        }
                        else
                        {
                            bHandled = TabOut(ref msg, modifiers);
                        }
                    }
                    else
                    {
                        if (_expectedAccTestState.TestType == AccelTestType.PFirst)
                        {
                            CoreLogger.LogStatus("HostedHwnd: parent first");
                            bHandled = ProcessAccelerators(ref msg, modifiers);
                            if (!bHandled)
                            {
                                if (ChildSinkWithFocus != null)
                                {
                                    bHandled = ChildSinkWithFocus.TranslateAccelerator(ref msg, modifiers);
                                }
                                else
                                    bHandled = true;
                            }
                            break;
                        }
                        else
                        {
                            CoreLogger.LogStatus("HostedHwnd: child first");
                            // default: child handles them first; then parent;
                            if (ChildSinkWithFocus != null)
                            {
                                bHandled = ChildSinkWithFocus.TranslateAccelerator(ref msg, modifiers);
                            }

                            if (!bHandled)
                            {
                                bHandled = ProcessAccelerators(ref msg, modifiers);
                            }
                        }

                    }
                    break;
                case NativeConstants.WM_KEYUP:
                case NativeConstants.WM_SYSKEYUP:
                case NativeConstants.WM_SYSKEYDOWN:
                    break;
                default:
                    break;
            }

            return bHandled;
        }

        bool IKeyboardInputSink.TranslateChar(ref MSG msg, ModifierKeys modifiers)
        {
           if (msg.message == NativeConstants.WM_CHAR && (NativeMethods.IntPtrToInt32(msg.wParam) == NativeConstants.VK_TAB))
           {
               _tabTestState.RecordedTabStops++;
               
               CoreLogger.LogStatus("HostedHwnd: translateChar");
               return true;
           }
           return false;
        }

        /// <summary>
        ///     This method is called whenever one of the component's
        ///     mnemonics is invoked.  The message must either be WM_KEYDOWN
        ///     or WM_SYSKEYDOWN.  It's illegal to modify the MSG structrure,
        ///     it's passed by reference only as a performance optimization.
        ///     If this component contains child components, the container
        ///     OnMnemonic will need to call the child's OnMnemonic method.
        /// </summary>
        bool IKeyboardInputSink.OnMnemonic(ref MSG msg, ModifierKeys modifiers)
        {
            //CoreLogger.LogStatus("IKeyboardInputSink.OnMnemonic");

            if (_mnemonicList != null)
            {
                foreach (int mnemonicLetter in _mnemonicList)
                {
                    //CoreLogger.LogStatus("msg.wParam " + NativeMethods.IntPtrToInt32(msg.wParam)+ " mnemonicLetter " + mnemonicLetter);

                    if ((NativeMethods.IntPtrToInt32(msg.wParam) == mnemonicLetter) && (modifiers == ModifierKeys.Alt))
                    {
                        CoreLogger.LogStatus("Got " + (char)mnemonicLetter + " mnemonic!!!");
                        _gotMnemonics++;
                        return true;
                    }
                }
            }

             if (_childSinks != null)
            {
                foreach (IKeyboardInputSink childSink in _childSinks)
                {
                    if (childSink.OnMnemonic(ref msg, modifiers) == true)
                    {
                        return true;
                    }
                }
            }

            // This control and none of its children handled the mnemonic.
            return false;
        }

        #endregion

        #region IKeyboardInteropTestControl
        void IKeyboardInteropTestControl.ResetTestState()
        {
            _expectedTabTestState.TestType = TabTestType.Default;
            _accTestState.ResetTestState();
            _expectedAccTestState.ResetTestState();

            _tabTestState.ResetTestState();
            _bFirstTab = true;
            _bShiftTab = false;
            _gotMnemonics = 0;
            
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
                       NativeMethods.SendMessage(new HandleRef(null, _hwnd), msg, wParam, lParam);
                       handled = true;
                       break;
               }
           }

           return IntPtr.Zero;
        }

        private IKeyboardInteropTestControl InsertHwndSource(ref int sourcePosition)
        {

           sourcePosition = 150;
           if (_childSinks == null)
           {
               sourcePosition = 30;
               _childSinks = new List<IKeyboardInputSink>();
           }

           
           HwndSourceParameters hwndParams = new HwndSourceParameters("Hwnd Sourced Avalon", 400, 30);

           hwndParams.ParentWindow = _hwnd;
           hwndParams.SetPosition(0, sourcePosition);
           hwndParams.WindowStyle = NativeConstants.WS_VISIBLE | NativeConstants.WS_CHILD;

           _childHwndSource = new SourcedAvalon(hwndParams);

           // parent override child's accelerator
           _childHwndSource.AddHook(new HwndSourceHook(ChildHwndSourceMsgFilter));

           _childSinks.Add((IKeyboardInputSink)_childHwndSource);

           return (IKeyboardInteropTestControl)_childHwndSource;
        }


        IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
        {
            int sourcePosition = 0;

            IKeyboardInteropTestControl child = null;

            switch (childType)
            {
                case "SourcedAvalon":
                    child = InsertHwndSource(ref sourcePosition);
                    break;
                default:
                    break;
            }

            if (child == null)
            {
                return child;
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
                "Hwnd button in HwndHost #" + (_buttons.Count + 1),  // lpszWindowName
                NativeConstants.WS_CHILD,   // style
                0,                          // x
                sourcePosition + 30,        // y
                400,                        // width
                30,                         // height
                _hwnd,                      // hWndParent
                IntPtr.Zero,                // hMenu
                IntPtr.Zero,                // hInst
                null                        // pvParam
                );

            NativeMethods.ShowWindow(new HandleRef(this, button), NativeConstants.SW_SHOW);

            _buttons.Add(button);

            _buttonHwndSubclassHook = new HwndWrapperHook(buttonHelper);
            _buttonSubclass = new HwndSubclass(_buttonHwndSubclassHook);
            _buttonSubclass.Attach(button);

            nextHwnd = button;

            // Create a keyboard input site for the HwndSource sink and connect it.
            // When there are no more tab stops in the child focus will move to the button hwnd.

            ((IKeyboardInputSink)child).KeyboardInputSite = new Win32KeyboardInputSite(prevHwnd, (IKeyboardInputSink)child, nextHwnd, null);

            return child;
        }

        void IKeyboardInteropTestControl.SetFocusToFirstChild(bool first)
        {
            if (_buttons != null)
            {
                if (first)
                    NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));
                else
                    NativeMethods.SetFocus(new HandleRef(null, _buttons[_buttons.Count - 1]));
            }
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

        private List<IntPtr> _buttons = null;
       //private bool _isParent = false;
    }
}
