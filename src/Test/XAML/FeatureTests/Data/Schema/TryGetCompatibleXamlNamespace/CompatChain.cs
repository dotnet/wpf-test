// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Markup;

[assembly: XmlnsDefinition("http://a", "Microsoft.Test.Xaml.Schema")]
[assembly: XmlnsDefinition("http://b", "Microsoft.Test.Xaml.Schema")]
[assembly: XmlnsDefinition("http://c", "Microsoft.Test.Xaml.Schema")]
[assembly: XmlnsCompatibleWith("http://a", "http://b")]
[assembly: XmlnsCompatibleWith("http://b", "http://c")]

namespace Microsoft.Test.Xaml.Schema
{
    /// <summary>
    /// Xmlns compat chain
    /// </summary>
    [SchemaTest]
    class CompatChain : TryGetCompatibleXamlNamespaceTest
    {
        public override void Run()
        {
            CallAndVerify("http://a", true, "http://b");
            CallAndVerify("http://b", true, "http://c");
            CallAndVerify("http://c", true, "http://c");
        }
    }
}
