// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using System.Collections;
using System.Text;


using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;


namespace DRT
{
    public class WindowStartupLocationApp : Application
    {
        [STAThread]
        public static int Main()
        {
            TestData.DisableProcessWindowsGhosting();
            WindowStartupLocationApp theApp = new WindowStartupLocationApp();
            int retVal = theApp.Run();

            theApp._logger.Log(retVal == 0? "Passed" : "Failed");
            return retVal;
        }

        // Tests WindowStartupLocation
        // Test 1
        // Width = Height = Top = Left = none, WindowStartupLocation = CenterScreen
        //
        // Test 2
        // Width = 400; Height = Top = Left = none; WindowStartupLocation = CenterScreen
        //
        // Test 3
        // Width = none; Height = 400; Top = Left = none; WindowStartupLocation = CenterScreen
        //
        // Test 4
        // Width = 400; Height = 400; Top = Left = none; WindowStartupLocation = CenterScreen
        //
        // Test 5
        // Width = Height = Top = Left = none, WindowStartupLocation = CenterOwner
        //          
        // Test 6
        // Width = none; Height = 400; Top = Left = none; WindowStartupLocation = CenterOwner
        //
        // Test 7
        // Width = 400; Height = none; Top = Left = none; WindowStartupLocation = CenterOwner
        //
        // Test 8
        // Width = 400; Height = 400; Top = Left = none; WindowStartupLocation = CenterOwner
        //

        private ArrayList           _array = new ArrayList();
        private bool                _result = true;
        public  Logger              _logger = new Logger("DrtWindowAutoLocation", "Microsoft", "Testing WindowStartupLocation API");

        protected override void OnStartup(StartupEventArgs e) 
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(InitTests),
                this);

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifyTests),
                this);
            
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(ShutdownApp),
                this);

            base.OnStartup(e);
        }

        private object ShutdownApp(object obj)
        {
            this.Shutdown(_result == true? 0 : 1);
            return null;
        }
        
        private object InitTests(object obj)
        {
            TestData td;
            double defaultVal = Double.NaN;

            // Test 1
            // Width = Height = Top = Left = none, WindowStartupLocation = CenterScreen
            //
            td = new TestData(defaultVal, defaultVal, defaultVal, defaultVal, WindowStartupLocation.CenterScreen);
            _array.Add(td);

            // Test 2
            // Width = 400; Height = Top = Left = none; WindowStartupLocation = CenterScreen
            //
            td = new TestData(400, defaultVal, defaultVal, defaultVal, WindowStartupLocation.CenterScreen);
            _array.Add(td);

            // Test 3
            // Width = none; Height = 400; Top = Left = none; WindowStartupLocation = CenterScreen
            //
            td = new TestData(defaultVal, 400, defaultVal, defaultVal, WindowStartupLocation.CenterScreen);
            _array.Add(td);

            // Test 4
            // Width = 400; Height = 400; Top = Left = none; WindowStartupLocation = CenterScreen
            //
            td = new TestData(400, 400, defaultVal, defaultVal, WindowStartupLocation.CenterScreen);
            _array.Add(td);

            // Test 5
            // Width = Height = Top = Left = none, WindowStartupLocation = CenterOwner
            //
            td = new TestData(defaultVal, defaultVal, defaultVal, defaultVal, WindowStartupLocation.CenterOwner);
            _array.Add(td);
            
            // Test 6
            // Width = none; Height = 400; Top = Left = none; WindowStartupLocation = CenterOwner
            //
            td = new TestData(defaultVal, 400, defaultVal, defaultVal, WindowStartupLocation.CenterOwner);
            _array.Add(td);

            // Test 7
            // Width = 400; Height = none; Top = Left = none; WindowStartupLocation = CenterOwner
            //
            td = new TestData(400, defaultVal, defaultVal, defaultVal, WindowStartupLocation.CenterOwner);
            _array.Add(td);

            // Test 8
            // Width = 400; Height = 400; Top = Left = none; WindowStartupLocation = CenterOwner
            //
            td = new TestData(400, 400, defaultVal, defaultVal, WindowStartupLocation.CenterOwner);
            _array.Add(td);

            
            return null;
        }

        private object VerifyTests(object obj)
        {
            foreach (TestData td in _array)
            {
                _result = (td.Verify() == false? false: _result);
            }
            
            return null;
        }
    }

    class TestData
    {
        double                  _width      = Double.NaN;
        double                  _height     = Double.NaN;
        double                  _top        = Double.NaN;
        double                  _left       = Double.NaN;
        WindowStartupLocation   _wsl        = WindowStartupLocation.Manual;
        Window                  _win;
        Window                  _ownerWin;
        IntPtr                  _winHandle  = IntPtr.Zero;

        public TestData(double width, double height, double left, double top, WindowStartupLocation wsl)
        {
            _width = width;
            _height = height;
            _left = left;
            _top = top;
            _wsl = wsl;
            
            _win = new Window();
            _win.Width = _width;
            _win.Height = _height;
            _win.Top = _top;
            _win.Left = _left;
            _win.WindowStartupLocation = _wsl;
            _win.Title = String.Format("Width={0}, Height={1}, Left={2}, Top={3}, WindowStartupLocation={4}", _width, _height, _left, _top, _wsl);

            if (_wsl == WindowStartupLocation.CenterOwner)
            {
                RECT workArea = new RECT();
                SystemParametersInfo(SPI_GETWORKAREA, 0, ref workArea, 0);
                int workAreaWidth = workArea.right - workArea.left;
                int workAreaHeight = workArea.bottom - workArea.top;
                
                _ownerWin = new Window();
                _ownerWin.Width = workAreaWidth/2;
                _ownerWin.Height = workAreaHeight/2;
                _ownerWin.Left = workArea.left + workAreaWidth/4;
                _ownerWin.Top = workArea.top + workAreaHeight/4;
                _ownerWin.Show();
                
                _win.Owner = _ownerWin;
            }

            _win.Show();

            WindowInteropHelper wih = new WindowInteropHelper(_win);
            _winHandle = wih.Handle;
        }

        public bool Verify()
        {
            int left, top, width, height;
            double ownerLeft = 0 , ownerTop = 0, ownerWidth = 0, ownerHeight = 0;

            RECT rc = new RECT();
            GetWindowRect(_winHandle, ref rc);
            left = rc.left;
            top = rc.top;

            if ((left != _win.Left) || (top != _win.Top))
            {
                App._logger.Log("********************************");
                App._logger.Log("Failed --- GetValue Top&Left");
                return false;
            }            

            width = rc.right - rc.left;
            height = rc.bottom - rc.top;

            if ((width != _win.ActualWidth) || (height != _win.ActualHeight))
            {
                App._logger.Log("********************************");
                App._logger.Log("Failed --- GetValue ActualWidth/ActualHeight");
                return false;
            }

            if (_ownerWin != null)
            {
                WindowInteropHelper wih = new WindowInteropHelper(_ownerWin);
                GetWindowRect(wih.Handle, ref rc);
                ownerLeft = rc.left;
                ownerTop = rc.top;
                ownerWidth = rc.right - rc.left;
                ownerHeight = rc.bottom - rc.top;
            }

            switch (_wsl)
            {
                case WindowStartupLocation.Manual:
                {
                    if (    (   Double.IsNaN(_width) == false && (int)_width != (int)width      ) 
                        ||  (   Double.IsNaN(_height) == false && (int)_height != (int)height   )
                        ||  (   Double.IsNaN(_left) == false && (int)_left != (int)left         )
                        ||  (   Double.IsNaN(_top) == false && (int)_top != (int)top            )
                       )
                    {
                        App._logger.Log("********************************");
                        App._logger.Log("Failed --- WindowStartupLocation.Manual");
                        App._logger.Log(String.Format("SetValues : Width={0}, Height={1}, Left={2}, Top={3}", _width, _height, _left, _top));
                        App._logger.Log(String.Format("GetValues : Width={0}, Height={1}, Left={2}, Top={3}", width, height, left, top));
                        return false;
                    }
                    break;
                }
                case WindowStartupLocation.CenterOwner:
                {
                    if ((AreClose((int)ownerLeft, (int)(left - (ownerWidth - width)/2)) == false) ||
                        (AreClose((int)ownerTop, (int)(top - (ownerHeight - height)/2)) == false))
                    {
                        App._logger.Log("********************************");
                        App._logger.Log("Failed --- WindowStartupLocation.CenterOwner");
                        App._logger.Log(String.Format("Owner co-ods : Width={0}, Height={1}, Left={2}, Top={3}", ownerWidth, ownerHeight, ownerLeft, ownerTop));
                        App._logger.Log(String.Format("SetValues : Width={0}, Height={1}, Left={2}, Top={3}", _width, _height, _left, _top));
                        App._logger.Log(String.Format("GetValues : Width={0}, Height={1}, Left={2}, Top={3}", width, height, left, top));
                        return false;
                    }
                    break;
                }
                case WindowStartupLocation.CenterScreen:
                {
                    IntPtr hMonitor = MonitorFromWindow(_winHandle, MONITOR_DEFAULTTONEAREST);

                    if (hMonitor != IntPtr.Zero)
                    {
                        MONITORINFOEX monitorInfo = new MONITORINFOEX();
                        monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));

                        if (GetMonitorInfo(hMonitor, monitorInfo))
                        {
                            RECT workAreaRect = monitorInfo.rcWork;
                            double workAreaWidth = workAreaRect.right - workAreaRect.left;
                            double workAreaHeight = workAreaRect.bottom - workAreaRect.top;

                            if ((AreClose(left, (int)(workAreaRect.left + (workAreaWidth - width)/2)) == false) ||
                                (AreClose(top, (int)(workAreaRect.top + (workAreaHeight - height)/2)) == false))
                            {
                                App._logger.Log("********************************");
                                App._logger.Log("Failed --- WindowStartupLocation.CenterScreen");
                                App._logger.Log(String.Format("Monitor (work area) co-ods : Width={0}, Height={1}, Left={2}, Top={3}", workAreaWidth, workAreaHeight, workAreaRect.left, workAreaRect.top));
                                App._logger.Log(String.Format("SetValues : Width={0}, Height={1}, Left={2}, Top={3}", _width, _height, _left, _top));
                                App._logger.Log(String.Format("GetValues : Width={0}, Height={1}, Left={2}, Top={3}", width, height, left, top));
                                
                                return false;
                            }
                        }
                    }                    
                    break;
                }
                default:
                    break;
            }
            return true;
        }

        private bool AreClose(int val1, int val2)
        {
            if (Math.Abs(val1 - val2) <= 1)
            {
                return true;
            }
            return false;
        }

        private WindowStartupLocationApp App
        {
            get
            {
                return (WindowStartupLocationApp)Application.Current;
            }
        }

        private const int SPI_GETWORKAREA           = 0x0030;
        private const int MONITOR_DEFAULTTONEAREST  = 0x00000002;
        
        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        private static extern bool GetWindowRect(IntPtr hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        private static extern bool SystemParametersInfo(int nAction, int nParam, ref RECT rc, int nUpdate);
        
        [DllImport("user32.dll", ExactSpelling=true)]
        private static extern IntPtr MonitorFromWindow(IntPtr handle, int flags);

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hmonitor, [In, Out]MONITORINFOEX info);

        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal extern static void DisableProcessWindowsGhosting();

        [StructLayout(LayoutKind.Sequential,CharSet=CharSet.Auto)]
        private class MONITORINFOEX {
            internal int     cbSize = Marshal.SizeOf(typeof(MONITORINFOEX));
            internal RECT    rcMonitor = new RECT();
            internal RECT    rcWork = new RECT();
            internal int     dwFlags = 0;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
            internal char[]  szDevice = new char[32];
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom) {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public static RECT FromXYWH(int x, int y, int width, int height) {
                return new RECT(x, y, x + width, y + height);
            }
        }        

    }
}

