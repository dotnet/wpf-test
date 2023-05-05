// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Markup;

namespace RegressionIssue61_CrossRefThemeScope
{
    /// <summary>
    /// Verify that resources defined in different xaml files which are merged into theme Resources 
    /// are NOT able to refer to resources outside the file they are defined in.
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Window window = new MainWindow();
                Console.WriteLine("Referencing across merged dictionaries worked in theme scope");
                Application.Current.Shutdown(1);
            }
            catch (XamlParseException)
            {
                Console.WriteLine("Referencing across merged dictionaries didn't work in theme scope");
                Application.Current.Shutdown(0);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Expected XamlParseException. But got unexpected exception - " + exception.ToString());
                Application.Current.Shutdown(1);
            }
        }
    }
}
