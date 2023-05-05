// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.Verifiers.ImplicitKeys
{
    /// <summary>
    /// ImplicitClr Test
    /// </summary>
    public static class ImplicitClr_Verify
    {
        /// <summary>
        /// Verifies the specified root element.
        /// </summary>
        /// <param name="rootElement">The root element.</param>
        /// <returns>bool value</returns>
        public static bool Verify(object rootElement)
        {
            bool result = true;
            Custom_IDictionaryHost host = rootElement as Custom_IDictionaryHost;
            if (host == null)
            {
                GlobalLog.LogStatus("The root element was not a Custom_IDictionaryHost");
                result = false;
                return result;
            }

            Custom_Clr_StringID element2 = host.Dictionary["Custom_Clr_StringIDxKey"] as Custom_Clr_StringID;
            Custom_Clr_StringID element3 = host.Dictionary["Custom_Clr_StringIDBothxKey"] as Custom_Clr_StringID;

            if (!(element2 != null && element2.ID == "Custom_Clr_StringIDxKey"))
            {
                GlobalLog.LogEvidence("element2 check failed");
                result = false;
            }

            if (!(element3 != null && element3.ID == "Custom_Clr_StringIDBoth"))
            {
                GlobalLog.LogEvidence("element3 check failed");
                result = false;
            }

            return result;
        }
    }
}
