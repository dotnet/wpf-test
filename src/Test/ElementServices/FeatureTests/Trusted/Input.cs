// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Input;
using Microsoft.Test.Win32;
using MTI = Microsoft.Test.Input;

namespace Avalon.Test.CoreUI.Trusted
{
    /// <summary>
    /// Class containing common methods for automating user input.
    /// </summary>
    /// <remarks>
    /// This class is meant to mimic UIAutomation.dll as closely as possible.
    /// </remarks>
    public sealed class Input
    {
        // 



        // 



        /// <summary>
        /// Inject keyboard input into the system.
        /// </summary>
        /// <param name="vk">The virtual key pressed or released. Can be one of the constants defined in the VKeys class.</param>
        /// <param name="press">true to inject a key press, false to inject a key release</param>
        /// <remarks>Input is sent to whatever window is in focus.</remarks>
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
		public static void SendKeyboardInput(byte vk, bool press)
		{
            // 


            Key k = KeyInterop.KeyFromVirtualKey(vk);
            MTI.Input.SendKeyboardInput(k, press);
        }

        /// <summary>
        /// Inject pointer input into the system.
        /// </summary>
        /// <param name="x">x coordinate of pointer, if Move flag specified</param>
        /// <param name="y">y coordinate of pointer, if Move flag specified</param>
        /// <param name="data">Wheel movement, or mouse X button, depending on flags.</param>
        /// <param name="flags">Flags to indicate which type of input occurred - move, button press/release, wheel move, etc.</param>
        /// <remarks>x, y are in pixels. If Absolute flag used, x and y are relative to desktop origin.</remarks>
        /// <remarks>Input is sent to whatever window is in focus.</remarks>
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
		public static void SendMouseInput(int x, int y, int data, SendMouseInputFlags flags)
		{
            // 


            SendMouseInput((double)x, (double)y, data, flags);
        }

        /// <summary>
        /// Inject pointer input into the system.
        /// </summary>
        /// <param name="x">x coordinate of pointer, if Move flag specified</param>
        /// <param name="y">y coordinate of pointer, if Move flag specified</param>
        /// <param name="data">Wheel movement, or mouse X button, depending on flags.</param>
        /// <param name="flags">Flags to indicate which type of input occurred - move, button press/release, wheel move, etc.</param>
        /// <remarks>x, y are in pixels. If Absolute flag used, x and y are relative to desktop origin.</remarks>
        /// <remarks>Input is sent to whatever window is in focus.</remarks>
        /// <outside_see conditional="false">
        /// This API does not work inside the secure execution environment.
        /// <exception cref="System.Security.Permissions.SecurityPermission"/>
        /// </outside_see>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
		public static void SendMouseInput(double x, double y, int data, SendMouseInputFlags flags)
		{
            // 


            MTI.Input.SendMouseInput(x, y, data, (MTI.SendMouseInputFlags)flags);
        }

        /// <summary>XButton</summary>
        public const int XButton1 = 1;

        /// <summary>XButton</summary>
        public const int XButton2 = 2;

        /// <summary>
        /// Retrieve real (physical) virtual key from any virtual key value.
        /// </summary>
        /// <param name="vk">VK value.</param>
        /// <returns>VK value corresponding to a physical virtual key.</returns>
        /// <remarks>
        /// This mimics USER32 semantics, where general VK modifier keys map to left-hand VK modifier keys.
        /// </remarks>
        public static byte RealVirtualKeyFromVirtualKey(byte vk)
        {
            byte realVk;

            switch (vk)
            {
                case VKeys.VkShift:
                    realVk = VKeys.VkLShift;
                    break;

                case VKeys.VkMenu:
                    realVk = VKeys.VkLeftAlt;
                    break;

                case VKeys.VkControl:
                    realVk = VKeys.VkLControl;
                    break;

                default:
                    // All other keys pass through.
                    realVk = vk;
                    break;
            }
            return realVk;
        }

        /// <summary>
        /// Retrieve a wheel delta number from a supplied mouse-wheel movement.
        /// </summary>
        /// <param name="direction">Which direction did the wheel go?</param>
        /// <param name="nNotchesMoved">How many notches did the wheel go?</param>
        /// <returns>An integer representating the delta of the mouse wheel action.</returns>
        /// <remarks>
        /// The return value can be used in USER32 mouse-wheel scenarios which demand a wheel delta.
        /// A positive number indicates a forward mouse-wheel.
        /// A negative number indicates a backward mouse-wheel.
        /// </remarks>
        public static int WheelDeltaFromWheelMovement(MouseWheelDirection direction, int nNotchesMoved)
        {
            int wheelDelta;

            switch (direction)
            {
                case MouseWheelDirection.Backward:
                    wheelDelta = nNotchesMoved * (-NativeConstants.WHEEL_DELTA);
                    break;

                case MouseWheelDirection.Forward:
                    wheelDelta = nNotchesMoved * NativeConstants.WHEEL_DELTA;
                    break;

                default:
                    wheelDelta = 0;
                    break;
            }
            return wheelDelta;
        }

        /// <summary>
        /// Get keyboard to its original startup state.
        /// </summary>
        /// <remarks>
        /// Currently the modifier keys (Shift, Control, Alt) are set to the Up state. 
        /// </remarks>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
		public static void ResetKeyboardState()
		{
            // Untoggle any toggled modifier keys
            byte[] states = new byte[256];
            NativeMethods.GetKeyboardState(states);
            foreach (byte vk in s_vkeysToReset)
            {
                states[vk] &= 0x0F;
            }
            NativeMethods.SetKeyboardState(states);
        }

        /// <summary>
        /// Keys to "unpress" when user asks for a keyboard reset.
        /// </summary>
        private static byte[] s_vkeysToReset = new byte[] {
            VKeys.VkLeftAlt,
            VKeys.VkRightAlt,
            VKeys.VkMenu,
            VKeys.VkLShift,
            VKeys.VkRShift,
            VKeys.VkShift,
            VKeys.VkLControl,
            VKeys.VkRControl,
            VKeys.VkControl,
            VKeys.VkLWin,
            VKeys.VkRWin,
            VKeys.VkCapital,
            VKeys.VkNumlock,
            VKeys.VkScroll,
        };
    }

    /// <summary>
    /// Flags to be combined and sent to SendMouseInput method.
    /// </summary>
    [Flags]
    public enum SendMouseInputFlags
    {
        /// <summary>Mouse input flag value</summary>
        Absolute = 32768,
        /// <summary>Mouse input flag value</summary>
        LeftDown = 2,
        /// <summary>Mouse input flag value</summary>
        LeftUp = 4,
        /// <summary>Mouse input flag value</summary>
        MiddleDown = 32,
        /// <summary>Mouse input flag value</summary>
        MiddleUp = 64,
        /// <summary>Mouse input flag value</summary>
        Move = 1,
        /// <summary>Mouse input flag value</summary>
        RightDown = 8,
        /// <summary>Mouse input flag value</summary>
        RightUp = 16,
        /// <summary>Mouse input flag value</summary>
        Wheel = 2048,
        /// <summary>Mouse input flag value</summary>
        XDown = 128,
        /// <summary>Mouse input flag value</summary>
        XUp = 256,
    } 

    /// <summary>
    /// Class containing virtual key definitions (VKs).
    /// </summary>
    public class VKeys
    {
        /// <summary>
        /// Construct this class.
        /// </summary>
        public VKeys() 
        {
            // do nothing
        }

        /// <summary>VK value</summary>
        public const byte Accept = 30;
        /// <summary>VK value</summary>
        public const byte Add = 107;
        /// <summary>VK value</summary>
        public const byte Apps = 93;
        /// <summary>VK value</summary>
        public const byte Attn = 246;
        /// <summary>VK value</summary>
        public const byte Back = 8;
        /// <summary>VK value</summary>
        public const byte BrowserBack = 166;
        /// <summary>VK value</summary>
        public const byte BrowserFavorites = 171;
        /// <summary>VK value</summary>
        public const byte BrowserForward = 167;
        /// <summary>VK value</summary>
        public const byte BrowserHome = 172;
        /// <summary>VK value</summary>
        public const byte BrowserRefresh = 168;
        /// <summary>VK value</summary>
        public const byte BrowserSearch = 170;
        /// <summary>VK value</summary>
        public const byte BrowserStop = 169;
        /// <summary>VK value</summary>
        public const byte Cancel = 3;
        /// <summary>VK value</summary>
        public const byte Capital = 20;
        /// <summary>VK value</summary>
        public const byte Clear = 12;
        /// <summary>VK value</summary>
        public const byte Control = 17;
        /// <summary>VK value</summary>
        public const byte Convert = 28;
        /// <summary>VK value</summary>
        public const byte CrSel = 247;
        /// <summary>VK value</summary>
        public const byte Decimal = 110;
        /// <summary>VK value</summary>
        public const byte Delete = 46;
        /// <summary>VK value</summary>
        public const byte Divide = 111;
        /// <summary>VK value</summary>
        public const byte Down = 40;
        /// <summary>VK value</summary>
        public const byte End = 35;
        /// <summary>VK value</summary>
        public const byte ErEof = 249;
        /// <summary>VK value</summary>
        public const byte Escape = 27;
        /// <summary>VK value</summary>
        public const byte Execute = 43;
        /// <summary>VK value</summary>
        public const byte ExSel = 248;
        /// <summary>VK value</summary>
        public const byte F1 = 112;
        /// <summary>VK value</summary>
        public const byte F10 = 121;
        /// <summary>VK value</summary>
        public const byte F11 = 122;
        /// <summary>VK value</summary>
        public const byte F12 = 123;
        /// <summary>VK value</summary>
        public const byte F13 = 124;
        /// <summary>VK value</summary>
        public const byte F14 = 125;
        /// <summary>VK value</summary>
        public const byte F15 = 126;
        /// <summary>VK value</summary>
        public const byte F16 = 127;
        /// <summary>VK value</summary>
        public const byte F17 = 128;
        /// <summary>VK value</summary>
        public const byte F18 = 129;
        /// <summary>VK value</summary>
        public const byte F19 = 130;
        /// <summary>VK value</summary>
        public const byte F2 = 113;
        /// <summary>VK value</summary>
        public const byte F20 = 131;
        /// <summary>VK value</summary>
        public const byte F21 = 132;
        /// <summary>VK value</summary>
        public const byte F22 = 133;
        /// <summary>VK value</summary>
        public const byte F23 = 134;
        /// <summary>VK value</summary>
        public const byte F24 = 135;
        /// <summary>VK value</summary>
        public const byte F3 = 114;
        /// <summary>VK value</summary>
        public const byte F4 = 115;
        /// <summary>VK value</summary>
        public const byte F5 = 116;
        /// <summary>VK value</summary>
        public const byte F6 = 117;
        /// <summary>VK value</summary>
        public const byte F7 = 118;
        /// <summary>VK value</summary>
        public const byte F8 = 119;
        /// <summary>VK value</summary>
        public const byte F9 = 120;
        /// <summary>VK value</summary>
        public const byte Final = 24;
        /// <summary>VK value</summary>
        public const byte Hangeul = 21;
        /// <summary>VK value</summary>
        public const byte Hangul = 21;
        /// <summary>VK value</summary>
        public const byte Hanja = 25;
        /// <summary>VK value</summary>
        public const byte Help = 47;
        /// <summary>VK value</summary>
        public const byte Home = 36;
        /// <summary>VK value</summary>
        public const byte Ico00 = 228;
        /// <summary>VK value</summary>
        public const byte IcoClear = 230;
        /// <summary>VK value</summary>
        public const byte IcoHelp = 227;
        /// <summary>VK value</summary>
        public const byte Insert = 45;
        /// <summary>VK value</summary>
        public const byte Junja = 23;
        /// <summary>VK value</summary>
        public const byte Kana = 21;
        /// <summary>VK value</summary>
        public const byte Kanji = 25;
        /// <summary>VK value</summary>
        public const byte LaunchApp1 = 182;
        /// <summary>VK value</summary>
        public const byte LaunchApp2 = 183;
        /// <summary>VK value</summary>
        public const byte LaunchMail = 180;
        /// <summary>VK value</summary>
        public const byte LaunchMediaSelect = 181;
        /// <summary>VK value</summary>
        public const byte LButton = 1;
        /// <summary>VK value</summary>
        public const byte LControl = 162;
        /// <summary>VK value</summary>
        public const byte Left = 37;
        /// <summary>VK value</summary>
        public const byte LeftAlt = 164;
        /// <summary>VK value</summary>
        public const byte LShift = 160;
        /// <summary>VK value</summary>
        public const byte LWin = 91;
        /// <summary>VK value</summary>
        public const byte MButton = 4;
        /// <summary>VK value</summary>
        public const byte MediaNextTrack = 176;
        /// <summary>VK value</summary>
        public const byte MediaPlayPause = 179;
        /// <summary>VK value</summary>
        public const byte MediaPrevTrack = 177;
        /// <summary>VK value</summary>
        public const byte MediaStop = 178;
        /// <summary>VK value</summary>
        public const byte Menu = 18;
        /// <summary>VK value</summary>
        public const byte ModeChange = 31;
        /// <summary>VK value</summary>
        public const byte Multiply = 106;
        /// <summary>VK value</summary>
        public const byte Next = 34;
        /// <summary>VK value</summary>
        public const byte NoName = 252;
        /// <summary>VK value</summary>
        public const byte NonConvert = 29;
        /// <summary>VK value</summary>
        public const byte Numlock = 144;
        /// <summary>VK value</summary>
        public const byte Numpad0 = 96;
        /// <summary>VK value</summary>
        public const byte Numpad1 = 97;
        /// <summary>VK value</summary>
        public const byte Numpad2 = 98;
        /// <summary>VK value</summary>
        public const byte Numpad3 = 99;
        /// <summary>VK value</summary>
        public const byte Numpad4 = 100;
        /// <summary>VK value</summary>
        public const byte Numpad5 = 101;
        /// <summary>VK value</summary>
        public const byte Numpad6 = 102;
        /// <summary>VK value</summary>
        public const byte Numpad7 = 103;
        /// <summary>VK value</summary>
        public const byte Numpad8 = 104;
        /// <summary>VK value</summary>
        public const byte Numpad9 = 105;
        /// <summary>VK value</summary>
        public const byte Oem1 = 186;
        /// <summary>VK value</summary>
        public const byte Oem102 = 226;
        /// <summary>VK value</summary>
        public const byte Oem2 = 191;
        /// <summary>VK value</summary>
        public const byte Oem3 = 192;
        /// <summary>VK value</summary>
        public const byte Oem4 = 219;
        /// <summary>VK value</summary>
        public const byte Oem5 = 220;
        /// <summary>VK value</summary>
        public const byte Oem6 = 221;
        /// <summary>VK value</summary>
        public const byte Oem7 = 222;
        /// <summary>VK value</summary>
        public const byte Oem8 = 223;
        /// <summary>VK value</summary>
        public const byte OemAttn = 240;
        /// <summary>VK value</summary>
        public const byte OemAuto = 243;
        /// <summary>VK value</summary>
        public const byte OemAx = 225;
        /// <summary>VK value</summary>
        public const byte OemBackTab = 245;
        /// <summary>VK value</summary>
        public const byte OemClear = 254;
        /// <summary>VK value</summary>
        public const byte OemComma = 188;
        /// <summary>VK value</summary>
        public const byte OemCopy = 242;
        /// <summary>VK value</summary>
        public const byte OemCuSel = 239;
        /// <summary>VK value</summary>
        public const byte OemEnlw = 244;
        /// <summary>VK value</summary>
        public const byte OemFinish = 241;
        /// <summary>VK value</summary>
        public const byte OemFjJisho = 146;
        /// <summary>VK value</summary>
        public const byte OemFjLoya = 149;
        /// <summary>VK value</summary>
        public const byte OemFjMasshou = 147;
        /// <summary>VK value</summary>
        public const byte OemFjRoya = 150;
        /// <summary>VK value</summary>
        public const byte OemFjTouroku = 148;
        /// <summary>VK value</summary>
        public const byte OemJump = 234;
        /// <summary>VK value</summary>
        public const byte OemMinus = 189;
        /// <summary>VK value</summary>
        public const byte OemNecEqual = 146;
        /// <summary>VK value</summary>
        public const byte OemPa1 = 235;
        /// <summary>VK value</summary>
        public const byte OemPa2 = 236;
        /// <summary>VK value</summary>
        public const byte OemPa3 = 237;
        /// <summary>VK value</summary>
        public const byte OemPeriod = 190;
        /// <summary>VK value</summary>
        public const byte OemPlus = 187;
        /// <summary>VK value</summary>
        public const byte OemReset = 233;
        /// <summary>VK value</summary>
        public const byte OemWsCtrl = 238;
        /// <summary>VK value</summary>
        public const byte Pa1 = 253;
        /// <summary>VK value</summary>
        public const byte Packet = 231;
        /// <summary>VK value</summary>
        public const byte Pause = 19;
        /// <summary>VK value</summary>
        public const byte Play = 250;
        /// <summary>VK value</summary>
        public const byte Print = 42;
        /// <summary>VK value</summary>
        public const byte Prior = 33;
        /// <summary>VK value</summary>
        public const byte ProcessKey = 229;
        /// <summary>VK value</summary>
        public const byte RButton = 2;
        /// <summary>VK value</summary>
        public const byte RControl = 163;
        /// <summary>VK value</summary>
        public const byte Return = 13;
        /// <summary>VK value</summary>
        public const byte Right = 39;
        /// <summary>VK value</summary>
        public const byte RightAlt = 165;
        /// <summary>VK value</summary>
        public const byte RShift = 161;
        /// <summary>VK value</summary>
        public const byte RWin = 92;
        /// <summary>VK value</summary>
        public const byte Scroll = 145;
        /// <summary>VK value</summary>
        public const byte Select = 41;
        /// <summary>VK value</summary>
        public const byte Separator = 108;
        /// <summary>VK value</summary>
        public const byte Shift = 16;
        /// <summary>VK value</summary>
        public const byte Sleep = 95;
        /// <summary>VK value</summary>
        public const byte Snapshot = 44;
        /// <summary>VK value</summary>
        public const byte Space = 32;
        /// <summary>VK value</summary>
        public const byte Subtract = 109;
        /// <summary>VK value</summary>
        public const byte Tab = 9;
        /// <summary>VK value</summary>
        public const byte Up = 38;
        /// <summary>VK value</summary>
        public const byte Vk0 = 48;
        /// <summary>VK value</summary>
        public const byte Vk1 = 49;
        /// <summary>VK value</summary>
        public const byte Vk2 = 50;
        /// <summary>VK value</summary>
        public const byte Vk3 = 51;
        /// <summary>VK value</summary>
        public const byte Vk4 = 52;
        /// <summary>VK value</summary>
        public const byte Vk5 = 53;
        /// <summary>VK value</summary>
        public const byte Vk6 = 54;
        /// <summary>VK value</summary>
        public const byte Vk7 = 55;
        /// <summary>VK value</summary>
        public const byte Vk8 = 56;
        /// <summary>VK value</summary>
        public const byte Vk9 = 57;
        /// <summary>VK value</summary>
        public const byte VkA = 65;
        /// <summary>VK value</summary>
        public const byte VkAccept = 30;
        /// <summary>VK value</summary>
        public const byte VkAdd = 107;
        /// <summary>VK value</summary>
        public const byte VkAlt = 18;
        /// <summary>VK value</summary>
        public const byte VkApps = 93;
        /// <summary>VK value</summary>
        public const byte VkAttn = 246;
        /// <summary>VK value</summary>
        public const byte VkB = 66;
        /// <summary>VK value</summary>
        public const byte VkBack = 8;
        /// <summary>VK value</summary>
        public const byte VkBrowserBack = 166;
        /// <summary>VK value</summary>
        public const byte VkBrowserFavorites = 171;
        /// <summary>VK value</summary>
        public const byte VkBrowserForward = 167;
        /// <summary>VK value</summary>
        public const byte VkBrowserHome = 172;
        /// <summary>VK value</summary>
        public const byte VkBrowserRefresh = 168;
        /// <summary>VK value</summary>
        public const byte VkBrowserSearch = 170;
        /// <summary>VK value</summary>
        public const byte VkBrowserStop = 169;
        /// <summary>VK value</summary>
        public const byte VkC = 67;
        /// <summary>VK value</summary>
        public const byte VkCancel = 3;
        /// <summary>VK value</summary>
        public const byte VkCapital = 20;
        /// <summary>VK value</summary>
        public const byte VkClear = 12;
        /// <summary>VK value</summary>
        public const byte VkControl = 17;
        /// <summary>VK value</summary>
        public const byte VkConvert = 28;
        /// <summary>VK value</summary>
        public const byte VkCrSel = 247;
        /// <summary>VK value</summary>
        public const byte VkD = 68;
        /// <summary>VK value</summary>
        public const byte VkDecimal = 110;
        /// <summary>VK value</summary>
        public const byte VkDelete = 46;
        /// <summary>VK value</summary>
        public const byte VkDivide = 111;
        /// <summary>VK value</summary>
        public const byte VkDown = 40;
        /// <summary>VK value</summary>
        public const byte VkE = 69;
        /// <summary>VK value</summary>
        public const byte VkEnd = 35;
        /// <summary>VK value</summary>
        public const byte VkErEof = 249;
        /// <summary>VK value</summary>
        public const byte VkEscape = 27;
        /// <summary>VK value</summary>
        public const byte VkExecute = 43;
        /// <summary>VK value</summary>
        public const byte VkExSel = 248;
        /// <summary>VK value</summary>
        public const byte VkF = 70;
        /// <summary>VK value</summary>
        public const byte VkF1 = 112;
        /// <summary>VK value</summary>
        public const byte VkF10 = 121;
        /// <summary>VK value</summary>
        public const byte VkF11 = 122;
        /// <summary>VK value</summary>
        public const byte VkF12 = 123;
        /// <summary>VK value</summary>
        public const byte VkF13 = 124;
        /// <summary>VK value</summary>
        public const byte VkF14 = 125;
        /// <summary>VK value</summary>
        public const byte VkF15 = 126;
        /// <summary>VK value</summary>
        public const byte VkF16 = 127;
        /// <summary>VK value</summary>
        public const byte VkF17 = 128;
        /// <summary>VK value</summary>
        public const byte VkF18 = 129;
        /// <summary>VK value</summary>
        public const byte VkF19 = 130;
        /// <summary>VK value</summary>
        public const byte VkF2 = 113;
        /// <summary>VK value</summary>
        public const byte VkF20 = 131;
        /// <summary>VK value</summary>
        public const byte VkF21 = 132;
        /// <summary>VK value</summary>
        public const byte VkF22 = 133;
        /// <summary>VK value</summary>
        public const byte VkF23 = 134;
        /// <summary>VK value</summary>
        public const byte VkF24 = 135;
        /// <summary>VK value</summary>
        public const byte VkF3 = 114;
        /// <summary>VK value</summary>
        public const byte VkF4 = 115;
        /// <summary>VK value</summary>
        public const byte VkF5 = 116;
        /// <summary>VK value</summary>
        public const byte VkF6 = 117;
        /// <summary>VK value</summary>
        public const byte VkF7 = 118;
        /// <summary>VK value</summary>
        public const byte VkF8 = 119;
        /// <summary>VK value</summary>
        public const byte VkF9 = 120;
        /// <summary>VK value</summary>
        public const byte VkFinal = 24;
        /// <summary>VK value</summary>
        public const byte VkG = 71;
        /// <summary>VK value</summary>
        public const byte VkH = 72;
        /// <summary>VK value</summary>
        public const byte VkHangul = 21;
        /// <summary>VK value</summary>
        public const byte VkHanja = 25;
        /// <summary>VK value</summary>
        public const byte VkHelp = 47;
        /// <summary>VK value</summary>
        public const byte VkHome = 36;
        /// <summary>VK value</summary>
        public const byte VkI = 73;
        /// <summary>VK value</summary>
        public const byte VkIco00 = 228;
        /// <summary>VK value</summary>
        public const byte VkIcoClear = 230;
        /// <summary>VK value</summary>
        public const byte VkIcoHelp = 227;
        /// <summary>VK value</summary>
        public const byte VkInsert = 45;
        /// <summary>VK value</summary>
        public const byte VkJ = 74;
        /// <summary>VK value</summary>
        public const byte VkJunja = 23;
        /// <summary>VK value</summary>
        public const byte VkK = 75;
        /// <summary>VK value</summary>
        public const byte VkKana = 21;
        /// <summary>VK value</summary>
        public const byte VkKanji = 25;
        /// <summary>VK value</summary>
        public const byte VkL = 76;
        /// <summary>VK value</summary>
        public const byte VkLAlt = 164;
        /// <summary>VK value</summary>
        public const byte VkLaunchApp1 = 182;
        /// <summary>VK value</summary>
        public const byte VkLaunchApp2 = 183;
        /// <summary>VK value</summary>
        public const byte VkLaunchMail = 180;
        /// <summary>VK value</summary>
        public const byte VkLaunchMediaSelect = 181;
        /// <summary>VK value</summary>
        public const byte VkLButton = 1;
        /// <summary>VK value</summary>
        public const byte VkLControl = 162;
        /// <summary>VK value</summary>
        public const byte VkLeft = 37;
        /// <summary>VK value</summary>
        public const byte VkLeftAlt = 164;
        /// <summary>VK value</summary>
        public const byte VkLShift = 160;
        /// <summary>VK value</summary>
        public const byte VkLWin = 91;
        /// <summary>VK value</summary>
        public const byte VkM = 77;
        /// <summary>VK value</summary>
        public const byte VkMButton = 4;
        /// <summary>VK value</summary>
        public const byte VkMediaNextTrack = 176;
        /// <summary>VK value</summary>
        public const byte VkMediaPlayPause = 179;
        /// <summary>VK value</summary>
        public const byte VkMediaPrevTrack = 177;
        /// <summary>VK value</summary>
        public const byte VkMediaStop = 178;
        /// <summary>VK value</summary>
        public const byte VkMenu = 18;
        /// <summary>VK value</summary>
        public const byte VkModeChange = 31;
        /// <summary>VK value</summary>
        public const byte VkMultiply = 106;
        /// <summary>VK value</summary>
        public const byte VkN = 78;
        /// <summary>VK value</summary>
        public const byte VkNext = 34;
        /// <summary>VK value</summary>
        public const byte VkNoName = 252;
        /// <summary>VK value</summary>
        public const byte VkNonConvert = 29;
        /// <summary>VK value</summary>
        public const byte VkNumlock = 144;
        /// <summary>VK value</summary>
        public const byte VkNumpad0 = 96;
        /// <summary>VK value</summary>
        public const byte VkNumpad1 = 97;
        /// <summary>VK value</summary>
        public const byte VkNumpad2 = 98;
        /// <summary>VK value</summary>
        public const byte VkNumpad3 = 99;
        /// <summary>VK value</summary>
        public const byte VkNumpad4 = 100;
        /// <summary>VK value</summary>
        public const byte VkNumpad5 = 101;
        /// <summary>VK value</summary>
        public const byte VkNumpad6 = 102;
        /// <summary>VK value</summary>
        public const byte VkNumpad7 = 103;
        /// <summary>VK value</summary>
        public const byte VkNumpad8 = 104;
        /// <summary>VK value</summary>
        public const byte VkNumpad9 = 105;
        /// <summary>VK value</summary>
        public const byte VkO = 79;
        /// <summary>VK value</summary>
        public const byte VkOem1 = 186;
        /// <summary>VK value</summary>
        public const byte VkOem102 = 226;
        /// <summary>VK value</summary>
        public const byte VkOem2 = 191;
        /// <summary>VK value</summary>
        public const byte VkOem3 = 192;
        /// <summary>VK value</summary>
        public const byte VkOem4 = 219;
        /// <summary>VK value</summary>
        public const byte VkOem5 = 220;
        /// <summary>VK value</summary>
        public const byte VkOem6 = 221;
        /// <summary>VK value</summary>
        public const byte VkOem7 = 222;
        /// <summary>VK value</summary>
        public const byte VkOem8 = 223;
        /// <summary>VK value</summary>
        public const byte VkOemAttn = 240;
        /// <summary>VK value</summary>
        public const byte VkOemAuto = 243;
        /// <summary>VK value</summary>
        public const byte VkOemAx = 225;
        /// <summary>VK value</summary>
        public const byte VkOemBackTab = 245;
        /// <summary>VK value</summary>
        public const byte VkOemClear = 254;
        /// <summary>VK value</summary>
        public const byte VkOemComma = 188;
        /// <summary>VK value</summary>
        public const byte VkOemCopy = 242;
        /// <summary>VK value</summary>
        public const byte VkOemCuSel = 239;
        /// <summary>VK value</summary>
        public const byte VkOemEnlw = 244;
        /// <summary>VK value</summary>
        public const byte VkOemFinish = 241;
        /// <summary>VK value</summary>
        public const byte VkOemFjJisho = 146;
        /// <summary>VK value</summary>
        public const byte VkOemFjLoya = 149;
        /// <summary>VK value</summary>
        public const byte VkOemFjMasshou = 147;
        /// <summary>VK value</summary>
        public const byte VkOemFjRoya = 150;
        /// <summary>VK value</summary>
        public const byte VkOemFjTouroku = 148;
        /// <summary>VK value</summary>
        public const byte VkOemJump = 234;
        /// <summary>VK value</summary>
        public const byte VkOemMinus = 189;
        /// <summary>VK value</summary>
        public const byte VkOemNecEqual = 146;
        /// <summary>VK value</summary>
        public const byte VkOemPa1 = 235;
        /// <summary>VK value</summary>
        public const byte VkOemPa2 = 236;
        /// <summary>VK value</summary>
        public const byte VkOemPa3 = 237;
        /// <summary>VK value</summary>
        public const byte VkOemPeriod = 190;
        /// <summary>VK value</summary>
        public const byte VkOemPlus = 187;
        /// <summary>VK value</summary>
        public const byte VkOemReset = 233;
        /// <summary>VK value</summary>
        public const byte VkOemWsCtrl = 238;
        /// <summary>VK value</summary>
        public const byte VkP = 80;
        /// <summary>VK value</summary>
        public const byte VkPa1 = 253;
        /// <summary>VK value</summary>
        public const byte VkPacket = 231;
        /// <summary>VK value</summary>
        public const byte VkPause = 19;
        /// <summary>VK value</summary>
        public const byte VkPlay = 250;
        /// <summary>VK value</summary>
        public const byte VkPrint = 42;
        /// <summary>VK value</summary>
        public const byte VkPrior = 33;
        /// <summary>VK value</summary>
        public const byte VkProcessKey = 229;
        /// <summary>VK value</summary>
        public const byte VkQ = 81;
        /// <summary>VK value</summary>
        public const byte VkR = 82;
        /// <summary>VK value</summary>
        public const byte VkRAlt = 165;
        /// <summary>VK value</summary>
        public const byte VkRButton = 2;
        /// <summary>VK value</summary>
        public const byte VkRControl = 163;
        /// <summary>VK value</summary>
        public const byte VkReturn = 13;
        /// <summary>VK value</summary>
        public const byte VkRight = 39;
        /// <summary>VK value</summary>
        public const byte VkRightAlt = 165;
        /// <summary>VK value</summary>
        public const byte VkRShift = 161;
        /// <summary>VK value</summary>
        public const byte VkRWin = 92;
        /// <summary>VK value</summary>
        public const byte VkS = 83;
        /// <summary>VK value</summary>
        public const byte VkScroll = 145;
        /// <summary>VK value</summary>
        public const byte VkSelect = 41;
        /// <summary>VK value</summary>
        public const byte VkSeparator = 108;
        /// <summary>VK value</summary>
        public const byte VkShift = 16;
        /// <summary>VK value</summary>
        public const byte VkSleep = 95;
        /// <summary>VK value</summary>
        public const byte VkSnapshot = 44;
        /// <summary>VK value</summary>
        public const byte VkSpace = 32;
        /// <summary>VK value</summary>
        public const byte VkSubtract = 109;
        /// <summary>VK value</summary>
        public const byte VkT = 84;
        /// <summary>VK value</summary>
        public const byte VkTab = 9;
        /// <summary>VK value</summary>
        public const byte VkU = 85;
        /// <summary>VK value</summary>
        public const byte VkUp = 38;
        /// <summary>VK value</summary>
        public const byte VkV = 86;
        /// <summary>VK value</summary>
        public const byte VkVolumeDown = 174;
        /// <summary>VK value</summary>
        public const byte VkVolumeMute = 173;
        /// <summary>VK value</summary>
        public const byte VkVolumeUp = 175;
        /// <summary>VK value</summary>
        public const byte VkW = 87;
        /// <summary>VK value</summary>
        public const byte VkX = 88;
        /// <summary>VK value</summary>
        public const byte VkXButton1 = 5;
        /// <summary>VK value</summary>
        public const byte VkXButton2 = 6;
        /// <summary>VK value</summary>
        public const byte VkY = 89;
        /// <summary>VK value</summary>
        public const byte VkZ = 90;
        /// <summary>VK value</summary>
        public const byte VkZoom = 251;
        /// <summary>VK value</summary>
        public const byte VolumeDown = 174;
        /// <summary>VK value</summary>
        public const byte VolumeMute = 173;
        /// <summary>VK value</summary>
        public const byte VolumeUp = 175;
        /// <summary>VK value</summary>
        public const byte XButton1 = 5;
        /// <summary>VK value</summary>
        public const byte XButton2 = 6;
        /// <summary>VK value</summary>
        public const byte Zoom = 251;
    }

    /// <summary>
    /// What directions can a mouse wheel go?
    /// </summary>
    public enum MouseWheelDirection
    {
        /// <summary>
        /// Mouse wheel goes backward.
        /// </summary>
        Backward = 0,
        /// <summary>
        /// Mouse wheel goes forward.
        /// </summary>
        Forward = 1,
    }


}
