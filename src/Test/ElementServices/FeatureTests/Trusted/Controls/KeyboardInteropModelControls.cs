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

namespace Avalon.Test.CoreUI.Trusted.Controls
{
    /// <summary>
    /// Interface implemented by each of the KeyboardInteropModel test controls. Defines functions
    /// for accessing recorded control state. This allows the same verification code to be used
    /// for any interop control combination.
    /// </summary>
    /// <remarks>
    /// Test controls should not record tabs/accelerators/mnemonics that their children receive.
    /// </remarks>    
    public interface IKeyboardInteropTestControl
    {
        /// <summary>
        /// Tab testing helper. Set tab to first child in a control. 
        /// </summary>
        void SetFocusToFirstChild();

        /// <summary>
        /// Reset recorded accelerators, mnemonics and tabs. 
        /// </summary>
        void ResetTestState();

        /// <summary>
        /// Return the number of local tab stop in the test control. 
        /// Does not count sinks.
        /// </summary>
        int LocalTabStops();

        /// <summary>
        /// Return the number of tabs hit in this test control. 
        /// Does not count sinks.
        /// </summary>
        int RecordedLocalTabs { get; }

        /// <summary>
        /// Should return the number of times the unique accelerator was recorded by the control.
        /// </summary>
        int RecordedUniqueAccelerator { get; }

        /// <summary>
        /// Should return the number of times the common accelerator was recorded by this control.
        /// </summary>
        int RecordedCommonAccelerator { get; }

        /// <summary>
        /// Add a mnemonic to the control.
        /// </summary>
        void AddMnemonic(string letter, ModifierKeys modifier);

        /// <summary>
        /// Should return true if specified mnemonic was recorded by the control.
        /// </summary>
        int RecordedMnemonic { get; }

        /// <summary>
        /// Add a child IKeyboardInteropTestControl component. 
        /// </summary>
        IKeyboardInteropTestControl AddChild(string ChildType);

    }



    /// <summary>
    /// Top level avalon window for keyboard input sink testing.
    /// </summary>
    public class AvalonWindow : IKeyboardInteropTestControl
    {
        private Window _window;
        private StackPanel _stackPanel;

        private List<Button> _buttons = null;   // Avalon buttons used as tab stops.

        private List<IKeyboardInteropTestControl> _childSinks = null;
                
        // Add accelerators, mnemonics or tab stops.
        bool _hasAccelerators;
        bool _hasMnemonics;
        bool _tabStops;

        // Number of accelerators caught.
        int _gotCommonCommand = 0;
        int _gotUniqueCommand = 0;
        int _gotMnemonic = 0;

        // Number of tabs received by one of this window's avalon buttons.
        int _tabCount = 0;

        // Number of events caught in routed event handler.
        int _keyDownCount = 0;

        /// <summary>
        /// A top level Avalon window.
        /// </summary>
        public AvalonWindow(bool TabStops, bool Accelerators, bool Mnemonics)
        {
            _tabStops = TabStops;
            _hasAccelerators = Accelerators;
            _hasMnemonics = Mnemonics;

            _window = new Window();
            _window.Title = "Top Level Avalon Window";
            _window.Width = 400;
            _window.Height = 500;

            _stackPanel = new StackPanel();
            _stackPanel.PreviewKeyDown += PreviewKeyDownHandler;

            _window.Content = _stackPanel;

            if (TabStops == true)
            {
                // Create first button for separating child sink. This will be the first tab stop.
                Button b = new Button();
                _buttons = new List<Button>();
                _buttons.Add(b);

                b.Height = 50;
                b.Focusable = true;
                b.Content = "Top Av button " + _buttons.Count;
                _stackPanel.Children.Add(b);

                b.GotFocus += this.OnGotFocus;
            }

            if (_hasAccelerators == true)
            {
                InitAccelerators();
            }

            if (_hasMnemonics == true)
            {
                InitMnemonics();
            }

            _window.Show();
            _window.Activate();            
        }       

        void OnGotFocus(object sender, RoutedEventArgs e)
        {
            _tabCount++;
        }

        private void PreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            _keyDownCount++;
        }

        /// <summary>
        /// Number of events caught by PreviewKeyDown handler.
        /// </summary>
        public int KeyDownCount
        {
            get
            {
                return _keyDownCount;
            }
            set
            {
                _keyDownCount = value;
            }
        }

#region Accelerators

        /// <summary>
        /// Attaches a unique accelerator and a common accelerator to the first Button.
        /// </summary>
        private void InitAccelerators()
        {
            //
            // Hookup unique command/accelerator Ctrl-A
            //

            // Create command.
            RoutedCommand uniqueCommand = new RoutedCommand("UniqueCommand", this.GetType(), null);

            // Create binding.
            CommandBinding uniqueCommandBinding = new CommandBinding(uniqueCommand);

            // Set up key binding to command.
            KeyBinding uniqueKeyBinding = new KeyBinding(uniqueCommand, Key.A, ModifierKeys.Control);

            // Attach event to command binding.
            uniqueCommandBinding.Executed += new ExecutedRoutedEventHandler(OnUniqueCommand);

            // Add command binding to the control.
            _buttons[0].CommandBindings.Add(uniqueCommandBinding);
            _buttons[0].InputBindings.Add(uniqueKeyBinding);

            //
            // Hookup common command/accelerator Ctrl-O
            //
            RoutedCommand commonCommand = new RoutedCommand("CommonCommand", this.GetType(), null);
            CommandBinding commonCommandBinding = new CommandBinding(commonCommand);
            KeyBinding commonKeyBinding = new KeyBinding(commonCommand, Key.O, ModifierKeys.Control);
            commonCommandBinding.Executed += new ExecutedRoutedEventHandler(OnCommonCommand);
            _buttons[0].CommandBindings.Add(commonCommandBinding);
            _buttons[0].InputBindings.Add(commonKeyBinding);
        }

        private void OnUniqueCommand(object target, ExecutedRoutedEventArgs args)
        {
            CoreLogger.LogStatus("Executed unique command on target " + target);
            _gotUniqueCommand++;
        }

        private void OnCommonCommand(object target, ExecutedRoutedEventArgs args)
        {
            CoreLogger.LogStatus("Executed common command on target " + target);
            _gotCommonCommand++;
        }

        private void InitMnemonics()
        {

        }

        // Handle AccessKey (mnemonic) on button.
        void onClickButton(object o, RoutedEventArgs args)
        {
            CoreLogger.LogStatus("onClickButton has been raised");
            _gotMnemonic++;
        }

#endregion // Accelerators

#region IKeyboardInteropTestControl

        /// <summary>
        /// Tab testing helper. Set tab to first child in a control. 
        /// </summary>
        void IKeyboardInteropTestControl.SetFocusToFirstChild()
        {
            if (_buttons != null)
            {
                _buttons[0].Focus();
            }
        }

        /// <summary>
        /// Reset recorded accelerators, mnemonics and tabs. 
        /// </summary>
        void IKeyboardInteropTestControl.ResetTestState()
        {
            _gotUniqueCommand = 0;
            _gotCommonCommand = 0;
            _gotMnemonic = 0;
            _tabCount = 0;

            _keyDownCount = 0;
        }


        /// <summary>
        /// Return the number of local tab stops in the test control. 
        /// Does not count sinks.
        /// </summary>
        int IKeyboardInteropTestControl.LocalTabStops()
        {
            if (_buttons != null)
            {
                return _buttons.Count;
            }

            return 0;
        }

        /// <summary>
        /// Return the number of tabs hit in this test control. 
        /// Does not count sinks.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedLocalTabs
        {
            get
            {
                return _tabCount;
            }
        }

        /// <summary>
        /// Should return true if specified accelerator was recorded by the control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedUniqueAccelerator
        {
            get
            {
                return _gotUniqueCommand;
            }
        }

        /// <summary>
        /// Should return true if the common test accelerator was recorded by this control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedCommonAccelerator
        {
            get
            {
                return _gotCommonCommand;
            }
        }

        /// <summary>
        /// Add a mnemonic to the control.
        /// </summary>
        void IKeyboardInteropTestControl.AddMnemonic(string letter, ModifierKeys modifier)
        {
            // Register mnemonic with first control.
            AccessKeyManager.Register(letter, _buttons[0]);
            
            _buttons[0].Click += new RoutedEventHandler(onClickButton);
        }

        /// <summary>
        /// Should return true if specified mnemonic was recorded by the control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedMnemonic
        {
            get
            {
                return _gotMnemonic;
            }
        }

        /// <summary>
        /// </summary>
        IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
        {
            if (childType != "HostedHwnd")
            {
                return null;
            }

            if (_childSinks == null)
            {
                _childSinks = new List<IKeyboardInteropTestControl>();
            }

            // Create an HwndHost and put it in the window.
            HostedHwndControl childHwndHost = new HostedHwndControl(_tabStops, _hasAccelerators, _hasMnemonics);
            childHwndHost.Focusable = true;
            childHwndHost.Height = 150;
            _stackPanel.Children.Add(childHwndHost);

            _childSinks.Add((IKeyboardInteropTestControl)childHwndHost);

            if (_tabStops == true)
            {
                // Add another Avalon button to separate this sink from the next.
                Button b = new Button();
                _buttons.Add(b);

                b.Height = 50;
                b.Focusable = true;
                b.Content = "Top Av button " + _buttons.Count;
                _stackPanel.Children.Add(b);

                b.GotFocus += this.OnGotFocus;

                //if (_hasAccelerators)
                //{
                //    AddAccelerators(b);
                //}
            }

            return childHwndHost as IKeyboardInteropTestControl;
        }
        
       #endregion

    }



    /// <summary>
    /// Top level win32 window.
    /// Implemented using Microsoft.Test.Win32.HwndWrapper.
    /// </summary>
    public class Win32Window : IKeyboardInteropTestControl
    {
        // Top level Hwnd and WndProc.
        private IntPtr _hwnd = IntPtr.Zero;
        private HwndWrapper _hwndWrapper = null;
        private HwndWrapperHook _hook;

        // Add accelerators, mnemonics or tab stops.
        bool _hasAccelerators;
        bool _hasMnemonics;
        bool _tabStops;

        // Hwnd buttons used for tab stops.
        private List<IntPtr> _buttons = null;

        // Button subclass.
        private HwndWrapperHook _buttonHwndSubclassHook;
        private HwndSubclass _buttonSubclass;

        private List<IKeyboardInputSink> _childSinks = null;

        private AccelaratorTable _acceleratorTable = null;
        private List<int> _mnemonicList = null;

        //
        // Test state variables.
        //

        // Record if window caught accelerators.
        int _gotCommonCommand = 0;
        int _gotUniqueCommand = 0;
        int _gotMnemonic = 0;

        int _tabCount;

        /// <summary>
        /// Constructor, if TabStops is true a button will be put in the window.
        /// </summary>
        public Win32Window(bool TabStops, bool Accelerators, bool Mnemonics)
        {
            _tabStops = TabStops;
            _hasAccelerators = Accelerators;
            _hasMnemonics = Mnemonics;
     
            _hwndWrapper = new HwndWrapper(
                0,                              // classStyle 
                NativeConstants.WS_OVERLAPPEDWINDOW | NativeConstants.WS_VISIBLE,  // style
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

            if (_hasAccelerators == true)
            {
                InitAccelerators();
            }

            if (_hasMnemonics == true)
            {
                InitMnemonics();
            }

            CoreLogger.LogStatus("Created top level hwnd " + _hwnd);

            // Show the hwnd and the buttons.
            NativeMethods.ShowWindow(new HandleRef(this, _hwndWrapper.Handle), NativeConstants.SW_SHOW);

            if (_tabStops)
            {
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


                NativeMethods.ShowWindow(new HandleRef(this, button), NativeConstants.SW_SHOW);

                // Start out with focus on the button.
                NativeMethods.SetFocus(new HandleRef(null, button));

                // Create subclass hook for buttonHelper callback.
                _buttonHwndSubclassHook = new HwndWrapperHook(buttonHelper);

                // Create subclass with the new hook
                _buttonSubclass = new HwndSubclass(_buttonHwndSubclassHook);

                // Attch subclass to the button.
                _buttonSubclass.Attach(button);

                CoreLogger.LogStatus("Created top hwnd button");
            }

            // Show the hwnd and the buttons.
            NativeMethods.ShowWindow(new HandleRef(this, _hwndWrapper.Handle), NativeConstants.SW_MINIMIZE);
            DispatcherHelper.DoEvents(250);
            NativeMethods.ShowWindow(new HandleRef(this, _hwndWrapper.Handle), NativeConstants.SW_RESTORE);
            DispatcherHelper.DoEvents(250);

        }

   
        IntPtr buttonHelper(IntPtr hwnd, int msg, IntPtr wParam,
            IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeConstants.WM_KEYDOWN:
                    // CoreLogger.LogStatus("Win32Window.buttonHelper: KEYDOWN message");
                    break;
                case NativeConstants.WM_CHAR:
                    // CoreLogger.LogStatus("Win32Window.buttonHelper: CHAR message");
                    break;
                case NativeConstants.WM_COMMAND:
                case NativeConstants.WM_SYSCOMMAND:
                    // CoreLogger.LogStatus("Win32Window.buttonHelper: COMMAND message");
                    break;
                case NativeConstants.WM_SETFOCUS:
                    // CoreLogger.LogStatus("Win32Window.buttonHelper: SETFOCUS message");
                    _tabCount++;
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }

        #region Accelerators

        private void InitAccelerators()
        {

            // Create accelerator table. IKIS.TranslateAccelerator will call NativeMethods.TranslateAccelerator which will
            // make use of this table.
            NativeStructs.ACCEL[] accelArray = new NativeStructs.ACCEL[2];
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
            accel.key = (short)(NativeMethods.VkKeyScan('O') & 0x00FF);
            accel.cmd = 0x0042; 

            accelArray[1] = accel;

            _acceleratorTable = new AccelaratorTable(accelArray);
            _acceleratorTable.Create();
        }

        private void InitMnemonics()
        {
            _mnemonicList = new List<int>();
        }

        void mnemonicsHandler(object o, MnemonicsEventArgs args)
        {
            CoreLogger.LogStatus("Mnemonics: " + args.Letter);

            // HwndHost hwndHost = (HwndHost)o;

            //if (args.Letter == (int)'m')
            //{
            //    _count |= 0x00F;

            //    if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
            //    {
            //        _count |= 0x10000;
            //    }
            //}
        }
        #endregion // Accelerators

        /// <summary>
        /// Top level hwnd WndProc
        /// </summary>
        private IntPtr TopLevelHwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeConstants.WM_KEYDOWN:
                    // CoreLogger.LogStatus("Win32Window.TopLevelHwndHook: KEYDOWN message");
                    break;
                case NativeConstants.WM_CHAR:
                    // CoreLogger.LogStatus("Win32Window.TopLevelHwndHook: CHAR message");
                    break;
                case NativeConstants.WM_SYSCOMMAND:
                case NativeConstants.WM_COMMAND:
                    
                    // Mask notification code (high order word) of Accelerator or Menu message source.
                    int command = NativeMethods.IntPtrToInt32(wParam) & 0x0000FFFF;

                    // CoreLogger.LogStatus("Win32Window.TopLevelHwndHook WM_COMMAND " + command);
                    switch (command)
                    {
                        case 0x00FF:
                            CoreLogger.LogStatus("Win32Window.TopLevelHwndHook got unique accelerator");
                            _gotUniqueCommand++;
                            handled = true;
                            break;
                        case 0x0042:
                            CoreLogger.LogStatus("Win32Window.TopLevelHwndHook got common accelerator");
                            _gotCommonCommand++;
                            handled = true;
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
        

        /// <summary>
        /// Handler for messages from the ComponentDispatcher. 
        /// Distributes accelerators to child controls or child sinks depending on focus.
        /// </summary>
        private void OnPreProcessMessage(ref MSG msg, ref bool handled)
        {
            switch (msg.message)
            {
                case NativeConstants.WM_KEYDOWN:
                    //CoreLogger.LogStatus("Win32Window.OnPreprocessMessage: KEYDOWN message");

                    // Get index of button with focus in _buttons (if none -1).
                    IntPtr focusedHandle = NativeMethods.GetFocus();
                    int index = _buttons.IndexOf(focusedHandle);

                    if (msg.wParam == new IntPtr(NativeConstants.VK_TAB))
                    {                        
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
                                    _tabCount++;
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
                                    _tabCount++;
                                }
                                else // Tab into previous sink.
                                {
                                    ((IKeyboardInputSink)_childSinks[index - 1]).TabInto(new TraversalRequest(FocusNavigationDirection.Last));
                                }
                            }

                            // note that if focus isn't currently in any of the win32 _buttons[] we pass this message to the sink
                            // with focus (if any) below.
                            handled = true;
                        } 
                        else if (_childSinks != null)   // pass accelarator to a child sink with focus.
                        {
                            foreach (IKeyboardInputSink childSink in _childSinks)
                            {
                                if (childSink.HasFocusWithin() == true)
                                {
                                    // todo: get modifier keys
                                    ModifierKeys m = new ModifierKeys();
                                    childSink.TranslateAccelerator(ref msg, m);
                                    break;
                                }
                            }
                        }
                    }
                    else  // Not a tab keydown message, handle as an accelerator.
                    {
                        // If focus is not in _buttons, give the the focused sink a chance to handle this message as an accerator.
                        if ((index < 0) && (_childSinks != null))
                        {
                            foreach (IKeyboardInputSink childSink in _childSinks)
                            {
                                // Pass the accelerator to the sink with focus.
                                if (childSink.HasFocusWithin() == true)
                                {
                                    if (childSink.TranslateAccelerator(ref msg, Keyboard.Modifiers) == true)
                                    {
                                        CoreLogger.LogStatus("Win32Window.OnPreProcessMessage: Accelerator handled by child HwndSource");
                                        handled = true;
                                    }
                                    else
                                    {
                                        CoreLogger.LogStatus("Win32Window.OnPreProcessMessage: Accelerator NOT handled by child HwndSource");
                                    }

                                    // Don't pass accelerator to any other sinks. 
                                    break;  
                                }
                            }
                        }
                        else // Focus is in _buttons, give the message to the unmanaged TranslateAccelerator.
                        {
                            if (_acceleratorTable != null)
                            {
                                // todo: Trim these debugging messages.
                                CoreLogger.LogStatus("Win32Window.OnPreProcessMessage - Calling native accelerator " + _hwnd + " " + _acceleratorTable);
                                if (0 != NativeMethods.TranslateAccelerator(new HandleRef(null, _hwnd), new HandleRef(null, _acceleratorTable.ACCELTable), ref msg))
                                {
                                    CoreLogger.LogStatus("  Win32Window.OnPreprocessMessage - Accelerator handled by native TranslateAccelerator " + _hwnd);
                                }
                                else
                                {
                                    CoreLogger.LogStatus("  Win32Window.OnPreprocessMessage - Accelerator NOT handled by native TranslateAccelerator");
                                }
                            }
                        }

                    }
                    break;

                case NativeConstants.WM_CHAR:
                case NativeConstants.WM_DEADCHAR:
                    // CoreLogger.LogStatus("Win32Window.OnPreprocessMessage: Char message");
                    break;

                case NativeConstants.WM_SYSCHAR:
                case NativeConstants.WM_SYSDEADCHAR:
                    if (_mnemonicList != null)
                    {
                        foreach (int mnemonicLetter in _mnemonicList)
                        {
                            // CoreLogger.LogStatus("Win32Window:OnPreprocessMessage msg.wParam " + NativeMethods.IntPtrToInt32(msg.wParam)+ " mnemonicLetter " + mnemonicLetter);

                            if ((NativeMethods.IntPtrToInt32(msg.wParam) == mnemonicLetter))
                            {
                                // CoreLogger.LogStatus("Got Alt-" + (char)mnemonicLetter + " mnemonic!!!");
                                _gotMnemonic++;
                                handled = true;
                            }
                        }
                    }

                    // Call OnMnemonic on children until one handles it.
                    if (!handled && (_childSinks != null))
                    {
                        foreach (IKeyboardInputSink childSink in _childSinks)
                        {
                            if (childSink.OnMnemonic(ref msg, ModifierKeys.Alt) == true)
                            {
                                handled = true;
                            }
                        }
                    }
                    break;

                case NativeConstants.WM_COMMAND:
                case NativeConstants.WM_SYSCOMMAND:
                    // CoreLogger.LogStatus("Win32Window.OnPreprocessMessage: Command message");
                    break;

                default:
                    break;
            }
        }

#region IKeyboardInteropTestControl

        /// <summary>
        /// Tab testing helper. Set tab to first child in a control. 
        /// </summary>
        void IKeyboardInteropTestControl.SetFocusToFirstChild()
        {
            if (_buttons != null)
            {
                NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));
            }
        }

        /// <summary>
        /// Reset recorded accelerators, mnemonics and tabs. 
        /// </summary>
        void IKeyboardInteropTestControl.ResetTestState()
        {
            _gotUniqueCommand = 0;
            _gotCommonCommand = 0;
            _gotMnemonic = 0;

            if (_childSinks != null)
            {
                foreach (IKeyboardInputSink childSink in _childSinks)
                {
                    Win32KeyboardInputSite site = (Win32KeyboardInputSite)childSink.KeyboardInputSite;
                    site.tabCount = 0;
                }
            }
        }

        /// <summary>
        /// Return the number of local tab stop in the test control. 
        /// Does not count sinks.
        /// </summary>
        int IKeyboardInteropTestControl.LocalTabStops()
        {
            if (_buttons != null)
            {
                // Top level controls always loop so count an extra tab focus event.
                return _buttons.Count;
            }

            return 0;
        }

        /// <summary>
        /// Return the number of tabs hit in this test control. 
        /// Does not count sinks.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedLocalTabs
        {
            get
            {
                int totalTabs = _tabCount;


                if (_childSinks != null)
                {
                    foreach (IKeyboardInputSink childSink in _childSinks)
                    {
                        Win32KeyboardInputSite site = (Win32KeyboardInputSite)childSink.KeyboardInputSite;
                        //CoreLogger.LogStatus("    site tab count " + site.tabCount);
                        totalTabs += site.tabCount;
                    }
                }

                return totalTabs;
            }
        }
        
        /// <summary>
        /// Should return true if specified accelerator was recorded by the control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedUniqueAccelerator
        {
            get
            {
                return _gotUniqueCommand;
            }
        }

        /// <summary>
        /// Should return true if the common test accelerator was recorded by this control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedCommonAccelerator
        {
            get
            {
                return _gotCommonCommand;
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

        /// <summary>
        /// Should return true if specified mnemonic was recorded by the control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedMnemonic
        {
            get
            {
                return _gotMnemonic;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
        {
            if (childType != "SourcedAvalon")
            {
                return null;
            }

            CoreLogger.LogStatus("Win32Window adding " + childType);

            int sourcePosition = 210;    // Second sink goes under first sink's tab stop.
            if (_childSinks == null)
            {
                sourcePosition = 30;    // Default child HwndSource position is right under hwnd button.
                _childSinks = new List<IKeyboardInputSink>();
            }

            // Config source avalon control.
            HwndSourceParameters hwndParams = new HwndSourceParameters("Hwnd Sourced Avalon", 400, 150);
            hwndParams.ParentWindow = _hwnd;
            hwndParams.SetPosition(0, sourcePosition);
            hwndParams.WindowStyle = NativeConstants.WS_VISIBLE | NativeConstants.WS_CHILD;

            // Add SourcedAvalon control.
            SourcedAvalonControl childHwndSource = new SourcedAvalonControl(hwndParams, _tabStops, _hasAccelerators, _hasMnemonics);

            _childSinks.Add((IKeyboardInputSink)childHwndSource);

            // Add another button for tab stop after source.
            if (_tabStops)
            {
                // Add hwnd button to the hwnd.
                IntPtr button = NativeMethods.CreateWindowEx(
                    0,                              //dwExStyle
                    "Button",                       // lpszClassName
                    "top level Win32 button #" + (_buttons.Count + 1),    // lpszWindowName
                    NativeConstants.WS_CHILD,       // style
                    0,                              // x
                    sourcePosition + 150,           // y
                    400,                            // width
                    30,                             // height
                    _hwnd,                          // hWndParent
                    IntPtr.Zero,                    // hMenu
                    IntPtr.Zero,                    // hInst
                    null                            // pvParam
                    );

                _buttons.Add(button);

                NativeMethods.ShowWindow(new HandleRef(this, button), NativeConstants.SW_SHOW);

                // Start out with focus on the button.
                NativeMethods.SetFocus(new HandleRef(null, button));

                CoreLogger.LogStatus("Created top hwnd button");
            }

            // Next tab stop on tab out of sink.
            // Create a site for HwndSource
            (childHwndSource as IKeyboardInputSink).KeyboardInputSite = new Win32KeyboardInputSite(_buttons[_buttons.Count - 2], childHwndSource, _buttons[_buttons.Count - 1]);

            CoreLogger.LogStatus("Win32Window added SourcedAvalon");
            return (IKeyboardInteropTestControl)childHwndSource;
        }

        #endregion

    }



    /// <summary>
    /// Site given to child components of an Hwnd.
    /// </summary>
    public class Win32KeyboardInputSite : IKeyboardInputSite
    {
        private IntPtr _nextHwnd, _prevHwnd;
        private IKeyboardInputSink _sink;
        private UIElement _sinkElement;

        /// <summary>
        /// 
        /// </summary>
        public int tabCount = 0;

        /// <summary>
        /// 
        /// </summary>
        public Win32KeyboardInputSite(IntPtr prevHwnd, IKeyboardInputSink sink, IntPtr nextHwnd)
        {
            _nextHwnd = nextHwnd;
            _prevHwnd = prevHwnd;

            _sink = sink;
            _sink.KeyboardInputSite = this;

            _sinkElement = sink as UIElement;
        }
        
        #region IKeyboardInputSite

        /// <summary>
        /// Called by the child sink when it wants do go away.
        /// </summary>
        void IKeyboardInputSite.Unregister()
        {
            _sink = null;
            _sinkElement = null;
        }

        /// <summary>
        /// Returns child sink associated with this site.
        /// </summary>
        IKeyboardInputSink IKeyboardInputSite.Sink
        {
            get
            {
                return _sink;
            }
        }

        bool IKeyboardInputSite.OnNoMoreTabStops(TraversalRequest request)
        {
            // CoreLogger.LogStatus("Win32KeyboardInputSite.OnNoMoreTabStops");
            IntPtr ptr = IntPtr.Zero;

            if ((request.FocusNavigationDirection == FocusNavigationDirection.Previous) || (request.FocusNavigationDirection == FocusNavigationDirection.Left))
            {
                // Set focus on the next win32 element.
                ptr = NativeMethods.SetFocus(new HandleRef(null, _prevHwnd));
            }
            else if ((request.FocusNavigationDirection == FocusNavigationDirection.Next) || (request.FocusNavigationDirection == FocusNavigationDirection.Right))
            {
                // Set focus on the next win32 element.
                ptr = NativeMethods.SetFocus(new HandleRef(null, _nextHwnd));
            }

            if (ptr != IntPtr.Zero)
            {
                tabCount++;
                return true;    // Non-null return value means focus was successfully set.
            }

            // CoreLogger.LogStatus("  returned false");
            return false;
        }

#endregion

    }



    /// <summary>
    /// Control to host an Hwnd in Avalon.
    /// </summary>
    public class HostedHwndControl : HwndHost, IKeyboardInputSink, IKeyboardInteropTestControl
    {
        private AccelaratorTable _acceleratorTable = null;
        private List<int> _mnemonicList = null;

        /// <summary>
        /// </summary>
        private IntPtr _hwnd = IntPtr.Zero;

        private List<IntPtr> _buttons = null;
        private List<SourcedAvalonControl> _childHwndSources;
        
        // Button subclass (WndProc)
        private HwndSubclass _buttonSubclass = null;
        private HwndWrapperHook _buttonHwndSubclassHook;

        // Add accelerators, mnemonics or tab stops.
        bool _hasAccelerators;
        bool _hasMnemonics;
        bool _tabStops;


        // Record if window caught accelerators.
        int _gotCommonCommand = 0;
        int _gotUniqueCommand = 0;
        int _gotMnemonic = 0;

        int _tabCount = 0; 

        /// <summary>
        /// 
        /// </summary>
        public HostedHwndControl(bool TabStops, bool Accelerators, bool Mnemonics)
        {
            _tabStops = TabStops;
            _hasAccelerators = Accelerators;
            _hasMnemonics = Mnemonics;

            if (_hasAccelerators)
            {
                InitAccelerators();
            }

            if (_hasMnemonics)
            {
                InitMnemonics();
            }

           
            HwndWrapper hwndWrapper = new HwndWrapper(
                0,                          // classStyle
                NativeConstants.WS_CHILD,   // style
                0,                          // exStyle
                0,                          // x
                0,                          // y
                400,                        // width
                150,                        // height
                "Creative title",           // title
                NativeConstants.HWND_MESSAGE,// parent
                null                        // hooks
                );

            _hwnd = hwndWrapper.Handle;

        }

        //void mnemonicEventHandler(object o, MnemonicEventArgs args)
        //{
        //    CoreLogger.LogStatus("***HostedHwnd got mnemonic!");
        //}

        #region Accelerators

        private void InitAccelerators()
        {

            // Create accelerator table. IKIS.TranslateAccelerator will call NativeMethods.TranslateAccelerator which will
            // make use of this table.
            NativeStructs.ACCEL[] accelArray = new NativeStructs.ACCEL[2];
            NativeStructs.ACCEL accel;

            //
            // Unique accelerator for HostedHwndControl Ctrl-H
            //
            accel = new NativeStructs.ACCEL();
            accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
            accel.key = (short)(NativeMethods.VkKeyScan('H') & 0x00FF);
            accel.cmd = 0x00FF;

            accelArray[0] = accel;

            // Common accelerator Ctrl-O
            accel = new NativeStructs.ACCEL();
            accel.fVirt = NativeConstants.FVIRTKEY | NativeConstants.FCONTROL;
            accel.key = (short)(NativeMethods.VkKeyScan('O') & 0x00FF);
            accel.cmd = 0x0042; 

            accelArray[1] = accel;

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

        #endregion // Accelerators

        #region HwndHost

        /// <summary>
        /// Build the hwnd under the given parent and return a handle to it.
        /// </summary>
        protected override HandleRef BuildWindowCore(HandleRef parent)
        {
            // 
            // Create hosted hwnd.
            //

            CoreLogger.LogStatus("HostedHwnd.BuildWindowCore");

            //HwndWrapper hwndWrapper = new HwndWrapper(
            //    0,                          // classStyle
            //    NativeConstants.WS_CHILD,   // style
            //    0,                          // exStyle
            //    0,                          // x
            //    0,                          // y
            //    400,                        // width
            //    150,                        // height
            //    "Creative title",           // title
            //    parent.Handle,              // parent
            //    null                        // hooks
            //    );

            //_hwnd = hwndWrapper.Handle;
            NativeMethods.SetParent(new HandleRef(this, _hwnd), parent);

            NativeMethods.ShowWindow(new HandleRef(this, _hwnd), NativeConstants.SW_SHOW);
            CoreLogger.LogStatus("Created hosted hwnd");

            // Add HwndHost message hook.
            this.MessageHook += new HwndSourceHook(HelperHwndSourceHook);

            if (_tabStops == true)
            {
                // Create button for t/m/a testing.
                _buttons = new List<IntPtr>();

                IntPtr button = NativeMethods.CreateWindowEx(
                    0,                          // dwExStyle
                    "Button",                   // lpszClassName
                    "HwndHost Hwnd button #" + (_buttons.Count + 1),  // lpszWindowName
                    NativeConstants.WS_CHILD,   // style
                    0,                          // x
                    0,                          // y
                    400,                        // width
                    30,                         // height
                    _hwnd,         // hWndParent
                    IntPtr.Zero,                // hMenu
                    IntPtr.Zero,                // hInst
                    null                        // pvParam
                    );

                NativeMethods.ShowWindow(new HandleRef(this, button), NativeConstants.SW_SHOW);

                _buttons.Add(button);

                _buttonHwndSubclassHook = new HwndWrapperHook(buttonHelper);
                _buttonSubclass = new HwndSubclass(_buttonHwndSubclassHook);
                _buttonSubclass.Attach(button);
            }

            CoreLogger.LogStatus("HwndHost finished");

            return new HandleRef(null, _hwnd);
        }

        /// <summary>
        /// Hook for each win32 button in the hosted control.
        /// </summary>
        IntPtr buttonHelper(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // CoreLogger.LogStatus("HostedHwndControl.buttonHelper");
            switch (msg)
            {
                case NativeConstants.WM_SETFOCUS:
                    CoreLogger.LogStatus("HostedHwndControl.buttonHelper: SETFOCUS message");
                    _tabCount++;
                    break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        IntPtr HelperHwndSourceHook(IntPtr window, int msg, IntPtr wParam,
            IntPtr lParam, ref bool handled)
        {
            // ButtonHook(window, message, firstParam, secondParam, ref handled);
             switch (msg)
            {
                case NativeConstants.WM_KEYDOWN:
                    // CoreLogger.LogStatus("HostedHwndControl.HelperHwndSourceHook WM_KEYDOWN ");          
                    break;

                case 0x102: // WM_CHAR
                case 0x103: // WM_DEADCHAR
                case 0x106: // WM_SYSCHAR
                case 0x107: // WM_SYSDEADCHAR
                    // CoreLogger.LogStatus("HostedHwndControl.HelperHwndSourceHook: Char message");
                    break;

                case NativeConstants.WM_COMMAND:
                case NativeConstants.WM_SYSCOMMAND:
                    // CoreLogger.LogStatus("HostedHwndControl.HelperHwndSourceHook: Command message" );
                     
                    // Mask notification code (high order word) of Accelerator or Menu message source.
                    int command = NativeMethods.IntPtrToInt32(wParam) & 0x0000FFFF;

                    switch (command)
                    {
                        case 0x00FF:
                            _gotUniqueCommand++;
                            handled = true;
                            break;
                        case 0x0042:
                            _gotCommonCommand++;
                            handled = true;
                            break;
                        default:
                            break;
                    }
                    
                    break;

                 case NativeConstants.WM_SETFOCUS:
                     //CoreLogger.LogStatus("HostHook WM_SETFOCUS");
                     //_tabCount++;
                     break;
                default:
                    break;
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// </summary>
        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            if (_buttonSubclass != null)
            {
                _buttonSubclass.Detach(true);
            }

            NativeMethods.DestroyWindow(hwnd);
        }
        #endregion // HwndHost

        #region IKeyboardInputSink


        // Methods
        bool IKeyboardInputSink.HasFocusWithin()
        {
            IntPtr focusedHandle = NativeMethods.GetFocus();

            // Check if the hosted hwnd itself has focus.
            if (focusedHandle == _hwnd)
            {
                return true;
            }

            // Check if our Win32 buttons have focus.
            if (_buttons != null)
            {
                foreach (IntPtr button in _buttons)
                {
                    if (focusedHandle == button)
                    {
                        // CoreLogger.LogStatus("HostedHwndControl.GetFocus - hwnd button has focus");
                        return true;
                    }
                }
            }

            if (_childHwndSources != null)
            {
                foreach (SourcedAvalonControl childHwndSource in _childHwndSources)
                {
                    if (((IKeyboardInputSink)childHwndSource).HasFocusWithin() == true)
                    {
                        // CoreLogger.LogStatus("HostedHwndControl.GetFocus - child sink has focus.");
                        return true;
                    }
                }
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
                    // CoreLogger.LogStatus("msg.wParam " + NativeMethods.IntPtrToInt32(msg.wParam)+ " mnemonicLetter " + mnemonicLetter);

                    if ((NativeMethods.IntPtrToInt32(msg.wParam) == mnemonicLetter) && (modifiers == ModifierKeys.Alt))
                    {
                        CoreLogger.LogStatus("Got Alt-" + (char)mnemonicLetter + " mnemonic!!!");
                        _gotMnemonic++;
                        return true;
                    }
                }
            }
            
            //CoreLogger.LogStatus("Got a different mnemonic " + KeyInterop.KeyFromVirtualKey(NativeMethods.IntPtrToInt32(msg.wParam)) + " " + Key.N);
            //CoreLogger.LogStatus("Passing mnemonics to child sinks");

            // Call OnMnemonic on children until one handles it.
            if (_childHwndSources != null)
            {
                foreach (SourcedAvalonControl childHwndSource in _childHwndSources)
                {
                    if (((IKeyboardInputSink)childHwndSource).OnMnemonic(ref msg, modifiers) == true)
                    {
                        return true;
                    }
                }
            }

            // This control and none of its children handled the mnemonic.
            return false;
        }

        IKeyboardInputSite IKeyboardInputSink.RegisterKeyboardInputSink(IKeyboardInputSink sink)
        {
            //
            // Child HwndSources are registered by assigning a sink to them, this should not be called.
            //
            throw new Exception("HostedHwndControl.RegisterKeyboardInputSink: Child HwndSource called RegisterKeyboardInputSink");
        }




        /// <summary>
        /// Accept tab entry. 
        /// </summary>
        bool IKeyboardInputSink.TabInto(TraversalRequest request)
        {
            CoreLogger.LogStatus("HostedHwndControl.TabInto");

            IntPtr ptr = IntPtr.Zero;
            if (_buttons != null)
            {
                if (request.FocusNavigationDirection == FocusNavigationDirection.First || (request.FocusNavigationDirection == FocusNavigationDirection.Right))
                {
                    ptr = NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));
                    //_tabCount++;
                }
                else if (request.FocusNavigationDirection == FocusNavigationDirection.Last || (request.FocusNavigationDirection == FocusNavigationDirection.Left))
                {
                    ptr = NativeMethods.SetFocus(new HandleRef(null, _buttons[_buttons.Count - 1]));
                    //_tabCount++;
                }
                else
                {
                    throw new Exception("The tab request was not expected with the current values.  Mode: " + request.FocusNavigationDirection.ToString());
                }

                if (ptr != IntPtr.Zero)
                {
                    CoreLogger.LogStatus("  Tabbed into");
                    return true;
                }
            }
            else
            {
                // todo: tab into child sink when there are no tab stops.
            }
            return false;

        }


        bool IKeyboardInputSink.TranslateAccelerator(ref MSG msg, ModifierKeys modifiers)
        {
            // Verify message type.
            switch (msg.message)
            {              
                case NativeConstants.WM_KEYDOWN:
                    IntPtr focusedHandle = NativeMethods.GetFocus();
                    int index = _buttons.IndexOf(focusedHandle);

                    // CoreLogger.LogStatus("HostedHwndControl.TranslateAccelerator WM_KEYDOWN");
                    if (msg.wParam == new IntPtr(NativeConstants.VK_TAB))
                    {
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
                                        _tabCount++;
                                    }
                                }
                                else  // Tab into next sink.
                                {
                                    ((IKeyboardInputSink)_childHwndSources[index]).TabInto(new TraversalRequest(FocusNavigationDirection.First));
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
                                    ((IKeyboardInputSink)_childHwndSources[index - 1]).TabInto(new TraversalRequest(FocusNavigationDirection.Last));
                                }
                            }

                            // note that if focus isn't currently in one of the win32 _buttons[] we pass this message to the sink
                            // with focus (if any) below.
                            return true;
                        }
                        else if (_childHwndSources != null) // Pass tab to child sinks
                        {
                            foreach (SourcedAvalonControl childHwndSource in _childHwndSources)
                            {
                                if (((IKeyboardInputSink)childHwndSource).HasFocusWithin() == true)
                                {
                                    ((IKeyboardInputSink)childHwndSource).TranslateAccelerator(ref msg, modifiers);
                                    break;
                                }
                            }
                        }

                    }
                    // Not a tab, some other accelerator...
                    else
                    {
                        // If this HostedHwndControl has focus handle the accelerator natively.
                        if ((index >= 0) && (_acceleratorTable != null))
                        {
                            int virtualKey = NativeMethods.IntPtrToInt32(msg.wParam);
                            // _cache = KeyInterop.KeyFromVirtualKey(virtualKey);
                            //CoreLogger.LogStatus("Calling native translate accelerator");
                            if (0 != NativeMethods.TranslateAccelerator(new HandleRef(null, _hwnd), new HandleRef(null, _acceleratorTable.ACCELTable), ref msg))
                            {
                                CoreLogger.LogStatus("  HostedHwndControl.TranslateAccelerator - Accelerator handled by native TranslateAccelerator");
                                return true;
                            }
                        }
                        // Otherwise pass the accelerator to the child sink with focus.
                        else
                        {
                            //if (_childHwndSources != null) 
                            //{
                            //    foreach (SourcedAvalonControl childHwndSource in _childHwndSources)
                            //    {
                            //        if (((IKeyboardInputSink)childHwndSource).HasFocusWithin() == true)
                            //        {
                            //            CoreLogger.LogStatus("   HostedHwndControl.TranslateAccelerator - Passing accelerator to child sink");
                            //            ((IKeyboardInputSink)childHwndSource).TranslateAccelerator(ref msg, modifiers);
                            //            break;
                            //        }
                            //    }
                            //}
                        }
                    }
                    break;

                case NativeConstants.WM_KEYUP:
                case NativeConstants.WM_SYSKEYUP:
                case NativeConstants.WM_SYSKEYDOWN:
                    break;

                case NativeConstants.WM_SETFOCUS:
                    CoreLogger.LogStatus("HostedHnwdControl.TranslateAccelerator SETFOCUS");
                    break;
                default:
                    break;
            }

            int key = NativeMethods.IntPtrToInt32(msg.wParam);

            //CoreLogger.LogStatus("HostedHwndControl received a key message!");

           
            
            return false;                        
        }

        bool IKeyboardInputSink.TranslateChar(ref MSG msg, ModifierKeys modifiers)
        {
            CoreLogger.LogStatus("HostedHwndControl.TranslateChar");
            return false;
        }

        // The site given to this sink used to pass tabs to the parent and unregister.
        private IKeyboardInputSite _site = null;
        IKeyboardInputSite IKeyboardInputSink.KeyboardInputSite
        {
            get
            {
                return _site;
            }
            set
            {
                _site = value;
            }
        }

#endregion // IKeyboardInputSink

        #region IKeyboardInteropTestControl

        /// <summary>
        /// Tab testing helper. Set tab to first child in a control. 
        /// </summary>
        void IKeyboardInteropTestControl.SetFocusToFirstChild()
        {
            if (_buttons != null)
            {
                NativeMethods.SetFocus(new HandleRef(null, _buttons[0]));
            }
        }

        /// <summary>
        /// Reset recorded accelerators, mnemonics and tabs. 
        /// </summary>
        void IKeyboardInteropTestControl.ResetTestState()
        {
            _gotUniqueCommand = 0;
            _gotCommonCommand = 0;
            _gotMnemonic = 0;
            _tabCount = 0;

            if (_childHwndSources != null)
            {
                foreach (SourcedAvalonControl childSource in _childHwndSources)
                {
                    Win32KeyboardInputSite site = (Win32KeyboardInputSite)((IKeyboardInputSink)childSource).KeyboardInputSite;
                    site.tabCount = 0;
                }
            }
        }

        /// <summary>
        /// Return the number of local tab stop in the test control. 
        /// Does not count sinks.
        /// </summary>
        int IKeyboardInteropTestControl.LocalTabStops()
        {
            int count = 0;
            if (_buttons != null)
            {
                count += _buttons.Count;
            }

            return count;
        }

        /// <summary>
        /// Return the number of tabs hit in this test control. 
        /// Does not count sinks.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedLocalTabs
        {
            get
            {
                int totalTabs = _tabCount;

                return totalTabs;
            }
        }

        /// <summary>
        /// Should return true if specified accelerator was recorded by the control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedUniqueAccelerator
        {
            get
            {
                return _gotUniqueCommand;
            }
        }

        /// <summary>
        /// Should return true if the common test accelerator was recorded by this control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedCommonAccelerator
        {
            get
            {
                return _gotCommonCommand;
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

        /// <summary>
        /// Returns true if specified mnemonic was recorded by the control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedMnemonic
        {
            get
            {
                return _gotMnemonic;
            }
        }        


        /// <summary>
        /// 
        /// </summary>
        IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
        {
            if (childType != "SourcedAvalon")
            {
                return null;
            }

            //
            // Create an HwndSource IKeyboardInputSink child of the top level Hwnd.
            //

            int sourcePosition = 90; // If this is the second child put it below second tab stop.
            if (_childHwndSources == null)
            {
                _childHwndSources = new List<SourcedAvalonControl>();
                sourcePosition = 30; // This is first child, put it below first tab stop.
            }


            // Set HwndSource parameters.
            HwndSourceParameters hwndParams = new HwndSourceParameters("Hwnd Sourced Avalon", 400, 30);
            hwndParams.ParentWindow = _hwnd;
            hwndParams.SetPosition(0, sourcePosition);
            hwndParams.WindowStyle = NativeConstants.WS_VISIBLE | NativeConstants.WS_CHILD;

            // Create the HwndSource.
            SourcedAvalonControl childHwndSource = new SourcedAvalonControl(hwndParams, _tabStops, _hasAccelerators, _hasMnemonics);

            // Add it to the child sources list.
            _childHwndSources.Add(childHwndSource);


            // todo: what if there are no tab stops!!!!

            IntPtr nextHwnd = IntPtr.Zero;
            IntPtr prevHwnd = IntPtr.Zero;
            if (_buttons != null)
            {
                prevHwnd = _buttons[_buttons.Count - 1];
            }

            if (_tabStops == true)
            {
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
                nextHwnd = button;

                _buttonHwndSubclassHook = new HwndWrapperHook(buttonHelper);
                _buttonSubclass = new HwndSubclass(_buttonHwndSubclassHook);
                _buttonSubclass.Attach(button);

            }



            // Create a keyboard input site for the HwndSource sink and connect it.
            // When there are no more tab stops in the child focus will move to the button hwnd.

            ((IKeyboardInputSink)childHwndSource).KeyboardInputSite = new Win32KeyboardInputSite(prevHwnd, childHwndSource, nextHwnd);

            return (IKeyboardInteropTestControl)childHwndSource;
        }

        #endregion // IKeyboardTestConrol



    }



    /// <summary>
    /// Control to source Avalon in an Hwnd.
    /// </summary>
    public class SourcedAvalonControl : HwndSource, IKeyboardInputSink, IKeyboardInteropTestControl
    {
        private StackPanel _stackPanel = null;

        private List<Button> _buttons = null;

        private List<HostedHwndControl> _childHwndHosts = null;

        // Add accelerators, mnemonics or tab stops.
        bool _hasAccelerators;
        bool _hasMnemonics;
        bool _tabStops;

        // Record if window caught accelerators.
        int _gotCommonCommand = 0;
        int _gotUniqueCommand = 0;
        int _gotMnemonic = 0;
       
        int _tabCount;

        /// <summary>
        /// Constructor, typical HwndSource plus some Avalon bits.
        /// </summary>
        public SourcedAvalonControl (HwndSourceParameters p, bool tabStops, bool Accelerators, bool Mnemonics) : base(p)
        {
            _tabStops = tabStops;
            _hasAccelerators = Accelerators;
            _hasMnemonics = Mnemonics;

            //
            // Create Avalon contents.
            //
            _stackPanel = new StackPanel();
            _stackPanel.Background = Brushes.Yellow;

            if (_tabStops == true)
            {
                _buttons = new List<Button>();

                Button button = new Button();
                button.Content = "Sourced Avalon Control Button #1";
                button.Height = 30;
                button.Focusable = true;
                _stackPanel.Children.Add(button);

                _buttons.Add(button);

                button.GotFocus += this.OnGotFocus;
            }

            this.RootVisual = (Visual)_stackPanel;

            if (_hasAccelerators == true)
            {
                InitAccelerators();
            }
        }

        void OnGotFocus(object sender, RoutedEventArgs a)
        {
            CoreLogger.LogStatus("SourcedAvalonControl.OnGotFocus");
            _tabCount++;
        }

        #region Accelerators

        /// <summary>
        /// Attaches a unique accelerator and a common accelerator to the AvalonWindow.
        /// </summary>
        private void InitAccelerators()
        {
            //
            // Hookup unique command/accelerator Ctrl-A
            //

            // Create command.
            RoutedCommand uniqueCommand = new RoutedCommand("UniqueCommand", this.GetType(), null);

            // Create binding.
            CommandBinding uniqueCommandBinding = new CommandBinding(uniqueCommand);

            // Set up key binding to command.
            KeyBinding uniqueKeyBinding = new KeyBinding(uniqueCommand, Key.S, ModifierKeys.Control);

            // Attach event to command binding.
            uniqueCommandBinding.Executed += new ExecutedRoutedEventHandler(OnUniqueCommand);

            // Add command binding to the control.
            _buttons[0].CommandBindings.Add(uniqueCommandBinding);
            _buttons[0].InputBindings.Add(uniqueKeyBinding);

            //
            // Hookup common command/accelerator Ctrl-O
            //
            RoutedCommand commonCommand = new RoutedCommand("CommonCommand", this.GetType(), null);
            CommandBinding commonCommandBinding = new CommandBinding(commonCommand);
            KeyBinding commonKeyBinding = new KeyBinding(commonCommand, Key.O, ModifierKeys.Control);
            commonCommandBinding.Executed += new ExecutedRoutedEventHandler(OnCommonCommand);
            _buttons[0].CommandBindings.Add(commonCommandBinding);
            _buttons[0].InputBindings.Add(commonKeyBinding);
        }

        private void OnUniqueCommand(object target, ExecutedRoutedEventArgs args)
        {
            CoreLogger.LogStatus("Executed unique command on target " + target);
            _gotUniqueCommand++;
        }

        private void OnCommonCommand(object target, ExecutedRoutedEventArgs args)
        {
            CoreLogger.LogStatus("Executed common command on target " + target);
            _gotCommonCommand++;
        }

#endregion // Accelerators

        void onClickButton(object o, RoutedEventArgs args)
        {
            CoreLogger.LogStatus("SourcedAvalonControl mnemonic raised!");
            _gotMnemonic++;
        }

        #region IKeyboardInteropTestControl
        /// <summary>
        /// Tab testing helper. Set tab to first child in a control. 
        /// </summary>
        void IKeyboardInteropTestControl.SetFocusToFirstChild()
        {
            if (_buttons != null)
            {
                _buttons[0].Focus();
            }
        }

        /// <summary>
        /// Reset recorded accelerators, mnemonics and tabs. 
        /// </summary>
        void IKeyboardInteropTestControl.ResetTestState()
        {
            _gotUniqueCommand = 0;
            _gotCommonCommand = 0;
            _gotMnemonic = 0;
            _tabCount = 0;
        }

        /// <summary>
        /// Return the number of local tab stop in the test control. 
        /// Does not count sinks.
        /// </summary>
        int IKeyboardInteropTestControl.LocalTabStops()
        {
            if (_buttons != null)
            {
                return _buttons.Count;
            }

            return 0;
        }

        /// <summary>
        /// Return the number of tabs hit in this test control. 
        /// Does not count sinks.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedLocalTabs
        {
            get
            {
                return _tabCount;
            }
        }

        
        /// <summary>
        /// Should return true if specified accelerator was recorded by the control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedUniqueAccelerator
        {
            get
            {
                return _gotUniqueCommand;
            }
        }

        /// <summary>
        /// Should return true if the common test accelerator was recorded by this control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedCommonAccelerator
        {
            get
            {
                return _gotCommonCommand;
            }
        }

        /// <summary>
        /// Add a mnemonic to the control.
        /// </summary>
        void IKeyboardInteropTestControl.AddMnemonic(string letter, ModifierKeys modifier)
        {
            AccessKeyManager.Register(letter, _buttons[0]);

            _buttons[0].Click += new RoutedEventHandler(onClickButton);
        }

        /// <summary>
        /// Returns true if specified mnemonic was recorded by the control.
        /// </summary>
        int IKeyboardInteropTestControl.RecordedMnemonic
        {
            get
            {
                return _gotMnemonic;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        IKeyboardInteropTestControl IKeyboardInteropTestControl.AddChild(string childType)
        {
            if (childType != "HostedHwnd")
            {
                return null;
            }

            // If this is the first child create child list.
            if (_childHwndHosts == null)
            {
                _childHwndHosts = new List<HostedHwndControl>();
            }

            // Create child HostedHwndControl with a tab stop.
            HostedHwndControl childHwndHost = new HostedHwndControl(_tabStops, _hasAccelerators, _hasMnemonics);
            childHwndHost.Height = 30;
            _stackPanel.Children.Add(childHwndHost);

            // Add child to host list.
            _childHwndHosts.Add(childHwndHost);

            if (_tabStops == true)
            {
                // Add a button as a tab stop.
                Button button = new Button();
                button.Content = "Sourced Avalon Control Button #" + (_buttons.Count + 1);
                button.Height = 30;
                button.Focusable = true;
                _stackPanel.Children.Add(button);

                _buttons.Add(button);

                button.GotFocus += this.OnGotFocus;
            }

            return (IKeyboardInteropTestControl)childHwndHost;
        }

        #endregion // IKeyboardInteropTestControl

    }
}
