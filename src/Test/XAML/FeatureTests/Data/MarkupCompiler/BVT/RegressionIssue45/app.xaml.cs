// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.Logging;

namespace RegressionIssue45
{
    /// <summary>
    /// not able to open resources compiled into a subdirectory
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                Window1 window1 = new Window1();
                window1.Show();
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Exception from Window1..ctor: " + exception.ToString());
                Application.Current.Shutdown(-1);
            }
        }
    }
}
