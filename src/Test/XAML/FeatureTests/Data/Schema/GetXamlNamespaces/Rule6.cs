// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("http://xmlns3", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns2", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]

namespace Microsoft.Test.Xaml.Schema
{
    /*
     * Rule1: [XmlCompat] If one namespace subsumes the other, favor the subsumer
     * Rule2: [XmlCompat] Favor namespaces that aren't subsumed over ones that are
     * Rule3: [XmlCompat] Favor namespaces that subsume a greater number of other namespaces
     * Rule4: [XmlNsPrefix] Favor namespaces with prefixes over namespaces without prefixes
     * Rule5: [XmlNsPrefix] Favor namespaces with shorter prefixes over namespaces with longer ones
     * Rule6: Fall back to the lesser string by ordinal comparison
     */
    [SchemaTest]
    class Rule6 : GetXamlNamespacesTest
    {
        public Rule6() : base(typeof(FooType)) { }

        protected override void SetExpectedNamespacesOverride()
        {
            ExpectedNamespaces.Add("http://xmlns1");
            ExpectedNamespaces.Add("http://xmlns2");
            ExpectedNamespaces.Add("http://xmlns3");
        }
    }
}

