// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;
using Microsoft.Test.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.RenderingVerification;
using System.Drawing;

namespace Microsoft.Test.AddIn
{
    public class CountClickAddInVerifier : IVerifyAddIn
    {

        #region Private Members

        private TestLog _log;
        private HostCountClicksContractToViewAdapter _hostView;
        private Panel _parent;
        private AddInHost _addInHost;

        #endregion

        #region Constructor

        public CountClickAddInVerifier()
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
            //Do nothing at this time. In future look into the string
            this._parent = parent;
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
        /// <param name="addInType">Type of the Host View of the AddIn</param>
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

            _log.LogStatus("Verifying initial AddIn values");

            if (!VerifyClickValue(0))
            {
                return TestResult.Fail;
            }
            
            _log.LogStatus("Attempting to move mouse over button and click");
            MoveMouseAndClick();
            _log.LogEvidence("Mouse move and click complete");

            WaitForPriority(DispatcherPriority.Background);

            if (!VerifyClickValue(1))
            {
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        private bool VerifyClickValue(int expected)
        {
            if (expected == _hostView.Clicks)
            {
                _log.LogStatus("Click value was expected value of " + expected);
                return true;
            }
            else
            {
                _log.LogEvidence("Expected click value was " + expected + " actual " + _hostView.Clicks.ToString());
                return false;
            }
        }

        private void MoveMouseAndClick()
        {
            _log.LogStatus("Determining point to move to");

            FrameworkElement element = _addInHost.AddInHostElement;
            if (element != null)
            {
                //Get a point that is 50,50 from the left of the FrameworkElement that wraps the AddIn
                Rectangle rect = ImageUtility.GetScreenBoundingRectangle(element);

                _log.LogStatus(rect.ToString());

                System.Windows.Point point = new System.Windows.Point(rect.X + 50, rect.Y + 50);
                _log.LogStatus("Moving mouse and clicking on point x=" + point.X.ToString() + " y=" + point.Y.ToString());
                Input.Input.MoveToAndClick(point);
                _log.LogStatus("Move and click complete");

            }
            else
            {
                _log.LogEvidence("AddIn Host is null");
            }
        }


        /// This is from Avalon Test. Should move to a base class or use helper function
        /// <summary>
        /// Waits for a specific Dispatcher Priority to occur
        /// </summary>
        /// <param name="priority">Dispatcher Priority to wait for</param>
        /// <returns>true if sucessfull otherwise false when a timeout occurs</returns>
        private void WaitForPriority(DispatcherPriority priority)
        {
            DispatcherHelper.DoEvents(0, priority);
        }

        #endregion

    }
}
