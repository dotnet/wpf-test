// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace DRT
{
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;
    using MS.Win32;

    /// <summary>Provides input emulation for DRT.</summary>
    class DrtInput
    {
        #region Input emulation support.

        #region Mouse support.

        [DllImport("user32.dll", EntryPoint="SendInput")]
        private static extern uint SendMouseInput(uint nInputs,
            MouseInput [] pInputs, int cbSize);

        // Some marshalling notes:
        // - arrays are a-ok
        // - DWORD is UInt32
        // - UINT is UInt32
        // - CHAR is char is Char with ANSI decoration
        // - LONG is Int32
        // - WORD is UInt16

        [StructLayout(LayoutKind.Sequential)]
        private struct MouseInput
        {
            public IntPtr type;

            public int dx;              // 32
            public int dy;              // 32 - 64
            public int mouseData;       // 32 - 96
            public uint dwFlags;        // 32 - 128
            public IntPtr time;         // 32 - 160
            public IntPtr dwExtraInfo;  // 32 - 192
        }


        /// <summary>GetSystemMetrics wrapper.</summary>
        /// <param name="nIndex">Index of metric to get.</param>
        /// <returns>System value.</returns>
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);

        private const uint INPUT_MOUSE = 0;
        private const uint INPUT_KEYBOARD = 1;

        private const uint MOUSEEVENTF_MOVE        = 0x0001;
        private const uint MOUSEEVENTF_LEFTDOWN    = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP      = 0x0004;
        private const uint MOUSEEVENTF_MOVE_NOCOALESCE = 0x2000;
        private const uint MOUSEEVENTF_ABSOLUTE    = 0x8000;

        /// <summary>Width of the screen of the primary display monitor, in pixels.</summary>
        public const int SM_CXSCREEN = 0;
        /// <summary>Height of the screen of the primary display monitor, in pixels.</summary>
        public const int SM_CYSCREEN = 1;

        public static void ClickScreenPoint(int x, int y)
        {
            MouseInput[] input = new MouseInput[3];
            input[0].type = new IntPtr(INPUT_MOUSE);
            // Hard-coded to a point inside the client area. Correct
            // thing to do is map from client area to screen points,
            // but it requires more P/Invoke.
            input[0].dx = x;
            input[0].dy = y;
            input[0].dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;

            // Absolute pixels must be specified in a screen of 65,535 by
            // 65,535 regardless of real size. Add half a pixel to account
            // for rounding problems.
            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);
            float ratioX = 65536 / screenWidth;
            float ratioY = 65536 / screenHeight;
            float halfX = 65536 / (screenWidth * 2);
            float halfY = 65536 / (screenHeight * 2);

            input[0].dx = (int) (input[0].dx * ratioX + halfX);
            input[0].dy = (int) (input[0].dy * ratioY + halfY);

            input[1].type = new IntPtr(INPUT_MOUSE);
            input[1].dwFlags = MOUSEEVENTF_LEFTDOWN;

            input[2].type = new IntPtr(INPUT_MOUSE);
            input[2].dwFlags = MOUSEEVENTF_LEFTUP;

            unsafe
            {
                SendMouseInput((uint)input.Length, input, sizeof(MouseInput));
            }
        }

        public static void MouseDown()
        {
            MouseInput[] input = new MouseInput[1];
            input[0].type = new IntPtr(INPUT_MOUSE);
            input[0].dwFlags = MOUSEEVENTF_LEFTDOWN;

            unsafe
            {
                SendMouseInput((uint)input.Length, input, sizeof(MouseInput));
            }
        }

        public static void MouseMove(int x, int y)
        {
            MouseInput[] input = new MouseInput[1];
            input[0].type = new IntPtr(INPUT_MOUSE);
            // Hard-coded to a point inside the client area. Correct
            // thing to do is map from client area to screen points,
            // but it requires more P/Invoke.
            input[0].dx = x;
            input[0].dy = y;
            input[0].dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE | MOUSEEVENTF_MOVE_NOCOALESCE;

            // Absolute pixels must be specified in a screen of 65,535 by
            // 65,535 regardless of real size. Add half a pixel to account
            // for rounding problems.
            int screenWidth = GetSystemMetrics(SM_CXSCREEN);
            int screenHeight = GetSystemMetrics(SM_CYSCREEN);
            double ratioX = (double)((double)65536 / (double)screenWidth);
            double ratioY = (double)((double)65536 / (double)screenHeight);
            double halfX = (double)((double)65536 / (double)(screenWidth * 2));
            double halfY = (double)((double)65536 / (double)(screenHeight * 2));
            input[0].dx = (int)(input[0].dx * ratioX + halfX);
            input[0].dy = (int)(input[0].dy * ratioY + halfY);

            unsafe
            {
                SendMouseInput((uint)input.Length, input, sizeof(MouseInput));
            }
        }

        public static void MouseUp()
        {
            MouseInput[] input = new MouseInput[1];
            input[0].type = new IntPtr(INPUT_MOUSE);
            input[0].dwFlags = MOUSEEVENTF_LEFTUP;
            
            unsafe
            {
                SendMouseInput((uint)input.Length, input, sizeof(MouseInput));
            }
        }

        #endregion Mouse support.

        #region Keyboard support.

        [DllImport ("user32.dll", EntryPoint = "SendInput")]
        private static extern uint SendKeyboardInput(uint nInputs,
            KeyboardInput [] pInputs, int cbSize);

        [DllImport ("user32.dll", CharSet = CharSet.Auto)]
        private static extern ushort VkKeyScan(char ch);

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

            public uint pad1;           // 32 - 160
            public uint pad2;           // 32 - 192

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
                this.pad1 = keyboardInput.pad1;
                this.pad2 = keyboardInput.pad2;
            }
        }

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

        /// <summary>Emulates typing on the keyboard.</summary>
        /// <param name='text'>Text to type.</param>
        /// <remarks>
        /// Case is not respected - everything goes in lowercase.
        /// To get uppercase characters, add a "+" in front of the
        /// character. The original design had the "+" toggle the
        /// shift state, but by resetting it we make text string
        /// compatible with CLR's SendKeys.Send.
        /// <para />
        /// Eg, to type "Hello, WORLD!", pass "+hello, +W+O+R+L+D+1"
        /// <para />
        /// This method has not been globalized to keep it simple.
        /// Non-US keyboard may break this functionality.
        /// </remarks>
        public static void KeyboardType(string text)
        {
            ArrayList list = new ArrayList();
            bool shiftIsPressed = false;
            bool controlIsPressed = false;
            const byte VK_SHIFT   = 0x10;
            const byte VK_CONTROL = 0x11;
            const byte VK_RETURN  = 0x0D;
            const byte VK_END = 0x23;
            const byte VK_HOME = 0x24;
            const byte VK_LEFT = 0x25;
            const byte VK_UP = 0x26;
            const byte VK_RIGHT = 0x27;
            const byte VK_DOWN = 0x28;

            int i = 0;
            while (i < text.Length)
            {
                char c = text[i];
                if (c == '+')
                {
                    KeyboardInput input = new KeyboardInput();
                    input.type = new IntPtr(INPUT_KEYBOARD);
                    input.wVk = VK_SHIFT;
                    if (shiftIsPressed)
                        input.dwFlags = KEYEVENTF_KEYUP;
                    list.Add(input);
                    shiftIsPressed = !shiftIsPressed;
                    i++;
                }
                else if (c == '^')
                {
                    KeyboardInput input = new KeyboardInput();
                    input.type = new IntPtr(INPUT_KEYBOARD);
                    input.wVk = VK_CONTROL;
                    if (controlIsPressed)
                        input.dwFlags = KEYEVENTF_KEYUP;
                    list.Add(input);
                    controlIsPressed = !controlIsPressed;
                    i++;
                }
                else if (c == '{')
                {
                    i++;
                    int closeIndex = text.IndexOf('}', i);
                    if (closeIndex == -1)
                    {
                        throw new ArgumentException(
                            "Malformed typing text: no closing '}' to match " +
                            "opening '{' at position " + i + ": " + text);
                    }
                    int length = closeIndex - i;
                    string escapeCode = text.Substring(i, length);
                    KeyboardInput input;
                    switch (escapeCode)
                    {
                        case "ENTER":
                            input = new KeyboardInput();
                            input.type = new IntPtr(INPUT_KEYBOARD);
                            input.wVk = VK_RETURN;
                            list.Add(input);

                            input = new KeyboardInput(input);
                            input.dwFlags |= KEYEVENTF_KEYUP;
                            list.Add(input);
                            break;
                        case "END":
                            input = new KeyboardInput();
                            input.type = new IntPtr(INPUT_KEYBOARD);
                            input.wVk = VK_END;
                            list.Add(input);
                           
                            input = new KeyboardInput(input);
                            input.dwFlags |= KEYEVENTF_KEYUP;
                            list.Add(input);

                            KeyboardInput reset = new KeyboardInput();
                            reset.type = new IntPtr(INPUT_KEYBOARD);
                            reset.wVk = VK_SHIFT;
                            reset.dwFlags = KEYEVENTF_KEYUP;
                            list.Add(reset);
                            shiftIsPressed = false;
                            break;
                        case "HOME":
                            input = new KeyboardInput();
                            input.type = new IntPtr(INPUT_KEYBOARD);
                            input.wVk = VK_HOME;
                            list.Add(input);

                            input = new KeyboardInput(input);
                            input.dwFlags |= KEYEVENTF_KEYUP;
                            list.Add(input);

                            reset = new KeyboardInput();
                            reset.type = new IntPtr(INPUT_KEYBOARD);
                            reset.wVk = VK_SHIFT;
                            reset.dwFlags = KEYEVENTF_KEYUP;
                            list.Add(reset);
                            shiftIsPressed = false;
                            break;
                        case "LEFT":
                            input = new KeyboardInput();
                            input.type = new IntPtr(INPUT_KEYBOARD);
                            input.dwFlags = KEYEVENTF_EXTENDEDKEY;
                            input.wVk = VK_LEFT;
                            list.Add(input);
                            
                            input = new KeyboardInput(input);
                            input.dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_EXTENDEDKEY;
                            list.Add(input);

                            reset = new KeyboardInput();
                            reset.type = new IntPtr(INPUT_KEYBOARD);
                            reset.wVk = VK_SHIFT;
                            reset.dwFlags = KEYEVENTF_KEYUP;
                            list.Add(reset);
                            shiftIsPressed = false;
                            break;
                        case "UP":
                            input = new KeyboardInput();
                            input.dwFlags = KEYEVENTF_EXTENDEDKEY;
                            input.type = new IntPtr(INPUT_KEYBOARD);
                            input.wVk = VK_UP;
                            list.Add(input);
                            
                            input = new KeyboardInput(input);
                            input.dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_EXTENDEDKEY;
                            list.Add(input);

                            reset = new KeyboardInput();
                            reset.type = new IntPtr(INPUT_KEYBOARD);
                            reset.wVk = VK_SHIFT;
                            reset.dwFlags = KEYEVENTF_KEYUP;
                            list.Add(reset);
                            shiftIsPressed = false;
                            break;
                        case "RIGHT":
                            input = new KeyboardInput();
                            input.type = new IntPtr(INPUT_KEYBOARD);
                            input.dwFlags = KEYEVENTF_EXTENDEDKEY;
                            input.wVk = VK_RIGHT;
                            list.Add(input);
                            
                            input = new KeyboardInput(input);
                            input.dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_EXTENDEDKEY;
                            list.Add(input);

                            reset = new KeyboardInput();
                            reset.type = new IntPtr(INPUT_KEYBOARD);
                            reset.wVk = VK_SHIFT;
                            reset.dwFlags = KEYEVENTF_KEYUP;
                            list.Add(reset);
                            shiftIsPressed = false;
                            break;
                        case "DOWN":
                            input = new KeyboardInput();
                            input.type = new IntPtr(INPUT_KEYBOARD);
                            input.dwFlags = KEYEVENTF_EXTENDEDKEY;
                            input.wVk = VK_DOWN;
                            list.Add(input);
                            
                            input = new KeyboardInput(input);
                            input.dwFlags = KEYEVENTF_KEYUP | KEYEVENTF_EXTENDEDKEY;
                            list.Add(input);

                            reset = new KeyboardInput();
                            reset.type = new IntPtr(INPUT_KEYBOARD);
                            reset.wVk = VK_SHIFT;
                            reset.dwFlags = KEYEVENTF_KEYUP;
                            list.Add(reset);
                            shiftIsPressed = false;
                            break;
                        default:
                            throw new ArgumentException(
                            "Malformed typing text: unknown escape code [" +
                            escapeCode + "]" + i + ": " + text);
                    }
                    i = closeIndex + 1;
                }
                else
                {
                    KeyboardInput input = new KeyboardInput();
                    input.type = new IntPtr(INPUT_KEYBOARD);
                    input.wVk = VkKeyScan(c);
                    list.Add(input);

                    input = new KeyboardInput(input);
                    input.dwFlags |= KEYEVENTF_KEYUP;
                    list.Add(input);

                    // Reset shift.
                    if (shiftIsPressed)
                    {
                        KeyboardInput reset = new KeyboardInput();
                        reset.type = new IntPtr(INPUT_KEYBOARD);
                        reset.wVk = VK_SHIFT;
                        reset.dwFlags = KEYEVENTF_KEYUP;
                        list.Add(reset);
                        shiftIsPressed = false;
                    }
                    // Reset shift.
                    if (controlIsPressed)
                    {
                        KeyboardInput reset = new KeyboardInput();
                        reset.type = new IntPtr(INPUT_KEYBOARD);
                        reset.wVk = VK_CONTROL;
                        reset.dwFlags = KEYEVENTF_KEYUP;
                        list.Add(reset);
                        controlIsPressed= false;
                    }
                    i++;
                }
            }

            KeyboardInput[] inputList = (KeyboardInput[])
                list.ToArray(typeof(KeyboardInput));
            unsafe
            {
                SendKeyboardInput((uint)inputList.Length, inputList, sizeof(KeyboardInput));
            }
        }

        #endregion Keyboard support.

        #endregion Input emulation support.
    }
}