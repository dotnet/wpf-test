// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

namespace Microsoft.Test.AddIn
{
    public class CountClickErrorCaseVerifier : IVerifyAddIn
    {

        #region Private Members

        private TestLog _log;
        private HostCountClicksContractToViewAdapter _hostView;
        private Panel _parent;
        private AddInHost _addInHost;

        #endregion

        #region Constructor

        public CountClickErrorCaseVerifier()
        {
            _log = TestLog.Current;
            _hostView = null;
        }

        #endregion

        #region IVerfiyAddIn Members

        /// <summary>
        /// Prepares Verifier to verify the AddIn
        /// </summary>
        /// <param name="addInParameters">Copy of the AddIn parameters that were passed to the AddIn</param>
        /// <param name="parent">AddIn Host parent panel</param>
        public void Initialize(string addInParameters, Panel parent)
        {
            //Do nothing at this time. In future look into the string
            this._parent = parent;
        }

        /// <summary>
        /// Verifies the AddIn
        /// </summary>
        /// <param name="hostView">Reference to the HostView instance</param>
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
                this._hostView = (HostCountClicksContractToViewAdapter)hostView;
                if (this._hostView == null)
                {
                    _log.LogEvidence("Can not cast " + hostView.ToString() + " to HostCountClicksAddInView");
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
        /// <param name="hostViewType">Type of the Host View of the AddIn</param>
        /// <returns>true if the Verifier can verify the AddIn, false if not</returns>
        public bool CanVerify(Type hostViewType)
        {
            _log.LogStatus("Determining if " + this.ToString() + " can verify " + hostViewType.ToString());
            return hostViewType == typeof(HostCountClicksContractToViewAdapter);
        }


        public Panel AddInHostParent 
        {
            get{ return _parent; }
            set{ _parent = value; }
        }

        public AddInHost AddInHost
        {
            get { return _addInHost; }
            set { _addInHost = value; }
        }

        #endregion

        #region Private Helper Methods

        private TestResult VerifyAddIn()
        {
            _log.LogStatus("Waiting for background");
            WaitForPriority(DispatcherPriority.Background);

            _log.LogStatus("Verifying calling ViewToContractAdapter(null) returns null");

            if (!VerifyViewToContractAdapterNull())
            {
                return TestResult.Fail;
            }
            

            if (!VerifyViewToContractAdapterChild())
            {
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        /// <summary>
        /// FrameworkElementAdapters.ViewToContractAdapter should return null if we hand in a null argument.  Verify that.
        /// </summary>
        private bool VerifyViewToContractAdapterNull()
        {
            if (null == _hostView.AdapterNull)
            {
                _log.LogEvidence("ViewToContractAdapter(null) returned null as expected");
                return true;
            }
            else
            {
                _log.LogEvidence("Expected return value was null, actual: " + _hostView.AdapterNull.ToString());
                return false;
            } 
        }

        /// <summary>
        /// FrameworkElementAdapters.ViewToContractAdapter should throw an InvalidOperationException if we hand in
        /// a child of the actual root element.  Verify that.
        /// </summary>
        private bool VerifyViewToContractAdapterChild()
        {
            try
            {
                object unused = _hostView.AdapterChild;
            }
            catch (InvalidOperationException exception)
            {
                _log.LogEvidence("Caught InvalidOperationException: " + exception.ToString());
                _log.LogStatus("ViewToContractAdapter(child) threw InvalidOperationException as expected");
                return true;
            }

            _log.LogEvidence("ViewToContractAdapter(child) did not throw expected InvalidOperationException - fail");
            return false;
        }


        /// This is from WPF Test. Should move to a base class or use helper function
        /// <summary>
        /// Waits for a specific Dispatcher Priority to occur
        /// </summary>
        /// <param name="priority">Dispatcher Priority to wait for</param>
        /// <returns>true if successful otherwise false when a timeout occurs</returns>
        private void WaitForPriority(DispatcherPriority priority)
        {
            DispatcherHelper.DoEvents(0, priority);
        }

        #endregion

    }
}
