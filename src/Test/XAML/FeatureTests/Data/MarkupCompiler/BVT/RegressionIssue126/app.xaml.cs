// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Navigation;

namespace RegressionIssue126
{
    /// <summary>
    /// not able to open resources compiled into a subdirectory
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Window1 window = new Window1();
            window.Show();
            base.OnStartup(e);
        }
    }
}
