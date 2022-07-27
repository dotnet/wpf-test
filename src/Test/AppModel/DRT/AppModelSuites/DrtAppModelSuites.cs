// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using DRT;

[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/loader_tulip.jpg")]
[assembly: System.Windows.Resources.AssemblyAssociatedContentFile(@"drtfiles/pagea.xaml")]


namespace DRT
{
    public sealed class DrtAppModelSuites : DrtBase
    {
        [STAThread]
        public static int Main(string[] args)
        {
            DrtBase drt = new DrtAppModelSuites();
            return drt.Run(args);
        }

        private DrtAppModelSuites()
        {
            WindowTitle = "AppModel DRT Suites";

            DrtName = "DrtAppModelSuites";
            TeamContact = "WPF";
            Contact = "Microsoft";

            Suites = new DrtTestSuite[]
                                {
                                    new NewLoaderSuite(),
                                    new DataStreamsSuite(),
                                    new CacheSuite(),
                                    new JournalingSuite(),
                                    new TransparentWindowSuite(),
                                };
        }

        public void SetMainWindow(Window window)
        {
            PropertyInfo info = window.GetType().GetProperty("HwndSourceWindow", BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic);
            object obj = info.GetValue(window, null);

            MainWindow = (HwndSource)obj;
        }

        public void UseNewWindow()
        {
            Window window = new Window();

            window.Visibility = Visibility.Visible;
            window.Title = "Window";

            SetMainWindow(window);
        }
    }
}
