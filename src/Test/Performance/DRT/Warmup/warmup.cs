// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable 1634, 1691

using System;
using System.Threading; 
using System.Windows.Threading;

using System.Windows;
using System.Windows.Navigation;
using System.Runtime.InteropServices;

namespace DevTest.Perf.Scenario
{
    public class MSN
    {
        [STAThread]
        public static void Main(string[] args)
        {
            String XAMLFileName = "DrtFiles/WarmupOpt/warmup.xaml";

            if (args.Length > 0)
            {
                for (int i=0;i<args.Length;i++)
                {
#pragma warning suppress 56506
                    if ( (args[i].Length >= 1) && ( (args[i][0] == '-') || (args[i][0] == '/') ) )
                    {
                        switch (args[i].Substring(1).ToLower())
                        {
                            case "timeout":
                                i++;
                                if (i < args.Length)
                                {
                                    try
                                    {
                                        s_timeout = Int32.Parse(args[i]);
                                    }
                                    catch (ArgumentNullException)
                                    {
                                        Console.Error.WriteLine("Please specify a valid timeout value.");
                                    }
                                    catch (FormatException)
                                    {
                                        Console.Error.WriteLine("Please specify a valid timeout value.");
                                    }
                                    catch (OverflowException)
                                    {
                                        Console.Error.WriteLine("Please specify a valid timeout value.");
                                    }
                                }
                                else
                                {
                                    Console.Error.WriteLine("Please specify a valid timeout value.");
                                }
                                break;

                            case "file":
                                i++;
                                if (i < args.Length)
                                {
                                    try
                                    {
                                        XAMLFileName = args[i];
                                    }
                                    catch (ArgumentNullException)
                                    {
                                        Console.Error.WriteLine("Please specify a valid XAML filevalue.");
                                    }
                                    catch (FormatException)
                                    {
                                        Console.Error.WriteLine("Please specify a valid XAML filevalue.");
                                    }
                                    catch (OverflowException)
                                    {
                                        Console.Error.WriteLine("Please specify a valid XAML filevalue.");
                                    }
                                }
                                else
                                {
                                    Console.Error.WriteLine("Please specify a valid XAML filevalue.");
                                }
                                break;
                        }

                    }
                }


            }

            s_navApp = new Application();
            s_navApp.StartupUri = new Uri("pack://siteoforigin:,,,/" + XAMLFileName, UriKind.RelativeOrAbsolute);
            if (s_timeout > 0)
            {
                s_navApp.Navigated += new NavigatedEventHandler(OnNavigated);
            }
            s_navApp.Run();
        }

        private static void OnNavigated(Object sender, NavigationEventArgs e)
        {
            s_dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;
            s_dispatcher.BeginInvoke(
                    System.Windows.Threading.DispatcherPriority.Background,
                    new System.Windows.Threading.DispatcherOperationCallback(OnIdle),
                    null);
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
            QuitWithDelay(s_timeout);

            return null;
        }

        private static System.Windows.Threading.Dispatcher s_dispatcher =
                                System.Windows.Threading.Dispatcher.CurrentDispatcher;


        private static Int32 s_timeout = 5000;

        private static Application s_navApp;

    }




}

