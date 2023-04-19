// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;

namespace Microsoft.Test.AddIn
{
    /// <summary>
    /// Class to wrap the Generic List so Serialization/Deserialization will work correctly
    /// </summary>
    [Serializable]
    public class AddInHostList : List<AddInHost> { }


    /// <summary>
    /// Class to hold all the Hosts for a given AppDomain
    /// </summary>
    [Serializable]
    public class AddInHostContainer
    {

        #region Private Members

        private AddInHostList _hostList;
        private Panel _rootPanel;
        private bool _hostsAreParented;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// Creates instance of each host based on the containerParameters
        /// </summary>
        /// <param name="containerParams"></param>
        public AddInHostContainer()
        {
            _hostList = new AddInHostList();
            _hostsAreParented = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes all the Hosts
        /// </summary>
        public void InitializeHosts()
        {
            for (int i = 0; i < _hostList.Count; i++)
            {
                _hostList[i].Parent = RootPanel;
                _hostList[i].Initialize();
                _hostsAreParented = true;
            }
        }

        /// <summary>
        /// Activates all the AddIns
        /// </summary>
        public void ActivateAddIns()
        {
            for (int i = 0; i < _hostList.Count; i++)
            {
                _hostList[i].ActivateAddIn();
            }
        }

        /// <summary>
        /// Initializes all the Verifiers
        /// </summary>
        public void InitializeVerifiers()
        {
            for (int i = 0; i < _hostList.Count; i++)
            {
                _hostList[i].InitializeVerifier();
            }
        }

        /// <summary>
        /// Initializes all the AddIns
        /// </summary>
        public void InitializeAddIns()
        {
            for (int i = 0; i < _hostList.Count; i++)
            {
                _hostList[i].InitializeAddIn();
            }
        }

        /// <summary>
        /// Initializes all the AddIns
        /// </summary>
        public void ShutdownAddIns()
        {
            for (int i = 0; i < _hostList.Count; i++)
            {
                _hostList[i].ShutDownAddIn();
            }
        }

        /// <summary>
        /// Verifies all the AddIns
        /// </summary>
        /// <returns>Fail if any Verifier failed
        /// Unkown if any Verifier had an Uknown result and none failed
        /// Pass if all passed
        /// Ignore if all returned ignore</returns>
        public TestResult VerifyTestAddIns()
        {
            TestResult finalResult = TestResult.Unknown;

            int[] results = new int[4] { 0, 0, 0, 0 };
            TestResult result;
            for (int i = 0; i < _hostList.Count; i++)
            {
                result = _hostList[i].VerifyAddIn();
                results[(int)result]++;
            }

            if (results[(int)TestResult.Ignore] > 0)
            {
                finalResult = TestResult.Ignore;
            }
            if (results[(int)TestResult.Pass] > 0)
            {
                finalResult = TestResult.Pass;
            }
            if (results[(int)TestResult.Unknown] > 0)
            {
                finalResult = TestResult.Unknown;
            }
            if (results[(int)TestResult.Fail] > 0)
            {
                finalResult = TestResult.Fail;
            }

            return finalResult;
        }

        #endregion

        #region Public Properties

        public AddInHostList Hosts
        {
            get { return _hostList; }
            set { _hostList = value; }
        }

        public Panel RootPanel
        {
            get { return _rootPanel; }
            set 
            {
                if (_hostsAreParented)
                {
                    throw new ArgumentException("RootPanel is already set on the Hosts");
                }
                _rootPanel = value; 
            }
        }

        #endregion

    }
}
