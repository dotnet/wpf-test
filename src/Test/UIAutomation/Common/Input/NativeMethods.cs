// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//***************************************************************************
// HOW TO USE THIS FILE
//
// If you need access to a Win32 API that is not exposed, simply uncomment
// it in one of the following files:
// 
//
// Only uncomment what you need to avoid code bloat.
//
// DO NOT adjust the visibility of anything in these files.  They are marked
// internal on pupose.
//***************************************************************************

using System;
using System.Runtime.InteropServices;

namespace MS.Win32
{
    internal class NativeMethods
    {
        internal const int WM_USER = 0x0400;

        internal const int SMTO_BLOCK = 0x0001;

        // ListView rectangle related constants
        internal const int LVIR_BOUNDS = 0;
        internal const int LVIR_ICON = 1;
        internal const int LVIR_LABEL = 2;
        internal const int LVIR_SELECTBOUNDS = 3;

        // List-view messages
        internal const int LVM_FIRST = 0x1000;
        internal const int LVM_GETITEMCOUNT = LVM_FIRST + 4;
        internal const int LVM_GETNEXTITEM = LVM_FIRST + 12;
        internal const int LVM_GETITEMRECT = LVM_FIRST + 14;
        internal const int LVM_GETITEMPOSITION = LVM_FIRST + 16;
        internal const int LVM_ENSUREVISIBLE = LVM_FIRST + 19;
        internal const int LVM_SCROLL = LVM_FIRST + 20;
        internal const int LVM_GETHEADER = LVM_FIRST + 31;
        internal const int LVM_GETITEMSTATE = LVM_FIRST + 44;
        internal const int LVM_SETITEMSTATE = LVM_FIRST + 43;
        internal const int LVM_GETEXTENDEDLISTVIEWSTYLE = LVM_FIRST + 55;
        internal const int LVM_GETSUBITEMRECT = LVM_FIRST + 56;
        internal const int LVM_SUBITEMHITTEST = LVM_FIRST + 57;
        internal const int LVM_APPROXIMATEVIEWRECT = LVM_FIRST + 64;
        internal const int LVM_GETITEMW = LVM_FIRST + 75;
        internal const int LVM_GETTOOLTIPS = LVM_FIRST + 78;
        internal const int LVM_EDITLABEL = LVM_FIRST + 118;
        internal const int LVM_GETVIEW = LVM_FIRST + 143;
        internal const int LVM_SETVIEW = LVM_FIRST + 142;
        internal const int LVM_GETGROUPINFO = LVM_FIRST + 149;
        internal const int LVM_GETGROUPMETRICS = LVM_FIRST + 156;
        internal const int LVM_HASGROUP = LVM_FIRST + 161;
        internal const int LVM_ISGROUPVIEWENABLED = LVM_FIRST + 175;

        // Listbox messages
        internal const int LB_ERR = -1;
        internal const int LB_SETSEL = 0x0185;
        internal const int LB_SETCURSEL = 0x0186;
        internal const int LB_GETSEL = 0x0187;
        internal const int LB_GETCURSEL = 0x0188;
        internal const int LB_GETTEXT = 0x0189;
        internal const int LB_GETTEXTLEN = 0x018A;
        internal const int LB_GETCOUNT = 0x018B;
        internal const int LB_GETSELCOUNT = 0x0190;
        internal const int LB_SETTOPINDEX = 0x0197;
        internal const int LB_GETITEMRECT = 0x0198;
        internal const int LB_GETITEMDATA = 0x0199;
        internal const int LB_SETCARETINDEX = 0x019E;
        internal const int LB_GETCARETINDEX = 0x019F;
        internal const int LB_ITEMFROMPOINT = 0x01A9;

        //  SpinControl
        internal const int UDM_GETRANGE = (WM_USER + 102);
        internal const int UDM_SETPOS = (WM_USER + 103);
        internal const int UDM_GETPOS = (WM_USER + 104);
        internal const int UDM_GETBUDDY = (WM_USER + 106);

        // System Metrics
        internal const int SM_CXSCREEN = 0;
        internal const int SM_CYSCREEN = 1;
        internal const int SM_CXVSCROLL = 2;
        internal const int SM_CYHSCROLL = 3;
        internal const int SM_CYCAPTION = 4;
        internal const int SM_CXBORDER = 5;
        internal const int SM_CYBORDER = 6;
        internal const int SM_CYVTHUMB = 9;
        internal const int SM_CXHTHUMB = 10;
        internal const int SM_CXICON = 11;
        internal const int SM_CYICON = 12;
        internal const int SM_CXCURSOR = 13;
        internal const int SM_CYCURSOR = 14;
        internal const int SM_CYMENU = 15;
        internal const int SM_CYKANJIWINDOW = 18;
        internal const int SM_MOUSEPRESENT = 19;
        internal const int SM_CYVSCROLL = 20;
        internal const int SM_CXHSCROLL = 21;
        internal const int SM_DEBUG = 22;
        internal const int SM_SWAPBUTTON = 23;
        internal const int SM_CXMIN = 28;
        internal const int SM_CYMIN = 29;
        internal const int SM_CXSIZE = 30;
        internal const int SM_CYSIZE = 31;
        internal const int SM_CXFRAME = 32;
        internal const int SM_CYFRAME = 33;
        internal const int SM_CXMINTRACK = 34;
        internal const int SM_CYMINTRACK = 35;
        internal const int SM_CXDOUBLECLK = 36;
        internal const int SM_CYDOUBLECLK = 37;
        internal const int SM_CXICONSPACING = 38;
        internal const int SM_CYICONSPACING = 39;
        internal const int SM_MENUDROPALIGNMENT = 40;
        internal const int SM_PENWINDOWS = 41;
        internal const int SM_DBCSENABLED = 42;
        internal const int SM_CMOUSEBUTTONS = 43;
        internal const int SM_CXFIXEDFRAME = 7;
        internal const int SM_CYFIXEDFRAME = 8;
        internal const int SM_SECURE = 44;
        internal const int SM_CXEDGE = 45;
        internal const int SM_CYEDGE = 46;
        internal const int SM_CXMINSPACING = 47;
        internal const int SM_CYMINSPACING = 48;
        internal const int SM_CXSMICON = 49;
        internal const int SM_CYSMICON = 50;
        internal const int SM_CYSMCAPTION = 51;
        internal const int SM_CXSMSIZE = 52;
        internal const int SM_CYSMSIZE = 53;
        internal const int SM_CXMENUSIZE = 54;
        internal const int SM_CYMENUSIZE = 55;
        internal const int SM_ARRANGE = 56;
        internal const int SM_CXMINIMIZED = 57;
        internal const int SM_CYMINIMIZED = 58;
        internal const int SM_CXMAXTRACK = 59;
        internal const int SM_CYMAXTRACK = 60;
        internal const int SM_CXMAXIMIZED = 61;
        internal const int SM_CYMAXIMIZED = 62;
        internal const int SM_NETWORK = 63;
        internal const int SM_CLEANBOOT = 67;
        internal const int SM_CXDRAG = 68;
        internal const int SM_CYDRAG = 69;
        internal const int SM_SHOWSOUNDS = 70;
        internal const int SM_CXMENUCHECK = 71;
        internal const int SM_CYMENUCHECK = 72;
        internal const int SM_MIDEASTENABLED = 74;
        internal const int SM_MOUSEWHEELPRESENT = 75;
        internal const int SM_XVIRTUALSCREEN = 76;
        internal const int SM_YVIRTUALSCREEN = 77;
        internal const int SM_CXVIRTUALSCREEN = 78;
        internal const int SM_CYVIRTUALSCREEN = 79;

        internal const int S_OK = 0x00000000;
        internal const int S_FALSE = 0x00000001;

        // Editbox styles
        internal const int ES_READONLY = 0x0800;
        internal const int ES_NUMBER = 0x2000;

        // Editbox messages
        internal const int WM_SETTEXT = 0x000C;
        internal const int EM_GETLIMITTEXT = 0x00D5;
        internal const int WM_GETTEXTLENGTH = 0x000E;
        internal const int EM_LIMITTEXT = 0x00C5;

        public struct Point 
        {
            internal double _x;
            internal double _y;
        
            /// <summary>
            /// Constructor which accepts the X and Y values
            /// </summary>
            /// <param name="x">The value for the X coordinate of the new Point</param>
            /// <param name="y">The value for the Y coordinate of the new Point</param>
            public Point(double x, double y)
            {
                _x = x;
                _y = y;
            }
            /// <summary>
            /// X - Default value is 0.
            /// </summary>
            public double X
            {
                get{return _x;}
                set{_x = value;}
            }

            /// <summary>
            /// Y - Default value is 0.
            /// </summary>
            public double Y
            {
                get{return _y;}
                set{_y = value;}
            }
        }

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

        [StructLayout(LayoutKind.Sequential)]
        internal struct SIZE
        {
            internal int cx;
            internal int cy;

            internal SIZE(int cx, int cy)
            {
                this.cx = cx;
                this.cy = cy;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public bool IsEmpty
            {
                get
                {
                    return left >= right || top >= bottom;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Rect
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;

            internal Win32Rect(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            internal Win32Rect(Win32Rect rcSrc)
            {
                this.left = rcSrc.left;
                this.top = rcSrc.top;
                this.right = rcSrc.right;
                this.bottom = rcSrc.bottom;
            }

            internal bool IsEmpty
            {
                get
                {
                    return left >= right || top >= bottom;
                }
            }

            static internal Win32Rect Empty
            {
                get
                {
                    return new Win32Rect(0, 0, 0, 0);
                }
            }

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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal sealed class OSVERSIONINFOEX
        {
            public OSVERSIONINFOEX()
            {
                osVersionInfoSize = (int)Marshal.SizeOf(this);
            }

            // The OSVersionInfoSize field must be set to SecurityHelper.SizeOf(this)
            public int osVersionInfoSize = 0;
            public int majorVersion = 0;
            public int minorVersion = 0;
            public int buildNumber = 0;
            public int platformId = 0;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string csdVersion = null;
            public short servicePackMajor = 0;
            public short servicePackMinor = 0;
            public short suiteMask = 0;
            public byte productType = 0;
            public byte reserved = 0;
        }

    }
}

