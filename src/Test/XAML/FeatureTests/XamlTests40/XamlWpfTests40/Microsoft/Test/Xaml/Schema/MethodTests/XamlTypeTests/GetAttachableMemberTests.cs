// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for XamlType.GetAttachableProperty
    /// </summary>
    public class GetAttachableMemberTests : XamlTypeTestBase
    {
        /// <summary>
        /// Attachable property with getter and setter
        /// </summary>
        public void AttachablePropertyWithGetterAndSetterTest()
        {
            GetAttachableMemberTest(typeof(TypeWithPropertiesAndEvents), "PublicAttachableStringProperty", typeof(string));
        }

        /// <summary>
        /// Attachable event with adder and remover
        /// </summary>
        public void AttachableEventWithAdderAndRemoverTest()
        {
            GetAttachableMemberTest(typeof(TypeWithPropertiesAndEvents), "PublicAttachableEvent1", typeof(EventHandler));
        }
    }
}
