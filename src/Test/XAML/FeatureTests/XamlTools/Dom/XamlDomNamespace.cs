// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xaml;

namespace Microsoft.Xaml.Tools.XamlDom
{
    public class XamlDomNamespace : XamlDomNode
    {
        public XamlDomNamespace()
        {
        }

        public XamlDomNamespace(NamespaceDeclaration namespaceDeclaration)
        {
            NamespaceDeclaration = namespaceDeclaration;
        }

        public NamespaceDeclaration NamespaceDeclaration
        {
            get { return _namespaceDeclaration; }
            set { CheckSealed(); _namespaceDeclaration = value; }
        }

        private NamespaceDeclaration _namespaceDeclaration;
    }
}
