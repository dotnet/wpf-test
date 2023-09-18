// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Description: Launches the application.

#if !XamlPadExpressApp
using System;
using System.Windows;
using System.Windows.Threading;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.ComponentModel;
using System.Threading;
using System.IO;


namespace XamlPadEdit
{
    public static class XamlPadMain
    {
        public static Thread _thread;
        public static bool _resourcesExist = false;
        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public static void Main()
        {
            if ((File.Exists("unzip.exe") && (File.Exists("zipResource.zip"))) == false)
            {
                _resourcesExist = false;
            }
            else
            {
                _resourcesExist = true;
            }
            _thread = new Thread(new ThreadStart(OpenSplashScreen));
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.IsBackground = false;
            _thread.Start();
            
            StartApplication();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void StartApplication()
        {
            XamlPadEdit.XamlPadApp app = new XamlPadEdit.XamlPadApp();
            
            app.InitializeComponent();
            app.Run(new Window());
        }

        private static void OpenSplashScreen()
        {
            s_splashScreen =(_resourcesExist)?new SplashScreen(2): new SplashScreen(10);
            s_splashScreen.Open(_resourcesExist);
        }

        delegate void SimpleDelegate();
        static void DoFocus()
        {
            Application.Current.MainWindow.Hide();
            Application.Current.MainWindow.Topmost = true;
            Application.Current.MainWindow.Visibility = Visibility.Visible;
            Application.Current.MainWindow.Topmost = false;
        }

        internal static void CloseSplashScreen()
        {
            try
            {
                _thread.Abort();
            }
            catch (Exception) { }
            Application.Current.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, new SimpleDelegate(DoFocus));
        }

        private static SplashScreen s_splashScreen;
    }
}
#endif
