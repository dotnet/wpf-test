// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsNamescope
    /// </summary>
    public class IsNamescopeTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type implementing INamescope interface
        /// </summary>
        public void TypeImplementingINamescopeTest()
        {
            IsNamescopeTest(typeof(TypeImplementingINamescope), true);
        }

        /// <summary>
        /// Type not implementing INamescope interface
        /// </summary>
        public void TypeNotImplementingINamescopeTest()
        {
            IsNamescopeTest(typeof(TypeNotInheritingOrImplementingAnything), false);
        }
    }
}
