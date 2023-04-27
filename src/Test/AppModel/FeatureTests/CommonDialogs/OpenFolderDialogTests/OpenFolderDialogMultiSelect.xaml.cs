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
    public partial class OpenFolderDialogMultiSelect : Page
    {
        OpenFolderDialog folderDialog = null;

        private static string folderName = "testdata";
        private static string[] expextedFileNames = new string[] {"dir1", "dir3"};
        
        private void OnLoadedShowOpenFolderDlg(object sender, RoutedEventArgs e)
        {     
            GlobalLog.LogEvidence("Creating test data structure for folder dialog tests");
            FolderDialogVerifyHelper.CreateTestFoldersStructure(Environment.CurrentDirectory);

            //this test is Vista-only since we actually try to click the CustomPlace.  Just return ignore on non-Vista.
            if (Environment.OSVersion.Version.Major < 6)
            { 
                GlobalLog.LogEvidence("OS is pre-Vista, ignore.");
                Common.ExitWithIgnore();
                return;
            }       

            GlobalLog.LogEvidence("Creating new OpenFolderDialog");
            folderDialog = new OpenFolderDialog();
            if (folderDialog == null)
            {
                statusText.Text = "Could not create a new OpenFolderDialog. Exiting.";
                Common.ExitWithError(statusText.Text);
                return;
            }
            
            GlobalLog.LogEvidence("Registering OpenFolderDialog eventhandlers");
            folderDialog.FileOk += new CancelEventHandler(OnOpenFolderDialogOk);
            
            // Set starting dir to current directory so that previous tests do not
            // affect the starting location of the OpenFolderDialog
            string initialDirectory = Path.Combine(Directory.GetCurrentDirectory(), "testdata");
            folderDialog.InitialDirectory = initialDirectory;

            // setting multiselect on dialog
            folderDialog.Multiselect = true;

            GlobalLog.LogEvidence("Displaying OpenFolderDialog with ShowDialog()");
            folderDialog.ShowDialog(); 
        }
        
        private void OnOpenFolderDialogOk(object sender, CancelEventArgs e)
        {
            bool testResult = true; 
            
            // [1] check multiple files are selected
            GlobalLog.LogEvidence("Checking the number of files selected ...");
            testResult = folderDialog.FileNames.Length > 1;

            // [2] check selected Folders
            GlobalLog.LogEvidence("Checking if the selected folders are the expected ones...");
            testResult = testResult && FolderDialogVerifyHelper.CheckSelectedFolders(folderDialog.FileNames, expextedFileNames);
            
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

