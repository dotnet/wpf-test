// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;


namespace Microsoft.Wpf.AppModel.CommonDialogs
{
    public partial class OpenFileDialogTest : Page
    {
        OpenFileDialog _ofd = null;
        
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
        }
        
        private void OnOpenFileDialogOk(object sender, CancelEventArgs e)
        {
            bool testResult = true; 
            Stream ofdStream = _ofd.OpenFile();
             
            // [1] check Stream properties
            testResult = OpenDialogVerifyHelper.CheckStreamProperties(ofdStream);
            if (!testResult)
            {
                Common.ExitWithError(statusText.Text);
            }
             
            // [3] try writing to Stream
            testResult = OpenDialogVerifyHelper.WriteToStream(ofdStream);
            if (!testResult)
            {
                Common.ExitWithError(statusText.Text);
            }
            else
            {
                Common.ExitWithPass();
            }
        }
    }
}

