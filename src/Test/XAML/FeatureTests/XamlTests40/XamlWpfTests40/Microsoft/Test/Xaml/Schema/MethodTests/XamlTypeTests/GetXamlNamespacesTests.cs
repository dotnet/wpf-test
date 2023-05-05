// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.NamespaceWithXmlnsDefinition;
using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.GetXamlNamespaces
    /// </summary>
    public class GetXamlNamespacesTests : XamlTypeTestBase
    {
        /// <summary>
        /// Xaml namespaces test
        /// </summary>
        public void XamlNamespacesTest()
        {
            string[] expected = new string[] 
            {
                "http://Microsoft/Test/Xaml/CustomTypes/NamespaceWithXmlnsDefinition",
                "clr-namespace:Microsoft.Test.Xaml.Types.NamespaceWithXmlnsDefinition;assembly=XamlClrTypes"
            };

            GetXamlNamespacesTest(typeof(TypeWithPreferredNamespace), expected);
        }
    }
}
