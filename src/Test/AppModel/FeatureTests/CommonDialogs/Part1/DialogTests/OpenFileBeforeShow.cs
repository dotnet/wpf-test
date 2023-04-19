// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Microsoft.Test.Logging;

namespace Microsoft.Test.WPF.AppModel.CommonDialogs
{
    public class OpenFileBeforeShow : Application
    {
        protected override void OnStartup (StartupEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();
            bool caught = false;

            // Try to call OpenFile before the Open dialog has been shown.  Expect InvalidOperationException.
            Logging.LogStatus("Calling OpenFile before the Open dialog has been shown.  Expect InvalidOperationException.");

            try
            {
                Stream unused = openDlg.OpenFile();
            }
            catch (InvalidOperationException ex)
            {
                caught = true;
                Logging.SetPass("Caught expected InvalidOperationException: " + ex.ToString());
            }
            catch (Exception ex)
            {
                Logging.SetFail("Caught unexpected exception: " + ex.ToString());
            }

            if (caught)
            {
                Logging.LogPass("Calling OpenFile before the Open dialog has been shown threw an InvalidOperation as expected.");
            }
            else
            {
                Logging.LogFail("Calling OpenFile before the Open dialog has been shown either did not throw an exception or threw the wrong exception.");
            }

            Shutdown();
        }
    }
}
