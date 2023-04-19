// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Threading;
using System.Windows.Automation;
using System.Reflection;
using System.Windows.Threading;

namespace SplashScreenTestApp
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        private static SplashScreen s_currentSplashScreen = null;
        private static TimeSpan s_splashTimeoutMillis = TimeSpan.FromMilliseconds(300);
        private static string[] s_cmdLineArgs = null;

        public Window1()
        {
            InitializeComponent();
            s_cmdLineArgs = Environment.GetCommandLineArgs();
        }

        private void DoSplashScreenAPITest(object sender, RoutedEventArgs e)
        {            
            string resourceName = "splash.bmp";
            string assemblyName = "none";
            bool forceClose = false;

            if ((s_cmdLineArgs != null) && (s_cmdLineArgs.Length == 4))
            {
                resourceName = s_cmdLineArgs[1];
                assemblyName = s_cmdLineArgs[2];
                forceClose = bool.Parse(s_cmdLineArgs[3]);

                Application.Current.MainWindow.Title = "Using:" + resourceName + " from " + ((assemblyName.ToLowerInvariant() == "none") ? " self" : assemblyName) + " and " +
                    (forceClose ? "" : "not") + " forcibly closing splash screen";
            }

            switch (((Button)sender).Name)
            {
                case "SplashTest1" :
                    TestWPFSplashScreen(resourceName, assemblyName, TimeSpan.FromMilliseconds(3000), false, forceClose, false);
                    break;
                case "SplashTest2":
                    TestWPFSplashScreen(resourceName, assemblyName, TimeSpan.FromMilliseconds(3000), true, forceClose, false);
                    break;
                case "SplashTest3":
                    TestWPFSplashScreen(resourceName, assemblyName, TimeSpan.FromMilliseconds(3000), false, false, true);
                    break;
                case "SplashTest4":
                    {
                        TestWPFSplashScreen(resourceName, assemblyName, TimeSpan.FromMilliseconds(3000), false, false, false);
                        new Thread(
                            new ThreadStart(
                                delegate
                                {
                                    Window1.s_currentSplashScreen.Close(TimeSpan.FromMilliseconds(200));
                                }
                                )).Start();
                        break;
                    }
                // Regression case .  Closing a SplashScreen that has not been shown throws nullref
                case "SplashTest5":
                    {
                        try
                        {
                            SplashScreen splashScreen = new SplashScreen("splash.bmp");
                            splashScreen.Close(new TimeSpan(0, 0, 0));
                            this.Title = "No Null Ref observed for closing un-Show()-n SplashScreen";
                        }
                        catch (System.NullReferenceException)
                        {
                            this.Title = "ERROR : Null Ref Exception caught";
                        }
                        break;
                    }
            }
        }

        private bool TestWPFSplashScreen(string resourceName, string assembly, TimeSpan closeTimeSpan, bool autoClose, bool forceClose, bool topMost)
        {
            s_splashTimeoutMillis = closeTimeSpan;
            SplashScreen splashScreenToTest = null;
            if (assembly.ToLowerInvariant() == "none")
            {
                splashScreenToTest = new SplashScreen(resourceName);
            }
            else
            {
                Assembly resourceAssembly = Assembly.LoadFile(System.IO.Path.GetFullPath(assembly));
                splashScreenToTest = new SplashScreen(resourceAssembly, resourceName);
            }
            if (splashScreenToTest == null)
            {
                Debug.WriteLine("Error: Could not create WPF Splash screen!");
                return false;
            }
            s_currentSplashScreen = splashScreenToTest;

#if TESTBUILD_CLR40
            splashScreenToTest.Show(autoClose, topMost);
#endif 
#if TESTBUILD_CLR20
            splashScreenToTest.Show(autoClose);
#endif 

            // Start closing the dialog as early as possible and do it twice.
            // This ensures 1) it's OK to call close twice (should be)
            // and 2) no issues with the dispatcher timer used in fadeout being disposed twice.
            if (forceClose)
            {
                splashScreenToTest.Close(TimeSpan.Zero);
                splashScreenToTest.Close(TimeSpan.Zero);
            }

            return true;
        }

        private void CloseSplashScreen(object sender, RoutedEventArgs e)
        {
            if (s_currentSplashScreen != null)
            {
                s_currentSplashScreen.Close(s_splashTimeoutMillis);
            }
        }        
    }    
}
