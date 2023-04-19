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
    public class DragDropAddInVerifier : IVerifyAddIn
    {

        #region Private Members

        private TestLog _log;
        private HostDragDropContractToViewAdapter _hostView;
        private Panel _parent;
        private AddInHost _addInHost;
        private TextBox _tb1;
        private TextBox _tb2;

        #endregion

        #region Constructor

        public DragDropAddInVerifier()
        {
            _log = TestLog.Current;
            _hostView = null;
            _tb1 = null;
            _tb2 = null;
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

            _log.LogStatus("Adding two TextBoxes to the root panel");
            
            _tb1 = new TextBox();
            _tb1.Name = "Host_TextBox1";
            _tb1.TabIndex = 1;
            _tb1.Height = 30;
            _tb1.Width = 100;
            _tb1.Text = "testing";
            this._parent.Children.Add(_tb1);

            _tb2 = new TextBox();
            _tb2.Name = "Host_TextBox2";
            _tb2.TabIndex = 2;
            _tb2.Height = 30;
            _tb2.Width = 100;
            this._parent.Children.Add(_tb2);

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
                this._hostView = (HostDragDropContractToViewAdapter)hostView;
                if (this._hostView == null)
                {
                    _log.LogEvidence("Can not cast " + hostView.ToString() + " to HostDragDropContractToViewAdapter");
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
            return hostViewType == typeof(HostDragDropContractToViewAdapter);
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

            //Click on TextBox1
            _addInHost.MoveMouseAndClick(_tb1);
            _log.LogStatus("clicked host textbox");

            _addInHost.WaitForPriority(DispatcherPriority.Background);
            System.Threading.Thread.Sleep(2000);

            //Select all text
            _tb1.Select(0, _tb1.Text.Length);
            _log.LogStatus("text selected");
            _addInHost.WaitForPriority(DispatcherPriority.Background);
            System.Threading.Thread.Sleep(2000);

            //Start drag
            UserInput.MouseLeftDown(_tb1, 6, 6);
            _log.LogStatus("mouse left down on selected text");

            //Do drag/drop
            FrameworkElement fe = _addInHost.AddInHostElement;

            UserInput.MouseLeftUp(fe);
            _log.LogStatus("mouse left up over addin textbox");
            _addInHost.WaitForPriority(DispatcherPriority.Background);

            //Verify drop worked
            if (_hostView.GetTextBoxText() == "testing")
            {
                _log.LogEvidence("Drag/drop was successful");
            }
            else
            {
                _log.LogEvidence("Drag/drop failed");
                return TestResult.Fail;
            }

            _addInHost.WaitForPriority(DispatcherPriority.Background);

            return TestResult.Pass;
        }

        #endregion

    }
}
