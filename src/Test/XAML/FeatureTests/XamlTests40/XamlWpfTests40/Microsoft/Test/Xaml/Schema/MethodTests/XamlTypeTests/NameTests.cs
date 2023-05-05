// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.Name
    /// </summary>
    public class NameTests : XamlTypeTestBase
    {
        /// <summary>
        /// Non-generic type test
        /// </summary>
        public void NonGenericTypeTest()
        {
            NameTest(typeof(NonGenericType), "NonGenericType");
        }

        /// <summary>
        /// Generic type test
        /// </summary>
        public void GenericTypeTest()
        {
            NameTest(typeof(GenericType1<int>), "GenericType1");
        }
    }
}
