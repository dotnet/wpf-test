// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;

[assembly: XmlnsDefinition("http://a", "Microsoft.Test.Xaml.Schema")]
[assembly: XmlnsDefinition("http://b", "Microsoft.Test.Xaml.Schema")]
[assembly: XmlnsCompatibleWith("http://a", "http://b")]

namespace Microsoft.Test.Xaml.Schema
{
    /// <summary>
    /// Simple test
    /// </summary>
    [SchemaTest]
    class SimpleTest : TryGetCompatibleXamlNamespaceTest
    {
        public override void Run()
        {
            CallAndVerify("http://a", true, "http://b");
            CallAndVerify("http://b", true, "http://b");
        }
    }
}
