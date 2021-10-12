// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Threading;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Automation;
using System.Threading;

namespace DRT
{
    public class Harness
    {
        [STAThread]
        public static int Main()
        {
            WindowAutoSizeApp.DisableProcessWindowsGhosting();
            WindowAutoSizeApp theApp = new WindowAutoSizeApp();

            try
            {
                WindowAutoSizeApp.BlockInput(1);
                theApp.Run();
            }
            finally
            {
                WindowAutoSizeApp.BlockInput(0);
            }

            if (WindowAutoSizeApp.Failed)
            {
                theApp.Logger.Log("Test Failed. DRTWindowAutoSize owner: Microsoft.");                
            }
            else
            {
                theApp.Logger.Log("Passed");                
            }

            return WindowAutoSizeApp.Failed ? 1 : 0;
        }
    }

    public struct RECT
    {
        public uint left;
        public uint top;
        public uint right;
        public uint bottom;
    }

    public class WindowAutoSizeApp : Application
    {
        private static Logger _logger;

        private Window _win;

        private IntPtr _handle;
                
        private Canvas _root;

        private static double _width = 700;
        private static double _height = 700;

        private static double _width0 = 300;
        private static double _height0 = 200;

        private static double _width1 = 400;
        private static double _height1 = 300;

        private static double _width2 = 500;
        private static double _height2 = 400;

        private static double _width3 = 600;
        private static double _height3 = 500;

        private static int TestCode = 0;
        
        public WindowAutoSizeApp() : base()
        {
            _logger = new Logger("DrtWindowAutoSize", "Microsoft", "Testing Window SizeToContent");
        }

        internal Logger Logger
        {
            get
            {
                return _logger;
            }
        }

        
        // Test Cases:
        //  SizeToContent
        //  1. Setting SizeToContent to Width. Set window width and height. 
        //     Verify that width is sized to content. Window.Width reflect that. And setting window height was effective.
        //
        //  2. Setting SizeToContent to Height. Set window width and height.
        //     Verify that height is sized to content. Window.Height reflect that. And setting window width was effective.
        //
        //  3. Change content's size. Setting SizeToContent to WidhtAndHeight. Set window width and height.
        //     Verify that height and width are sized to content. Window.Width and Height reflect that.
        //
        //  4. Resize the window and verify that SizeToContent is turned off
        //
        //  5. Set SizeToContent to Manual and verify that changing size of the window works
        //
         
        protected override void OnStartup(StartupEventArgs e)
        {
            _win = new Window();
            _win.Top = 0;
            _win.Left = 0;
            _win.Title = "Window SizeToContent Test";

            // Testing SizeToContent.Width 
            _win.SizeToContent = SizeToContent.Width;

//#if NEVER
            // and setting Width should not be effective 
            _win.Width = _width;  // 700
//#endif        
            // but setting Height should work fine
            _win.Height = _height;  // 700

            _root = new Canvas ();
            // Set root's width and height to the first set of testing data
            _root.Width = _width0;  // 300
            _root.Height = _height0;    //200

            _root.Background = new SolidColorBrush(Colors.Blue);
            _win.Content = _root;                        
                        
            _win.Show();            
            _handle = (new WindowInteropHelper(_win)).Handle;
            
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifyWidthAndTestHeight),
                this);

            base.OnStartup(e);
        }

        private object VerifyWidthAndTestHeight(object obj)
        {
            // Verifying SizeToContent.Width test
            VerifyWindowSize(_width0, _height0, _width, _height);

            // Testing SizeToContent.Height
            // Set SizeToContent to Height
            _win.SizeToContent = SizeToContent.Height;

            // Setting Width should work fine
            _win.Width = _width1;   // 400            

//#if NEVER
            // And setting Height should not be effective
            _win.Height = _height1;   // 300
//#endif

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifyHeightAndTestWidthAndHeight),
                this);
            return null;
        }

        private object VerifyHeightAndTestWidthAndHeight(object obj)
        {
            // Verifying SizeToContent.Height behavior
            VerifyWindowSize(_width0, _height0, _width1, _height1);            

            // Testing SizeToContent.WidthAndHeight
            
            _root.Width = _width2; // 500
            _root.Height = _height2; // 400            

            // Set SizeToContent to WidthAndHeight
            _win.SizeToContent = SizeToContent.WidthAndHeight;

//#if NEVER
            // Setting Width and Height should not work
            _win.Width = _width3; //600
            _win.Height = _height3; //500
//#endif

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifyWidthAndHeightAndTestResize),
                this);
            return null;
        }

        private object VerifyWidthAndHeightAndTestResize(object obj)
        {
            VerifyWindowSize(_width2, _height2, _width3, _height3);
            ResizeWindow();            
            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifyResizeAndTestManual),
                this);
            return null;
        }

        private object VerifyResizeAndTestManual(object obj)
        {
            if (_win.SizeToContent != SizeToContent.Manual)
            {
                SetError("User resize should turn off SizeToContent");
            }

            // Testing SizeToContent.Manual behavior
            _win.Width = _width3;   // 600
            _win.Height = _height3; // 500

            Dispatcher.CurrentDispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifyManualAndTestWidthUsingNaN),
                this);
            return null;
        }

        private object VerifyManualAndTestWidthUsingNaN(object obj)
        {
            // Verifying SizeToContent.Manual behavior
            VerifyWindowSize(_width2, _height2, _width3, _height3);
            _win.Close();
            return null;
        }

        private void ResizeWindow()
        {
            RECT rect = new RECT();

            IntPtr hwnd = GetForegroundWindow();

            if (hwnd != _handle)
            {
                string error = String.Format("Error: Foreground window (hwnd={0:X}; title={1}) did not match DRT window {2:X}", hwnd, GetWindowTitle(hwnd), _handle);
                SetError(error);
                return;
            }

            if (IsHungAppWindow(_handle))
            {
                string error = String.Format("Main window hung and has been ghosted. This is a bad time to be sending input!");
                SetError(error);
                return;
            }

            GetWindowRect(_handle, ref rect);

            int visualWindowWidth = (int)(rect.right - rect.left);
            int visualWindowHeight = (int)(rect.bottom - rect.top);
            Point startpt, endpt;

            startpt = new Point(_win.Left + visualWindowWidth - 2, _win.Top + visualWindowHeight - 2);
            endpt = new Point(startpt.X + 200, startpt.Y + 200);
            
            Input.SendMouseInput(startpt.X, startpt.Y, 0, SendMouseInputFlags.Absolute | SendMouseInputFlags.Move);
            Input.SendMouseInput(startpt.X, startpt.Y, 0, SendMouseInputFlags.Absolute | (SystemParameters.SwapButtons ? SendMouseInputFlags.RightDown : SendMouseInputFlags.LeftDown));
            Input.SendMouseInput(endpt.X, endpt.Y, 0, SendMouseInputFlags.Absolute | SendMouseInputFlags.Move);
//            Thread.Sleep(1000);
            Input.SendMouseInput(endpt.X, endpt.Y, 0, SendMouseInputFlags.Absolute | (SystemParameters.SwapButtons ? SendMouseInputFlags.RightUp : SendMouseInputFlags.LeftUp));
        }

        private void VerifyWindowSize(double contentWidth, double contentHeight, double windowWidth, double windowHeight)
        {
            HwndSource hwndSource = PresentationSource.FromVisual(_root) as HwndSource;
            Debug.Assert(hwndSource != null, "PresentaionSource has to be HwndSource here");

            RECT rect = new RECT();
            GetClientRect(_handle, ref rect);

            Point visualClientSizeLogicalUnits = hwndSource.CompositionTarget.TransformFromDevice.Transform(
                new Point(rect.right - rect.left, rect.bottom - rect.top));
         
            GetWindowRect(_handle, ref rect);

            Point visualWindowSizeLogicalUnits = hwndSource.CompositionTarget.TransformFromDevice.Transform(
                new Point(rect.right - rect.left, rect.bottom - rect.top));

            switch (_win.SizeToContent)
            {
                // contentWidth is ignored
                // contentHeight is ignored
                // window.Width should reflect Visual Width
                // window.Height should refect visual height                
                case SizeToContent.Manual:
                    if ((_win.ActualWidth != visualWindowSizeLogicalUnits.X) || (_win.ActualWidth != windowWidth))
                    {
                        SetError("Window.Width was not effective after setting SizeToContent to None");
                    }
                    if ((_win.ActualHeight != visualWindowSizeLogicalUnits.Y) || (_win.ActualHeight != windowHeight))
                    {
                        SetError("Window.Height was not effective after setting SizeToContent to None");
                    }

                    break;
                // contentWidth should be respected. 
                // contentHeight is ignored
                // window.Width should reflect contentWidth
                // window.Height should be respected.
                case SizeToContent.Width:                   
                    if (Math.Round(visualClientSizeLogicalUnits.X) != contentWidth)
                    {
                        SetError(String.Format("Setting SizeToContent to Width was not effective when setting SizeToContent to Width -- visualClientSizeLogcialUnits.X = {0}, contentWidth = {1}", visualClientSizeLogicalUnits.X, contentWidth));
                    }
                   
                    if (Math.Round(_win.ActualWidth) != Math.Round(visualWindowSizeLogicalUnits.X))
                    {
                        SetError(String.Format("Window.Width didn't reflect content's size when setting SizeToContent to Width -- _win.ActualWidth = {0}, visualWindowSizeLogicalUnits.X = {1}", _win.ActualWidth, visualWindowSizeLogicalUnits.X));
                    }

                    if ((Math.Round(_win.ActualHeight) != windowHeight) || (Math.Round(visualWindowSizeLogicalUnits.Y) != windowHeight))
                    {
                        SetError(String.Format("Setting Window.Height was not effective when SizeToContent is Width -- _win.ActualHeight = {0}, windowHeight = {1}, visualWindowSizeLogicalUnits.Y = {2}", _win.ActualHeight, windowHeight, visualWindowSizeLogicalUnits.Y));
                    }

                    break;
                // contentWidth is ignored
                // contentHeight should be respected. 
                // window.Width should be respected
                // window.Height should reflect contentHeight                
                case SizeToContent.Height:
                    if (visualClientSizeLogicalUnits.Y != contentHeight)
                    {
                        SetError("Setting SizeToContent to Height was not effective when setting SizeToContent to Height");
                    }

                    if (_win.ActualHeight != visualWindowSizeLogicalUnits.Y)
                    {
                        SetError("Window.Height didn't reflect content's size when setting SizeToContent to Height");
                    }

                    if ((_win.ActualWidth != windowWidth) || (visualWindowSizeLogicalUnits.X != windowWidth))
                    {
                        SetError("Setting Window.Width was not effective when SizeToContent is Height");
                    }

                    break;
                // contentWidth and contentHeight should both be respected
                // window.Width and window.Height should reflect content size
                case SizeToContent.WidthAndHeight:
                    if (Math.Round(visualClientSizeLogicalUnits.X) != contentWidth)
                    {
                        SetError(String.Format("Window client width not equal to contentWidth when setting SizeToContent to WidthAndHeight -- visualClientSizeLogicalUnits.X = {0}, contentWidth = {1}", visualClientSizeLogicalUnits.X, contentWidth));
                    }

                    if (Math.Round(visualClientSizeLogicalUnits.Y) != contentHeight)
                    {
                        SetError(String.Format("Window client height not equal to contentHeight when Setting SizeToContent to WidthAndHeight -- visualClientSizeLogicalUnits.Y = {0}, contentHeight = {1}", visualClientSizeLogicalUnits.Y, contentHeight));
                    }

                    if (Math.Round(_win.ActualWidth) != Math.Round(visualWindowSizeLogicalUnits.X))
                    {
                        SetError(String.Format("Window.Width didn't reflect content's size when setting SizeToContent to WidthAndHeight -- _win.ActualWidth = {0}, visualWindowSizeLogicalUnits.X = {1}", _win.ActualWidth, visualWindowSizeLogicalUnits.X));
                    }

                    if (Math.Round(_win.ActualHeight) != Math.Round(visualWindowSizeLogicalUnits.Y))
                    {
                        SetError(String.Format("Window.Height didn't reflect content's size when setting SizeToContent to WidthAndHeight -- _win.ActualHeight = {0}, visualWindowSizeLogicalUnits.Y = {1}", _win.ActualHeight, visualWindowSizeLogicalUnits.Y));
                    }

                    break;
            }
        }        
 
        public static void SetError(string msg)
        {
            _logger.Log("FAILURE: " + msg);
            SetError();
        }

        public static void SetError()
        {
            TestCode = 1;
        }

        public static bool Failed
        {
            get { return TestCode != 0; }
        }        

        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hwnd, ref RECT _rect);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, ref RECT _rect);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowText(HandleRef hWnd, [Out] StringBuilder lpString, int nMaxCount);

        private static string GetWindowTitle(IntPtr hwnd)
        {
            StringBuilder sb = new StringBuilder(500);

            GetWindowText(new HandleRef(null, hwnd), sb, 500);
            return sb.ToString();
        }

        [DllImport("user32.dll")]
        private static extern bool IsHungAppWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int BlockInput(int fBlockIt);

        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        internal extern static void DisableProcessWindowsGhosting();
    }
}
