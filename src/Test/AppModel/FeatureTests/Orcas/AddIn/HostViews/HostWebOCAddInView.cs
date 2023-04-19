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
    public abstract class HostWebOCView : HostViewBase
    {
        /// <summary>
        /// The Uri that the WebBrowser in the addin is navigated to as a string
        /// </summary>
        public abstract string Uri { get; set;}

    }
}
