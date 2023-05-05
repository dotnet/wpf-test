// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace LineInfoServiceProviderTest
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
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            }
            catch (Exception exception)
            {
                Console.WriteLine("MainWindow threw exception: " + exception.ToString());
                Application.Current.Shutdown(1);
            }
        }
    }
}
