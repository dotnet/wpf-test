// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xaml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.TypeConverter
    /// </summary>
    public class TextSyntaxTests : XamlTypeTestBase
    {
        /// <summary>
        /// String test
        /// </summary>
        public void StringTest()
        {
            TextSyntaxTest(typeof(string), "StringConverter");
        }

        /// <summary>
        /// Object test
        /// </summary>
        public void ObjectTest()
        {
            XamlType xamlType = SchemaContext.GetXamlType(typeof(object));
            if (xamlType.TypeConverter != null)
            {
                Validate<string>(xamlType.TypeConverter.Name, "Object");
            }
            else
            {
                GlobalLog.LogEvidence("TypeConverter is null");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Nullable of (Guid) test
        /// </summary>
        public void NullableOfGuidTest()
        {
            TextSyntaxTest(typeof(Nullable<Guid>), "GuidConverter");
        }

        /// <summary>
        /// Type with type converter test
        /// </summary>
        public void TypeWithTypeConverterTest()
        {
            TextSyntaxTest(typeof(TypeWithTypeConverter), "TypeInheritingTypeConverter");
        }
    }
}
