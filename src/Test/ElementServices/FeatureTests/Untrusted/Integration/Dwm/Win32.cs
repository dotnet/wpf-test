// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//  Win32
//

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Avalon.Test.CoreUI.Dwm
{
    /// <summary>
    /// 
    /// </summary>
    public class Win32
    {
        //
        //  Structures
        //

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SIZE
        {
            /// <summary>
            /// 
            /// </summary>
            public int cx;
            /// <summary>
            /// 
            /// </summary>
            public int cy;
        };

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>
            /// 
            /// </summary>
            public int left;
            /// <summary>
            /// 
            /// </summary>
            public int top;
            /// <summary>
            /// 
            /// </summary>
            public int right;
            /// <summary>
            /// 
            /// </summary>
            public int bottom;
        };


        //
        //  Functions
        //

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="rect"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetClientRect(
            IntPtr hwnd,
            ref RECT rect);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateRectRgn(
            int left,
            int top,
            int right,
            int bottom);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        /// <param name="widthOfEllipse"></param>
        /// <param name="heightOfEllipse"></param>
        /// <returns></returns>
        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateRoundRectRgn(
            int left,
            int top,
            int right,
            int bottom,
            int widthOfEllipse,   // width of the ellipse used to create the rounded corners
            int heightOfEllipse);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="hrgn"></param>
        /// <param name="redrawWindow"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SetWindowRgn(
            IntPtr hwnd,
            IntPtr hrgn,
            bool redrawWindow);

    }
}
