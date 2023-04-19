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
    public partial class OpenFileDialogAllCustomPlacesTest : Page
    {
        OpenFileDialog _openFileDialog = null;

        bool _fileOkHappened = false;
        
        private void OnLoadedShowOpenFileDlg(object sender, RoutedEventArgs e)
        {     
            GlobalLog.LogEvidence("Creating new OpenFileDialog");
            _openFileDialog = new OpenFileDialog();
            if (_openFileDialog == null)
            {
                statusText.Text = "Could not create a new OpenFileDialog. Exiting.";
                Common.ExitWithError(statusText.Text);
                return;
            }
            
            GlobalLog.LogEvidence("Registering OpenFileDialog eventhandlers");
            _openFileDialog.FileOk += new CancelEventHandler(OnOpenFileDialogOk);
            
            // Set starting dir to current directory so that previous tests do not
            // affect the starting location of the OpenFileDialog
            _openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

            //Add all available CustomPlaces.  On Vista these will be shown.  On XP they won't be.  But it's valid to add them.
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Contacts);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.RoamingApplicationData);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.LocalApplicationData);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Cookies);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Favorites);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Programs);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Music);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Pictures);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.SendTo);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.StartMenu);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Startup);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.System);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Templates);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Desktop);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Documents);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.ProgramFiles);
            _openFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.ProgramFilesCommon);

            GlobalLog.LogEvidence("Displaying OpenFileDialog with ShowDialog()");
            _openFileDialog.ShowDialog(); 

            if (!_fileOkHappened) // this is the expected case where we canceled the dialog instead of OK'ing it.
            {
                GlobalLog.LogEvidence("As expected, dialog was closed without using OK button.");
                Common.ExitWithPass();
            }
        }
        
        private void OnOpenFileDialogOk(object sender, CancelEventArgs e)
        {
            _fileOkHappened = true;       
            statusText.Text = "Not expected: dialog was closed using OK button instead of Cancel.";     
            Common.ExitWithError(statusText.Text);
        } 
    }
}

