// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace RegressionIssue61_CrossRefApplicationScope
{
    /// <summary>
    /// Verify that resources defined in different xaml files which are merged into Application.Resources 
    /// are able to refer to resources outside the file they are defined in.
    /// This behavior is likely a bug in .Net 3.5, but cannot be fixed without breaking app compat.
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Window window = new MainWindow();
                window.ShowDialog();
                Console.WriteLine("Referencing across merged dictionaries worked in Application scope");
                Application.Current.Shutdown(0);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Unexpected exception - " + exception.ToString());
                Application.Current.Shutdown(1);
            }
        }
    }
}
