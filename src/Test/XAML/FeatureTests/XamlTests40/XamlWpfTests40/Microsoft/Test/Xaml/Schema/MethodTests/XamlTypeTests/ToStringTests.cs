// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.ToString
    /// </summary>
    public class ToStringTests : XamlTypeTestBase
    {
        /// <summary>
        /// Non-generic type test
        /// </summary>
        public void NonGenericTypeTest()
        {
            ToStringTest(typeof(NonGenericType), "Microsoft.Test.Xaml.Types.Schema.NonGenericType");
        }

        /// <summary>
        /// Generic type test
        /// </summary>
        public void GenericTypeTest()
        {
            ToStringTest(typeof(GenericType1<string>), "Microsoft.Test.Xaml.Types.Schema.GenericType1(System.String)");
        }
    }
}
