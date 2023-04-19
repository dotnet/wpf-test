// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.AddIn.Pipeline;
using System.AddIn.Contract;

namespace Microsoft.Test.AddIn
{
    /// <summary>
    /// Tests passing an int as a property, a method return, and as a method parameter
    /// The AddIn should have some UI that will count clicks
    /// </summary>
    [AddInContract]
    public interface ICountClicksTestAddIn : INativeHandleContract
    {
        /// <summary>
        /// Returns the number of clicks the AddIn counted
        /// </summary>
        /// <returns>Number of clicks the AddIn counted</returns>
        int GetClicks();

        /// <summary>
        /// Sets the number of clicks the AddIn has counted
        /// </summary>
        /// <param name="clickCount">Number of clicks the AddIn has counted</param>
        void SetClicks(int clickCount);

        /// <summary>
        /// Try returning the ContractToViewAdapter from null (should return null)
        /// </summary>
        object GetAdapterNull();

        /// <summary>
        /// Try returning the ContractToViewAdapter from a child (should throw InvalidOperationException)
        /// </summary>
        object GetAdapterChild();

        #region Methods

        /// <summary>
        /// Prepares AddIn to be used.
        /// </summary>
        /// <param name="addInParameters">Dictionary of the AddIn Parameters</param>
        void Initialize(string addInParameters);

        #endregion


    }
}
