// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsXData
    /// </summary>
    public class IsXDataTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type implementing IXmlSerializable interface
        /// </summary>
        public void TypeImplementingIXmlSerializableTest()
        {
            IsXDataTest(typeof(TypeImplementingIXmlSerializable), true);
        }

        /// <summary>
        /// Type not implementing IXmlSerializable interface
        /// </summary>
        public void TypeNotImplementingIXmlSerializableTest()
        {
            IsXDataTest(typeof(TypeNotInheritingOrImplementingAnything), false);
        }
    }
}
