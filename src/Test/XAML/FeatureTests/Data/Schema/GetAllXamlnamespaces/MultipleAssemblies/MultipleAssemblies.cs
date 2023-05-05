// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Xaml;
using Microsoft.Test.Logging;
using NamespaceA1;
using NamespaceB1;

namespace Microsoft.Test.Xaml.Schema
{
    [SchemaTest]
    class MultipleAssemblies : GetAllXamlNamespacesTest
    {
        public MultipleAssemblies()
        {
            //Load AssemblyA and AssemblyB
            A1 a = new A1();
            a = null;
            B1 b = new B1();
            b = null;
        }

        protected override void SetExpectedNamespacesOverride()
        {
            ExpectedNamespaces.Add("http://xmlns1");
            ExpectedNamespaces.Add("http://xmlns2");
            ExpectedNamespaces.Add("http://xmlns3");
            ExpectedNamespaces.Add("http://xmlns4");
            ExpectedNamespaces.Add("http://XamlTestTypes");
        }
    }
}
