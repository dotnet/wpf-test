// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
    using System.Windows;
    using System.Windows.Navigation;
    using System.Windows.Controls;
    

    [assembly:CLSCompliant(true)]

    
    namespace Local
    {
        public class MyButton : Button
        {
        }
    }
    

    namespace Harness
    {
        public class OnStartupBVT : System.Windows.Application
        {
            protected override void OnStartup(System.Windows.StartupEventArgs e)
            {
                this.StartupUri = new Uri(@"MappingPIMarkup.xaml", UriKind.RelativeOrAbsolute);
                this.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(OnLoad);
                base.OnStartup(e);
            }

            void OnLoad(object sender, NavigationEventArgs e)
            {
                Console.WriteLine("LoadCompleted event fired");

                ShutdownApp();
            }

            protected override void OnExit(ExitEventArgs e)
            {
                Console.WriteLine("Shutting down application");
            }

            private void ShutdownApp()
            {
                Application.Current.Shutdown(0);
                Microsoft.Test.Loaders.ApplicationMonitor.NotifyStopMonitoring();
            }
        }
           
        public class EntryPoint
        {
            [System.STAThread()]
            public static int Main(string[] args)
            {
                System.Threading.Thread.CurrentThread.SetApartmentState(System.Threading.ApartmentState.STA);
                OnStartupBVT app = new OnStartupBVT();
                return app.Run();
            }
        }
    }    

