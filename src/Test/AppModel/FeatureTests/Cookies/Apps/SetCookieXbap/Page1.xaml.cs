// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;

namespace SetCookieXbap
{
    public partial class Page1
    {

        void OnLoaded(object source, EventArgs e)
        {

            if (TestLog.Current != null)
            {
                TestLog.Current.LogStatus("Trying to set a cookie");
            }

            try
            {
                // Set cookie
                Application.SetCookie(BrowserInteropHelper.Source, "PersistentCookieFromXBAP=1978; expires=Mon, 21-Nov-2078 00:09:25 GMT");
                this.CookiePage.WindowTitle = "PersistentCookieFromXBAP=1978";
            }
            catch (Win32Exception ex)
            {

                if (TestLog.Current != null)
                {
                    TestLog.Current.LogEvidence("Error creating a cookie" + ex.Message + " (Native Error Code=" + ex.NativeErrorCode + ")");
                    TestLog.Current.Result = TestResult.Fail;
                }

                ApplicationMonitor.NotifyStopMonitoring();
            }
        }


    }
}
