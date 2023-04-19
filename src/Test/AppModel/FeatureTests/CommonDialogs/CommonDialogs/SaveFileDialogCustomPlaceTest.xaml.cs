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
    public partial class SaveFileDialogCustomPlaceTest : Page
    {
        SaveFileDialog _saveFileDialog = null;
        
        private void OnLoadedShowSaveFileDlg(object sender, RoutedEventArgs e)
        {     
            //this test is Vista-only since it tries to click a CustomPlace.  Just return ignore on non-Vista.
            if (Environment.OSVersion.Version.Major < 6)
            { 
                GlobalLog.LogEvidence("OS is pre-Vista, ignore.");
                Common.ExitWithIgnore();
                return;
            }       

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

            //set Pictures as our custom place
            _saveFileDialog.CustomPlaces.Add(FileDialogCustomPlaces.Pictures);

            GlobalLog.LogEvidence("Displaying SaveFileDialog with ShowDialog()");
            _saveFileDialog.ShowDialog(); 
        }
        
        private void OnSaveFileDialogOk(object sender, CancelEventArgs e)
        {
            bool testResult = true; 
            Stream fileStream = _saveFileDialog.OpenFile();
             
            // [1] check Stream properties
            testResult = SaveDialogVerifyHelper.CheckStreamProperties(fileStream);
            if (!testResult)
            {
                Common.ExitWithError(statusText.Text);
            }
             
            // [2] try writing to Stream
            testResult = SaveDialogVerifyHelper.WriteToStream(fileStream);
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

