//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------
using System;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Runtime.InteropServices;

using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.Actions.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct SHELLEXECUTEINFO
    {
        public UInt32 cbSize;
        public UInt32 fMask;
        public IntPtr hwnd;
        public String lpVerb;
        public String lpFile;
        public String lpParameters;
        public String lpDirectory;
        public int nShow;
        public IntPtr hInstApp;
        public IntPtr lpIDList;
        public String lpClass;
        public IntPtr hkeyClass;
        public UInt32 dwHotKey;
        public IntPtr hUnionHandler;
        public IntPtr hProcess;
    }

    /// <summary>
    /// call native API to change current Theme
    /// </summary>
    internal static class ChangeThemesHelper
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern Int32 GetWindowText(IntPtr hwnd, StringBuilder strText, int count);

        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        public static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO shell);

        private static bool FindDisplayDialog()
        {
            IntPtr hWnd = GetForegroundWindow();
            WindowInteropHelper current = new WindowInteropHelper(Application.Current.MainWindow);

            if (hWnd == IntPtr.Zero || hWnd == current.Handle)
            {
                GlobalLog.LogEvidence("GetForegroundWindow Fail");
                return false;
            }

            //get Window Text 
            StringBuilder text = new StringBuilder(256);
            GetWindowText(hWnd, text, 256);

            return text.ToString() == "Display Properties";
        }

        private const int SW_SHOWMAXIMIZED = 3;

        public static void ChangeTheme(WindowTheme theme)
        {
            SHELLEXECUTEINFO shellExecInfo = new SHELLEXECUTEINFO();

            shellExecInfo.cbSize = (UInt32)Marshal.SizeOf(shellExecInfo);
            shellExecInfo.lpVerb = null;
            shellExecInfo.lpParameters = null;
            shellExecInfo.lpDirectory = null;
            shellExecInfo.nShow = SW_SHOWMAXIMIZED;
            shellExecInfo.hInstApp = IntPtr.Zero;

            switch (theme)
            {
                case WindowTheme.Classic:
                    shellExecInfo.lpFile = System.Environment.GetEnvironmentVariable("windir") + @"\Resources\Themes\Windows Classic.theme";
                    break;

                case WindowTheme.Luna:
                    shellExecInfo.lpFile = System.Environment.GetEnvironmentVariable("windir") + @"\Resources\Themes\Luna.theme";
                    break;

                default:
                    throw new ArgumentException("Only Classic and Luna is supported");
            }

            //run Theme file to invoke Display Properties Dialog
            ShellExecuteEx(ref shellExecInfo);

            //wait 3 sec to Display Properties Dialog shown
            QueueHelper.WaitTillTimeout(new TimeSpan(0, 0, 3));

            //try to find Display Properties Dialog
            bool bFlag = FindDisplayDialog();

            if (bFlag)
            {//if get the Display Properties Dialog, press Enter key to confirm switch
                GlobalLog.LogStatus("Press Enter Key to confirm switch theme operation");
                UserInput.KeyPress("Enter");
                QueueHelper.WaitTillQueueItemsProcessed();
            }
            else
            {
                throw new InvalidOperationException("Can not find Display Properties Dialog");
            }
        }
    }
}
