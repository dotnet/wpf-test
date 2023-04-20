// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;

[assembly: XmlnsPrefix("http://xmlns1", "srt")]
[assembly: XmlnsPrefix("http://xmlns1", "long")]
[assembly: XmlnsPrefix("http://xmlns1", "longer")]
[assembly: XmlnsPrefix("http://xmlns1", "longest")]

namespace Microsoft.Test.Xaml.Schema
{
    /*
     * Rule1: Choose the shorter prefix
     */
    [SchemaTest]
    class Rule1 : GetPreferredPrefixTest
    {
        public Rule1() : base("http://xmlns1")
        {
            ExpectedPrefix = "srt";
        }
    }
}

