// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsAssignableFrom
    /// </summary>
    public class IsAssignableFromTests : XamlTypeTestBase
    {
        /// <summary>
        /// Object from any type
        /// </summary>
        public void ObjectFromAnyTypeTest()
        {
            IsAssignableFromTest(typeof(object), typeof(AnyType), true);
        }

        /// <summary>
        /// IList from List
        /// </summary>
        public void IListFromListTest()
        {
            IsAssignableFromTest(typeof(IList<string>), typeof(List<string>), true);
        }

        /// <summary>
        /// Any type from any unrelated type
        /// </summary>
        public void AnyTypeFromTypeUnrelatedToAnyTypeTest()
        {
            IsAssignableFromTest(typeof(AnyType), typeof(TypeUnrelatedToAnyType), false);
        }
    }
}
