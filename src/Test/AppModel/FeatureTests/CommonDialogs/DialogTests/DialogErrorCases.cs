// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace Microsoft.Windows.Test.Client.AppSec.P1
{
    public class DialogErrorCases : BaseTestNavApp 
    {
        public DialogErrorCases()
        {
            Description = "Check specific error cases in CommonDialog classes";
        }

        protected override void OnStartup (StartupEventArgs e)
        {
            bool caught = false;

            Window testWindow = new Window();
            OpenFileDialog openDlg = new OpenFileDialog();
            SaveFileDialog saveDlg = new SaveFileDialog();


            //Case 1: OpenFileDialog.ShowDialog(Window w) where w is created but not shown (it doesn't have an hwnd at that point)
            try
            {
                openDlg.ShowDialog(testWindow);
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }

            if(caught)
            {
                DialogUtilities.LogPass("Correctly got InvalidOperationException when calling OpenDlg.ShowDialog(Window) on non-shown window");
            }
            else
            {
                DialogUtilities.LogFail("Failed to get InvalidOperationException when calling OpenDlg.ShowDialog(Window) on non-shown window");
            }

            //Case 2: OpenFileDialog.ShowDialog() where the initial filename is not a valid file name.
            //NOTE: on the new Vista dialog, this will not be considered invalid.  By design.  So we check for Vista and skip this.
            if (Environment.OSVersion.Version.Major < 6)
            { 
                caught = false;
                openDlg.FileName = "/*";
                try
                {
                    openDlg.ShowDialog();
                }
                catch (InvalidOperationException)
                {
                    caught = true;
                }

                if(caught)
                {
                    DialogUtilities.LogPass("Correctly got InvalidOperationException when calling OpenDlg.ShowDialog() on invalid initial filename");
                }
                else
                {
                    DialogUtilities.LogFail("Failed to get InvalidOperationException when calling OpenDlg.ShowDialog() on invalid initial filename");
                }
            }

            //Case 3: OpenFileDialog.OpenFile() where the filename is ""
            caught = false;
            openDlg.FileName = "";
            try
            {
                Stream s1 = openDlg.OpenFile();
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }
            
            if(caught)
            {
                DialogUtilities.LogPass("Correctly got InvalidOperationException when calling OpenDlg.OpenFile on empty string");
            }
            else
            {
                DialogUtilities.LogFail("Failed to get InvalidOperationException when calling OpenDlg.OpenFile on empty string");
            }


            //Case 4: SaveFileDialog.OpenFile() where the filename is ""
            caught = false;
            saveDlg.FileName = "";
            try
            {
                Stream s2 = saveDlg.OpenFile();
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }

            if(caught)
            {
                DialogUtilities.LogPass("Correctly got InvalidOperationException when calling SaveDlg.OpenFile on empty string");
            }
            else
            {
                DialogUtilities.LogFail("Failed to get InvalidOperationException when calling SaveDlg.OpenFile on empty string");
            }


            //Case 5: OpenFileDialog.OpenFiles() where filename[0] is ""
            caught = false;
            try
            {
                Stream s3 = openDlg.OpenFiles()[0];
            }
            catch (InvalidOperationException)
            {
                caught = true;
            }

            if(caught)
            {
                DialogUtilities.LogPass("Correctly got InvalidOperationException when calling OpenDlg.OpenFiles on empty string");
            }
            else
            {
                DialogUtilities.LogFail("Failed to get InvalidOperationException when calling OpenDlg.OpenFiles on empty string");
            }

            testWindow.Close();
            Shutdown();
        }
    }
}
