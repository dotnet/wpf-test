// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]
[assembly: XmlnsCompatibleWith("http://xmlns1", "http://xmlns1")]

namespace Microsoft.Test.Xaml.Schema
{
    [SchemaTest]
    class XmlnsCompatCycleSelf : GetXamlNamespacesTest
    {
        public XmlnsCompatCycleSelf() : base(typeof(FooType))
        {
            StringResourceIdName = "XmlnsCompatCycle";
        }

        protected override void SetExpectedNamespacesOverride()
        {
            //Do nothing
        }
    }
}

