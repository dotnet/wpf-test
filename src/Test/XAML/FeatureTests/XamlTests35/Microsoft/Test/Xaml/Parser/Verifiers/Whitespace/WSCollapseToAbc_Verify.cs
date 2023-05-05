// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.Verifiers.Whitespace
{
    /// <summary/>
    public static class WSCollapseToAbc_Verify
    {
        /// <summary>
        /// WSCollapseToAbcVerify ensures that all string-content-capable uielements equals "abc"
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            String testTargetString = "abc";
            bool   result           = true;
            XamlTestHelper.WSCollapseVerify(rootElement, testTargetString, ref result);
            result &= WSCollapseGeneric_Verify.Verify(rootElement);
            return result;
        }
    }
}
