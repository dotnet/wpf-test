// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns2", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns3", "FooNamespace")]
[assembly: XmlnsCompatibleWith("http://xmlns1", "http://xmlns2")]
[assembly: XmlnsCompatibleWith("http://xmlns2", "http://xmlns3")]

namespace Microsoft.Test.Xaml.Schema
{
    /*
     * Rule1: [XmlCompat] If one namespace subsumes the other, favor the subsumer
     * Rule2: [XmlCompat] Favor namespaces that aren't subsumed over ones that are
     */
    [SchemaTest]
    class Rule2 : GetXamlNamespacesTest
    {
        public Rule2() : base(typeof(FooType)) { }

        protected override void SetExpectedNamespacesOverride()
        {
            ExpectedNamespaces.Add("http://xmlns3");
            ExpectedNamespaces.Add("http://xmlns2");
            ExpectedNamespaces.Add("http://xmlns1");
        }
    }
}

