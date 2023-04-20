// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns2", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns3", "FooNamespace")]
[assembly: XmlnsPrefix("http://xmlns1", "looongpfx")]
[assembly: XmlnsPrefix("http://xmlns3", "shortpfx")]

namespace Microsoft.Test.Xaml.Schema
{
    [SchemaTest]
    class AllPrefixRules : GetXamlNamespacesTest
    {
        public AllPrefixRules() : base(typeof(FooType)) { }

        protected override void SetExpectedNamespacesOverride()
        {
            ExpectedNamespaces.Add("http://xmlns3");
            ExpectedNamespaces.Add("http://xmlns1");
            ExpectedNamespaces.Add("http://xmlns2");
            
        }
    }
}

