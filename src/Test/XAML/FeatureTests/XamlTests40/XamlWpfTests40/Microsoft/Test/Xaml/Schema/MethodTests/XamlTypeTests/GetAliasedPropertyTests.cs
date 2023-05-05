// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xaml;
using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.GetAliasedProperty
    /// </summary>
    public class GetAliasedPropertyTests : XamlTypeTestBase
    {
        /// <summary>
        /// Type with DictinaryKeyPropertyAttribute
        /// </summary>
        public void TypeWithDictionaryKeyPropertyTest()
        {
            GetAliasedPropertyTest(typeof(TypeWithDictionaryKeyProperty), XamlLanguage.Key, "KeyProperty", typeof(string));
        }

        /// <summary>
        /// Type with RuntimeNamePropertyAttribute
        /// </summary>
        public void TypeWithRuntimeNamePropertyTest()
        {
            GetAliasedPropertyTest(typeof(TypeWithRuntimeNameProperty), XamlLanguage.Name, "NameProperty", typeof(string));
        }

        /// <summary>
        /// Type with UidPropertyAttribute
        /// </summary>
        public void TypeWithUidPropertyTest()
        {
            GetAliasedPropertyTest(typeof(TypeWithUidProperty), XamlLanguage.Uid, "UidProperty", typeof(string));
        }

        /// <summary>
        /// Type with XmlLangPropertyAttribute
        /// </summary>
        public void TypeWithXmlLangPropertyTest()
        {
            GetAliasedPropertyTest(typeof(TypeWithXmlLangProperty), XamlLanguage.Lang, "XmlLangProperty", typeof(string));
        }
    }
}
