// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.IsCollection and XamlType.ItemType
    /// </summary>
    public class IsCollectionAndItemTypeTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type implementing IEnumerable and with 1-parameter add method
        /// </summary>
        public void TypeImplementingIEnumerableAndWithOneParameterAddMethodTest()
        {
            IsCollectionAndItemTypeTest(typeof(TypeImplementingIEnumerableAndWithOneParameterAddMethod), true, "String");
        }

        /// <summary>
        /// Type implementing IEnumerable and ICollection of (string)
        /// </summary>
        public void TypeImplementingIEnumerableAndICollectionOfStringTest()
        {
            IsCollectionAndItemTypeTest(typeof(TypeImplementingIEnumerableAndICollectionOfString), true, "String");
        }

        /// <summary>
        /// Type with GetEnumerator and 1-parameter add method
        /// </summary>
        public void TypeWithGetEnumeratorAndOneParameterAddMethodTest()
        {
            IsCollectionAndItemTypeTest(typeof(TypeWithGetEnumeratorAndOneParameterAddMethod), true, "String");
        }

        /// <summary>
        /// Type with GetEnumerator and Add (object) method
        /// </summary>
        public void TypeWithGetEnumeratorAndAddObjectMethodTest()
        {
            IsCollectionAndItemTypeTest(typeof(TypeWithGetEnumeratorAndAddObjectMethod), true, "Object");
        }

        /// <summary>
        /// Type with GetEnumerator and implementing IList
        /// </summary>
        public void TypeWithGetEnumeratorAndImplementingIListTest()
        {
            IsCollectionAndItemTypeTest(typeof(TypeWithGetEnumeratorAndImplementingIList), true, "Object");
        }

        /// <summary>
        /// Type with GetEnumerator and implementing ICollection of (string)
        /// </summary>
        public void TypeWithGetEnumeratorAndImplementingICollectionOfStringTest()
        {
            IsCollectionAndItemTypeTest(typeof(TypeWithGetEnumeratorAndImplementingICollectionOfString), true, "String");
        }

        /// <summary>
        /// Type with GetEnumerator, two 1-parameter add methods and Add (object) method
        /// </summary>
        public void TypeWithGetEnumeratorAndTwoOneParameterAddMethodsAndAddObjectMethodTest()
        {
            IsCollectionAndItemTypeTest(typeof(TypeWithGetEnumeratorAndTwoOneParameterAddMethodsAndAddObjectMethod), true, "Object");
        }

        /// <summary>
        /// Type With AmbiguousCollectionInterface
        /// </summary>
        public void TypeWithAmbiguousCollectionInterfaceMethodTest()
        {
            IsCollectionAndItemTypeExceptionTest(typeof(TypeWithAmbiguousCollectionInterface), true);
        }

        /// <summary>
        /// Collection Type With Two ParameterAdd
        /// </summary>
        public void CollectionTypeWithTwoParameterAddMethodTest()
        {
            IsCollectionAndItemTypeTest(typeof(CollectionTypeWithTwoParameterAdd), true, "String");
        }
    }
}
