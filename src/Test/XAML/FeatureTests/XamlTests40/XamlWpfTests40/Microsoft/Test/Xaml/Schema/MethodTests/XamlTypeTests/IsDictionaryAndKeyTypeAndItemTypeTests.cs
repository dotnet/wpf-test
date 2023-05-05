// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsDictionary, XamlType.KeyType and XamlType.ItemType
    /// </summary>
    public class IsDictionaryAndKeyTypeAndItemTypeTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type implementing IEnumerable and with 2-parameter add method
        /// </summary>
        public void TypeImplementingIEnumerableAndWithTwoParameterAddMethodTest()
        {
            IsDictionaryAndKeyTypeAndItemTypeTest(typeof(TypeImplementingIEnumerableAndWithTwoParameterAddMethod), true, "Int32", "String");
        }

        /// <summary>
        /// Type implementing IEnumerable and IDictionary Of (int and string)
        /// </summary>
        public void TypeImplementingIEnumerableAndIDictionaryOfIntStringTest()
        {
            IsDictionaryAndKeyTypeAndItemTypeTest(typeof(TypeImplementingIEnumerableAndIDictionaryOfIntString), true, "Int32", "String");
        }

        /// <summary>
        /// Type with GetEnumerator and 2-parameter add method
        /// </summary>
        public void TypeWithGetEnumeratorAndTwoParameterAddMethodTest()
        {
            IsDictionaryAndKeyTypeAndItemTypeTest(typeof(TypeWithGetEnumeratorAndTwoParameterAddMethod), true, "Int32", "String");
        }

        /// <summary>
        /// Type with GetEnumerator and Add (object, object) method
        /// </summary>
        public void TypeWithGetEnumeratorAndAddObjectObjectMethodTest()
        {
            IsDictionaryAndKeyTypeAndItemTypeTest(typeof(TypeWithGetEnumeratorAndAddObjectObjectMethod), true, "Object", "Object");
        }

        /// <summary>
        /// Type with GetEnumerator and implementing IDictionary
        /// </summary>
        public void TypeWithGetEnumeratorAndImplementingIDictionaryTest()
        {
            IsDictionaryAndKeyTypeAndItemTypeTest(typeof(TypeWithGetEnumeratorAndImplementingIDictionary), true, "Object", "Object");
        }

        /// <summary>
        /// Type with GetEnumerator and implementing IDictionary Of (int, string)
        /// </summary>
        public void TypeWithGetEnumeratorAndImplementingIDictionaryOfIntStringTest()
        {
            IsDictionaryAndKeyTypeAndItemTypeTest(typeof(TypeWithGetEnumeratorAndImplementingIDictionaryOfIntString), true, "Int32", "String");
        }

        /// <summary>
        /// Type with GetEnumerator, two 2-parameter add methods and Add (object, object) method
        /// </summary>
        public void TypeWithGetEnumeratorAndTwoTwoParameterAddMethodsAndAddObjectObjectMethodTest()
        {
            IsDictionaryAndKeyTypeAndItemTypeTest(typeof(TypeWithGetEnumeratorAndTwoTwoParameterAddMethodsAndAddObjectObjectMethod), true, "Object", "Object");
        }

        /// <summary>
        /// Type With Ambiguous Dictionary Interface
        /// </summary>
        public void TypeWithAmbiguousDictionaryInterfaceMethodTest()
        {
            IsDictionaryAndItemTypeExceptionTest(typeof(TypeWithAmbiguousDictionaryInterface), true);
        }
    }
}
