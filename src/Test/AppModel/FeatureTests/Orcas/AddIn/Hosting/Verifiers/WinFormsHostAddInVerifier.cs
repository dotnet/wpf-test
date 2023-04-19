// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Input;
using System.Globalization;

namespace Microsoft.Test.AddIn
{
    public class WinFormsHostAddInVerifier : IVerifyAddIn
    {

        #region Private Members

        private TestLog _log;
        private HostWinFormsHostContractToViewAdapter _hostView;
        private Panel _parent;
        private AddInHost _addInHost;

        #endregion

        #region Constructor

        public WinFormsHostAddInVerifier()
        {
            _log = TestLog.Current;
            _hostView = null;
        }

        #endregion

        #region IVerfiyAddIn Members

        /// <summary>
        /// Prepares Verifier to verify the AddIn
        /// </summary>
        /// <param name="hostParameters">Copy of the AddIn parameters that were passed to the AddIn</param>
        /// <param name="parent">AddIn Host parent panel</param>
        public void Initialize(string addInParameters, Panel parent)
        {
            //In future look into the string to determine if AddIns are nested
            this._parent = parent;

            _addInHost.WaitForPriority(DispatcherPriority.Background);
        }

        /// <summary>
        /// Verifies the AddIn
        /// </summary>
        /// <param name="testAddIn">Reference to the HostView instance</param>
        /// <returns>Pass if the AddIn worked as expected
        /// Fail if it did not respond correctly
        /// Unknown if the Verifier can not verify the AddIn</returns>
        public TestResult VerifyTestAddIn(object hostView)
        {
            if (!CanVerify(hostView.GetType()))
            {
                _log.LogEvidence("Can not verify " + hostView.ToString() + " with " + this.ToString() + " as the verifier");
                return TestResult.Unknown;
            }
            else
            {
                this._hostView = (HostWinFormsHostContractToViewAdapter)hostView;
                if (this._hostView == null)
                {
                    _log.LogEvidence("Can not cast " + hostView.ToString() + " to HostWinFormsHostContractToViewAdapter");
                    return TestResult.Fail;
                }
                else
                {
                    return VerifyAddIn();
                }
            }
        }

        /// <summary>
        /// Indicates if the Verifier can verify a given AddIn
        /// </summary>
        /// <param name="addInType">Type of the Host View of the AddIn</param>
        /// <returns>true if the Verifier can verify the AddIn, false if not</returns>
        public bool CanVerify(Type hostViewType)
        {
            _log.LogStatus("Determining if " + this.ToString() + " can verify " + hostViewType.ToString());
            return hostViewType == typeof(HostWinFormsHostContractToViewAdapter);
        }

        /// <summary>
        /// Property to access the Panel that will host the AddIn UI
        /// </summary>
        public Panel AddInHostParent 
        {
            get{ return _parent; }
            set{ _parent = value; }
        }

        /// <summary>
        /// Reference to the AddInHost that this verifier is contained in
        /// </summary>
        public AddInHost AddInHost
        {
            get { return _addInHost; }
            set { _addInHost = value; }
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Verifies the AddIn
        /// </summary>
        /// <returns>Pass if the AddIn passes, Fail if it does not perform as expected</returns>
        private TestResult VerifyAddIn()
        {
            _addInHost.WaitForPriority(DispatcherPriority.Background);

            FrameworkElement fe = _addInHost.AddInHostElement;
            //Click on TextBox in WinFormsHost in Addin
            _addInHost.MoveMouseAndClick(fe);
            _log.LogStatus("clicked WindowsFormsHost textbox");

            _addInHost.WaitForPriority(DispatcherPriority.Background);
            System.Threading.Thread.Sleep(2000);

            Input.Input.SendUnicodeString("testing");
            _addInHost.WaitForPriority(DispatcherPriority.Background);
            System.Threading.Thread.Sleep(2000);

            //Verify text entry in WinForms textbox worked
            if (_hostView.GetTextBoxText() == "testing")
            {
                _log.LogEvidence("Using WindowsFormsHost textbox was successful");
            }
            else
            {
                _log.LogEvidence("Using WindowsFormsHost textbox failed");
                return TestResult.Fail;
            }

            _addInHost.WaitForPriority(DispatcherPriority.Background);

            return TestResult.Pass;
        }

        #endregion

    }
}
