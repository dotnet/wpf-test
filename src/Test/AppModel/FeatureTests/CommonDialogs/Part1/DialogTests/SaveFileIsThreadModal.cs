// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.Test.Logging;

namespace Microsoft.Test.WPF.AppModel.CommonDialogs
{
    public class SaveFileIsThreadModal : Application
    {
        DispatcherTimer _timer = new DispatcherTimer();

        protected override void OnStartup (StartupEventArgs e)
        {
            SaveFileDialog saveDialog = new SaveFileDialog();

            //We need to verify IsThreadModal while the dialog is up.  Use a timer that fires 2 seconds from now.
            _timer.Tick += TimerTick;
            _timer.Interval = new TimeSpan(0, 0, 2);
            _timer.Start();

            Logging.LogStatus("Showing the SaveDialog.");
            saveDialog.ShowDialog();
        }
 
        private void TimerTick(object sender, EventArgs e)
        {
            _timer.Stop();

            if (ComponentDispatcher.IsThreadModal)
            {
                Logging.LogPass("Calling ComponentDispatcher.IsThreadModal while the Save dialog was shown returned true as expected.");
            }
            else
            {
                Logging.LogFail("Calling ComponentDispatcher.IsThreadModal while the Save dialog was shown returned false.  Expected: true");
            }

            Shutdown();
        }
    }
}
