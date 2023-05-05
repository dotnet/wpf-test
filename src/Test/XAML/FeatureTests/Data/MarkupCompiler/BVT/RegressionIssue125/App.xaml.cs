// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

namespace WpfApplication1
{
    /// <summary>
    /// Repro for bug 641006.
    /// When multiple assemblies with the same short name gets loaded in current app domain,
    /// BAML loading failed when trying to resolve assembly.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Custom Main method to do exception handling for app.Run
        /// To skip auto generation of Main, App.xaml build action is changed 
        /// from ApplicationDefinition to Page
        /// </summary>
        [System.STAThreadAttribute()]
        public static void Main()
        {
            WpfApplication1.App app = new WpfApplication1.App();
            app.InitializeComponent();
            try
            {
                Application.Current.Shutdown(app.Run());
            }
            // This is the bug 641006. 
            // The bug caused InvalidOperationException (wrapped inside couple of XamlParseExceptions) to be thrown
            catch (Exception e) 
            {
                Console.WriteLine("Caught following exception:\n" + e.ToString());
                Application.Current.Shutdown(1);
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Loading a second copy of application assembly in current appdomain
            // Assembly paths need to be different for two copies of the same assembly to get loaded
            Assembly.LoadFile(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"Dir1\RegressionIssue125.exe");
        }
    }
}
