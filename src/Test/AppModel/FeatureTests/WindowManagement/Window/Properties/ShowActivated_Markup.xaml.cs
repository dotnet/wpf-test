// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;

namespace WindowTest
{
    public partial class ShowActivated_Markup : Window
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            // Test 6: verify window shown with ShowActivated="false" set in markup isn't the active window
            WindowInteropHelper windowInterop = new WindowInteropHelper(this);
            IntPtr hwnd = windowInterop.Handle;

            IntPtr hwndActive = TestUtil.GetActiveWindow();
  
            if (hwnd != hwndActive)
            {
                Logger.Status("[VALIDATION PASSED] Window was shown but is not active.");
            }
            else
            {
                Logger.LogFail("Window was activated when shown!");
            }

            TestHelper.Current.TestCleanup();
        }
    }
}
