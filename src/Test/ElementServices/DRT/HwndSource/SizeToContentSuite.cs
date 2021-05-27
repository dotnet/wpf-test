// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.InteropServices;   // NativeMethods
using System.Windows.Interop;

namespace DRT
{
    public class SizeToContentSuite : DrtTestSuite
    {
        #region DrtTestSuite

        public SizeToContentSuite() : base("Size to content suite")
        {
            Contact = "Microsoft";
        }

        public override DrtTest[] PrepareTests()
        {
            // return the list of tests to run against the tree
            return new DrtTest[] { new DrtTest(RunTest) };
        }

        #endregion // DrtTestSuite

        // Create HwndSource
        // Set SizeToContent to SizeToContent.Width, add RootVisual, and call SetWindowPos
        // Set SizeToContent to SizeToContent.Height, call SetWindowPos
        // Resize RootVisual, set SizeToContent to SizeToContent.WidthAndHeight, and call SetWindowPos
        private void RunTest()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
            
            Console.WriteLine("Testing HwndSource SizeToContent behavior");
            HwndSourceParameters param = new HwndSourceParameters("DrtHwndSource", 100, 100);
            param.SetPosition(0, 0);
            if (!DRT.KeepAlive)
            {
                _source = new HwndSource(param);

                _source.ContentRendered += new EventHandler(OnContentRendered);
                _source.AddHook(new HwndSourceHook(ApplicationFilterMessage));

                _dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(SetAutoWidth),
                    null);
            }
            else  // 'hold' was passed in on the command line - don't finish this suite
            {
                _source = new HwndSource(param);
                _source.AddHook(new HwndSourceHook(ApplicationFilterMessage));
            }
            
            DRT.Suspend();
        }

        private  void OnContentRendered(object sender, EventArgs e)
        {
            _success = true;
        }

        private  IntPtr ApplicationFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Quit the application if the source window is closed.
            if ((msg == WM_CLOSE))
            {
                _dispatcher.BeginInvoke(
                    DispatcherPriority.Background,
                    new DispatcherOperationCallback(Quit),
                    null);

                handled = true;
            }
            else if (msg == 0x0005) // WM_SIZE
            {
                _width = ((int)lParam) & 0xFFFF;
                _height = (((int)lParam) >> 16);
            }

            return IntPtr.Zero;
        }

        private object Quit(object arg)
        {
            if (!_success)
            {
                DRT.ReturnCode = 1;                
            }

            _source.Dispose();

            DRT.Resume();
            return null;
        }

        private  object SetAutoWidth(object arg)
        {
            _source.SizeToContent = SizeToContent.Width;
            _resizer = new Resizer();
            _source.RootVisual = _resizer;

            SetWindowPos(_source.Handle, IntPtr.Zero, 0, 0, 150, 150, SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED | SWP_DRAWFRAME | SWP_NOACTIVATE);
            _dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifyAndSetAutoHeight),
                null);

            return null;
        }

        private  object VerifyAndSetAutoHeight(object arg)
        {
            if (_width < 200)
            {
               Console.WriteLine("SizeToContent.Width failed");
               Console.WriteLine("Hwnd Width should be 200. Instead it is " + _width.ToString());
                _success = false;
            }

            _source.SizeToContent = SizeToContent.Height;
            SetWindowPos(_source.Handle, IntPtr.Zero, 0, 0, 150, 150, SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED | SWP_DRAWFRAME | SWP_NOACTIVATE);

            _dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(VerifyAndResize),
                null);

            return null;
        }

        private  object VerifyAndResize(object arg)
        {
            if (_height < 200)
            {
                Console.WriteLine("SizeToContent.Height failed");
                Console.WriteLine("Hwnd Height should be 200. Intead it is " + _height.ToString());
                _success = false;
            }

            _resizer.Large = true;
            _source.SizeToContent = SizeToContent.WidthAndHeight;
            SetWindowPos(_source.Handle, IntPtr.Zero, 0, 0, 150, 150, SWP_NOMOVE | SWP_NOZORDER | SWP_FRAMECHANGED | SWP_DRAWFRAME | SWP_NOACTIVATE);

            _dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(End),
                null);

            return null;
        }

        private  object End(object arg)
        {
            if ((_width < 400) || (_height < 400))
            {
                Console.WriteLine("SizeToContent.Full failed");
                Console.WriteLine("Hwnd Width and Height should be 400. Instead they are " + _width.ToString() + " " + _height.ToString());
                _success = false;
            }

            _dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new DispatcherOperationCallback(Quit),
                null);

            return null;
        }

        private  Dispatcher _dispatcher;
        private  HwndSource _source = null;
        private  Resizer _resizer = null;
        private  int WM_CLOSE = 0x10;
        private  bool _success = false;
        private  int _width = 0, _height = 0;

        private const int SWP_DRAWFRAME = 0x0020, SWP_NOSIZE = 0x0001, SWP_NOMOVE = 0x0002, SWP_NOZORDER = 0x0004, SWP_NOACTIVATE = 0x0010, SWP_FRAMECHANGED = 0x0020;

        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int flags);

        private class Resizer : FrameworkElement
        {
            public bool Large = false;

            protected override Size MeasureOverride(Size constraint)
            {
                if (Large)
                {
                    return new Size(400d, 400d);
                }

                return new Size(200d, 200d);
            }
        }
    }
}

