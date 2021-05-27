// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;

using MS.Win32;

namespace DRT
{
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


    /////////////////////////////////////////////////////////////////////////
    [ComImport, Guid("A5B020FD-E04B-4e67-B65A-E7DEED25B2CF") ]
    public class WisptisMgrDrt {}

    /////////////////////////////////////////////////////////////////////////

    [ComImport, Guid("A56AB812-2AC7-443d-A87A-F1EE1CD5A0E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown) ]
    interface ITabletManagerDrt
    {
        bool IsTabletPresent(string sTablet);
        void SimulatePacket(string sTablet, int x, int y, [MarshalAs(UnmanagedType.Bool)] bool fCursorDown);
        void EnablePacketsTransfer([MarshalAs(UnmanagedType.Bool)] bool fEnable);
        void SimulateCursorInRange(uint cursorKey);
        void SimulateCursorOutOfRange(uint cursorKey);
        void GetTabletRectangle(string sTablet, out RECT rect);
        void FindTablet(string sTablet, out ulong iTablet);
        void SimulatePacketWithButton(string sTablet, int x, int y, [MarshalAs(UnmanagedType.Bool)] bool fCursorDown, [MarshalAs(UnmanagedType.Bool)] bool fButtonDown);
        void SimulateCursorInRangeForTablet(string sTablet, uint cursorKey);
        void SimulateCursorOutOfRangeForTablet(string sTablet, uint cursorKey);
        void EnsureTablet(string sTablet);
    }

    /////////////////////////////////////////////////////////////////////////

    public class PenDrtManager
    {
        /////////////////////////////////////////////////////////////////////

        ITabletManagerDrt   m_wisptisDrtMgr;
        bool                m_loggingEnabled = false;
        bool                m_logTime = false;
        bool                m_logPacketsSent = false;
        int                 m_packetCount = 0;
		
        /////////////////////////////////////////////////////////////////////

        public PenDrtManager()
        {
            object wisptisDrtMgrObj = new WisptisMgrDrt();
            m_wisptisDrtMgr = (ITabletManagerDrt)wisptisDrtMgrObj;
        }

        /////////////////////////////////////////////////////////////////////

        public void ConnectToWisptisDrt()
        {
            IsTabletPresent("Mouse");
        }

        /////////////////////////////////////////////////////////////////////

        public void EnableLogging(bool enable, bool logTime, bool logPacketsSent)
        {
            m_loggingEnabled = enable;
            m_logTime = logTime;
            m_logPacketsSent = logPacketsSent;
        }

        /////////////////////////////////////////////////////////////////////

        public void ResetPacketSentCount()
        {
            m_packetCount = 0;
            LogPacket("  *PacketInjected* Reset to 0.", false);
        }

        /////////////////////////////////////////////////////////////////////

        public void EnsureTablet(string sTablet)
        {
            LogTime("EnsureTablet enter");
            m_wisptisDrtMgr.EnsureTablet(sTablet);
            LogTime("EnsureTablet exit");
        }

        /////////////////////////////////////////////////////////////////////

        public bool IsTabletPresent(string sTablet)
        {
            LogTime("IsTabletPresent enter");
            bool isTabletPresent = m_wisptisDrtMgr.IsTabletPresent(sTablet);
            LogTime("IsTabletPresent exit");
            return isTabletPresent;
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulatePacket(string sTablet, int x, int y, bool fCursorDown)
        {
            LogTime("SimulatePacket enter");
            m_wisptisDrtMgr.SimulatePacket(sTablet, x, y, fCursorDown);
            LogPacket(FormatPacket(sTablet, x, y, fCursorDown, false), true);
            LogTime("SimulatePacket exit");
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulatePacketWithButton(string sTablet, int x, int y, bool fCursorDown, bool fButtonDown)
        {
            LogTime("SimulatePacketWithButton enter");
            m_wisptisDrtMgr.SimulatePacketWithButton(sTablet, x, y, fCursorDown, fButtonDown);
            LogPacket(FormatPacket(sTablet, x, y, fCursorDown, fButtonDown), true);
            LogTime("SimulatePacketWithButton exit");
        }

        /////////////////////////////////////////////////////////////////////

        public void EnablePacketsTransfer(bool f)
        {
            LogTime("EnablePacketsTransfer enter");
            m_wisptisDrtMgr.EnablePacketsTransfer(f);
            LogTime("EnablePacketsTransfer exit");
        }

        /////////////////////////////////////////////////////////////////////

        public void GetTabletRectangle(string sTablet, out Rect rect)
        {
            RECT rc0;
            LogTime("GetTabletRectangle enter");
            m_wisptisDrtMgr.GetTabletRectangle(sTablet, out rc0);
            rect = new Rect(rc0.left, rc0.top, rc0.right - rc0.left, rc0.bottom - rc0.top);
            LogTime("GetTabletRectangle exit");
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateCursorInRange(uint cursorId)
        {
            LogTime("SimulateCursorInRange enter");
            m_wisptisDrtMgr.SimulateCursorInRange(cursorId);
            LogPacket("SimulateCursorInRange cursorId=" + cursorId.ToString(), false);
            LogTime("SimulateCursorInRange exit");
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateCursorOutOfRange(uint cursorId)
        {
            LogTime("SimulateCursorOutOfRange enter");
            m_wisptisDrtMgr.SimulateCursorOutOfRange(cursorId);
            LogPacket("SimulateCursorOutOfRange cursorId=" + cursorId.ToString(), false);
            LogTime("SimulateCursorOutOfRange exit");
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateCursorInRangeForTablet(string sTablet, uint cursorId)
        {
            LogTime("SimulateCursorInRangeForTablet enter");
            m_wisptisDrtMgr.SimulateCursorInRangeForTablet(sTablet, cursorId);
            LogPacket("SimulateCursorInRangeForTablet Tablet=" + sTablet + " cursorId=" + cursorId.ToString(), false);
            LogTime("SimulateCursorInRangeForTablet exit");
        }

        /////////////////////////////////////////////////////////////////////

        public void SimulateCursorOutOfRangeForTablet(string sTablet, uint cursorId)
        {
            LogTime("SimulateCursorOutOfRangeForTablet enter");
            m_wisptisDrtMgr.SimulateCursorOutOfRangeForTablet(sTablet, cursorId);
            LogPacket("SimulateCursorOutOfRangeForTablet Tablet=" + sTablet + " cursorId=" + cursorId.ToString(), false);
            LogTime("SimulateCursorOutOfRangeForTablet exit");
        }

        /////////////////////////////////////////////////////////////////////

        private string FormatPacket(string sTablet, int x, int y, bool fCursorDown, bool fButtonDown)
        {
            string s = String.Format("  *PacketInjected* Tablet={0} ({1},{2}) CursorDown={3}, ButtonDown={4}", 
                                        sTablet, x.ToString(), y.ToString(), 
                                        fCursorDown.ToString(), fButtonDown.ToString());
            return s;
        }
        
        /////////////////////////////////////////////////////////////////////

        private void LogPacket(string sfunction, bool updateCount)
        {
            if (updateCount)
                m_packetCount += 1;
            
            if (m_logPacketsSent)
            {
                string s = sfunction + (updateCount ? " [PacketCountSent=" + m_packetCount.ToString() + "]" : "");
                
                if (m_logTime)
                {
                    DateTime now = DateTime.Now;
                    s = s + String.Format(" [Time:{0}:{1:00}:{2:00}:{3:00}]", now.Hour, now.Minute, now.Second, now.Millisecond);
                }
                
                Console.WriteLine(s);
            }
        }
	
        /////////////////////////////////////////////////////////////////////

        private void LogTime(string sfunction)
        {
            if (m_loggingEnabled && m_logTime) // put under log time till another cmd line option is added.
            {
                string s = sfunction;
                
                if (m_logTime)
                {
                    DateTime now = DateTime.Now;
                    s = s + String.Format(" [time:{0}:{1:00}:{2:00}:{3:00}]", sfunction, now.Hour, now.Minute, now.Second, now.Millisecond);
                }
                
                Console.WriteLine(s);
            }
        }
	
    }
}
