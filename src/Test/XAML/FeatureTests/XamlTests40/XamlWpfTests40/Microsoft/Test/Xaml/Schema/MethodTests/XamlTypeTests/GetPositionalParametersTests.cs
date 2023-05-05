// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.GetPositionalParameters
    /// </summary>
    public class GetPositionalParametersTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type with public constructor that has three parameters
        /// </summary>
        public void TypeWithPublicConstructorWithThreeParametersTest()
        {
            GetPositionalParametersTest(typeof(TypeWithPublicConstructorWithThreeParameters), 3, new string[] { "String", "Int32", "Boolean" });
        }
    }
}
