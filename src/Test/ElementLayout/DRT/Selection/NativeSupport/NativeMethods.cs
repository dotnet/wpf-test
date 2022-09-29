// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//***************************************************************************
// HOW TO USE THIS FILE
//
// If you need access to a Win32 API that is not exposed, simply uncomment
// it in one of the following files:
// 
// NativeMethods.cs
// UnsafeNativeMethods.cs
// SafeNativeMethods.cs
//
// Only uncomment what you need to avoid code bloat.
//
// DO NOT adjust the visibility of anything in these files.  They are marked
// internal on pupose.
//***************************************************************************


namespace MS.Win32
{
    using Accessibility;
    using System.Runtime.InteropServices;
    using System;
    using System.Security.Permissions;
    using System.Collections;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using MS.Win32;
    
    internal class NativeMethods
    {
        public const int 
            SPI_GETNONCLIENTMETRICS = 0x0029,
            SPI_SETNONCLIENTMETRICS = 0x002A;

        [StructLayout(LayoutKind.Sequential)]
        public class NONCLIENTMETRICS
        {
            public int cbSize = Marshal.SizeOf(typeof(NONCLIENTMETRICS));
            public int iBorderWidth = 0;
            public int iScrollWidth = 0;
            public int iScrollHeight = 0;
            public int iCaptionWidth = 0;
            public int iCaptionHeight = 0;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfCaptionFont = null;
            public int iSmCaptionWidth = 0;
            public int iSmCaptionHeight = 0;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfSmCaptionFont = null;
            public int iMenuWidth = 0;
            public int iMenuHeight = 0;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfMenuFont = null;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfStatusFont = null;
            [MarshalAs(UnmanagedType.Struct)]
            public LOGFONT lfMessageFont = null;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class LOGFONT
        {
            public int lfHeight = 0;
            public int lfWidth = 0;
            public int lfEscapement = 0;
            public int lfOrientation = 0;
            public int lfWeight = 0;
            public byte lfItalic = 0;
            public byte lfUnderline = 0;
            public byte lfStrikeOut = 0;
            public byte lfCharSet = 0;
            public byte lfOutPrecision = 0;
            public byte lfClipPrecision = 0;
            public byte lfQuality = 0;
            public byte lfPitchAndFamily = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string lfFaceName = null;
        }
    }
}

