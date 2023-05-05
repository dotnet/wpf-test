// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace RegressionIssue62_UnknownBamlRecord
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
                Window window = new MainWindow();
                window.ShowDialog();
            }
            catch (Exception exception)
            {
                Console.WriteLine("Caught following exception.\n" + exception.ToString());
                Application.Current.Shutdown(1);
            }
            Console.WriteLine("TypeSerializerInfo record was correctly read by Baml reader");
            Application.Current.Shutdown(0);
        }
    }
}
