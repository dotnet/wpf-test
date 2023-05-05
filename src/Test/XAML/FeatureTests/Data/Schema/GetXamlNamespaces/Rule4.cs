// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns2", "FooNamespace")]
[assembly: XmlnsPrefix("http://xmlns2", "pfx")]

namespace Microsoft.Test.Xaml.Schema
{
    /*
     * Rule1: [XmlCompat] If one namespace subsumes the other, favor the subsumer
     * Rule2: [XmlCompat] Favor namespaces that aren't subsumed over ones that are
     * Rule3: [XmlCompat] Favor namespaces that subsume a greater number of other namespaces
     * Rule4: [XmlNsPrefix] Favor namespaces with prefixes over namespaces without prefixes
     */
    [SchemaTest]
    class Rule4 : GetXamlNamespacesTest
    {
        public Rule4() : base(typeof(FooType)) { }

        protected override void SetExpectedNamespacesOverride()
        {
            ExpectedNamespaces.Add("http://xmlns2");
            ExpectedNamespaces.Add("http://xmlns1");
        }
    }
}

