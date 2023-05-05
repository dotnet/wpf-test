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
    public partial class OpenFolderDialogAllCustomPlacesTest : Page
    {
        OpenFolderDialog folderDialog = null;

        bool fileOkHappened = false;
        
        private void OnLoadedShowOpenFolderDlg(object sender, RoutedEventArgs e)
        {     
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

            //Add all available CustomPlaces.  On Vista these will be shown.  On XP they won't be.  But it's valid to add them.
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Contacts);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.RoamingApplicationData);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.LocalApplicationData);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Cookies);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Favorites);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Programs);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Music);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Pictures);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.SendTo);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.StartMenu);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Startup);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.System);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Templates);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Desktop);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.Documents);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.ProgramFiles);
            folderDialog.CustomPlaces.Add(FileDialogCustomPlaces.ProgramFilesCommon);

            GlobalLog.LogEvidence("Displaying OpenFolderDialog with ShowDialog()");
            folderDialog.ShowDialog(); 

            if (!fileOkHappened) // this is the expected case where we canceled the dialog instead of OK'ing it.
            {
                GlobalLog.LogEvidence("As expected, dialog was closed without using OK button.");
                Common.ExitWithPass();
            }
        }
        
        private void OnOpenFolderDialogOk(object sender, CancelEventArgs e)
        {
            fileOkHappened = true;       
            statusText.Text = "Not expected: dialog was closed using OK button instead of Cancel.";     
            Common.ExitWithError(statusText.Text);
        } 
    }
}

