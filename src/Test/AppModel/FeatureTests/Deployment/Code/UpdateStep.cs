// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Deployment;
using System.Resources;
using System.IO;
using System.Collections;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Loaders;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Deployment;
using System.Windows.Automation;
using UIA=System.Windows.Automation;
using System.Threading; using System.Windows.Threading;
using Microsoft.Win32;

namespace Microsoft.Test.Deployment {

    /// <summary>
    /// Loader Step that can be used to update an Avalon .application application type
    /// in the same manner that an end user would.
    /// </summary>
    
    public class UpdateStep : AppMaintenanceStep
    {

        #region private data
        FileHost _fileHost;  // UpdateStep expects to have a filehost defined, as it needs to share the host
                            // with whatever step it may follow.
        TestLog _testLog;
        ApplicationMonitor _appMonitor;
        UIHandler[] _uiHandlers = new UIHandler[0];

        const string SIDEBYSIDE_REGISTRY_KEY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\SideBySide";
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new UpdateStep
        /// </summary>
        public UpdateStep() 
        {

        }
        #endregion

        #region Public members


		/// <summary>
        /// Gets or sets the string of the first version to check for in the registry.
        /// </summary>
        public string PreUpdateVer = null;

        /// <summary>
        /// Gets or sets the string of the second version to check for in the registry.
        /// </summary>
        public string PostUpdateVer = null;

        /// <summary>
        /// Gets or sets the Scheme to when activating the application
        /// </summary>
        public ActivationScheme Scheme = ActivationScheme.Local;

        /// <summary>
        /// Gets or sets an array of SupportFiles that should be remotely hosted
        /// </summary>
        public SupportFile[] SupportFiles = new SupportFile[0];
        
        /// <summary>
        /// gets or sets an array of UIHandlers that will be registered with Activation Host
        /// </summary>
        public UIHandler[] UIHandlers {
            get { return _uiHandlers; }
            set {
                foreach (UIHandler handler in value)
                    handler.Step = this;
                _uiHandlers = value;
            }
        }

		/// <summary>
		/// Gets Method to use for activation (Always Launch with an updateStep,
		/// as it launches the app from the start menu.
		/// </summary>
		public ActivationMethod Method { get { return ActivationMethod.Launch; } }

		/// <summary>
        /// Gets the ApplicationMonitor that is monitoring the target application
        /// </summary>
        /// <value>the ApplicationMonitor that is monitoring the target application</value>
        public ApplicationMonitor Monitor {
            get { return _appMonitor; }
        }
        #endregion


        #region Step Implementation

        /// <summary>
        /// Performs the Activation step
        /// </summary>
        /// <returns>returns true if the rest of the steps should be executed, otherwise, false</returns>
        protected override bool BeginStep() 
        {
            // Get current Test Log
            this._testLog = TestLog.Current;
            if (this._testLog == null)
            {
                throw new InvalidOperationException("Update step must be created in the context of a test log");
            }

            //Create ApplicationMonitor
            _appMonitor = new ApplicationMonitor();

            // Walk up to parent FileHost (To test update we need a shared Filehost, so as to write to the same location)
            LoaderStep parent = this.ParentStep;

            while (parent != null)
            {
                if (parent.GetType() == typeof(Microsoft.Test.Loaders.Steps.FileHostStep))
                {
                    GlobalLog.LogDebug("Found a filehost in the parent FileHostStep to use");
                    this._fileHost = ((FileHostStep)parent).fileHost;
                    break;
                }
                // Failed to find it in the immediate parent: try til we hit null or the right one
                parent = parent.ParentStep;
            }
            if (parent == null) // Couldnt find a parent that was a FileHostStep
            {
                GlobalLog.LogDebug("Failed to find a filehost in the parent FileHostStep to use");
                throw new InvalidOperationException("UpdateStep must be used in the context of a FileHost");
            }

            bool isLocal = (Scheme == ActivationScheme.Local);

            FileHostUriScheme fhus = FileHostUriScheme.Local;
            switch (this.Scheme)
            {
                case ActivationScheme.HttpInternet:
                    fhus = FileHostUriScheme.HttpInternet;
                    break;
                case ActivationScheme.HttpIntranet:
                    fhus = FileHostUriScheme.HttpIntranet;
                    break;
                case ActivationScheme.HttpsInternet:
                    fhus = FileHostUriScheme.HttpsInternet;
                    break;
                case ActivationScheme.HttpsIntranet:
                    fhus = FileHostUriScheme.HttpsIntranet;
                    break;
                case ActivationScheme.Unc:
                    fhus = FileHostUriScheme.Unc;
                    break;
                case ActivationScheme.Local:
                    fhus = FileHostUriScheme.Local;
                    break;
            }

            //upload files to FileHost 
            if (SupportFiles.Length > 0)
            {
                foreach (SupportFile suppFile in SupportFiles)
                {
                    if (suppFile.IncludeDependencies)
                        _fileHost.UploadFileWithDependencies(suppFile.Name, isLocal);
                    else
                        _fileHost.UploadFile(suppFile.Name, isLocal);
                }
            }
            string[] stuffToSign = Directory.GetFiles(Directory.GetCurrentDirectory(), "*" + ApplicationDeploymentHelper.STANDALONE_APPLICATION_EXTENSION, SearchOption.AllDirectories);
            string currentDir = Directory.GetCurrentDirectory();
            foreach (string depMan in stuffToSign)
            {
                GlobalLog.LogDebug("Examining: " + depMan);
                string manifestToSign = depMan;
                if (manifestToSign.StartsWith(currentDir))
                {
                    manifestToSign = manifestToSign.Remove(0, currentDir.Length);
                }

                if (isLocal)
                {
                    // Use static version in ApplicationDeploymentHelper, since filehost may be null.
                    ApplicationDeploymentHelper.SignManifest(depMan, depMan);
                }
                else
                {
                    try
                    {
                        _fileHost.SyncManifest(Path.GetFileName(manifestToSign), fhus);
                    }
                    catch (FileNotFoundException)
                    {
                        // If it's not on the server, we don't want to sign it.
                        GlobalLog.LogDebug("Tried to sign " + manifestToSign + " but it didn't exist on the server.");
                    }
                }
            }

            // register UIHandlers
            foreach (UIHandler handler in UIHandlers) 
            {
                if (handler.NamedRegistration != null)
                    _appMonitor.RegisterUIHandler(handler, handler.NamedRegistration, handler.Notification);
                else
                    _appMonitor.RegisterUIHandler(handler, handler.ProcessName, handler.WindowTitle, handler.Notification);
            }

            // Register handler for dismissing update dialog
            // Get the localized text that will say "Update Available" in whatever language...
            ResourceManager resMan = new ResourceManager("System.Deployment", typeof(System.Deployment.Application.ApplicationDeployment).Assembly);

            _appMonitor.RegisterUIHandler(new DismissUpdateDialog(), "dfsvc", resMan.GetString("UI_UpdateTitle"), UIHandlerNotification.All);

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

			return true;
        }

        /// <summary>
        /// Waits for the ApplicationMonitor to Abort and Closes any remaining
        /// processes
        /// </summary>
        /// <returns>true</returns>
        protected override bool EndStep() 
        {
            _appMonitor.WaitForUIHandlerAbort();
            bool preUpdateStatus = ExistsPackageMetadataRegistryKey(this.PreUpdateVer);
            bool postUpdateStatus = ExistsPackageMetadataRegistryKey(this.PostUpdateVer);

            if (preUpdateStatus && postUpdateStatus)
            // Pass if we find registry entries for both versions and the correct shortcuts
            {
                GlobalLog.LogEvidence("Passed:  Registry entries for both versions exist");
                this._testLog.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Failed:  Registry entries for one or both versions not present");
                this._testLog.Result = TestResult.Fail;
            }
            
            //Wait for the application to be done
            _appMonitor.WaitForUIHandlerAbort();
            _appMonitor.Close();

            return true;
        }

        #endregion


    }


}
