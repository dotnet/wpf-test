// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//---------------------------------------------------------------------------
//
// 
//
// Description: Provides mouse and keyboard input functionality
//
//               Note: this fucntionality also is in ClientTestRuntime, but 
//                     is duplicated here to mitigate adding a new 
//                     dependency to existing perf tests
//               Note: This actually consoldiates 4 sources files found in
//                     ClientTestRuntime:
//                          SafeNativeMethods.cs
//                          UnsaveNativeMethods.cs
//                          NativeMethods.cs
//                          Input.cs
//               Note: The input class itself was renamed to InputPerf so as
//                     not to collide with the System.Windows.Input namespace
//                     which is used for the key definitions that InputPerf 
//                     requires
//               Note: Do not call MoveToAndClick (actually, do not make any calls 
//                     to UIAutomationClient) or MoveTo from the UI thread if your test is in 
//                     the same process as the UI being tested.  UIAutomation calls 
//                     back into Avalon core for UI information (e.g. ClickablePoint) 
//                     and must be on the UI thread to get this information.  If your 
//                     test is making calls from the UI thread you are going to deadlock...
//
//                     The version in ClientTestRuntime has MoveTo/MoveToAndClick
//                     implementations which use UIElment instead of AutomationElement
//                     not ported over to reduce working set size (didn't seem to be
//                     needed).
//---------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Security.Permissions;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Collections;
using System.IO;
using System.Text;

namespace Microsoft.Test.Performance
{
    /// <summary>
    /// Flags for PerfInput.SendMouseInput, indicate whether movent took place,
    /// or whether buttons were pressed or released.
    /// </summary>
    [Flags]
    public enum SendMouseInputFlags
    {
        /// <summary>Specifies that the pointer moved.</summary>
        Move       = 0x0001,
        /// <summary>Specifies that the left button was pressed.</summary>
        LeftDown   = 0x0002,
        /// <summary>Specifies that the left button was released.</summary>
        LeftUp     = 0x0004,
        /// <summary>Specifies that the right button was pressed.</summary>
        RightDown  = 0x0008,
        /// <summary>Specifies that the right button was released.</summary>
        RightUp    = 0x0010,
        /// <summary>Specifies that the middle button was pressed.</summary>
        MiddleDown = 0x0020,
        /// <summary>Specifies that the middle button was released.</summary>
        MiddleUp   = 0x0040,
        /// <summary>Specifies that the x button was pressed.</summary>
        XDown      = 0x0080,
        /// <summary>Specifies that the x button was released. </summary>
        XUp        = 0x0100,
        /// <summary>Specifies that the wheel was moved</summary>
        Wheel      = 0x0800,
        /// <summary>Specifies that x, y are absolute, not relative</summary>
        Absolute   = 0x8000,
    };


    /// <summary>
    /// Provides methods for sending mouse and keyboard input
    /// </summary>
    public sealed class InputPerf
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        // Static class - Private to prevent creation
        InputPerf()
        {
        }

        #endregion Constructors


        //------------------------------------------------------
        //
        //  Public Constants / Readonly Fields
        //
        //------------------------------------------------------

        #region Public Constants and Readonly Fields

        /// <summary>The first X mouse button</summary>
        public const int XButton1 = 0x01;

        /// <summary>The second X mouse button</summary>
        public const int XButton2 = 0x02;

        #endregion Public Constants and Readonly Fields


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// Inject pointer input into the system
        /// </summary>
        /// <param name="x">x coordinate of pointer, if Move flag specified</param>
        /// <param name="y">y coordinate of pointer, if Move flag specified</param>
        /// <param name="data">wheel movement, or mouse X button, depending on flags</param>
        /// <param name="flags">flags to indicate which type of input occurred - move, button press/release, wheel move, etc.</param>
        /// <remarks>x, y are in pixels. If Absolute flag used, are relative to desktop origin.</remarks>
        /// 
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
        public static void SendMouseInput( double x, double y, int data, SendMouseInputFlags flags )
        {
            //CASRemoval:AutomationPermission.Demand( AutomationPermissionFlag.Input );

            int intflags = (int) flags;

            if( ( intflags & (int)SendMouseInputFlags.Absolute ) != 0 )
            {
                int vscreenWidth = SafeNativeMethods.GetSystemMetrics( SafeNativeMethods.SM_CXVIRTUALSCREEN );
                int vscreenHeight = SafeNativeMethods.GetSystemMetrics( SafeNativeMethods.SM_CYVIRTUALSCREEN );
                int vscreenLeft = SafeNativeMethods.GetSystemMetrics( SafeNativeMethods.SM_XVIRTUALSCREEN );
                int vscreenTop = SafeNativeMethods.GetSystemMetrics( SafeNativeMethods.SM_YVIRTUALSCREEN );

                // Absolute input requires that input is in 'normalized' coords - with the entire
                // desktop being (0,0)...(65535,65536). Need to convert input x,y coords to this
                // first.
                //
                // In this normalized world, any pixel on the screen corresponds to a block of values
                // of normalized coords - eg. on a 1024x768 screen,
                // y pixel 0 corresponds to range 0 to 85.333,
                // y pixel 1 corresponds to range 85.333 to 170.666,
                // y pixel 2 correpsonds to range 170.666 to 256 - and so on.
                // Doing basic scaling math - (x-top)*65536/Width - gets us the start of the range.
                // However, because int math is used, this can end up being rounded into the wrong
                // pixel. For example, if we wanted pixel 1, we'd get 85.333, but that comes out as
                // 85 as an int, which falls into pixel 0's range - and that's where the pointer goes.
                // To avoid this, we add on half-a-"screen pixel"'s worth of normalized coords - to
                // push us into the middle of any given pixel's range - that's the 65536/(Width*2)
                // part of the formula. So now pixel 1 maps to 85+42 = 127 - which is comfortably
                // in the middle of that pixel's block.
                // The key ting here is that unlike points in coordinate geometry, pixels take up
                // space, so are often better treated like rectangles - and if you want to target
                // a particular pixel, target its rectangle's midpoint, not its edge.
                x = ( ( x - vscreenLeft ) * 65536 ) / vscreenWidth + 65536 / ( vscreenWidth * 2 );
                y = ( ( y - vscreenTop ) * 65536 ) / vscreenHeight + 65536 / ( vscreenHeight * 2 );

                intflags |= UnsafeNativeMethods.MOUSEEVENTF_VIRTUALDESK;
            }

            UnsafeNativeMethods.INPUT mi = new UnsafeNativeMethods.INPUT();
            mi.type = UnsafeNativeMethods.INPUT_MOUSE;
            mi.union.mouseInput.dx = (int) x;
            mi.union.mouseInput.dy = (int)y;
            mi.union.mouseInput.mouseData = data;
            mi.union.mouseInput.dwFlags = intflags;
            mi.union.mouseInput.time = 0;
            mi.union.mouseInput.dwExtraInfo = new IntPtr( 0 );
            //Console.WriteLine("Sending");
            if( UnsafeNativeMethods.SendInput( 1, ref mi, Marshal.SizeOf(mi) ) == 0 )
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }


        /// <summary>
        /// Inject keyboard input into the system
        /// </summary>
        /// <param name="key">indicates the key pressed or released. Can be one of the constants defined in the Key enum</param>
        /// <param name="press">true to inject a key press, false to inject a key release</param>
        /// 
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
        public static void SendKeyboardInput( Key key, bool press )
        {
            //CASRemoval:AutomationPermission.Demand( AutomationPermissionFlag.Input );

            UnsafeNativeMethods.INPUT ki = new UnsafeNativeMethods.INPUT();
            ki.type = UnsafeNativeMethods.INPUT_KEYBOARD;
            ki.union.keyboardInput.wVk = (short) KeyInterop.VirtualKeyFromKey( key );
            ki.union.keyboardInput.wScan = (short) UnsafeNativeMethods.MapVirtualKey( ki.union.keyboardInput.wVk, 0 );
            int dwFlags = 0;
            if( ki.union.keyboardInput.wScan > 0 )
                dwFlags |= UnsafeNativeMethods.KEYEVENTF_SCANCODE;
            if( !press )
                dwFlags |= UnsafeNativeMethods.KEYEVENTF_KEYUP;
            ki.union.keyboardInput.dwFlags = dwFlags;
            if( IsExtendedKey( key ) )
            {
                ki.union.keyboardInput.dwFlags |= UnsafeNativeMethods.KEYEVENTF_EXTENDEDKEY;
            }
            ki.union.keyboardInput.time = 0;
            ki.union.keyboardInput.dwExtraInfo = new IntPtr( 0 );
            if( UnsafeNativeMethods.SendInput( 1, ref ki, Marshal.SizeOf(ki) ) == 0 )
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Injects a unicode character as keyboard input into the system
        /// </summary>
        /// <param name="key">indicates the key to be pressed or released. Can be any unicode character</param>
        /// <param name="press">true to inject a key press, false to inject a key release</param>
        /// 
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
        public static void SendUnicodeKeyboardInput(char key, bool press)
        {
            //CASRemoval:AutomationPermission.Demand(AutomationPermissionFlag.Input);

            UnsafeNativeMethods.INPUT ki = new UnsafeNativeMethods.INPUT();

            ki.type = UnsafeNativeMethods.INPUT_KEYBOARD;
            ki.union.keyboardInput.wVk = (short)0;
            ki.union.keyboardInput.wScan = (short)key;
            ki.union.keyboardInput.dwFlags = UnsafeNativeMethods.KEYEVENTF_UNICODE | (press ? 0 : UnsafeNativeMethods.KEYEVENTF_KEYUP);
            ki.union.keyboardInput.time = 0;
            ki.union.keyboardInput.dwExtraInfo = new IntPtr(0);
            if (UnsafeNativeMethods.SendInput(1, ref ki, Marshal.SizeOf(ki)) == 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        /// <summary>
        /// Injects a string of Unicode characters using simulated keyboard input
        /// It should be noted that this overload just sends the whole string
        /// with no pauses, depending on the recieving applications input processing
        /// it may not be able to keep up with the speed, resulting in corruption or
        /// loss of the input data.
        /// </summary>
        /// <param name="data">The unicode string to be sent</param>
        public static void SendUnicodeString(string data)
        {
            InternalSendUnicodeString(data, -1, 0);
        }

        /// <summary>
        /// Injects a string of Unicode characters using simulated keyboard input
        /// with user defined timing.
        /// </summary>
        /// <param name="data">The unicode string to be sent</param>
        /// <param name="sleepFrequency">How many characters to send between sleep calls</param>
        /// <param name="sleepLength">How long, in milliseconds, to sleep for at each sleep call</param>
        public static void SendUnicodeString(string data, int sleepFrequency, int sleepLength)
        {
            if( sleepFrequency < 1 )
                throw new ArgumentOutOfRangeException("sleepFrequency");
            if( sleepLength < 0 )
                throw new ArgumentOutOfRangeException("sleepLength");
            InternalSendUnicodeString(data, sleepFrequency, sleepLength);
        }

        /// <summary>
        /// Checks whether the specified key is currently up or down
        /// </summary>
        /// <param name="key">The Key to check</param>
        /// <returns>true if the specified key is currently down (being pressed), false if it is up</returns>
        public static bool GetAsyncKeyState(Key key)
        {
            int vKey = KeyInterop.VirtualKeyFromKey(key);
            int resp = UnsafeNativeMethods.GetAsyncKeyState(vKey);

            if( resp == 0 )
                throw new InvalidOperationException("GetAsyncKeyStateFailed");

            return resp < 0;
        }

        /// <summary>
        /// Move the mouse to an element. 
        ///
        /// IMPORTANT!
        /// 
        /// Do not call MoveToAndClick (actually, do not make any calls to UIAutomationClient) 
        /// from the UI thread if your test is in the same process as the UI being tested.  
        /// UIAutomation calls back into Avalon core for UI information (e.g. ClickablePoint) 
        /// and must be on the UI thread to get this information.  If your test is making calls 
        /// from the UI thread you are going to deadlock...
        /// 
        /// </summary>
        /// <param name="el">The element that the mouse will move to</param>
        /// <exception cref="NoClickablePointException">If there is not clickable point for the element</exception>
        /// 
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
        public static void MoveTo( AutomationElement el )
        {
            if (el == null)
            {
                throw new ArgumentNullException("el");
            }
            MoveTo( el.GetClickablePoint() );
        }

        /// <summary>
        /// Move the mouse to a point. 
        /// </summary>
        /// <param name="pt">The point that the mouse will move to.</param>
        /// <remarks>pt are in pixels that are relative to desktop origin.</remarks>
        /// 
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
        public static void MoveTo( Point pt )
        {
            SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );
        }

        /// <summary>
        /// Move the mouse to an element and click on it.  The primary mouse button will be used
        /// this is usually the left button except if the mouse buttons are swaped.
        ///
        /// IMPORTANT!
        /// 
        /// Do not call MoveToAndClick (actually, do not make any calls to UIAutomationClient) 
        /// from the UI thread if your test is in the same process as the UI being tested.  
        /// UIAutomation calls back into Avalon core for UI information (e.g. ClickablePoint) 
        /// and must be on the UI thread to get this information.  If your test is making calls 
        /// from the UI thread you are going to deadlock...
        /// 
        /// </summary>
        /// <param name="el">The element to click on</param>
        /// <exception cref="NoClickablePointException">If there is not clickable point for the element</exception>
        /// 
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
        public static void MoveToAndClick( AutomationElement el )
        {
            if (el == null)
            {
                throw new ArgumentNullException("el");
            }
            MoveToAndClick(el.GetClickablePoint());
        }

        /// <summary>
        /// Move the mouse to a point and click.  The primary mouse button will be used
        /// this is usually the left button except if the mouse buttons are swaped.
        /// </summary>
        /// <param name="pt">The point to click at</param>
        /// <remarks>pt are in pixels that are relative to desktop origin.</remarks>
        /// 
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
        public static void MoveToAndClick( Point pt )
        {
            SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );

            // send SendMouseInput works in term of the phisical mouse buttons, therefore we need
            // to check to see if the mouse buttons are swapped because this method need to use the primary
            // mouse button.
            if ( SafeNativeMethods.GetSystemMetrics( SafeNativeMethods.SM_SWAPBUTTON ) == 0 )
            {
                // the mouse buttons are not swaped the primary is the left
                SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.LeftDown | SendMouseInputFlags.Absolute );
                SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.LeftUp | SendMouseInputFlags.Absolute );
            }
            else
            {
                // the mouse buttons are swaped so click the right button which as actually the primary
                SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.RightDown | SendMouseInputFlags.Absolute );
                SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.RightUp | SendMouseInputFlags.Absolute );
            }
        }

        #endregion Public Methods


        //------------------------------------------------------
        //
        //  Internal Methods
        //
        //------------------------------------------------------

        #region Internal Methods


        // Used internally by the HWND SetFocus code - it sends a hotkey to
        // itself - because it uses a VK that's not on the keyboard, it needs
        // to send the VK directly, not the scan code, which regular
        // SendKeyboardInput does.
        // Note that this method is public, but this class is private, so
        // this is not externally visible.
        internal static void SendKeyboardInputVK( byte vk, bool press )
        {
            UnsafeNativeMethods.INPUT ki = new UnsafeNativeMethods.INPUT();
            ki.type = UnsafeNativeMethods.INPUT_KEYBOARD;
            ki.union.keyboardInput.wVk = vk;
            ki.union.keyboardInput.wScan = 0;
            ki.union.keyboardInput.dwFlags = press ? 0 : UnsafeNativeMethods.KEYEVENTF_KEYUP;
            ki.union.keyboardInput.time = 0;
            ki.union.keyboardInput.dwExtraInfo = new IntPtr( 0 );
            if( UnsafeNativeMethods.SendInput( 1, ref ki, Marshal.SizeOf(ki) ) == 0 )
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }


        internal static bool IsExtendedKey( Key key )
        {
            // From the SDK:
            // The extended-key flag indicates whether the keystroke message originated from one of
            // the additional keys on the enhanced keyboard. The extended keys consist of the ALT and
            // CTRL keys on the right-hand side of the keyboard; the INS, DEL, HOME, END, PAGE UP,
            // PAGE DOWN, and arrow keys in the clusters to the left of the numeric keypad; the NUM LOCK
            // key; the BREAK (CTRL+PAUSE) key; the PRINT SCRN key; and the divide (/) and ENTER keys in
            // the numeric keypad. The extended-key flag is set if the key is an extended key. 
            //
            // - docs appear to be incorrect. Use of Spy++ indicates that break is not an extended key.
            // Also, menu key and windows keys also appear to be extended.
            return key == Key.RightAlt
                || key == Key.RightCtrl
                || key == Key.NumLock
                || key == Key.Insert
                || key == Key.Delete
                || key == Key.Home
                || key == Key.End
                || key == Key.Prior
                || key == Key.Next
                || key == Key.Up
                || key == Key.Down
                || key == Key.Left
                || key == Key.Right
                || key == Key.Apps
                || key == Key.RWin
                || key == Key.LWin;

            // Note that there are no distinct values for the following keys:
            // numpad divide
            // numpad enter
        }

        #endregion Internal Methods


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        // Injects a string of Unicode characters using simulated keyboard input
        // with user defined timing
        // <param name="data">The unicode string to be sent</param>
        // <param name="sleepFrequency">How many characters to send between sleep calls
        // A sleepFrequency of -1 means to never sleep</param>
        // <param name="sleepLength">How long, in milliseconds, to sleep for at each sleep call</param>
        private static void InternalSendUnicodeString(string data, int sleepFrequency, int sleepLength)
        {
            char[] chardata = data.ToCharArray();
            int counter = -1;

            foreach (char c in chardata)
            {
                // Every sleepFrequency characters, sleep for sleepLength ms to avoid overflowing the input buffer.
                if (++counter == sleepFrequency)
                {
                    counter = 0;
                    System.Threading.Thread.Sleep(sleepLength);
                }

                SendUnicodeKeyboardInput(c, true);
                SendUnicodeKeyboardInput(c, false);
            }
        }

        #endregion Private Methods

    }

    // This class *MUST* be internal for security purposes
    //CASRemoval:[SuppressUnmanagedCodeSecurity]
    internal class SafeNativeMethods
    {
        //
        // GetSystemMetrics
        //

        public const int SM_CXMAXTRACK = 59;
        public const int SM_CYMAXTRACK = 60;
        public const int SM_XVIRTUALSCREEN = 76;
        public const int SM_YVIRTUALSCREEN = 77;
        public const int SM_CXVIRTUALSCREEN = 78;
        public const int SM_CYVIRTUALSCREEN = 79;
        public const int SM_SWAPBUTTON = 23;


        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int metric);

        //
        // Window style information
        //

        public const int GWL_STYLE = -16;
        public const int GWL_EXSTYLE = -20;

        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;
        public const int ES_MULTILINE = 0x0004;
        public const int ES_AUTOVSCROLL = 0x0040;
        public const int ES_AUTOHSCROLL = 0x0080;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowLong(NativeMethods.HWND hWnd, int nIndex);

    }

    internal class NativeMethods
    {
        //        public const int MAX_PATH   = 260;
        //
        [StructLayout(LayoutKind.Sequential)]
        public struct HWND
        {
            public IntPtr h;

            public static HWND Cast(IntPtr h)
            {
                HWND hTemp = new HWND();
                hTemp.h = h;
                return hTemp;
            }

            public static implicit operator IntPtr(HWND h)
            {
                return h.h;
            }

            public static HWND NULL
            {
                get
                {
                    HWND hTemp = new HWND();
                    hTemp.h = IntPtr.Zero;
                    return hTemp;
                }
            }

            public static bool operator ==(HWND hl, HWND hr)
            {
                return hl.h == hr.h;
            }

            public static bool operator !=(HWND hl, HWND hr)
            {
                return hl.h != hr.h;
            }

            override public bool Equals(object oCompare)
            {
                HWND hr = Cast((HWND)oCompare);
                return h == hr.h;
            }

            public override int GetHashCode()
            {
                return (int)h;
            }
        }
    }

    // This class *MUST* be internal for security purposes
    //CASRemoval:[SuppressUnmanagedCodeSecurity]
    internal class UnsafeNativeMethods
    {
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        //
        // SendInput related
        //

        public const int VK_SHIFT = 0x10;
        public const int VK_CONTROL = 0x11;
        public const int VK_MENU = 0x12;

        public const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        public const int KEYEVENTF_KEYUP = 0x0002;
        public const int KEYEVENTF_UNICODE = 0x0004;
        public const int KEYEVENTF_SCANCODE = 0x0008;

        public const int MOUSEEVENTF_VIRTUALDESK = 0x4000;

        [StructLayout(LayoutKind.Sequential)]
        public struct INPUT
        {
            public int type;
            public INPUTUNION union;
        };

        [StructLayout(LayoutKind.Explicit)]
        public struct INPUTUNION
        {
            [FieldOffset(0)]
            public MOUSEINPUT mouseInput;
            [FieldOffset(0)]
            public KEYBDINPUT keyboardInput;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public int mouseData;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct KEYBDINPUT
        {
            public short wVk;
            public short wScan;
            public int dwFlags;
            public int time;
            public IntPtr dwExtraInfo;
        };

        public const int INPUT_MOUSE = 0;
        public const int INPUT_KEYBOARD = 1;

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int SendInput(int nInputs, ref INPUT mi, int cbSize);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
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
        public static extern IntPtr SendMessage(
            NativeMethods.HWND hWnd, int nMsg, IntPtr wParam, IntPtr lParam);

        // Overload for WM_GETTEXT
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(
            NativeMethods.HWND hWnd, int nMsg, IntPtr wParam, StringBuilder lParam);

        public const int EM_SETSEL = 0x00B1;
        public const int EM_GETLINECOUNT = 0x00BA;
        public const int EM_LINEFROMCHAR = 0x00C9;
    }
}
