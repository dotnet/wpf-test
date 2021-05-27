// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace HostingUtilities
{
    class GDI32
    {
        [DllImport("GDI32.dll")]
        public static extern bool BitBlt(IntPtr hDcDest, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hDcSrc, int nXSrc, int nYSrc, int dwRop);
        [DllImport("GDI32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDc, int nWidth, int nHeight);
        [DllImport("GDI32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteDC(IntPtr hDc);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(IntPtr hDc, int nIndex);
        [DllImport("GDI32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDc, IntPtr hgdiobj);
    }

    class User32
    {
        [DllImport("User32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("User32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("User32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);
    }

    class ScreenCapture
    {
        public static void Snapshot(string fileName)
        {
            IntPtr hDcSrc = User32.GetWindowDC(User32.GetDesktopWindow());
            IntPtr hDcDest = GDI32.CreateCompatibleDC(hDcSrc);
            IntPtr hBitmap = GDI32.CreateCompatibleBitmap(hDcSrc, GDI32.GetDeviceCaps(hDcSrc, 8), GDI32.GetDeviceCaps(hDcSrc, 10));

            try
            {
                GDI32.SelectObject(hDcDest, hBitmap);
                GDI32.BitBlt(hDcDest, 0, 0, GDI32.GetDeviceCaps(hDcSrc, 8), GDI32.GetDeviceCaps(hDcSrc, 10), hDcSrc, 0, 0, 0x00CC0020);
                Image.FromHbitmap(hBitmap).Save(fileName, ImageFormat.Png);
            }
            finally
            {
                User32.ReleaseDC(User32.GetDesktopWindow(), hDcSrc);
                GDI32.DeleteDC(hDcDest);
                GDI32.DeleteObject(hBitmap);
            }
        }
    }
}
