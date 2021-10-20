// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Navigation;

namespace DrtStartupUriXamlNavigationWindow
{
    public partial class DrtStartupUriXamlNavigationWindowApp: Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Log(@"DrtStartupUriXamlNavigationWindow [AppModel\Microsoft]");

            base.OnStartup(e);

            Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(Verify), null);
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ShutdownApp), null);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_success)
                Log("Test PASSED");
            else
                Log("Test FAILED");

            base.OnExit(e);
        }

        private object Verify (object obj)
        {
            Window w = Application.Current.MainWindow;

            if (w.Visibility != Visibility.Visible)
            {
                _success = false;
                Log("NavigationWindow.Visibility test failed: actual value = {0}; expected value = Visibility.Visibile", w.Visibility);
            }

            if (w.ActualWidth != 500)
            {
                _success = false;
                Log("NavigationWindow Width test failed: actual value = {0}; expected value = 500", w.ActualWidth);
            }

            if (w.ActualHeight != 500)
            {
                _success = false;
                Log("NavigationWindow Height test failed: actual value = {0}; expected value = 500", w.ActualHeight);
            }

            if (BrowserInteropHelper.IsBrowserHosted)
            {
                _success = false;
                Log("BrowserInteropHelper.IsBrowserHosted incorrectly returned true");
            }

            object o = BrowserInteropHelper.ClientSite;
            if (o != null)
            {
                _success = false;
               Log("BrowserInteropHelper.ClientSite incorrectly returned a value");
            }

            return null;
        }

        private object ShutdownApp (object obj)
        {
            Application.Current.Shutdown(_success ? 0 : 1);
            return null;
        }


        private void Log(string message, params object[] args)
        {
            _logger.Log(message, args);
        }

        private bool    _success    = true;


        private DRT.Logger _logger  = new DRT.Logger("DrtStartupUriXamlNavigationWindow", "Microsoft", "Testing StartupUri xaml pointing to NavigationWindow");
    }
}
