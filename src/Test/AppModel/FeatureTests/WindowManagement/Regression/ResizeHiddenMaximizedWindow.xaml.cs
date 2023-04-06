// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;
using Microsoft.Test.Win32;

namespace WindowTest
{
    //  Regression test
    //  Create a new window, maximize it, show it, hide it, and resize it.  Expect it shouldn't get re-shown.
    public partial class ResizeHiddenMaximizedWindow
    {
        DispatcherTimer _timer = new DispatcherTimer();
        Window _window;

        void OnContentRendered(object sender, EventArgs e)
        {
            _timer.Interval = new TimeSpan(0,0,2);
            _timer.Tick += new EventHandler(timer_Tick);
            _timer.Start();

            Logger.Status("Creating new window");

            _window = new Window();
            _window.Title = "Window2";
            _window.WindowState = WindowState.Maximized;
            _window.Show();
            _window.Hide();
            _window.Height = 400;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            _timer.Stop();

            IntPtr hwnd = new WindowInteropHelper(_window).Handle;
            //GWL_STYLE always returns a 32-bit value, even on 64-bit, so no danger of conversion failure here.
            int style = NativeMethods.GetWindowLong(hwnd, NativeConstants.GWL_STYLE).ToInt32();
            bool isVisible = ((style & NativeConstants.WS_VISIBLE) == NativeConstants.WS_VISIBLE);

            if (isVisible)
            {
                Logger.LogFail("Window should not have been visible, but is.");
            }
            else
            {
                Logger.LogPass("As expected, window is not visible after resize.");
            }

            _window.Close();
            this.Close();
        }
    }
}
