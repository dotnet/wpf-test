// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// MultiTouch related constants, DLLImports, structs, etc. 
    /// </summary>
    public static class MultiTouchNativeMethods
    {
        #region Constants

        public const int GCF_ABORTIFHUNG = 2;
        public const int GCF_ABORTIFHUNGANY = 4;
        public const int GCF_SYNC = 1;
        public const int GCI_COMMAND_INERTIA = 4;
        public const int GCI_COMMAND_PAN = 2;
        public const int GCI_COMMAND_ROLLOVER = 6;
        public const int GCI_COMMAND_ROTATE = 3;
        public const int GCI_COMMAND_TWOFINGERTAP = 5;
        public const int GCI_COMMAND_ZOOM = 1;
        public const int LOGPIXELSX = 0x58;
        public const int LOGPIXELSY = 90;
        public const int TWF_FINETOUCH = 1;

        //Nonzero if the current operating system is Windows 7 or Windows Server 2008 R2 and the Tablet PC Input service is started; otherwise, 0. 
        //The return value is a bit mask that specifies the type of digitizer input supported by the device.
        //NOTE - Windows Server 2008, Windows Vista, and Windows XP/2000:  This value is not supported!!!
        public const int SM_DIGITIZER = 94;

        // Touch event window message constants 
        public const int WM_GESTURECOMMAND = 0x0119;
        public const int WM_TOUCHMOVE = 0x0240;
        public const int WM_TOUCHDOWN = 0x0241;
        public const int WM_TOUCHUP = 0x0242;
        public const int WM_FLICK = 715;
        public const int WM_TABLET_QUERYSYSTEMGESTURESTATUS = 716;

        // Gesture commands 
        public const int GID_ZOOM = 1;
        public const int GID_PAN = 2;
        public const int GID_ROTATE = 3;
        public const int GID_INERTIA = 4;
        public const int GID_TWOFINGERTAP = 5;
        public const int GID_ROLLOVER = 6;
        public const int GID_GESTURE_BEGIN = 252;
        public const int GID_GESTURE_END = 253;
        public const int GID_BEGIN = 254;
        public const int GID_END = 255;

        // Touch event flags
        public const int GESTURE_STATUSF_ROTATE_ENABLE = 0x02000000;
        public const int TOUCHEVENTF_MOVE = 0x0001;
        public const int TOUCHEVENTF_DOWN = 0x0002;
        public const int TOUCHEVENTF_UP = 0x0004;
        public const int TOUCHEVENTF_INRANGE = 0x0008;
        public const int TOUCHEVENTF_PRIMARY = 0x0010;
        public const int TOUCHEVENTF_NOCOALESCE = 0x0020;
        public const int TOUCHEVENTF_PEN = 0x0040;

        // Touch input mask values
        public const int TOUCHINPUTMASKF_TIMEFROMSYSTEM = 0x0001;  // the dwTime field contains a system generated value
        public const int TOUCHINPUTMASKF_EXTRAINFO = 0x0002;       // the dwExtraInfo field is valid
        public const int TOUCHINPUTMASKF_CONTACTAREA = 0x0004;     // the cxContact and cyContact fields are valid
        
        // General window message constants
        public const int WM_CLOSE = 0x0010;
        public const int WM_ACTIVATE = 0x0006;

        #endregion
        
        #region ExternDll

        public static class ExternDll
        {
            public const string Gdi32 = "gdi32.dll";
            public const string User32 = "user32.dll";
            public const string Uxtheme = "uxtheme.dll";
        }

        #endregion

        #region DllImport
        
        [DllImport(ExternDll.User32)]
        public static extern bool SetProp(
            IntPtr hWnd, 
            string lpString, 
            IntPtr hData);

        [DllImport(ExternDll.User32, BestFitMapping = false, CharSet = CharSet.Auto)]
        public static extern bool SetPropMT(
            HandleRef hWnd, 
            string propName, 
            HandleRef data);
        
        [DllImport(ExternDll.User32, ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int GetSystemMetrics(int nIndex);

        [DllImport(ExternDll.User32)]
        public static extern bool GetGestureCommandInfo(
            int uMsg, 
            IntPtr wParam, 
            IntPtr lParam, 
            IntPtr lExtraInfo, 
            out GESTURECOMMANDINFO pGestureCommandInfo);

        [DllImport(ExternDll.User32)]
        public static extern bool GetTouchInputInfo(
            IntPtr hTouchInput, 
            int cInput, 
            [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] TOUCHINPUT[] pInputs, 
            int cbSize);

        // WMGesture
        [DllImport(ExternDll.User32)]
        public static extern bool RegisterGestureHandlerWindow(
            IntPtr hwnd, 
            long flags);

        // WMTouch
        [DllImport(ExternDll.User32)]
        public static extern bool RegisterTouchWindow(
            IntPtr hwnd, 
            long flags);

        [DllImport(ExternDll.User32)]
        public static extern bool UnregisterGestureHandlerWindow(IntPtr hwnd);

        [DllImport(ExternDll.User32)]
        public static extern bool UnregisterTouchWindow(IntPtr hwnd);

        [DllImport(ExternDll.User32)]
        public static extern bool CloseTouchInputHandle(IntPtr hTouchInput);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Unicode)]
        public static extern bool BeginPanningFeedback(HandleRef hwnd);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Unicode)]
        public static extern bool UpdatePanningFeedback(
                                                HandleRef hwnd, 
                                                int lTotalOverpanOffsetX,
                                                int lTotalOverpanOffsetY, 
                                                bool fInInertia);

        [DllImport(ExternDll.Uxtheme, CharSet = CharSet.Unicode)]
        public static extern bool EndPanningFeedback(HandleRef hwnd, bool fAnimateBack);

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GetRawInputDeviceList(
                                                [In, Out] RAWINPUTDEVICELIST[] ridl,
                                                [In, Out] ref uint numDevices,
                                                uint sizeInBytes);

        [DllImport(ExternDll.User32, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GetRawInputDeviceInfo(
                                                IntPtr hDevice,
                                                uint command,
                                                [In] ref RID_DEVICE_INFO ridInfo,
                                                ref uint sizeInBytes);

        [DllImport(ExternDll.User32)]
        public static extern IntPtr GetDC(IntPtr HWND);

        [DllImport(ExternDll.Gdi32, SetLastError = true)]
        public static extern int GetDeviceCaps(
            IntPtr HDC, 
            int flagIndex);

        [DllImport(ExternDll.User32)]
        public static extern int ReleaseDC(
            IntPtr HWND, 
            IntPtr HDC);

        #endregion

        #region Structs

        [StructLayout(LayoutKind.Sequential)]
        public struct GESTURECOMMANDINFO
        {
            public uint cbSize;
            public uint dwFlags;
            public uint dwCommand;
            public uint dwArguments;
            public ushort usX;
            public ushort usY;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TOUCHINPUT
        {
            public int x;
            public int y;
            public IntPtr hSource;
            public uint dwID;
            public uint dwFlags;
            public uint dwMask;
            public uint dwTime;
            public UIntPtr dwExtraInfo;
            public uint cxContact;
            public uint cyContact;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINTS
        {
            public short x;
            public short y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct GESTUREINFO
        {
            public int cbSize;
            public int dwFlags;
            public int dwCommand;
            public int dwArguments;
            [MarshalAs(UnmanagedType.Struct)]
            public POINTS ptsLocation;
        }

        #region WPF Tablet

        [StructLayout(LayoutKind.Sequential)]
        public struct RAWINPUTDEVICELIST
        {
            public IntPtr hDevice;
            public uint dwType;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RID_DEVICE_INFO_MOUSE
        {
            public uint dwId;
            public uint dwNumberOfButtons;
            public uint dwSampleRate;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RID_DEVICE_INFO_KEYBOARD
        {
            public uint dwType;
            public uint dwSubType;
            public uint dwKeyboardMode;
            public uint dwNumberOfFunctionKeys;
            public uint dwNumberOfIndicators;
            public uint dwNumberOfKeysTotal;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RID_DEVICE_INFO_HID
        {
            public uint dwVendorId;
            public uint dwProductId;
            public uint dwVersionNumber;
            public ushort usUsagePage;
            public ushort usUsage;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct RID_DEVICE_INFO
        {
            [FieldOffset(0)]
            public uint cbSize;
            [FieldOffset(4)]
            public uint dwType;
            [FieldOffset(8)]
            public RID_DEVICE_INFO_MOUSE mouse;
            [FieldOffset(8)]
            public RID_DEVICE_INFO_KEYBOARD keyboard;
            [FieldOffset(8)]
            public RID_DEVICE_INFO_HID hid;
        }

        public const uint RIDI_DEVICEINFO = 0x2000000b;
        public const uint RIM_TYPEHID = 2;
        public const ushort HID_USAGE_PAGE_DIGITIZER = 0x0D;
        public const ushort HID_USAGE_DIGITIZER_DIGITIZER = 1;
        public const ushort HID_USAGE_DIGITIZER_PEN = 2;
        public const ushort HID_USAGE_DIGITIZER_LIGHTPEN = 3;
        public const ushort HID_USAGE_DIGITIZER_TOUCHSCREEN = 4;

        #endregion

        #endregion

        #region Input Interaction

        [DllImport(ExternDll.User32, CharSet = CharSet.Auto, EntryPoint = "BlockInput")]
        private static extern int Win32BlockInput(int fBlockIt);

        /// <summary>
        /// Prevent user from sending any input to the system.  (Press CTRL-ALT-DEL to unblock if you need to).
        /// </summary>
        private static void Win32BlockInput(bool blockIt)
        {
            int hr = Win32BlockInput(blockIt ? 1 : 0);
            if (hr < 0)
            {
                throw new Win32Exception();
            }
        }

        #endregion
    }
}
