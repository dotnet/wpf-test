// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Xaml;
using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.GetMember
    /// </summary>
    public class GetMemberTests : XamlTypeTestBase
    {
        /// <summary>
        /// Public property test
        /// </summary>
        public void PublicPropertyTest()
        {
            GetMemberTest(typeof(TypeWithPropertiesAndEvents), "PublicStringProperty", typeof(string));
        }
    }
}
