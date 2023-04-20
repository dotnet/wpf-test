// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.TemplateConverter
    /// </summary>
    public class TemplateConverterTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type with template converter test
        /// </summary>
        public void TypeWithTemplateConverterTest()
        {
            TemplateConverterTest(typeof(TypeWithTemplateConverter), "ConverterType");
        }

        /// <summary>
        /// Type with no template converter test
        /// </summary>
        public void TypeWithNoTemplateConverterTest()
        {
            TemplateConverterTest(typeof(TypeWithNoAttributes), null);
        }
    }
}
