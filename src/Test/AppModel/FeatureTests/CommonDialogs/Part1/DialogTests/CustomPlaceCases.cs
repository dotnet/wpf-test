// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Microsoft.Test.Logging;

namespace Microsoft.Test.WPF.AppModel.CommonDialogs
{
    public class CustomPlaceCases : Application
    {
        public CustomPlaceCases()
        {
        }

        protected override void OnStartup (StartupEventArgs e)
        {
            OpenFileDialog openDlg = new OpenFileDialog();

            //Case 1: Verify CustomPlaces is initially empty.
            if (openDlg.CustomPlaces.Count == 0)
            {
                Logging.SetPass("OpenFileDialog.CustomPlaces was initally empty, as expected.");
            }
            else
            {
                Logging.SetFail("OpenFileDialog.CustomPlaces was not empty as expected.  Instead, it has " + openDlg.CustomPlaces.Count + " items.");
            }

            //Case 2: Add a CustomPlace and verify it's there.
            openDlg.CustomPlaces.Add(new FileDialogCustomPlace("c:\\bogus"));
            if (openDlg.CustomPlaces.Count == 1)
            {
                Logging.LogPass("OpenFileDialog.CustomPlaces contains 1 item, as expected.");
            }
            else
            {
                Logging.LogFail("OpenFileDialog.CustomPlaces did not have 1 item as expected.  Instead, it has " + openDlg.CustomPlaces.Count + " items.");
            }

            Shutdown();
        }
    }
}
