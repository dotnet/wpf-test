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
    public class FileDialogCustomPlaceCases : Application
    {
        public FileDialogCustomPlaceCases()
        {
        }

        protected override void OnStartup (StartupEventArgs e)
        {
            FileDialogCustomPlace customPlace = null;
            try
            {
                //Case 1: Verify we can get back the file Path after setting it
                customPlace = new FileDialogCustomPlace("c:\\bogus");
                if (customPlace.Path == "c:\\bogus")
                {
                    Logging.SetPass("FileDialogCustomPlace was c:\\bogus, as expected.");
                }
                else
                {
                    Logging.SetFail("FileDialogCustomPlace was not c:\\bogus, as expected.  Instead it was: " + customPlace.Path);
                }

                //Case 2: Verify FileDialogCustomPlace created with string returns empty Guid
                if (customPlace.KnownFolder == Guid.Empty)
                {
                    Logging.SetPass("FileDialogCustomPlace.KnownFolder was Guid.Empty, as expected.");
                }
                else
                {
                    Logging.SetFail("FileDialogCustomPlace was not Guid.Empty, as expected.  Instead it was: " + customPlace.KnownFolder.ToString());
                }

                //Case 3: Verify we can get back the Guid after setting it
                customPlace = FileDialogCustomPlaces.Documents;
                if (customPlace.KnownFolder == new Guid("FDD39AD0-238F-46AF-ADB4-6C85480369C7"))
                {
                    Logging.SetPass("FileDialogCustomPlace.KnownFolder was FileDialogCustomPlaces.Documents, as expected.");
                }
                else
                {
                    Logging.SetFail("FileDialogCustomPlace.KnownFolder was not FileDialogCustomPlaces.Documents, as expected.  Instead it was: " + customPlace.KnownFolder.ToString());
                }

                //Case 4: Verify FileDialogCustomPlace created with Guid returns null string
                customPlace = FileDialogCustomPlaces.Documents;
                if (customPlace.Path == null)
                {
                    Logging.SetPass("FileDialogCustomPlace.Path was null, as expected.");
                }
                else
                {
                    Logging.SetFail("FileDialogCustomPlace.Path was not null, as expected.  Instead it was: " + customPlace.Path);
                }

                //Case 5: FileDialogCustomPlace.ToString()
                string customPlaceString = null;
                customPlaceString = customPlace.ToString();
                if (customPlaceString != null)
                {
                    Logging.SetPass("FileDialogCustomPlace.ToString() was non-null, as expected: " + customPlaceString);
                }
                else
                {
                    Logging.SetFail("FileDialogCustomPlace.ToString() unexpectedly returned null");
                }

                //Case 6: Instantiate all FileDialogCustomPlaces.
                OpenFileDialog openDialog = new OpenFileDialog();
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Contacts);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.RoamingApplicationData);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.LocalApplicationData);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Cookies);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Favorites);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Programs);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Music);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Pictures);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.SendTo);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.StartMenu);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Startup);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.System);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Templates);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Desktop);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.Documents);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.ProgramFiles);
                openDialog.CustomPlaces.Add(FileDialogCustomPlaces.ProgramFilesCommon);
                if (openDialog.CustomPlaces.Count == 17)
                {
                    Logging.LogPass("Every FileDialogCustomPlace was created.");
                }
                else
                {
                    Logging.LogFail("Count of FileDialogCustomPlaces should have been 17.  Instead it was: " + openDialog.CustomPlaces.Count);
                }
            }
            catch (Exception ex)
            {
                Logging.LogFail("Unexpected exception: " + ex.ToString());
            }

            Shutdown();
        }
    }
}
