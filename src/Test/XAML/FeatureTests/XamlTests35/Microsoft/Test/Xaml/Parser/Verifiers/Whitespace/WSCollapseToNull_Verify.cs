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
    public static class WSCollapseToNull_Verify
    {
        /// <summary>
        /// WSCollapseToEmptyVerify ensures that all string-content-capable uielements equals Null
        /// </summary>
        public static bool Verify(UIElement rootElement)
        {
            String testTargetString = null;
            bool   result           = true;
            XamlTestHelper.WSCollapseVerify(rootElement, testTargetString, ref result);
            result &= WSCollapseGeneric_Verify.Verify(rootElement);
            return result;
        }
    }
}
