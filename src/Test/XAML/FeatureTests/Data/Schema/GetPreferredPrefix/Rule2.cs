// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;

[assembly: XmlnsPrefix("http://xmlns1", "prefix4")]
[assembly: XmlnsPrefix("http://xmlns1", "prefix3")]
[assembly: XmlnsPrefix("http://xmlns1", "prefix2")]
[assembly: XmlnsPrefix("http://xmlns1", "prefix1")]

namespace Microsoft.Test.Xaml.Schema
{
    /*
     * Rule2: If they’re the same length, take the lesser string by ordinal comparison.
     */
    [SchemaTest]
    class Rule2 : GetPreferredPrefixTest
    {
        public Rule2() : base("http://xmlns1")
        {
            ExpectedPrefix = "prefix1";
        }
    }
}

