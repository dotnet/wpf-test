// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.AddIn.Pipeline;
using System.AddIn.Contract;

namespace Microsoft.Test.AddIn
{
    /// <summary>
    /// Allows testing UI that will respond to focus events.
    /// </summary>
    [AddInContract]
    public interface IWinFormsHostTestAddIn : INativeHandleContract
    {
        /// <summary>
        /// Returns the text in TextBox
        /// </summary>
        /// <returns>text in TextBox</returns>
        string GetTextBoxText();

        /// <summary>
        /// Prepares AddIn to be used. Indicates if the AddIn should host other AddIns
        /// </summary>
        /// <param name="addInParameters">AddIn Parameters</param>
        void Initialize(string addInParameters);
    }
}
