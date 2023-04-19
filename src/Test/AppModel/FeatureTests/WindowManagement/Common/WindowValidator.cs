// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon
{
    using System;
    using System.Windows;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    public class WindowValidator
    {
        #region Win32 Enums
        [Flags]
        enum WindowMessages
        {
            WM_QUERYENDSESSION = 0x11
        }

        enum SessionEndingReasons
        {
            ENDINGSESSION_SHUTDOWN = 0x0,
            ENDINGSESSION_CLOSEAPP = 0x1,
            ENDINGSESSION_LOGOFF = 0x1
        }

        enum WS : long
        {
            WS_OVERLAPPED = 0x00000000L,
            WS_POPUP = 0x80000000L,
            WS_CHILD = 0x40000000L,
            WS_CLIPSIBLINGS = 0x04000000L,
            WS_CLIPCHILDREN = 0x02000000L,
            WS_VISIBLE = 0x10000000L,
            WS_DISABLED = 0x08000000L,
            WS_MINIMIZE = 0x20000000L,
            WS_MAXIMIZE = 0x01000000L,
            WS_CAPTION = 0x00C00000L,
            WS_BORDER = 0x00800000L,
            WS_DLGFRAME = 0x00400000L,
            WS_VSCROLL = 0x00200000L,
            WS_HSCROLL = 0x00100000L,
            WS_SYSMENU = 0x00080000L,
            WS_THICKFRAME = 0x00040000L,
            WS_MINIMIZEBOX = 0x00020000L,
            WS_MAXIMIZEBOX = 0x00010000L,
            WS_GROUP = 0x00020000L,
            WS_TABSTOP = 0x00010000L
        }

        [Flags]
        enum ExWS : long
        {
            WS_EX_DLGMODALFRAME = 0x00000001L,
            WS_EX_NOPARENTNOTIFY = 0x00000004L,
            WS_EX_TOPMOST = 0x00000008L,
            WS_EX_ACCEPTFILES = 0x00000010L,
            WS_EX_TRANSPARENT = 0x00000020L,
            WS_EX_NOINHERITLAYOUT = 0x00100000L,
            WS_EX_LAYOUTRTL = 0x00400000L,
            WS_EX_MDICHILD = 0x00000040L,
            WS_EX_TOOLWINDOW = 0x00000080L,
            WS_EX_WINDOWEDGE = 0x00000100L,
            WS_EX_CLIENTEDGE = 0x00000200L,
            WS_EX_CONTEXTHELP = 0x00000400L,
            WS_EX_RIGHT = 0x00001000L,
            WS_EX_LEFT = 0x00000000L,
            WS_EX_RTLREADING = 0x00002000L,
            WS_EX_LTRREADING = 0x00000000L,
            WS_EX_LEFTSCROLLBAR = 0x00004000L,
            WS_EX_RIGHTSCROLLBAR = 0x00000000L,
            WS_EX_CONTROLPARENT = 0x00010000L,
            WS_EX_STATICEDGE = 0x00020000L,
            WS_EX_APPWINDOW = 0x00040000L
        }

        struct WINDOWPLACEMENT
        {
            public uint length;
            public uint flags;
            public uint showCmd;
            public Point ptMinPosition;
            public Point ptMaxPosition;
            public Rect rcNormalPosition;
        }

        struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        enum WINDOWPLACEMENT_showCmd : uint
        {
            /* Values taken from window.h */
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_MAX = 9
        }

        enum GetWindowLongPtr_nIndex
        {
            GWL_WNDPROC = -4,
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20,
        }

        enum GetWindow_uCmd
        {
            /* Taken from winuser.h */
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
        }
        #endregion

        #region Win32 API Imports
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow([In] string className, [In] string windowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindowEx(IntPtr hWnd, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Auto,
#if WIN64
        EntryPoint="GetWindowLongPtr"
#else
 EntryPoint = "GetWindowLong"
#endif
)]
        private static extern IntPtr GetWindowLongRedirected(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT windowPlacement);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetClientRect(IntPtr hWnd, ref RECT lpRect);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);
        #endregion

        #region Private Methods
        private static bool ContainExStyle(string windowTitle, ExWS ws)
        {
            Logger.Status("  [ContainExStyle] Entered. [Expected ExWindowStyle=" + ws.ToString() + "]");
            IntPtr winStyle = GetWindowLongRedirected(FindWindowProc(null, windowTitle), (int)GetWindowLongPtr_nIndex.GWL_EXSTYLE);
#if WIN64
            return ((winStyle.ToInt64() & (long)ws) == (long)ws);
#else
            return ((winStyle.ToInt32() & (uint)ws) == (uint)ws);
#endif
        }

        private static bool ContainStyle(string windowTitle, WS ws)
        {
            Logger.Status("  [ContainStyle] Entered. [Expected WindowStyle=" + ws.ToString() + "]");
            IntPtr winStyle = GetWindowLongRedirected(FindWindowProc(null, windowTitle), (int)GetWindowLongPtr_nIndex.GWL_STYLE);
#if WIN64
            return ((winStyle.ToInt64() & (long)ws) == (long)ws);
#else
            return ((winStyle.ToInt32() & (uint)ws) == (uint)ws);
#endif
        }

        private static IntPtr FindWindowProc(string className, string windowTitle)
        {
            Logger.Status("  [FindWindowProc] Entered. [ClassName=" + (String.IsNullOrEmpty(className) ? "null" : className) + "] [WindowName=" + windowTitle + "]");
            IntPtr AvaWin = FindWindow(className, windowTitle);
            if (IntPtr.Zero.Equals(AvaWin))
            {
                Logger.Status("  [FindWindowProc] Cannot find Window with Title=" + windowTitle);
            }
            else
            {
                Logger.Status("  [FindWindowProc] Window with Title=" + windowTitle + " found!");
            }

            return AvaWin;
        }
        #endregion

        #region Public Methods
        public static bool ValidateWindowState(String WindowTitle, WindowState expectedWindowState)
        {
            Logger.Status("  [ValidateWindowState] Entered. ExpectedWindowState=" + expectedWindowState.ToString());
            Logger.Status("  [ValidateWindowState] Calling GetWindowPlacement API to query WINDOWPLACEMENT enum");
            WINDOWPLACEMENT windowPlacement = new WINDOWPLACEMENT();
            if (!GetWindowPlacement(FindWindowProc(null, WindowTitle), ref windowPlacement))
            {
                Logger.LogFail("Error gettng WindowPlacement");
            }
            Logger.Status("  [ValidateWindowState] WINDOWPLACEMENT Value successfully received");

            if (windowPlacement.showCmd == (uint)WINDOWPLACEMENT_showCmd.SW_SHOWNORMAL)
            {
                Logger.Status("  [ValidateWindowState] ActualWindowState=Normal");
                return (WindowState.Normal == expectedWindowState);
            }
            else if (windowPlacement.showCmd == (uint)WINDOWPLACEMENT_showCmd.SW_SHOWMINIMIZED)
            {
                Logger.Status("  [ValidateWindowState] ActualWindowState=Minimized");
                return (WindowState.Minimized == expectedWindowState);
            }
            else if (windowPlacement.showCmd == (uint)WINDOWPLACEMENT_showCmd.SW_SHOWMAXIMIZED)
            {
                Logger.Status("  [ValidateWindowState] ActualWindowState=Maximized");
                return (WindowState.Maximized == expectedWindowState);
            }
            else
            {
                Logger.Status("  [ValidateWindowState] ActualWindowState=UNRECOGNIZED!");
                return false;
            }
        }

        public static bool ValidateTitle(string expectedTitle)
        {
            Logger.Status("  [ValidateTitle] Entered");
            return (!IntPtr.Zero.Equals(FindWindowProc(null, expectedTitle)));
        }

        public static bool ValidateTopmost(string windowTitle, bool expectsTopmost)
        {   
            Logger.Status("  [ValidateTopmost] Entered");
            bool ContainTopmostStyle = ContainExStyle(windowTitle, ExWS.WS_EX_TOPMOST);
            Logger.Status("  [ValidateTopmost] ContainTopmostStyle=" + ContainTopmostStyle.ToString());
            return ((ContainTopmostStyle && expectsTopmost) || (!ContainTopmostStyle && !expectsTopmost));
        }

        public static bool ValidateSizeToContent(string windowTitle, double expectedContentWidth, double expectedContentHeight)
        {
            Logger.Status("  [ValidateSizeToContent] Entered. [ExpectedContentWidth=" + expectedContentWidth.ToString() + "] [ExpcetedContentHeight=" + expectedContentHeight.ToString() + "]");
            return (ValidateContentWidth(windowTitle, expectedContentWidth) && ValidateContentHeight(windowTitle, expectedContentHeight));
        }

        public static bool ValidateContentWidth(string windowTitle, double expectedContentWidthLogicalUnits)
        {
            Logger.Status("  [ValidateContentWidth] Entered. [ExpectedContentWidthLogicalUnits=" + expectedContentWidthLogicalUnits.ToString() + "]");
            RECT clientRect = new RECT();
            if (!GetClientRect(FindWindowProc(null, windowTitle), ref clientRect))
            {
                Logger.LogFail("GetClientRect failed!");
                return false;
            }

            double contentWidthDeviceUnits = clientRect.right - clientRect.left;
            double dpiRatio = TestUtil.DPIXRatio;
            double contentWidthLogicalUnits = contentWidthDeviceUnits / dpiRatio;
            Logger.Status("  [ValidateContentWidth] Actual Content Width before DPI Conversion=" + contentWidthDeviceUnits.ToString());
            Logger.Status("  [ValidateContentWidth] DPI Ratio=" + dpiRatio.ToString());
            Logger.Status("  [ValidateContentWidth] Actual Content Width after DPI Conversion=" + contentWidthLogicalUnits.ToString());

            return (TestUtil.IsEqual(contentWidthLogicalUnits, expectedContentWidthLogicalUnits));
        }
        
        public static bool ValidateContentHeight(string windowTitle, double expectedContentHeightLogicalUnits)
        {
            Logger.Status("  [ValidateContentHeight] Entered. [ExpectedContentHeightLogicalUnits=" + expectedContentHeightLogicalUnits.ToString() + "]");
            RECT clientRect = new RECT();
            if (!GetClientRect(FindWindowProc(null, windowTitle), ref clientRect))
            {
                Logger.LogFail("GetClientRect failed!");
                return false;
            }

            double contentHeightDeviceUnits = clientRect.bottom - clientRect.top;
            double dpiRatio = TestUtil.DPIYRatio;
            double contentHeightLogicalUnits = contentHeightDeviceUnits / dpiRatio;
            Logger.Status("  [ValidateContentWidth] Actual Content Height before DPI Conversion=" + contentHeightDeviceUnits.ToString());
            Logger.Status("  [ValidateContentWidth] DPI Ratio=" + dpiRatio.ToString());
            Logger.Status("  [ValidateContentWidth] Actual Content Height after DPI Conversion=" + contentHeightLogicalUnits.ToString());

            return (TestUtil.IsEqual(contentHeightLogicalUnits, expectedContentHeightLogicalUnits));
        }

        public static bool ValidateFlowDirection(string windowTitle, FlowDirection fd)
        {
            Logger.Status("  [ValidateFlowDirection] Entered. [ExpctedFlowDirection=" + fd.ToString() + "]");
            bool ContainRTLStyle = ContainExStyle(windowTitle, ExWS.WS_EX_LAYOUTRTL);
            return ((fd == FlowDirection.RightToLeft && ContainRTLStyle) || (fd == FlowDirection.LeftToRight && !ContainRTLStyle));
        }

        public static bool ValidateLeft(string windowTitle, double expectedLeftLogicalUnits)
        {

            Logger.Status("  [ValidateLeft] Entered [ExpectedLeftLogicalUnits=" + expectedLeftLogicalUnits.ToString() + "]");
            RECT windowRect = new RECT();
            if (!GetWindowRect(FindWindowProc(null, windowTitle), ref windowRect))
            {
                Logger.LogFail("Fail to get Window Rect");
                return false;
            }

            double dpiRatio = TestUtil.DPIXRatio;
            Logger.Status("  [ValidateLeft] ActualLeft before DPI Conversion=" + windowRect.left.ToString());
            Logger.Status("  [ValidateLeft] DPI Ratio=" + dpiRatio.ToString());
            Logger.Status("  [ValidateLeft] ActualLeft after DPI Conversion=" + (windowRect.left / dpiRatio).ToString());

            return (TestUtil.IsEqual((windowRect.left /dpiRatio), expectedLeftLogicalUnits));
        }

        public static bool ValidateTop(string windowTitle, double expectedTopLogicalUnits)
        {
            Logger.Status("  [ValidateTop] Entered. [ExpectedTopLogicalUnits=" + expectedTopLogicalUnits.ToString() + "]");
            
            RECT windowRect = new RECT();
            if (!GetWindowRect(FindWindowProc(null, windowTitle), ref windowRect))
            {
                Logger.LogFail("Fail to get Window Rect");
                return false;
            }
            double dpiRatio = TestUtil.DPIYRatio;
            Logger.Status("  [ValidateTop] ActualTop before DPI Conversion=" + windowRect.top.ToString());
            Logger.Status("  [ValidateTop] DPI Ratio=" + dpiRatio.ToString());
            Logger.Status("  [ValidateTop] ActualTop after DPI Conversion=" + (windowRect.top/dpiRatio).ToString());
            return (TestUtil.IsEqual((windowRect.top /dpiRatio), expectedTopLogicalUnits));
        }

        public static bool ValidateWidth(string windowTitle, double expectedWidth)
        {
            Logger.Status("  [ValidateWidth] Entered. [ExpectedWidth=" + expectedWidth.ToString() + "]");
            RECT windowRect = new RECT();
            if (!GetWindowRect(FindWindowProc(null, windowTitle), ref windowRect))
            {
                Logger.LogFail("Fail to get Window Rect");
                return false;
            }

            double dpiRatio = TestUtil.DPIXRatio;
            Logger.Status("  [ValidateWidth] ActualWidth Before DPI Conversion=" + (windowRect.right - windowRect.left).ToString());
            Logger.Status("  [ValidateWidth] DPI Ratio = " + dpiRatio.ToString());
            Logger.Status("  [ValidateWidth] ActualWidth After DPI Conversion=" + (windowRect.right / dpiRatio - windowRect.left / dpiRatio).ToString());
            return (TestUtil.IsEqual((windowRect.right - windowRect.left) / dpiRatio, expectedWidth));
        }

        public static bool ValidateHeight(string windowTitle, double expectedHeight)
        {
            Logger.Status("  [ValidateHeight] Entered. [ExpectedHeight=" + expectedHeight.ToString() + "]");
            RECT windowRect = new RECT();
            if (!GetWindowRect(FindWindowProc(null, windowTitle), ref windowRect))
            {
                Logger.LogFail("Fail to get Window Rect");
                return false;
            }
            double dpiRatio = TestUtil.DPIXRatio;
            Logger.Status("  [ValidateHeight] ActualHeight Before DPI Conversion=" + (windowRect.bottom - windowRect.top).ToString());
            Logger.Status("  [ValidateHeight] DPI Ratio = " + dpiRatio.ToString());
            Logger.Status("  [ValidateHeight] ActualHeight After DPI Conversion=" + (windowRect.bottom / dpiRatio - windowRect.top / dpiRatio).ToString());
            return (TestUtil.IsEqual((windowRect.bottom - windowRect.top)/dpiRatio, expectedHeight));
        }

        public static bool ValidateRestoreBounds(string windowTitle, Rect expectedBounds)
        {

            Logger.Status("  [ValidateRestoreBounds] Entered. [Expected Rect=" + expectedBounds.ToString());
            return (ValidateRestoreBounds(windowTitle, expectedBounds.Left, expectedBounds.Top, expectedBounds.Right - expectedBounds.Left, expectedBounds.Bottom - expectedBounds.Top));
        }

        public static bool ValidateRestoreBounds(string windowTitle, double left, double top, double width, double height)
        {
            Logger.Status("  [ValidateRestoreBounds] Entered. [Expected Left=" + left.ToString() + " Top=" + top.ToString() + " Width=" + width.ToString() + " Height" + height.ToString() +"]");
            return (ValidateTop(windowTitle, top) &&
                    ValidateLeft(windowTitle, left) &&
                    ValidateWidth(windowTitle, width) &&
                    ValidateHeight(windowTitle, height));
        }

        public static bool ValidateResizeMode(string windowTitle, ResizeMode mode)
        {
            Logger.Status("  [ValidateResizeMode] Entered. [Expected ResizeMode=" + mode.ToString() +"]");
            bool HasMinimizeBoxStyle = ContainStyle(windowTitle, WS.WS_MINIMIZEBOX);
            bool HasMaximizeBoxStyle = ContainStyle(windowTitle, WS.WS_MAXIMIZEBOX);
            bool HasThickFrameStyle = ContainStyle(windowTitle, WS.WS_THICKFRAME);

            Logger.Status("  [ValidateResizeMode] ContainMinimizeBoxStyle=" + HasMinimizeBoxStyle.ToString());
            Logger.Status("  [ValidateResizeMode] ContainMaximizeBoxStyle=" + HasMaximizeBoxStyle.ToString());
            Logger.Status("  [ValidateResizeMode] ContainThickFrameStyle=" + HasThickFrameStyle.ToString());
            if (mode == ResizeMode.CanMinimize)
            {
                return (!HasThickFrameStyle && HasMinimizeBoxStyle && !HasMaximizeBoxStyle);
            }
            else if (mode == ResizeMode.CanResize || mode == ResizeMode.CanResizeWithGrip)
            {
                return (HasMaximizeBoxStyle && HasMinimizeBoxStyle && HasThickFrameStyle);
            }
            else // no resize
            {
                return (!HasThickFrameStyle && !HasMaximizeBoxStyle && !HasMinimizeBoxStyle);
            }

        }

        public static bool ValidateWindowStyle(string windowTitle, WindowStyle ws)
        {
            Logger.Status("  [ValidateWindowStyle] Entered. Expected WindowStyle=" + ws.ToString() + "]");
            bool HasCaptionStyle = ContainStyle(windowTitle, WS.WS_CAPTION);
            bool HasClientEdge = ContainExStyle(windowTitle, ExWS.WS_EX_CLIENTEDGE);
            bool HasToolWindowStyle = ContainExStyle(windowTitle, ExWS.WS_EX_TOOLWINDOW);

            Logger.Status("  [ValidateWindowStyle] ContainCaptionStyle=" + HasCaptionStyle.ToString());
            Logger.Status("  [ValidateWindowStyle] ContainClientEdge=" + HasClientEdge.ToString());
            Logger.Status("  [ValidateWindowStyle] ContainToolWindowStyle=" + HasToolWindowStyle.ToString());

            if (ws == WindowStyle.ToolWindow)
            {
                return (HasToolWindowStyle && HasCaptionStyle && !HasClientEdge);
            }
            else if (ws == WindowStyle.SingleBorderWindow)
            {
                return (HasCaptionStyle && !HasClientEdge && !HasToolWindowStyle);
            }
            else if (ws == WindowStyle.ThreeDBorderWindow)
            {
                return (HasCaptionStyle && HasClientEdge && !HasToolWindowStyle);
            }
            else // No Style
            {
                return (!HasCaptionStyle && !HasClientEdge && !HasToolWindowStyle);
            }
        }


        public static bool ValidateWindowStartupLocation(string windowTitle, WindowStartupLocation wsl)
        {


            Logger.Status("  [ValidateWindowStartupLocation] Entered. [Expected WindowStartupLocation=" + wsl.ToString() +"]");
            RECT desktopRes = new RECT();
            RECT windowRect = new RECT();
            RECT taskbarRect = new RECT();

            if (!GetWindowRect(FindWindowProc("Shell_TrayWnd", null), ref taskbarRect))
            {
                Logger.LogFail("Get Taskbar Rect Failed");
                return false;
            }

            if (!GetClientRect(GetDesktopWindow(), ref desktopRes))
            {
                Logger.LogFail("Get Client Area Failed");
                return false;
            }

            if (!GetWindowRect(FindWindowProc(null, windowTitle), ref windowRect))
            {
                Logger.LogFail("Get Window Rect Failed");
                return false;
            }

            Logger.Status("  [ValidateWindowStyle] Desktop Resolution=" + desktopRes.ToString());
            Logger.Status("  [ValidateWindowStyle] TaskbarRect=" + taskbarRect.ToString());
            Logger.Status("  [ValidateWindowStyle] windowRect=" + windowRect.ToString());

            if (wsl == WindowStartupLocation.CenterScreen)
            {
                return (System.Math.Abs((desktopRes.right - desktopRes.left) / 2 - (windowRect.right - windowRect.left) / 2 - windowRect.left) <= 1 &&
                       System.Math.Abs((taskbarRect.top - desktopRes.top) / 2 - (windowRect.bottom - windowRect.top) / 2 - windowRect.top) <= 1);
            }

            return false;
        }

        public static bool ValidateShowInTaskbar(string windowTitle, bool showInTaskbar)
        {
            Logger.Status("  [ValidateShowInTaskbar] Entered. [Expected ShowInTaskbar=" + showInTaskbar.ToString() + "]");
            bool HasAppWindowStyle = ContainExStyle(windowTitle, ExWS.WS_EX_APPWINDOW);
            Logger.Status("  [ValidateShowInTaskbar] ContainAppWindowStyle=" + HasAppWindowStyle.ToString());
            return ((HasAppWindowStyle && showInTaskbar) || (!HasAppWindowStyle && !showInTaskbar));
        }

        public static bool ValidateVisibility(string windowTitle, Visibility visibility)
        {
            // Today, Window makes no difference between Hidden and Collapsed.
            Logger.Status("  [ValidateVisibility] Entered");
            Logger.Status("  [ValidateVisibility] Expected Visibility=" + visibility.ToString());
            bool HasVisibleStyle = ContainStyle(windowTitle, WS.WS_VISIBLE);
            bool IsVisible = (visibility == Visibility.Visible ? true : false);
            Logger.Status("  [ValidateVisibility] ContainVisibleStyle=" + HasVisibleStyle.ToString());
            Logger.Status("  [ValidateVisibility] IsVisible=" + IsVisible.ToString());
            return ((HasVisibleStyle && IsVisible) || (!HasVisibleStyle && !IsVisible));
        }

        public static bool ValidateChildWindow(string parentWindowTitle, string childWindowTitle)
        {
            /*
            IntPtr childWinHandle = FindWindowProc(childWindowTitle);
            IntPtr parentWinHandle = FindWindowProc(parentWindowTitle);
            IntPtr childWinHandleFromParent = GetWindow(parentWinHandle, (uint) GetWindow_uCmd.GW_CHILD);
            return childWinHandle.Equals(childWinHandleFromParent);
             */

            return (ContainStyle(childWindowTitle, WS.WS_CHILD));
            // make sure the child window does not exist in the taskbar
            // Call GetWindow with GW_CHILD as uCmd param
            // make sure the parent window exists

        }

        public static bool ValidateOwnedWindows(string windowTitle, WindowCollection wc)
        {
            return false;
        }


        public static double GetBorderLength(string windowTitle)
        {
            Logger.Status("  [GetBorderLength] Entered");
            IntPtr windowPtr = FindWindow(null, windowTitle);

            RECT clientRect = new RECT();
            RECT windowRect = new RECT();
            
            Logger.Status("  [GetBorderLength] Calling GetClientRect & GetWindowRect");
            if (!GetClientRect(windowPtr, ref clientRect) || !GetWindowRect(windowPtr, ref windowRect))
            {
                Logger.LogFail("Calling GetClientRect OR GetWindowRect Failed!");
                return -1;
            }

            return ((windowRect.right - windowRect.left - clientRect.right - clientRect.left) / 2 / TestUtil.DPIXRatio);
        }

        public static double GetCaptionHeight(string windowTitle)
        {
            Logger.Status("  [GetCaptionHeight] Entered");
            IntPtr windowPtr = FindWindow(null, windowTitle);

            RECT clientRect = new RECT();
            RECT windowRect = new RECT();
            
            Logger.Status("  [GetCaptionHeight] Calling GetClientRect & GetWindowRect");
            if (!GetClientRect(windowPtr, ref clientRect) || !GetWindowRect(windowPtr, ref windowRect))
            {
                Logger.LogFail("Calling GetClientRect OR GetWindowRect Failed!");
                return -1;
            }

            return ((windowRect.bottom - windowRect.top - clientRect.bottom - clientRect.top - (windowRect.right - windowRect.left - clientRect.right - clientRect.left) / 2) / TestUtil.DPIYRatio);
        }

        public static void SimulateLogoff()
        {
            //System.Diagnostics.Process.Start(
            SendMessage(FindWindow(null, "WindowTest"), (int)WindowMessages.WM_QUERYENDSESSION, 0, (int)SessionEndingReasons.ENDINGSESSION_LOGOFF);
        }

        public static void SimulateShutdown()
        {
            SendMessage(FindWindow(null, "WindowTest"), (int)WindowMessages.WM_QUERYENDSESSION, 0, (int)SessionEndingReasons.ENDINGSESSION_SHUTDOWN);
        }

        #endregion

    }
}

