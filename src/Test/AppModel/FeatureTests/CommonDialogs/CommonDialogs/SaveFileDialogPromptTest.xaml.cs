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
    public partial class SaveFileDialogPromptTest : Page
    {
        SaveFileDialog _sfd = null;
        
        private void OnLoadedShowSaveFileDlg(object sender, RoutedEventArgs e)
        {
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
            _sfd.CreatePrompt = true;
            
            // Set starting dir to current directory so that previous tests do not
            // affect the starting location of the SaveFileDialog
            _sfd.InitialDirectory = Directory.GetCurrentDirectory();

            GlobalLog.LogEvidence("Displaying SaveFileDialog with ShowDialog()");
            _sfd.ShowDialog(); 
        }
        
        private void OnSaveFileDialogOk(object sender, CancelEventArgs e)
        {
            bool testResult = true;
            Stream sfdStream = _sfd.OpenFile();

            // [1] get code coverage for event remove
            _sfd.FileOk -= new CancelEventHandler(OnSaveFileDialogOk);
            
            // [2] Check Stream properties
            testResult = SaveDialogVerifyHelper.CheckStreamProperties(sfdStream);
            if (!testResult)
            {
                Common.ExitWithError(statusText.Text);
            }
                                
            // [3] Attempt writing to stream and log when the new data was written
            testResult = SaveDialogVerifyHelper.WriteToStream(sfdStream);
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
