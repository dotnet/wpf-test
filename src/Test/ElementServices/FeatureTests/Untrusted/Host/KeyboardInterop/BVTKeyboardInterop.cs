// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Automation;
using System.Windows.Media.Animation;
using System.ComponentModel;

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
    /// Simple Hello UIElement to render something on the screen
    /// </summary>
    public class testElement : System.Windows.FrameworkElement
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public testElement(){}

        /// <summary>
        /// Event where tells you that the control is already rendered.
        /// </summary>
        public event EventHandler Rendered;

        /// <summary>
        /// Event where tells you that the control is already rendered.
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            Size desiredSize = new Size(0,0);
            return desiredSize;
        }

        /// <summary>
        /// Rendered method that it is called by the MIL
        /// </summary>
        /// <param name="ctx"></param>
        protected override void OnRender(DrawingContext ctx)
        {
                       
            if(this.Rendered != null && !_painted)
            {
                _painted = true;

                ThreadingHelper.DispatcherTimerHelper(DispatcherPriority.SystemIdle,new TimeSpan(0,0,0,0,3010),new EventHandler(aSync), null);
            }
        }

        bool _painted = false;

        void aSync(object o, EventArgs args)
        {
            DispatcherTimer t = o as DispatcherTimer;
            t.Stop();
            this.Rendered(this,EventArgs.Empty);
        }

    }

    /// <summary>
    ///     
    ///     
    /// </summary>
    /// <remarks>
    ///</remarks>
    [TestDefaults]
    public class BVTKeyboardInterop : TestCase
    {
        
        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        public BVTKeyboardInterop() :base(TestCaseType.ContextSupport){}
        
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        override public void Run()
        {}


        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        [Test(0, @"Hosting\KeyboardInterop", TestCaseSecurityLevel.FullTrust, "RunTab", Area = "AppModel", Disabled = true)]
        public void RunTab()
        {
            CoreLogger.BeginVariation();
            MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(_runTab), null);
            Dispatcher.Run();

            if (_count != 15) 
                throw new Microsoft.Test.TestValidationException("One of the buttons were no pressed " + _count.ToString());
            CoreLogger.EndVariation();
        }

        Window _window;

        int _count = 0;

        Button _startbtn;
        
        object _runTab(object o)
        {
             _window = new Window();
            _window.Show();

            StackPanel panel = new StackPanel();
            _window.Content = panel;
 
            Button b = new Button();
            b.Click += new RoutedEventHandler(ClickLastTab);
            b.Content = "Button 1";
            panel.Children.Add(b);
            _startbtn = b;

            b = new Button();
            b.Content = "Button 2";
            panel.Children.Add(b);

            StackPanel panelOne = new StackPanel();
            AvalonHwndControl cOne = new AvalonHwndControl();
            cOne.Height = 100;
            panel.Children.Add(cOne);
            cOne.RootVisual = panelOne;
            cOne.Focusable = true;

            b = new Button();
            b.Click += new RoutedEventHandler(ClickInsideOne);
            b.Content = "ButtonOne  Inside";
            panelOne.Children.Add(b);

            b = new Button();
            b.Click += new RoutedEventHandler(ClickafterShiftTab);
            b.Content = "ButtonOne Inside";
            panelOne.Children.Add(b);

            b = new Button();
            b.Content = "Button 3";
            panel.Children.Add(b);

            StackPanel panelTwo = new StackPanel();
            AvalonHwndControl cTwo = new AvalonHwndControl();
            cTwo.Height = 100;
            cTwo.Focusable = true;
            panel.Children.Add(cTwo);
            cTwo.RootVisual = panelTwo;

            b = new Button();
            b.Click += new RoutedEventHandler(ClickInsideTwo);
            b.Content = "ButtonTwo  Inside";
            panelTwo.Children.Add(b);

            testElement t = new testElement();
            t.Rendered += new EventHandler(injectFirstTab);
            panel.Children.Add(t);

            _window.Activate();
            return null;
        }



        void _exit(object o, EventArgs a)
        {
            MainDispatcher.InvokeShutdown();
        }
        
        void injectFirstTab(object o, EventArgs a)
        {

            bool bFocus = _startbtn.Focus();
            CoreLogger.LogStatus("IsFocus = " + bFocus);
            CoreLogger.LogStatus("Injecting first {Tab,Tab,Space}");

            Key[] keys ={Key.Tab, Key.Tab,Key.Space};
            KeyboardHelper.TypeKey(keys);

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(injectSecondTab), null);
        }


        void ClickInsideOne(object o, RoutedEventArgs args)
        {
            _count |= 1;
            CoreLogger.LogStatus("Button Click 1");
        }


       object injectSecondTab(object o)
        {

            CoreLogger.LogStatus("Injecting Second {Tab,Tab,Tab,Space}");

            Key[] keys ={Key.Tab, Key.Tab, Key.Tab, Key.Space};

            KeyboardHelper.TypeKey(keys);

            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(injectThirdTab), null);
            return null;
        }

        void ClickInsideTwo(object o, RoutedEventArgs args)
        {
            _count |= 2;
            CoreLogger.LogStatus("Button Click 2");            
        }

        object injectThirdTab(object o)
        {
            CoreLogger.LogStatus("Injecting Third {Tab,Space}");

            Key[] keys ={Key.Tab, Key.Space};

            KeyboardHelper.TypeKey(keys);

            return null;
        }


        void ClickLastTab(object o, RoutedEventArgs args)
        {
            _count |= 4;

            CoreLogger.LogStatus("Button Click 3"); 
            CoreLogger.LogStatus("Injecting LastInput {LShift,Tab,Tab,Tab,LShift,Space}");

            Key[] keys = { Key.Tab, Key.Tab, Key.Tab };

            KeyboardHelper.PressKey(Key.LeftShift);
            KeyboardHelper.TypeKey(keys);
            KeyboardHelper.ReleaseKey(Key.LeftShift);          
            KeyboardHelper.TypeKey(Key.Space);
        }


        void ClickafterShiftTab(object o, RoutedEventArgs args)
        {
            _count |= 8;
            CoreLogger.LogStatus("Button Click 4");               
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(exit), null);
        }

        object exit(object o)
        {
           Microsoft.Test.Threading.DispatcherHelper.ShutDown();
            return null;
        }


        /// <summary>
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        [Test(0, @"Hosting\KeyboardInterop", TestCaseSecurityLevel.FullTrust, "TestMnemonics", Area = "AppModel")]
        public void TestMnemonics()
        {
            CoreLogger.BeginVariation();

            KeyboardHelper.IsSynchronous = false;

            MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(_testMnemonics), null);
            TestLog.Current.LogStatus("Dispatcher.Run");
            Dispatcher.Run();

            TestLog.Current.LogStatus("Last Validation for Mnemonics that fired,  Expected: 0xFFF; Real: " + _count.ToString());

            if (_count != 0xFFF)
            {
                throw new Microsoft.Test.TestValidationException("Not all the Mnemonics where fired");
            }

            CoreLogger.EndVariation();
        }


        object _testMnemonics(object o)
        {
            _window = new Window();
            StackPanel sp = new StackPanel();
            _window.Content = sp;

            Button b = new Button();
            b.Content = "Button One";
            sp.Children.Add(b);

            _startbtn = b;

            SingleHwndControl h = new SingleHwndControl();
            h.Focusable = true;
            sp.Children.Add(h);

            MnemonicsTable list = new MnemonicsTable();

            list.Add((int)'L', ModifierKeys.Shift, new MnemonicsEventHandler(mNemonicsHandler));
            list.Add((int)'m', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandler));
            list.Add((int)'n', ModifierKeys.Alt, new MnemonicsEventHandler(mNemonicsHandler));

            TestLog.Current.LogStatus("Register Mnemonics on the HwndHost");
            h.RegisterMnemonics(list);

            b = new Button();
            b.Content = "Button Two";
            sp.Children.Add(b);

            // Start Injecting Input
            testElement t = new testElement();
            t.Rendered += new EventHandler(injectControl_M);
            sp.Children.Add(t);

            _window.Show();

            _window.Activate();
            return null;
        }


        void mNemonicsHandler(object o, MnemonicsEventArgs args)
        {
            TestLog.Current.LogStatus("Mnemonics: " + args.Letter.ToString());

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
                MainDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback(exit), null);

        }

        int _exitCount = 0;

        void injectControl_M(object o, EventArgs a)
        {
            _startbtn.Focus();
            TestLog.Current.LogStatus("Start Simulating Input");

            // Mnemonic Alt-M
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.M);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);          

            // Tab
            KeyboardHelper.TypeKey(Key.Tab);

            // Mnemonic Alt-N
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
        /// Constructor.  On the base class pass TestCaseType.ContextSupport 
        /// </summary>
        [Test(0, @"Hosting\KeyboardInterop", TestCaseSecurityLevel.FullTrust, "TestAccelerators", Area = "AppModel")]
        public void TestAccelerators()
        {
            CoreLogger.BeginVariation();

            KeyboardHelper.IsSynchronous = false;

            MainDispatcher.BeginInvoke(DispatcherPriority.Normal, new DispatcherOperationCallback(_testAccelerator), null);
            TestLog.Current.LogStatus("Dispatcher.Run");
            Dispatcher.Run();

            TestLog.Current.LogStatus("Last Validation for Mnemonics that fired,  Expected: 0xFF00; Real: " + _count.ToString());

            if (_count > 0x10000)
            {
               throw new Microsoft.Test.TestValidationException("Wrong focus on the window");
            }
            if (_count != 0xFF00)
            {
                throw new Microsoft.Test.TestValidationException("Not all the Mnemonics where fired");
            }
            CoreLogger.EndVariation();
        }

  
        object _testAccelerator(object o)
        {
            _window = new Window();
            StackPanel sp = new StackPanel();
            _window.Content = sp;

            Button b = new Button();
            b.Content = "Button One";
            sp.Children.Add(b);

            _startbtn = b;

            SingleHwndControl h = new SingleHwndControl();
            h.Focusable = true;
            sp.Children.Add(h);

            List<AccelTest> list = new List<AccelTest>();

            AccelTest accelone = new AccelTest();
            accelone.fVirt = NativeConstants.FVIRTKEY |  NativeConstants.FSHIFT;
            accelone.key = (short)(NativeMethods.VkKeyScan('L') & 0x00FF);
            list.Add(accelone);

            accelone = new AccelTest();
            accelone.fVirt = NativeConstants.FVIRTKEY |  NativeConstants.FCONTROL ;
            accelone.key = (short)(NativeMethods.VkKeyScan('M') & 0x00FF);
            list.Add(accelone);

            accelone = new AccelTest();
            accelone.fVirt = NativeConstants.FVIRTKEY |  NativeConstants.FCONTROL | NativeConstants.FALT;
            accelone.key = (short)(NativeMethods.VkKeyScan('N') & 0x00FF);
            list.Add(accelone);

            accelone = new AccelTest();
            accelone.fVirt = NativeConstants.FVIRTKEY |  NativeConstants.FCONTROL | NativeConstants.FALT;
            accelone.key = (short)(NativeMethods.VkKeyScan('I') & 0x00FF);
            list.Add(accelone);

            TestLog.Current.LogStatus("Register Accelerators on the HwndHost");
            h.RegisterAccelerators(list);
            h.Accelerator += new AcceleratorEventHandler(acceleratorHandler);
            h.Mnemonic += new MnemonicEventHandler(mNemonicsHandlerNeg);

            b = new Button();
            b.Content = "Button Two";
            sp.Children.Add(b);

            // Start Injecting Input
            testElement t = new testElement();
            t.Rendered += new EventHandler(injectControl_A);
            sp.Children.Add(t);

            _window.Show();

            _window.Activate();
            
            return null;
        }

        void mNemonicsHandlerNeg(object o, MnemonicEventArgs args)
        {
            throw new Microsoft.Test.TestValidationException("Mnemonics should not be called");
        }


        void acceleratorHandler(object o, AcceleratorEventArgs args)
        {
            TestLog.Current.LogStatus("Accelerator: " +  args.TextKey.ToString());

            _exitCount++;

            HwndHost hwndHost = (HwndHost)o;
            
            if (args.TextKey == Key.M)
            {
                _count |= 0x00F;
                
                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _count |= 0x10000;
                }
            }

            if (args.TextKey == Key.L)
            {
                _count |= 0x0F0;
                if (InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _count |= 0x10000;
                }
            }
        
            if (args.TextKey == Key.N)
            {
                _count |= 0xF00;
                if (!InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _count |= 0x20000;
                }
            }


            if (args.TextKey == Key.I)
            {
                _count |= 0xF000;
                if (!InputHelper.HasFocusWithin((IKeyboardInputSink)hwndHost))
                {
                    _count |= 0x20000;
                }
            }

            if (_exitCount == 2)
                MainDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, new DispatcherOperationCallback (exit), null);
        }


        void injectControl_A(object o, EventArgs a)
        {
            _startbtn.Focus();

            TestLog.Current.LogStatus("Start Simulating Input");

            KeyboardHelper.PressKey(Key.LeftCtrl);
            KeyboardHelper.TypeKey(Key.M);
            KeyboardHelper.ReleaseKey(Key.LeftCtrl);    
            
            KeyboardHelper.TypeKey(Key.Tab);

            KeyboardHelper.PressKey(Key.LeftCtrl);
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.N);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);                        
            KeyboardHelper.ReleaseKey(Key.LeftCtrl);   

            KeyboardHelper.TypeKey(Key.Tab);

            KeyboardHelper.PressKey(Key.LeftShift);
            KeyboardHelper.TypeKey(Key.L);
            KeyboardHelper.ReleaseKey(Key.LeftShift);    

            KeyboardHelper.PressKey(Key.LeftShift);
            KeyboardHelper.TypeKey(Key.Tab);
            KeyboardHelper.ReleaseKey(Key.LeftShift);    

            KeyboardHelper.PressKey(Key.LeftCtrl);
            KeyboardHelper.PressKey(Key.LeftAlt);
            KeyboardHelper.TypeKey(Key.I);
            KeyboardHelper.ReleaseKey(Key.LeftAlt);                        
            KeyboardHelper.ReleaseKey(Key.LeftCtrl);   

            TestLog.Current.LogStatus("End Simulating Input");
        }
    }
}


