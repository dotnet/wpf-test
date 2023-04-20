// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace SetEventsWithTemplatesAndStyles
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                var window = new Window1();
                window.Show();
                if (Window1.EventCount != 7)
                {
                    throw new Exception("Expected 7 events.  Actual: " + Window1.EventCount);
                }
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Exception from Window1..ctor: " + exception.ToString());
                Application.Current.Shutdown(-1);
            }
            Application.Current.Shutdown(0);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).Content = "Button clicked!";
            Window1.EventCount++;
        }

    }
}
