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
    public partial class OpenFileDialogCustomPlaceTest : Page
    {
        OpenFileDialog _openFileDialog = null;

        private static string s_fileName = "hello.txt";
        
        private void OnLoadedShowOpenFileDlg(object sender, RoutedEventArgs e)
        {     
            //this test is Vista-only since we actually try to click the CustomPlace.  Just return ignore on non-Vista.
            if (Environment.OSVersion.Version.Major < 6)
            { 
                GlobalLog.LogEvidence("OS is pre-Vista, ignore.");
                Common.ExitWithIgnore();
                return;
            }       

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

            //create test folder under the current dir and copy our file to it
            string pathName = Path.Combine(_openFileDialog.InitialDirectory, "test");
            Directory.CreateDirectory(pathName);
            File.Copy(Path.Combine(_openFileDialog.InitialDirectory, s_fileName), Path.Combine(pathName, s_fileName), true);

            //set test as our custom place
            _openFileDialog.CustomPlaces.Add(new FileDialogCustomPlace(pathName));

            GlobalLog.LogEvidence("Displaying OpenFileDialog with ShowDialog()");
            _openFileDialog.ShowDialog(); 
        }
        
        private void OnOpenFileDialogOk(object sender, CancelEventArgs e)
        {
            bool testResult = true; 
            Stream fileStream = _openFileDialog.OpenFile();
             
            // [1] check Stream properties
            testResult = OpenDialogVerifyHelper.CheckStreamProperties(fileStream);
            if (!testResult)
            {
                Common.ExitWithError(statusText.Text);
            }
             
            // [2] try writing to Stream
            testResult = OpenDialogVerifyHelper.WriteToStream(fileStream);
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

