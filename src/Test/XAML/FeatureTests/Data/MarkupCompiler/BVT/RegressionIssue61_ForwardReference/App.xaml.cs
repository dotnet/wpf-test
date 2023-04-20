// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Markup;

namespace RegressionIssue61_ForwardReference
{
    /// <summary>
    /// Verify that forward reference to resources doesn't work
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                Window window = new MainWindow();
                Console.WriteLine("Forward references worked");
                Application.Current.Shutdown(1);
            }
            catch (XamlParseException)
            {
                Console.WriteLine("Forward references didn't work");
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
