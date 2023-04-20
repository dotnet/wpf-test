// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.AllowedContentTypes
    /// </summary>
    public class AllowedContentTypesTests : XamlTypeTestBase
    {
        /// <summary>
        /// Collection type with two ContentWrapperAttribute
        /// </summary>
        public void CollectionTypeWithTwoContentWrappersTest()
        {
            AllowedContentTypesTest(typeof(CollectionTypeWithTwoContentWrappers), new string[] { "Object", "String", "Int32" });
        }
    }
}
