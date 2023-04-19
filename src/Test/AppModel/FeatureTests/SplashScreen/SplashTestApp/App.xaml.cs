// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace SplashScreenTestApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App() : base()
        {
            if ( (Environment.GetCommandLineArgs().Length == 2) &&
                 (Environment.GetCommandLineArgs()[1].ToLowerInvariant() == "pauseonstartup"))
            {
                // Create a semaphore and hog its one resource
                Semaphore brokenSemaphore = new Semaphore(1, 1);
                brokenSemaphore.WaitOne();

                Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, (DispatcherOperationCallback)delegate(object unused)
                {
                    // Now try to get it.  This should never acquire! Muahahaha!
                    brokenSemaphore.WaitOne();
                    return null;
                },
                null);
            }

        }


    }
}
