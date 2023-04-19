// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;

namespace WebOCXBAP
{
    public partial class Page1
    {
        void OnLoaded(object source, EventArgs e)
        {
            //Setting a timer to allow enough time for WebOC content to load
            System.Timers.Timer aTimer = new System.Timers.Timer();
            aTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            // Set the Interval to 8 seconds.
            aTimer.Interval = 8000;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new System.Windows.Threading.DispatcherOperationCallback(delegate
            {
                TestLog.Current.LogStatus("Trying to get previously set cookie");
                try
                {
                    // Get cookie
                    this.CookiePage.WindowTitle = Application.GetCookie(BrowserInteropHelper.Source);
                    this.CookiePage.CookieLabel.Content = this.CookiePage.WindowTitle;
                }
                catch (Win32Exception ex)
                {
                    TestLog.Current.LogStatus("Error getting a cookie" + ex.Message + " (Native Error Code=" + ex.NativeErrorCode + ")");
                    TestLog.Current.Result = TestResult.Fail;
                    ApplicationMonitor.NotifyStopMonitoring();
                }

                return null;
            }), null);
        }
    }
}
