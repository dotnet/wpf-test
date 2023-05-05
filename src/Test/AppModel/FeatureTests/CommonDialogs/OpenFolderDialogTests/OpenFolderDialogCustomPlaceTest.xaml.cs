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
    public partial class OpenFolderDialogCustomPlaceTest : Page
    {
        OpenFolderDialog folderDialog = null;

        private static string folderName = "testdata";
        private static string[] expectedFileNames = new string[] {"dir1"};

        private void OnLoadedShowOpenFolderDlg(object sender, RoutedEventArgs e)
        {     
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
            folderDialog.InitialDirectory = Directory.GetCurrentDirectory();

            //set testdata as our custom place
            string pathName = Path.Combine(folderDialog.InitialDirectory, "testdata");
            folderDialog.CustomPlaces.Add(new FileDialogCustomPlace(pathName));

            GlobalLog.LogEvidence("Displaying OpenFolderDialog with ShowDialog()");
            folderDialog.ShowDialog(); 
        }
        
        private void OnOpenFolderDialogOk(object sender, CancelEventArgs e)
        {
            bool testResult = true; 
            
            // [1] check selected Folders
            testResult = FolderDialogVerifyHelper.CheckSelectedFolders(folderDialog.FileNames, expectedFileNames);
            if (!testResult)
            {
                Common.ExitWithError(statusText.Text);
            }             
        }
    }
}

