// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Security.Policy;
using Microsoft.Test.Logging;
using Microsoft.Win32;


/*****************************************************
 * The logic in this file is maintained by the Application Model team
 *****************************************************/

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// Class containing code taken from ClickOnce team for reseting ClickOnce store state.
    /// </summary>
    public static class ClickOnceHelper
    {

        #region private data

        const string STORE_FOLDER_NAME_XP = @"Apps\2.0";
        const string STORE_FOLDER_NAME_VISTA = @"Local\Apps\2.0";
        const string STATE_MANAGER_SUBKEY_NAME = @"Software\Microsoft\Windows\CurrentVersion\StateManager";
        const string SIDEBYSIDE_REGISTRY_KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\SideBySide";
        const string DEPLOYMENT_REGISTRY_KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Deployment";
        const int CLICKONCE_STORE_FORMAT_VERSION = 4;

        #endregion


        #region public members

        /// <summary>
        /// Cleans the ClickOnceCache so ClickOnce deployments will always be installed fresh
        /// </summary>
        public static void CleanClickOnceCache()
        {
            //Shutdown the Deployment Framework service
            foreach (Process dproc in Process.GetProcessesByName("dfsvc"))
            {
                dproc.Kill();
                dproc.WaitForExit();
            }
            // Clear application trust cache
            PurgeAppTrustCacheEntries();
            // Clean up the store state
            ResetStoreState();
            // Delete shortcuts of installed apps
            DeleteShortcuts();
            // Set Key representing deployment format version
            // this is a work around for the ClickOnce dialog that pops up on the first run of dfsvc on a machine
            SetFormatVersion();
        }

        #endregion


        #region Private implementation

        static void PurgeAppTrustCacheEntries()
        {
            //Clear the User Application cache of trust decisions that have been made
            //by clearing the User Application cache
            try
            {
                ApplicationSecurityManager.UserApplicationTrusts.Clear();
            }
            catch (Exception exp)
            {
                logDebug("WARNING: Exception while cleaning up trust entries - " + exp.Message + exp.StackTrace);
                logDebug("This is not neccesarily a failure");
            }
        }

        static void SetFormatVersion()
        {
            try
            {
                // Delete the key to ensure no mis-ACL'ing...
                Registry.CurrentUser.DeleteSubKeyTree("Software\\Classes\\" + DEPLOYMENT_REGISTRY_KEY_PATH);
            }
            catch
            {   // Only happens if the key is missing, but fail politely if so...
                logDebug("Hit exception trying to delete HKCU\\" + "Software\\Classes\\" + DEPLOYMENT_REGISTRY_KEY_PATH);
            }
            RegistryKey deployKey =
                Registry.CurrentUser.CreateSubKey("Software\\Classes\\" + DEPLOYMENT_REGISTRY_KEY_PATH, RegistryKeyPermissionCheck.ReadWriteSubTree);

            deployKey.SetValue("FormatVersion", CLICKONCE_STORE_FORMAT_VERSION);
            deployKey.Close();
        }

        static void DeleteShortcuts()
        {
            string appShortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);

            // For XP cases.  The env. var we were using no longer applies to shortcut storage in avalon.
            if (!Directory.Exists(appShortcutPath))
            {
                logDebug(appShortcutPath + " did not exist... ");
                logDebug("Failed to delete the shortcuts from previous shell-visible apps.  Directory " + appShortcutPath + " did not exist...");
                logDebug("Please contact Microsoft to fix this issue.");
                return;
            }

            foreach (string directory in Directory.GetDirectories(appShortcutPath))
            {
                // if the directory contains an appref-ms file
                if (Directory.GetFiles(directory, "*.appref-ms").Length > 0)
                {
                    // delete the directory
                    Directory.Delete(directory, true);
                }
            }
        }

        private static void ResetStoreState()
        {
            //          
            // ---- store folders and registry entries.
            //

            RegistryKey storeRegKey = null;
            RegistryKey stateManagerRegKey = null;
            string[] regValues = null;
            string localStorePath;

            try
            {
                storeRegKey = Registry.CurrentUser.CreateSubKey(SIDEBYSIDE_REGISTRY_KEY_PATH);
                stateManagerRegKey = Registry.CurrentUser.CreateSubKey(STATE_MANAGER_SUBKEY_NAME);

                // Delete all store registry entries.
                regValues = storeRegKey.GetValueNames();
                if (regValues.Length > 0)
                {
                    foreach (string regValue in regValues)
                    {
                        storeRegKey.DeleteValue(regValue);
                    }
                }
                Registry.CurrentUser.DeleteSubKeyTree(SIDEBYSIDE_REGISTRY_KEY_PATH);
                Registry.CurrentUser.DeleteSubKeyTree(STATE_MANAGER_SUBKEY_NAME);
            }
            catch (Exception exp)
            {
                // Do nothing...
                logDebug("WARNING: Exception while deleting registry keys: " + exp.Message);
            }

            // Get the store folder name and delete the folder with all its contents.
            try
            {
                localStorePath = Path.GetDirectoryName(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
                string possibleLocalStorePath = Path.Combine(localStorePath, STORE_FOLDER_NAME_XP);
                if (Directory.Exists(possibleLocalStorePath))
                {
                    localStorePath = possibleLocalStorePath;
                }
                else
                {
                    localStorePath = Path.Combine(localStorePath, STORE_FOLDER_NAME_VISTA);
                }

                if (Directory.Exists(localStorePath))
                {
                    Directory.Delete(localStorePath, true);
                }
                else
                {
                    logDebug("WARNING!!!!! \nDIRECTORY ( " + localStorePath + " )DID NOT EXIST. \nThe app store was not cleaned and this can cause strange clickonce app failures. \nPlease contact Microsoft for information on fixing this error ASAP!");
                }
            }
            catch (Exception exp)
            {
                logDebug("WARNING: Exception while trying to delete Store folder - " + exp.Message);
            }
        }

        private static void logDebug(string debugOutput)
        {
            GlobalLog.LogDebug(debugOutput);
        }

        #endregion
    }
}
