// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides common wrappers for test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Wrappers/Wrappers.cs $")]

namespace Test.Uis.Wrappers
{
    #region Namespaces.

    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    #endregion Namespaces.

    /// <summary>This class provides native function imports.</summary>
    /// <remarks>Note that you need unmanaged permissions to invoke this
    /// methods. The preferred way to access this functionality is
    /// by encapsulating the call in the common library.</remarks>
    /// <example>The following sample shows how to call the GetSystemMetrics
    /// method.<code>...
    /// public static int SafeGetSystemMetrics(int x) {
    ///   new SecurityPermission(PermissionState.Unrestricted).Assert();
    ///   return Win32.GetSystemMetrics(x);
    /// }
    /// </code></example>
    [SuppressUnmanagedCodeSecurity]
    public class Win32
    {
        #region Virtual key codes.

        /// <summary>VK_LBUTTON constant.</summary>
        public const byte VK_LBUTTON = 0x01;
        /// <summary>VK_RBUTTON constant.</summary>
        public const byte VK_RBUTTON = 0x02;
        /// <summary>VK_CANCEL constant.</summary>
        public const byte VK_CANCEL = 0x03;
        /// <summary>VK_MBUTTON constant.</summary>
        public const byte VK_MBUTTON = 0x04    /* NOT contiguous with L & RBUTTON */;

        /// <summary>VK_XBUTTON1 constant.</summary>
        public const byte VK_XBUTTON1 = 0x05    /* NOT contiguous with L & RBUTTON */;
        /// <summary>VK_XBUTTON2 constant.</summary>
        public const byte VK_XBUTTON2 = 0x06    /* NOT contiguous with L & RBUTTON */;

        // 0x07 : unassigned

        /// <summary>VK_BACK constant.</summary>
        public const byte VK_BACK = 0x08;
        /// <summary>VK_TAB constant.</summary>
        public const byte VK_TAB = 0x09;

        // 0x0A - 0x0B : reserved

        /// <summary>VK_CLEAR constant.</summary>
        public const byte VK_CLEAR = 0x0C;
        /// <summary>VK_RETURN constant.</summary>
        public const byte VK_RETURN = 0x0D;

        /// <summary>VK_SHIFT constant.</summary>
        public const byte VK_SHIFT = 0x10;
        /// <summary>VK_CONTROL constant.</summary>
        public const byte VK_CONTROL = 0x11;
        /// <summary>VK_MENU constant.</summary>
        public const byte VK_MENU = 0x12;
        /// <summary>VK_PAUSE constant.</summary>
        public const byte VK_PAUSE = 0x13;
        /// <summary>VK_CAPITAL constant.</summary>
        public const byte VK_CAPITAL = 0x14;

        /// <summary>VK_KANA constant.</summary>
        public const byte VK_KANA = 0x15;
        /// <summary>VK_HANGEUL constant.</summary>
        public const byte VK_HANGEUL = 0x15  /* old name - should be here for compatibility */;
        /// <summary>VK_HANGUL constant.</summary>
        public const byte VK_HANGUL = 0x15;
        /// <summary>VK_JUNJA constant.</summary>
        public const byte VK_JUNJA = 0x17;
        /// <summary>VK_FINAL constant.</summary>
        public const byte VK_FINAL = 0x18;
        /// <summary>VK_HANJA constant.</summary>
        public const byte VK_HANJA = 0x19;
        /// <summary>VK_KANJI constant.</summary>
        public const byte VK_KANJI = 0x19;

        /// <summary>VK_ESCAPE constant.</summary>
        public const byte VK_ESCAPE = 0x1B;

        /// <summary>VK_CONVERT constant.</summary>
        public const byte VK_CONVERT = 0x1C;
        /// <summary>VK_NONCONVERT constant.</summary>
        public const byte VK_NONCONVERT = 0x1D;
        /// <summary>VK_ACCEPT constant.</summary>
        public const byte VK_ACCEPT = 0x1E;
        /// <summary>VK_MODECHANGE constant.</summary>
        public const byte VK_MODECHANGE = 0x1F;

        /// <summary>VK_SPACE constant.</summary>
        public const byte VK_SPACE = 0x20;
        /// <summary>VK_PRIOR constant.</summary>
        public const byte VK_PRIOR = 0x21;
        /// <summary>VK_NEXT constant.</summary>
        public const byte VK_NEXT = 0x22;
        /// <summary>VK_END constant.</summary>
        public const byte VK_END = 0x23;
        /// <summary>VK_HOME constant.</summary>
        public const byte VK_HOME = 0x24;
        /// <summary>VK_LEFT constant.</summary>
        public const byte VK_LEFT = 0x25;
        /// <summary>VK_UP constant.</summary>
        public const byte VK_UP = 0x26;
        /// <summary>VK_RIGHT constant.</summary>
        public const byte VK_RIGHT = 0x27;
        /// <summary>VK_DOWN constant.</summary>
        public const byte VK_DOWN = 0x28;
        /// <summary>VK_SELECT constant.</summary>
        public const byte VK_SELECT = 0x29;
        /// <summary>VK_PRINT constant.</summary>
        public const byte VK_PRINT = 0x2A;
        /// <summary>VK_EXECUTE constant.</summary>
        public const byte VK_EXECUTE = 0x2B;
        /// <summary>VK_SNAPSHOT constant.</summary>
        public const byte VK_SNAPSHOT = 0x2C;
        /// <summary>VK_INSERT constant.</summary>
        public const byte VK_INSERT = 0x2D;
        /// <summary>VK_DELETE constant.</summary>
        public const byte VK_DELETE = 0x2E;
        /// <summary>VK_HELP constant.</summary>
        public const byte VK_HELP = 0x2F;

        // VK_0 - VK_9 are the same as ASCII '0' - '9' (0x30 - 0x39)
        // 0x40 : unassigned
        // VK_A - VK_Z are the same as ASCII 'A' - 'Z' (0x41 - 0x5A)
        /// <summary>VK_LWIN constant.</summary>
        public const byte VK_LWIN = 0x5B;
        /// <summary>VK_RWIN constant.</summary>
        public const byte VK_RWIN = 0x5C;
        /// <summary>VK_APPS constant.</summary>
        public const byte VK_APPS = 0x5D;

        // 0x5E : reserved
        /// <summary>VK_SLEEP constant.</summary>
        public const byte VK_SLEEP = 0x5F;

        /// <summary>VK_NUMPAD0 constant.</summary>
        public const byte VK_NUMPAD0 = 0x60;
        /// <summary>VK_NUMPAD1 constant.</summary>
        public const byte VK_NUMPAD1 = 0x61;
        /// <summary>VK_NUMPAD2 constant.</summary>
        public const byte VK_NUMPAD2 = 0x62;
        /// <summary>VK_NUMPAD3 constant.</summary>
        public const byte VK_NUMPAD3 = 0x63;
        /// <summary>VK_NUMPAD4 constant.</summary>
        public const byte VK_NUMPAD4 = 0x64;
        /// <summary>VK_NUMPAD5 constant.</summary>
        public const byte VK_NUMPAD5 = 0x65;
        /// <summary>VK_NUMPAD6 constant.</summary>
        public const byte VK_NUMPAD6 = 0x66;
        /// <summary>VK_NUMPAD7 constant.</summary>
        public const byte VK_NUMPAD7 = 0x67;
        /// <summary>VK_NUMPAD8 constant.</summary>
        public const byte VK_NUMPAD8 = 0x68;
        /// <summary>VK_NUMPAD9 constant.</summary>
        public const byte VK_NUMPAD9 = 0x69;
        /// <summary>VK_MULTIPLY constant.</summary>
        public const byte VK_MULTIPLY = 0x6A;
        /// <summary>VK_ADD constant.</summary>
        public const byte VK_ADD = 0x6B;
        /// <summary>VK_SEPARATOR constant.</summary>
        public const byte VK_SEPARATOR = 0x6C;
        /// <summary>VK_SUBTRACT constant.</summary>
        public const byte VK_SUBTRACT = 0x6D;
        /// <summary>VK_DECIMAL constant.</summary>
        public const byte VK_DECIMAL = 0x6E;
        /// <summary>VK_DIVIDE constant.</summary>
        public const byte VK_DIVIDE = 0x6F;
        /// <summary>VK_F1 constant.</summary>
        public const byte VK_F1 = 0x70;
        /// <summary>VK_F2 constant.</summary>
        public const byte VK_F2 = 0x71;
        /// <summary>VK_F3 constant.</summary>
        public const byte VK_F3 = 0x72;
        /// <summary>VK_F4 constant.</summary>
        public const byte VK_F4 = 0x73;
        /// <summary>VK_F5 constant.</summary>
        public const byte VK_F5 = 0x74;
        /// <summary>VK_F6 constant.</summary>
        public const byte VK_F6 = 0x75;
        /// <summary>VK_F7 constant.</summary>
        public const byte VK_F7 = 0x76;
        /// <summary>VK_F8 constant.</summary>
        public const byte VK_F8 = 0x77;
        /// <summary>VK_F9 constant.</summary>
        public const byte VK_F9 = 0x78;
        /// <summary>VK_F10 constant.</summary>
        public const byte VK_F10 = 0x79;
        /// <summary>VK_F11 constant.</summary>
        public const byte VK_F11 = 0x7A;
        /// <summary>VK_F12 constant.</summary>
        public const byte VK_F12 = 0x7B;
        /// <summary>VK_F13 constant.</summary>
        public const byte VK_F13 = 0x7C;
        /// <summary>VK_F14 constant.</summary>
        public const byte VK_F14 = 0x7D;
        /// <summary>VK_F15 constant.</summary>
        public const byte VK_F15 = 0x7E;
        /// <summary>VK_F16 constant.</summary>
        public const byte VK_F16 = 0x7F;
        /// <summary>VK_F17 constant.</summary>
        public const byte VK_F17 = 0x80;
        /// <summary>VK_F18 constant.</summary>
        public const byte VK_F18 = 0x81;
        /// <summary>VK_F19 constant.</summary>
        public const byte VK_F19 = 0x82;
        /// <summary>VK_F20 constant.</summary>
        public const byte VK_F20 = 0x83;
        /// <summary>VK_F21 constant.</summary>
        public const byte VK_F21 = 0x84;
        /// <summary>VK_F22 constant.</summary>
        public const byte VK_F22 = 0x85;
        /// <summary>VK_F23 constant.</summary>
        public const byte VK_F23 = 0x86;
        /// <summary>VK_F24 constant.</summary>
        public const byte VK_F24 = 0x87;

        // 0x88 - 0x8F : unassigned
        /// <summary>VK_NUMLOCK constant.</summary>
        public const byte VK_NUMLOCK = 0x90;
        /// <summary>VK_SCROLL constant.</summary>
        public const byte VK_SCROLL = 0x91;

        // NEC PC-9800 kbd definitions
        /// <summary>VK_OEM_NEC_EQUAL constant.</summary>
        public const byte VK_OEM_NEC_EQUAL = 0x92  ; // '=' key on numpad;

        // Fujitsu/OASYS kbd definitions
        /// <summary>VK_OEM_FJ_JISHO constant.</summary>
        public const byte VK_OEM_FJ_JISHO = 0x92  ; // 'Dictionary' key;
        /// <summary>VK_OEM_FJ_MASSHOU constant.</summary>
        public const byte VK_OEM_FJ_MASSHOU = 0x93  ; // 'Unregister word' key;
        /// <summary>VK_OEM_FJ_TOUROKU constant.</summary>
        public const byte VK_OEM_FJ_TOUROKU = 0x94  ; // 'Register word' key;
        /// <summary>VK_OEM_FJ_LOYA constant.</summary>
        public const byte VK_OEM_FJ_LOYA = 0x95  ; // 'Left OYAYUBI' key;
        /// <summary>VK_OEM_FJ_ROYA constant.</summary>
        public const byte VK_OEM_FJ_ROYA = 0x96  ; // 'Right OYAYUBI' key;

        // 0x97 - 0x9F : unassigned

        // VK_L* & VK_R* - left and right Alt, Ctrl and Shift virtual keys.
        // Used only as parameters to GetAsyncKeyState() and GetKeyState().
        // No other API or message will distinguish left and right keys in this way.
        /// <summary>VK_LSHIFT constant.</summary>
        public const byte VK_LSHIFT = 0xA0;
        /// <summary>VK_RSHIFT constant.</summary>
        public const byte VK_RSHIFT = 0xA1;
        /// <summary>VK_LCONTROL constant.</summary>
        public const byte VK_LCONTROL = 0xA2;
        /// <summary>VK_RCONTROL constant.</summary>
        public const byte VK_RCONTROL = 0xA3;
        /// <summary>VK_LMENU constant.</summary>
        public const byte VK_LMENU = 0xA4;
        /// <summary>VK_RMENU constant.</summary>
        public const byte VK_RMENU = 0xA5;

        /// <summary>VK_BROWSER_BACK constant.</summary>
        public const byte VK_BROWSER_BACK = 0xA6;
        /// <summary>VK_BROWSER_FORWARD constant.</summary>
        public const byte VK_BROWSER_FORWARD = 0xA7;
        /// <summary>VK_BROWSER_REFRESH constant.</summary>
        public const byte VK_BROWSER_REFRESH = 0xA8;
        /// <summary>VK_BROWSER_STOP constant.</summary>
        public const byte VK_BROWSER_STOP = 0xA9;
        /// <summary>VK_BROWSER_SEARCH constant.</summary>
        public const byte VK_BROWSER_SEARCH = 0xAA;
        /// <summary>VK_BROWSER_FAVORITES constant.</summary>
        public const byte VK_BROWSER_FAVORITES = 0xAB;
        /// <summary>VK_BROWSER_HOME constant.</summary>
        public const byte VK_BROWSER_HOME = 0xAC;

        /// <summary>VK_VOLUME_MUTE constant.</summary>
        public const byte VK_VOLUME_MUTE = 0xAD;
        /// <summary>VK_VOLUME_DOWN constant.</summary>
        public const byte VK_VOLUME_DOWN = 0xAE;
        /// <summary>VK_VOLUME_UP constant.</summary>
        public const byte VK_VOLUME_UP = 0xAF;
        /// <summary>VK_MEDIA_NEXT_TRACK constant.</summary>
        public const byte VK_MEDIA_NEXT_TRACK = 0xB0;
        /// <summary>VK_MEDIA_PREV_TRACK constant.</summary>
        public const byte VK_MEDIA_PREV_TRACK = 0xB1;
        /// <summary>VK_MEDIA_STOP constant.</summary>
        public const byte VK_MEDIA_STOP = 0xB2;
        /// <summary>VK_MEDIA_PLAY_PAUSE constant.</summary>
        public const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
        /// <summary>VK_LAUNCH_MAIL constant.</summary>
        public const byte VK_LAUNCH_MAIL = 0xB4;
        /// <summary>VK_LAUNCH_MEDIA_SELECT constant.</summary>
        public const byte VK_LAUNCH_MEDIA_SELECT = 0xB5;
        /// <summary>VK_LAUNCH_APP1 constant.</summary>
        public const byte VK_LAUNCH_APP1 = 0xB6;
        /// <summary>VK_LAUNCH_APP2 constant.</summary>
        public const byte VK_LAUNCH_APP2 = 0xB7;

        // 0xB8 - 0xB9 : reserved

        /// <summary>VK_OEM_1 constant.</summary>
        public const byte VK_OEM_1 = 0xBA  ; // ';:' for US;
        /// <summary>VK_OEM_PLUS constant.</summary>
        public const byte VK_OEM_PLUS = 0xBB  ; // '+' any country;
        /// <summary>VK_OEM_COMMA constant.</summary>
        public const byte VK_OEM_COMMA = 0xBC  ; // ',' any country;
        /// <summary>VK_OEM_MINUS constant.</summary>
        public const byte VK_OEM_MINUS = 0xBD  ; // '-' any country;
        /// <summary>VK_OEM_PERIOD constant.</summary>
        public const byte VK_OEM_PERIOD = 0xBE  ; // '.' any country;
        /// <summary>VK_OEM_2 constant.</summary>
        public const byte VK_OEM_2 = 0xBF  ; // '/?' for US;
        /// <summary>VK_OEM_3 constant.</summary>
        public const byte VK_OEM_3 = 0xC0  ; // '`~' for US;

        // 0xC1 - 0xD7 : reserved

        // 0xD8 - 0xDA : unassigned

        /// <summary>VK_OEM_4 constant.</summary>
        public const byte VK_OEM_4 = 0xDB ; //  '[{' for US;
        /// <summary>VK_OEM_5 constant.</summary>
        public const byte VK_OEM_5 = 0xDC ; //  '\|' for US;
        /// <summary>VK_OEM_6 constant.</summary>
        public const byte VK_OEM_6 = 0xDD ; //  ']}' for US;
        /// <summary>VK_OEM_7 constant.</summary>
        public const byte VK_OEM_7 = 0xDE ; //  ''"' for US;
        /// <summary>VK_OEM_8 constant.</summary>
        public const byte VK_OEM_8 = 0xDF;

        // 0xE0 : reserved

        // Various extended or enhanced keyboards
        /// <summary>VK_OEM_AX constant.</summary>
        public const byte VK_OEM_AX = 0xE1 ; //  'AX' key on Japanese AX kbd;
        /// <summary>VK_OEM_102 constant.</summary>
        public const byte VK_OEM_102 = 0xE2 ; //  "<>" or "\|" on RT 102-key kbd.;
        /// <summary>VK_ICO_HELP constant.</summary>
        public const byte VK_ICO_HELP = 0xE3 ; //  Help key on ICO;
        /// <summary>VK_ICO_00 constant.</summary>
        public const byte VK_ICO_00 = 0xE4 ; //  00 key on ICO;

        /// <summary>VK_PROCESSKEY constant.</summary>
        public const byte VK_PROCESSKEY = 0xE5;
        /// <summary>VK_ICO_CLEAR constant.</summary>
        public const byte VK_ICO_CLEAR = 0xE6;
        /// <summary>VK_PACKET constant.</summary>
        public const byte VK_PACKET = 0xE7;

        // 0xE8 : unassigned

        // Nokia/Ericsson definitions
        /// <summary>VK_OEM_RESET constant.</summary>
        public const byte VK_OEM_RESET = 0xE9;
        /// <summary>VK_OEM_JUMP constant.</summary>
        public const byte VK_OEM_JUMP = 0xEA;
        /// <summary>VK_OEM_PA1 constant.</summary>
        public const byte VK_OEM_PA1 = 0xEB;
        /// <summary>VK_OEM_PA2 constant.</summary>
        public const byte VK_OEM_PA2 = 0xEC;
        /// <summary>VK_OEM_PA3 constant.</summary>
        public const byte VK_OEM_PA3 = 0xED;
        /// <summary>VK_OEM_WSCTRL constant.</summary>
        public const byte VK_OEM_WSCTRL = 0xEE;
        /// <summary>VK_OEM_CUSEL constant.</summary>
        public const byte VK_OEM_CUSEL = 0xEF;
        /// <summary>VK_OEM_ATTN constant.</summary>
        public const byte VK_OEM_ATTN = 0xF0;
        /// <summary>VK_OEM_FINISH constant.</summary>
        public const byte VK_OEM_FINISH = 0xF1;
        /// <summary>VK_OEM_COPY constant.</summary>
        public const byte VK_OEM_COPY = 0xF2;
        /// <summary>VK_OEM_AUTO constant.</summary>
        public const byte VK_OEM_AUTO = 0xF3;
        /// <summary>VK_OEM_ENLW constant.</summary>
        public const byte VK_OEM_ENLW = 0xF4;
        /// <summary>VK_OEM_BACKTAB constant.</summary>
        public const byte VK_OEM_BACKTAB = 0xF5;

        /// <summary>VK_ATTN constant.</summary>
        public const byte VK_ATTN = 0xF6;
        /// <summary>VK_CRSEL constant.</summary>
        public const byte VK_CRSEL = 0xF7;
        /// <summary>VK_EXSEL constant.</summary>
        public const byte VK_EXSEL = 0xF8;
        /// <summary>VK_EREOF constant.</summary>
        public const byte VK_EREOF = 0xF9;
        /// <summary>VK_PLAY constant.</summary>
        public const byte VK_PLAY = 0xFA;
        /// <summary>VK_ZOOM constant.</summary>
        public const byte VK_ZOOM = 0xFB;
        /// <summary>VK_NONAME constant.</summary>
        public const byte VK_NONAME = 0xFC;
        /// <summary>VK_PA1 constant.</summary>
        public const byte VK_PA1 = 0xFD;
        /// <summary>VK_OEM_CLEAR constant.</summary>
        public const byte VK_OEM_CLEAR = 0xFE;

        // 0xFF : reserved

        /// <summary>Lists all known virtual key codes.</summary>
        /// <returns>An array of all known virtual key codes.</returns>
        public static byte[] GetVirtualKeys()
        {
            return new byte[] {
                VK_LBUTTON,
                VK_RBUTTON,
                VK_CANCEL,
                VK_MBUTTON,
                VK_XBUTTON1,
                VK_XBUTTON2,
                VK_BACK,
                VK_TAB,
                VK_CLEAR,
                VK_RETURN,
                VK_SHIFT,
                VK_CONTROL,
                VK_MENU,
                VK_PAUSE,
                VK_CAPITAL,
                VK_KANA,
                VK_HANGEUL,
                VK_HANGUL,
                VK_JUNJA,
                VK_FINAL,
                VK_HANJA,
                VK_KANJI,
                VK_ESCAPE,
                VK_CONVERT,
                VK_NONCONVERT,
                VK_ACCEPT,
                VK_MODECHANGE,
                VK_SPACE,
                VK_PRIOR,
                VK_NEXT,
                VK_END,
                VK_HOME,
                VK_LEFT,
                VK_UP,
                VK_RIGHT,
                VK_DOWN,
                VK_SELECT,
                VK_PRINT,
                VK_EXECUTE,
                VK_SNAPSHOT,
                VK_INSERT,
                VK_DELETE,
                VK_HELP,
                VK_LWIN,
                VK_RWIN,
                VK_APPS,
                VK_SLEEP,
                VK_NUMPAD0,
                VK_NUMPAD1,
                VK_NUMPAD2,
                VK_NUMPAD3,
                VK_NUMPAD4,
                VK_NUMPAD5,
                VK_NUMPAD6,
                VK_NUMPAD7,
                VK_NUMPAD8,
                VK_NUMPAD9,
                VK_MULTIPLY,
                VK_ADD,
                VK_SEPARATOR,
                VK_SUBTRACT,
                VK_DECIMAL,
                VK_DIVIDE,
                VK_F1,
                VK_F2,
                VK_F3,
                VK_F4,
                VK_F5,
                VK_F6,
                VK_F7,
                VK_F8,
                VK_F9,
                VK_F10,
                VK_F11,
                VK_F12,
                VK_F13,
                VK_F14,
                VK_F15,
                VK_F16,
                VK_F17,
                VK_F18,
                VK_F19,
                VK_F20,
                VK_F21,
                VK_F22,
                VK_F23,
                VK_F24,
                VK_NUMLOCK,
                VK_SCROLL,
                VK_OEM_NEC_EQUAL,
                VK_OEM_FJ_JISHO,
                VK_OEM_FJ_MASSHOU,
                VK_OEM_FJ_TOUROKU,
                VK_OEM_FJ_LOYA,
                VK_OEM_FJ_ROYA,
                VK_LSHIFT,
                VK_RSHIFT,
                VK_LCONTROL,
                VK_RCONTROL,
                VK_LMENU,
                VK_RMENU,
                VK_BROWSER_BACK,
                VK_BROWSER_FORWARD,
                VK_BROWSER_REFRESH,
                VK_BROWSER_STOP,
                VK_BROWSER_SEARCH,
                VK_BROWSER_FAVORITES,
                VK_BROWSER_HOME,
                VK_VOLUME_MUTE,
                VK_VOLUME_DOWN,
                VK_VOLUME_UP,
                VK_MEDIA_NEXT_TRACK,
                VK_MEDIA_PREV_TRACK,
                VK_MEDIA_STOP,
                VK_MEDIA_PLAY_PAUSE,
                VK_LAUNCH_MAIL,
                VK_LAUNCH_MEDIA_SELECT,
                VK_LAUNCH_APP1,
                VK_LAUNCH_APP2,
                VK_OEM_1,
                VK_OEM_PLUS,
                VK_OEM_COMMA,
                VK_OEM_MINUS,
                VK_OEM_PERIOD,
                VK_OEM_2,
                VK_OEM_3,
                VK_OEM_4,
                VK_OEM_5,
                VK_OEM_6,
                VK_OEM_7,
                VK_OEM_8,
                VK_OEM_AX,
                VK_OEM_102,
                VK_ICO_HELP,
                VK_ICO_00,
                VK_PROCESSKEY,
                VK_ICO_CLEAR,
                VK_PACKET,
                VK_OEM_RESET,
                VK_OEM_JUMP,
                VK_OEM_PA1,
                VK_OEM_PA2,
                VK_OEM_PA3,
                VK_OEM_WSCTRL,
                VK_OEM_CUSEL,
                VK_OEM_ATTN,
                VK_OEM_FINISH,
                VK_OEM_COPY,
                VK_OEM_AUTO,
                VK_OEM_ENLW,
                VK_OEM_BACKTAB,
                VK_ATTN,
                VK_CRSEL,
                VK_EXSEL,
                VK_EREOF,
                VK_PLAY,
                VK_ZOOM,
                VK_NONAME,
                VK_PA1,
                VK_OEM_CLEAR
            };
        }

        #endregion Virtual key codes.

        #region Legacy Win32 UI

        // We haven't got every button type here
        // please add if you want to use them
        /// <summary>OK button</summary>
        public const int MB_OK                    = 0;

        /// <summary>OK / Cancel button</summary>
        public const int MB_OKCANCEL              = 1;

        /// <summary>Yes / No button</summary>
        public const int MB_YESNO                 = 4;


        /// <summary>pull in user32!MessageBox</summary>
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int MessageBox(HandleRef hWnd, string text, string caption, int type);

        /// <summary>
        /// A helper function to create a pop up box
        /// This gives us a chance to attach debugger to
        /// a process if we don't allow that to be started in
        /// the debugger (e.g. remote debugger on process startup)
        /// </summary>
        public static void CreateAttachDebuggerMessageBox()
        {
            new SecurityPermission(PermissionState.Unrestricted).Assert();

            int pid = GetCurrentProcessId();

            string message = String.Format("Please attach debugger to process {0}", pid.ToString());

            MessageBox(new HandleRef(null, IntPtr.Zero), message, "Attach debugger", MB_OK);
        }

        #endregion

        #region System metrics support.

        /// <summary>GetSystemMetrics wrapper.</summary>
        /// <param name="nIndex">Index of metric to get.</param>
        /// <returns>System value.</returns>
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(int nIndex);

        /// <summary>Width of the screen of the primary display monitor, in pixels.</summary>
        public const int SM_CXSCREEN = 0;
        /// <summary>Height of the screen of the primary display monitor, in pixels.</summary>
        public const int SM_CYSCREEN = 1;
        /// <summary>TRUE if the system is enabled for Hebrew and Arabic languages.</summary>
        public const int SM_MIDEASTENABLED = 74;
        /// <summary>Coordinates for the left side of the virtual screen.</summary>
        public const int SM_XVIRTUALSCREEN = 76;
        /// <summary>Coordinates for the top of the virtual screen.</summary>
        public const int SM_YVIRTUALSCREEN = 77;
        /// <summary>Width of the screen of the virtual screen, in pixels.</summary>
        public const int SM_CXVIRTUALSCREEN = 78;
        /// <summary>Height of the screen of the virtual screen, in pixels.</summary>
        public const int SM_CYVIRTUALSCREEN = 79;

        /// <summary>Scroll bar gray area.</summary>
        public const int COLOR_SCROLLBAR         = 0;
        /// <summary>Desktop.</summary>
        public const int COLOR_BACKGROUND        = 1;
        /// <summary>Active window title bar.</summary>
        /// <remarks>Specifies the left side color in the color gradient of an active window's title bar if the gradient effect is enabled.</remarks>
        public const int COLOR_ACTIVECAPTION     = 2;
        /// <summary>Inactive window caption.</summary>
        /// <remarks>Specifies the left side color in the color gradient of an inactive window's title bar if the gradient effect is enabled.</remarks>
        public const int COLOR_INACTIVECAPTION   = 3;
        /// <summary>Menu background.</summary>
        public const int COLOR_MENU              = 4;
        /// <summary>Window background.</summary>
        public const int COLOR_WINDOW            = 5;
        /// <summary>Window frame.</summary>
        public const int COLOR_WINDOWFRAME       = 6;
        /// <summary>Text in menus.</summary>
        public const int COLOR_MENUTEXT          = 7;
        /// <summary>Text in windows.</summary>
        public const int COLOR_WINDOWTEXT        = 8;
        /// <summary>Text in caption, size box, and scroll bar arrow box.</summary>
        public const int COLOR_CAPTIONTEXT       = 9;
        /// <summary>Active window border.</summary>
        public const int COLOR_ACTIVEBORDER      = 10;
        /// <summary>Inactive window border.</summary>
        public const int COLOR_INACTIVEBORDER    = 11;
        /// <summary>Background color of multiple document interface (MDI) applications.</summary>
        public const int COLOR_APPWORKSPACE      = 12;
        /// <summary>Item(s) selected in a control.</summary>
        public const int COLOR_HIGHLIGHT         = 13;
        /// <summary>Text of item(s) selected in a control.</summary>
        public const int COLOR_HIGHLIGHTTEXT     = 14;
        /// <summary>Face color for three-dimensional display elements and for dialog box backgrounds.</summary>
        public const int COLOR_BTNFACE           = 15;
        /// <summary>Shadow color for three-dimensional display elements (for edges facing away from the light source).</summary>
        public const int COLOR_BTNSHADOW         = 16;
        /// <summary>Grayed (disabled) text.</summary>
        public const int COLOR_GRAYTEXT          = 17;
        /// <summary>Text on push buttons.</summary>
        public const int COLOR_BTNTEXT           = 18;
        /// <summary>Color of text in an inactive caption.</summary>
        public const int COLOR_INACTIVECAPTIONTEXT = 19;
        /// <summary>Highlight color for three-dimensional display elements (for edges facing the light source.)</summary>
        public const int COLOR_BTNHIGHLIGHT      = 20;

        /// <summary>Dark shadow for three-dimensional display elements.</summary>
        public const int COLOR_3DDKSHADOW        = 21;
        /// <summary>Light color for three-dimensional display elements (for edges facing the light source.)</summary>
        public const int COLOR_3DLIGHT           = 22;
        /// <summary>Text color for tooltip controls.</summary>
        public const int COLOR_INFOTEXT          = 23;
        /// <summary>Background color for tooltip controls.</summary>
        public const int COLOR_INFOBK            = 24;

        /// <summary>Color for a hot-tracked item.</summary>
        public const int COLOR_HOTLIGHT          = 26;
        /// <summary>Right side color in the color gradient of an active window's title bar. COLOR_ACTIVECAPTION specifies the left side color.</summary>
        public const int COLOR_GRADIENTACTIVECAPTION = 27;
        /// <summary>Right side color in the color gradient of an inactive window's title bar. COLOR_INACTIVECAPTION specifies the left side color.</summary>
        public const int COLOR_GRADIENTINACTIVECAPTION = 28;
        /// <summary>The color used to highlight menu items when the menu appears as a flat menu.</summary>
        public const int COLOR_MENUHILIGHT       = 29;
        /// <summary>The background color for the menu bar when menus appear as flat menus.</summary>
        public const int COLOR_MENUBAR           = 30;

        /// <summary>Desktop.</summary>
        public const int COLOR_DESKTOP           = COLOR_BACKGROUND;
        /// <summary>Face color for three-dimensional display elements and for dialog box backgrounds.</summary>
        public const int COLOR_3DFACE            = COLOR_BTNFACE;
        /// <summary>Shadow color for three-dimensional display elements (for edges facing away from the light source).</summary>
        public const int COLOR_3DSHADOW          = COLOR_BTNSHADOW;
        /// <summary>Highlight color for three-dimensional display elements (for edges facing the light source.)</summary>
        public const int COLOR_3DHIGHLIGHT       = COLOR_BTNHIGHLIGHT;
        /// <summary>Highlight color for three-dimensional display elements (for edges facing the light source.)</summary>
        public const int COLOR_3DHILIGHT         = COLOR_BTNHIGHLIGHT;
        /// <summary>Highlight color for three-dimensional display elements (for edges facing the light source.)</summary>
        public const int COLOR_BTNHILIGHT        = COLOR_BTNHIGHLIGHT;

        /// <summary>GetSysColor wrapper.</summary>
        /// <param name="nIndex">Index of metric to get.</param>
        /// <returns>System value.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern uint GetSysColor(int nIndex);

        /// <summary>GetSysColor wrapper.</summary>
        /// <param name="nIndex">Index of metric to get.</param>
        /// <returns>System value.</returns>
        public static uint SafeGetSysColor(int nIndex)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return GetSysColor(nIndex);
        }

        /// <summary>Sets the colors for one or more display elements.</summary>
        /// <param name="cElements">Number of display elements in the lpaElements array.</param>
        /// <param name="lpaElements">Array of integers that specify the display elements to be changed.</param>
        /// <param name="lpaRgbValues">Array of COLORREF values that contain the new red, green, blue (RGB) color values for the display elements in the array pointed to by the lpaElements parameter.</param>
        /// <returns>System value.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError=true)]
        private static extern bool SetSysColors(int cElements, int[] lpaElements,
            uint[] lpaRgbValues);


        /// <summary>Sets the colors for one or more display elements.</summary>
        /// <param name="cElements">Number of display elements in the lpaElements array.</param>
        /// <param name="lpaElements">Array of integers that specify the display elements to be changed.</param>
        /// <param name="lpaRgbValues">Array of COLORREF values that contain the new red, green, blue (RGB) color values for the display elements in the array pointed to by the lpaElements parameter.</param>
        /// <returns>System value.</returns>
        public static bool SafeSetSysColors(int cElements, int[] lpaElements,
            uint[] lpaRgbValues)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return SetSysColors(cElements, lpaElements, lpaRgbValues);
        }

        #endregion System metrics support.

        #region Error handling.

        /// <summary>This function returns the calling thread's last-error code value.</summary>
        /// <returns>The calling thread's last-error code value indicating success.</returns>
        [DllImport("kernel32.dll")]
        public static extern int GetLastError();

        #endregion Error handling.

        #region Keyboard support.

        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        private static extern int GetKeyboardLayoutList(int size, [Out, MarshalAs(UnmanagedType.LPArray)] IntPtr [] hkls);

        /// <summary>
        /// Retrieves the input locale identifiers (formerly called keyboard
        /// layout handles) corresponding to the current set of input locales
        /// in the system.
        /// </summary>
        /// <param name='size'>Maximum number of handles that the buffer can hold.</param>
        /// <param name='hkls'>Buffer that receives the array of input locale identifiers.</param>
        /// <returns>
        /// If the function succeeds, the return value is the number of
        /// input locale identifiers copied to the buffer or, if nBuff is
        /// zero, the return value is the size, in array elements, of the
        /// buffer needed to receive all current input locale identifiers.
        /// </returns>
        public static int SafeGetKeyboardLayoutList(int size, IntPtr[] hkls)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return GetKeyboardLayoutList(size, hkls);
        }

        /// <summary>This function returns a 256 entries byte about the virtual key state</summary>
        /// <returns>1 means success, failure otherwise.</returns>
        [DllImport ("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetKeyboardState(byte[] byteKeyboardState);

        /// <summary>
        /// Copies the status of the 256 virtual keys to the specified buffer.
        /// </summary>
        /// <param name='byteKeyboardState'>
        /// 256-byte array that receives the status data for each virtual key.
        /// </param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool SafeGetKeyboardState(byte[] byteKeyboardState)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return GetKeyboardState(byteKeyboardState);
        }

        [DllImport ("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool SetKeyboardState(byte[] byteKeyboardState);

        /// <summary>
        /// Copies a 256-byte array of keyboard key states into the calling
        /// thread's keyboard input-state table. This is the same table
        /// accessed by the GetKeyboardState and GetKeyState functions.
        /// Changes made to this table do not affect keyboard input to any
        /// other thread.
        /// </summary>
        /// <param name='byteKeyboardState'>
        /// A 256-byte array that contains keyboard key states.
        /// </param>
        /// <returns>true if successful, false otherwise.</returns>
        public static bool SafeSetKeyboardState(byte[] byteKeyboardState)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return SetKeyboardState(byteKeyboardState);
        }


        /// <summary>
        /// user32 function GetKeyboardLayout
        /// supplying threadID = 0 means the current thread
        /// </summary>
        /// <param name="threadID">thread id, 0 means current thread</param>
        /// <returns>return hkl for the thread</returns>
        [DllImport ("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetKeyboardLayout(IntPtr threadID);

        /// <summary>
        /// user32 function GetKeyboardLayout
        /// supplying threadID = 0 means the current thread
        /// </summary>
        /// <param name="threadID">thread id, 0 means current thread</param>
        /// <returns>return hkl for the thread</returns>
        public static IntPtr SafeGetKeyboardLayout(IntPtr threadID)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return GetKeyboardLayout(threadID);
        }

        /// <summary>
        /// Loads a new input locale identifier (formerly called the
        /// keyboard layout) into the system.
        /// </summary>
        [DllImport ("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr LoadKeyboardLayout(string pwszKLID, int flags);

        /// <summary>
        /// Loads a new input locale identifier (formerly called the
        /// keyboard layout) into the system.
        /// </summary>
        /// <param name="pwszKLID">A string composed of the hexadecimal value of the Language Identifier (low word) and a device identifier (high word).</param>
        /// <param name="flags">Specifies how the input locale identifier is to be loaded.</param>
        /// <returns>
        /// The input locale identifier to the locale matched with the
        /// requested name. If no matching locale is available, the return
        /// value is IntPtr.Zero.
        /// </returns>
        public static IntPtr SafeLoadKeyboardLayout(string pwszKLID, int flags)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return LoadKeyboardLayout(pwszKLID, flags);
        }

        /// <summary>
        /// Sets the input locale identifier (formerly called the keyboard
        /// layout handle) for the calling thread or the current process.
        /// </summary>
        [DllImport("user32.dll", ExactSpelling=true, CharSet=CharSet.Auto, SetLastError=true)]
        private static extern IntPtr ActivateKeyboardLayout(IntPtr hkl, int uFlags);

        /// <summary>
        /// Sets the input locale identifier (formerly called the keyboard
        /// layout handle) for the calling thread or the current process.
        /// </summary>
        /// <param name="hkl">Input locale identifier to be activated.</param>
        /// <param name="flags">Specifies how the input locale identifier is to be activated.</param>
        /// <returns>
        /// If the function succeeds, the return value is the previous input
        /// locale identifier. Otherwise, it is IntPtr.Zero.
        /// </returns>
        public static IntPtr SafeActivateKeyboardLayout(IntPtr hkl, int flags)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return ActivateKeyboardLayout(hkl, flags);
        }

        /// <summary>
        /// This allows retrieving scancode for a virutal key. This takes keyboard layout / locale into account
        /// </summary>
        /// <param name="nVirtKey">virtual key</param>
        /// <param name="nMapType">map type</param>
        /// <param name="hkl">hkl for the virtual key to be interpreted</param>
        /// <returns></returns>
        [DllImport ("user32.dll", CharSet = CharSet.Auto)]
        public static extern uint MapVirtualKeyEx (uint nVirtKey, int nMapType, IntPtr hkl);

        /// <summary>
        /// Translates a character to the corresponding virtual-key code and shift state,
        /// using the input language and physical keyboard layout identified by
        /// the input locale identifier.
        /// </summary>
        /// <param name='ch'>Specifies the character to be translated into a virtual-key code.</param>
        /// <param name='hkl'>
        /// Input locale identifier used to translate the character.
        /// This parameter can be any input locale identifier previously
        /// returned by the LoadKeyboardLayout function.
        /// </param>
        /// <returns>If the function succeeds, the low-order byte of the return value
        /// contains the virtual-key code and the high-order byte contains the shift state
        /// (1=shift, 2=ctrl, 4=alt, 8=hankaku, 16 and 32 reserved).</returns>
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern ushort VkKeyScanEx(char ch, IntPtr hkl);

        /// <summary>
        /// Blocks keyboard and mouse input events from reaching applications.
        /// </summary>
        /// <param name="fBlockIt">
        /// If this parameter is TRUE, keyboard and mouse input events are
        /// blocked. If this parameter is FALSE, keyboard and mouse events
        /// are unblocked. Note that only the thread that blocked input can
        /// successfully unblock input.
        /// </param>
        /// <returns>Nonzero if the function succeeds, zero if already blocked.</returns>
        /// <remarks>
        /// When input is blocked, real physical input from the mouse or
        /// keyboard will not affect the input queue's synchronous key state
        /// (reported by GetKeyState and GetKeyboardState), nor will it
        /// affect the asynchronous key state (reported by GetAsyncKeyState).
        /// However, the thread that is blocking input can affect both of
        /// these key states by calling SendInput. No other thread can do
        /// this.
        /// </remarks>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int BlockInput(bool fBlockIt);

        /// <summary>
        /// Makes a safe call to BlockInput.
        /// </summary>
        /// <param name="blockIt">Whether to block or unblock input.</param>
        /// <returns>Nonzero on success, zero otherwise.</returns>
        public static int SafeBlockInput(bool blockIt)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return BlockInput(blockIt);
        }

        #endregion Keyboard support.

        #region Caret support.

        /// <summary>
        /// Sets the time required to invert the caret's pixels in milliseconds.
        /// </summary>
        /// <param name="val">Milliseconds between caret inversions.</param>
        /// <returns>true on success, false otherwise.</returns>
        [DllImport("user32.dll")]
        public static extern bool SetCaretBlinkTime(int val);

        /// <summary>
        /// Returns the time required to invert the caret's pixels in
        /// milliseconds.
        /// </summary>
        /// <returns>Milliseconds requierd to invert caret, zero if error.</returns>
        [DllImport("user32.dll")]
        public static extern int GetCaretBlinkTime();

        /// <summary>
        /// Returns nonzero if the function succeeds, else returns zero
        /// </summary>
        /// <returns>Non zero value if function succeeds else zero</returns>
        [DllImport("user32.dll")]
        public static extern bool GetCaretPos(out POINT point);

        /// <summary>
        /// The SetCaretPos function moves the caret to the specified coordinates. 
        /// If the window that owns the caret was created with the CS_OWNDC class style, 
        /// then the specified coordinates are subject to the mapping mode of the device 
        /// context associated with that window.        
        /// </summary>
        /// <returns>Non zero value if function succeeds else zero</returns>
        [DllImport("user32.dll")]
        public static extern bool SetCaretPos(int x, int y);

        /// <summary>Retrieves the caret width in edit controls, in pixels.</summary>
        /// <remarks>
        /// The pvParam parameter must point to a DWORD that receives this
        /// value.
        /// </remarks>
        public static int GetCaretWidthInPixels()
        {
            int result;
            uint width;

            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            width = 0;
            if (!SystemParametersInfo(SPI_GETCARETWIDTH, 0, ref width, 0))
            {
                throw new System.ComponentModel.Win32Exception();
            }
            result = (int) width;
            return result;
        }

        /// <summary>Sets the caret width in edit controls.</summary>
        /// <param name="width">Caret width.</param>
        /// <param name="updateIni">
        /// Whether the user profile should be updated with the new value.
        /// </param>
        /// <remarks>
        /// Sets pvParam to the desired width, in pixels. The default
        /// and minimum value is 1.
        /// </remarks>
        public static void SetCaretWidthInPixels(int width, bool updateIni)
        {
            int updateFlag;
            uint caretWidth;

            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            updateFlag = updateIni? SPIF_SENDCHANGE | SPIF_UPDATEINIFILE : 0;
            caretWidth = (uint)width;
            if (!SystemParametersInfo(SPI_SETCARETWIDTH, 0, caretWidth, updateFlag))
            {
                throw new System.ComponentModel.Win32Exception();
            }
        }

        #endregion Caret support.

        #region Clipboard support.

        #region Clipboard format constants.

        // From winuser.h.

        /// <summary>Clipboard format.</summary>
        public const uint CF_TEXT = 1;
        /// <summary>Clipboard format.</summary>
        public const uint CF_BITMAP = 2;
        /// <summary>Clipboard format.</summary>
        public const uint CF_METAFILEPICT = 3;
        /// <summary>Clipboard format.</summary>
        public const uint CF_SYLK = 4;
        /// <summary>Clipboard format.</summary>
        public const uint CF_DIF = 5;
        /// <summary>Clipboard format.</summary>
        public const uint CF_TIFF = 6;
        /// <summary>Clipboard format.</summary>
        public const uint CF_OEMTEXT = 7;
        /// <summary>Clipboard format.</summary>
        public const uint CF_DIB = 8;
        /// <summary>Clipboard format.</summary>
        public const uint CF_PALETTE = 9;
        /// <summary>Clipboard format.</summary>
        public const uint CF_PENDATA = 10;
        /// <summary>Clipboard format.</summary>
        public const uint CF_RIFF = 11;
        /// <summary>Clipboard format.</summary>
        public const uint CF_WAVE = 12;
        /// <summary>Clipboard format.</summary>
        public const uint CF_UNICODETEXT = 13;
        /// <summary>Clipboard format.</summary>
        public const uint CF_ENHMETAFILE = 14;
        /// <summary>Clipboard format.</summary>
        public const uint CF_HDROP = 15;
        /// <summary>Clipboard format.</summary>
        public const uint CF_LOCALE = 16;
        /// <summary>Clipboard format.</summary>
        public const uint CF_DIBV5 = 17;

        /// <summary>Clipboard format.</summary>
        public const uint CF_OWNERDISPLAY = 0x0080;
        /// <summary>Clipboard format.</summary>
        public const uint CF_DSPTEXT = 0x0081;
        /// <summary>Clipboard format.</summary>
        public const uint CF_DSPBITMAP = 0x0082;
        /// <summary>Clipboard format.</summary>
        public const uint CF_DSPMETAFILEPICT = 0x0083;
        /// <summary>Clipboard format.</summary>
        public const uint CF_DSPENHMETAFILE = 0x008E;

        // "Private" formats don't get GlobalFree()'d
        /// <summary>Clipboard format.</summary>
        public const uint CF_PRIVATEFIRST = 0x0200;
        /// <summary>Clipboard format.</summary>
        public const uint CF_PRIVATELAST = 0x02FF;

        // "GDIOBJ" formats do get DeleteObject()'d
        /// <summary>Clipboard format.</summary>
        public const uint CF_GDIOBJFIRST = 0x0300;
        /// <summary>Clipboard format.</summary>
        public const uint CF_GDIOBJLAST = 0x03FF;

        #endregion Clipboard format constants.

        /// <summary>Closes the clipboard.</summary>
        /// <returns>true if successfull, false otherwise.</returns>
        [DllImport("user32.dll",ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern bool CloseClipboard();

        /// <summary>Closes the clipboard.</summary>
        /// <returns>true if successfull, false otherwise.</returns>
        public static bool SafeCloseClipboard()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return CloseClipboard();
        }

        /// <summary>Retrieves the number of different data formats currently on the clipboard.</summary>
        /// <returns>The number of formats if it succeeds, 0 otherwise.</returns>
        [DllImport("user32.dll",ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern int CountClipboardFormats();

        /// <summary>Empties the clipboard and frees handles to data in the clipboard.</summary>
        /// <returns>true if successfull, false otherwise.</returns>
        [DllImport("user32.dll",ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern bool EmptyClipboard();

        /// <summary>Empties the clipboard and frees handles to data in the clipboard.</summary>
        /// <returns>true if successfull, false otherwise.</returns>
        public static bool SafeEmptyClipboard()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return EmptyClipboard();
        }

        /// <summary>Retrieves data from the clipboard in a specified format.</summary>
        /// <param name="format">Type of format to be retrieved.</param>
        /// <returns>Handle to data if successfull, 0 otherwise.</returns>
        [DllImport("user32.dll",ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern IntPtr GetClipboardData(uint format);

        /// <summary>Retrieves from the clipboard the name of the specified registered format.</summary>
        /// <param name="format">Type of format to be retrieved.</param>
        /// <param name="formatName">Pointer to the buffer that is to receive the format name.</param>
        /// <param name="maxCount">Maximum length, in characters, of the string to be copied to the buffer.</param>
        /// <returns>The length, in characters, of the string copied to the buffer indicates success.</returns>
        [DllImport("user32.dll")]
        public static extern int GetClipboardFormatName(uint format, IntPtr formatName, int maxCount);

        /// <summary>Enumerate the data formats that are currently available on the clipboard.</summary>
        /// <param name="format">Unsigned integer that specifies a clipboard format that is known
        /// to be available.</param>
        /// <returns>0 for failure or no more items, otherwise next item for format.</returns>
        [DllImport("user32.dll",ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern uint EnumClipboardFormats(uint format);

        /// <summary>Opens the clipboard for examination and prevents other applications from modifying the clipboard content.</summary>
        /// <param name="hwndNewOwner">Handle to the window to be associated with the open clipboard. If this parameter is NULL, the open clipboard is associated with the current task.</param>
        /// <returns>true if successfull, false otherwise.</returns>
        [DllImport("user32.dll",ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern bool OpenClipboard(HWND hwndNewOwner);

        /// <summary>Opens the clipboard for examination and prevents other applications from modifying the clipboard content.</summary>
        /// <param name="hwndNewOwner">Handle to the window to be associated with the open clipboard. If this parameter is NULL, the open clipboard is associated with the current task.</param>
        /// <returns>true if successfull, false otherwise.</returns>
        public static bool SafeOpenClipboard(HWND hwndNewOwner)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return OpenClipboard(hwndNewOwner);
        }

        /// <summary>Registers a new clipboard format.</summary>
        /// <param name="str">Pointer to a null-terminated string that names the new format.</param>
        /// <returns>0 if it fails, the registered format otherwise.</returns>
        [DllImport("user32.dll")]
        public static extern int RegisterClipboardFormat(string str);

        /// <summary>Places data on the clipboard in a specified clipboard format.</summary>
        /// <param name="uFormat">Specifies a clipboard format.</param>
        /// <param name="hMem">Handle to the data in the specified format.</param>
        /// <returns>The handle of the data indicates success. NULL indicates failure.</returns>
        [DllImport("user32.dll",ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern IntPtr SetClipboardData(uint uFormat,  IntPtr hMem);

        /// <summary>Places data on the clipboard in a specified clipboard format.</summary>
        /// <param name="uFormat">Specifies a clipboard format.</param>
        /// <param name="hMem">Handle to the data in the specified format.</param>
        /// <returns>The handle of the data indicates success. NULL indicates failure.</returns>
        public static IntPtr SafeSetClipboardData(uint uFormat,  IntPtr hMem)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return SetClipboardData(uFormat, hMem);
        }

        #endregion Clipboard support.

        #region Cursor support.

        /// <summary>Win32 constants</summary>
        public const int IDC_ARROW       = 32512;
        /// <summary>Win32 constants</summary>
        public const int IDC_IBEAM       = 32513;
        /// <summary>Win32 constants</summary>
        public const int IDC_WAIT        = 32514;
        /// <summary>Win32 constants</summary>
        public const int IDC_CROSS       = 32515;
        /// <summary>Win32 constants</summary>
        public const int IDC_UPARROW     = 32516;
        /// <summary>Win32 constants</summary>
        public const int IDC_SIZE        = 32640;
        /// <summary>Win32 constants</summary>
        public const int IDC_ICON        = 32641;
        /// <summary>Win32 constants</summary>
        public const int IDC_SIZENWSE    = 32642;
        /// <summary>Win32 constants</summary>
        public const int IDC_SIZENESW    = 32643;
        /// <summary>Win32 constants</summary>
        public const int IDC_SIZEWE      = 32644;
        /// <summary>Win32 constants</summary>
        public const int IDC_SIZENS      = 32645;
        /// <summary>Win32 constants</summary>
        public const int IDC_SIZEALL     = 32646;
        /// <summary>Win32 constants</summary>
        public const int IDC_NO          = 32648;
        /// <summary>Win32 constants</summary>
        public const int IDC_HAND        = 32649;
        /// <summary>Win32 constants</summary>
        public const int IDC_APPSTARTING = 32650;
        /// <summary>Win32 constants</summary>
        public const int IDC_HELP        = 32651;

        /// <summary>Represents a handle to a cursor.</summary>
        [Serializable]
        public struct HCURSOR
        {
            /// <summary>Handle.</summary>
            public IntPtr h;

            /// <summary>Creates a wrapped HCURSOR from an unmanaged handle.</summary>
            /// <param name='hCursor'>Handle to wrap.</param>
            public HCURSOR(IntPtr hCursor)
            {
                this.h = hCursor;
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="hl"></param>
            /// <param name="hr"></param>
            /// <returns></returns>
            public static bool operator==(HCURSOR hl, HCURSOR hr)
            {
                return (hl.h == hr.h);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="hl"></param>
            /// <param name="hr"></param>
            /// <returns></returns>
            public static bool operator!=(HCURSOR hl, HCURSOR hr)
            {
                return (hl.h != hr.h);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="oCompare"></param>
            /// <returns></returns>
            override public bool Equals(object oCompare)
            {
                HCURSOR hr = Cast((IntPtr)oCompare);
                return (h == hr.h);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="h"></param>
            /// <returns></returns>
            public static HCURSOR Cast(IntPtr h)
            {
                HCURSOR hTemp = new HCURSOR();
                hTemp.h = h;
                return hTemp;
            }

            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return (int) h;
            }

            /// <summary>Returns the string representation for the cursor.</summary>
            /// <returns>The string representation for the cursor handle.</returns>
            public override string ToString()
            {
                return h.ToString();
            }
        }

        /// <summary>Loads a cursor given the ID for the cursor name.</summary>
        /// <param name="hinstance"></param>
        /// <param name="nCursorName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern HCURSOR LoadCursor(HINSTANCE hinstance, int nCursorName);

        /// <summary>Loads a cursor given the cursor name.</summary>
        /// <param name="hinstance"></param>
        /// <param name="stCursorName"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern HCURSOR LoadCursor(HINSTANCE hinstance, string stCursorName);

        /// <summary>Retrieves a handle to the current cursor.</summary>
        /// <returns>
        /// The handle to the current cursor. If there is no cursor, the
        /// return value is NULL.
        /// </returns>
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern HCURSOR GetCursor();

        /// <summary>Retrieves a handle to the current cursor.</summary>
        /// <returns>
        /// The handle to the current cursor. If there is no cursor, the
        /// return value is NULL.
        /// </returns>
        public static HCURSOR SafeGetCursor()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return GetCursor();
        }

        #endregion Cursor support.

        #region Memory management.

        /// <summary>Allocates the specified number of bytes from the heap.</summary>
        /// <param name="uFlags">Memory allocation attributes.</param>
        /// <param name="dwBytes">Number of bytes to allocate.</param>
        /// <returns>A handle to the allocated memory object, NULL otherwise.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalAlloc(uint uFlags, /* SIZE_T */ int dwBytes);

        /// <summary>Frees the specified global memory object and invalidates its handle.</summary>
        /// <param name="hMem">Handle to the global memory object.</param>
        /// <returns>NULLl if the function succeeds, a handle to the object otherwise.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalFree(IntPtr hMem);

        /// <summary>Locks a global memory object and returns a pointer to the first byte of the object's memory block.</summary>
        /// <param name="hMem">Handle to the global memory object.</param>
        /// <returns>A pointer to the locked memory on success, NULL otherwise.</returns>
        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        /// <summary>Locks a global memory object and returns a pointer to the first byte of the object's memory block.</summary>
        /// <param name="hMem">Handle to the global memory object.</param>
        /// <returns>A pointer to the locked memory on success, NULL otherwise.</returns>
        public static IntPtr SafeGlobalLock(IntPtr hMem)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return GlobalLock(hMem);
        }

        /// <summary>Decrements the lock count associated with a memory object that was allocated with GMEM_MOVEABLE.</summary>
        /// <param name="hMem">Handle to the global memory object.</param>
        /// <returns>true if the object is still locked, false otherwise (false and NO_ERROR in GetLastError means
        /// last release succeeded).</returns>
        [DllImport("kernel32.dll")]
        public static extern bool GlobalUnlock(IntPtr hMem);

        /// <summary>Decrements the lock count associated with a memory object that was allocated with GMEM_MOVEABLE.</summary>
        /// <param name="hMem">Handle to the global memory object.</param>
        /// <returns>true if the object is still locked, false otherwise (false and NO_ERROR in GetLastError means
        /// last release succeeded).</returns>
        public static bool SafeGlobalUnlock(IntPtr hMem)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return GlobalUnlock(hMem);
        }

        /// <summary>Initializes a new instance of the String class to the value indicated by a pointer to an array of 8-bit signed integers.</summary>
        /// <param name="memory">A pointer to a null terminated array of 8-bit signed integers.</param>
        /// <param name="isUtf16">Whether the memory points to UTF16-encoded text.</param>
        /// <returns>If value is a null pointer, the new instance is initialized to the empty string ("").</returns>
        public static string ReadNullTerminatedString(IntPtr memory, bool isUtf16)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return UnsafeReadNullTerminatedString(memory, isUtf16);
        }

        /// <summary>Initializes a new instance of the String class to the value indicated by a pointer to an array of 8-bit signed integers.</summary>
        /// <param name="memory">A pointer to a null terminated array of 8-bit signed integers.</param>
        /// <param name="isUtf16">Whether the memory points to UTF16-encoded text.</param>
        /// <returns>If value is a null pointer, the new instance is initialized to the empty string ("").</returns>
        private static unsafe string UnsafeReadNullTerminatedString(IntPtr memory, bool isUtf16)
        {
            if (memory == IntPtr.Zero)
            {
                return "";
            }
            if (isUtf16)
            {
                return new string((char*)memory);
            }
            else
            {
                return new string((sbyte*)memory);
            }
        }

        #endregion Memory management.

        #region Device contexts and GDI
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

        //Ternary ROP codes
        /// <summary>BLACKNESS raster operation code.</summary>
        public const int BLACKNESS      = 0x00000042;
        /// <summary>NOTSRCERASE raster operation code.</summary>
        public const int NOTSRCERASE    = 0x001100A6;
        /// <summary>NOTSRCCOPY raster operation code.</summary>
        public const int NOTSRCCOPY     = 0x00330008;
        /// <summary>SRCERASE raster operation code.</summary>
        public const int SRCERASE       = 0x00440328;
        /// <summary>DSTINVERT raster operation code.</summary>
        public const int DSTINVERT      = 0x00550009;
        /// <summary>PATINVERT raster operation code.</summary>
        public const int PATINVERT      = 0x005A0049;
        /// <summary>SRCINVERT raster operation code.</summary>
        public const int SRCINVERT      = 0x00660046;
        /// <summary>SRCAND raster operation code.</summary>
        public const int SRCAND         = 0x008800C6;
        /// <summary>MERGEPAINT raster operation code.</summary>
        public const int MERGEPAINT     = 0x00BB0226;
        /// <summary>MERGECOPY raster operation code.</summary>
        public const int MERGECOPY      = 0x00C000CA;
        /// <summary>SRCCOPY raster operation code.</summary>
        public const int SRCCOPY        = 0x00CC0020;
        /// <summary>SRCPAINT raster operation code.</summary>
        public const int SRCPAINT       = 0x00EE0086;
        /// <summary>PATCOPY raster operation code.</summary>
        public const int PATCOPY        = 0x00F00021;
        /// <summary>PATPAINT raster operation code.</summary>
        public const int PATPAINT       = 0x00FB0A09;
        /// <summary>WHITENESS raster operation code.</summary>
        public const int WHITENESS      = 0x00FF0062;

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

            /// <summary>Creates a new Test.Uis.Wrappers.Win32.RECT instance.</summary>
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

            /// <summary>Creates a new Test.Uis.Wrappers.Win32.RECT instance.</summary>
            /// <param name="rcSrc">Specifies the rectangle to copy.</param>
            public RECT(RECT rcSrc)
            {
                this.left   = rcSrc.left;
                this.top    = rcSrc.top;
                this.right  = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            /// <summary>Whether the rectangle has any area.</summary>
            public bool IsEmpty
            {
                get
                {
                    return left >= right || top >= bottom;
                }
            }

            /// <summary>Calculated height.</summary>
            public int Height { get { return bottom - top; } }
            /// <summary>Calculated width.</summary>
            public int Width { get { return right - left; } }
        }

        /// <summary>Defines the x- and y- coordinates of a point.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            /// <summary>Specifies the x-coordinate of the point.</summary>
            public int x;
            /// <summary>Specifies the x-coordinate of the point.</summary>
            public int y;

            /// <summary>Creates a new Test.Uis.Wrappers.Win32.POINT instance.</summary>
            /// <param name="x">Specifies the x-coordinate of the point.</param>
            /// <param name="y">Specifies the y-coordinate of the point.</param>
            public POINT(int x, int y)
            {
                this.x  = x;
                this.y  = y;
            }
        }

        /// <summary>Retrieves the coordinates of a window's client area.</summary>
        /// <param name="hwnd">Handle to the window whose client coordinates are to be retrieved.</param>
        /// <param name="rc">RECT structure that receives the client coordinates.</param>
        /// <returns>true if the function succeeds, false otherwise.</returns>
        [DllImport("user32.dll")]
        public static extern bool GetClientRect(
            IntPtr hwnd, [In, Out] ref RECT rc);

        /// <summary>Retrieves the dimensions of the bounding rectangle of the specified window.</summary>
        /// <param name="hwnd">Handle to the window whose dimensions are to be retrieved.</param>
        /// <param name="rc">RECT structure that receives the coordinates.</param>
        /// <returns>true if the function succeeds, false otherwise.</returns>
        /// <remarks>The dimensions are given in screen coordinates that are relative to the upper-left corner of the screen.</remarks>
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(
            IntPtr hwnd, [In, Out] ref RECT rc);

        /// <summary>Converts the client-area coordinates of a specified point to screen coordinates.</summary>
        /// <param name="hwndFrom">Handle to the window whose client area is used for the conversion.</param>
        /// <param name="pt">POINT structure that contains the client coordinates to be converted.</param>
        /// <returns>true if the function succeeds, false otherwise.</returns>
        [DllImport("user32.dll")]
        public static extern bool ClientToScreen(
            IntPtr hwndFrom, [In, Out] ref POINT pt);

        /// <summary>Converts the client-area coordinates of a specified point to screen coordinates.</summary>
        /// <param name="hwndFrom">Handle to the window whose client area is used for the conversion.</param>
        /// <param name="pt">POINT structure that contains the client coordinates to be converted.</param>
        /// <returns>true if the function succeeds, false otherwise.</returns>
        public static bool SafeClientToScreen(IntPtr hwndFrom, ref POINT pt)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return ClientToScreen(hwndFrom, ref pt);
        }

        /// <summary>converts the screen coordinates of a specified point on the screen to client-area coordinates.</summary>
        /// <param name="hwndFrom">Handle to the window whose client area is used for the conversion.</param>
        /// <param name="pt">POINT structure that contains the screen coordinates to be converted.</param>
        /// <returns>true if the function succeeds, false otherwise.</returns>
        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(
            IntPtr hwndFrom, [In, Out] ref POINT pt);

        #endregion Device contexts and GDI.

        #region OLE support.

        #region OLE constants.

        /// <summary>Success.</summary>
        public const int S_OK = 0;
        /// <summary>Success, but 'false' in context of call.</summary>
        public const int S_FALSE = 1;
        /// <summary>Method not implemented.</summary>
        public const int E_NOTIMPL = unchecked((int)0x80004001);
        /// <summary>Ran out of memory.</summary>
        public const int E_OUTOFMEMORY = unchecked((int)0x8007000E);
        /// <summary>One or more arguments are invalid.</summary>
        public const int E_INVALIDARG = unchecked((int)0x80070057);
        /// <summary>No such interface supported.</summary>
        public const int E_NOINTERFACE = unchecked((int)0x80004002);
        /// <summary>Invalid pointer.</summary>
        public const int E_POINTER = unchecked((int)0x80004003);
        /// <summary>Unspecified error.</summary>
        public const int E_FAIL = unchecked((int)0x80004005);
        /// <summary>Data has same FORMATETC.</summary>
        public const int DATA_S_SAMEFORMATETC = unchecked((int)0x00040130);

        #region FACILITY_STORAGE error codes.

        /// <summary>Unable to perform requested operation.</summary>
        private const int STG_E_INVALIDFUNCTION = unchecked((int)0x80030001);
        /// <summary>File could not be found.</summary>
        private const int STG_E_FILENOTFOUND = unchecked((int)0x80030002);
        /// <summary>The path could not be found.</summary>
        private const int STG_E_PATHNOTFOUND = unchecked((int)0x80030003);
        /// <summary>There are insufficient resources to open another file.</summary>
        private const int STG_E_TOOMANYOPENFILES = unchecked((int)0x80030004);
        /// <summary>Access Denied.</summary>
        private const int STG_E_ACCESSDENIED = unchecked((int)0x80030005);
        /// <summary>Attempted an operation on an invalid object.</summary>
        private const int STG_E_INVALIDHANDLE = unchecked((int)0x80030006);
        /// <summary>There is insufficient memory available to complete operation.</summary>
        private const int STG_E_INSUFFICIENTMEMORY = unchecked((int)0x80030008);
        /// <summary>Invalid pointer error.</summary>
        private const int STG_E_INVALIDPOINTER = unchecked((int)0x80030009);
        /// <summary>There are no more entries to return.</summary>
        private const int STG_E_NOMOREFILES = unchecked((int)0x80030012);
        /// <summary>Disk is write-protected.</summary>
        private const int STG_E_DISKISWRITEPROTECTED = unchecked((int)0x80030013);
        /// <summary>An error occurred during a seek operation.</summary>
        private const int STG_E_SEEKERROR = unchecked((int)0x80030019);
        /// <summary>A disk error occurred during a write operation.</summary>
        private const int STG_E_WRITEFAULT = unchecked((int)0x8003001D);
        /// <summary>A disk error occurred during a read operation.</summary>
        private const int STG_E_READFAULT = unchecked((int)0x8003001E);
        /// <summary>A share violation has occurred.</summary>
        private const int STG_E_SHAREVIOLATION = unchecked((int)0x80030020);
        /// <summary>A lock violation has occurred.</summary>
        private const int STG_E_LOCKVIOLATION = unchecked((int)0x80030021);
        /// <summary>Invalid parameter error.</summary>
        private const int STG_E_INVALIDPARAMETER = unchecked((int)0x80030057);
        /// <summary>There is insufficient disk space to complete operation.</summary>
        public const int STG_E_MEDIUMFULL = unchecked((int)0x80030070);

        #endregion FACILITY_STORAGE error codes.

        #region FACILITY_ITF error codes.

        /// <summary>Invalid OLEVERB structure</summary>
        public const int OLE_E_OLEVERB = unchecked((int)0x80040000);
        /// <summary>Invalid advise flags</summary>
        public const int OLE_E_ADVF = unchecked((int)0x80040001);
        /// <summary>Can't enumerate any more, because the associated data is missing</summary>
        public const int OLE_E_ENUM_NOMORE = unchecked((int)0x80040002);
        /// <summary>This implementation doesn't take advises</summary>
        public const int OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003);
        /// <summary>There is no connection for this connection ID</summary>
        public const int OLE_E_NOCONNECTION = unchecked((int)0x80040004);
        /// <summary>Need to run the object to perform this operation</summary>
        public const int OLE_E_NOTRUNNING = unchecked((int)0x80040005);
        /// <summary>There is no cache to operate on</summary>
        public const int OLE_E_NOCACHE = unchecked((int)0x80040006);
        /// <summary>Uninitialized object</summary>
        public const int OLE_E_BLANK = unchecked((int)0x80040007);
        /// <summary>Linked object's source class has changed</summary>
        public const int OLE_E_CLASSDIFF = unchecked((int)0x80040008);
        /// <summary>Not able to get the moniker of the object</summary>
        public const int OLE_E_CANT_GETMONIKER = unchecked((int)0x80040009);
        /// <summary>Not able to bind to the source</summary>
        public const int OLE_E_CANT_BINDTOSOURCE = unchecked((int)0x8004000A);
        /// <summary>Object is static; operation not allowed</summary>
        public const int OLE_E_STATIC = unchecked((int)0x8004000B);
        /// <summary>User canceled out of save dialog</summary>
        public const int OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C);
        /// <summary>Invalid rectangle</summary>
        public const int OLE_E_INVALIDRECT = unchecked((int)0x8004000D);
        /// <summary>compobj.dll is too old for the ole2.dll initialized</summary>
        public const int OLE_E_WRONGCOMPOBJ = unchecked((int)0x8004000E);
        /// <summary>Invalid window handle</summary>
        public const int OLE_E_INVALIDHWND = unchecked((int)0x8004000F);
        /// <summary>Object is not in any of the inplace active states</summary>
        public const int OLE_E_NOT_INPLACEACTIVE = unchecked((int)0x80040010);
        /// <summary>Not able to convert object</summary>
        public const int OLE_E_CANTCONVERT = unchecked((int)0x80040011);
        /// <summary>Not able to perform the operation because object is not given storage yet</summary>
        public const int OLE_E_NOSTORAGE = unchecked((int)0x80040012);
        /// <summary>Invalid FORMATETC structure</summary>
        public const int DV_E_FORMATETC = unchecked((int)0x80040064);
        /// <summary>Invalid DVTARGETDEVICE structure</summary>
        public const int DV_E_DVTARGETDEVICE = unchecked((int)0x80040065);
        /// <summary>Invalid STDGMEDIUM structure</summary>
        public const int DV_E_STGMEDIUM = unchecked((int)0x80040066);
        /// <summary>Invalid STATDATA structure</summary>
        public const int DV_E_STATDATA = unchecked((int)0x80040067);
        /// <summary>Invalid lindex</summary>
        public const int DV_E_LINDEX = unchecked((int)0x80040068);
        /// <summary>Invalid tymed</summary>
        public const int DV_E_TYMED = unchecked((int)0x80040069);
        /// <summary>Invalid clipboard format</summary>
        public const int DV_E_CLIPFORMAT = unchecked((int)0x8004006A);
        /// <summary>Invalid aspect(s)</summary>
        public const int DV_E_DVASPECT = unchecked((int)0x8004006B);
        /// <summary>tdSize parameter of the DVTARGETDEVICE structure is invalid</summary>
        public const int DV_E_DVTARGETDEVICE_SIZE = unchecked((int)0x8004006C);
        /// <summary>Object doesn't support IViewObject interface</summary>
        public const int DV_E_NOIVIEWOBJECT = unchecked((int)0x8004006D);

        #endregion FACILITY_ITF error codes.

        /// <summary>
        /// Provides a representation of an object so it can
        /// be displayed as an embedded object inside of a container.
        /// </summary>
        public const int DVASPECT_CONTENT = 0x1;
        /// <summary>
        /// Provides a thumbnail representation of an object so it can
        /// be displayed in a browsing tool.
        /// </summary>
        public const int DVASPECT_THUMBNAIL = 0x2;
        /// <summary>Provides an iconic representation of an object.</summary>
        public const int DVASPECT_ICON = 0x4;
        /// <summary>
        /// Provides a representation of the object on the screen as
        /// though it were printed to a printer using the Print command
        /// from the File menu.
        /// </summary>
        public const int DVASPECT_DOCPRINT = 0x8;

        /// <summary>Direction of data is for reading.</summary>
        public const int DATADIR_GET = 1;

        /// <summary>Direction of data is for writing.</summary>
        public const int DATADIR_SET = 2;

        /// <summary>
        /// The storage medium is a global memory handle (HGLOBAL).
        /// Allocate the global handle with the GMEM_SHARE flag.
        /// </summary>
        public const int TYMED_HGLOBAL  = 1;
        /// <summary>
        /// The storage medium is a disk file identified by a path.
        /// </summary>
        public const int TYMED_FILE     = 2;
        /// <summary>
        /// The storage medium is a stream object identified by an IStream pointer.
        /// </summary>
        public const int TYMED_ISTREAM  = 4;
        /// <summary>
        /// The storage medium is a storage component identified by an IStorage pointer.
        /// </summary>
        public const int TYMED_ISTORAGE = 8;
        /// <summary>The storage medium is a GDI component (HBITMAP).</summary>
        public const int TYMED_GDI      = 16;
        /// <summary>The storage medium is a metafile (HMETAFILE).</summary>
        public const int TYMED_MFPICT   = 32;
        /// <summary>The storage medium is an enhanced metafile.</summary>
        public const int TYMED_ENHMF    = 64;
        /// <summary>No data is being passed.</summary>
        public const int TYMED_NULL     = 0;

        #endregion OLE constants.

        /// <summary>
        /// The FORMATETC structure is a generalized Clipboard format.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public sealed class FORMATETC
        {
            /// <summary>Particular clipboard format of interest.</summary>
            public short cfFormat = 0;
            /// <summary>Dummy for alignment.</summary>
            public short dummy = 0;
            /// <summary>
            /// Pointer to a DVTARGETDEVICE structure containing
            /// information about the target device for which the
            /// data is being composed. A NULL value is used whenever
            /// the specified data format is independent of the target
            /// device or when the caller doesn't care what device is
            /// used.
            /// </summary>
            public IntPtr ptd = IntPtr.Zero;
            /// <summary>
            /// One of the DVASPECT enumeration constants that indicate
            /// how much detail should be contained in the rendering.
            /// </summary>
            public int dwAspect = 0;
            /// <summary>
            /// Part of the aspect when the data must be split across
            /// page boundaries. The most common value is -1, which
            /// identifies all of the data.
            /// </summary>
            public int lindex = 0;
            /// <summary>
            /// One of the TYMED enumeration constants which indicate
            /// the type of storage medium used to transfer the object's data.
            /// </summary>
            public int tymed = 0;
        }

        /// <summary>
        /// The FORMATETC class, as a struct (useful for marshalling as arrays).
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct FORMATETCStruct
        {
            /// <summary>Particular clipboard format of interest.</summary>
            public short cfFormat;
            /// <summary>Dummy for alignment.</summary>
            public short dummy;
            /// <summary>
            /// Pointer to a DVTARGETDEVICE structure containing
            /// information about the target device for which the
            /// data is being composed. A NULL value is used whenever
            /// the specified data format is independent of the target
            /// device or when the caller doesn't care what device is
            /// used.
            /// </summary>
            public IntPtr ptd;
            /// <summary>
            /// One of the DVASPECT enumeration constants that indicate
            /// how much detail should be contained in the rendering.
            /// </summary>
            public int dwAspect;
            /// <summary>
            /// Part of the aspect when the data must be split across
            /// page boundaries. The most common value is -1, which
            /// identifies all of the data.
            /// </summary>
            public int lindex;
            /// <summary>
            /// One of the TYMED enumeration constants which indicate
            /// the type of storage medium used to transfer the object's data.
            /// </summary>
            public int tymed;
        }

        /// <summary>
        ///
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Auto)]
        public class STGMEDIUM
        {
            /// <summary></summary>
            public int tymed = 0;
            /// <summary></summary>
            public IntPtr unionmember = IntPtr.Zero;
            /// <summary></summary>
            public IntPtr pUnkForRelease = IntPtr.Zero;
        }

        /// <summary>
        /// The IEnumFORMATETC interface is used to enumerate an array
        /// of FORMATETC structures.
        /// </summary>
        [ComImport(), Guid("00000103-0000-0000-C000-000000000046"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        [SuppressUnmanagedCodeSecurity]
        public interface IEnumFORMATETC {
            /// <summary>
            ///
            /// </summary>
            /// <param name="celt"></param>
            /// <param name="rgelt"></param>
            /// <param name="pceltFetched"></param>
            /// <returns></returns>
            /// <remarks>
            /// If the IEnumFORMATETC::Next method returns a non-NULL
            /// DVTARGETDEVICE pointer in the ptd member of the FORMATETC
            /// structure, it must be allocated with the CoTaskMalloc
            /// function (or its equivalent).
            ///
            /// If the IEnumFORMATETC::Next method returns a non-NULL
            /// DVTARGETDEVICE pointer in the ptd member of the FORMATETC
            /// structure, the memory must be freed with the CoTaskMemFree
            /// function (or its equivalent). Failure to do so will
            /// result in a memory leak.
            /// </remarks>
            [PreserveSig]
            unsafe int Next(
                [In, MarshalAs(UnmanagedType.U4)] int celt,
                [In, Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex=0)] Win32.FORMATETCStruct[] rgelt,
                [In, Out] int* pceltFetched);

            /// <summary>
            ///
            /// </summary>
            /// <param name="celt"></param>
            /// <returns></returns>
            [PreserveSig]
            int Skip([In, MarshalAs(UnmanagedType.U4)] int celt);

            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            [PreserveSig]
            int Reset();

            /// <summary>
            ///
            /// </summary>
            /// <param name="ppenum"></param>
            /// <returns></returns>
            [PreserveSig]
            int Clone([Out, MarshalAs(UnmanagedType.Interface)] out Win32.IEnumFORMATETC ppenum);
        }

        /// <summary>
        /// The IAdviseSink interface enables containers and other objects to
        /// receive notifications of data changes, view changes, and
        /// compound-document changes occurring in objects of interest.
        /// </summary>
        /// <remarks>
        /// Container applications, for example, require notifications
        /// to keep cached presentations of their linked and embedded objects
        /// up-to-date. Calls to IAdviseSink methods are asynchronous, so the
        /// call is sent and then the next instruction is executed without
        /// waiting for the call's return.
        /// </remarks>
        [ComImport(), Guid("0000010f-0000-0000-C000-000000000046"), InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAdviseSink
        {
            /// <summary>
            /// Called by the server to notify a data object's currently
            /// registered advise sinks that data in the object has changed.
            /// </summary>
            /// <param name='pFormatetc'></param>
            /// <param name='pStgmed'></param>
            [PreserveSig]
            void OnDataChange([In]FORMATETC pFormatetc, [In]STGMEDIUM pStgmed);

            /// <summary>
            /// Notifies an object's registered advise sinks that its view
            /// has changed.
            /// </summary>
            /// <param name='dwAspect'>The aspect, or view, of the object.</param>
            /// <param name='lindex'>The portion of the view that has changed.</param>
            [PreserveSig]
            void OnViewChange([In]int dwAspect, [In]int lindex);

            /// <summary>
            /// Called by the server to notify all registered advisory sinks
            /// that the object has been renamed.
            /// </summary>
            /// <param name='pmk'>
            /// Pointer to the IMoniker interface on the new full moniker of
            /// the object.
            /// </param>
            [PreserveSig]
            void OnRename([In, MarshalAs(UnmanagedType.Interface)]object pmk);

            /// <summary>
            /// Called by the server to notify all registered advisory sinks
            /// that the object has been saved.
            /// </summary>
            [PreserveSig]
            void OnSave();

            /// <summary>
            /// Called by the server to notify all registered advisory sinks
            /// that the object has changed from the running to the loaded
            /// state.
            /// </summary>
            [PreserveSig]
            void OnClose();
        }

        /// <summary>
        /// Specifies methods that enable data transfer and
        /// notification of changes in data.
        /// </summary>
        [ComImport(), Guid("0000010E-0000-0000-C000-000000000046"),
         InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
        [SuppressUnmanagedCodeSecurity]
        public interface IOleDataObject
        {
            /// <summary>
            /// Renders the data described in a FORMATETC structure and
            /// transfers it through the STGMEDIUM structure.
            /// </summary>
            /// <param name="pFormatetc">
            /// Defines the format, medium, and target device to use
            /// when passing the data.
            /// </param>
            /// <param name="pMedium">
            /// Indicates the storage medium containing the returned
            /// data through its tymed member, and the responsibility
            /// for releasing the medium through the value of its
            /// pUnkForRelease member.
            /// </param>
            /// <returns>
            /// S_OK if data was successfully retrieved and placed
            /// in the storage medium provided, an error value
            /// otherwise.
            /// </returns>
            [PreserveSig]
            int OleGetData(Win32.FORMATETC pFormatetc, [Out] Win32.STGMEDIUM pMedium);

            /// <summary>
            /// Called by a data consumer to obtain data from a
            /// source data object.
            /// </summary>
            /// <param name="pFormatetc">
            /// Defines the format, medium, and target device to use
            /// when passing the data. Only one medium can be specified
            /// in TYMED, and only the following TYMED values are
            /// valid: TYMED_STORAGE, TYMED_STREAM, TYMED_HGLOBAL,
            /// or TYMED_FILE.
            /// </param>
            /// <param name="pMedium">
            /// Defines the storage medium containing the data being
            /// transferred.
            /// </param>
            /// <returns>
            /// S_OK if data was successfully retrieved and placed
            /// in the storage medium provided, an error value
            /// otherwise.
            /// </returns>
            /// <remarks>
            /// This method differs from the GetData method in that
            /// the caller must allocate and free the specified
            /// storage medium.
            /// </remarks>
            [PreserveSig]
            int OleGetDataHere([In]Win32.FORMATETC pFormatetc,
                [In, Out] Win32.STGMEDIUM pMedium);

            /// <summary>
            /// Determines whether the data object is capable of
            /// rendering the data described in the FORMATETC structure.
            /// </summary>
            /// <param name="pFormatetc">
            /// Defines the format, medium, and target device to use for
            /// the query.
            /// </param>
            /// <returns>
            /// S_OK if a subsequent call to IDataObject::GetData would
            /// probably be successful, an error code otherwise.
            /// </returns>
            [PreserveSig]
            int OleQueryGetData(Win32.FORMATETC pFormatetc);

            /// <summary>
            /// Provides a standard FORMATETC structure that is logically
            /// equivalent to one that is more complex.
            /// </summary>
            /// <param name="pformatectIn">
            /// The format, medium, and target device that the caller
            /// would like to use to retrieve data in a subsequent call
            /// such as IDataObject::GetData.
            /// </param>
            /// <param name="pformatetcOut">
            /// Contains the most general information possible for a
            /// specific rendering, making it canonically equivalent to
            /// pFormatetcIn.
            /// </param>
            /// <returns>
            /// S_OK if the returned FORMATETC structure is different
            /// from the one that was passed, DATA_S_SAMEFORMATETC
            /// if the FORMATETC structures are the same and NULL is
            /// returned in pFormatetcOut, an error code otherwise.
            /// </returns>
            [PreserveSig]
            int OleGetCanonicalFormatEtc(
                [In, Out] ref Win32.FORMATETCStruct pformatectIn,
                [In, Out] ref Win32.FORMATETCStruct pformatetcOut);

            /// <summary>
            /// Called by an object containing a data source to
            /// transfer data to the object that implements this method.
            /// </summary>
            /// <param name="pFormatectIn">
            /// Defines the format used by the data object when
            /// interpreting the data contained in the storage
            /// medium.
            /// </param>
            /// <param name="pmedium">
            /// Defines the storage medium in which the data is being passed.
            /// </param>
            /// <param name="fRelease">
            /// If true, the data object called, which implements
            /// IDataObject::SetData, owns the storage medium after the
            /// call returns.
            /// </param>
            /// <returns>
            /// S_OK if the data was successfully transferred, an error
            /// code otherwise.
            /// </returns>
            [PreserveSig]
            int OleSetData(Win32.FORMATETC pFormatectIn,
                Win32.STGMEDIUM pmedium, bool fRelease);

            /// <summary>
            /// Creates an object for enumerating the FORMATETC
            /// structures for a data object.
            /// </summary>
            /// <param name="dwDirection">
            /// Direction of the data through a value from the
            /// enumeration DATADIR (1=get, 2=set).
            /// </param>
            /// <param name="enumFormatEtc">
            /// Enumerator for supproted FORMATETCs.
            /// </param>
            /// <returns>
            /// Address of IEnumFORMATETC* pointer variable that receives
            /// the interface pointer to the new enumerator object.
            /// </returns>
            [PreserveSig]
            int OleEnumFormatEtc(
                [In, MarshalAs(UnmanagedType.U4)] int dwDirection,
                [Out, MarshalAs(UnmanagedType.Interface)] out Win32.IEnumFORMATETC enumFormatEtc);

            /// <summary>
            /// Called by an object supporting an advise sink to create
            /// a connection between a data object and the advise sink.
            /// </summary>
            /// <param name="pFormatetc">
            /// Pointer to a FORMATETC structure that defines the format,
            /// target device, aspect, and medium that will be used for
            /// future notifications.
            /// </param>
            /// <param name="advf"></param>
            /// <param name="pAdvSink">Pointer to the IAdviseSink interface on the advisory sink that will receive the change notification.</param>
            /// <param name="pdwConnection">
            /// Pointer to a DWORD token that identifies this connection. You
            /// can use this token later to delete the advisory connection
            /// (by passing it to IDataObject::DUnadvise). If this value is
            /// zero, the connection was not established.
            /// </param>
            /// <returns></returns>
            [PreserveSig]
            unsafe int OleDAdvise(
                [In] Win32.FORMATETC pFormatetc,
                [In, MarshalAs(UnmanagedType.U4)] int advf,
                [In, MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink,
                [In, Out] int* pdwConnection);

            /// <summary>
            /// Destroys a notification connection that had been
            /// previously set up.
            /// </summary>
            /// <param name="dwConnection"></param>
            /// <returns></returns>
            [PreserveSig]
            int OleDUnadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);

            /// <summary>
            /// Creates an object that can be used to enumerate the
            /// current advisory connections.
            /// </summary>
            /// <param name="ppenumAdvise"></param>
            /// <returns></returns>
            [PreserveSig]
            int OleEnumDAdvise([Out, MarshalAs(UnmanagedType.Interface)] out object ppenumAdvise);
        }

        /// <summary>
        /// Renders the data described in a FORMATETC structure and
        /// transfers it through the STGMEDIUM structure.
        /// </summary>
        /// <param name="dataObject">Interface to invoke method on.</param>
        /// <param name="pFormatetc">
        /// Defines the format, medium, and target device to use
        /// when passing the data.
        /// </param>
        /// <param name="pMedium">
        /// Indicates the storage medium containing the returned
        /// data through its tymed member, and the responsibility
        /// for releasing the medium through the value of its
        /// pUnkForRelease member.
        /// </param>
        /// <returns>
        /// S_OK if data was successfully retrieved and placed
        /// in the storage medium provided, an error value
        /// otherwise.
        /// </returns>
        [PreserveSig]
        public static int SafeOleGetData(IOleDataObject dataObject, Win32.FORMATETC pFormatetc, [Out] Win32.STGMEDIUM pMedium)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return dataObject.OleGetData(pFormatetc, pMedium);
        }
        /// <summary>
        /// Called by a data consumer to obtain data from a
        /// source data object.
        /// </summary>
        /// <param name="dataObject">Interface to invoke method on.</param>
        /// <param name="pFormatetc">
        /// Defines the format, medium, and target device to use
        /// when passing the data. Only one medium can be specified
        /// in TYMED, and only the following TYMED values are
        /// valid: TYMED_STORAGE, TYMED_STREAM, TYMED_HGLOBAL,
        /// or TYMED_FILE.
        /// </param>
        /// <param name="pMedium">
        /// Defines the storage medium containing the data being
        /// transferred.
        /// </param>
        /// <returns>
        /// S_OK if data was successfully retrieved and placed
        /// in the storage medium provided, an error value
        /// otherwise.
        /// </returns>
        /// <remarks>
        /// This method differs from the GetData method in that
        /// the caller must allocate and free the specified
        /// storage medium.
        /// </remarks>
        [PreserveSig]
        public static int SafeOleGetDataHere(IOleDataObject dataObject, Win32.FORMATETC pFormatetc,
            [In, Out] Win32.STGMEDIUM pMedium)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return dataObject.OleGetDataHere(pFormatetc, pMedium);
        }

        /// <summary>
        /// Determines whether the data object is capable of
        /// rendering the data described in the FORMATETC structure.
        /// </summary>
        /// <param name="dataObject">Interface to invoke method on.</param>
        /// <param name="pFormatetc">
        /// Defines the format, medium, and target device to use for
        /// the query.
        /// </param>
        /// <returns>
        /// S_OK if a subsequent call to IDataObject::GetData would
        /// probably be successful, an error code otherwise.
        /// </returns>
        public static int SafeOleQueryGetData(IOleDataObject dataObject, Win32.FORMATETC pFormatetc)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return dataObject.OleQueryGetData(pFormatetc);
        }
        /// <summary>
        /// Called by an object containing a data source to
        /// transfer data to the object that implements this method.
        /// </summary>
        /// <param name="dataObject">Interface to invoke method on.</param>
        /// <param name="pFormatectIn">
        /// Defines the format used by the data object when
        /// interpreting the data contained in the storage
        /// medium.
        /// </param>
        /// <param name="pmedium">
        /// Defines the storage medium in which the data is being passed.
        /// </param>
        /// <param name="fRelease">
        /// If true, the data object called, which implements
        /// IDataObject::SetData, owns the storage medium after the
        /// call returns.
        /// </param>
        /// <returns>
        /// S_OK if the data was successfully transferred, an error
        /// code otherwise.
        /// </returns>
        [PreserveSig]
        public static int SafeOleSetData(IOleDataObject dataObject, Win32.FORMATETC pFormatectIn,
            Win32.STGMEDIUM pmedium, bool fRelease)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return dataObject.OleSetData(pFormatectIn, pmedium, fRelease);
        }

        /// <summary>
        /// Creates an object for enumerating the FORMATETC
        /// structures for a data object.
        /// </summary>
        /// <param name="dataObject">Interface to invoke method on.</param>
        /// <param name="dwDirection">
        /// Direction of the data through a value from the
        /// enumeration DATADIR (1=get, 2=set).
        /// </param>
        /// <param name="enumFormatEtc">
        /// Enumerator for supproted FORMATETCs.
        /// </param>
        /// <returns>
        /// Address of IEnumFORMATETC* pointer variable that receives
        /// the interface pointer to the new enumerator object.
        /// </returns>
        public static int SafeOleEnumFormatEtc(IOleDataObject dataObject,
            int dwDirection, out Win32.IEnumFORMATETC enumFormatEtc)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return dataObject.OleEnumFormatEtc(dwDirection, out enumFormatEtc);
        }

        /// <summary>Retrieves the next celt items in the enumeration sequence.</summary>
        /// <param name='enumFormatEtc'>Interface to invoke method on.</param>
        /// <param name='celt'>Count of elements asked for.</param>
        /// <param name='rgelt'>Array of size celt (or larger) of the elements of interest.</param>
        /// <returns>S_OK if the number of elements supplied is celt; S_FALSE otherwise.</returns>
        public static int SafeEnumFormatEtcNext(IEnumFORMATETC enumFormatEtc,
            int celt, Win32.FORMATETCStruct[] rgelt)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            int fetched;
            return EnumFormatEtcNext(enumFormatEtc, celt, rgelt, false, out fetched);
        }

        /// <summary>Retrieves the next celt items in the enumeration sequence.</summary>
        /// <param name='enumFormatEtc'>Interface to invoke method on.</param>
        /// <param name='celt'>Count of elements asked for.</param>
        /// <param name='rgelt'>Array of size celt (or larger) of the elements of interest.</param>
        /// <param name='fetched'>Count of fetched items.</param>
        /// <returns>S_OK if the number of elements supplied is celt; S_FALSE otherwise.</returns>
        public static int SafeEnumFormatEtcNext(IEnumFORMATETC enumFormatEtc,
            int celt, Win32.FORMATETCStruct[] rgelt, out int fetched)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return EnumFormatEtcNext(enumFormatEtc, celt, rgelt, true, out fetched);
        }

        private unsafe static int EnumFormatEtcNext(IEnumFORMATETC enumFormatEtc,
            int celt, Win32.FORMATETCStruct[] rgelt, bool useFetched, out int fetched)
        {
            int result;
            if (useFetched)
            {
                fixed(int* pFetched = &fetched)
                {
                    result = enumFormatEtc.Next(celt, rgelt, pFetched);
                }
            }
            else
            {
                fetched = 0;
                result = enumFormatEtc.Next(celt, rgelt, null);
            }
            return result;
        }

        /// <summary>
        /// Called by an object supporting an advise sink to create
        /// a connection between a data object and the advise sink.
        /// </summary>
        /// <param name="dataObject">Interface to invoke method on.</param>
        /// <param name="pFormatetc">
        /// Pointer to a FORMATETC structure that defines the format,
        /// target device, aspect, and medium that will be used for
        /// future notifications.
        /// </param>
        /// <param name="advf"></param>
        /// <param name="pAdvSink">Pointer to the IAdviseSink interface on the advisory sink that will receive the change notification.</param>
        /// <param name="pdwConnection">
        /// Pointer to a DWORD token that identifies this connection. You
        /// can use this token later to delete the advisory connection
        /// (by passing it to IDataObject::DUnadvise). If this value is
        /// zero, the connection was not established.
        /// </param>
        /// <returns></returns>
        public static int SafeOleDAdvise(IOleDataObject dataObject,
            Win32.FORMATETC pFormatetc, int advf,
            IAdviseSink pAdvSink, out int pdwConnection)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            unsafe
            {
                fixed(int* p = &pdwConnection)
                {
                    return dataObject.OleDAdvise(pFormatetc, advf, pAdvSink, p);
                }
            }
        }

        /// <summary>
        /// Destroys a notification connection that had been
        /// previously set up.
        /// </summary>
        /// <param name="dataObject">Interface to invoke method on.</param>
        /// <param name="dwConnection"></param>
        /// <returns></returns>
        public static int SafeOleDUnadvise(IOleDataObject dataObject, int dwConnection)
        {
            return dataObject.OleDUnadvise(dwConnection);
        }

        /// <summary>
        /// Creates an object that can be used to enumerate the
        /// current advisory connections.
        /// </summary>
        /// <param name="dataObject">Interface to invoke method on.</param>
        /// <param name="ppenumAdvise"></param>
        /// <returns></returns>
        public static int SafeOleEnumDAdvise(IOleDataObject dataObject, out object ppenumAdvise)
        {
            return dataObject.OleEnumDAdvise(out ppenumAdvise);
        }

        /// <summary>
        /// Creates an embedded object from the contents of a named file.
        /// </summary>
        /// <param name='rclsid'>Reserved. Must be CLSID_NULL.</param>
        /// <param name='lpszFileName'>
        /// Pointer to a string specifying the full path of the file from
        /// which the object should be initialized.
        /// </param>
        /// <param name='riid'>
        /// Reference to the identifier of the interface the caller later
        /// uses to communicate with the new object (usually
        /// IID_IOleObject).
        /// </param>
        /// <param name='renderopt'>
        /// Value from the enumeration OLERENDER that indicates the locally
        /// cached drawing or data-retrieval capabilities the newly created
        /// object is to have. The OLERENDER value chosen affects the
        /// possible values for the pFormatEtc parameter.
        /// </param>
        /// <param name='lpFormatEtc'>
        /// Depending on which of the OLERENDER flags is used as the value
        /// of renderopt, pointer to one of the FORMATETC enumeration
        /// values. Refer also to the OLERENDER enumeration for restrictions.
        /// </param>
        /// <param name='pClientSite'>
        /// Pointer to an instance of IOleClientSite, the primary interface
        /// through which the object will request services from its container.
        /// May be NULL.</param>
        /// <param name='pStg'>
        /// Pointer to the IStorage interface on the storage object. This
        /// parameter may not be NULL.
        /// </param>
        /// <param name='ppvObj'>
        /// Address of pointer variable that receives the interface pointer
        /// requested in riid. Upon successful return, *ppvObj contains the
        /// requested interface pointer on the newly created object.
        /// </param>
        /// <returns>
        /// S_OK, STG_E_FILENOTFOUND, OLE_E_CANTBINDTOSOURCE, STG_E_MEDIUMFULL,
        /// DV_E_TYMED, DV_E_LINDEX or DV_E_FORMATETC.
        /// </returns>
        [DllImport("ole32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        private static extern int OleCreateFromFile(
            [In, Out] ref Guid rclsid,
            /* LPCOLESTR */ [MarshalAs(UnmanagedType.BStr)] string lpszFileName,
            [In, Out] Guid riid, int renderopt, FORMATETC lpFormatEtc,
            /* LPOLECLIENTSITE */ [MarshalAs(UnmanagedType.IUnknown)] object pClientSite,
            /* LPSTORAGE */ [MarshalAs(UnmanagedType.Interface)] object pStg,
            /* OUT LPVOID FAR* */ [MarshalAs(UnmanagedType.Interface)] out object ppvObj);

        /// <summary>
        /// Retrieves a data object that you can use to access
        /// the contents of the clipboard.
        /// </summary>
        /// <param name="data">
        /// Variable that receives the interface pointer to the
        /// clipboard data object.
        /// </param>
        /// <returns>
        /// One of E_INVALIDARG, E_OUTOFMEMORY, S_OK, CLIPBRD_E_CANT_CLOSE
        /// or CLIPBRD_E_CANT_OPEN.
        /// </returns>
        [DllImport("ole32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern int OleGetClipboard([In, Out] ref IOleDataObject data);

        /// <summary>
        /// Places a pointer to a specific data object onto the clipboard.
        /// This makes the data object accessible to the OleGetClipboard
        /// function.
        /// </summary>
        /// <param name="pDataObj">
        /// Data object from which the data to be placed on the clipboard
        /// can be obtained. This parameter can be NULL; in which case
        /// the clipboard is emptied.
        /// </param>
        /// <returns>
        /// One of S_OK, CLIPBRD_E_CANT_OPEN, CLIPBRD_E_CANT_EMPTY,
        /// CLIPBRD_E_CANT_CLOSE or CLIPBRD_E_CANT_SET.
        /// </returns>
        [DllImport("ole32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern int OleSetClipboard(IOleDataObject pDataObj);

        /// <summary>
        /// Carries out the clipboard shutdown sequence. It also releases
        /// the IDataObject pointer that was placed on the clipboard by
        /// the OleSetClipboard function.
        /// </summary>
        /// <returns>
        /// One of S_OK, CLIPBRD_E_CANT_CLOSE or CLIPBRD_E_CANT_OPEN.
        /// </returns>
        [DllImport("ole32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern int OleFlushClipboard();

        /// <summary>
        /// Determines whether the data object pointer previously placed
        /// on the clipboard by the OleSetClipboard function is still on
        /// the clipboard.
        /// </summary>
        /// <param name="pDataObj">
        /// Data object containing clipboard data of interest, which the
        /// caller previously placed on the clipboard.
        /// </param>
        /// <returns>
        /// S_OK if the IDataObject pointer is currently on
        /// the clipboard and the caller is the owner of the clipboard,
        /// S_FALSE otherwise.
        /// </returns>
        [DllImport("ole32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern int OleIsCurrentClipboard(IOleDataObject pDataObj);

        /// <summary>
        /// Frees the specified storage medium.
        /// </summary>
        /// <param name="pmedium">
        /// Pointer to the storage medium that is to be freed.
        /// </param>
        [DllImport("ole32.dll", ExactSpelling=true, CharSet=CharSet.Auto)]
        public static extern void ReleaseStgMedium(STGMEDIUM pmedium);

        /// <summary>
        /// Frees the specified storage medium.
        /// </summary>
        /// <param name="pmedium">
        /// Pointer to the storage medium that is to be freed.
        /// </param>
        public static void SafeReleaseStgMedium(STGMEDIUM pmedium)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            ReleaseStgMedium(pmedium);
        }

        /// <summary>Gets a dummy target device for a FORMATETC structure.</summary>
        /// <returns>An non-null invalid target device for a FORMATETC structure.</returns>
        public static IntPtr SafeGetDummyTargetDevice()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return GetDummyTargetDevice();
        }

        /// <summary>Gets a dummy target device for a FORMATETC structure.</summary>
        /// <returns>An non-null invalid target device for a FORMATETC structure.</returns>
        private unsafe static IntPtr GetDummyTargetDevice()
        {
            return (IntPtr)(0x00000001);
        }

        #endregion OLE support.

        #region Process management support.

        /// <summary>Handle to process or module instance.</summary>
        public struct HINSTANCE
        {
            /// <summary>Handle value.</summary>
            public IntPtr h;

            /// <summary>
            ///
            /// </summary>
            /// <param name="h"></param>
            /// <returns></returns>
            public static HINSTANCE Cast(IntPtr h)
            {
                HINSTANCE hTemp = new HINSTANCE();
                hTemp.h = h;
                return hTemp;
            }

            /// <summary>
            ///
            /// </summary>
            public static HINSTANCE NULL
            {
                get
                {
                    HINSTANCE hTemp = new HINSTANCE();
                    hTemp.h = IntPtr.Zero;
                    return hTemp;
                }
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="hl"></param>
            /// <param name="hr"></param>
            /// <returns></returns>
            public static bool operator==(HINSTANCE hl, HINSTANCE hr)
            {
                return (hl.h == hr.h);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="hl"></param>
            /// <param name="hr"></param>
            /// <returns></returns>
            public static bool operator!=(HINSTANCE hl, HINSTANCE hr)
            {
                return (hl.h != hr.h);
            }

            /// <summary>
            ///
            /// </summary>
            /// <param name="oCompare"></param>
            /// <returns></returns>
            override public bool Equals(object oCompare)
            {
                HINSTANCE hr = Cast((IntPtr)oCompare);
                return (h == hr.h);
            }

            /// <summary>
            ///
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return (int) h;
            }
        }

        #endregion Process management support.

        #region System parameter information support.

        /// <summary>
        /// Retrieves or sets the value of one of the system-wide parameters.
        /// </summary>
        /// <param name="action">System-wide parameter to be retrieved or set.</param>
        /// <param name="param">Depends on the system parameter being queried or set.</param>
        /// <param name="pvParam">Depends on the system parameter being queried or set.</param>
        /// <param name="winIni">
        /// If a system parameter is being set, specifies whether the user
        /// profile is to be updated, and if so, whether the WM_SETTINGCHANGE
        /// message is to be broadcast to all top-level windows to notify
        /// them of the change.</param>
        /// <returns>true if succeeded, false otherwise.</returns>
        [DllImport("user32.dll",SetLastError=true)]
        public static extern bool SystemParametersInfo(
            uint action, uint param, IntPtr pvParam, int winIni);

        /// <summary>
        /// Retrieves or sets the value of one of the system-wide parameters.
        /// </summary>
        /// <param name="action">System-wide parameter to be retrieved or set.</param>
        /// <param name="param">Depends on the system parameter being queried or set.</param>
        /// <param name="pvParam">Depends on the system parameter being queried or set.</param>
        /// <param name="winIni">
        /// If a system parameter is being set, specifies whether the user
        /// profile is to be updated, and if so, whether the WM_SETTINGCHANGE
        /// message is to be broadcast to all top-level windows to notify
        /// them of the change.</param>
        /// <returns>true if succeeded, false otherwise.</returns>
        [DllImport("user32.dll",SetLastError=true)]
        public static extern bool SystemParametersInfo(
            uint action, uint param, ref uint pvParam, int winIni);

        /// <summary>
        /// Retrieves or sets the value of one of the system-wide parameters.
        /// </summary>
        /// <param name="action">System-wide parameter to be retrieved or set.</param>
        /// <param name="param">Depends on the system parameter being queried or set.</param>
        /// <param name="pvParam">Depends on the system parameter being queried or set.</param>
        /// <param name="winIni">
        /// If a system parameter is being set, specifies whether the user
        /// profile is to be updated, and if so, whether the WM_SETTINGCHANGE
        /// message is to be broadcast to all top-level windows to notify
        /// them of the change.</param>
        /// <returns>true if succeeded, false otherwise.</returns>
        [DllImport("user32.dll",SetLastError=true)]
        public static extern bool SystemParametersInfo(
            uint action, uint param, uint pvParam, int winIni);

        /// <summary>Retrieves the caret width in edit controls, in pixels.</summary>
        public const uint SPI_GETCARETWIDTH = 0x2006;
        /// <summary>Sets the caret width in edit controls.</summary>
        public const uint SPI_SETCARETWIDTH = 0x2007;

        /// <summary>
        /// Writes the new system-wide parameter setting to the user profile.
        /// </summary>
        public const int SPIF_UPDATEINIFILE = 0x0001;

        /// <summary>
        /// Broadcasts the WM_SETTINGCHANGE message after updating the user
        /// profile.
        /// </summary>
        public const int SPIF_SENDCHANGE = 0x0002;

        /// <summary>Same as SPIF_SENDCHANGE.</summary>
        public const int SPIF_SENDWININICHANGE = SPIF_SENDCHANGE;

        #endregion System parameter information support.

        #region Visual styles (theme) support.

        /// <summary>
        /// Tests if a visual style for the current application is active.
        /// </summary>
        /// <returns>
        /// true if a visual style is enabled, and windows with visual styles 
        /// applied should call OpenThemeData to start using theme drawing 
        /// services; false otherwise.
        /// </returns>
        [DllImport("uxtheme.dll")]
        public static extern bool IsThemeActive();

        ///<SecurityNote> 
        ///Critical - elevates via a SUC. 
        ///</SecurityNote>         
        /// <summary>Returns the current theme name.</summary>        
        [SuppressUnmanagedCodeSecurity, SecurityCritical]
        [DllImport("Uxtheme", CharSet = CharSet.Auto, BestFitMapping = false)]
        public static extern int GetCurrentThemeName(System.Text.StringBuilder pszThemeFileName, 
            int dwMaxNameChars, System.Text.StringBuilder pszColorBuff, int dwMaxColorChars, 
            System.Text.StringBuilder pszSizeBuff, int cchMaxSizeChars);

        /// <summary>
        /// Max Path name length
        /// </summary>
        public const int MAX_PATH = 260;

        /// <summary>Returns the current theme name.</summary>
        /// <returns>
        /// Theme name. Returns string.Empty if operation fails.
        /// </returns>
        [SecurityCritical, SecurityTreatAsSafe]
        public static string SafeGetCurrentThemeName()
        {
            System.Text.StringBuilder themeName = new System.Text.StringBuilder(MAX_PATH);
            System.Text.StringBuilder themeColor = new System.Text.StringBuilder(MAX_PATH);

            if (GetCurrentThemeName(themeName, themeName.Capacity, themeColor, themeColor.Capacity, null, 0) == 0)
            {
                //Success
                return System.IO.Path.GetFileNameWithoutExtension(themeName.ToString());
            }
            else
            {
                //Fail
                return string.Empty;
            }
        }

        /// <summary>Returns the current theme color.</summary>
        /// <returns>
        /// Theme color. Returns string.Empty if operation fails.
        /// </returns>
        [SecurityCritical, SecurityTreatAsSafe]
        public static string SafeGetCurrentThemeColor()
        {
            System.Text.StringBuilder themeName = new System.Text.StringBuilder(MAX_PATH);
            System.Text.StringBuilder themeColor = new System.Text.StringBuilder(MAX_PATH);

            if (GetCurrentThemeName(themeName, themeName.Capacity, themeColor, themeColor.Capacity, null, 0) == 0)
            {
                //Success
                return themeColor.ToString();                
            }
            else
            {
                //Fail
                return string.Empty;
            }
        }

        #endregion Visual styles (theme) support.

        #region Windows.

        /// <summary>Show window constant.</summary>
        public const int SW_HIDE             = 0;
        /// <summary>Show window constant.</summary>
        public const int SW_SHOWNORMAL       = 1;
        /// <summary>Show window constant.</summary>
        public const int SW_NORMAL           = 1;
        /// <summary>Show window constant.</summary>
        public const int SW_SHOWMINIMIZED    = 2;
        /// <summary>Show window constant.</summary>
        public const int SW_SHOWMAXIMIZED    = 3;
        /// <summary>Show window constant.</summary>
        public const int SW_MAXIMIZE         = 3;
        /// <summary>Show window constant.</summary>
        public const int SW_SHOWNOACTIVATE   = 4;
        /// <summary>Show window constant.</summary>
        public const int SW_SHOW             = 5;
        /// <summary>Show window constant.</summary>
        public const int SW_MINIMIZE         = 6;
        /// <summary>Show window constant.</summary>
        public const int SW_SHOWMINNOACTIVE  = 7;
        /// <summary>Show window constant.</summary>
        public const int SW_SHOWNA           = 8;
        /// <summary>Show window constant.</summary>
        public const int SW_RESTORE          = 9;
        /// <summary>Show window constant.</summary>
        public const int SW_SHOWDEFAULT      = 10;
        /// <summary>Show window constant.</summary>
        public const int SW_FORCEMINIMIZE    = 11;
        /// <summary>Show window constant.</summary>
        public const int SW_MAX              = 11;


        /// <summary>Windows handle structure.</summary>
        [Serializable]
        public struct HWND
        {
            /// <summary>Handle.</summary>
            public IntPtr h;

            /// <summary>Handle cast.</summary>
            public static implicit operator IntPtr(HWND h)
            {
                return h.h;
            }

            /// <summary>NULL handle.</summary>
            public static HWND NULL
            {
                get
                {
                    HWND hTemp = new HWND();
                    hTemp.h = IntPtr.Zero;
                    return hTemp;
                }
            }
        }

        /// <summary>
        /// Brings the specified window to the top of the Z order. If the
        /// window is a top-level window, it is activated. If the window
        /// is a child window, the top-level parent window associated with
        /// the child window is activated.
        /// </summary>
        /// <param name="hwnd">
        /// Handle to the window to bring to the top of the Z order.
        /// </param>
        /// <returns>Nonzero on success, zero otherwise.</returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool BringWindowToTop(IntPtr hwnd);

        /// <summary>
        /// Makes a safe call to BringWindowToTop.
        /// </summary>
        /// <param name="hwnd">
        /// Handle to the window to bring to the top of the Z order.
        /// </param>
        /// <returns>Nonzero on success, zero otherwise.</returns>
        public static bool SafeBringWindowToTop(IntPtr hwnd)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return BringWindowToTop(hwnd);
        }

        /// <summary>
        /// Returns a handle to the desktop window.
        /// </summary>
        /// <returns>A handle to the desktop window.</returns>
        /// <remarks>
        /// The desktop window covers the entire screen. The desktop window
        /// is the area on top of which all icons and other windows are
        /// painted.
        /// </remarks>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// Makes a safe call to GetDesktopWindow.
        /// </summary>
        /// <returns>A handle to the desktop window.</returns>
        public static IntPtr SafeGetDesktopWindow()
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return GetDesktopWindow();
        }

        /// <summary>Changes the position and dimensions of the specified window.</summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="x">Specifies the new position of the left side of the window.</param>
        /// <param name="y">Specifies the new position of the top of the window.</param>
        /// <param name="nWidth">Specifies the new width of the window.</param>
        /// <param name="nHeight">Specifies the new height of the window.</param>
        /// <param name="bRepaint">Specifies the window is to be repainted.</param>
        /// <remarks>For a top-level window, the position and dimensions are
        /// relative to the upper-left corner of the screen. For a child
        /// window, they are relative to the upper-left corner of the parent
        /// window's client area.</remarks>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int MoveWindow(IntPtr hWnd, int x, int y,
            int nWidth, int nHeight, bool bRepaint);

        /// <summary>Changes the position and dimensions of the specified window
        /// without requiring the caller to assert security permission.</summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="x">Specifies the new position of the left side of the window.</param>
        /// <param name="y">Specifies the new position of the top of the window.</param>
        /// <param name="nWidth">Specifies the new width of the window.</param>
        /// <param name="nHeight">Specifies the new height of the window.</param>
        /// <param name="bRepaint">Specifies the window is to be repainted.</param>
        /// <remarks>For a top-level window, the position and dimensions are
        /// relative to the upper-left corner of the screen. For a child
        /// window, they are relative to the upper-left corner of the parent
        /// window's client area.</remarks>
        public static int SafeMoveWindow(IntPtr hWnd, int x, int y,
            int nWidth, int nHeight, bool bRepaint)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return MoveWindow(hWnd, x, y, nWidth, nHeight, bRepaint);
        }

        /// <summary>Show the Window with the specified window's show state.</summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">Specifies how the window is to be shown.</param>
        /// <remarks>ShowWindow allows user to Specify how the window is to be shown
        /// with nCmdShow. nCmdShow can be 0 to 11 which 0=SW_HIDE (see ShowWindow constant).</remarks>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, int nCmdShow);

        /// <summary>Show the Window with the specified window's show state.
        /// without requiring the caller to assert security permission.</summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="nCmdShow">Specifies how the window is to be shown.</param>
        /// <remarks>ShowWindow allows user to Specify how the window is to be shown
        /// with nCmdShow. nCmdShow can be 0 to 11 which 0=SW_HIDE (see ShowWindow constant).</remarks>
        public static int SafeShowWindow(IntPtr hWnd, int nCmdShow)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return ShowWindow(hWnd, nCmdShow);
        }

        /// <summary>Updates the client area of the specified window by sending
        /// a WM_PAINT message to the window if the window's update region
        /// is not empty.</summary>
        /// <param name="hWnd">Handle to the window to be updated.</param>
        /// <remarks>The function sends a WM_PAINT mesage directly to the
        /// window procedure of the specified window, bypassing the
        /// appilcation queue. If the update region is empty, no message is
        /// sent.</remarks>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int UpdateWindow(IntPtr hWnd);

        /// <summary>Updates the client area of the specified window by sending
        /// a WM_PAINT message to the window if the window's update region
        /// is not empty, without requiring the caller to assert security
        /// permission.</summary>
        /// <param name="hWnd">Handle to the window to be updated.</param>
        /// <remarks>The function sends a WM_PAINT mesage directly to the
        /// window procedure of the specified window, bypassing the
        /// appilcation queue. If the update region is empty, no message is
        /// sent.</remarks>
        public static int SafeUpdateWindow(IntPtr hWnd)
        {
            new System.Security.Permissions.SecurityPermission(
                System.Security.Permissions.PermissionState.Unrestricted)
                .Assert();
            return UpdateWindow(hWnd);
        }

        #endregion Windows.

        #region Messaging.
        /// <summary>Does not remove a message when peeking.</summary>
        public const int PM_NOREMOVE           = 0x0000;
        /// <summary>Remove a message when peeking.</summary>
        public const int PM_REMOVE             = 0x0001;
        /// <summary>Does not yield when peeking.</summary>
        public const int PM_NOYIELD            = 0x0002;

        /// <summary>Represents a native message.</summary>
        [StructLayout(LayoutKind.Sequential)]
        public class MSG
        {
            /// <summary>Handle to target window.</summary>
            public HWND     hwnd;
            /// <summary>Message ID</summary>
            public UIntPtr  message;
            /// <summary>W Param.</summary>
            public IntPtr   wParam;
            /// <summary>L Param.</summary>
            public IntPtr   lParam;
            /// <summary>Message time.</summary>
            public int     time;
            /// <summary>X point for message.</summary>
            public int      pt_x;
            /// <summary>Y point for message.</summary>
            public int      pt_y;
        }

        /// <summary>Gets the next message.</summary>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int GetMessage(
                MSG msg, HWND hwnd, int nMsgFilterMin, int nMsgFilterMax);

        /// <summary>Peeks the queue to see what message is coming up next.</summary>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern bool PeekMessage(
                MSG msg, HWND hwnd, int nMsgFilterMin, int nMsgFilterMax, int wRemoveMsg);

        /// <summary>Translates accelerators.</summary>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern bool TranslateMessage(MSG msg);

        /// <summary>Dispatches a message to the correct window procedure.</summary>
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern bool DispatchMessage(MSG msg);

        /// <summary>Retrieve current process id.</summary>
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        public static extern int GetCurrentProcessId();        

        /// <summary>Retrieve current thread id.</summary>
        [System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet=CharSet.Auto)]
        public static extern int GetCurrentThreadId();

        [System.Runtime.InteropServices.DllImport("kernel32.dll",ExactSpelling = true,CharSet = CharSet.Unicode)]
        internal static extern int GetLocaleInfoW(int locale, int type, string data, int dataSize);


        /// <summary>WM_NULL message.</summary>
        public const int WM_NULL                = 0x0000;
        /// <summary>WM_CREATE message.</summary>
        public const int WM_CREATE              = 0x0001;
        /// <summary>WM_DESTROY message.</summary>
        public const int WM_DESTROY             = 0x0002;
        /// <summary>WM_MOVE message.</summary>
        public const int WM_MOVE                = 0x0003;
        /// <summary>WM_SIZE message.</summary>
        public const int WM_SIZE                = 0x0005;
        /// <summary>WM_ACTIVATE message.</summary>
        public const int WM_ACTIVATE            = 0x0006;
        /// <summary>WM_SETFOCUS message.</summary>
        public const int WM_SETFOCUS            = 0x0007;
        /// <summary>WM_KILLFOCUS message.</summary>
        public const int WM_KILLFOCUS           = 0x0008;
        /// <summary>WM_ENABLE message.</summary>
        public const int WM_ENABLE              = 0x000A;
        /// <summary>WM_SETREDRAW message.</summary>
        public const int WM_SETREDRAW           = 0x000B;
        /// <summary>WM_SETTEXT message.</summary>
        public const int WM_SETTEXT             = 0x000C;
        /// <summary>WM_GETTEXT message.</summary>
        public const int WM_GETTEXT             = 0x000D;
        /// <summary>WM_GETTEXTLENGTH message.</summary>
        public const int WM_GETTEXTLENGTH       = 0x000E;
        /// <summary>WM_PAINT message.</summary>
        public const int WM_PAINT               = 0x000F;
        /// <summary>WM_CLOSE message.</summary>
        public const int WM_CLOSE               = 0x0010;
        /// <summary>WM_QUIT message.</summary>
        public const int WM_QUIT                = 0x0012;
        /// <summary>WM_ERASEBKGND message.</summary>
        public const int WM_ERASEBKGND          = 0x0014;
        /// <summary>WM_ACTIVATEAPP message.</summary>
        public const int WM_ACTIVATEAPP         = 0x001C;
        /// <summary>WM_SETCURSOR message.</summary>
        public const int WM_SETCURSOR           = 0x0020;
        /// <summary>WM_MOUSEACTIVATE message.</summary>
        public const int WM_MOUSEACTIVATE       = 0x0021;
        /// <summary>WM_SETFONT message.</summary>
        public const int WM_SETFONT             = 0x0030;
        /// <summary>WM_GETOBJECT message.</summary>
        public const int WM_GETOBJECT           = 0x003D;
        /// <summary>WM_WINDOWPOSCHANGED message.</summary>
        public const int WM_WINDOWPOSCHANGED    = 0x0047;
        /// <summary>WM_INPUTLANGCHANGE message.</summary>
        public const int WM_INPUTLANGCHANGE     = 0x0051;
        /// <summary>WM_DISPLAYCHANGE message.</summary>
        public const int WM_DISPLAYCHANGE       = 0x007E;
        /// <summary>WM_NCHITTEST message.</summary>
        public const int WM_NCHITTEST           = 0x0084;
        /// <summary>WM_PRINTCLIENT message.</summary>
        public const int WM_PRINTCLIENT         = 0x0318;

        /// <summary>WM_CAPTURECHANGED message.</summary>
        public const int WM_CAPTURECHANGED      = 0x0215;
        /// <summary>WM_EXITSIZEMOVE message.</summary>
        public const int WM_EXITSIZEMOVE        = 0x0232;
        /// <summary>WM_USER message.</summary>
        public const int WM_USER                = 0x0400;

        /// <summary>WM_NCCREATE message.</summary>
        public const int WM_NCCREATE            = 0x0081;
        /// <summary>WM_NCDESTROY message.</summary>
        public const int WM_NCDESTROY           = 0x0082;

        /// <summary>WM_KEYDOWN message.</summary>
        public const int WM_KEYDOWN             = 0x0100;
        /// <summary>WM_KEYUP message.</summary>
        public const int WM_KEYUP               = 0x0101;
        /// <summary>WM_CHAR message.</summary>
        public const int WM_CHAR                = 0x0102;
        /// <summary>WM_SYSKEYDOWN message.</summary>
        public const int WM_SYSKEYDOWN          = 0x0104;
        /// <summary>WM_SYSKEYUP message.</summary>
        public const int WM_SYSKEYUP            = 0x0105;
        /// <summary>WM_SYSCHAR message.</summary>
        public const int WM_SYSCHAR             = 0x0106;

        /// <summary>WM_COMMAND message.</summary>
        public const int WM_COMMAND             = 0x0111;
        /// <summary>WM_TIMER message.</summary>
        public const int WM_TIMER               = 0x0113;
        /// <summary>WM_HSCROLL message.</summary>
        public const int WM_HSCROLL             = 0x0114;

        /// <summary>WM_MOUSEHOVER message.</summary>
        public const int WM_MOUSEHOVER          = 0x02A1;
        /// <summary>WM_MOUSELEAVE message.</summary>
        public const int WM_MOUSELEAVE          = 0x02A3;

        /// <summary>WM_MOUSEFIRST message.</summary>
        public const int WM_MOUSEFIRST       =        0x0200;
        /// <summary>WM_MOUSEMOVE message.</summary>
        public const int WM_MOUSEMOVE        =        0x0200;
        /// <summary>WM_LBUTTONDOWN message.</summary>
        public const int WM_LBUTTONDOWN      =        0x0201;
        /// <summary>WM_LBUTTONUP message.</summary>
        public const int WM_LBUTTONUP        =        0x0202;
        /// <summary>WM_LBUTTONDBLCLK message.</summary>
        public const int WM_LBUTTONDBLCLK    =        0x0203;
        /// <summary>WM_RBUTTONDOWN message.</summary>
        public const int WM_RBUTTONDOWN      =        0x0204;
        /// <summary>WM_RBUTTONUP message.</summary>
        public const int WM_RBUTTONUP        =        0x0205;
        /// <summary>WM_RBUTTONDBLCLK message.</summary>
        public const int WM_RBUTTONDBLCLK    =        0x0206;
        /// <summary>WM_MBUTTONDOWN message.</summary>
        public const int WM_MBUTTONDOWN      =        0x0207;
        /// <summary>WM_MBUTTONUP message.</summary>
        public const int WM_MBUTTONUP        =        0x0208;
        /// <summary>WM_MBUTTONDBLCLK message.</summary>
        public const int WM_MBUTTONDBLCLK    =        0x0209;
        /// <summary>WM_MOUSEWHEEL message.</summary>
        public const int WM_MOUSEWHEEL       =        0x020A;
        /// <summary>WM_XBUTTONDOWN message.</summary>
        public const int WM_XBUTTONDOWN      =        0x020B;
        /// <summary>WM_XBUTTONUP message.</summary>
        public const int WM_XBUTTONUP        =        0x020C;
        /// <summary>WM_XBUTTONDBLCLK message.</summary>
        public const int WM_XBUTTONDBLCLK    =        0x020D;
        /// <summary>WM_MOUSELAST message.</summary>
        public const int WM_MOUSELAST        =        0x020D;

        /// <summary>WM_HOTKEY message.</summary>
        public const int WM_HOTKEY           =        0x0312;

        /// <summary>WM_NCLBUTTONDOWN message.</summary>
        public const int WM_NCLBUTTONDOWN    =        0x00A1;
        /// <summary>WM_NCLBUTTONUP message.</summary>
        public const int WM_NCLBUTTONUP      =        0x00A2;
        /// <summary>WM_NCLBUTTONDBLCLK message.</summary>
        public const int WM_NCLBUTTONDBLCLK  =        0x00A3;
        /// <summary>WM_NCRBUTTONDOWN message.</summary>
        public const int WM_NCRBUTTONDOWN    =        0x00A4;
        /// <summary>WM_NCRBUTTONUP message.</summary>
        public const int WM_NCRBUTTONUP      =        0x00A5;
        /// <summary>WM_NCRBUTTONDBLCLK message.</summary>
        public const int WM_NCRBUTTONDBLCLK  =        0x00A6;
        /// <summary>WM_NCMBUTTONDOWN message.</summary>
        public const int WM_NCMBUTTONDOWN    =        0x00A7;
        /// <summary>WM_NCMBUTTONUP message.</summary>
        public const int WM_NCMBUTTONUP      =        0x00A8;
        /// <summary>WM_NCMBUTTONDBLCLK message.</summary>
        public const int WM_NCMBUTTONDBLCLK  =        0x00A9;
        /// <summary>WM_NCXBUTTONDOWN message.</summary>
        public const int WM_NCXBUTTONDOWN    =        0x00AB;
        /// <summary>WM_NCXBUTTONUP message.</summary>
        public const int WM_NCXBUTTONUP      =        0x00AC;
        /// <summary>WM_NCXBUTTONDBLCLK message.</summary>
        public const int WM_NCXBUTTONDBLCLK  =        0x00AD;

        /// <summary>Gets a string with the message description.</summary>
        /// <param name="msg">Message record.</param>
        /// <returns>A string description of the message.</returns>
        public static string GetMessageDescription(MSG msg)
        {
            switch (msg.message.ToUInt32())
            {
                case WM_NULL: return "WM_NULL";
                case WM_CREATE: return "WM_CREATE";
                case WM_DESTROY: return "WM_DESTROY";
                case WM_MOVE: return "WM_MOVE";
                case WM_SIZE: return "WM_SIZE";
                case WM_ACTIVATE: return "WM_ACTIVATE";
                case WM_SETFOCUS: return "WM_SETFOCUS";
                case WM_KILLFOCUS: return "WM_KILLFOCUS";
                case WM_ENABLE: return "WM_ENABLE";
                case WM_SETREDRAW: return "WM_SETREDRAW";
                case WM_SETTEXT: return "WM_SETTEXT";
                case WM_GETTEXT: return "WM_GETTEXT";
                case WM_GETTEXTLENGTH: return "WM_GETTEXTLENGTH";
                case WM_PAINT: return "WM_PAINT";
                case WM_CLOSE: return "WM_CLOSE";
                case WM_QUIT: return "WM_QUIT";
                case WM_ERASEBKGND: return "WM_ERASEBKGND";
                case WM_ACTIVATEAPP: return "WM_ACTIVATEAPP";
                case WM_SETCURSOR: return "WM_SETCURSOR";
                case WM_MOUSEACTIVATE: return "WM_MOUSEACTIVATE";
                case WM_SETFONT: return "WM_SETFONT";
                case WM_GETOBJECT: return "WM_GETOBJECT";
                case WM_WINDOWPOSCHANGED: return "WM_WINDOWPOSCHANGED";
                case WM_INPUTLANGCHANGE: return "WM_INPUTLANGCHANGE";
                case WM_DISPLAYCHANGE: return "WM_DISPLAYCHANGE";
                case WM_NCHITTEST: return "WM_NCHITTEST";
                case WM_PRINTCLIENT: return "WM_PRINTCLIENT";

                case WM_CAPTURECHANGED: return "WM_CAPTURECHANGED";
                case WM_EXITSIZEMOVE: return "WM_EXITSIZEMOVE";
                case WM_USER: return "WM_USER";

                case WM_NCCREATE: return "WM_NCCREATE";
                case WM_NCDESTROY: return "WM_NCDESTROY";

                case WM_KEYDOWN: return "WM_KEYDOWN";
                case WM_KEYUP: return "WM_KEYUP";
                case WM_CHAR: return "WM_CHAR";
                case WM_SYSKEYDOWN: return "WM_SYSKEYDOWN";
                case WM_SYSKEYUP: return "WM_SYSKEYUP";
                case WM_SYSCHAR: return "WM_SYSCHAR";

                case WM_COMMAND: return "WM_COMMAND";
                case WM_TIMER: return "WM_TIMER";
                case WM_HSCROLL: return "WM_HSCROLL";

                case WM_MOUSEHOVER: return "WM_MOUSEHOVER";
                case WM_MOUSELEAVE: return "WM_MOUSELEAVE";

                //case Win32.WM_MOUSEFIRST: return "WM_MOUSEFIRST";
                case WM_MOUSEMOVE: return "WM_MOUSEMOVE";
                case WM_LBUTTONDOWN: return "WM_LBUTTONDOWN";
                case WM_LBUTTONUP: return "WM_LBUTTONUP";
                case WM_LBUTTONDBLCLK: return "WM_LBUTTONDBLCLK";
                case WM_RBUTTONDOWN: return "WM_RBUTTONDOWN";
                case WM_RBUTTONUP: return "WM_RBUTTONUP";
                case WM_RBUTTONDBLCLK: return "WM_RBUTTONDBLCLK";
                case WM_MBUTTONDOWN: return "WM_MBUTTONDOWN";
                case WM_MBUTTONUP: return "WM_MBUTTONUP";
                case WM_MBUTTONDBLCLK: return "WM_MBUTTONDBLCLK";
                case WM_MOUSEWHEEL: return "WM_MOUSEWHEEL";
                case WM_XBUTTONDOWN: return "WM_XBUTTONDOWN";
                case WM_XBUTTONUP: return "WM_XBUTTONUP";
                case WM_XBUTTONDBLCLK: return "WM_XBUTTONDBLCLK";
                //case Win32.WM_MOUSELAST: return "WM_MOUSELAST";

                case WM_HOTKEY: return "WM_HOTKEY";

                case WM_NCLBUTTONDOWN: return "WM_NCLBUTTONDOWN";
                case WM_NCLBUTTONUP: return "WM_NCLBUTTONUP";
                case WM_NCLBUTTONDBLCLK: return "WM_NCLBUTTONDBLCLK";
                case WM_NCRBUTTONDOWN: return "WM_NCRBUTTONDOWN";
                case WM_NCRBUTTONUP: return "WM_NCRBUTTONUP";
                case WM_NCRBUTTONDBLCLK: return "WM_NCRBUTTONDBLCLK";
                case WM_NCMBUTTONDOWN: return "WM_NCMBUTTONDOWN";
                case WM_NCMBUTTONUP: return "WM_NCMBUTTONUP";
                case WM_NCMBUTTONDBLCLK: return "WM_NCMBUTTONDBLCLK";
                case WM_NCXBUTTONDOWN: return "WM_NCXBUTTONDOWN";
                case WM_NCXBUTTONUP: return "WM_NCXBUTTONUP";
                case WM_NCXBUTTONDBLCLK: return "WM_NCXBUTTONDBLCLK";
            }
            return msg.message.ToString();
        }
        #endregion Messaging.
    }
}
