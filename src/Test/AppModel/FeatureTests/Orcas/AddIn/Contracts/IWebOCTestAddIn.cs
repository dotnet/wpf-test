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
    /// Allows testing an addin with a WebBrowser control in it
    /// </summary>
    [AddInContract]
    public interface IWebOCTestAddIn : INativeHandleContract
    {
        /// <summary>
        /// Returns the current Uri for the WebBrowser as a string
        /// </summary>
        /// <returns>Uri as string</returns>
        string GetUri();

        /// <summary>
        /// Sets the current Uri for the WebBrowser
        /// </summary>
        void SetUri(string Uri);

        //consider adding a way to set the Uri later on

        #region Methods

        /// <summary>
        /// Prepares AddIn to be used.
        /// </summary>
        /// <param name="addInParameters">Dictionary of the AddIn Parameters</param>
        void Initialize(string addInParameters);

        #endregion


    }
}
