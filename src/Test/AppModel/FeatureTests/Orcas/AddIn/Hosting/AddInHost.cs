// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Security;
using Microsoft.Test.Logging;
using System.Windows;
using System.AddIn.Hosting;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Security.Wrappers;
using Microsoft.Test.Threading;
using Microsoft.Test.Utilities;


namespace Microsoft.Test.AddIn
{
    [Serializable]
    public class AddInHost
    {

        #region Private Members

        //Settable properties
        private string _addInParameters;
        private Type _addInType;
        private Type _verifierType;
        private AddInSecurityLevel _addInSecurityLevel;
        private PermissionSet _permissionSet;
        private Panel _parent;
        private PipelineStoreLocation _pipelineStoreLocation;
        private string _relativePipelinePath;
        private string _relativeAddInPath;
        private bool _useFullPath;
        private bool _useEnum;
        
        //Derived properties
        private string _hostPath;

        //Properties that are instantiated by other actions
        private IVerifyAddIn _verifier;
        private object _hostView;
        private FrameworkElement _addInHostElement;


        #endregion

        #region Constructor

        public AddInHost()
        {
            _useEnum = false;
            _useFullPath = false;
            _hostPath = PathSW.GetFullPath(AssemblySW.GetAssembly(typeof(AddInHost)).Location);
            _hostPath = PathSW.GetDirectoryName(_hostPath);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set properties and prepare the AddInHost to host an AddIn.
        /// </summary>
        public void Initialize()
        {
            this._verifier = (IVerifyAddIn)Activator.CreateInstance(_verifierType);
        }

        /// <summary>
        /// Activates the AddIn using token.Activate 
        /// </summary>
        /// <returns>FrameworkElement of the AddIn to add to Host UI Tree</returns>
        public FrameworkElement ActivateAddIn()
        {
            Collection<AddInToken> tokens;

            //Build pipeline store
            if (_useEnum)
            {
                AddInStore.Rebuild(_pipelineStoreLocation);
                tokens = AddInStore.FindAddIns(_addInType, _pipelineStoreLocation);
            }
            else
            {
                if (_useFullPath)
                {
                    AddInStore.Rebuild(PipelinePath);
                    AddInStore.UpdateAddIns(AddInPath);
                    tokens = AddInStore.FindAddIns(_addInType, PipelinePath, AddInPath);
                }
                else
                {
                    AddInStore.Rebuild(_relativePipelinePath);
                    AddInStore.UpdateAddIns(_relativeAddInPath);
                    tokens = AddInStore.FindAddIns(_addInType, _relativePipelinePath, _relativeAddInPath);
                }
            }
            
            //Which token to activate?
            if (tokens.Count < 1)
            {
                throw new ArgumentException("Could not find any AddIns of type: " + this._addInType.ToString());
            }

            //Activate the first one - Need to have a better way to choose a token
            if (_permissionSet != null)
            {
                _hostView = tokens[0].Activate<object>(_permissionSet);
            }
            else
            {
                //Activate the selected AddInToken in a new AppDomain set sandboxed in the internet zone
                _hostView = tokens[0].Activate<object>(_addInSecurityLevel);
            }

            if (_hostView is FrameworkElement)
            {
                _addInHostElement = (FrameworkElement)_hostView;
            }
            else if (_hostView is HostViewBase)
            {
                _addInHostElement = ((HostViewBase)_hostView).GetAddInUserInterface();
            }
            else
            {
                throw new ArgumentException("Can not get UI from the AddInView");
            }
            return _addInHostElement;
        }

        /// <summary>
        /// Shuts down the AddIn. Sets the AddInHostElement to null. Sets the HostView to null.
        /// </summary>
        public void ShutDownAddIn()
        {
            _addInHostElement = null;
            AddInController.GetAddInController(_hostView).Shutdown();
            _hostView = null;
        }

        /// <summary>
        /// Pass test parameters to the Verifier, prepares Verifier to verify the AddIn
        /// </summary>
        /// <param name="addInParameters">String of the AddIn Parameters</param>
        public void InitializeVerifier()
        {
            _verifier.AddInHost = this;
            _verifier.Initialize(_addInParameters, _parent);
        }

        /// <summary>
        /// Pass test parameters to the AddIn. Prepares AddIn to be used
        /// </summary>
        /// <param name="addInParameters">String of the AddIn Parameters</param>
        public void InitializeAddIn()
        {
            ((IHostView)_hostView).Initialize(_addInParameters);
        }

        /// <summary>
        /// Verifies the AddIn's behavior
        /// </summary>
        /// <returns>Pass if the AddIn worked as expected
        /// Fail if it did not respond correctly</returns>
        public TestResult VerifyAddIn()
        {
            return _verifier.VerifyTestAddIn(_hostView);
        }

        public void ParentTheAddInUI()
        {
            _parent.Children.Add(_addInHostElement);
            if (_verifier != null)
            {
                _verifier.AddInHostParent = _parent;
            }
        }

        /// <summary>
        /// Move the mouse cursor over the specified FrameworkElement and press the left click
        /// </summary>
        /// <param name="element"></param>
        public void MoveMouseAndClick(FrameworkElement element)
        {
            TestLog log = TestLog.Current;
            log.LogStatus("Determining point to move to based on " + element.Name);
            Rectangle rect = ImageUtility.GetScreenBoundingRectangle(element);
            log.LogStatus(rect.ToString());
            System.Windows.Point point = new System.Windows.Point(rect.X + 10, rect.Y + 10);
            log.LogStatus("Moving mouse and clicking on point x=" + point.X.ToString() + " y=" + point.Y.ToString());
            Input.Input.MoveToAndClick(point);
            WaitForPriority(DispatcherPriority.Background);
            log.LogStatus("Move and click complete");
        }

        /// <summary>
        /// Waits for a specific Dispatcher Priority to occur
        /// </summary>
        /// <param name="priority">Dispatcher Priority to wait for</param>
        /// <returns>true if sucessful otherwise false when a timeout occurs</returns>
        public void WaitForPriority(DispatcherPriority priority)
        {
            DispatcherHelper.DoEvents(0, priority);
        }

        #endregion Methods

        #region Public Properties

        /// <summary>
        /// Relative path from the executing assembly to the Pipeline
        /// </summary>
        public string RelativePipelinePath
        {
            get { return _relativePipelinePath; }
            set { _relativePipelinePath = value; }
        }

        /// <summary>
        /// Relative path from the executing assembly to the AddIn
        /// </summary>
        public string RelativeAddInPath
        {
            get { return _relativeAddInPath; }
            set { _relativeAddInPath = value; }
        }

        /// <summary>
        /// Enum value for the Pipeline location
        /// </summary>
        public PipelineStoreLocation PipelineStoreLocation
        {
            get { return _pipelineStoreLocation; }
            set { _pipelineStoreLocation = value; }
        }

        /// <summary>
        /// Determine if the PipelineStoreLocation should be used for 
        /// the Pipeline location or if the Pipeline path should be used
        /// </summary>
        public bool UseEnum
        {
            get { return _useEnum; }
            set { _useEnum = value; }
        }

        /// <summary>
        /// Determines if the relative path should be converted into a full path, 
        /// or if the AddInStore should use the relative path alone.
        /// </summary>
        public bool UseFullPath
        {
            get { return _useFullPath; }
            set { _useFullPath = value; }
        }

        /// <summary>
        /// Type of AddIn to activate
        /// </summary>
        public Type AddInType
        {
            get { return _addInType; }
            set { _addInType = value; }
        }

        /// <summary>
        /// Type of verifier to use to verify the AddIn
        /// </summary>
        public Type VerifierType
        {
            get { return _verifierType; }
            set { _verifierType = value; }
        }

        /// <summary>
        /// AddInSecurityLevel to activate the AddIn with
        /// if PermissionSet is set, PermissionSet is used
        /// </summary>
        public AddInSecurityLevel AddInSecurityLevel
        {
            get { return _addInSecurityLevel; }
            set { _addInSecurityLevel = value; }
        }

        /// <summary>
        /// PermissionSet activate the AddIn with
        /// if null, the AddInSecurityLevel must be set
        /// if this is set, PermissionSet is used
        /// </summary>
        public PermissionSet PermissionSet
        {
            get { return _permissionSet; }
            set { _permissionSet = value; }
        }

        /// <summary>
        /// Parameters passed to the AddIn during Initialization
        /// </summary>
        public string AddInParameters
        {
            get { return _addInParameters; }
            set { _addInParameters = value; }
        }

        
        /// <summary>
        /// The instance of the Verifier
        /// </summary>
        public IVerifyAddIn Verifier
        {
            get { return _verifier; }
        }

        /// <summary>
        /// Host's view of the AddIn, which is an instance of the Adapter
        /// </summary>
        public object HostView
        {
            get { return _hostView; }
        }

        /// <summary>
        /// The FrameworkElement from Activation
        /// This will be null until Activate is called
        /// </summary>
        public FrameworkElement AddInHostElement
        {
            get { return _addInHostElement; }
        }

        /// <summary>
        /// Assembly path of the AddIn
        /// </summary>
        public string AddInPath
        {
            get
            {
                return MergePath(_hostPath, _relativeAddInPath);
            }
        }

        /// <summary>
        /// Path of the Pipeline
        /// </summary>
        public string PipelinePath
        {
            get
            {
                return MergePath(_hostPath, _relativePipelinePath);
            }
        }

        /// <summary>
        /// Panel that is the parent of the AddIn's UI
        /// </summary>
        public Panel Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }


        #endregion

        #region Private Methods

        private string MergePath(string hostPath, string relativePath)
        {
            if (Path.IsPathRooted(relativePath))
            {
                return relativePath;
            }
            else
            {
                return Path.Combine(hostPath, relativePath);
            }
        }

        #endregion

    }
}
