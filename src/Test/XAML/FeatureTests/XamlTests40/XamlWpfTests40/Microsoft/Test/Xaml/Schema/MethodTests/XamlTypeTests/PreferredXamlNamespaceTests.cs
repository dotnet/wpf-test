// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.NamespaceWithXmlnsDefinition;
using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.PreferredXamlNamespace
    /// </summary>
    public class PreferredXamlNamespaceTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type with preferred namespace
        /// </summary>
        public void TypeWithPreferredNamespaceTest()
        {
            PreferredXamlNamespaceTest(typeof(TypeWithPreferredNamespace), "http://Microsoft/Test/Xaml/CustomTypes/NamespaceWithXmlnsDefinition");
        }
    }
}
