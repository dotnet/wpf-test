// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace RegressionIssue48
{
    public partial class App : Application
    {
        void AppStartingUp(object sender, EventArgs e)
        {
            Window1 mainWindow = new Window1();
            mainWindow.Show();
        }
    }
}
