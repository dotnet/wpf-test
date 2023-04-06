// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Test.Threading;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    /// <summary>
    /// New for Dev10: WindowInteropHelper.EnsureHandle
    /// </summary>
    public partial class EnsureHandleP1
    {
        void OnContentRendered(object sender, EventArgs e)
        {
#if TESTBUILD_CLR40
            bool caught = false;
            WindowInteropHelper interopHelper = null;
            IntPtr handle = IntPtr.Zero;
            Window newWindow = new Window();
            newWindow.Owner = Application.Current.MainWindow;
            
            // Case 1: Call WindowInteropHelper.EnsureHandle, then verify we can close the window without issue.
            // We expect a nonzero handle back.
            Logger.Status("Case 1: Call WindowInteropHelper.EnsureHandle, then verify we can close the window without issue.");
            try
            {
                interopHelper = new WindowInteropHelper(newWindow);
                handle = interopHelper.EnsureHandle();
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
                caught = true;
            }

            if (!caught)
            {
                if ((interopHelper.Handle == handle) && (handle != IntPtr.Zero))
                {
                    Logger.LogPass("WindowInteropHelper.EnsureHandle returned the proper handle");
                }
                else
                {
                    Logger.LogFail("WindowInteropHelper.EnsureHandle failed to return the proper handle.  Got: " + handle.ToString());
                }
            }

            caught = false;
            Logger.Status("Closing the non-shown but handle-ensured window");
            try
            {
                newWindow.Close();
                DispatcherHelper.DoEvents();
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
                caught = true;
            }

            if (!caught)
            {
                handle = interopHelper.Handle;
                Logger.LogPass("Successfully closed window after call to WindowInteropHelper.EnsureHandle: " + handle.ToString());
            }

            // Case 2: Create and Show a window, then verify we can call EnsureHandle without issue.
            caught = false;
            Logger.Status("Case 2 - creating and showing a new window");
            try
            {
                newWindow = new Window();
                newWindow.Show();
                DispatcherHelper.DoEvents();
                interopHelper = new WindowInteropHelper(newWindow);
                handle = interopHelper.EnsureHandle();
            }
            catch (Exception ex)
            {
                Logger.LogFail("Unexpected exception caught: " + ex.ToString());
                caught = true;
            }

            if (!caught)
            {
                if ((interopHelper.Handle == handle) && (handle != IntPtr.Zero))
                {
                    Logger.LogPass("WindowInteropHelper.EnsureHandle after Show returned the proper handle");
                }
                else
                {
                    Logger.LogFail("WindowInteropHelper.EnsureHandle after show failed to return the proper handle.  Got: " + handle.ToString());
                }
            }
            newWindow.Close();

            // Case 3: Verify we can use CenterOwner on a child window to center over a non-shown parent that called EnsureHandle.
            Window parentWindow = new Window();
            interopHelper = new WindowInteropHelper(parentWindow);
            handle = interopHelper.EnsureHandle();

            Logger.Status("Case 3 - Verify we can use CenterOwner on a child window to center over a non-shown parent that called EnsureHandle");

            Window childWindow = new Window();
            childWindow.Width = 150;
            childWindow.Height = 100;
            childWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            childWindow.Owner = parentWindow;

            Logger.Status("Showing child window");
            childWindow.Show();
            DispatcherHelper.DoEvents();

            RECT parentRect;
            // Managed width/height don't exist yet - get native window coordinates
            bool retval = GetWindowRect(interopHelper.Handle, out parentRect);

            // note the native coords need to be DPI converted
            double leftDiff = Math.Abs((parentRect.Left / TestUtil.DPIXRatio) - childWindow.Left);
            double rightDiff = Math.Abs((parentRect.Right / TestUtil.DPIXRatio) - (childWindow.Left + childWindow.Width));
            double topDiff = Math.Abs((parentRect.Top / TestUtil.DPIYRatio) - childWindow.Top);
            double bottomDiff = Math.Abs((parentRect.Bottom / TestUtil.DPIYRatio) - (childWindow.Top + childWindow.Height));

            Logger.Status("Parent: (" + (parentRect.Left / TestUtil.DPIXRatio) + "," + (parentRect.Top / TestUtil.DPIYRatio) + ")-(" + (parentRect.Right / TestUtil.DPIXRatio) + "," + (parentRect.Bottom / TestUtil.DPIYRatio) + ")");
            Logger.Status("Child: (" + childWindow.Left + "," + childWindow.Top + ")-(" + (childWindow.Left + childWindow.Width) + "," + (childWindow.Top + childWindow.Height) + ")");

            if (TestUtil.IsEqual(rightDiff, leftDiff)) 
            {
                Logger.LogPass("Child window was centered horizontally over non-shown parent.");
            }
            else
            {
                Logger.LogFail("Child window was not centered horizontally over non-shown parent.  Left diff: " + leftDiff.ToString() + " Right diff: " + rightDiff.ToString());
            }

            if (TestUtil.IsEqual(bottomDiff, topDiff))
            {
                Logger.LogPass("Child window was centered vertically over non-shown parent.");
            }
            else
            {
                Logger.LogFail("Child window was not centered vertically over non-shown parent.  Top diff: " + topDiff.ToString() + " Bottom diff: " + bottomDiff.ToString());
            }

            childWindow.Close();
            parentWindow.Close();

#endif

            Logger.LogPass("test complete");

            this.Close();
        }

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public int Height { get { return Bottom - Top; } }
            public int Width { get { return Right - Left; } }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    }
}
