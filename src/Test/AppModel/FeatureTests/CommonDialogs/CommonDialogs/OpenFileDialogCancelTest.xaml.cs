// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Controls;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;


namespace Microsoft.Wpf.AppModel.CommonDialogs
{
    public partial class OpenFileDialogCancelTest : Page
    {
        OpenFileDialog _ofd = null;
        bool _fileOkHappened = false;
        
        private void OnLoadedShowOpenFileDlg(object sender, RoutedEventArgs e)
        {
            GlobalLog.LogEvidence("Creating new OpenFileDialog");
            _ofd = new OpenFileDialog();
            if (_ofd == null)
            {
                statusText.Text = "Could not create a new OpenFileDialog. Exiting.";
                Common.ExitWithError(statusText.Text);
                return;
            }
            
            GlobalLog.LogEvidence("Registering OpenFileDialog eventhandlers");
            _ofd.FileOk += new CancelEventHandler(OnOpenFileDialogOk);
            
            // Set starting dir to current directory so that previous tests do not
            // affect the starting location of the OpenFileDialog
            _ofd.InitialDirectory = Directory.GetCurrentDirectory();

            GlobalLog.LogEvidence("Displaying OpenFileDialog with ShowDialog()");
            _ofd.ShowDialog(); 

            if (!_fileOkHappened) // this is the case where we canceled the dialog instead of OK'ing it.
            {
                GlobalLog.LogEvidence("Dialog was closed without using OK button.");
                Common.ExitWithPass();
            }
        }
        
        private void OnOpenFileDialogOk(object sender, CancelEventArgs e)
        {
            _fileOkHappened = true;       
            statusText.Text = "Dialog was closed using OK button instead of Cancel.";     
            Common.ExitWithError(statusText.Text);
        }
    }
}

