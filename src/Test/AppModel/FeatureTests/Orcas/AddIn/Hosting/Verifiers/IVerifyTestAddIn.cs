// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using Microsoft.Test.Logging;
using System.Collections.Generic;
using System.Windows.Controls;

namespace Microsoft.Test.AddIn
{
    //Verifies the TestAddIn
    public interface IVerifyAddIn
    {

        #region Methods

        /// <summary>
        /// Prepares Verifier to verify the AddIn
        /// </summary>
        /// <param name="hostParameters">Copy of the AddIn parameters that were passed to the AddIn</param>
        /// <param name="parent">AddIn Host parent panel</param>
        void Initialize(string addInParameters, Panel parent);

        /// <summary>
        /// Verifies the AddIn
        /// </summary>
        /// <param name="testAddIn">Reference to the HostView instance</param>
        /// <returns>Pass if the AddIn worked as expected
        /// Fail if it did not respond correctly
        /// Unknown if the Verifier can not verify the AddIn</returns>
        TestResult VerifyTestAddIn(object hostView);

        /// <summary>
        /// Indicates if the Verifier can verify a given AddIn
        /// </summary>
        /// <param name="addInType">Type of the Host View of the AddIn</param>
        /// <returns>true if the Verifier can verify the AddIn, false if not</returns>
        bool CanVerify(Type hostViewType);

        #endregion

        #region Properties

        Panel AddInHostParent { get; set;}

        AddInHost AddInHost { get; set;}

        #endregion


    }
}
