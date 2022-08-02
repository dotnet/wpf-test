// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Threading;
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
    public class WindowResizeGripFlowDirectionApp : Application
    {        
        [STAThread]
        public static int Main(String[] args)
        {
            WindowResizeGripFlowDirectionApp.DisableProcessWindowsGhosting();
            WindowResizeGripFlowDirectionApp theApp = new WindowResizeGripFlowDirectionApp();
            try
            {
                WindowResizeGripFlowDirectionApp.BlockInput(true);
                theApp.Run();
            }
            catch(ApplicationException e)
            {
                theApp._logger.Log(e);
                return 1;
            }
            finally
            {
                WindowResizeGripFlowDirectionApp.BlockInput(false);
            }
            
            theApp._logger.Log("Passed");
            return 0;
        }

        private Window      _win;
        private Button      _b;
        private double      _initHeight = 400;
        private double      _initWidth = 400;
        private int         _delta = 100, _rtldelta = -50;
        private bool        _hold = false;
        private bool        _verbose = false;
        private Logger      _logger = new Logger("DrtWindowResizeGripFlowDirection", "Microsoft", "Testing Window ResizeGrip, FlowDireciton APIs");

        // Test Case:
        // 1) Verify that resize grip is working over the resizeGripControl
        // 2) FlowDirection
        protected override void OnStartup(StartupEventArgs e) 
        {
            ParseCmdLine(e.Args);
            _win = new Window();
            _b = new Button();
            _b.Content = "Hello World";
            _win.Content = _b;

            _win.Title = "DrtWindowResizeGrip";
            _win.ResizeMode = ResizeMode.CanResizeWithGrip;
            _win.Width = _initWidth;            
            _win.Height = _initHeight;            
            _win.Show();
            _win.Topmost = true;
            _initHeight = _win.Height;
            _initWidth = _win.Width;

            if(_hold == false)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(RunTest),
                    this);
            }
            base.OnStartup(e);
        }

        private object RunTest(object obj)
        {
            VerboseOutput("RunTest begin");
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ResizeUsingResizeGrip),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifySizeAfterResize),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(TestFlowDirection),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ResizeUsingResizeGrip),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifySizeAfterResize),
                this);

           Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ShutdownApp),
                this);
           return null;
        }
        
        private object ResizeUsingResizeGrip(object obj)
        {  
            if (_win.FlowDirection == FlowDirection.RightToLeft)
            {
                MoveToClickAndDrag(new Point(_win.Left + 10, _win.Top + _initHeight - _delta - 10), _rtldelta);
            }
            else 
            {
                MoveToClickAndDrag(new Point(_win.Left + _initWidth - 10, _win.Top +_initHeight - 10), _delta);
            } 
            return null;
        }

        private void MoveToClickAndDrag( Point pt, int delta )
        {
            IntPtr hwnd = GetForegroundWindow();

            WindowInteropHelper wih = new WindowInteropHelper(_win);
            if (hwnd != wih.Handle)
            {
                string error = String.Format("Error: Foreground window {0:X} ({1}) did not match DRT window {2:X}", hwnd, GetWindowTitle(hwnd), wih.Handle);
                throw new ApplicationException(error);
            }

            HwndSource hwndSource = PresentationSource.FromVisual(_b) as HwndSource;
            Debug.Assert(hwndSource != null, "PresentaionSource has to be HwndSource here");

            Point ptDeviceUnits = hwndSource.CompositionTarget.TransformToDevice.Transform(pt);
            double deltaDeviceUnits = (hwndSource.CompositionTarget.TransformToDevice.Transform(new Point(delta, 0))).X;

            Input.SendMouseInput( ptDeviceUnits.X, ptDeviceUnits.Y, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );

            // send SendMouseInput works in term of the physical mouse buttons, therefore we need
            // to check to see if the mouse buttons are swapped because this method need to use the primary
            // mouse button.
            if (SystemParameters.SwapButtons != true)
            {
                // the mouse buttons are not swaped the primary is the left
                Input.SendMouseInput( ptDeviceUnits.X, ptDeviceUnits.Y, 0, SendMouseInputFlags.LeftDown | SendMouseInputFlags.Absolute );
                Input.SendMouseInput( ptDeviceUnits.X-deltaDeviceUnits, ptDeviceUnits.Y-deltaDeviceUnits, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );
                Input.SendMouseInput( ptDeviceUnits.X, ptDeviceUnits.Y, 0, SendMouseInputFlags.LeftUp | SendMouseInputFlags.Absolute );
            }
            else
            {
                // the mouse buttons are swaped so click the right button which as actually the primary
                Input.SendMouseInput( ptDeviceUnits.X, ptDeviceUnits.Y, 0, SendMouseInputFlags.RightDown | SendMouseInputFlags.Absolute );
                Input.SendMouseInput( ptDeviceUnits.X-deltaDeviceUnits, ptDeviceUnits.Y-deltaDeviceUnits, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );                
                Input.SendMouseInput( ptDeviceUnits.X, ptDeviceUnits.Y, 0, SendMouseInputFlags.RightUp | SendMouseInputFlags.Absolute );
            }
        }

        private object VerifySizeAfterResize(object obj)
        {
            String stringMessage;
            if (_win.FlowDirection == FlowDirection.LeftToRight)
            {
                if (_win.ActualHeight != _initHeight - _delta)
                {
                    stringMessage = String.Format("Did not get correct Height after resizing through ResizeGrip. expected:{0} -- actual:{1}", _initHeight - _delta, _win.ActualHeight);
                    throw new ApplicationException(stringMessage);
                }

                if (_win.ActualWidth != _initWidth - _delta)
                {
                    stringMessage = String.Format("Did not get correct Width after resizing through ResizeGrip. expected:{0} -- actual:{1}", _initWidth - _delta, _win.ActualWidth);
                    throw new ApplicationException(stringMessage);
                }
            } 
            else 
            {
                if (_win.ActualHeight != _initHeight - _delta - _rtldelta)
                {
                    stringMessage = String.Format("Did not get correct Height after changing FlowDirection and resizing through ResizeGrip. expected:{0} -- actual:{1}", _initHeight - _delta - _rtldelta, _win.ActualHeight);
                    throw new ApplicationException(stringMessage);
                }

                if (_win.ActualWidth != _initWidth - _delta + _rtldelta)
                {
                    stringMessage = String.Format("Did not get correct Width after changing FlowDirection and resizing through ResizeGrip. expected:{0} -- actual:{1}", _initWidth - _delta + _rtldelta, _win.ActualWidth);
                    throw new ApplicationException(stringMessage);
                }
            }
            return null;
        }

        private object TestFlowDirection(object obj)
        {
            _win.FlowDirection = FlowDirection.RightToLeft;

            WindowInteropHelper wih = new WindowInteropHelper(_win);
            int styleEx = WindowResizeGripFlowDirectionApp .IntPtrToInt32(WindowResizeGripFlowDirectionApp .GetWindowLong(wih.Handle, WindowResizeGripFlowDirectionApp .GWL_EXSTYLE));

            if ((styleEx & WindowResizeGripFlowDirectionApp .WS_EX_LAYOUTRTL) == 0)
            {
                throw new ApplicationException("FlowDirection set to RightToLeft did not change WS_EX_LAYOUTRTL in styleEx");
            }

            _win.FlowDirection = FlowDirection.LeftToRight;
            styleEx = WindowResizeGripFlowDirectionApp .IntPtrToInt32(WindowResizeGripFlowDirectionApp .GetWindowLong(wih.Handle, WindowResizeGripFlowDirectionApp .GWL_EXSTYLE));

            if ( (styleEx & WindowResizeGripFlowDirectionApp .WS_EX_LAYOUTRTL) != 0)
            {
                throw new ApplicationException("FlowDirection set to LeftToRight did not change WS_EX_LAYOUTRTL in styleEx");
            }

            _win.FlowDirection = FlowDirection.RightToLeft;
            return null;
        }

        
        
        private object ShutdownApp(object obj)
        {
            VerboseOutput(String.Format("_win.Width={0}; _win.Height={1}", _win.ActualWidth, _win.ActualHeight));
            this.Shutdown();
            return null;
        }

        private void ParseCmdLine(String[] args)
        {
            foreach (string arg in args)
            {
                switch(arg.ToLower())
                {
                    case "/verbose":
                    case "-verbose":
                        _verbose = true;
                        break;
                    case "/hold":
                    case "-hold":
                        _hold = true;
                        break;
                    default:
                        break;
                }
            }            
        }

        private void VerboseOutput(String str)
        {
            if (_verbose == true)
            {
                _logger.Log(str);
            }
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

