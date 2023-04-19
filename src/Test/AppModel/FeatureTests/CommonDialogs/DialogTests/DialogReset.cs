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
    public class DialogReset : BaseTestNavApp 
    {
        public DialogReset()
        {
            Description = "Test that FileDialog.Reset resets expected properties";
        }

        protected override void OnStartup (StartupEventArgs e)
        {
            bool initialAddExtension;
            bool initialCheckPathExists;
            bool initialCreatePrompt;
            bool initialShowReadOnly;
            bool initialOverwritePrompt;

            OpenFileDialog openDlg = new OpenFileDialog();
            SaveFileDialog saveDlg = new SaveFileDialog();

            //Case 1: OpenFileDialog.Reset() resets expected properties
            initialAddExtension = openDlg.AddExtension;
            initialCheckPathExists = openDlg.CheckPathExists;
            initialShowReadOnly = openDlg.ShowReadOnly;
            openDlg.AddExtension = !initialAddExtension;
            openDlg.CheckPathExists = !initialCheckPathExists;
            openDlg.ShowReadOnly = !initialShowReadOnly;
            openDlg.Reset();

            if(openDlg.AddExtension == initialAddExtension)
            {
                DialogUtilities.LogPass("Reset properly reset AddExtension");
            }
            else
            {
                DialogUtilities.LogFail("Reset did not reset AddExtension");
            }

            if(openDlg.CheckPathExists == initialCheckPathExists)
            {
                DialogUtilities.LogPass("Reset properly reset CheckPathExists");
            }
            else
            {
                DialogUtilities.LogFail("Reset did not reset CheckPathExists");
            }

            if(openDlg.ShowReadOnly == initialShowReadOnly)
            {
                DialogUtilities.LogPass("Reset properly reset ShowReadOnly");
            }
            else
            {
                DialogUtilities.LogFail("Reset did not reset ShowReadOnly");
            }


            //Case 2: SaveFileDialog.Reset() resets expected properties
            initialAddExtension = saveDlg.AddExtension;
            initialCheckPathExists = saveDlg.CheckPathExists;
            initialCreatePrompt = saveDlg.CreatePrompt;
            initialOverwritePrompt = saveDlg.OverwritePrompt;
            saveDlg.AddExtension = !initialAddExtension;
            saveDlg.CheckPathExists = !initialCheckPathExists;
            saveDlg.CreatePrompt = !initialCreatePrompt;
            saveDlg.OverwritePrompt = !initialOverwritePrompt;
            saveDlg.Reset();

            if(saveDlg.AddExtension == initialAddExtension)
            {
                DialogUtilities.LogPass("Reset properly reset AddExtension");
            }
            else
            {
                DialogUtilities.LogFail("Reset did not reset AddExtension");
            }

            if(saveDlg.CheckPathExists == initialCheckPathExists)
            {
                DialogUtilities.LogPass("Reset properly reset CheckPathExists");
            }
            else
            {
                DialogUtilities.LogFail("Reset did not reset CheckPathExists");
            }

            if(saveDlg.CreatePrompt == initialCreatePrompt)
            {
                DialogUtilities.LogPass("Reset properly reset CreatePrompt");
            }
            else
            {
                DialogUtilities.LogFail("Reset did not reset CreatePrompt");
            }

            if(saveDlg.OverwritePrompt == initialOverwritePrompt)
            {
                DialogUtilities.LogPass("Reset properly reset OverwritePrompt");
            }
            else
            {
                DialogUtilities.LogFail("Reset did not reset OverwritePrompt");
            }

            Shutdown();
        }
    }
}
