// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.GetAllAttachableMembers
    /// </summary>
    public class GetAllAttachableMembersTests : XamlTypeTestBase
    {
        /// <summary>
        /// All attachable members test
        /// </summary>
        public void AllAttachableMembersTest()
        {
            string[] expected = new string[] 
            { 
                "PublicAttachableStringProperty", "PublicAttachableIntProperty", 
                "InternalAttachableStringProperty", "InternalAttachableIntProperty", 
                "PublicAttachableEvent1", "PublicAttachableEvent2", 
                "InternalAttachableEvent1", "InternalAttachableEvent2" 
            };

            GetAllAttachableMembersTest(typeof(TypeWithPropertiesAndEvents), expected);
        }
    }
}
