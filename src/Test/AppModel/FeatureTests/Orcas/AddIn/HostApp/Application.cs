// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;

namespace ClickCountHostApp
{
    /// <summary>
    /// Instead of using App.xaml/App.xaml.cs to create the appdef, we do it manually.
    /// This is so we can set the MultiDomainHost loader optimization, so the adding of new addins will be fast.
    /// </summary>
    public class App : Application
    {
        public App()
        {
            this.StartupUri = new System.Uri("Window1.xaml", System.UriKind.Relative);
        }

        public static Application MyApp;

        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        public static void Main()
        {
            ClickCountHostApp.App app = new ClickCountHostApp.App();
            MyApp = app;
            app.Run();
        }
    }
}
