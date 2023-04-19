// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;
using Microsoft.Test.Win32;

namespace WindowTest
{
    // Regression test
    // Add an HwndSourceHook to ignore WM_NCCALCSIZE, then move the window left a bit.  If it jumps upwards, it's a bug.
    public partial class AdjustWindowRectBug
    {
        void OnSourceInitialized(object sender, EventArgs e)
        {
            HwndSource source = (HwndSource)HwndSource.FromVisual(this);
            source.AddHook(MyHwndSourceHook);            
        }

        IntPtr MyHwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case NativeConstants.WM_NCCALCSIZE:
                    handled = true;
                    break;
                default:
                    handled = false;
                    break;
            }
            return IntPtr.Zero;
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            double preTop = Top;

            Logger.Status("Moving the window back and forth.  It should end up exactly where it was.");
            Left += 5;
            Left -= 5;

            if((preTop - Top) == 0)
            {
                Logger.LogPass("As expected, window did not move upwards when Left was changed.");
            }
            else
            {
                Logger.LogFail("Window unexpectedly moved upwards when Left was changed. Before: " + preTop + " After: " + Top);
            }

            Close();
        }
    }
}
