// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System;

namespace RegressionIssue63_SubclassingControlTemplate
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
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception from Window1..ctor: " + exception.ToString());
                Application.Current.Shutdown(-1);
            }

            Application.Current.Shutdown(0);
        }
    }
}
