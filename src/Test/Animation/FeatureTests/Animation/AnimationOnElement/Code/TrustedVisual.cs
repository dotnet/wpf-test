// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//  This file provides a place for all unsafe pixel pushing manipulations
//  using user32.dll and standard Win32 API.
//
//
// $Id:$ $Change:$
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

using System.Drawing;
using System.Drawing.Imaging;

using System.Windows;
using System.Windows.Media;


namespace Microsoft.Test.Animation
{

    /// <summary>
    /// Win32/User32.dll wrappers ... Thanks to UIS team for this!
    /// </summary>
    internal class Win32
    {
        /// <summary>The GetDC function retrieves a handle to a display device context (DC)
        /// for the client area of a specified window or for the entire screen.</summary>
        /// <param name="hwnd">Handle to the window whose DC is to be retrieved.</param>
        /// <returns>A handle to the DC for the specified window's client area.</returns>
        [DllImport("User32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        /// <summary>Releases a device context (DC), freeing it for use by other
        /// applications.</summary>
        /// <param name="hwnd">Handle to the window whose DC is to be released.</param>
        /// <param name="hdc">Handle to the DC to be released.</param>
        /// <returns>1 if the DC was released, 0 otherwise.</returns>
        [DllImport("User32.dll")]
        public static extern int ReleaseDC(IntPtr hwnd, IntPtr hdc);

        /// <summary>Performs a bit-block transfer of the color data corresponding to
        /// a rectangle of pixels from the specified source device context into a
        /// destination device context.</summary>
        /// <param name="hdcDest">Handle to the destination device context.</param>
        /// <param name="xDest">Specifies the x-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="yDest">Specifies the y-coordinate, in logical units, of the upper-left corner of the destination rectangle.</param>
        /// <param name="cxDest">Specifies the width, in logical units, of the source and destination rectangles.</param>
        /// <param name="cyDest">Specifies the height, in logical units, of the source and the destination rectangles.</param>
        /// <param name="hdcSrc">Handle to the source device context.</param>
        /// <param name="xSrc">Specifies the x-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
        /// <param name="ySrc">Specifies the y-coordinate, in logical units, of the upper-left corner of the source rectangle.</param>
        /// <param name="dwRop">Specifies a raster-operation code.</param>
        /// <returns>true on success, false otherwise.</returns>
        [DllImport("gdi32.dll", CharSet=CharSet.Auto)]
        public static extern bool BitBlt(
            IntPtr hdcDest, int xDest, int yDest, int cxDest, int cyDest,
            IntPtr hdcSrc, int xSrc, int ySrc, int dwRop);

        /// <summary>SRCCOPY raster operation code.</summary>
        public const int SRCCOPY        = 0x00CC0020;

        /// <summary>Defines the coordinates of the upper-left and lower-right corners
        /// of a rectangle.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            /// <summary>Specifies the x-coordinate of the upper-left corner of the rectangle.</summary>
            public int left;
            /// <summary>Specifies the y-coordinate of the upper-left corner of the rectangle.</summary>
            public int top;
            /// <summary>Specifies the x-coordinate of the lower-right corner of the rectangle.</summary>
            public int right;
            /// <summary>Specifies the y-coordinate of the lower-right corner of the rectangle.</summary>
            public int bottom;

            /// <summary>Creates a new Win32.RECT instance.</summary>
            /// <param name="left">Specifies the x-coordinate of the upper-left corner of the rectangle.</param>
            /// <param name="top">Specifies the y-coordinate of the upper-left corner of the rectangle.</param>
            /// <param name="right">Specifies the x-coordinate of the lower-right corner of the rectangle.</param>
            /// <param name="bottom">Specifies the y-coordinate of the lower-right corner of the rectangle.</param>
            public RECT(int left, int top, int right, int bottom)
            {
                this.left   = left;
                this.top    = top;
                this.right  = right;
                this.bottom = bottom;
            }

            /// <summary>Calculated height.</summary>
            public int Height { get { return bottom - top; } }
            /// <summary>Calculated width.</summary>
            public int Width { get { return right - left; } }
        }
        /// <summary>
        /// Win32.POINT struct
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>Specifies the x-coordinate</summary>
            public int x;
            /// <summary>Specifies the y-coordinate</summary>
            public int y;

            /// <summary>Creates a new Win32.POINT instance.</summary>
            /// <param name="x">x coordinate</param>
            /// <param name="y">y coordinate</param>
            public POINT(int x, int y)
            {
                this.x   = x;
                this.y   = y;
            }
        }

        /// <summary>Retrieves the coordinates of a window's client area.</summary>
        /// <param name="hwnd">Handle to the window whose client coordinates are to be retrieved.</param>
        /// <param name="rc">RECT structure that receives the client coordinates.</param>
        /// <returns>true if the function succeeds, false otherwise.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(
            IntPtr hwnd, [In, Out] ref RECT rc);
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr ChildWindowFromPoint(IntPtr hWndParent, POINT point);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
    }

}




