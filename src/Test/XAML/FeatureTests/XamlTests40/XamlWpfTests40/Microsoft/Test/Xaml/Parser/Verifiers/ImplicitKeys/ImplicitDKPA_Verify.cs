// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Types.Attributes;

namespace Microsoft.Test.Xaml.Parser.Verifiers.ImplicitKeys
{
    /// <summary>
    /// ImplicitDKPA Test
    /// </summary>
    public static class ImplicitDKPA_Verify
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

            DKPAString element2 = host.Dictionary["DKPAString1"] as DKPAString;
            DKPAString element3 = host.Dictionary["DKPAStringxKey"] as DKPAString;
            DKPAString element4 = host.Dictionary["DKPAStringxKey1"] as DKPAString;

            if (!(element2 != null && element2.ID == "DKPAStringName"))
            {
                GlobalLog.LogEvidence("element2 check failed");
                result = false;
            }

            if (!(element3 != null && element3.ID == "DKPAStringxKey"))
            {
                GlobalLog.LogEvidence("element3 check failed");
                result = false;
            }

            if (!(element4 != null && element4.ID == "DKPAStringBoth"))
            {
                GlobalLog.LogEvidence("element4 check failed");
                result = false;
            }

            return result;
        }
    }
}
