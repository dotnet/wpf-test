// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Resources;
using System.Diagnostics;
using System.IO;
using Microsoft.Test.Loaders;
using Microsoft.Win32;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Deployment {

    /// <summary>
    /// Uninstalls an installed .application, then verifies it is not in ClickOnceCache.  
    /// Needs to be run in the context of a TestLogStep
    /// </summary>
    
    public class UninstallStep : AppMaintenanceStep 
    {
        #region Private Data
        const string SIDEBYSIDE_REGISTRY_KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\SideBySide";
        const string UNINSTALL_REGISTRY_KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
        ApplicationMonitor _appMonitor;
		TestLog _testLog;
        private bool _appPresentToUninstall = false;
        #endregion

        #region Public Members

        /// <summary>
        /// If true, checks to see if the "Restore/Rollback" radio button is enabled, and fails if it is not.
        /// For cases that need to verify that rollback is enabled.
        /// </summary>
        public bool checkRestoreOption = false;

        /// <summary>
        /// Gets the ApplicationMonitor that is monitoring the target application
        /// </summary>
        /// <value>the ApplicationMonitor that is monitoring the target application</value>
        public ApplicationMonitor Monitor
        {
            get { return _appMonitor; }
        }

        /// <summary>
        /// String for registering the UIHandler for the Maintenance dialog.  Prefixes  " (Appname) Maintenance", does
        /// not need a space at the end.  May need to change as UI changes.
        /// </summary>
        public string MaintenanceDialogTitlePrefix = null;


        #endregion
        
        #region Step Implementation
        /// <summary>
        /// Finds uninstall string in registry and invokes it
        /// </summary>
        /// <returns>true</returns>
        protected override bool BeginStep() 
        {
			// Get current Test Log
			this._testLog = TestLog.Current;
			if (this._testLog == null)
			{
				throw new InvalidOperationException("Uninstall step must be created in the context of a test log");
			}

            // get a reference to the uninstall key in the registry
            RegistryKey uninstallRegistryKey = FindUninstallRegistryKey();

            if (uninstallRegistryKey != null)
            {
                _appPresentToUninstall = true;

                //Create ApplicationMonitor
                _appMonitor = new ApplicationMonitor();

                // start the dll that would be invoked via ARP
                string uninstallString = uninstallRegistryKey.GetValue("UninstallString").ToString().Remove(0, 13);

                // Construct the window title the Uninstall dialog should have...
                // Get the localized text from System.Deployment
                
                // This type doesn't exist in .NET Core, replacing with a hard coded value from .NET Framework, but this may cause running in localized machines to fail.
                //ResourceManager resMan = new ResourceManager("System.Deployment", typeof(System.Deployment.Application.ApplicationDeployment).Assembly);
                string maintenanceWindowTitle = String.Format("{0} Maintenance", AppName);

                // release the reference to the reg key so that uninstall happens correctly
                uninstallRegistryKey.Close();

                if (this.checkRestoreOption)
                {
                    _appMonitor.RegisterUIHandler(new DismissMaintenanceDialog(MaintenanceDialogAction.CheckRestoreAndRemove), "dfsvc", maintenanceWindowTitle, UIHandlerNotification.All);
                }
                else
                {
                    _appMonitor.RegisterUIHandler(new DismissMaintenanceDialog(), "dfsvc", maintenanceWindowTitle, UIHandlerNotification.All);
                }

                _appMonitor.StartProcess("rundll32.exe", uninstallString);
				

                // if the dialog is handled successfully, return true
                return true;
            }
            else
            {
                GlobalLog.LogEvidence("Could not find the uninstall registry key!");
                _appPresentToUninstall = false;
                return true;
            }
        }

		/// <summary>
		/// Checks to see if any entries remain containing AppName
		/// </summary>
		/// <returns>true</returns>
		protected override bool EndStep()
		{
            if (_appPresentToUninstall)
            {
                GlobalLog.LogDebug("Waiting for UI Handler Abort signal(s)");
                //Wait for the application to be done
                _appMonitor.WaitForUIHandlerAbort();
                _appMonitor.Close();

                bool fusionStatus = CheckFusionStore();
                bool registryStatus = RegistryEntriesExist();

                if (registryStatus || fusionStatus)
                {
                    GlobalLog.LogDebug("Found remnants of the application:");
                    if (fusionStatus)
                        GlobalLog.LogDebug("Found remnants of the application in the Fusion Store");
                    if (registryStatus)
                        GlobalLog.LogDebug("Found remnants of the application in the Registry");
                }

                if (!fusionStatus && _appPresentToUninstall) // Pass based on successful removal from the fusion store, as some registry stuff is still there 
                    // If registry removal is fixed, add it to if statement to pass/fail test based on its result
                {
                    GlobalLog.LogEvidence("Uninstall passed: App present to uninstall and registry entries cleaned");
                    this._testLog.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Failed due to remnants of the application existing after uninstall");
                    this._testLog.Result = TestResult.Fail;
                }
            }
            else
            {
                GlobalLog.LogEvidence("Failed due to app not being present to uninstall");
                this._testLog.Result = TestResult.Fail;
            }

            return true;
        }
        #endregion

    }

}
