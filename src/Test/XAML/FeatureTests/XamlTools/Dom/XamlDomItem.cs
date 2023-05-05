// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public abstract class XamlDomItem : XamlDomNode
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DefaultValue(null)]
        public virtual XamlDomMember Parent
        {
            get { return _parent; }
            set
            {
                CheckSealed();
                _parent = value;
            }
        }

        private XamlDomMember _parent;
    }
}
