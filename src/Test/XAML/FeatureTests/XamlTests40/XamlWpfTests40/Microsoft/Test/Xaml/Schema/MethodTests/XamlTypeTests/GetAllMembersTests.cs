// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.GetAllMembers
    /// </summary>
    public class GetAllMembersTests : XamlTypeTestBase
    {
        /// <summary>
        /// All members test
        /// </summary>
        public void AllMembersTest()
        {
            string[] expected = new string[] 
            {
                "PublicStringProperty", "PublicIntProperty", 
                "InternalStringProperty", "InternalIntProperty", 
                "PublicEvent1", "PublicEvent2", 
                "InternalEvent1", "InternalEvent2" 
            };

            GetAllMembersTest(typeof(TypeWithPropertiesAndEvents), expected);
        }
    }
}
