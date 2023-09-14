// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Unsafe P/Invokes used by UIAutomation
//

//

using System.Threading;
using System;
using Accessibility;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Collections;
using System.IO;
using System.Text;
using System.Security;

namespace MS.Win32
{

    // This class *MUST* be internal for security purposes
    //CASRemoval:[SuppressUnmanagedCodeSecurity]
    internal class UnsafeNativeMethods
    {
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        internal static Guid IID_IUnknown = new Guid(0x00000000, 0x0000, 0x0000, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
        internal static Guid IID_IDispatch = new Guid(0x00020400, 0x0000, 0x0000, 0xC0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46);
        internal static Guid IID_IAccessible = new Guid(0x618736e0, 0x3c3d, 0x11cf, 0x81, 0x0c, 0x00, 0xaa, 0x00, 0x38, 0x9b, 0x71);

        [DllImport("oleacc.dll")]
        public static extern int AccessibleObjectFromWindow(IntPtr hwnd, int idObject, ref Guid iid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppvObject);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessageTimeout(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, int flags, int uTimeout, out IntPtr pResult);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessageTimeout(IntPtr hwnd, int uMsg, IntPtr wParam, StringBuilder lParam, int flags, int uTimeout, out IntPtr result);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessageTimeout(IntPtr hwnd, int uMsg, IntPtr wParam, ref NativeMethods.Win32Rect lParam, int flags, int uTimeout, out IntPtr result);
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr SendMessageTimeout(IntPtr hwnd, int uMsg, out int wParam, out int lParam, int flags, int uTimeout, out IntPtr result);
        [DllImport("User32.dll", ExactSpelling = true, SetLastError = true)]
        internal static extern int MapWindowPoints(IntPtr hWndFrom, IntPtr hWndTo, [In, Out] ref NativeMethods.Win32Rect rect, int cPoints);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        internal static extern bool GetVersionEx([In, Out] NativeMethods.OSVERSIONINFOEX ver);
        //
        // SendInput related
        //
        ///<summary/>
        [DllImport("user32.dll")]
        internal static extern ushort VkKeyScan(char ch);

        public const int VK_SHIFT    = 0x10;
        public const int VK_CONTROL  = 0x11;
        public const int VK_MENU     = 0x12;
        public const byte VK_LBUTTON = 0x01;
        public const byte VK_RBUTTON = 0x02;
        public const byte VK_CANCEL = 0x03;
        public const byte VK_MBUTTON = 0x04;    /* NOT contiguous with L & RBUTTON */
        public const byte VK_XBUTTON1 = 0x05;    /* NOT contiguous with L & RBUTTON */
        public const byte VK_XBUTTON2 = 0x06;    /* NOT contiguous with L & RBUTTON */
        public const byte VK_BACK = 0x08;
        public const byte VK_TAB = 0x09;
        public const byte VK_CLEAR = 0x0C;
        public const byte VK_RETURN = 0x0D;
        public const byte VK_PAUSE = 0x13;
        public const byte VK_CAPITAL = 0x14;
        public const byte VK_KANA = 0x15;
        public const byte VK_HANGEUL = 0x15;  /* old name - should be here for compatibility */
        public const byte VK_HANGUL = 0x15;
        public const byte VK_JUNJA = 0x17;
        public const byte VK_FINAL = 0x18;
        public const byte VK_HANJA = 0x19;
        public const byte VK_KANJI = 0x19;
        public const byte VK_ESCAPE = 0x1B;
        public const byte VK_CONVERT = 0x1C;
        public const byte VK_NONCONVERT = 0x1D;
        public const byte VK_ACCEPT = 0x1E;
        public const byte VK_MODECHANGE = 0x1F;
        public const byte VK_SPACE = 0x20;
        public const byte VK_PRIOR = 0x21;
        public const byte VK_NEXT = 0x22;
        public const byte VK_END = 0x23;
        public const byte VK_HOME = 0x24;
        public const byte VK_LEFT = 0x25;
        public const byte VK_UP = 0x26;
        public const byte VK_RIGHT = 0x27;
        public const byte VK_DOWN = 0x28;
        public const byte VK_SELECT = 0x29;
        public const byte VK_PRINT = 0x2A;
        public const byte VK_EXECUTE = 0x2B;
        public const byte VK_SNAPSHOT = 0x2C;
        public const byte VK_INSERT = 0x2D;
        public const byte VK_DELETE = 0x2E;
        public const byte VK_HELP = 0x2F;
        public const byte VK_LSHIFT = 0xA0;
        public const byte VK_RSHIFT = 0xA1;
        public const byte VK_LCONTROL = 0xA2;
        public const byte VK_RCONTROL = 0xA3;
        public const byte VK_LMENU = 0xA4;
        public const byte VK_RMENU = 0xA5;
        public const byte VK_NUMLOCK = 0x90;
        public const byte VK_SCROLL = 0x91;
        public const byte VK_LWIN = 0x5B;
        public const byte VK_RWIN = 0x5C;
        public const byte VK_APPS = 0x5D;

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP       = 0x0002;
        public const int KEYEVENTF_UNICODE     = 0x0004;
        public const int KEYEVENTF_SCANCODE    = 0x0008;

        public const int MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000;
        public const int MOUSEEVENTF_VIRTUALDESK = 0x4000;

        [StructLayout(LayoutKind.Sequential)]
            public struct INPUT
        {
            public int    type;
            public INPUTUNION    union;
        };

        [StructLayout(LayoutKind.Explicit)]
            public struct INPUTUNION
        {
            [FieldOffset(0)] public MOUSEINPUT mouseInput;
            [FieldOffset(0)] public KEYBDINPUT keyboardInput;
        };

        [StructLayout(LayoutKind.Sequential)]
            public struct MOUSEINPUT
        {
            public int    dx;
            public int    dy;
            public int    mouseData;
            public int    dwFlags;
            public int    time;
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
            public struct KEYBDINPUT
        {
            public short  wVk;
            public short  wScan;
            public int    dwFlags;
            public int    time;
            public IntPtr dwExtraInfo;
        };

        public const int INPUT_MOUSE             = 0;
        public const int INPUT_KEYBOARD          = 1;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SendInput( int nInputs, ref INPUT mi, int cbSize );

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int MapVirtualKey(int nVirtKey, int nMapType);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetAsyncKeyState(int nVirtKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetKeyState(int nVirtKey);

        //
        // Keyboard state
        //
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern int GetKeyboardState(byte[] keystate);

        [DllImport("user32.dll", ExactSpelling = true, EntryPoint = "keybd_event", CharSet = CharSet.Auto)]
        internal static extern void Keybd_event(byte vk, byte scan, int flags, int extrainfo);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        internal static extern int SetKeyboardState(byte[] keystate);


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(NativeMethods.HWND hWnd, int nMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hwnd, int msg, IntPtr wParam, StringBuilder sb);


        // Overload for WM_GETTEXT
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(NativeMethods.HWND hWnd, int nMsg, IntPtr wParam, StringBuilder lParam);

        public const int EM_SETSEL               = 0x00B1;
        public const int EM_GETLINECOUNT         = 0x00BA;
        public const int EM_LINEFROMCHAR         = 0x00C9;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr FindWindow(string className, string windowName);


    }
}
