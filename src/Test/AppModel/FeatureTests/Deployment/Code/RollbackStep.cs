// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Automation;
using System.Threading; 
using System.Windows.Threading;
using System.Resources;
using Microsoft.Win32;
using Microsoft.Test.Loaders;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Deployment {

    /// <summary>
    /// Rolls back an installed .application, then verifies it is not in ClickOnceCache.  
    /// Needs to be run in the context of a TestLogStep
    /// </summary>cd
    
    public class RollbackStep : AppMaintenanceStep
    {
        #region private data
        ApplicationMonitor _appMonitor;
		TestLog _testLog;
        UIHandler[] _uiHandlers = new UIHandler[0];
        private bool _appPresentToRollback = false;

        #endregion

        #region public members
        /// <summary>
        /// Gets or sets a string representing the version of the App to uninstall
        /// </summary>
        public string RemovedVersion = null;

        /// <summary>
        /// gets or sets an array of UIHandlers that will be registered with Activation Host
        /// </summary>
        public UIHandler[] UIHandlers
        {
            get { return _uiHandlers; }
            set
            {
                foreach (UIHandler handler in value)
                    handler.Step = this;
                _uiHandlers = value;
            }
        }

        /// <summary>
        /// Gets the ApplicationMonitor that is monitoring the target application
        /// </summary>
        /// <value>the ApplicationMonitor that is monitoring the target application</value>
        public ApplicationMonitor Monitor
        {
            get { return _appMonitor; }
        }
        #endregion

        #region Step Implementation
        /// <summary>
        /// Finds uninstall string in registry and invokes it, using a MaintenanceDialog handler to click "Restore\Rollback"
        /// </summary>
        /// <returns>true</returns>
        protected override bool BeginStep() 
        {
			// Get current Test Log
			this._testLog = TestLog.Current;
			if (this._testLog == null)
			{
				throw new InvalidOperationException("Rollback step must be created in the context of a test log");
			}
			//Create ApplicationMonitor
            _appMonitor = new ApplicationMonitor();

            // get a reference to the uninstall key in the registry
            RegistryKey uninstallRegistryKey = FindUninstallRegistryKey();

            if (uninstallRegistryKey != null)
            {
                _appPresentToRollback = true;

                // start the dll that would be invoked via ARP
                string uninstallString = uninstallRegistryKey.GetValue("UninstallString").ToString().Remove(0, 13);

                // release the reference to the reg key so that uninstall happens correctly
                uninstallRegistryKey.Close();

                // Construct the window title the Uninstall dialog should have...
                // Get the localized text from System.Deployment
                // This type doesn't exist in .NET Core, replacing with a hard coded value from .NET Framework, but this may cause running in localized machines to fail.
                //ResourceManager resMan = new ResourceManager("System.Deployment", typeof(System.Deployment.Application.ApplicationDeployment).Assembly);
                string maintenanceWindowTitle = String.Format("{0} Maintenance", AppName);
                string rollBackCompletedWindowTitle = "Application Restored";
                
                _appMonitor.RegisterUIHandler(new DismissMaintenanceDialog(MaintenanceDialogAction.Restore), "dfsvc", maintenanceWindowTitle, UIHandlerNotification.All);
                _appMonitor.RegisterUIHandler(new DismissMaintenanceDialog(MaintenanceDialogAction.RestoreOK), "dfsvc", rollBackCompletedWindowTitle, UIHandlerNotification.All);

                _appMonitor.StartProcess("rundll32.exe", uninstallString);

                GlobalLog.LogDebug("Waiting for UI Handler Abort signal(s)");
                //Wait for the application to be done
                _appMonitor.WaitForUIHandlerAbort();
                _appMonitor.Close();

                return true;
            }
            else
            {
                GlobalLog.LogDebug("Could not find the uninstall registry key!");
                _appPresentToRollback = false;
                return false;
            }
        }

		/// <summary>
		/// Checks to see if any entries remain containing AppName
		/// </summary>
		/// <returns>true</returns>
		protected override bool EndStep()
		{
            //Create ApplicationMonitor for monitoring v1.0 of app
            _appMonitor = new ApplicationMonitor();

            // register UIHandlers (for app type being verified)
            foreach (UIHandler handler in UIHandlers)
            {
                if (handler.NamedRegistration != null)
                    _appMonitor.RegisterUIHandler(handler, handler.NamedRegistration, handler.Notification);
                else
                    _appMonitor.RegisterUIHandler(handler, handler.ProcessName, handler.WindowTitle, handler.Notification);
            }


            if (_appPresentToRollback) // Pass based on successful removal from the fusion store, as some registry stuff is still there 
                // If registry removal is fixed, add it to if statement to pass/fail test based on its result
            {
                GlobalLog.LogEvidence("Passed: App present to uninstall and removed from registry properly.");
                this._testLog.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failed: App not present to uninstall or remnants in registry.");
                this._testLog.Result = TestResult.Fail;
                return false; // Don't run the app if the old app is still there... test has already failed.
            }

            string appShortcutPath = ApplicationDeploymentHelper.GetAppShortcutPath(AppName);

            GlobalLog.LogDebug("Launching appmonitor with shortcut path:");
            GlobalLog.LogDebug(appShortcutPath);
            try
            {
                _appMonitor.StartProcess(appShortcutPath);
            }
            catch (System.ComponentModel.Win32Exception)
            {
                GlobalLog.LogEvidence("Application shortcut not present to relaunch - failed to install properly");
                this._testLog.Result = TestResult.Fail;
                return false;
            }

            GlobalLog.LogDebug("Waiting for UI Handler Abort signal(s)");
			//Wait for the application to be done
			_appMonitor.WaitForUIHandlerAbort();
			_appMonitor.Close();

            return true;
        }
        #endregion

        
    }

}
