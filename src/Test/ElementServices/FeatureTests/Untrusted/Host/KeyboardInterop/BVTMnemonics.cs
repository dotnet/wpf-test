// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************************
*    Source Control Information
*    
 
  
*    Revision:         $Revision: 1 $
 
*
******************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Automation;
using System.Windows.Media.Animation;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

namespace Avalon.Test.Hosting
{

    /// <summary>
    /// Class that contains BVT test cases for Mnemonics and Interop
    /// </summary>
    /// <remarks>
    ///</remarks>
    [TestDefaults]
    public class BVTMnemonics : TestCase
    {
        
        /// <summary>
        /// </summary>
        public BVTMnemonics() :base(TestCaseType.None){}
        
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        override public void Run()
        {}



        /// <summary>
        /// Register Three Mnmonics on the HwndHost. Later inject Input for raise the ACCEL for with and without the 
        /// hwndhost having
        /// </summary>
        [TestAttribute(0, @"Hosting\KeyboardInterop", TestCaseSecurityLevel.FullTrust, "TestMnemonics", Area = "AppModel")]
        public void TestMnemonics()
        {
            CoreLogger.BeginVariation();

            KeyboardHelper.IsSynchronous = false;

            MainDispatcher.BeginInvoke(DispatcherPriority.Normal,new DispatcherOperationCallback(_testMnemonics), null);
            TestLog.Current.LogStatus("Dispatcher.Run");
            Dispatcher.Run();

            TestLog.Current.LogStatus("Last Validation for Mnemonics that fired,  Expected: 0xFFF; Real: " + _count.ToString());

            if (_count != 0xFFF)
            {
                throw new Microsoft.Test.TestValidationException("Not all the Mnemonics where fired");
            }

            CoreLogger.EndVariation();
        }

        Button _startbtn;

        /// <summary>
        /// Creating the UIObject inside of a context
        /// </summary>
        object _testMnemonics(object o)
        {
            Button button; // Ref to a Button use;

            _window = new Window();
            StackPanel sp = new StackPanel();
            _window.Content = sp;

            button = new Button();
            button.Content = "Button One";
            sp.Children.Add(button);

            _startbtn = button;

            SingleHwndControl h = new SingleHwndControl();
            h.Focusable = true;
            sp.Children.Add(h);

            // Creating a ACCEL table for the HwndHost
            MnemonicsTable list = new MnemonicsTable();

            list.Add((int)'L', ModifierKeys.Shift, new MnemonicsEventHandler(mNemonicsHandler));
            list.Add((int)'m', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandler));

            list.Add((int)'n', ModifierKeys.Alt , new MnemonicsEventHandler(mNemonicsHandler));

            TestLog.Current.LogStatus("Register Mnemonics on the HwndHost");
            h.RegisterMnemonics(list);

            button = new Button();
            button.Content = "Button Two";
            sp.Children.Add(button);

            // Start Injecting Input
            testElement t = new testElement();
            t.Rendered += new EventHandler(injectControl_M);
            sp.Children.Add(t);

            _window.Show();

            _window.Activate();
            return null;
        }

        /// <summary>
        /// Handler called on a mnemonic event happen on the HwndHost
        /// </summary>
        void mNemonicsHandler(object o, MnemonicsEventArgs args)
        {
            TestLog.Current.LogStatus("Mnemonics: " +  args.Letter);

            _exitCount++;

            HwndHost hwndHost = (HwndHost)o;
            
            if (args.Letter == (int)'m')
            {
                _count |= 0x00F;
                
                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _count |= 0x10000;
                }
            }

            if (args.Letter == (int)'L')
            {
                _count |= 0x0F0;
                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _count |= 0x10000;
                }
            }

            if (args.Letter == (int)'n')
            {
                _count |= 0xF00;
                if (!InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _count |= 0x20000;
                }
            }

            if (_exitCount == 3)
                MainDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback (exit), null);

        }

        int _exitCount = 0;

        /// <summary>
        /// Code for inject Mnemonics for the first test case
        /// </summary>
        void injectControl_M(object o, EventArgs a)
        {
            _startbtn.Focus();

            TestLog.Current.LogStatus("Start Simulating Input");

            // Mnemonic Ctrl-M
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.M);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);    

            // Tab
            KeyboardHelper.TypeKey(Key.Tab);

            // Mnemonic Ctrl-Alt-N
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.N);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);    

            // Tab
            KeyboardHelper.TypeKey(Key.Tab);

            // Mnemonic Shift - L
            KeyboardHelper.PressKey(Key.LeftShift);
            KeyboardHelper.TypeKey(Key.L);
            KeyboardHelper.ReleaseKey(Key.LeftShift);    

            TestLog.Current.LogStatus("End Simulating Input");
        }


        /// <summary>
        /// This is use to exit the dispatcher if a Timer or 30 seconds
        /// </summary>
        void _exit(object o, EventArgs a)
        {
            MainDispatcher.InvokeShutdown();
        }

        /// <summary>
        /// Used as convinience to exit the dispatcher Async
        /// </summary>
        object exit(object o)
        {
           Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            return null;
        }


        /// <summary>
        /// Three HwndHost controls with two mnemonics each.  Raising each mnemonic without tab- in to any control
        /// </summary>
        [TestAttribute(0, @"Hosting\KeyboardInterop", TestCaseSecurityLevel.FullTrust, "MultipleHosts", Area = "AppModel")]
        public void MultipleHosts()
        {
            CoreLogger.BeginVariation();

            KeyboardHelper.IsSynchronous = false;

            MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(_testTwo), null);
            TestLog.Current.LogStatus("Dispatcher.Run");
            Dispatcher.Run();

            TestLog.Current.LogStatus("Last Validation for Mnemonics that fired,  Expected: 0x3f; Real: " + _count.ToString());

            if (_count != 0x3F)
            {
                throw new Microsoft.Test.TestValidationException("Not all the Mnemonics where fired");
            }

            if (_failed)
            {
                throw new Microsoft.Test.TestValidationException("HasFocus Call failed");
            }

            TestLog.Current.LogStatus(_count.ToString());
            CoreLogger.EndVariation();
        }


        object _testTwo(object o)
        {
            Button button; // Use to create buttons between HwndHost controls

            _window = new Window();
            StackPanel sp = new StackPanel();
            _window.Content = sp;

            button = new Button();
            button.Content = "Button One";
            sp.Children.Add(button);

            // Adding Host One with L and M mnemonics
            SingleHwndControl h = new SingleHwndControl();
            h.Focusable = true;
            sp.Children.Add(h);

            MnemonicsTable listOne = new MnemonicsTable();

            listOne.Add((int)'l', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandlerTwo));
            listOne.Add((int)'m', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandlerTwo));

            TestLog.Current.LogStatus("Register Mnemonics on the HwndHost One");
            h.RegisterMnemonics(listOne);

            button = new Button();
            button.Content = "Button Two";
            sp.Children.Add(button);

            // Adding Host Two with K and N mnemonics
            SingleHwndControl hTwo = new SingleHwndControl();
            hTwo.Focusable = true;
            sp.Children.Add(hTwo);

            MnemonicsTable listTwo = new MnemonicsTable();

            listTwo.Add((int)'k', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandlerTwo));
            listTwo.Add((int)'n', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandlerTwo));

            TestLog.Current.LogStatus("Register Mnemonics on the HwndHost Two");
            hTwo.RegisterMnemonics(listTwo);

            // End Adding Host Two with K and N mnemonics

            button = new Button();
            button.Content = "Button Three";
            sp.Children.Add(button);

            // Adding Host Two with J and H mnemonics
            SingleHwndControl hThree = new SingleHwndControl();
            hThree.Focusable = true;
            sp.Children.Add(hThree);

            MnemonicsTable listThree = new MnemonicsTable();

            listThree.Add((int)'J', ModifierKeys.Alt | ModifierKeys.Shift, new MnemonicsEventHandler(mNemonicsHandlerTwo));
            listThree.Add((int)'h', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandlerTwo));

            TestLog.Current.LogStatus("Register Mnemonics on the HwndHost Three");
            hThree.RegisterMnemonics(listThree);

            // End Adding Host Two with J and H mnemonics

            // Start Injecting Input
            testElement t = new testElement();
            t.Rendered += new EventHandler(mnemonicTwoInjectInput);
            sp.Children.Add(t);

            _window.Show();

            // Making sure the window is Active            
            _window.Activate();

            return null;
        }


        void mnemonicTwoInjectInput(object o, EventArgs a)
        {
            TestLog.Current.LogStatus("Start Simulating Input");

            // Mnemonic ALT-L
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.L);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);    

            // Mnemonic ALT-K
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.K);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);    

            // Mnemonic ALT-SHIFT-J
            KeyboardHelper.PressKey(Key.LeftShift);
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.J);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);                            
            KeyboardHelper.ReleaseKey(Key.LeftShift);    

            // Mnemonic Alt-M
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.M);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);    

            // Mnemonic ALT-N
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.N);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);    

            // Mnemonic Alt-H
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.H);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);    
        }

        void mNemonicsHandlerTwo(object o, MnemonicsEventArgs args)
        {
            TestLog.Current.LogStatus("Mnemonics: " + args.Letter.ToString());

            _exitCount++;

            HwndHost hwndHost = (HwndHost)o;

            if (args.Letter == (int)'l')
            {
                _count |= 0x001;

                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _failed = true;
                    TestLog.Current.LogStatus("Failure the HwndHost.HasFocus return true");
                }
            }

            if (args.Letter == (int)'J')
            {
                _count |= 0x002;

                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _failed = true;
                    TestLog.Current.LogStatus("Failure the HwndHost.HasFocus return true");
                }
            }

            if (args.Letter == (int)'k')
            {
                _count |= 0x004;

                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _failed = true;
                    TestLog.Current.LogStatus("Failure the HwndHost.HasFocus return true");
                }
            }

            if (args.Letter == (int)'h')
            {
                _count |= 0x008;

                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _failed = true;
                    TestLog.Current.LogStatus("Failure the HwndHost.HasFocus return true");
                }
            }

            if (args.Letter == (int)'n')
            {
                _count |= 0x010;

                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _failed = true;
                    TestLog.Current.LogStatus("Failure the HwndHost.HasFocus return true");
                }
            }

            if (args.Letter == (int)'m')
            {
                _count |= 0x020;

                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _failed = true;
                    TestLog.Current.LogStatus("Failure the HwndHost.HasFocus return true");
                }

            }

            // This is done so we can quit the pump after 6 mnemonics
            if (_exitCount == 6)
                MainDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(exit), null);
        }

        /// <summary>
        /// Register an AccessKey on Avalon and register the same Mnemonic on a HwndHost
        /// </summary>
        [TestAttribute(0, @"Hosting\KeyboardInterop", TestCaseSecurityLevel.FullTrust, "CollidingAccessKeyAndMnemonic", Area = "AppModel")]
        public void ColladingAccessKeyAndMnemonic()
        {
            CoreLogger.BeginVariation();

            KeyboardHelper.IsSynchronous = false;

            MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(_testThree), null);
            TestLog.Current.LogStatus("Dispatcher.Run");
            Dispatcher.Run();

            TestLog.Current.LogStatus("Last Validation for Mnemonics that fired,  Expected: 0x3; Real: " + _count.ToString());

            if (_count != 0x3)
            {
                throw new Microsoft.Test.TestValidationException("Not all the Mnemonics where fired");
            }

            if (_failed)
            {
                throw new Microsoft.Test.TestValidationException("Bad State");
            }

            TestLog.Current.LogStatus(_count.ToString());
            CoreLogger.EndVariation();
        }


        object _testThree(object o)
        {
            Button button; // Use to create buttons between HwndHost controls

            _window = new Window();
            StackPanel sp = new StackPanel();
            _window.Content = sp;

            button = new Button();
            button.Content = "Button One";
            button.Click += new RoutedEventHandler(buttonHandler);
            sp.Children.Add(button);

            AccessKeyManager.Register("L", button);

            // Adding Host One with L and M mnemonics
            SingleHwndControl h = new SingleHwndControl();
            h.Focusable = true;
            sp.Children.Add(h);

            MnemonicsTable listOne = new MnemonicsTable();

            listOne.Add((int)'l', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandlerThree));
            listOne.Add((int)'m', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandlerThree));

            TestLog.Current.LogStatus("Register Mnemonics on the HwndHost One");
            h.RegisterMnemonics(listOne);

            // Start Injecting Input
            testElement t = new testElement();
            t.Rendered += new EventHandler(mnemonicThreeInjectInput);
            sp.Children.Add(t);

            _window.Show();

            // Making sure the window is Active            
            _window.Activate();

            return null;
        }


        void buttonHandler(object o, RoutedEventArgs args)
        {
            if ((_count & 0x2) == 2)
                _failed = true;

            TestLog.Current.LogStatus("OnButton Click has raised");
            _count |= 0x2;
        }


        void mNemonicsHandlerThree(object o, MnemonicsEventArgs args)
        {
            TestLog.Current.LogStatus("Mnemonics: " + args.Letter.ToString());
            _exitCount++;

            HwndHost hwndHost = (HwndHost)o;

            if (args.Letter == (int)'l')
            {
                _failed = true;
            }

            if (args.Letter == (int)'m')
            {
                _count |= 0x001;

                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _failed = true;
                    TestLog.Current.LogStatus("Failure the HwndHost.HasFocus return true");
                }
            }

            // This is done so we can quit the pump after 1 mnemonic
            if (_exitCount == 1)
                MainDispatcher.BeginInvokeShutdown(DispatcherPriority.SystemIdle);
        }

        void mnemonicThreeInjectInput(object o, EventArgs a)
        {
            // Mnemonic ALT-L
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.L);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);    
            
            // Mnemonic ALT-M
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.M);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);    
        }

        bool _failed = false;

        Window _window;

        int _count = 0;
    }

}
