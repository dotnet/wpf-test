// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsConstructible
    /// </summary>
    public class IsConstructibleTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type with public constructor
        /// </summary>
        public void TypeWithPublicConstructorTest()
        {
            IsConstructibleTest(typeof(TypeWithPublicConstructor), true);
        }

        /// <summary>
        /// Type with private constructor
        /// </summary>
        public void TypeWithPrivateConstructorTest()
        {
            IsConstructibleTest(typeof(TypeWithPrivateConstructor), false);
        }

        /// <summary>
        /// Abstract type test
        /// </summary>
        public void AbstractTypeTest()
        {
            IsConstructibleTest(typeof(AbstractType), false);
        }

        /// <summary>
        /// Value type test
        /// </summary>
        public void ValueTypeTest()
        {
            IsConstructibleTest(typeof(bool), true);
        }
    }
}
