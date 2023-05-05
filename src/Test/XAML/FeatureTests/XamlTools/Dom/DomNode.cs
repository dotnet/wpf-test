// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public abstract class DomNode
    {
        public int LineNumber { get; set; }
        public int LinePosition { get; set; }
        public abstract bool IsSealed { get; }

        public abstract void Seal();
    }
}
