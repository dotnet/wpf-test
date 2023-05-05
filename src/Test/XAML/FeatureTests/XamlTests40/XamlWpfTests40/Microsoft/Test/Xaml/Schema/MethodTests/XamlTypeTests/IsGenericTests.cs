// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsGeneric
    /// </summary>
    public class IsGenericTests : XamlTypeTestBase
    {
        /// <summary>
        /// Open generic type test
        /// </summary>
        public void OpenGenericTypeTest()
        {
            IsGenericTest(typeof(GenericTypeWithTwoTypeArguments<,>), true);
        }

        /// <summary>
        /// Closed generic type test
        /// </summary>
        public void ClosedGenericTypeTest()
        {
            IsGenericTest(typeof(GenericTypeWithTwoTypeArguments<int, string>), true);
        }

        /// <summary>
        /// Partially closed generic type test
        /// </summary>
        public void PartiallyClosedGenericTypeTest()
        {
            IsGenericTest(typeof(DerivedTypeThatPartiallyClosesGenericTypeWithTwoTypeArguments<>).BaseType, true);
        }
    }
}
