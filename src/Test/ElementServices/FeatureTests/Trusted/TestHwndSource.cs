// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Test.Win32;
using Microsoft.Test.Threading;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// A lite test source.
    /// </summary>
    public class TestHwndSource
    {
        /// <summary>
        /// Hide objects from being inserted into collections.
        /// </summary>
        private TestHwndSource()
        {
        }

        /// <summary>
        /// Create an HwndSource based on a standard Win32 window.
        /// </summary>
        /// <param name="x">x position of window (screen coordinates).</param>
        /// <param name="y">y position of window (screen coordinates).</param>
        /// <param name="w">Width of window (pixels).</param>
        /// <param name="h">Height of window (pixels).</param>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]        
        public TestHwndSource(int x, int y, int w, int h)
        {
            HwndSourceParameters parameters = new HwndSourceParameters("DrtHwndSource", w, h);
            parameters.WindowStyle = NativeConstants.WS_VISIBLE | NativeConstants.WS_OVERLAPPEDWINDOW;
            parameters.SetPosition(x, y);
            parameters.ParentWindow = IntPtr.Zero;
            parameters.HwndSourceHook = new HwndSourceHook(ApplicationFilterMessage);
    
            this.Source = new HwndSource(parameters);
        }


        /// <summary>
        /// The underlying HWND source.
        /// </summary>
        public HwndSource Source
        {
            get { return _source; }
            set { _source = value; }
        }
        private HwndSource _source;
        

        /// <summary>
        /// Application hook to handle incoming Windows messages.
        /// </summary>
        /// <param name="hwnd">Window handle.</param>
        /// <param name="msg">Message sent to the window.</param>
        /// <param name="wParam">WPARAM value of the message.</param>
        /// <param name="lParam">LPARAM value of the message.</param>
        /// <param name="handled">Handled-ness of the message.</param>
        /// <returns>0 if everything is OK, non-zero otherwise.</returns>
        private IntPtr ApplicationFilterMessage(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Quit the application if the source window is closed.
            if  ( (msg == NativeConstants.WM_CLOSE) )
            {
                DispatcherHelper.ShutDownBackground();

                handled = true;
            }

            return IntPtr.Zero;
        }


    }
}



