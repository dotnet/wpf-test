// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.AddIn
{
    public interface IHostView
    {
        /// <summary>
        /// Prepares AddIn to be used.
        /// </summary>
        /// <param name="addInParameters">string of the AddIn Parameters</param>
        void Initialize(string addInParameters);
    }
}
