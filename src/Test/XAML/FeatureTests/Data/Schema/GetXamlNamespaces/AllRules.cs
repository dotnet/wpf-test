// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns2", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns3", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns4", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns5", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns6", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns7", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns8", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns9", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns10", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns11", "FooNamespace")]
[assembly: XmlnsCompatibleWith("http://xmlns1", "http://xmlns3")]
[assembly: XmlnsCompatibleWith("http://xmlns2", "http://xmlns3")]
[assembly: XmlnsCompatibleWith("http://xmlns3", "http://xmlns5")]
[assembly: XmlnsCompatibleWith("http://xmlns4", "http://xmlns5")]
[assembly: XmlnsCompatibleWith("http://xmlns6", "http://xmlns10")]
[assembly: XmlnsCompatibleWith("http://xmlns7", "http://xmlns10")]
[assembly: XmlnsCompatibleWith("http://xmlns8", "http://xmlns10")]
[assembly: XmlnsCompatibleWith("http://xmlns9", "http://xmlns10")]
[assembly: XmlnsPrefix("http://xmlns3", "123456789")]
[assembly: XmlnsPrefix("http://xmlns4", "12345678")]
[assembly: XmlnsPrefix("http://xmlns5", "1234567")]
[assembly: XmlnsPrefix("http://xmlns6", "123456")]
[assembly: XmlnsPrefix("http://xmlns7", "12345")]
[assembly: XmlnsPrefix("http://xmlns8", "1234")]
[assembly: XmlnsPrefix("http://xmlns9", "123")]
[assembly: XmlnsPrefix("http://xmlns10", "12")]
[assembly: XmlnsPrefix("http://xmlns11", "1")]

namespace Microsoft.Test.Xaml.Schema
{
    /* 
     * The rules for namespace ordering are:
     * Rule1: [XmlCompat] If one namespace subsumes the other, favor the subsumer
     * Rule2: [XmlCompat] Favor namespaces that aren't subsumed over ones that are
     * Rule3: [XmlCompat] Favor namespaces that subsume a greater number of other namespaces
     * Rule4: [XmlNsPrefix] Favor namespaces with prefixes over namespaces without prefixes
     * Rule5: [XmlNsPrefix] Favor namespaces with shorter prefixes over namespaces with longer ones
     * Rule6: Fall back to the lesser string by ordinal comparison
     *Namespace ordering for a given assembly is based only on the XmlCompat attributes in that particularly assembly.
     */
    [SchemaTest]
    class AllRules : GetXamlNamespacesTest
    {
        public AllRules() : base(typeof(FooType)) { }

        protected override void SetExpectedNamespacesOverride()
        {
            ExpectedNamespaces.Add("http://xmlns10");
            ExpectedNamespaces.Add("http://xmlns5");
            ExpectedNamespaces.Add("http://xmlns11");
            ExpectedNamespaces.Add("http://xmlns3");
            ExpectedNamespaces.Add("http://xmlns9");
            ExpectedNamespaces.Add("http://xmlns8");
            ExpectedNamespaces.Add("http://xmlns7");
            ExpectedNamespaces.Add("http://xmlns6");
            ExpectedNamespaces.Add("http://xmlns4");
            ExpectedNamespaces.Add("http://xmlns1");
            ExpectedNamespaces.Add("http://xmlns2");
        }
    }
}

