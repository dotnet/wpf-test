// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsMarkupExtension
    /// </summary>
    public class IsMarkupExtensionTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type inheriting from MarkupExtension class
        /// </summary>
        public void TypeInheritingMarkupExtensionTest()
        {
            IsMarkupExtensionTest(typeof(TypeInheritingMarkupExtension), true);
        }

        /// <summary>
        /// Type not inheriting from MarkupExtension class
        /// </summary>
        public void TypeNotInheritingMarkupExtensionTest()
        {
            IsMarkupExtensionTest(typeof(TypeNotInheritingOrImplementingAnything), false);
        }
    }
}
