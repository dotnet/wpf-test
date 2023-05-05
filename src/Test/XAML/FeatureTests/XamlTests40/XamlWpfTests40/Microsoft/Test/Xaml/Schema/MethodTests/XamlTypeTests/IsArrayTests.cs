// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsArray
    /// </summary>
    public class IsArrayTests : XamlTypeTestBase
    {
        /// <summary>
        /// Array type test
        /// </summary>
        public void ArrayTypeTest()
        {
            IsArrayTest(typeof(string[]), true);
        }

        /// <summary>
        /// Non-array type test
        /// </summary>
        public void NonArrayTypeTest()
        {
            IsArrayTest(typeof(string), false);
        }
    }
}
