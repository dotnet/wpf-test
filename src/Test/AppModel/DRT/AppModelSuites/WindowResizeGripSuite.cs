// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.ComponentModel;
using System.Text;
using System.Security;

using System.Windows.Input;
using System.Windows.Automation;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace DRT
{
//    public class DrtTestSuiteWindow : DrtTestSuite
//    {
//        protected DrtTestSuiteWindow(string name) : base(name)
//        {
//            using (DrtBase.Context.Access())
//            {
//                Window window = new Window();
//
//                window.Visibility = Visibility.Visible;
//                window.Text = "Window";
//
//                PropertyInfo info = window.GetType().GetProperty("HwndSourceWindow", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
//                object obj = info.GetValue(window, null);
//
//                DRT.MainWindow = (HwndSource)obj;
//            }
//        }
//    }

    public class WindowResizeGripSuite : DrtTestSuite
    {
        private Window      _win;
        private Length      _initHeight = new Length(400);
        private Length      _initWidth = new Length(400);
        private int         _delta = 200;
        private bool        _hold = false;
        private bool        _verbose = false;
        private bool        _inputSent = false;

        public WindowResizeGripSuite() : base("WindowResizeGrip")
        {
            TeamContact = "WPF";
            Contact     = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]
                    {
                        new DrtTest(ResizeUsingResizeGrip),
                        new DrtTest(VerifySizeAfterResize),
                        new DrtTest(TestFlowDirection),
                    };
        }

        // Test Case:
        // 1) Verify that resize grip is working over the resizeGripControl
        // 2) FlowDirection

        private void ResizeUsingResizeGrip()
        {
            //_win = new Window();
            ((DrtAppModelSuites)DRT).UseNewWindow();

            _win = (Window) DRT.MainWindow.RootVisual;
            Button b = new Button();
            b.Content = "Hello World";
            _win.Content = b;

            _win.Text = "DrtWindowResizeGrip";
            _win.ResizeMode = ResizeMode.CanResizeWithGrip;
            _win.Width = _initWidth;
            _win.Height = _initHeight;
            _win.Show();
            _win.TopMost = true;
            _initHeight = new Length(_win.ActualHeight);
            _initWidth = new Length(_win.ActualWidth);

            MoveToClickAndDrag(new Point(_win.Left.Value + _initWidth.Value - 10, _win.Top.Value + _initHeight.Value - 10), _delta);
        }

        private void MoveToClickAndDrag( Point pt, int delta )
        {
            IntPtr hwnd = GetForegroundWindow();

            WindowInteropHelper wih = new WindowInteropHelper(_win);
            if (hwnd != wih.Handle)
            {
                string error = String.Format("Warning: Foreground window {0:X} ({1}) did not match DRT window {2:X}", hwnd, GetWindowTitle(hwnd), wih.Handle);

                Console.WriteLine(error);
                return;
            }

            Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );

            // send SendMouseInput works in term of the physical mouse buttons, therefore we need
            // to check to see if the mouse buttons are swapped because this method need to use the primary
            // mouse button.
            if (SystemMetrics.SwapButtons != true)
            {
                // the mouse buttons are not swaped the primary is the left
                Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.LeftDown | SendMouseInputFlags.Absolute );
                Input.SendMouseInput( pt.X-delta, pt.Y-delta, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );
                Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.LeftUp | SendMouseInputFlags.Absolute );
            }
            else
            {
                // the mouse buttons are swaped so click the right button which as actually the primary
                Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.RightDown | SendMouseInputFlags.Absolute );
                Input.SendMouseInput( pt.X-delta, pt.Y-delta, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );                
                Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.RightUp | SendMouseInputFlags.Absolute );
            }

            _inputSent = true;
        }

        private void VerifySizeAfterResize()
        {
            if (_inputSent == false)
            {
                Console.WriteLine("The UiContextItem to send mouse input to the window has not yet happened or has failed. So this test cannot pass. Skipping verifying");
                return;
            }

            String stringMessage;

            if (_win.ActualHeight != _initHeight.Value - _delta)
            {
                stringMessage = String.Format("Did not get correct Height after resizing through ResizeGrip. expected:{0} -- actual:{1}", _initHeight.Value - _delta, _win.ActualHeight);
                throw new ApplicationException(stringMessage);
            }

            if (_win.ActualWidth != _initWidth.Value - _delta)
            {
                stringMessage = String.Format("Did not get correct Width after resizing through ResizeGrip. expected:{0} -- actual:{1}", _initWidth.Value - _delta, _win.ActualWidth);
                throw new ApplicationException(stringMessage);
            }          
        }

        private void TestFlowDirection()
        {
            _win.FlowDirection = FlowDirection.RightToLeft;

            WindowInteropHelper wih = new WindowInteropHelper(_win);
            int styleEx = WindowResizeGripSuite .IntPtrToInt32(WindowResizeGripSuite .GetWindowLong(wih.Handle, WindowResizeGripSuite .GWL_EXSTYLE));

            if ( (styleEx & WindowResizeGripSuite .WS_EX_LAYOUTRTL) == 0)
            {
                throw new ApplicationException("FlowDirection set to RightToLeft did not change WS_EX_LAYOUTRTL in styleEx");
            }

            _win.FlowDirection = FlowDirection.LeftToRight;
            styleEx = WindowResizeGripSuite .IntPtrToInt32(WindowResizeGripSuite .GetWindowLong(wih.Handle, WindowResizeGripSuite .GWL_EXSTYLE));

            if ( (styleEx & WindowResizeGripSuite .WS_EX_LAYOUTRTL) != 0)
            {
                throw new ApplicationException("FlowDirection set to LeftToRight did not change WS_EX_LAYOUTRTL in styleEx");
            }

            _win.FlowDirection = FlowDirection.RightToLeft;
        }


        private object ShutdownApp(object obj)
        {
            //VerboseOutput(String.Format("_win.Width={0}; _win.Height={1}", _win.ActualWidth, _win.ActualHeight));
            //this.Shutdown();
            return null;
        }

        private static int IntPtrToInt32(IntPtr intPtr)
        {
            return unchecked((int)intPtr.ToInt64());
        }

        [DllImport("user32.dll",
        #if WIN64
        EntryPoint="GetWindowLongPtr",
        #endif
        CharSet=CharSet.Auto)
        ]
        private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int BlockInput(bool fBlockIt);

        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal extern static void DisableProcessWindowsGhosting();

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount);

        private static string GetWindowTitle(IntPtr hwnd)
        {
            StringBuilder sb = new StringBuilder(500);

            GetWindowText(new HandleRef(null, hwnd), sb, 500);
            return sb.ToString();
        }

        private const int GWL_EXSTYLE           = (-20);
        private const int WS_EX_LAYOUTRTL       = 0x00400000;
    }
}

