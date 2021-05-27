// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using System.Security.Permissions;
using System.Runtime.InteropServices;

using System.Windows;
using System.Windows.Automation;
using System.Windows.Interop;
using DRT; // Input class

using MS.Win32;
using MS.Internal;

namespace StylusInputSimulation
{
    /////////////////////////////////////////////////////////////////////////

    public class Mouse
    {
        /////////////////////////////////////////////////////////////////////

        public Mouse()
        {
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateMove(HwndSource window, double x, double y)
        {
            SimulateMove(window, new Point(x, y));
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateMove(HwndSource window, Point ptClient)
        {
            Point ptScreen = PointUtil.ClientToScreen(ptClient, window);
            Input.MoveTo(ptScreen);
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateLeftDown(HwndSource window, double x, double y)
        {
            SimulateLeftDown(window, new Point(x, y));
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateLeftDown(HwndSource window, Point ptClient)
        {
            Point ptScreen = PointUtil.ClientToScreen(ptClient, window);
            Input.SendMouseInput(ptScreen.X, ptScreen.Y, 0, SendMouseInputFlags.LeftDown);
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateLeftUp(HwndSource window, double x, double y)
        {
            SimulateLeftUp(window, new Point(x, y));
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateLeftUp(HwndSource window, Point ptClient)
        {
            Point ptScreen = PointUtil.ClientToScreen(ptClient, window);
            Input.SendMouseInput(ptScreen.X, ptScreen.Y, 0, SendMouseInputFlags.LeftUp);
        }

        /////////////////////////////////////////////////////////////////////
        public void SimulateLeftDoubleClick(HwndSource window, Point ptClient)
        {
            Point ptScreen = PointUtil.ClientToScreen(ptClient, window);
            Input.SendMouseInput(ptScreen.X, ptScreen.Y, 0, SendMouseInputFlags.LeftDown);
            Input.SendMouseInput(ptScreen.X, ptScreen.Y, 0, SendMouseInputFlags.LeftUp);
            Input.SendMouseInput(ptScreen.X, ptScreen.Y, 0, SendMouseInputFlags.LeftDown);
            Input.SendMouseInput(ptScreen.X, ptScreen.Y, 0, SendMouseInputFlags.LeftUp);
        }
    }

    /////////////////////////////////////////////////////////////////////////

    public class Hid
    {
        /////////////////////////////////////////////////////////////////////

        public Hid()
        {
            try
            {
                object o = new WisptisMgrDrt();
                _wisptis = (ITabletManagerDrt)o; 
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not connect to DRT version of wisptis. Check that wisptis_.exe is present and running (note the '_').");
                Console.WriteLine(e.ToString());
                throw e;
            }
            _wisptis.EnsureTablet("HidDrt");
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateDrtPacket(HwndSource window, double x, double y, bool fStylusDown)
        {
            SimulateDrtPacket(window, new Point(x,y), fStylusDown);
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateDrtPacket(HwndSource window, Point ptClient, bool fStylusDown)
        {
            Point ptScreen = PointUtil.ClientToScreen(ptClient, window);
            Point ptTablet = TabletPointFromScreenPoint(ptScreen);
            _wisptis.SimulatePacketWithButton("HidDrt", (int)ptTablet.X, (int)ptTablet.Y, fStylusDown, /*fButtonDown*/false);
        }

        /////////////////////////////////////////////////////////////////////

        Point TabletPointFromScreenPoint(Point ptScreen)
        {
            if (!_fScreenMeasured)
            {
                _fScreenMeasured = true;
                _rcScreen = H.VirtualScreen;
            }

            if (!_fDrtTabletMeasured)
            {
                _fDrtTabletMeasured = true;
                NativeMethods.RECT rc;
                _wisptis.GetTabletRectangle("HidDrt", out rc);
                _rcDrtTablet = new Rect(rc.left, rc.top, rc.right - rc.left, rc.bottom - rc.top);
            }

            int xTablet = (int)(0.5 + _rcDrtTablet.Left + (ptScreen.X - _rcScreen.Left) * _rcDrtTablet.Width  / _rcScreen.Width );
            int yTablet = (int)(0.5 + _rcDrtTablet.Top  + (ptScreen.Y - _rcScreen.Top)  * _rcDrtTablet.Height / _rcScreen.Height);

            return new Point(xTablet, yTablet);
        }

        /////////////////////////////////////////////////////////////////////
        [SuppressUnmanagedCodeSecurity()]
        [ComImport, Guid("A5B020FD-E04B-4e67-B65A-E7DEED25B2CF") ]
        class WisptisMgrDrt {}

        /////////////////////////////////////////////////////////////////////////
        [SuppressUnmanagedCodeSecurity()]
        [ComImport, Guid("A56AB812-2AC7-443d-A87A-F1EE1CD5A0E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown) ]
        interface ITabletManagerDrt
        {
            bool IsTabletPresent(string sTablet);
            void SimulatePacket(string sTablet, int x, int y, [MarshalAs(UnmanagedType.Bool)] bool fCursorDown);
            void EnablePacketsTransfer([MarshalAs(UnmanagedType.Bool)] bool fEnable);
            void SimulateCursorInRange(uint cursorKey);
            void SimulateCursorOutOfRange(uint cursorKey);
            void GetTabletRectangle(string sTablet, out NativeMethods.RECT rect);
            void FindTablet(string sTablet, out ulong iTablet);
            void SimulatePacketWithButton(string sTablet, int x, int y, [MarshalAs(UnmanagedType.Bool)] bool fCursorDown, [MarshalAs(UnmanagedType.Bool)] bool fButtonDown);
            void SimulateCursorInRangeForTablet(string sTablet, uint cursorKey);
            void SimulateCursorOutOfRangeForTablet(string sTablet, uint cursorKey);
            void EnsureTablet(string sTablet);
        }
 
        /////////////////////////////////////////////////////////////////////////

        ITabletManagerDrt   _wisptis;
        bool                _fScreenMeasured;
        Rect                _rcScreen;
        bool                _fDrtTabletMeasured;
        Rect                _rcDrtTablet;
    }

    /////////////////////////////////////////////////////////////////////////
    
    internal class H // protected helper
    {
        /////////////////////////////////////////////////////////////////////

        internal static Rect VirtualScreen
        {
            get
            {
                return new Rect(
                    InternalUnsafeNativeMethods.GetSystemMetrics(InternalNativeMethods.SM_XVIRTUALSCREEN),
                    InternalUnsafeNativeMethods.GetSystemMetrics(InternalNativeMethods.SM_YVIRTUALSCREEN),
                    InternalUnsafeNativeMethods.GetSystemMetrics(InternalNativeMethods.SM_CXVIRTUALSCREEN),
                    InternalUnsafeNativeMethods.GetSystemMetrics(InternalNativeMethods.SM_CYVIRTUALSCREEN));
            }
        } 
    }
}
