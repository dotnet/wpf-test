// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;
using FooNamespace;

[assembly: XmlnsDefinition("http://xmlns1", "FooNamespace")]
[assembly: XmlnsDefinition("http://xmlns2", "FooNamespace")]
[assembly: XmlnsCompatibleWith("", "")]
[assembly: XmlnsCompatibleWith("http://xmlns1", "")]
[assembly: XmlnsCompatibleWith("", "http://xmlns2")]
[assembly: XmlnsCompatibleWith("http://xmlns1", "http://xmlns2")]

namespace Microsoft.Test.Xaml.Schema
{
    [SchemaTest]
    class BadXmlnsCompat : GetXamlNamespacesTest
    {
        public BadXmlnsCompat() : base(typeof(FooType))
        {
            StringResourceIdName = "BadXmlnsCompat";
        }

        protected override void SetExpectedNamespacesOverride()
        {
            //Do nothing
        }
    }
}

