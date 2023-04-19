// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;

namespace Dummy
{
    class Program
    {
        static Application s_app = null;
        static Page s_mainPage = null;
        static bool s_firstCheck = false;
        static bool s_secondCheck = false;

        [STAThread]
        static void Main(string[] args)
        {
            s_app = new Application();
            s_app.Startup += new StartupEventHandler(app_StartingUp);
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(MyTest), null);
        }

        static void app_StartingUp(object sender, StartupEventArgs e)
        {
            s_mainPage = new Page();
            s_mainPage.WindowTitle = "Code Only Xapp - Waiting to call App.Run()";
            s_app.MainWindow.Content = s_mainPage;
            try
            {
                s_app.Run();
                s_firstCheck = false;
            }
            catch (InvalidOperationException)
            {
                s_firstCheck = true;
            }                        
        }

        private static object MyTest(object notused)
        {
            try
            {
                Application.Current.Run();
                s_secondCheck = false;
            }
            catch (InvalidOperationException)
            {
                s_secondCheck = true;
            }
            if (s_firstCheck && s_secondCheck)
            {
                s_mainPage.WindowTitle = "PASSED - EXCEPTIONS THROWN";
            }
            else
            {
                s_mainPage.WindowTitle = "FAILED - EXCEPTIONS NOT THROWN";
            }
            return null;
        }
    }
}
