// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;

[assembly: XmlnsPrefix("", "")]
[assembly: XmlnsPrefix("http://xmlns1", "")]
[assembly: XmlnsPrefix("", "prefix1")]
[assembly: XmlnsPrefix("http://xmlns1", "prefix1")]

namespace Microsoft.Test.Xaml.Schema
{
    [SchemaTest]
    class BadXmlnsPrefix : GetPreferredPrefixTest
    {
        public BadXmlnsPrefix() : base("http://xmlns1")
        {
            StringResourceIdName = "BadXmlnsPrefix";
        }
    }
}

