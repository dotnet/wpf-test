// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Win32;
using Microsoft.Test.Logging;

namespace Microsoft.Test.WPF.AppModel.CommonDialogs
{
    public class OpenFileOnSecondThread : Application
    {
        DispatcherTimer _timer = new DispatcherTimer();
        OpenFileDialog _openDialog = new OpenFileDialog();

        protected override void OnStartup (StartupEventArgs e)
        {
            //We need to verify we can't show the dialog on a second thread, so start one.
            ThreadStart start = new ThreadStart(NewThread);
            Thread thread = new Thread(start);
            thread.Start();

            thread.Join();

            Shutdown();
        }
 
        private void NewThread()
        {
            bool caught = false;

            try
            {
                _openDialog.ShowDialog();
                // if we don't get any exception, the test will eventually fail by timing out.
            }
            catch (InvalidOperationException)
            {
                caught = true;
                Logging.LogPass("As expected, calling ShowDialog on a second thread threw an InvalidOperationException");
            }
            catch (Exception exception)
            {
                Logging.LogFail("Calling ShowDialog on a second thread threw an unexpected exception: " + exception.ToString());
            }

            if(!caught)
            {
                Logging.LogFail("Calling ShowDialog on a second thread threw an unexpected exception or no exception.");
            }
        }
    }
}
