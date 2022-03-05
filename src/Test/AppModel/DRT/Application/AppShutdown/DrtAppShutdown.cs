// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;

namespace DRT
{
/// <summary>
/// Calling Shutdown from inside a dialog causes InvalidOperationException
/// The problem is actually caused by calling Shutdown with more than one window is open.
/// 
/// Note that this DRT does not assert anything, its failure mode is an exception on shutdown.
/// </summary>
    class DrtAppShutdown : Application
    {
        private Logger _logger = new Logger("DrtAppShutdown", "Microsoft", "Tests app shutdown when multiple windows open that have owned windows");
        const int WindowCount = 2; // We just need more than one.

        static bool s_passed = false;

        protected override void OnStartup(StartupEventArgs e)
        {
            // Open a bunch of windows
            for (int ownerWindowCount = 0; ownerWindowCount < WindowCount; ++ownerWindowCount)
            {
                Window ownerWindow = TestWindow();

                ownerWindow.Title = "Main:" + ownerWindowCount.ToString();
                ownerWindow.Show();

                // Since owned windows are closed when their parent is closed,
                // and they were once an issue, make some here.
                for (int ownedWindowCount = 0; ownedWindowCount < WindowCount; ++ownedWindowCount)
                {
                    Window ownedWindow = TestWindow();

                    ownedWindow.Title = "Owned: " + ownerWindowCount.ToString() + ":" + ownedWindowCount.ToString();
                    ownedWindow.Owner = ownerWindow;
                    ownedWindow.Show();
                }
            }

            // Calling shutdown used to cause a crash because DoShutdown was iterating over a list while
            // the list was being changed.
            this.Shutdown();
            s_passed = true;
        }

        Window TestWindow()
        {
            Window window = new Window();

            window.Width = 200;
            window.Height = 100;
            return window;
        }

        [STAThread]
        public static int Main()
        {
            DrtAppShutdown theApp = new DrtAppShutdown();
            theApp.Run();
            if (s_passed)
            {
                theApp._logger.Log("s_passed.");
                return 0;
            }
            else
            {
                theApp._logger.Log("Failed.");
                return 1;
            }
        }
    }
}
