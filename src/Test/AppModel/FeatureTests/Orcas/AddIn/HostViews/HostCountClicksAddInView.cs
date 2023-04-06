// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.AddIn.Pipeline;

namespace Microsoft.Test.AddIn
{
    [AddInBase]
    public abstract class HostCountClicksAddInView : HostViewBase
    {
        /// <summary>
        /// Number of clicks the AddIn has counted
        /// </summary>
        public abstract int Clicks { get; set;}

        /// <summary>
        /// Try returning the ContractToViewAdapter from null (should return null)
        /// </summary>
        public abstract object AdapterNull { get; }

        /// <summary>
        /// Try returning the ContractToViewAdapter from a child (should throw InvalidOperationException)
        /// </summary>
        public abstract object AdapterChild { get; }

    }
}
