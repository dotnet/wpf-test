// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;

namespace Microsoft.Test.Xaml.Schema
{
    [SchemaTest]
    class PPrefix : GetPreferredPrefixTest
    {
        public PPrefix() : base("http://undefined")
        {
            ExpectedPrefix = "p";
        }
    }
}

