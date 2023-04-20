// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Logging;

namespace MarkupExtensionInResources
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
                Window1 window1 = new Window1();
                window1.Show();
                SolidColorBrush[] brushes = (SolidColorBrush[])window1.Resources["brushes"];
                if (brushes.Length != 3)
                {
                    throw new Exception("Expected 3 brushes.  Actual: " + brushes.Length);
                }

            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Exception from Window1..ctor: " + exception.ToString());
                Application.Current.Shutdown(-1);
            }
            Application.Current.Shutdown(0);
        }
    }
}
