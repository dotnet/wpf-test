// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace RegressionIssue65_DefaultStyleInMD_OneLevel
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                MainWindow window = new MainWindow();
                window.ShowDialog();
            }
            catch (Exception exception)
            {
                Console.WriteLine("MainWindow constructor threw exception.\n" + exception.ToString());
            }
        }
    }
}
