// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using System.Xaml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types.Schema;

namespace Microsoft.Test.Xaml.Schema.MethodTests.XamlTypeTests
{
    /// <summary>
    /// Tests for GetPreferredPrefix
    /// </summary>
    public class GetPreferredPrefixTests : XamlTypeTestBase
    {
        /// <summary>
        /// XscWithEmptyAssembly test - Regression coverage for 562062
        /// </summary>
        public void XscWithEmptyAssembly()
        {
            XamlSchemaContext sc = new XamlSchemaContext(new Assembly[0]);

            if (String.Equals("x", sc.GetPreferredPrefix(XamlLanguage.Xaml2006Namespace), StringComparison.InvariantCultureIgnoreCase) == false)
            {
                throw new TestValidationException("Expected Prefix [x] Actual [" + sc.GetPreferredPrefix(XamlLanguage.Xaml2006Namespace) + "]");
            }
        }
    }
}
