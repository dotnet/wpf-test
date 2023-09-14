// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Provides mouse and keyboard input functionality
//
//

using System.Globalization;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Input;
using System.Security.Permissions;
using System.ComponentModel;
using System.Runtime.InteropServices;
using MS.Win32;
using System.Collections;
using System.Windows.Interop;

using System;

namespace ATGTestInput
{
    /// <summary>
    /// Flags for Input.SendMouseInput, indicate whether movent took place,
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
    public sealed class Input
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        // Static class - Private to prevent creation
        Input()
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

        /// <summary>Delay time between key up and down</summary>
        const int _millisecondKeyboardDelay = 10;

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

            // don't coalesce mouse moves - tests expect to see the results immediately
            if ((intflags & (int)SendMouseInputFlags.Move) != 0)
            {
                intflags |= UnsafeNativeMethods.MOUSEEVENTF_MOVE_NOCOALESCE;
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

            if ((intflags & (int)SendMouseInputFlags.Wheel) != 0)
            {
                // MouseWheel input seems to be getting coalesced by the OS, similar to mouse-move.
                // There isn't a NOCOALESCE flag to turn this off, so instead just sleep for
                // a short time, hopefully enough to avoid the coalescing.
                System.Threading.Thread.Sleep(50);
            }
        }

        //------------------------------------------------------
        /// <summary>
        /// Does a look up of the string in keyCombination, converts
        /// the string to a Keys enum, and then sends the results to
        /// SendKeyboardInput().
        /// </summary>
        //------------------------------------------------------
        internal static void SendKeyboardInput(string keyCombination)
        {
            SendKeyboardInput(keyCombination, _millisecondKeyboardDelay);
        }

        //------------------------------------------------------
        /// <summary>
        /// Does a look up of the string in keyCombination, converts
        /// the string to a Keys enum, and then sends the results to
        /// SendKeyboardInput().
        /// </summary>
        /// <param name="keyCombination">String representation such as "Ctrl+A"</param>
        /// <param name="millisecondDelay">Seconds to delay between
        /// each keystroke</param>
        //------------------------------------------------------
        internal static void SendKeyboardInput(string keyCombination, int millisecondDelay)
        {
            ArrayList keysSequence = new ArrayList();
            string[] keys = keyCombination.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
            object keyStroke;

            try
            {
                foreach (string key in keys)
                {
                    switch (key.Trim().ToLower())
                    {
                        case "space":
                            keysSequence.Add(Key.Space);
                            break;
                        case "~":
                            keysSequence.Add(Key.OemTilde);
                            break;
                        case "alt":
                            keysSequence.Add(Key.LeftAlt);
                            break;
                        case "shift":
                            keysSequence.Add(Key.LeftAlt);
                            break;
                        case "ctrl":
                            keysSequence.Add(Key.LeftCtrl);
                            break;
                        case "0":
                            keysSequence.Add(Key.D0);
                            break;
                        case "1":
                            keysSequence.Add(Key.D1);
                            break;
                        case "2":
                            keysSequence.Add(Key.D2);
                            break;
                        case "3":
                            keysSequence.Add(Key.D3);
                            break;
                        case "4":
                            keysSequence.Add(Key.D4);
                            break;
                        case "5":
                            keysSequence.Add(Key.D5);
                            break;
                        case "6":
                            keysSequence.Add(Key.D6);
                            break;
                        case "7":
                            keysSequence.Add(Key.D7);
                            break;
                        case "8":
                            keysSequence.Add(Key.D8);
                            break;
                        case "9":
                            keysSequence.Add(Key.D9);
                            break;
                        case "!":
                            keysSequence.Add(Key.LeftShift);
                            keysSequence.Add(Key.D1);
                            break;
                        case "@":
                            keysSequence.Add(Key.LeftShift);
                            keysSequence.Add(Key.D2);
                            break;
                        case "#":
                            keysSequence.Add(Key.LeftShift);
                            keysSequence.Add(Key.D3);
                            break;
                        case "$":
                            keysSequence.Add(Key.LeftShift);
                            keysSequence.Add(Key.D4);
                            break;
                        case "%":
                            keysSequence.Add(Key.LeftShift);
                            keysSequence.Add(Key.D5);
                            break;
                        case "^":
                            keysSequence.Add(Key.LeftShift);
                            keysSequence.Add(Key.D6);
                            break;
                        case "&":
                            keysSequence.Add(Key.LeftShift);
                            keysSequence.Add(Key.D7);
                            break;
                        case "*":
                            keysSequence.Add(Key.LeftShift);
                            keysSequence.Add(Key.D8);
                            break;
                        default:
                            keysSequence.Add((Key)Enum.Parse(typeof(Key), key.ToUpper()));
                            break;

                    }
                    if (key.Length == 1)
                    {
                        keyStroke = (short)(UnsafeNativeMethods.VkKeyScan(Convert.ToChar(key)));
                    }
                }

                if (keysSequence.Count > 0)
                {
                    // SendKeyboardInput requires an array of Keys[],
                    // keysSequence is a collection so need to copy
                    // the results over to the Keys[] and then sent
                    // it through.
                    Key[] keyCopy = new Key[keysSequence.Count];
                    for (int i = 0; i < keysSequence.Count; i++)
                        keyCopy[i] = (Key)keysSequence[i];

                    SendKeyboardInput(keyCopy, millisecondDelay);

                }
            }
            catch (Exception error)
            {
                System.Diagnostics.Trace.WriteLine(error);
            }
        }

        /// -------------------------------------------------------------------
        /// <summary>Helper: Sends the up and down keyboard input</summary>
        /// -------------------------------------------------------------------
        internal static void SendKeyboardInput(Key key)
        {
            SendKeyboardInput(key, _millisecondKeyboardDelay);
        }

        /// -------------------------------------------------------------------
        /// <summary>Helper: Sends the up and down keyboard input</summary>
        /// -------------------------------------------------------------------
        internal static void SendKeyboardInput(Key key, int millisecondDelay)
        {
            SendKeyboardInput(new Key[] { key }, millisecondDelay);
        }

        /// -------------------------------------------------------------------
        /// <summary>Helper: Sends the up and down keyboard input</summary>
        /// -------------------------------------------------------------------
        internal static void SendKeyboardInput(Key[] keys)
        {
            SendKeyboardInput(keys, _millisecondKeyboardDelay);
        }

        /// -------------------------------------------------------------------
        /// <summary>Helper: Sends the up and down keyboard input</summary>
        /// -------------------------------------------------------------------
        internal static void SendKeyboardInput(Key[] key, int millisecondDelay)
        {
            for (int i = 0; i < key.Length; i++)
            {
                SendKeyboardInput(key[i], true);

                if (millisecondDelay > 0)
                    Thread.Sleep(millisecondDelay);
            }
            for (int i = key.Length; i > 0; i--)
            {
                SendKeyboardInput(key[i - 1], false);

                if (millisecondDelay > 0)
                    Thread.Sleep(millisecondDelay);
            }
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

        ///---------------------------------------------------------------------------
        /// <summary>
        /// Gets WindowHandle from an AutomationElement
        /// </summary>
        ///---------------------------------------------------------------------------
        internal static IntPtr GetWindowHandleFromAutomationElement(AutomationElement element)
        {
            if( element == null )
                throw new ArgumentException("Automation Element cannot be null");

            AutomationElement parent = element;
            while (parent.Current.NativeWindowHandle == 0)
                parent = TreeWalker.ControlViewWalker.GetParent(parent);

            object objHwnd = parent.GetCurrentPropertyValue(AutomationElement.NativeWindowHandleProperty);
            IntPtr ptr = IntPtr.Zero;

            if (objHwnd is IntPtr)
                ptr = (IntPtr)objHwnd;
            else
                ptr = new IntPtr(Convert.ToInt64(objHwnd, CultureInfo.CurrentCulture));

            if (ptr == IntPtr.Zero)
                throw new InvalidOperationException("Could not get the handle to the element(window)");

            // use WPF way; get the HwndSource and add a WinProc to IT and will be used later
            //HwndSource source = HwndSource.FromHwnd(objHwnd);
            //source.AddHook(new HwndSourceHook(WndProc));

            return ptr;
        }

        /// <summary>
        /// Is bit set?
        /// </summary>
        internal static bool IsBitSet(int flags, int bit)
        {
            return (flags & bit) == bit;
        }

        /// <summary>
        /// Move the mouse to an element.
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
            Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );
        }

        /// <summary>
        /// Move the mouse to an element and click on it.  The primary mouse button will be used
        /// this is usually the left button except if the mouse buttons are swaped.
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
            Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.Move | SendMouseInputFlags.Absolute );

            // send SendMouseInput works in term of the phisical mouse buttons, therefore we need
            // to check to see if the mouse buttons are swapped because this method need to use the primary
            // mouse button.
            if ( SafeNativeMethods.GetSystemMetrics( SafeNativeMethods.SM_SWAPBUTTON ) == 0 )
            {
                // the mouse buttons are not swaped the primary is the left
                Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.LeftDown | SendMouseInputFlags.Absolute );
                Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.LeftUp | SendMouseInputFlags.Absolute );
            }
            else
            {
                // the mouse buttons are swaped so click the right button which as actually the primary
                Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.RightDown | SendMouseInputFlags.Absolute );
                Input.SendMouseInput( pt.X, pt.Y, 0, SendMouseInputFlags.RightUp | SendMouseInputFlags.Absolute );
            }
        }

        /// <summary>
        /// This is a hyrbird of the ValuePattern.SetValue code extracted from the UIAutomation sources
        /// NOTE: Requires the ability to set focus to the current automation element
        /// </summary>
        public static void SetValue( AutomationElement element, string value )
        {
            IntPtr _hwnd = IntPtr.Zero;

            // Validate arguments / initial setup
            if( value == null )
                throw new ArgumentNullException("string parameter 'value' must not be null");

            if( element == null )
                throw new ArgumentNullException("AutomationElement parameter 'element' must not be null");

            // Get hwnd
            _hwnd = GetWindowHandleFromAutomationElement( element );

            // Sanity check #1: Is window enabled???
            if (!SafeNativeMethods.IsWindowEnabled(_hwnd))
            {
                throw new ElementNotEnabledException();
            }

            // Sanity check #2: Are there styles that prohibit us sending text to this control?
            NativeMethods.HWND hwnd = NativeMethods.HWND.Cast(_hwnd);
            int WindowStyle = SafeNativeMethods.GetWindowLong(hwnd, SafeNativeMethods.GWL_STYLE);

            if (IsBitSet(WindowStyle, NativeMethods.ES_READONLY))
            {
                throw new InvalidOperationException("Cannot set text to a read-only field");
            }

            // Sanity check #3: Is the size of the text we want to set greater than what the control accepts?
            IntPtr result;
            int resultInt;

            IntPtr resultSendMessage = UnsafeNativeMethods.SendMessageTimeout(hwnd, NativeMethods.EM_GETLIMITTEXT, IntPtr.Zero, IntPtr.Zero, 1, 10000, out result);
            int lastWin32Error = Marshal.GetLastWin32Error();

            if (resultSendMessage == IntPtr.Zero)
            {
                throw new InvalidOperationException("SendMessageTimeout() timed out");
            }
            resultInt = unchecked((int)(long)result);

            // A result of -1 means that no limit is set.
            if (resultInt != -1 && resultInt < value.Length)
            {
                throw new InvalidOperationException("Length of text (" + value.Length + ") is greater than upper limit of control (" + resultInt.ToString() + ")");
            }

            // Send the message...!
            result = UnsafeNativeMethods.SendMessageTimeout(hwnd, NativeMethods.WM_SETTEXT, IntPtr.Zero, new StringBuilder(value), 1, 10000, out result);
            resultInt = unchecked((int)(long)result);
            if (resultInt != 1)
            {
                throw new InvalidOperationException("Unable to set the text of the control, text = " + value );
            }
        }

        /// <summary>
        /// use WndProc to handle all communications between WPF controls and keyboard,
        
        /// </summary>
        /// <param name="hwnd">window handle</param>
        /// <param name="msg">msg to communicate</param>
        /// <param name="wParam">addtional meg-specific info</param>
        /// <param name="lParam">addtional meg-specific info</param>
        /// <param name="handled">result</param>
        /// <returns></returns>
        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // handle any Win32 message
            if (msg == NativeMethods.EM_GETLIMITTEXT)
            {
                //...
                handled = true;
            }
            return IntPtr.Zero;
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

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        // Stateless object, has no private fields

        #endregion Private Fields
    }
}
