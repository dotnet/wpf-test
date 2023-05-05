// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsNullable
    /// </summary>
    public class IsNullableTests : XamlTypeTestBase
    {
        /// <summary>
        /// Reference type test
        /// </summary>
        public void ReferenceTypeTest()
        {
            IsNullableTest(typeof(object), true);
        }

        /// <summary>
        /// Value type test
        /// </summary>
        public void ValueTypeTest()
        {
            IsNullableTest(typeof(bool), false);
        }

        /// <summary>
        /// Nullable Of T test
        /// </summary>
        public void NullableOfTTest()
        {
            IsNullableTest(typeof(bool?), true);
        }
    }
}
