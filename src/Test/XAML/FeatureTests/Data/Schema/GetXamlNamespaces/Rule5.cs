// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns2", "FooNamespace")]
[assembly: XmlnsPrefix("http://xmlns1", "looongpfx")]
[assembly: XmlnsPrefix("http://xmlns2", "shortpfx")]

namespace Microsoft.Test.Xaml.Schema
{
    /*
     * The rules for namespace ordering are:
     * 1.[XmlCompat] If one namespace subsumes the other, favor the subsumer
     * 2.[XmlCompat] Favor namespaces that aren't subsumed over ones that are
     * 3.[XmlCompat] Favor namespaces that subsume a greater number of other namespaces
     * 4.[XmlNsPrefix] Favor namespaces with prefixes over namespaces without prefixes
     * 5.[XmlNsPrefix] Favor namespaces with shorter prefixes over namespaces with longer ones
     */
    [SchemaTest]
    class Rule5 : GetXamlNamespacesTest
    {
        public Rule5() : base(typeof(FooType)) { }

        protected override void SetExpectedNamespacesOverride()
        {
            ExpectedNamespaces.Add("http://xmlns2");
            ExpectedNamespaces.Add("http://xmlns1");
        }
    }
}

