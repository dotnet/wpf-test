// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Sprinkle
{
     public partial class SprinkleApp
     {
         protected override void OnStartup(System.Windows.StartupEventArgs e)
         {
             // Setup the application window.
             System.Windows.Navigation.NavigationWindow window = new System.Windows.Navigation.NavigationWindow();
             window.ResizeMode = ResizeMode.CanResize;
             window.Width = 800.0;
             window.Height = 700.0;
             window.Left  = 0.0;
             window.Top  = 0.0;
             window.Title = "Sprinkle - Controls Performance Test";
             window.Show();

             // Add Navigated EventHandler
             Navigated += new System.Windows.Navigation.NavigatedEventHandler(OnNavigated);

             // Navigate to the startup page
             window.Navigate(new Uri("Sprinkle.xaml", UriKind.RelativeOrAbsolute));
         }

            private void OnNavigated(Object sender, System.Windows.Navigation.NavigationEventArgs e)
            {
                if (!s_initDone)
                {
                    s_dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
                    s_dispatcher.BeginInvoke(
                            System.Windows.Threading.DispatcherPriority.Background,
                            new System.Windows.Threading.DispatcherOperationCallback(OnIdle),
                            null);
                }
            }


            private static void QuitWithDelay(int delayInMs)
            {
                System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer(
                                System.TimeSpan.FromMilliseconds(delayInMs),
                                System.Windows.Threading.DispatcherPriority.Background,
                                new EventHandler(OnQuitTimer),
                                s_dispatcher);
            }

            private static void OnQuitTimer(object sender, EventArgs e)
            {
                System.Windows.Threading.DispatcherTimer timer = (System.Windows.Threading.DispatcherTimer)sender;
                timer.IsEnabled = false;

                s_dispatcher.InvokeShutdown();
            }

            private static object OnIdle(object arg)
            {
                QuitWithDelay(200);

                return null;
            }

            private static bool s_initDone = false;
            private static System.Windows.Threading.Dispatcher s_dispatcher =
                                        System.Windows.Threading.Dispatcher.CurrentDispatcher;


    }
}
