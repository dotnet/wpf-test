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
    public partial class SaveFileDialogInvalidCustomPlacesTest : Page
    {
        SaveFileDialog _saveFileDialog = null;
        bool _fileOkHappened = false;
        
        private void OnLoadedShowSaveFileDlg(object sender, RoutedEventArgs e)
        {     
            try
            {
                GlobalLog.LogEvidence("Creating new SaveFileDialog");
                _saveFileDialog = new SaveFileDialog();
                if (_saveFileDialog == null)
                {
                    statusText.Text = "Could not create a new SaveFileDialog. Exiting.";
                    Common.ExitWithError(statusText.Text);
                    return;
                }
                
                GlobalLog.LogEvidence("Registering SaveFileDialog eventhandlers");
                _saveFileDialog.FileOk += new CancelEventHandler(OnSaveFileDialogOk);
                
                // Set starting dir to current directory so that previous tests do not
                // affect the starting location of the OpenFileDialog
                _saveFileDialog.InitialDirectory = Directory.GetCurrentDirectory();

                //It's OK to add CustomPlaces on XP even though they won't be shown.  (Invalid ones won't be shown on Vista either in any case.)

                //set a bogus path as a custom place
                _saveFileDialog.CustomPlaces.Add(new FileDialogCustomPlace("c:\\boguspath"));

                //try to use an existing file as a custom place.  If this threw an exception, but not a non-existing file, it could be
                //used in a local file-detection attack in an .xbap
                _saveFileDialog.CustomPlaces.Add(new FileDialogCustomPlace("c:\\boot.ini"));

                //set a null path as a custom place
                _saveFileDialog.CustomPlaces.Add(new FileDialogCustomPlace(null));

                //set an empty guid as a custom place
                _saveFileDialog.CustomPlaces.Add(new FileDialogCustomPlace(Guid.Empty));

                //set a bogus guid as a custom place
                _saveFileDialog.CustomPlaces.Add(new FileDialogCustomPlace(new Guid("8983036C-1337-BAAD-F00D-102D10DCFD74")));

                GlobalLog.LogEvidence("Displaying SaveFileDialog with ShowDialog()");
                _saveFileDialog.ShowDialog(); 

                if (!_fileOkHappened) // this is the expected case where we canceled the dialog instead of OK'ing it.
                {
                    GlobalLog.LogEvidence("As expected, dialog was closed without using OK button.");
                    Common.ExitWithPass();
                }
            }
            catch (Exception ex)
            {
                GlobalLog.LogEvidence("Unexpected exception: " + ex.ToString());
                Common.ExitWithError("Fail due to unexpected exception");
            }
        }
        
        private void OnSaveFileDialogOk(object sender, CancelEventArgs e)
        {
            _fileOkHappened = true;       
            statusText.Text = "Not expected: dialog was closed using OK button instead of Cancel.";     
            Common.ExitWithError(statusText.Text);
        } 
    }
}

