// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace RegressionIssue67_xArrayInsideRD
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
                Window1 window1 = new Window1();
                window1.ShowDialog();
                Console.WriteLine("Window1 is created and shown successfully.");
                Application.Current.Shutdown(0);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Window1 constructor threw the following exception.\n" + exception.ToString());
                Application.Current.Shutdown(1);
            }
        }
    }
}
