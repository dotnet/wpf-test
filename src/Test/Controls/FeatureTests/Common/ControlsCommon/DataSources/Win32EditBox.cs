using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// We use this win32 editbox to test RestoreFocusMode feature.
    /// </summary>
    public class Win32EditBox : HwndHost
    {
        // For Win32 focus testing purposes, it's useful to be able to get the Hwnd of this textbox
        private IntPtr hwnd = IntPtr.Zero;
        public IntPtr Hwnd
        {
            get
            {
                return hwnd;
            }
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            hwnd = CreateWindowEx(0, "EDIT", "Win32 Edit Box", WS_CHILD | WS_VISIBLE, 0, 0, 200, 100, hwndParent, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
            return new HandleRef(this, hwnd);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr CreateWindowEx(
           uint dwExStyle,
           string lpClassName,
           string lpWindowName,
           uint dwStyle,
           int x,
           int y,
           int nWidth,
           int nHeight,
           HandleRef hWndParent,
           IntPtr hMenu,
           IntPtr hInstance,
           IntPtr lpParam);

        [DllImport("user32.dll", EntryPoint = "DestroyWindow", CharSet = CharSet.Auto)]
        internal static extern bool DestroyWindow(
            IntPtr hwnd);

        private const uint WS_CHILD = 0x40000000;
        private const uint WS_MINIMIZE = 0x20000000;
        private const uint WS_VISIBLE = 0x10000000;
    }
}
