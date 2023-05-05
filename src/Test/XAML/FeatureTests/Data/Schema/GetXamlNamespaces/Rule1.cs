// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns2", "FooNamespace")]
[assembly: XmlnsCompatibleWith("http://xmlns1", "http://xmlns2")]

namespace Microsoft.Test.Xaml.Schema
{
    /*
     * Rule1: [XmlCompat] If one namespace subsumes the other, favor the subsumer
     */
    [SchemaTest]
    class Rule1 : GetXamlNamespacesTest
    {
        public Rule1() : base(typeof(FooType)) {}

        protected override void SetExpectedNamespacesOverride()
        {
            ExpectedNamespaces.Add("http://xmlns2");
            ExpectedNamespaces.Add("http://xmlns1");
        }
    }
}

