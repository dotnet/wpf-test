// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Low-level keyboard and mouse emulation internal support for the library.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 11 $ $Source: //depot/vbl_wcp_avalon/windowstest/client/wcptests/uis/Common/Library/Utils/InputGenerator.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Globalization;
    using System.Collections;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Runtime.InteropServices;

    using Point = System.Drawing.Point;
    using Win32 = Test.Uis.Wrappers.Win32;
    using Test.Uis.Loggers;

    #endregion Namespaces.

    /// <summary>
    /// Wraps the OS API used to emulate user input.
    /// </summary>
    sealed class InputGenerator
    {
        #region DllImports.

        [DllImport("user32.dll", EntryPoint="SendInput")]
        private static extern uint SendMouseInput(uint nInputs, MouseInput [] pInputs, int cbSize);

        /// <summary>
        ///
        /// </summary>
        /// <param name="nInputs">Specifies the number of structures in the pInputs array.</param>
        /// <param name="pInputs">Pointer to an array of INPUT structures. Each structure represents an event to be inserted into the keyboard or mouse input stream.</param>
        /// <param name="cbSize">Specifies the size, in bytes, of an INPUT structure.</param>
        /// <returns></returns>
        ///
        [DllImport ("user32.dll", EntryPoint = "SendInput")]
        private static extern uint SendKeyboardInput (uint nInputs, KeyboardInput [] pInputs, int cbSize);

        [DllImport ("user32.dll", CharSet = CharSet.Auto)]
        private static extern int ToUnicodeEx (uint virtualKey, uint scanCode, byte[] keyState, char[] Buff, int cch, uint flags, IntPtr hkl);

        #endregion DllImports.

        #region Input structures.

        // Some marshalling notes:
        // - arrays are a-ok
        // - DWORD is UInt32
        // - UINT is UInt32
        // - CHAR is char is Char with ANSI decoration
        // - LONG is Int32
        // - WORD is UInt16

        [StructLayout(LayoutKind.Sequential)]
        internal struct MouseInput
        {
            public IntPtr type;

            public int dx;              // 32
            public int dy;              // 32 - 64
            public int mouseData;       // 32 - 96
            public uint dwFlags;        // 32 - 128
            public IntPtr time;         // 32 - 160
            public IntPtr dwExtraInfo;  // 32 - 192
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KeyboardInput
        {
            public IntPtr type;

            /// <summary>
            /// Specifies a virtual-key code.
            /// </summary>
            public ushort wVk;          // 16
            /// <summary>
            /// Specifies a hardware scan code for the key.
            /// </summary>
            public ushort wScan;        // 16 - 32
            /// <summary>
            /// Specifies various aspects of a keystroke.
            /// </summary>
            public uint dwFlags;        // 32 - 64
            /// <summary>
            /// Time stamp for the event, in milliseconds.
            /// </summary>
            public IntPtr time;           // 32 - 96
            /// <summary>
            /// Specifies an additional value associated with the keystroke.
            /// </summary>
            public IntPtr dwExtraInfo;  // 32 - 128

            // we just need to make it the same size as the mouse input
            public int unused;           // 32 - 160
            public int unused2;

            /// <summary>Copy constructor.</summary>
            /// <param name='keyboardInput'>Keyboard input to copy.</param>
            public KeyboardInput(KeyboardInput keyboardInput)
            {
                type = new IntPtr(INPUT_KEYBOARD);
                this.wVk = keyboardInput.wVk;
                this.wScan = keyboardInput.wScan;
                this.dwFlags = keyboardInput.dwFlags;
                this.time = keyboardInput.time;
                this.dwExtraInfo = keyboardInput.dwExtraInfo;
                this.unused = 0;
                this.unused2 = 0;
            }
        }

        #endregion Input structures.

        #region Input type flags.

        /// <summary>
        /// Specifies that the event is a mouse event.
        /// </summary>
        internal const uint INPUT_MOUSE = 0;
        /// <summary>
        /// Specifies that the event is a keyboard event.
        /// </summary>
        internal const uint INPUT_KEYBOARD = 1;
        /// <summary>
        /// Specifies that the event is from input hardware other than a
        /// keyboard or mouse.
        /// </summary>
        internal const uint INPUT_HARDWARE = 2;

        #endregion Input type flags.

        #region Keyboard flags.

        /// <summary>
        /// If specified, the scan code was preceded by a prefix
        /// byte that has the value 0xE0 (224).
        /// </summary>
        internal const uint KEYEVENTF_EXTENDEDKEY = 1;
        /// <summary>
        /// If specified, the key is being released. If not specified, the
        /// key is being pressed.
        /// </summary>
        internal const uint KEYEVENTF_KEYUP = 2;
        /// <summary>
        /// Windows 2000/XP: If specified, the system synthesizes a
        /// VK_PACKET keystroke. The wVk parameter must be zero.
        /// </summary>
        internal const uint KEYEVENTF_UNICODE = 4;
        /// <summary>
        /// If specified, wScan identifies the key and wVk is ignored.
        /// </summary>
        internal const uint KEYEVENTF_SCANCODE = 8;

        #endregion Keyboard flags.

        #region Mouse flags.

        /// <summary>
        /// Mouse move
        /// </summary>
        private const uint MOUSEEVENTF_MOVE        = 0x0001;
        /// <summary>
        /// left button down.
        /// </summary>
        private const uint MOUSEEVENTF_LEFTDOWN    = 0x0002;
        /// <summary>
        /// left button up.
        /// </summary>
        private const uint MOUSEEVENTF_LEFTUP      = 0x0004;
        /// <summary>
        /// right button down.
        /// </summary>
        private const uint MOUSEEVENTF_RIGHTDOWN   = 0x0008;
        /// <summary>
        /// right button up.
        /// </summary>
        private const uint MOUSEEVENTF_RIGHTUP     = 0x0010;
        /// <summary>
        /// middle button down.
        /// </summary>
        private const uint MOUSEEVENTF_MIDDLEDOWN  = 0x0020;
        /// <summary>
        /// middle button up.
        /// </summary>
        private const uint MOUSEEVENTF_MIDDLEUP    = 0x0040;
        /// <summary>
        /// x button down.
        /// </summary>
        private const uint MOUSEEVENTF_XDOWN       = 0x0080;
        /// <summary>
        /// x button down.
        /// </summary>
        private const uint MOUSEEVENTF_XUP         = 0x0100;
        /// <summary>
        /// wheel button rolled.
        /// </summary>
        private const uint MOUSEEVENTF_WHEEL       = 0x0800;
        /// <summary>
        /// do not coalesce mouse moves.
        /// </summary>
        private const uint MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000;
        /// <summary>
        /// map to entire virtual desktop.
        /// </summary>
        private const uint MOUSEEVENTF_VIRTUALDESK = 0x4000;
        /// <summary>
        /// absolute move.
        /// </summary>
        private const uint MOUSEEVENTF_ABSOLUTE    = 0x8000;

        #endregion Mouse flags.


        #region Internal methods.

        internal static void PressVirtualKey(ushort virtualKey)
        {
            SendKey (virtualKey, true);
        }

        internal static void ReleaseVirtualKey(ushort virtualKey)
        {
            SendKey (virtualKey, false);
        }

        /// <summary>
        /// This sends one virtual key.
        /// No conversion of character based on locale done
        /// </summary>
        /// <param name="vKey">virutal key to be sent</param>
        internal static void SendKeyCode (ushort vKey)
        {
            SendKey (vKey, true);
            SendKey (vKey, false);
        }

        /// <summary>
        /// This method allows sending like Ctrl-A
        /// SendKeyCode(KeyCode.VkShift, KeyCode.VkA);
        /// </summary>
        /// <param name="vKey1">first virtual key</param>
        /// <param name="vKey2">second virtual key</param>
        internal static void SendKeyCode (ushort vKey1, ushort vKey2)
        {
            SendKey (vKey1, true);
            SendKey (vKey2, true);
            SendKey (vKey2, false);
            SendKey (vKey1, false);
        }

        /// <summary>
        /// This method allows sending like Alt-Shift-A
        /// SendKeyCode(KeyCode.VkMenu, KeyCode.VkShift, KeyCode.VkA);
        /// </summary>
        /// <param name="vKey1">first virtual key</param>
        /// <param name="vKey2">second virtual key</param>
        /// <param name="vKey3">third virtual key</param>
        internal static void SendKeyCode (ushort vKey1, ushort vKey2, ushort vKey3)
        {
            SendKey(vKey1, true);
            SendKey(vKey2, true);
            SendKey(vKey3, true);
            SendKey(vKey3, false);
            SendKey(vKey2, false);
            SendKey(vKey1, false);
        }

        /// <summary>Parses the string and sends keys.</summary>
        /// <param name="text">String to be sent</param>
        /// <param name="inputLocale">Input locale identifier to turn text into keys.</param>
        /// <remarks>SendKeyboardInput is used to send keystrokes.</remarks>
        internal static void SendString(string text, string inputLocale)
        {
            InputGenerator.KeyboardInput[] keyboardInputs;  // Input items to send.
            int numberOfKeyUps;                             // Number of key ups in keyboardInputs.
            IntPtr hkl;

            hkl = Win32.SafeLoadKeyboardLayout(inputLocale, 0);
            if (hkl == IntPtr.Zero)
            {
                throw new Exception("Unable to load input locale " + inputLocale);
            }

            LogSendString(text, hkl);

            keyboardInputs = SendKeysParser.ConvertLiteralStringToKeyCodeStructures(text, hkl);

            // Count number of key ups we are looking for.
            numberOfKeyUps = 0;
            for (int i = 0; i < keyboardInputs.Length; i++)
            {
                if ((keyboardInputs[i].dwFlags & KEYEVENTF_KEYUP) != 0)
                {
                    numberOfKeyUps++;
                }
            }

            //
            // the following creates a KeyboardPostProcessInputMonitor (monitoring key up) and gets the
            // thing started
            //
            if (numberOfKeyUps > 0 && InputMonitorManager.Current.IsEnabled)
            {
                InputMonitorManager.Current.AddInputMonitor(
                    new KeyboardPostProcessInputMonitor(numberOfKeyUps));
            }

            unsafe
            {
                if (keyboardInputs.Length > 0)
                {
                    SendKeyboardInput((uint)keyboardInputs.Length, keyboardInputs, sizeof(KeyboardInput));
                }
            }
        }

        /// <summary>
        /// Hold / release one key.
        /// This method takes keystrokeDescription string (e.g. "A" or "{BACK}")
        /// and this string can only hold *one* key. If the keystrokeDescription
        /// contain more than 1 key, it throws exception. Also, this method doesn't
        /// do key combination (e.g. +{BACK})
        /// </summary>
        /// <param name="keystrokeDescription">keystroke description string</param>
        /// <param name="hkl">locale identifier to specify locale to interpret keystrokeDescription</param>
        /// <param name="pressed">hole key = true, false otherwise</param>
        internal static void PressOrReleaseOneKey(string keystrokeDescription, IntPtr hkl, bool pressed)
        {
            InputGenerator.KeyboardInput [] input = SendKeysParser.ConvertLiteralStringToKeyCodeStructures(keystrokeDescription, hkl);

            if (input.Length < 1)
            {
                throw new InvalidOperationException("Nothing is returned from InputGenerator.ConvertLiteralStringToKeyCodeStructures");
            }

            //
            // if we have more than 2 elements in the array, that means the keystrokeDescription
            // is likely to constitute more than 2 keystrokes, in which case we will throw
            // exception
            //

            if (input.Length > 2)
            {
                throw new InvalidOperationException("More than 1 key is specified in keystrokeDescription to PressOneKey call");
            }

            SendKey(input[0].wVk, pressed);

        }

        /// <summary>
        /// Send a single keystroke
        /// </summary>
        /// <param name="wVk">VK to Send</param>
        /// <param name="pressed">Is the key pressed?</param>
        internal static void SendKey (ushort wVk, bool pressed)
        {
            InputGenerator.KeyboardInput [] input = new InputGenerator.KeyboardInput[1];

            input[0] = SendKeysParser.CreateSingleKeyboardInputFromVk (wVk, pressed);

            KeyboardPostProcessInputMonitor keyboardPostProcessInputMonitor;

            if (InputMonitorManager.Current.IsEnabled)
            {
                if (pressed)
                {
                    //
                    // if it is a press, we look for a key down event
                    //
                    keyboardPostProcessInputMonitor = new KeyboardPostProcessInputMonitor(1, Microsoft.Test.Input.RawKeyboardActions.KeyDown);
                }
                else
                {
                    //
                    // otherwise, it is a keyup (default parameter is keyup)
                    //
                    keyboardPostProcessInputMonitor = new KeyboardPostProcessInputMonitor();
                }

                InputMonitorManager.Current.AddInputMonitor(keyboardPostProcessInputMonitor);
            }
            unsafe
            {
                SendKeyboardInput ((uint)input.Length, input, sizeof(KeyboardInput));
            }
        }



        /// <summary>
        /// Convert a vitual key based on the supplied hkl
        /// e.g. to convert VkA to English a you can call
        /// ConvertVkToCharinHKL(VKeys.VkA, 0x04090409);
        /// </summary>
        /// <param name="vKey"></param>
        /// <param name="hkl"></param>
        /// <returns></returns>
        internal static char ConvertVkToCharinHKL (ushort vKey, IntPtr hkl)
        {
            new System.Security.Permissions.SecurityPermission (System.Security.Permissions.PermissionState.Unrestricted).Assert ();

            uint scanCode = Win32.MapVirtualKeyEx ((uint)(vKey & 0x00ff), 0, hkl);
            char[] charBuffer = new char[2];
            byte[] byteKeyboardState = new byte[256];

            if (!Win32.GetKeyboardState (byteKeyboardState))
            {
                return '\0';
            }

            // We need to take care of the shift-state here
            // if 0x0100 of vKey is set, this is a shift-state
            // and we need to set the keyboardstate accordingly
            if ((vKey & 0x0100) == 0x0100)
            {
                byteKeyboardState[Win32.VK_SHIFT] |= 0x80;
            }

            return ToUnicodeEx ((uint)vKey, scanCode, byteKeyboardState, charBuffer, 1, 0, hkl) > 0 ? charBuffer[0] : '\0';
        }

        /// <summary>
        /// Convert the supplied virtual key to a character based on current thread
        /// input locale.
        /// </summary>
        /// <param name="vkey"></param>
        /// <returns></returns>
        internal static char ConvertVkToCharinCurrentHKL (ushort vkey)
        {
            return ConvertVkToCharinHKL (vkey, Win32.GetKeyboardLayout (IntPtr.Zero));
        }

        /// <summary>
        /// This method converts the "keystroke string" into string in language of
        /// hkl specified in the parameter list. For example:
        /// English "abc" can be converted into Russian "abc"
        /// </summary>
        /// <param name="str"></param>
        /// <param name="hkl"></param>
        /// <returns></returns>
        internal static string ConvertKeystrokeStringToLocaleSpecificString (string str, IntPtr hkl)
        {
            new System.Security.Permissions.SecurityPermission (System.Security.Permissions.PermissionState.Unrestricted).Assert ();

            string outputString = String.Empty;

            bool isInSpecialRange = false;

            for (int i = 0; i < str.Length; i++)
            {
                // If the keystoke string char is not special char
                // and it is not the keystroke identifier
                if (str[i] != '^' && str[i] != '%' && str[i] != '+' && str[i] != '{' && str[i] != '}' && !isInSpecialRange)
                {
                    // Pass the virtual key with state. ConvertVkToCharinCurrentHKL
                    // needs to know the state so that shift characters can be
                    // converted correctly.
                    ushort vKey = Win32.VkKeyScanEx (str[i], hkl);

                    outputString += ConvertVkToCharinCurrentHKL (vKey);
                }
                else if(str[i] == '{')
                {
                    outputString += str[i];
                    isInSpecialRange = true;
                }
                else if (str[i] == '}')
                {
                    outputString += str[i];
                    isInSpecialRange = false;
                }
                else
                {
                    outputString += str[i];
                }
            }

            return outputString;
        }

        /// <summary>
        /// This is basically the same as ConvertKeystrokeStringToLocaleSpecificString(string, IntPtr)
        /// except that it defaults to use US English hkl
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        internal static string ConvertKeystrokeStringToLocaleSpecificString (string str)
        {
            return ConvertKeystrokeStringToLocaleSpecificString (str, (IntPtr)0x04090409);
        }

        /// <summary>
        /// This helper is required to generate all input in one run.
        /// </summary>
        /// <returns>Steps taken for drag.</returns>
        internal static int GenerateMouseDrag(int x, int y, int xDest, int yDest)
        {
            int xSteps = Math.Abs(xDest - x);
            int ySteps = Math.Abs(yDest - y);
            int stepCount = 1 + ((xSteps > ySteps)? xSteps : ySteps);

            Point[] steps = new Point[stepCount];
            steps[0] = new Point(x, y);

            int xDelta = (x > xDest)? -1 : 1;
            int yDelta = (y > yDest)? -1 : 1;
            int stepIndex = 1;
            while (x != xDest || y != yDest)
            {
                if (x != xDest) x += xDelta;
                if (y != yDest) y += yDelta;
                steps[stepIndex] = new Point(x, y);
                stepIndex++;

                // Perform a send and a sleep for each mouse movement.
                SendMouseMoveAbsolute(x, y);
                System.Threading.Thread.Sleep(10);
            }
            return steps.Length;
        }

        internal static void SendMouseMoveAbsolute(int x, int y)
        {
            InternalSendMouseMove(true, new Point[] { new Point(x, y) });
        }

        internal static void SendMouseMoveRelative(int x, int y)
        {
            InternalSendMouseMove(false, new Point[] { new Point(x, y) });
        }

        internal static void SendMouseMoveAbsolute(Point[] points)
        {
            InternalSendMouseMove(true, points);
        }

        internal static void SendMouseMoveRelative(Point[] points)
        {
            InternalSendMouseMove(false, points);
        }

        internal static void SendMouseMoveWheel(int clicks)
        {
            const int WHEEL_DELTA = 120; /* as per SDK */
            MouseInput [] input = new MouseInput[1];

            input[0] = new MouseInput();

            input[0].type = new IntPtr(INPUT_MOUSE);
            input[0].mouseData = clicks * WHEEL_DELTA;
            input[0].dwFlags = MOUSEEVENTF_WHEEL;

            unsafe
            {
                SendMouseInput(1, input, sizeof(MouseInput));
            }

            // MouseWheel input seems to be getting coalesced by the OS, similar to mouse-move.
            // There isn't a NOCOALESCE flag to turn this off, so instead just sleep for
            // a short time, hopefully enough to avoid the coalescing.
            System.Threading.Thread.Sleep(50);
        }

        internal static void SendMouseButton(bool left, bool middle, bool right, bool press)
        {
            MouseInput [] input = new MouseInput[1];
            Microsoft.Test.Input.RawMouseActions mouseActions = Microsoft.Test.Input.RawMouseActions.None;

            input[0] = new MouseInput();

            input[0].type = new IntPtr(INPUT_MOUSE);
            uint flags = 0;

            if (left && press)
            {
                flags |= MOUSEEVENTF_LEFTDOWN;
                mouseActions |= Microsoft.Test.Input.RawMouseActions.Button1Press;
            }
            else if (left && !press)
            {
                flags |= MOUSEEVENTF_LEFTUP;
                mouseActions |= Microsoft.Test.Input.RawMouseActions.Button1Release;
            }

            if (right && press)
            {
                flags |= MOUSEEVENTF_RIGHTDOWN;
                mouseActions |= Microsoft.Test.Input.RawMouseActions.Button2Press;
            }
            else if (right && !press)
            {
                flags |= MOUSEEVENTF_RIGHTUP;
                mouseActions |= Microsoft.Test.Input.RawMouseActions.Button2Release;
            }

            if (middle && press)
            {
                flags |= MOUSEEVENTF_MIDDLEDOWN;
                mouseActions |= Microsoft.Test.Input.RawMouseActions.Button3Press;
            }
            else if (middle && !press)
            {
                flags |= MOUSEEVENTF_MIDDLEUP;
                mouseActions |= Microsoft.Test.Input.RawMouseActions.Button3Release;
            }

            input[0].dwFlags = flags;

            if (InputMonitorManager.Current.IsEnabled)
            {
                InputMonitorManager.Current.AddInputMonitor(new MousePostProcessInputMonitor(1, mouseActions));
            }

            unsafe
            {
                SendMouseInput(1, input, sizeof(MouseInput));
            }
        }

        #endregion Internal methods.

        #region Private methods.

        private static void InternalSendMouseMove(bool absolute,
            Point[] points)
        {
            int count = points.Length;
            uint flags = absolute?
                MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE : MOUSEEVENTF_MOVE;
            flags |= MOUSEEVENTF_MOVE_NOCOALESCE;
            MouseInput[] input = new MouseInput[count];
            for (int i = 0; i < count; i++)
            {
                input[i].type = new IntPtr(INPUT_MOUSE);
                input[i].dx = points[i].X;
                input[i].dy = points[i].Y;
                input[i].dwFlags = flags;
            }

            // Absolute pixels must be specified in a screen of 65,535 by
            // 65,535 regardless of real size. Add half a pixel to account
            // for rounding problems.
            if (absolute)
            {
                int screenWidth = Win32.GetSystemMetrics(Win32.SM_CXSCREEN);
                int screenHeight = Win32.GetSystemMetrics(Win32.SM_CYSCREEN);
                double ratioX = (double)65536 / (double)screenWidth;
                double ratioY = (double)65536 / (double)screenHeight;
                double halfX = (double)65536 / ((double)screenWidth * 2);
                double halfY = (double)65536 / ((double)screenHeight * 2);
                for (int i = 0; i < count; i++)
                {
                    input[i].dx = (int) (input[i].dx * ratioX + halfX);
                    input[i].dy = (int) (input[i].dy * ratioY + halfY);
                }
            }

            //
            // Well, I don't know what to do with mouse move.
            //
            unsafe
            {
                SendMouseInput((uint) count, input, sizeof(MouseInput));
            }
        }

        private static void LogSendString(string text, IntPtr hkl)
        {
            string logString;

            logString = String.Format("Parsing keystroke string [{0}] with hkl [{1}]",
                text, ((int)hkl).ToString("x8", System.Globalization.CultureInfo.InvariantCulture));
            Logger.Current.Log(logString);
        }

        #endregion Private methods.
    }

    /// <summary>
    /// Parses a string in SendKeys.Send format into an array of KeyboardInput
    /// structures.
    /// </summary>
    internal sealed class SendKeysParser
    {
        #region Internal methods.

        /// <summary>
        /// Whether a virtual key is an extended key.
        /// </summary>
        /// <param name="virtualKey"></param>
        /// <returns></returns>
        /// <remarks>
        /// An extended key is one that comes from an "enhanced keyboard".
        /// Some additional keys have not yet been implemented (windows,
        /// left/rigth ctrl, others).
        /// </remarks>
        internal static bool IsVirtualKeyExtended(ushort virtualKey)
        {
            return virtualKey == Win32.VK_BACK ||
            virtualKey == Win32.VK_INSERT || virtualKey == Win32.VK_DELETE ||
            virtualKey == Win32.VK_HOME || virtualKey == Win32.VK_END ||
            virtualKey == Win32.VK_PRIOR ||virtualKey == Win32.VK_NEXT ||
            virtualKey == Win32.VK_UP || virtualKey == Win32.VK_DOWN ||
            virtualKey == Win32.VK_LEFT || virtualKey == Win32.VK_RIGHT;
        }

        #endregion Internal methods.

        #region Inner types.

        /// <summary>A generc list of keyboard inputs.</summary>
        class KeyboardInputList: System.Collections.Generic.List<InputGenerator.KeyboardInput> { }

        #endregion Inner types.

        #region Escape action code support.

        /// <summary>Encapsulates an escape action code.</summary>
        struct EscapeCode
        {
            public readonly string Name;
            public readonly ushort VirtualKey;

            public EscapeCode(string name, ushort virtualKey)
            {
                this.Name = name;
                this.VirtualKey = virtualKey;
            }

            public bool IsExtended
            {
                get { return IsVirtualKeyExtended(VirtualKey); }
            }
        }

        /// <summary>Array of known escape action codes.</summary>
        private static readonly EscapeCode[] s_escapeCodes = new EscapeCode[] {
            new EscapeCode ("BACK", Win32.VK_BACK),
            new EscapeCode ("BACKSPACE", Win32.VK_BACK),
            new EscapeCode ("BKSP", Win32.VK_BACK),
            new EscapeCode ("BS", Win32.VK_BACK),
            new EscapeCode ("DEL", Win32.VK_DELETE),
            new EscapeCode ("DELETE", Win32.VK_DELETE),
            new EscapeCode ("ENTER", Win32.VK_RETURN),
            new EscapeCode ("ESC", Win32.VK_ESCAPE),
            new EscapeCode ("HELP", Win32.VK_HELP),
            new EscapeCode ("INS", Win32.VK_INSERT),
            new EscapeCode ("INSERT", Win32.VK_INSERT),
            new EscapeCode ("TAB", Win32.VK_TAB),
            new EscapeCode ("CAPSLOCK", Win32.VK_CAPITAL),
            new EscapeCode ("NUMLOCK", Win32.VK_NUMLOCK),
            new EscapeCode ("PRTSC", Win32.VK_PRINT),
            new EscapeCode ("SCROLLOCK", Win32.VK_SCROLL),
            new EscapeCode ("DOWN", Win32.VK_DOWN),
            new EscapeCode ("END", Win32.VK_END),
            new EscapeCode ("HOME", Win32.VK_HOME),
            new EscapeCode ("LEFT", Win32.VK_LEFT),
            new EscapeCode ("PGDN", Win32.VK_NEXT),
            new EscapeCode ("PGUP", Win32.VK_PRIOR),
            new EscapeCode ("RIGHT", Win32.VK_RIGHT),
            new EscapeCode ("UP", Win32.VK_UP),
            new EscapeCode ("F1", Win32.VK_F1),
            new EscapeCode ("F2", Win32.VK_F2),
            new EscapeCode ("F3", Win32.VK_F3),
            new EscapeCode ("F4", Win32.VK_F4),
            new EscapeCode ("F5", Win32.VK_F5),
            new EscapeCode ("F6", Win32.VK_F6),
            new EscapeCode ("F7", Win32.VK_F7),
            new EscapeCode ("F8", Win32.VK_F8),
            new EscapeCode ("F9", Win32.VK_F9),
            new EscapeCode ("F10", Win32.VK_F10),
            new EscapeCode ("F11", Win32.VK_F11),
            new EscapeCode ("F12", Win32.VK_F12),
            new EscapeCode ("F13", Win32.VK_F13),
            new EscapeCode ("F14", Win32.VK_F14),
            new EscapeCode ("F15", Win32.VK_F15),
            new EscapeCode ("F16", Win32.VK_F16),
            new EscapeCode ("ADD", Win32.VK_ADD),
            new EscapeCode ("SUBTRACT", Win32.VK_SUBTRACT),
            new EscapeCode ("MULTIPLY", Win32.VK_MULTIPLY),
            new EscapeCode ("DIVIDE", Win32.VK_DIVIDE),
            new EscapeCode ("SPACE", Win32.VK_SPACE),
            new EscapeCode ("ALT", Win32.VK_LMENU),
            new EscapeCode ("CTRL", Win32.VK_LCONTROL),
            new EscapeCode ("SHIFT", Win32.VK_LSHIFT)
        };

        #endregion Escape action code support.

        /// <summary>
        /// This function create a list of KeyCodeWithState structures
        /// by parsing the literal key string
        /// </summary>
        /// <param name="literalString">the raw keystroke string</param>
        /// <param name="hkl">Keyboard layout handle.</param>
        /// <returns>ArrayList of KeyCodeWithStates</returns>
        public static InputGenerator.KeyboardInput[] ConvertLiteralStringToKeyCodeStructures(
            string literalString, IntPtr hkl)
        {
            bool isShiftPressed = false;
            bool isAltPressed = false;
            bool isCtrlPressed = false;
            char lastCharacter;
            KeyboardInputList result = new KeyboardInputList();

            // Check the literalString argument.
            if (literalString == null)
            {
                throw new ArgumentNullException("literalString");
            }
            if (literalString.Length == 0)
            {
                return new InputGenerator.KeyboardInput[0];
            }
            lastCharacter = literalString[literalString.Length - 1];
            if (lastCharacter == '+' || lastCharacter == '^' ||
                lastCharacter == '%' || lastCharacter == '{')
            {
                throw new KeystrokeStringParsingException(literalString,
                    "Special character is not allowed at the end: " + lastCharacter);
            }

            // i is the index to the Keystroke string
            // j is also the index to the keystroke string, but this is used only when we process {} key sequence
            // k is the number of iterations for special key injections, used only with {} key
            for (int i = 0; i < literalString.Length; i++)
            {
                switch (literalString[i])
                {
                    // shift key
                    case '+':
                        HoldSpecialKey(result, ref isShiftPressed, Win32.VK_LSHIFT);
                        break;

                    // alt key
                    case '%':
                        HoldSpecialKey(result, ref isAltPressed, Win32.VK_LMENU);
                        break;

                    // control key
                    case '^':
                        HoldSpecialKey(result, ref isCtrlPressed, Win32.VK_LCONTROL);
                        break;

                    // start of {} key sequence
                    case '{':
                        bool identifierFound = false;

                        // intialize j for {} scan
                        int j = i;

                        for (; j < literalString.Length; j++)
                        {
                            // if we are able to find the closing blacket, we process that chunk of text
                            // otherwise, we bail out
                            if (literalString[j] == '}')
                            {
                                identifierFound = true;

                                // this is to extract the text inside the blacket
                                // e.g. key sequence +{BACK 4} will have "BACK 4" in specialKeyString
                                string specialKeyString = literalString.Substring (i + 1, j - i - 1);

                                // We need to process the "4" in {BACK 4}
                                string[] splitString = specialKeyString.Split (new char[] { ' ' });

                                // we don't expect it to contain more than 1 space
                                if (splitString.Length > 2)
                                {
                                    throw new KeystrokeStringParsingException (literalString,
                                        specialKeyString + " can't be parsed");
                                }

                                int iterations = 1;

                                if (splitString.Length == 2 && !String.IsNullOrEmpty (splitString[1]))
                                {
                                    if (!Int32.TryParse(splitString[1], out iterations))
                                    {
                                        throw new KeystrokeStringParsingException(literalString,
                                            specialKeyString + " can't be parsed - cannot convert " +
                                            splitString[1] + " to an Int32.");
                                    }
                                }

                                // Iterate through each of the escape code and find the target one
                                string upperCaseCode = splitString[0].ToUpper(CultureInfo.InvariantCulture);
                                foreach (EscapeCode ec in s_escapeCodes)
                                {
                                    if (ec.Name == upperCaseCode)
                                    {
                                        for (int k = 0; k < iterations; k++)
                                        {
                                            AddKeys(result, isShiftPressed, isAltPressed, isCtrlPressed,
                                                (ushort)(ec.VirtualKey & 0x00FF));
                                        }

                                        // time to reset the combination key
                                        // consider +{BACK}
                                        SpecialStateKeyCleanup(result,
                                            ref isShiftPressed, ref isAltPressed, ref isCtrlPressed);

                                        break;
                                    }
                                }

                                if (identifierFound)
                                {
                                    break;
                                }
                            }
                        }

                        // We have open bracket, but we can't find close bracket
                        // throw an exception so that the test case writer can fix
                        // the test string
                        if (!identifierFound)
                        {
                            throw new KeystrokeStringParsingException(literalString,
                                "Invalid string - open bracket without corresponding close bracket found.");
                        }

                        // identifier string need not be treated as ordinary
                        // string, resetting index and move onward
                        i = j;

                        break;

                    default:
                        // Grab the scan code of the character
                        ushort vKeyWithState = Win32.VkKeyScanEx(literalString[i], hkl);

                        AddKeys(result, isShiftPressed, isAltPressed, isCtrlPressed, vKeyWithState);

                        // We need to reset the special key state when any ordinary key
                        // is detected
                        SpecialStateKeyCleanup(result, ref isShiftPressed, ref isAltPressed, ref isCtrlPressed);
                        break;
                }
            }

            SpecialStateKeyCleanup(result, ref isShiftPressed, ref isAltPressed, ref isCtrlPressed);

            return result.ToArray();
        }

        private static void HoldSpecialKey(KeyboardInputList result, ref bool isKeyDown, byte wVk)
        {
            isKeyDown = true;
            result.Add(CreateSingleKeyboardInputFromVk (wVk, true));
        }

        private static void SpecialStateKeyCleanup(KeyboardInputList result,
            ref bool isShiftPressed, ref bool isAltPressed, ref bool isCtrlPressed)
        {
            if (isShiftPressed)
            {
                isShiftPressed = false;
                result.Add (CreateSingleKeyboardInputFromVk (Win32.VK_LSHIFT, false));
            }
            if (isAltPressed)
            {
                isAltPressed = false;
                result.Add (CreateSingleKeyboardInputFromVk (Win32.VK_LMENU, false));
            }
            if (isCtrlPressed)
            {
                isCtrlPressed = false;
                result.Add (CreateSingleKeyboardInputFromVk (Win32.VK_LCONTROL, false));
            }
        }

        private static void AddKeys(KeyboardInputList result,
            bool isShiftPressed, bool isAltPressed, bool isCtrlPressed, ushort vKeyWithState)
        {
            // This is tricky here.
            // We don't need the character in shift state when it is combined
            // with other special keys
            if ((vKeyWithState & 0x0100) == 0x0100 && !isShiftPressed && !isAltPressed && !isCtrlPressed)
            {
                result.Add (CreateSingleKeyboardInputFromVk (Win32.VK_LSHIFT, true));
                result.Add (CreateSingleKeyboardInputFromVk ((byte)(vKeyWithState & 0x00FF), true));
                result.Add (CreateSingleKeyboardInputFromVk ((byte)(vKeyWithState & 0x00FF), false));
                result.Add (CreateSingleKeyboardInputFromVk (Win32.VK_LSHIFT, false));
            }
            else
            {
                result.Add (CreateSingleKeyboardInputFromVk ((byte)(vKeyWithState & 0x00FF), true));
                result.Add (CreateSingleKeyboardInputFromVk ((byte)(vKeyWithState & 0x00FF), false));
            }
        }

        public static InputGenerator.KeyboardInput CreateSingleKeyboardInputFromVk (ushort wVk, bool pressed)
        {
            InputGenerator.KeyboardInput input = new InputGenerator.KeyboardInput ();

            input.type = new IntPtr(InputGenerator.INPUT_KEYBOARD);
            input.wVk = wVk;
            input.wScan = (ushort)Win32.MapVirtualKeyEx ((uint)wVk, 0, Win32.GetKeyboardLayout (IntPtr.Zero));
            if (IsVirtualKeyExtended (wVk))
            {
                input.dwFlags |= InputGenerator.KEYEVENTF_EXTENDEDKEY;
            }

            if (!pressed)
            {
                input.dwFlags |= InputGenerator.KEYEVENTF_KEYUP;
            }

            input.time = IntPtr.Zero;
            input.dwExtraInfo = IntPtr.Zero;
            input.unused = 0;
            return input;
        }
    }

    /// <summary>
    /// This exception is thrown when SendKeysParser finds a parsing error
    /// in the keystroke string.
    /// </summary>
    class KeystrokeStringParsingException: System.Exception
    {
        #region Constructors.

        /// <summary>
        /// Creates a new KeystrokeStringParsingException instance.
        /// </summary>
        /// <param name="keystrokeString">Keystroke string to be passed</param>
        /// <param name="descriptionString">
        /// Description string, usually the contains the part which can't be parsed.
        /// </param>
        internal KeystrokeStringParsingException(string keystrokeString,
            string descriptionString)
            : base(descriptionString)
        {
            this._descriptionString = descriptionString;
            this._keyStrokeString = keystrokeString;
        }

        #endregion Constructors.

        #region Overrides.

        /// <summary>
        /// We override ToString for logging purposes.
        /// </summary>
        /// <returns>A string description of the exception.</returns>
        public override string ToString()
        {
            return String.Format(basicExceptionString,
                this._keyStrokeString, this._descriptionString);
        }

        #endregion Overrides.

        #region Private members

        private string _descriptionString;
        private string _keyStrokeString;

        private const string basicExceptionString = "KeystrokeString [{0}] can't be parsed: {1}";

        #endregion
    }
}