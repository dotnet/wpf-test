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
    public partial class OpenFolderDialogCancelTest : Page
    {
        OpenFolderDialog ofd = null;
        bool fileOkHappened = false;
        
        private void OnLoadedShowOpenFolderDlg(object sender, RoutedEventArgs e)
        {
            GlobalLog.LogEvidence("Creating new OpenFolderDialog");
            ofd = new OpenFolderDialog();
            if (ofd == null)
            {
                statusText.Text = "Could not create a new OpenFolderDialog. Exiting.";
                Common.ExitWithError(statusText.Text);
                return;
            }
            
            GlobalLog.LogEvidence("Registering OpenFolderDialog eventhandlers");
            ofd.FileOk += new CancelEventHandler(OnOpenFolderDialogOk);
            
            // Set starting dir to current directory so that previous tests do not
            // affect the starting location of the OpenFolderDialog
            ofd.InitialDirectory = Directory.GetCurrentDirectory();

            GlobalLog.LogEvidence("Displaying OpenFolderDialog with ShowDialog()");
            ofd.ShowDialog(); 

            if (!fileOkHappened) // this is the case where we canceled the dialog instead of OK'ing it.
            {
                GlobalLog.LogEvidence("Dialog was closed without using OK button.");
                Common.ExitWithPass();
            }
        }
        
        private void OnOpenFolderDialogOk(object sender, CancelEventArgs e)
        {
            fileOkHappened = true;       
            statusText.Text = "Dialog was closed using OK button instead of Cancel.";     
            Common.ExitWithError(statusText.Text);
        }
    }
}

