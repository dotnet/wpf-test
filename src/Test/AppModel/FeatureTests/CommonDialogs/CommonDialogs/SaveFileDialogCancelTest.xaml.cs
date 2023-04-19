// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;

using System.Windows.Interop;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;


namespace Microsoft.Wpf.AppModel.CommonDialogs
{
    public partial class SaveFileDialogCancelTest:Page
    {
        SaveFileDialog _sfd = null;
        TestLog        _log = null;
        bool _fileOkHappened = false;
        
        private void OnLoadedShowSaveFileDlg(object sender, RoutedEventArgs e)
        {
            _log = TestLog.Current;
            if (_log == null)
            {
                statusText.Text = "Could not find a currentTestLog.  Exiting.";
                return;
            }
            
            GlobalLog.LogEvidence("Creating a new SaveFileDialog.");
            _sfd = new SaveFileDialog();
            if (_sfd == null)
            {
                statusText.Text = "Could not create a new SaveFileDialog. Exiting.";
                Common.ExitWithError(statusText.Text);
                return;
            }
            
            GlobalLog.LogEvidence("Registering SaveFileDialog eventhandlers");
            _sfd.FileOk += new CancelEventHandler(OnSaveFileDialogOk);
            
            // Set starting dir to current directory so that previous tests do not
            // affect the starting location of the SaveFileDialog
            _sfd.InitialDirectory = Directory.GetCurrentDirectory();

            GlobalLog.LogEvidence("Displaying SaveFileDialog with ShowDialog()");
            _sfd.ShowDialog(); 

            if (!_fileOkHappened) // this is the case where we canceled the dialog instead of OK'ing it.
            {
                GlobalLog.LogEvidence("Dialog was closed without using OK button.");
                Common.ExitWithPass();
            }
        }
        
        private void OnSaveFileDialogOk(object sender, CancelEventArgs e)
        {
            _fileOkHappened = true;       
            statusText.Text = "Dialog was closed using OK button instead of Cancel.";     
            Common.ExitWithError(statusText.Text);
        }      
    }
}
