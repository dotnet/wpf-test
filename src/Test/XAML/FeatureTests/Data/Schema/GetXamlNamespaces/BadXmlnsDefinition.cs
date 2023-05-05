// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("", "")]
[assembly: XmlnsDefinition("http://xmlns1", "")]
[assembly: XmlnsDefinition("", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]

namespace Microsoft.Test.Xaml.Schema
{
    [SchemaTest]
    class EmptyXmlnsDefinition: GetXamlNamespacesTest
    {
        public EmptyXmlnsDefinition() : base(typeof(FooType))
        {
            StringResourceIdName = "BadXmlnsDefinition";
        }

        protected override void SetExpectedNamespacesOverride()
        {
            //Do nothing
        }
    }
}

