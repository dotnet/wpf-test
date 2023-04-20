// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.ContentProperty
    /// </summary>
    public class ContentPropertyTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type with one ContentPropertyAttribute
        /// </summary>
        public void TypeWithOneContentPropertyTest()
        {
            ContentPropertyTest(typeof(TypeWithOneContentProperty), "StringProperty");
        }
    }
}
