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

namespace GetnSetPersistentCookie
{
    public partial class Page1
    {

        void OnLoaded(object source, EventArgs e)
        {

            TestLog.Current.LogStatus("Trying to get previously set cookie");

            try
            {
                // Get cookie
                if (Application.GetCookie(BrowserInteropHelper.Source).Contains("_PersistentCookieFromHTML=value1"))
                {
                    if (TestLog.Current != null)
                    {
                        TestLog.Current.LogStatus("Trying to set a cookie");
                    }

                    try
                    {
                        // Set cookie
                        Application.SetCookie(BrowserInteropHelper.Source, "NewPersistentCookieFromXBAP=1978; expires=Mon, 21-Nov-2078 00:09:25 GMT");
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
                else
                {
                    if (TestLog.Current != null)
                    {
                        TestLog.Current.LogEvidence("The content of the cookie received does not match the expected value of _PersistentCookieFromHTML=value1 ");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    ApplicationMonitor.NotifyStopMonitoring();

                }
            }
            catch (Win32Exception ex)
            {
                if (TestLog.Current != null)
                {
                    TestLog.Current.LogEvidence("Error getting a cookie" + ex.Message + " (Native Error Code=" + ex.NativeErrorCode + ")");
                    TestLog.Current.Result = TestResult.Fail;
                }
                ApplicationMonitor.NotifyStopMonitoring();
            }
        }
    }
}
