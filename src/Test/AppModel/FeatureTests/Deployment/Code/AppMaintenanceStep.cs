// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Automation;
using System.Threading; using System.Windows.Threading;
//using UIA = System.Windows.Automation;
using Microsoft.Win32;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;
using MTI = Microsoft.Test.Input;

namespace Microsoft.Test.Deployment {

    /// <summary>
    /// Base loaderstep for steps that do app maintenance: (i.e. rollback, update, uninstall, etc)
    /// Contains convenient methods for:
    ///  -Navigating through the start menu
    ///  -Finding if an app has entries in the fusion cache
    ///  -Finding the uninstall reg. key of the app
    /// 
    /// </summary>
    
    public class AppMaintenanceStep : LoaderStep 
    {
        #region private data
        const string SIDEBYSIDE_REGISTRY_KEY_PATH = @"Software\Classes\Software\Microsoft\Windows\CurrentVersion\Deployment\SideBySide\2.0";
        const string UNINSTALL_REGISTRY_KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

        // some ClickOnce paths use a AppName.Substring(0,15)
        int longAppNameLength
        {
            get {
                if (AppName.Length >= 16)
                {
                    return 15;
                }
                else
                {
                    return AppName.Length;
                }
            }
        }

        // other ClickOnce paths use AppName.Substring(0,10)
        int shortAppNameLength
        {
            get {
                if (AppName.Length >= 11)
                {
                    return 10;
                }
                else
                {
                    return AppName.Length;
                }
            }
        }

        #endregion

        #region public members
        /// <summary>
        /// Gets or sets the Name of the App to uninstall
        /// </summary>
        public string AppName = null;
        #endregion
        
        #region privateMethods

        internal bool tryStartMenuNavigate(params string[] items)
        {
            foreach (string menuItem in items)
            {
                if (tryStartMenuClick(menuItem) == false)
                    return false;
            }
            return true;
        }

        internal bool tryStartMenuClick(string menuItemName)
        {
            // Will throw a noClickablePointException if start menu not currently invoked.
            bool succeeded = false;
            System.Windows.Automation.Condition itemInvokeCondition =
                new AndCondition(new PropertyCondition(AutomationElement.NameProperty, menuItemName),
                                 new PropertyCondition(AutomationElement.IsInvokePatternAvailableProperty, true));

            AutomationElement itemToClick = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, itemInvokeCondition);

            if (itemToClick != null)
            {
                MTI.Input.MoveToAndClick(itemToClick);
                succeeded = true;
            }
            Thread.Sleep(750);
            return succeeded;
        }

        internal bool invokeStartMenu()
        {
            System.Windows.Automation.Condition startBtnCondition = new AndCondition(new PropertyCondition(AutomationElement.NameProperty, "Start"),
                                        new PropertyCondition(AutomationElement.IsInvokePatternAvailableProperty, true));
            AutomationElement startMenu = AutomationElement.RootElement.FindFirst(TreeScope.Descendants, startBtnCondition);
            if (startMenu != null)
            {
                object patternObject;
                startMenu.TryGetCurrentPattern(InvokePattern.Pattern, out patternObject);
                InvokePattern invokePattern = patternObject as InvokePattern;
                invokePattern.Invoke();
                Thread.Sleep(750);
                return true;
            }
            return false;
        }

        internal RegistryKey FindUninstallRegistryKey()
        {
            RegistryKey uninstallKey = Registry.CurrentUser.OpenSubKey(UNINSTALL_REGISTRY_KEY_PATH);

            if (uninstallKey == null)
            {
                GlobalLog.LogDebug("Registry Key: " + UNINSTALL_REGISTRY_KEY_PATH + " did not exist... returning ");
                return null;
            }

            // get a list of all key names  
            string[] registryKeyNames = uninstallKey.GetSubKeyNames();

            // find the one corresponding to the test app
            foreach (string name in registryKeyNames)
            {
                // get the key's subkey and it's name
                RegistryKey openedSubKey = uninstallKey.OpenSubKey(name);
                if (openedSubKey == null)
                {
                    GlobalLog.LogDebug("Subkey " + name + " did not exist... returning");
                    return null;
                }
                else
                {
                    GlobalLog.LogEvidence("Found subkey " + name);
                    string displayName = openedSubKey.GetValue("DisplayName").ToString();
                    if (displayName.ToLowerInvariant().Contains(this.AppName.ToLowerInvariant()))
                    {
                        GlobalLog.LogEvidence("Found uninstall key...");
                        return openedSubKey;
                    }
                }                  
            }
            // if no key was found, return null
            GlobalLog.LogEvidence("Could not find key containing " + this.AppName);
            return null;
        }

        internal bool ExistsFusionStoreEntries(string versionString)
        {
            string FusionCachePath = ApplicationDeploymentHelper.FusionCachePath;
            if (FusionCachePath == null)
            {
                return false;
            }
            string[] fusionContents = Directory.GetDirectories(FusionCachePath);
            foreach (string dirName in fusionContents)
            {
                if ((dirName.ToLower().Contains(this.AppName.ToLower().Substring(0, shortAppNameLength))) && (dirName.ToLower().Contains(versionString)))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool ExistsPackageMetadataRegistryKey(string versionString)
        {
            // get a reference to the PackageMetadata key
            RegistryKey packageMetadata = Registry.CurrentUser.OpenSubKey(SIDEBYSIDE_REGISTRY_KEY_PATH + @"\PackageMetadata");
            if (packageMetadata == null)
            {
                GlobalLog.LogDebug("Registry Key: " + SIDEBYSIDE_REGISTRY_KEY_PATH + @"\PackageMetadata" + " did not exist... returning ");
                return false;
            }
            // get a list of all key names  (Dont need to check for null here: packageMetadata at least is not null
            string[] registryKeyNames = packageMetadata.GetSubKeyNames();

            // find the one corresponding to the test app
            foreach (string name in registryKeyNames)
            {
                // get the key's subkey and it's name
                RegistryKey openedSubKey = packageMetadata.OpenSubKey(name);
                if (openedSubKey == null)
                {
                    GlobalLog.LogDebug("Subkey " + name + " did not exist... returning");
                    return false;
                }
                string[] appKeyNames = openedSubKey.GetSubKeyNames();

                foreach (string appKeyName in appKeyNames)
                {
                    RegistryKey currentKey = packageMetadata.OpenSubKey(name).OpenSubKey(appKeyName);
                    string keyName = currentKey.Name;
                    // if the name matches the app and is not the version dependent key
                    if (keyName.Contains(GetAppShortName(this.AppName)) && keyName.Contains(versionString))
                    {
                        GlobalLog.LogEvidence("Found registry entry for " + AppName + " Version " + versionString);
                        openedSubKey.Close();
                        currentKey.Close();
                        packageMetadata.Close();
                        return true;
                    }
                }
            }
            // if no matching key was found, return false
            GlobalLog.LogEvidence("Failed to find registry entry for " + AppName + " Version " + versionString);
            return false;
        }

        internal string GetAppShortName(string name)
        {
            if (name.Length <= 10)
            {
                return name.ToLowerInvariant();
            }
            else
            {
                return name.ToLowerInvariant().Substring(0, 4) + ".." + name.ToLowerInvariant().Substring(name.Length - 4);
            }
        }

        internal bool CheckFusionStore()
        {
            // not implemented - waiting until Whidbey Beta 1 or later
            // API to be provided by ClickOnce team

            string FusionCachePath = ApplicationDeploymentHelper.FusionCachePath;
            if (FusionCachePath == null)
            {
                return false;
            }
            string[] fusionContents = Directory.GetDirectories(FusionCachePath);
            foreach (string dirName in fusionContents)
            {
                if (dirName.ToLower().Contains(this.AppName.ToLower().Substring(0, longAppNameLength)))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool RegistryEntriesExist()
        {
            // get a reference to the PackageMetadata key
            RegistryKey packageMetadata = Registry.CurrentUser.OpenSubKey(SIDEBYSIDE_REGISTRY_KEY_PATH + @"\PackageMetadata");

            if (packageMetadata == null)
            {
                return false; // Entire section not in registry, thus this apps keys are not as well
            }

            // get a list of all key names
            string[] registryKeyNames = packageMetadata.GetSubKeyNames();

            // find the one corresponding to the test app
            foreach (string name in registryKeyNames)
            {
                // get the key's subkey and it's name
                string[] appKeyNames = packageMetadata.OpenSubKey(name).GetSubKeyNames();

                foreach (string appKeyName in appKeyNames)
                {
                    RegistryKey currentKey = packageMetadata.OpenSubKey(name).OpenSubKey(appKeyName);
                    string keyName = currentKey.Name;

                    // if the name matches the app 
                    if (keyName.Contains(this.AppName.ToLower().Substring(0, longAppNameLength)))
                    {
                        return true;
                    }
                }
            }
            // if no key was found, return false
            return false;
        }

        #endregion
    }

}
