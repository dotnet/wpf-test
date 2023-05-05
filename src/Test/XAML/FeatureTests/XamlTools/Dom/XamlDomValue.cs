// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.Xaml.Tools.XamlDom
{
    [DebuggerDisplay("{Value}")]
    public class XamlDomValue : XamlDomItem
    {
        public XamlDomValue()
        {
        }

        public XamlDomValue(object value)
        {
            _value = value;
        }

        [DefaultValue(null)]
        public virtual object Value
        {
            get { return _value; }
            set { CheckSealed(); _value = value; }
        }

        private object _value;
    }
}
