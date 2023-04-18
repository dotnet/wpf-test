// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    /// <summary>
    /// WindowInteropHelper.EnsureHandle
    /// </summary>
    public partial class EnsureHandleInterop
    {
        void OnContentRendered(object sender, EventArgs e)
        {
#if TESTBUILD_CLR40
            WindowInteropHelper interopHelper = null;
            IntPtr handle = IntPtr.Zero;
            Window newWindow = new Window();
            newWindow.Owner = Application.Current.MainWindow;
            
            // Call WindowInteropHelper.EnsureHandle, set windows Top,Left,Width,Height via PInvoke, then show.  Verify sizing.
            Logger.Status("Calling WindowInteropHelper.EnsureHandle");

            interopHelper = new WindowInteropHelper(newWindow);
            handle = interopHelper.EnsureHandle();

            Logger.Status("Setting window dimensions via PInvoke");
            NativeMethods.SetWindowPos(handle, IntPtr.Zero, 120, 120, 200, 200, 0);

            double expectedLeft = 120 / TestUtil.DPIXRatio;
            double expectedTop = 120 / TestUtil.DPIYRatio;
            double expectedWidth = 200 / TestUtil.DPIXRatio;
            double expectedHeight = 200 / TestUtil.DPIYRatio;

            Logger.Status("Showing window");
            newWindow.Show();
            DispatcherHelper.DoEvents();

            if (TestUtil.IsEqual(newWindow.Left, expectedLeft))
            {
                Logger.LogPass("Left window coordinate matched");
            }
            else
            {
                Logger.LogFail("Left window coordinate did not match.  Expected: " + expectedLeft + " Got: " + newWindow.Left);
            }

            if (TestUtil.IsEqual(newWindow.Top, expectedTop))
            {
                Logger.LogPass("Top window coordinate matched");
            }
            else
            {
                Logger.LogFail("Top window coordinate did not match.  Expected: " + expectedTop + " Got: " + newWindow.Top);
            }

            if (TestUtil.IsEqual(newWindow.ActualWidth, expectedWidth))
            {
                Logger.LogPass("Width matched");
            }
            else
            {
                Logger.LogFail("Width did not match.  Expected: " + expectedWidth + " Got: " + newWindow.ActualWidth);
            }

            if (TestUtil.IsEqual(newWindow.ActualHeight, expectedHeight))
            {
                Logger.LogPass("Height matched");
            }
            else
            {
                Logger.LogFail("Height did not match.  Expected: " + expectedHeight + " Got: " + newWindow.ActualHeight);
            }
#endif

            Logger.LogPass("test complete");

            this.Close();
        }
    }
}
