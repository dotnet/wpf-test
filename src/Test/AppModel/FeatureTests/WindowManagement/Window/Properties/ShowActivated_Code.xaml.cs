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
    public partial class ShowActivated_Code : Window
    {
        void OnLoaded(object sender, EventArgs e)
        {
            // Test 1: verify default of ShowActivated is True
            bool expectedValue = true;
            if (ShowActivated == expectedValue)
            {
                Logger.Status("[VALIDATION PASSED] Default Window.ShowActivated == " + expectedValue.ToString());
            }
            else
            {
                Logger.LogFail("ShowActivated default != " + expectedValue.ToString());
            }
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            // Test 2: verify window shown with ShowActivated = false isn't the active window
            // prepare for Test 2
            Window newWin = new Window();
            newWin.Width = 300;
            newWin.Height = 300;
            newWin.ShowActivated = false;
            newWin.Show();
            Thread.Sleep(2000);

            WindowInteropHelper windowInterop = new WindowInteropHelper(newWin);
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
            newWin.Close();

            TestHelper.Current.TestCleanup();
        }
    }
}
