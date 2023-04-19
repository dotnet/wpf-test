// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Wpf.AppModel.CommonDialogs
{
    internal static class Common
    {
        static Microsoft.Windows.Test.Client.AppSec.FrameworkLoggerWrapper s_log = new Microsoft.Windows.Test.Client.AppSec.FrameworkLoggerWrapper();
               
        internal static void ExitWithError(String failMsg)
        {
            s_log.LogEvidence("TEST FAILED: " + failMsg);
            s_log.Result = TestResult.Fail;
            ShutdownTest();
        }

        internal static void ExitWithPass()
        {
            s_log.LogEvidence("TEST PASSED");
            s_log.Result = TestResult.Pass;
            ShutdownTest();
        }

        internal static void ExitWithIgnore()
        {
            s_log.LogEvidence("Test result set to ignore.");
            s_log.Result = TestResult.Ignore;
            ShutdownTest();
        }
        
        internal static void ShutdownTest()
        {
            ApplicationMonitor.NotifyStopMonitoring();
        }        
    }
}
