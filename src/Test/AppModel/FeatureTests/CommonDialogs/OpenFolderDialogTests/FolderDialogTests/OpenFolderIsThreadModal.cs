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
    public class OpenFolderIsThreadModal : Application
    {
        DispatcherTimer timer = new DispatcherTimer();

        protected override void OnStartup (StartupEventArgs e)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();

            //We need to verify IsThreadModal while the dialog is up.  Use a timer that fires 2 seconds from now.
            timer.Tick += TimerTick;
            timer.Interval = new TimeSpan(0, 0, 2);
            timer.Start();

            Logging.LogStatus("Showing the OpenFolderDialog.");
            openFolderDialog.ShowDialog();
        }
 
        private void TimerTick(object sender, EventArgs e)
        {
            timer.Stop();

            if (ComponentDispatcher.IsThreadModal)
            {
                Logging.LogPass("Calling ComponentDispatcher.IsThreadModal while the openFolderDialog was shown returned true as expected.");
            }
            else
            {
                Logging.LogFail("Calling ComponentDispatcher.IsThreadModal while the openFolderDialog was shown returned false.  Expected: true");
            }

            Shutdown();
        }
    }
}